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
        /// <para>常に正円で描画されます。指定したWidth、Heightの小さいいずれかの値でスタンプを描画します。</para>
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Bitmap Create(ThreeAreaCircularStamp stamp)
        {
            StampUtils.RequiredArgument(stamp, "stamp");


            int imageWidth = stamp.Size.Width + stamp.Margin.LeftRight * 2;
            int imageHeight = stamp.Size.Height + stamp.Margin.TopBottom * 2;
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



            // 小さい方をスタンプサイズとする
            int stampSize = Math.Min(stamp.Size.Width, stamp.Size.Height);

            // 半径
            int r = stampSize / 2;

            int outerSpaceX = stamp.Margin.LeftRight;
            int outerSpaceY = stamp.Margin.TopBottom;

            if (stamp.Size.Width > stamp.Size.Height)
            {
                outerSpaceX += Math.Abs(stamp.Size.Width - stamp.Size.Height) / 2;
            }
            else if (stamp.Size.Width < stamp.Size.Height)
            {
                outerSpaceY += Math.Abs(stamp.Size.Width - stamp.Size.Height) / 2;
            }

            // 2重円
            if (stamp.EdgeType == StampEdgeType.Double)
            {
                // 外円描画
                graphics.DrawEllipse(edgePen, outerSpaceX, outerSpaceY, 2 * r, 2 * r);

                // 内円の設定へ更新
                r -= stamp.DoubleEdgeOffset;
                outerSpaceX += stamp.DoubleEdgeOffset;
                outerSpaceY += stamp.DoubleEdgeOffset;

            }

            // 分割ライン描画用の角度　高さや弦の算出基準
            int angle = 15;

            // 印鑑の縁
            graphics.DrawEllipse(edgePen, outerSpaceX, outerSpaceY, 2 * r, 2 * r);

            if (stamp.IsFillColor)
            {
                using (var fillBrush = new SolidBrush(stamp.Color))
                {
                    graphics.FillEllipse(fillBrush, outerSpaceX, outerSpaceY, 2 * r, 2 * r);
                }

            }

#if DEBUG
            if (StampUtils.IsDebug())
            {
                using (var pen = GetDebugPen())
                {
                    graphics.DrawRectangle(pen, outerSpaceX, outerSpaceY, 2 * r, 2 * r);
                }
            }
#endif

            // 弦（上下の分割線長）
            int chord = (int)Math.Round(2 * r * Math.Sin(StampUtils.ConvertToRadian(90 - angle)));
            int y = (int)Math.Round(r * Math.Sin(StampUtils.ConvertToRadian(angle)));

            // 分割ラインと縁が重なる点からのスペース
            int space = (r * 2 - chord) / 2;

            int topLineY = imageHeight / 2 - y;
            int bottomLineY = imageHeight / 2 + y;

            Pen dividerPen = stamp.IsFillColor ? new Pen(stamp.Color.GetInvertColor()) : new Pen(stamp.Color);

            dividerPen.Width = stamp.DividerWidth;

            if (stamp.IsFillColor)
            {
                space -= Math.Max((int)(stamp.DoubleEdgeOffset * 0.6), 0);
            }

            graphics.SmoothingMode = SmoothingMode.HighSpeed;

            // 上部ライン
            graphics.DrawLine(dividerPen, space + outerSpaceX + 1, topLineY, imageWidth - (space + outerSpaceX) - 1, topLineY);
            // 下部ライン
            graphics.DrawLine(dividerPen, space + outerSpaceX + 1, bottomLineY, imageWidth - (space + outerSpaceX) - 1, bottomLineY);

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

                float centerPosX = (imageWidth - rect.Width) / 2;
                float centerPosY = topLineY - stamp.TopBottomTextOffset - rect.Height;


#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif

                centerPosX -= rect.X;
                centerPosY -= rect.Y;

                graphics.DrawString(topText.Value, font, fontBrush, centerPosX, centerPosY, sf);

            }

            // 中段テキスト
            StampText middleText = stamp.MiddleText;
            if (middleText != null)
            {
                Font font = new Font(middleText.FontFamily, middleText.Size);

                SizeF size = graphics.MeasureString(middleText.Value, font, imageWidth, sf);
                RectangleF rect = graphics.MeasureDrawedString(middleText.Value, font, stamp.Size, sf);

                float centerPosX = (imageWidth - rect.Width) / 2;
                float centerPosY = (imageHeight - rect.Height) / 2;

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif

                centerPosX -= rect.X;
                centerPosY -= rect.Y;


                graphics.DrawString(middleText.Value, font, fontBrush, centerPosX, centerPosY, sf);
            }

            // 下段テキスト
            StampText bottomText = stamp.BottomText;
            if (bottomText != null)
            {

                Font font = new Font(bottomText.FontFamily, bottomText.Size);

                SizeF size = graphics.MeasureString(bottomText.Value, font, imageWidth, sf);
                RectangleF rect = graphics.MeasureDrawedString(bottomText.Value, font, stamp.Size, sf);

                float centerPosX = (imageWidth - rect.Width) / 2;
                float centerPosY = bottomLineY + stamp.TopBottomTextOffset;


#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif

                centerPosX -= rect.X;
                centerPosY -= rect.Y;

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

            int imageWidth = stamp.Size.Width + stamp.Margin.LeftRight * 2;
            int imageHeight = stamp.Size.Height + stamp.Margin.TopBottom * 2;

            Pen edgePen = new Pen(stamp.Color)
            {
                Width = stamp.EdgeWidth
            };

            // 回転後にひつような描画サイズへ更新
            Size rotatedSize = new Size(imageWidth, imageHeight).GetRotatedContainerSize(stamp.RotationAngle);
            imageWidth = rotatedSize.Width;
            imageHeight = rotatedSize.Height;

            Bitmap stampImage = new Bitmap(rotatedSize.Width, rotatedSize.Height);

            Graphics graphics = Graphics.FromImage(stampImage);

            graphics.SmoothingMode = SmoothingMode.HighQuality;

            // 回転
            int halfImageWidth = imageWidth / 2;
            int halfImageHeight = imageHeight / 2;


            graphics.TranslateTransform(-halfImageWidth, -halfImageHeight);
            graphics.RotateTransform(-stamp.RotationAngle, MatrixOrder.Append);
            graphics.TranslateTransform(halfImageWidth, halfImageHeight, MatrixOrder.Append);


            int stampWidth = stamp.Size.Width;
            int stampHeight = stamp.Size.Height;

            int outerSpaceX = (imageWidth - stamp.Size.Width) / 2;
            int outerSpaceY = (imageHeight - stamp.Size.Height) / 2;

            int edgeRadius = stamp.EdgeRadius;

            // 2重
            if (stamp.EdgeType == StampEdgeType.Double)
            {
                // 外描画
                graphics.DrawRoundedRectangle(edgePen, outerSpaceX, outerSpaceY, stampWidth, stampHeight, edgeRadius);

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
            if (stamp.IsFillColor)
            {
                using (var edgeBrush = new SolidBrush(edgePen.Color))
                {
                    graphics.FillRoundedRectangle(edgeBrush, outerSpaceX, outerSpaceY, stampWidth, stampHeight, edgeRadius);
                }
            }
            else
            {
                graphics.DrawRoundedRectangle(edgePen, outerSpaceX, outerSpaceY, stampWidth, stampHeight, edgeRadius);
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

                float centerPosX = (imageWidth - rect.Width) / 2;
                float centerPosY = (imageHeight - rect.Height) / 2;

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif


                centerPosX -= rect.X;
                centerPosY -= rect.Y;


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

            int imageWidth = stamp.Size.Width + stamp.Margin.LeftRight * 2;
            int imageHeight = stamp.Size.Height + stamp.Margin.TopBottom * 2;

            Pen edgePen = new Pen(stamp.Color)
            {
                Width = stamp.EdgeWidth
            };

            if (stamp.Size.Width != stamp.Size.Height)
            {
                Size rotatedSize = new Size(imageWidth, imageHeight).GetRotatedContainerSize(stamp.RotationAngle);
                imageWidth = rotatedSize.Width;
                imageHeight = rotatedSize.Height;
            }

            Bitmap stampImage = new Bitmap(imageWidth, imageHeight);

            Graphics graphics = Graphics.FromImage(stampImage);

            graphics.SmoothingMode = SmoothingMode.HighQuality;

            // 回転
            int halfImageWidth = imageWidth / 2;
            int halfImageHeight = imageHeight / 2;

            graphics.TranslateTransform(-halfImageWidth, -halfImageHeight);
            graphics.RotateTransform(-stamp.RotationAngle, MatrixOrder.Append);
            graphics.TranslateTransform(halfImageWidth, halfImageHeight, MatrixOrder.Append);


            int stampWidth = stamp.Size.Width;
            int stampHeight = stamp.Size.Height;


            int outerSpaceX = (imageWidth - stamp.Size.Width) / 2;
            int outerSpaceY = (imageHeight - stamp.Size.Height) / 2;


            // 2重円
            if (stamp.EdgeType == StampEdgeType.Double)
            {
                // 外円描画
                graphics.DrawEllipse(edgePen, outerSpaceX, outerSpaceY, stampWidth, stampHeight);


                // 内円の設定へ更新
                stampWidth -= stamp.DoubleEdgeOffset * 2;
                stampHeight -= stamp.DoubleEdgeOffset * 2;
                outerSpaceX += stamp.DoubleEdgeOffset;
                outerSpaceY += stamp.DoubleEdgeOffset;

            }

            // 印鑑の縁
            graphics.DrawEllipse(edgePen, outerSpaceX, outerSpaceY, stampWidth, stampHeight);

            if (stamp.IsFillColor)
            {
                using (var fillBrush = new SolidBrush(stamp.Color))
                {
                    graphics.FillEllipse(fillBrush, outerSpaceX, outerSpaceY, stampWidth, stampHeight);
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

                float centerPosX = (imageWidth - rect.Width) / 2;
                float centerPosY = (imageHeight - rect.Height) / 2;

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    using (var pen = GetDebugPen())
                    {
                        graphics.DrawRectangle(pen, centerPosX, centerPosY, rect.Width, rect.Height);
                    }
                }
#endif


                centerPosX -= rect.X;
                centerPosY -= rect.Y;


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


                int xInit = rand.Next(grungeBitmap.Size.Width);
                int yInit = rand.Next(grungeBitmap.Size.Height);

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
