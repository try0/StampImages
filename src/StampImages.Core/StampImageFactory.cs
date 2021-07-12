using StampImages.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

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
            g.InterpolationMode = InterpolationMode.High;

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
            if (stamp.EdgeType == StampEdgeType.Double)
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

            if (stamp.IsFillColor)
            {
                using (var fillBrush = new SolidBrush(stamp.Color))
                {
                    graphics.FillEllipse(fillBrush, outerSpace, outerSpace, 2 * r, 2 * r);
                }

            }

#if DEBUG
            if (StampUtils.IsDebug())
            {
                using (var pen = GetDebugPen())
                {
                    graphics.DrawRectangle(pen, outerSpace, outerSpace, 2 * r, 2 * r);
                }
            }
#endif

            // 弦（上下の分割線長）
            int chord = (int)Math.Round(2 * r * Math.Sin(StampUtils.ConvertToRadian(90 - angle)));
            int y = (int)Math.Round(r * Math.Sin(StampUtils.ConvertToRadian(angle)));

            // 分割ラインと縁が重なる点からのスペース
            int space = (r * 2 - chord) / 2;

            int topLineY = imageWidth / 2 - y;
            int bottomLineY = imageWidth / 2 + y;

            Pen dividerPen = stamp.IsFillColor ? new Pen(stamp.Color.GetInvertColor()) : new Pen(stamp.Color);

            dividerPen.Width = stamp.DividerWidth;

            if (stamp.IsFillColor)
            {
                space -= Math.Max((int)( stamp.DoubleEdgeOffset * 0.6), 0);
            }

            graphics.SmoothingMode = SmoothingMode.HighSpeed;

            // 上部ライン
            graphics.DrawLine(dividerPen, space + outerSpace, topLineY, imageWidth - (space + outerSpace), topLineY);
            // 下部ライン
            graphics.DrawLine(dividerPen, space + outerSpace, bottomLineY, imageWidth - (space + outerSpace), bottomLineY);

            graphics.SmoothingMode = SmoothingMode.HighQuality;

 

            StringFormat sf = new StringFormat(StringFormat.GenericTypographic);

            Brush fontBrush = stamp.IsFillColor ? new SolidBrush(stamp.Color.GetInvertColor()) : new SolidBrush(stamp.Color);
            float stringX = imageWidth / 2;
            // 上段テキスト
            StampText topText = stamp.TopText;
            if (topText != null)
            {
                Font font = new Font(topText.FontFamily, topText.Size);

                SizeF size = graphics.MeasureString(topText.Value, font, imageWidth, sf);
                RectangleF rect = graphics.MeasureDrawedString(topText.Value, font, stamp.Size, sf);

                var basePosY = topLineY - stamp.TopBottomTextOffset - rect.Height;

                var tmpPosX = ((stamp.Size.Width - size.Width) / 2);

                var centerPosX = (stamp.Size.Width - rect.Width) / 2;
                var centerPosY = basePosY - (size.Height - rect.Height) / 2;


#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif
                centerPosX -= (centerPosX - tmpPosX);

                graphics.DrawString(topText.Value, font, fontBrush, centerPosX, centerPosY, sf);

            }

            // 中段テキスト
            StampText middleText = stamp.MiddleText;
            if (middleText != null)
            {
                Font font = new Font(middleText.FontFamily, middleText.Size);

                SizeF size = graphics.MeasureString(middleText.Value, font, imageWidth, sf);
                RectangleF rect = graphics.MeasureDrawedString(middleText.Value, font, stamp.Size, sf);

                var tmpPosX = ((stamp.Size.Width - size.Width) / 2);
                var tmpPosY = ((stamp.Size.Height - size.Height) / 2);

                var centerPosX = (stamp.Size.Width - rect.Width) / 2;
                var centerPosY = (stamp.Size.Height - rect.Height) / 2;

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif


                centerPosX -= (centerPosX - tmpPosX);
                centerPosY -= (centerPosY - tmpPosY);


                graphics.DrawString(middleText.Value, font, fontBrush, centerPosX, centerPosY, sf);
            }

            // 下段テキスト
            StampText bottomText = stamp.BottomText;
            if (bottomText != null)
            {

                Font font = new Font(bottomText.FontFamily, bottomText.Size);

                SizeF size = graphics.MeasureString(bottomText.Value, font, imageWidth, sf);
                RectangleF rect = graphics.MeasureDrawedString(bottomText.Value, font, stamp.Size, sf);

                var basePosY = bottomLineY + stamp.TopBottomTextOffset;

                var tmpPosX = ((stamp.Size.Width - size.Width) / 2);

                var centerPosX = (stamp.Size.Width - rect.Width) / 2;
                var centerPosY = basePosY - (size.Height - rect.Height) / 2;


#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif
                centerPosX -= (centerPosX - tmpPosX);

                graphics.DrawString(bottomText.Value, font, fontBrush, centerPosX, centerPosY, sf);
            }


            // 背景透過
            stampImage.MakeTransparent();
            if (stamp.IsFillColor)
            {
                stampImage.MakeTransparent(stamp.Color.GetInvertColor());
            }

