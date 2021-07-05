﻿using StampImages.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace StampImages.Core
{

    /// <summary>
    /// 職印画像生成処理
    /// </summary>
    public sealed class StampImageFactory : IDisposable
    {
        /// <summary>
        /// 設定
        /// </summary>
        public StampImageFactoryConfig Config { get; set; } = new StampImageFactoryConfig();


        /// <summary>
        /// コンストラクター
        /// </summary>
        public StampImageFactory()
        {
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="config"></param>
        public StampImageFactory(StampImageFactoryConfig config)
        {
            this.Config = config;
        }

        /// <summary>
        /// リサイズ
        /// </summary>
        /// <param name="src"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Bitmap Resize(Bitmap src, int width, int height)
        {

            StampUtils.RequiredArgument(src, "src");

            Bitmap canvas = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(canvas);

            g.SmoothingMode = SmoothingMode.HighQuality;

            g.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.High;
            g.DrawImage(src, 0, 0, width, height);

            g.Dispose();

            return canvas;
        }

        /// <summary>
        /// イメージを保存します
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="fileName"></param>
        public void Save(BaseStamp stamp, string fileName)
        {
            StampUtils.RequiredArgument(stamp, "stamp");
            StampUtils.RequiredArgument(fileName, "fileName");

            using (Bitmap image = Create(stamp))
            {

                if (!fileName.ToLower().EndsWith(".png"))
                {
                    fileName += ".png";
                }

                image.Save(fileName, ImageFormat.Png);
            }

        }

        /// <summary>
        /// イメージを作成します
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Bitmap Create(BaseStamp stamp)
        {

            if (stamp is ThreeAreaCircularStamp threeAreaCircularStamp)
            {
                return Create(threeAreaCircularStamp);
            }

            if (stamp is RectangleStamp squareStamp)
            {
                return Create(squareStamp);
            }

            if (stamp is CircularStamp circularStamp)
            {
                return Create(circularStamp);
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// 垂直に3分割した印鑑イメージを作成します。
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Bitmap Create(ThreeAreaCircularStamp stamp)
        {
            StampUtils.RequiredArgument(stamp, "stamp");


            int imageWidth = stamp.Size.Width;
            int imageHeight = stamp.Size.Height;
            Pen edgePen = new Pen(stamp.Color)
            {
                Width = stamp.EdgeWidth
            };


            Bitmap stampImage = new Bitmap(imageWidth, imageHeight);

            Graphics graphics = Graphics.FromImage(stampImage);

            graphics.SmoothingMode = SmoothingMode.HighQuality;

            // 回転
            int halfImageWidth = imageWidth / 2;
            int halfImageHeight = imageHeight / 2;

            graphics.TranslateTransform(-halfImageWidth, -halfImageHeight);
            graphics.RotateTransform(-stamp.RotationAngle, MatrixOrder.Append);
            graphics.TranslateTransform(halfImageWidth, halfImageHeight, MatrixOrder.Append);

            // 半径
            int r = (imageWidth - (imageWidth / 20)) / 2;

            //int r = 120;
            // 画像の縁からスタンプの縁までの上下左右の最も短い絶対値
            int outerSpace = (imageWidth - (r * 2)) / 2;


            // 2重円
            if (stamp.EdgeType == StampEdgeType.DOUBLE)
            {
                // 外円描画
                graphics.DrawEllipse(edgePen, outerSpace, outerSpace, 2 * r, 2 * r);


                // 内円の設定へ更新
                r -= stamp.DoubleEdgeOffset;
                outerSpace += stamp.DoubleEdgeOffset;

            }

            // 分割ライン描画用の角度　高さや弦の算出基準
            int angle = 15;

            // 印鑑の縁
            graphics.DrawEllipse(edgePen, outerSpace, outerSpace, 2 * r, 2 * r);

#if DEBUG
            if (StampUtils.IsDebug())
            {
                graphics.DrawRectangle(new Pen(Color.Green), outerSpace, outerSpace, 2 * r, 2 * r);
            }
#endif

            // 弦（上下の分割線長）
            int chord = (int)Math.Round(2 * r * Math.Sin(StampUtils.ConvertToRadian(90 - angle)));
            int y = (int)Math.Round(r * Math.Sin(StampUtils.ConvertToRadian(angle)));

            // 分割ラインと縁が重なる点からのスペース
            int space = (r * 2 - chord) / 2;

            int topLineY = imageWidth / 2 - y;
            int bottomLineY = imageWidth / 2 + y;

            Pen dividerPen = new Pen(stamp.Color)
            {
                Width = stamp.DividerWidth
            };

            // 上部ライン
            graphics.DrawLine(dividerPen, space + outerSpace, topLineY, imageWidth - (space + outerSpace), topLineY);
            // 下部ライン
            graphics.DrawLine(dividerPen, space + outerSpace, bottomLineY, imageWidth - (space + outerSpace), bottomLineY);


            StringFormat sf = new StringFormat();
            //sf.FormatFlags = StringFormatFlags.DirectionVertical;
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            Brush fontBrush = new SolidBrush(stamp.Color);
            float stringX = imageWidth / 2;
            // 上段テキスト
            StampText topText = stamp.TopText;
            if (topText != null)
            {
                Font font = new Font(topText.FontFamily, topText.Size);
                float descentSize = GetFontDescentSize(graphics, topText, font);
                SizeF topStringSize = graphics.MeasureString(topText.Value, font);
                float topStringY = topLineY - topStringSize.Height / 2 - stamp.TopBottomTextOffset + (topText.IsIgnoreFontDescent ? descentSize / 2 : 0);
                graphics.DrawString(topText.Value, font, fontBrush, stringX, topStringY, sf);


#if DEBUG
                if (StampUtils.IsDebug())
                {
                    graphics.DrawRectangle(new Pen(Color.Green), stringX, topStringY, topStringSize.Width, topStringSize.Height);
                }
#endif
            }

            // 中段テキスト
            StampText middleText = stamp.MiddleText;
            if (middleText != null)
            {
                Font font = new Font(middleText.FontFamily, middleText.Size);
                float descentSize = GetFontDescentSize(graphics, middleText, font);
                SizeF middleStringSize = graphics.MeasureString(middleText.Value, font);
                float middleStringY = imageHeight / 2 + (middleText.IsIgnoreFontDescent ? descentSize / 2 : 0);
                graphics.DrawString(middleText.Value, font, fontBrush, stringX, middleStringY, sf);

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    graphics.DrawRectangle(new Pen(Color.Green), stringX, middleStringY, middleStringSize.Width, middleStringSize.Height);
                }
#endif
            }

            // 下段テキスト
            StampText bottomText = stamp.BottomText;
            if (bottomText != null)
            {
                Font font = new Font(bottomText.FontFamily, bottomText.Size);
                SizeF bottomStringSize = graphics.MeasureString(bottomText.Value, font);
                float bottomStringY = bottomLineY + bottomStringSize.Height / 2 + stamp.TopBottomTextOffset;
                graphics.DrawString(bottomText.Value, font, fontBrush, stringX, bottomStringY, sf);

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    graphics.DrawRectangle(new Pen(Color.Green), stringX, bottomStringY, bottomStringSize.Width, bottomStringSize.Height);
                }
#endif
            }


            // 背景透過
            stampImage.MakeTransparent();



            if (stamp.EffectTypes.Contains(StampEffectType.NOISE))
            {
                AppendNoise(stamp, stampImage);
            }

            fontBrush.Dispose();
            edgePen.Dispose();
            graphics.Dispose();

            return stampImage;
        }

        /// <summary>
        /// 4角形の印鑑イメージを作成します。
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Bitmap Create(RectangleStamp stamp)
        {
            StampUtils.RequiredArgument(stamp, "stamp");

            int imageWidth = stamp.Size.Width;
            int imageHeight = stamp.Size.Height;

            Pen edgePen = new Pen(stamp.Color)
            {
                Width = stamp.EdgeWidth
            };


            Bitmap stampImage = new Bitmap(imageWidth, imageHeight);

            Graphics graphics = Graphics.FromImage(stampImage);

            graphics.SmoothingMode = SmoothingMode.HighQuality;

            // 回転
            int halfImageWidth = imageWidth / 2;
            int halfImageHeight = imageHeight / 2;


            graphics.TranslateTransform(-halfImageWidth, -halfImageHeight);
            graphics.RotateTransform(-stamp.RotationAngle, MatrixOrder.Append);
            graphics.TranslateTransform(halfImageWidth, halfImageHeight, MatrixOrder.Append);

            // 半径
            int stampWidth = (imageWidth - (imageWidth / 20));
            int stampHeight = (imageHeight - (imageHeight / 20));

            int outerSpaceX = (imageWidth - stampWidth) / 2;
            int outerSpaceY = (imageHeight - stampHeight) / 2;

            int edgeRadius = stamp.EdgeRadius;

            // 2重
            if (stamp.EdgeType == StampEdgeType.DOUBLE)
            {
                // 外描画
                DrawRoundedRectangle(graphics, edgePen, outerSpaceX, outerSpaceY, stampWidth, stampHeight, edgeRadius);

                // 内の設定へ更新
                stampHeight -= stamp.DoubleEdgeOffset * 2;
                stampWidth -= stamp.DoubleEdgeOffset * 2;
                outerSpaceX += stamp.DoubleEdgeOffset;
                outerSpaceY += stamp.DoubleEdgeOffset;

                if (edgeRadius > stamp.DoubleEdgeOffset)
                {
                    // できるだけ、コーナーの線の距離を直線とそろえる
                    edgeRadius -= stamp.DoubleEdgeOffset;
                }
            }


            // 印鑑の縁
            DrawRoundedRectangle(graphics, edgePen, outerSpaceX, outerSpaceY, stampWidth, stampHeight, edgeRadius);



            StringFormat sf = new StringFormat();

            if (stamp.TextOrientationType == TextOrientationType.VERTICAL)
            {
                sf.FormatFlags = StringFormatFlags.DirectionVertical;
            }
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            Brush fontBrush = new SolidBrush(stamp.Color);
            float stringX = imageWidth / 2;

            StampText stampText = stamp.Text;
            if (stampText != null)
            {
                Font font = new Font(stampText.FontFamily, stampText.Size);
                float descentSize = GetFontDescentSize(graphics, stampText, font);
                float stringY = imageHeight / 2;
                if (stampText.IsIgnoreFontDescent && stamp.TextOrientationType == TextOrientationType.HORIZONTAL)
                {
                    stringY += descentSize / 2;
                }
                graphics.DrawString(stampText.Value, font, fontBrush, stringX, stringY, sf);
            }


            // 背景透過
            stampImage.MakeTransparent();

            if (stamp.EffectTypes.Contains(StampEffectType.NOISE))
            {
                AppendNoise(stamp, stampImage);
            }


            fontBrush.Dispose();
            edgePen.Dispose();
            graphics.Dispose();

            return stampImage;
        }

        /// <summary>
        /// 円形の印鑑イメージを作成します。
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Bitmap Create(CircularStamp stamp)
        {
            StampUtils.RequiredArgument(stamp, "stamp");

            int imageWidth = stamp.Size.Width;
            int imageHeight = stamp.Size.Height;
            Pen edgePen = new Pen(stamp.Color)
            {
                Width = stamp.EdgeWidth
            };


            Bitmap stampImage = new Bitmap(imageWidth, imageHeight);

            Graphics graphics = Graphics.FromImage(stampImage);

            graphics.SmoothingMode = SmoothingMode.HighQuality;

            // 回転
            int halfImageWidth = imageWidth / 2;
            int halfImageHeight = imageHeight / 2;

            graphics.TranslateTransform(-halfImageWidth, -halfImageHeight);
            graphics.RotateTransform(-stamp.RotationAngle, MatrixOrder.Append);
            graphics.TranslateTransform(halfImageWidth, halfImageHeight, MatrixOrder.Append);

            // 半径
            int r = (imageWidth - (imageWidth / 20)) / 2;

            //int r = 120;
            // 画像の縁からスタンプの縁までの上下左右の最も短い絶対値
            int outerSpace = (imageWidth - (r * 2)) / 2;


            // 2重円
            if (stamp.EdgeType == StampEdgeType.DOUBLE)
            {
                // 外円描画
                graphics.DrawEllipse(edgePen, outerSpace, outerSpace, 2 * r, 2 * r);


                // 内円の設定へ更新
                r -= stamp.DoubleEdgeOffset;
                outerSpace += stamp.DoubleEdgeOffset;

            }

            // 印鑑の縁
            graphics.DrawEllipse(edgePen, outerSpace, outerSpace, 2 * r, 2 * r);


            // TODO 微妙にずれる
            StringFormat sf = new StringFormat();

            if (stamp.TextOrientationType == TextOrientationType.VERTICAL)
            {
                sf.FormatFlags = StringFormatFlags.DirectionVertical;
            }
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            Brush fontBrush = new SolidBrush(stamp.Color);
            float stringX = imageWidth / 2;

            StampText stampText = stamp.Text;
            if (stampText != null)
            {
                Font font = new Font(stampText.FontFamily, stampText.Size);
                float descentSize = GetFontDescentSize(graphics, stampText, font);

                float stringY = imageHeight / 2;

                if (stampText.IsIgnoreFontDescent && stamp.TextOrientationType == TextOrientationType.HORIZONTAL)
                {
                    stringY += descentSize / 2;
                }
                graphics.DrawString(stampText.Value, font, fontBrush, stringX, stringY, sf);

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    var dPen = new Pen(Color.Green)
                    {
                        Width = 2
                    };
                    graphics.DrawLine(dPen, stringX, 0, stringX, stamp.Size.Height);
                    graphics.DrawLine(dPen, 0, stringY, stamp.Size.Width, stringY);
                }
#endif
            }


            // 背景透過
            stampImage.MakeTransparent();



            if (stamp.EffectTypes.Contains(StampEffectType.NOISE))
            {
                AppendNoise(stamp, stampImage);
            }


            fontBrush.Dispose();
            edgePen.Dispose();
            graphics.Dispose();

            return stampImage;
        }

        /// <summary>
        /// ランダムにノイズを付与します。
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="stampImage"></param>
        static void AppendNoise(BaseStamp stamp, Bitmap stampImage)
        {
            // TODO 適当だからもっとスタンプ風になる加工あるか調べよ
            Random rand = new Random();

            for (int i = 0; i < stamp.Size.Width; i++)
            {
                for (int j = 0; j < stamp.Size.Height; j++)
                {
                    Color pixelColor = stampImage.GetPixel(i, j);

                    if (pixelColor.A == 0)
                    {
                        continue;
                    }

                    if (1 > rand.Next(5))
                    {
                        stampImage.SetPixel(i, j, Color.FromArgb(rand.Next(64), pixelColor));
                    }
                }
            }
        }

        /// <summary>
        /// 丸角の四角形を描画します。
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pen"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        static void DrawRoundedRectangle(Graphics graphics, Pen pen, int x, int y, int width, int height, int radius)
        {

            if (radius == 0)
            {
                graphics.DrawRectangle(pen, x, y, width, height);
                return;
            }

            GraphicsPath path = new GraphicsPath();
            // 左上
            path.AddArc(x, y, 2 * radius, 2 * radius, 180, 90);

            // 上ライン
            path.AddLine(x + radius, y, x + width - radius, y);

            // 右上
            path.AddArc(x + width - 2 * radius, y, 2 * radius, 2 * radius, 270, 90);

            // 右ライン
            path.AddLine(x + width, y + radius, x + width, y + height - radius);

            // 右下
            path.AddArc(x + width - 2 * radius, y + height - 2 * radius, radius + radius, radius + radius, 0, 90);

            // 下ライン
            path.AddLine(x + radius, y + height, x + width - radius, y + height);

            // 左下
            path.AddArc(x, y + height - 2 * radius, 2 * radius, 2 * radius, 90, 90);


            path.CloseFigure();
            graphics.DrawPath(pen, path);
        }

        /// <summary>
        /// <see href="https://docs.microsoft.com/ja-jp/dotnet/desktop/winforms/advanced/how-to-obtain-font-metrics?view=netframeworkdesktop-4.8"/>
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="stampText"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        static float GetFontDescentSize(Graphics graphics, StampText stampText, Font font)
        {
            SizeF size = graphics.MeasureString(stampText.Value, font);

            int descent = stampText.FontFamily.GetCellDescent(font.Style);
            int emHeight = stampText.FontFamily.GetEmHeight(font.Style);

            float descentSize = size.Height * descent / emHeight;

            return descentSize;
        }

        /// <summary>
        /// <see href="https://docs.microsoft.com/ja-jp/dotnet/desktop/winforms/advanced/how-to-obtain-font-metrics?view=netframeworkdesktop-4.8"/>
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="stampText"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        static float GetFontAscentSize(Graphics graphics, StampText stampText, Font font)
        {
            SizeF size = graphics.MeasureString(stampText.Value, font);

            int ascent = stampText.FontFamily.GetCellAscent(font.Style);
            int emHeight = stampText.FontFamily.GetEmHeight(font.Style);

            float ascentSize = size.Height * ascent / emHeight;

            return ascentSize;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Config.Dispose();
        }
    }




}
