using StampImages.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace StampImages.Core
{

    /// <summary>
    /// 職印画像生成処理
    /// </summary>
    public class StampImageFactory : IDisposable
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

        public void Save(string topString, string middleString, string bottomString, string fileName)
        {
            StampUtils.RequiredArgument(fileName, "fileName");

            var texts = new Stamp
            {
                TopText = new StampText(topString),
                MiddleText = new StampText(middleString),
                BottomText = new StampText(bottomString)
            };

            Save(texts, fileName);
        }

        public void Save(Stamp stamp, string fileName)
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
        /// 
        /// </summary>
        /// <param name="middleString"></param>
        /// <returns></returns>
        public Bitmap Create(string middleString)
        {
            return Create(new Stamp { MiddleText = new StampText(middleString) });
        }

        /// <summary>
        /// 垂直に3分割した印鑑イメージを作成します。
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        public Bitmap Create(Stamp stamp)
        {
            StampUtils.RequiredArgument(stamp, "stamp");

            int edgeSize = stamp.Option.ImageEdgeSize;
            Pen pen = stamp.Option.Pen;

            Bitmap stampImage = new Bitmap(edgeSize, edgeSize);

            Graphics graphics = Graphics.FromImage(stampImage);

            graphics.SmoothingMode = SmoothingMode.HighQuality;

            // 回転
            int halfImageSize = stamp.Option.ImageEdgeSize / 2;

            graphics.TranslateTransform(-halfImageSize, -halfImageSize);
            graphics.RotateTransform(-stamp.Option.RotationAngle, System.Drawing.Drawing2D.MatrixOrder.Append);
            graphics.TranslateTransform(halfImageSize, halfImageSize, System.Drawing.Drawing2D.MatrixOrder.Append);

            // 半径
            int r = (edgeSize - (edgeSize / 20)) / 2;

            //int r = 120;
            // 画像の縁からスタンプの縁までの上下左右の最も短い絶対値
            int outerSpace = (edgeSize - (r * 2)) / 2;


            // 2重円
            if (stamp.Option.IsDoubleStampEdge)
            {
                // 外円描画
                graphics.DrawEllipse(pen, outerSpace, outerSpace, 2 * r, 2 * r);


                // 内円の設定へ更新
                r -= stamp.Option.DoubleStampEdgeOffset;
                outerSpace += stamp.Option.DoubleStampEdgeOffset;

            }

            // 分割ライン描画用の角度　高さや弦の算出基準
            int angle = 15;

            // 印鑑の縁
            graphics.DrawEllipse(pen, outerSpace, outerSpace, 2 * r, 2 * r);

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

            int topLineY = edgeSize / 2 - y;
            int bottomLineY = edgeSize / 2 + y;
            // 上部ライン
            graphics.DrawLine(pen, space + outerSpace, topLineY, edgeSize - (space + outerSpace), topLineY);
            // 下部ライン
            graphics.DrawLine(pen, space + outerSpace, bottomLineY, edgeSize - (space + outerSpace), bottomLineY);


            StringFormat sf = new StringFormat();
            //sf.FormatFlags = StringFormatFlags.DirectionVertical;
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;


            int stringX = edgeSize / 2;
            // 上段テキスト
            StampText topText = stamp.TopText;
            if (topText != null)
            {
                SizeF topStringSize = graphics.MeasureString(topText.Value, topText.Font);
                int topStringY = (int)Math.Round(topLineY - topStringSize.Height / 2 - stamp.Option.TopBottomTextOffset);
                graphics.DrawString(topText.Value, topText.Font, topText.Brush, stringX, topStringY, sf);


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
                SizeF middleStringSize = graphics.MeasureString(middleText.Value, middleText.Font);
                int middleStringY = edgeSize / 2;
                graphics.DrawString(middleText.Value, middleText.Font, middleText.Brush, stringX, middleStringY, sf);

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
                SizeF bottomStringSize = graphics.MeasureString(bottomText.Value, bottomText.Font);
                int bottomStringY = (int)Math.Round(bottomLineY + bottomStringSize.Height / 2 + stamp.Option.TopBottomTextOffset);
                graphics.DrawString(bottomText.Value, bottomText.Font, bottomText.Brush, stringX, bottomStringY, sf);

#if DEBUG
                if (StampUtils.IsDebug())
                {
                    graphics.DrawRectangle(new Pen(Color.Green), stringX, bottomStringY, bottomStringSize.Width, bottomStringSize.Height);
                }
#endif
            }


            // 背景透過
            stampImage.MakeTransparent();



            if (stamp.Option.IsAppendNoise)
            {
                // TODO 適当だからもっとスタンプ風になる加工あるか調べよ
                Random rand = new Random();

                for (int i = 0; i < stamp.Option.ImageEdgeSize; i++)
                {
                    for (int j = 0; j < stamp.Option.ImageEdgeSize; j++)
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



            graphics.Dispose();

            return stampImage;

        }

        public void Dispose()
        {
            Config.Dispose();
        }
    }
}