#if DEBUG
            if (StampUtils.IsDebug())
            {
                DrawCenterLines(stampImage, graphics);
            }
#endif


            AppendEffects(stamp, stampImage);

            sf.Dispose();
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
            if (stamp.EdgeType == StampEdgeType.Double)
            {
                // 外描画
                DrawRoundedRectangle(graphics, edgePen, outerSpaceX, outerSpaceY, stampWidth, stampHeight, edgeRadius, false);

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
            DrawRoundedRectangle(graphics, edgePen, outerSpaceX, outerSpaceY, stampWidth, stampHeight, edgeRadius, stamp.IsFillColor);



            StringFormat sf = new StringFormat(StringFormat.GenericTypographic);

            if (stamp.TextOrientationType == TextOrientationType.Vertical)
            {
                sf.FormatFlags = StringFormatFlags.DirectionVertical;
            }


            Brush fontBrush = stamp.IsFillColor ? new SolidBrush(stamp.Color.GetInvertColor()) : new SolidBrush(stamp.Color);
            float stringX = imageWidth / 2;

            StampText stampText = stamp.Text;
            if (stampText != null)
            {
                Font font = new Font(stampText.FontFamily, stampText.Size);

                var size = graphics.MeasureString(stampText.Value, font, imageWidth, sf);
                RectangleF rect = graphics.MeasureDrawedString(stampText.Value, font, stamp.Size, sf);

                var tmpPosX = ((stamp.Size.Width - size.Width) / 2);
                var tmpPosY = ((stamp.Size.Height - size.Height) / 2);

                var centerPosX = (stamp.Size.Width - rect.Width) / 2;
                var centerPosY = (stamp.Size.Height - rect.Height) / 2;

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif


                centerPosX -= (centerPosX - tmpPosX);
                centerPosY -= (centerPosY - tmpPosY);


                graphics.DrawString(stampText.Value, font, fontBrush, centerPosX, centerPosY, sf);
            }


            // 背景透過
            stampImage.MakeTransparent();
            if (stamp.IsFillColor)
            {
                stampImage.MakeTransparent(stamp.Color.GetInvertColor());
            }

#if DEBUG
            if (StampUtils.IsDebug())
            {
                DrawCenterLines(stampImage, graphics);
            }
#endif


            AppendEffects(stamp, stampImage);

            sf.Dispose();
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
            if (stamp.EdgeType == StampEdgeType.Double)
            {
                // 外円描画
                graphics.DrawEllipse(edgePen, outerSpace, outerSpace, 2 * r, 2 * r);


                // 内円の設定へ更新
                r -= stamp.DoubleEdgeOffset;
                outerSpace += stamp.DoubleEdgeOffset;

            }

            // 印鑑の縁
            graphics.DrawEllipse(edgePen, outerSpace, outerSpace, 2 * r, 2 * r);

            if (stamp.IsFillColor)
            {
                using (var fillBrush = new SolidBrush(stamp.Color))
                {
                    graphics.FillEllipse(fillBrush, outerSpace, outerSpace, 2 * r, 2 * r);
                }
            }



            StringFormat sf = new StringFormat(StringFormat.GenericTypographic);

            if (stamp.TextOrientationType == TextOrientationType.Vertical)
            {
                sf.FormatFlags = StringFormatFlags.DirectionVertical;
            }


            Brush fontBrush = stamp.IsFillColor ? new SolidBrush(stamp.Color.GetInvertColor()) : new SolidBrush(stamp.Color);
            float stringX = imageWidth / 2;

            StampText stampText = stamp.Text;
            if (stampText != null)
            {
                Font font = new Font(stampText.FontFamily, stampText.Size);

                var size = graphics.MeasureString(stampText.Value, font, imageWidth, sf);
                RectangleF rect = graphics.MeasureDrawedString(stampText.Value, font, stamp.Size, sf);

                var tmpPosX = ((stamp.Size.Width - size.Width) / 2);
                var tmpPosY = ((stamp.Size.Height - size.Height) / 2);

                var centerPosX = (stamp.Size.Width - rect.Width) / 2;
                var centerPosY = (stamp.Size.Height - rect.Height) / 2;

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif


                centerPosX -= (centerPosX - tmpPosX);
                centerPosY -= (centerPosY - tmpPosY);


                graphics.DrawString(stampText.Value, font, fontBrush, centerPosX, centerPosY, sf);

            }

#if DEBUG
            if (StampUtils.IsDebug())
            {
                DrawCenterLines(stampImage, graphics);
            }
#endif


            // 背景透過
            stampImage.MakeTransparent();

            if (stamp.IsFillColor)
            {
                stampImage.MakeTransparent(stamp.Color.GetInvertColor());
            }

            AppendEffects(stamp, stampImage);

            sf.Dispose();
            fontBrush.Dispose();
            edgePen.Dispose();
            graphics.Dispose();

            return stampImage;
        }

        /// <summary>
        /// 加工処理を実行します
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="stampImage"></param>
        static void AppendEffects(BaseStamp stamp, Bitmap stampImage)
        {
            if (stamp.EffectTypes.Contains(StampEffectType.Grunge))
            {
                AppendGrunge(stamp, stampImage);
            }

            if (stamp.EffectTypes.Contains(StampEffectType.Noise))
            {
                AppendNoise(stamp, stampImage);
            }
        }

        /// <summary>
        /// ランダムにノイズを付与します。
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="stampImage"></param>
        static void AppendNoise(BaseStamp stamp, Bitmap stampImage)
        {
            Random rand = new Random();


            // 配列へ展開
            PixelFormat stampPixelFormat = PixelFormat.Format32bppArgb;
            int stampPixelSize = 4;

            BitmapData stampData =
                stampImage.LockBits(new Rectangle(0, 0, stampImage.Width, stampImage.Height), ImageLockMode.ReadWrite, stampPixelFormat);

            IntPtr stampPtr = stampData.Scan0;
            int stampBytes = Math.Abs(stampData.Stride) * stampImage.Height;
            byte[] stampRgbValues = new byte[stampBytes];
            Marshal.Copy(stampPtr, stampRgbValues, 0, stampBytes);


            for (int x = 0; x < stampData.Width; x++)
            {
                for (int y = 0; y < stampData.Height; y++)
                {
                    int pos = y * stampData.Stride + x * stampPixelSize;


                    byte a = stampRgbValues[pos + 3];

                    if (a == 0)
                    {
                        continue;
                    }

                    if (1 > rand.Next(5))
                    {
                        stampRgbValues[pos + 3] = (byte)rand.Next(64);
                    }

                }
            }

            Marshal.Copy(stampRgbValues, 0, stampPtr, stampRgbValues.Length);
            stampImage.UnlockBits(stampData);


        }

        /// <summary>
        /// 汚し加工を付与します。
        /// <para><see href="https://jp.freepik.com/photos/grunge"/>Freepik.comのリソースを使用しています</para>
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="stampImage"></param>
        static void AppendGrunge(BaseStamp stamp, Bitmap stampImage)
        {
            Random rand = new Random();

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (var grungeBitmap = new Bitmap(assembly.GetManifestResourceStream(@"StampImages.Core.Resource.effect_grunge.jpg")))
            {

                // 配列へ展開
                PixelFormat stampPixelFormat = stampImage.PixelFormat;
                int stampPixelSize = Image.GetPixelFormatSize(stampPixelFormat) / 8;
                if (stampPixelSize < 3 || 4 < stampPixelSize)
                {
                    throw new ArgumentException();
                }


                BitmapData stampData =
                    stampImage.LockBits(new Rectangle(0, 0, stampImage.Width, stampImage.Height), ImageLockMode.ReadWrite, stampPixelFormat);

                IntPtr stampPtr = stampData.Scan0;
                int stampBytes = Math.Abs(stampData.Stride) * stampImage.Height;
                byte[] stampRgbValues = new byte[stampBytes];
                Marshal.Copy(stampPtr, stampRgbValues, 0, stampBytes);



                PixelFormat pixelFormat = grungeBitmap.PixelFormat;
                int pixelSize = Image.GetPixelFormatSize(pixelFormat) / 8;
                if (pixelSize < 3 || 4 < pixelSize)
                {
                    throw new ArgumentException();
                }


                BitmapData bmpData =
                    grungeBitmap.LockBits(new Rectangle(0, 0, grungeBitmap.Width, grungeBitmap.Height), ImageLockMode.ReadOnly, pixelFormat);

                IntPtr ptr = bmpData.Scan0;
                int bytes = Math.Abs(bmpData.Stride) * grungeBitmap.Height;
                byte[] rgbValues = new byte[bytes];
                Marshal.Copy(ptr, rgbValues, 0, bytes);


                int xRange = grungeBitmap.Size.Width - stampImage.Size.Width;
                int yRange = grungeBitmap.Size.Height - stampImage.Size.Height;

                int xInit = rand.Next(xRange);
                int yInit = rand.Next(yRange);

                int grungeLength = grungeBitmap.Width * grungeBitmap.Height * pixelSize;

                for (int x = xInit; x < xInit + stampData.Width; x++)
                {
                    for (int y = yInit; y < yInit + stampData.Height; y++)
                    {
                        int pos = (y * bmpData.Stride + x * pixelSize) % grungeLength;

                        // RGB
                        byte b = rgbValues[pos];
                        byte g = rgbValues[pos + 1];
                        byte r = rgbValues[pos + 2];


                        int stampPos = (y - yInit) * stampData.Stride + (x - xInit) * stampPixelSize;

                        var stampAlpha = stampRgbValues[stampPos + 3];
                        if (stampAlpha != 0)
                        {
                            stampRgbValues[stampPos + 3] = (byte)((r + g + b) / 3);
                        }

                    }
                }

                Marshal.Copy(stampRgbValues, 0, stampPtr, stampRgbValues.Length);
                grungeBitmap.UnlockBits(bmpData);
                stampImage.UnlockBits(stampData);
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
        static void DrawRoundedRectangle(Graphics graphics, Pen pen, int x, int y, int width, int height, int radius, bool isFill)
        {

            if (radius == 0)
            {
                graphics.DrawRectangle(pen, x, y, width, height);

                if (isFill)
                {
                    using (var fillBrush = new SolidBrush(pen.Color))
                    {
                        graphics.FillRectangle(fillBrush, x, y, width, height);
                    }
                }
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

            if (isFill)
            {
                using (var fillBrush = new SolidBrush(pen.Color))
                {
                    graphics.FillPath(fillBrush, path);
                }
            }
        }


        #region DEBUG

        private static void DrawCenterLines(Bitmap bitmap, Graphics graphics)
        {
            var dPen = GetDebugPen();
            graphics.DrawLine(dPen, bitmap.Size.Width / 2, 0, bitmap.Size.Width / 2, bitmap.Size.Height);
            graphics.DrawLine(dPen, 0, bitmap.Size.Height / 2, bitmap.Size.Width, bitmap.Size.Height / 2);
            dPen.Dispose();
        }

        private static Pen GetDebugPen() => new Pen(Color.Green)
        {
            Width = 3
        };

        #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            Config.Dispose();
        }
    }




}
