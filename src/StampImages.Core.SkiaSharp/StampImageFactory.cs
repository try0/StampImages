using SkiaSharp;
using StampImages.Core;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StampImages.Core.SkiaSharp
{

    /// <summary>
    /// 職印画像生成処理
    /// </summary>
    public class StampImageFactory : IDisposable, IStampImageFactory<SKBitmap>
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
        public SKBitmap Resize(SKBitmap src, int width, int height)
        {
            StampUtils.RequiredArgument(src, "src");
            SKBitmap resize = src.Resize(new SKImageInfo(width, height), SKFilterQuality.High);

            return resize;
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

            using (SKBitmap bitmap = Create(stamp))
            {

                if (!fileName.ToLower().EndsWith(".png"))
                {
                    fileName += ".png";
                }

                var image = SKImage.FromBitmap(bitmap);

                using (var stream = File.Create(fileName))
                {
                    var data = image.Encode(SKEncodedImageFormat.Png, 100);
                    data.SaveTo(stream);
                }

            }

        }

        /// <summary>
        /// イメージを作成します
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public SKBitmap Create(BaseStamp stamp)
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
        public SKBitmap Create(ThreeAreaCircularStamp stamp)
        {
            StampUtils.RequiredArgument(stamp, "stamp");


            int imageWidth = stamp.Size.Width + stamp.Margin.LeftRight * 2;
            int imageHeight = stamp.Size.Height + stamp.Margin.TopBottom * 2;

            SKPaint edgePaint = new SKPaint();
            edgePaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
            edgePaint.StrokeWidth = stamp.EdgeWidth;
            edgePaint.Style = SKPaintStyle.Stroke;



            SKBitmap stampImage = new SKBitmap(imageWidth, imageHeight);

            SKCanvas graphics = new SKCanvas(stampImage);
            graphics.Clear(SKColors.Transparent);


            // 回転
            int halfImageWidth = imageWidth / 2;
            int halfImageHeight = imageHeight / 2;

            graphics.Translate(-halfImageWidth, -halfImageHeight);
            graphics.RotateDegrees(-stamp.RotationAngle);
            graphics.Translate(halfImageWidth, halfImageHeight);



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
                graphics.DrawOval(imageWidth / 2, imageHeight / 2, r - outerSpaceX, r - outerSpaceY, edgePaint);

                // 内円の設定へ更新
                r -= stamp.DoubleEdgeOffset;
                outerSpaceX += stamp.DoubleEdgeOffset;
                outerSpaceY += stamp.DoubleEdgeOffset;

            }

            // 分割ライン描画用の角度　高さや弦の算出基準
            int angle = 15;

            // 印鑑の縁
            graphics.DrawOval(imageWidth / 2, imageHeight / 2, r - outerSpaceX, r - outerSpaceY, edgePaint);

            if (stamp.IsFillColor)
            {
                using (SKPaint fillEdgePaint = new SKPaint())
                {
                    fillEdgePaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
                    fillEdgePaint.Style = SKPaintStyle.Fill;

                    graphics.DrawOval(imageWidth / 2, imageHeight / 2, r - outerSpaceX, r - outerSpaceY, fillEdgePaint);
                }

            }


            // 弦（上下の分割線長）
            int chord = (int)Math.Round(2 * r * Math.Sin(StampUtils.ConvertToRadian(90 - angle)));
            int y = (int)Math.Round(r * Math.Sin(StampUtils.ConvertToRadian(angle)));

            // 分割ラインと縁が重なる点からのスペース
            int space = (r * 2 - chord) / 2;

            int topLineY = imageHeight / 2 - y;
            int bottomLineY = imageHeight / 2 + y;

            SKPaint dividerPaint = new SKPaint();

            dividerPaint.StrokeWidth = stamp.DividerWidth;
            dividerPaint.Style = SKPaintStyle.Stroke;
            if (stamp.IsFillColor)
            {
                dividerPaint.Color = SKColors.Transparent;
                dividerPaint.BlendMode = SKBlendMode.DstIn;
            }
            else
            {
                dividerPaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
            }

            if (stamp.IsFillColor)
            {
                space -= Math.Max((int)(stamp.DoubleEdgeOffset * 0.6), 0);
            }


            // 上部ライン
            graphics.DrawLine(space + outerSpaceX + stamp.EdgeWidth, topLineY, imageWidth - (space + outerSpaceX) - stamp.EdgeWidth, topLineY, dividerPaint);
            // 下部ライン
            graphics.DrawLine(space + outerSpaceX + stamp.EdgeWidth, bottomLineY, imageWidth - (space + outerSpaceX) - stamp.EdgeWidth, bottomLineY, dividerPaint);




            float stringX = imageWidth / 2;
            // 上段テキスト
            StampText topText = stamp.TopText;
            if (topText != null)
            {

                using (SKPaint fontPaint = new SKPaint())
                {

                    fontPaint.Typeface = SKTypeface.FromFamilyName(
                        topText.FontFamily,
                        SKFontStyleWeight.Normal,
                        SKFontStyleWidth.Normal,
                        SKFontStyleSlant.Upright);
                    fontPaint.TextSize = topText.Size;
                    fontPaint.TextAlign = SKTextAlign.Center;
                    if (stamp.IsFillColor)
                    {
                        fontPaint.Color = SKColors.Transparent;
                        fontPaint.BlendMode = SKBlendMode.DstIn;
                    }
                    else
                    {
                        fontPaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
                    }

                    // 実際に描画して、中央に来るよう調整
                    RectangleF rect = graphics.MeasureDrawedString(topText.Value, stamp.Size, fontPaint);
                    float centerPosX = (imageWidth / 2) + (stamp.Size.Width / 2) - (rect.X + (rect.Width / 2));
                    float centerPosY = (topLineY - stamp.TopBottomTextOffset - rect.Height) + (stamp.Size.Height / 2) - (rect.Y + (rect.Height / 2));

                    graphics.DrawText(topText.Value, centerPosX, centerPosY, fontPaint);
                }

            }

            // 中段テキスト
            StampText middleText = stamp.MiddleText;
            if (middleText != null)
            {
                using (SKPaint fontPaint = new SKPaint())
                {
                    fontPaint.Typeface = SKTypeface.FromFamilyName(
                        middleText.FontFamily,
                        SKFontStyleWeight.Normal,
                        SKFontStyleWidth.Normal,
                        SKFontStyleSlant.Upright);
                    fontPaint.TextSize = middleText.Size;
                    fontPaint.TextAlign = SKTextAlign.Center;
                    if (stamp.IsFillColor)
                    {
                        fontPaint.Color = SKColors.Transparent;
                        fontPaint.BlendMode = SKBlendMode.DstIn;
                    }
                    else
                    {
                        fontPaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
                    }

                    // 実際に描画して、中央に来るよう調整
                    RectangleF rect = graphics.MeasureDrawedString(middleText.Value, stamp.Size, fontPaint);
                    float centerPosX = (imageWidth / 2) + (stamp.Size.Width / 2) - (rect.X + (rect.Width / 2));
                    float centerPosY = (imageHeight / 2) + (stamp.Size.Height / 2) - (rect.Y + (rect.Height / 2));

                    graphics.DrawText(middleText.Value, centerPosX, centerPosY, fontPaint);
                }
            }

            // 下段テキスト
            StampText bottomText = stamp.BottomText;
            if (bottomText != null)
            {

                using (SKPaint fontPaint = new SKPaint())
                {
                    fontPaint.Typeface = SKTypeface.FromFamilyName(
                        bottomText.FontFamily,
                        SKFontStyleWeight.Normal,
                        SKFontStyleWidth.Normal,
                        SKFontStyleSlant.Upright);
                    fontPaint.TextSize = bottomText.Size;
                    fontPaint.TextAlign = SKTextAlign.Center;
                    if (stamp.IsFillColor)
                    {
                        fontPaint.Color = SKColors.Transparent;
                        fontPaint.BlendMode = SKBlendMode.DstIn;
                    }
                    else
                    {
                        fontPaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
                    }

                    // 実際に描画して、中央に来るよう調整
                    RectangleF rect = graphics.MeasureDrawedString(bottomText.Value, stamp.Size, fontPaint);
                    float centerPosX = (imageWidth / 2) + (stamp.Size.Width / 2) - (rect.X + (rect.Width / 2));
                    float centerPosY = bottomLineY + stamp.TopBottomTextOffset + (rect.Height) + (stamp.Size.Height / 2) - (rect.Y + (rect.Height / 2));

                    graphics.DrawText(bottomText.Value, centerPosX, centerPosY, fontPaint);
                }
            }





            AppendEffects(stamp, stampImage);

            edgePaint.Dispose();
            graphics.Dispose();

            return stampImage;
        }

        /// <summary>
        /// 4角形の印鑑イメージを作成します。
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public SKBitmap Create(RectangleStamp stamp)
        {
            StampUtils.RequiredArgument(stamp, "stamp");

            int imageWidth = stamp.Size.Width + stamp.Margin.LeftRight * 2;
            int imageHeight = stamp.Size.Height + stamp.Margin.TopBottom * 2;

            SKPaint edgePaint = new SKPaint();
            edgePaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
            edgePaint.StrokeWidth = stamp.EdgeWidth;
            edgePaint.Style = SKPaintStyle.Stroke;



            // 回転後にひつような描画サイズへ更新
            Size rotatedSize = new Size(imageWidth, imageHeight).GetRotatedContainerSize(stamp.RotationAngle);
            imageWidth = rotatedSize.Width;
            imageHeight = rotatedSize.Height;

            SKBitmap stampImage = new SKBitmap(rotatedSize.Width, rotatedSize.Height);

            SKCanvas graphics = new SKCanvas(stampImage);
            graphics.Clear(SKColors.Transparent);


            // 回転
            int halfImageWidth = imageWidth / 2;
            int halfImageHeight = imageHeight / 2;


            graphics.Translate(-halfImageWidth, -halfImageHeight);
            graphics.RotateDegrees(-stamp.RotationAngle);
            graphics.Translate(halfImageWidth, halfImageHeight);


            int stampWidth = stamp.Size.Width;
            int stampHeight = stamp.Size.Height;

            int outerSpaceX = (imageWidth - stamp.Size.Width) / 2;
            int outerSpaceY = (imageHeight - stamp.Size.Height) / 2;

            int edgeRadius = stamp.EdgeRadius;

            // 2重
            if (stamp.EdgeType == StampEdgeType.Double)
            {
                // 外描画
                SKRect rect = new SKRect(outerSpaceX, outerSpaceY, outerSpaceX + stampWidth, outerSpaceY + stampHeight);
                SKRoundRect roundRect = new SKRoundRect(rect, edgeRadius);
                graphics.DrawRoundRect(roundRect, edgePaint);

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
                using (SKPaint fillEdgePaint = new SKPaint())
                {
                    fillEdgePaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
                    fillEdgePaint.Style = SKPaintStyle.Fill;

                    SKRect rect = new SKRect(outerSpaceX, outerSpaceY, outerSpaceX + stampWidth, outerSpaceY + stampHeight);
                    SKRoundRect roundRect = new SKRoundRect(rect, edgeRadius);
                    graphics.DrawRoundRect(roundRect, fillEdgePaint);
                }
            }
            else
            {
                SKRect rect = new SKRect(outerSpaceX, outerSpaceY, outerSpaceX + stampWidth, outerSpaceY + stampHeight);
                SKRoundRect roundRect = new SKRoundRect(rect, edgeRadius);
                graphics.DrawRoundRect(roundRect, edgePaint);
            }


            StampText stampText = stamp.Text;
            if (stampText != null)
            {

                using (SKPaint fontPaint = new SKPaint())
                {
                    fontPaint.Typeface = SKTypeface.FromFamilyName(
                        stampText.FontFamily,
                        SKFontStyleWeight.Normal,
                        SKFontStyleWidth.Normal,
                        SKFontStyleSlant.Upright);
                    fontPaint.TextSize = stampText.Size;
                    fontPaint.TextAlign = SKTextAlign.Center;
                    if (stamp.IsFillColor)
                    {
                        fontPaint.Color = SKColors.Transparent;
                        fontPaint.BlendMode = SKBlendMode.DstIn;
                    }
                    else
                    {
                        fontPaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
                    }

                    // 実際に描画して、中央に来るよう調整
                    RectangleF rect = graphics.MeasureDrawedString(stampText.Value, stamp.Size, fontPaint);
                    float centerPosX = (imageWidth / 2) + (stamp.Size.Width / 2) - (rect.X + (rect.Width / 2));
                    float centerPosY = (imageHeight / 2) + (stamp.Size.Height / 2) - (rect.Y + (rect.Height / 2));

                    if (stamp.TextOrientationType == TextOrientationType.Vertical)
                    {
                        char[] chars = stampText.Value.ToCharArray();
                        var multiline = string.Join("\n", chars);
                        graphics.DrawMutiLineText(multiline, centerPosX, centerPosY, fontPaint);
                    }
                    else
                    {

                        graphics.DrawText(stampText.Value, centerPosX, centerPosY, fontPaint);
                    }
                }
            }



            AppendEffects(stamp, stampImage);

            edgePaint.Dispose();
            graphics.Dispose();

            return stampImage;
        }

        /// <summary>
        /// 円形の印鑑イメージを作成します。
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public SKBitmap Create(CircularStamp stamp)
        {
            StampUtils.RequiredArgument(stamp, "stamp");

            int imageWidth = stamp.Size.Width + stamp.Margin.LeftRight * 2;
            int imageHeight = stamp.Size.Height + stamp.Margin.TopBottom * 2;

            SKPaint edgePaint = new SKPaint();
            edgePaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
            edgePaint.StrokeWidth = stamp.EdgeWidth;
            edgePaint.Style = SKPaintStyle.Stroke;

            if (stamp.Size.Width != stamp.Size.Height)
            {
                Size rotatedSize = new Size(imageWidth, imageHeight).GetRotatedContainerSize(stamp.RotationAngle);
                imageWidth = rotatedSize.Width;
                imageHeight = rotatedSize.Height;
            }

            SKBitmap stampImage = new SKBitmap(imageWidth, imageHeight);

            SKCanvas graphics = new SKCanvas(stampImage);
            graphics.Clear(SKColors.Transparent);

            // 回転
            int halfImageWidth = imageWidth / 2;
            int halfImageHeight = imageHeight / 2;

            graphics.Translate(-halfImageWidth, -halfImageHeight);
            graphics.RotateDegrees(-stamp.RotationAngle);
            graphics.Translate(halfImageWidth, halfImageHeight);


            int stampWidth = stamp.Size.Width;
            int stampHeight = stamp.Size.Height;


            int outerSpaceX = (imageWidth - stamp.Size.Width) / 2;
            int outerSpaceY = (imageHeight - stamp.Size.Height) / 2;


            // 2重円
            if (stamp.EdgeType == StampEdgeType.Double)
            {
                // 外円描画
                graphics.DrawOval(imageWidth / 2, imageHeight / 2, stampWidth / 2 - outerSpaceX, stampHeight / 2 - outerSpaceY, edgePaint);


                // 内円の設定へ更新
                stampWidth -= stamp.DoubleEdgeOffset * 2;
                stampHeight -= stamp.DoubleEdgeOffset * 2;
                outerSpaceX += stamp.DoubleEdgeOffset;
                outerSpaceY += stamp.DoubleEdgeOffset;

            }

            // 印鑑の縁
            graphics.DrawOval(imageWidth / 2, imageHeight / 2, stampWidth / 2 - outerSpaceX, stampHeight / 2 - outerSpaceY, edgePaint);

            if (stamp.IsFillColor)
            {
                using (SKPaint fillEdgePaint = new SKPaint())
                {
                    fillEdgePaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
                    fillEdgePaint.Style = SKPaintStyle.Fill;

                    graphics.DrawOval(imageWidth / 2, imageHeight / 2, stampWidth / 2 - outerSpaceX, stampHeight / 2 - outerSpaceY, fillEdgePaint);
                }
            }


            StampText stampText = stamp.Text;
            if (stampText != null)
            {

                using (SKPaint fontPaint = new SKPaint())
                {
                    fontPaint.Typeface = SKTypeface.FromFamilyName(
                        stampText.FontFamily,
                        SKFontStyleWeight.Normal,
                        SKFontStyleWidth.Normal,
                        SKFontStyleSlant.Upright);
                    fontPaint.TextSize = stampText.Size;
                    fontPaint.TextAlign = SKTextAlign.Center;
                    if (stamp.IsFillColor)
                    {
                        fontPaint.Color = SKColors.Transparent;
                        fontPaint.BlendMode = SKBlendMode.DstIn;
                    }
                    else
                    {
                        fontPaint.Color = new SKColor(stamp.Color.R, stamp.Color.G, stamp.Color.B);
                    }

                    // 実際に描画して、中央に来るよう調整
                    RectangleF rect = graphics.MeasureDrawedString(stampText.Value, stamp.Size, fontPaint);
                    float centerPosX = (imageWidth / 2) + (stamp.Size.Width / 2) - (rect.X + (rect.Width / 2));
                    float centerPosY = (imageHeight / 2) + (stamp.Size.Height / 2) - (rect.Y + (rect.Height / 2));

                    if (stamp.TextOrientationType == TextOrientationType.Vertical)
                    {
                        char[] chars = stampText.Value.ToCharArray();
                        var multiline = string.Join("\n", chars);
                        graphics.DrawMutiLineText(multiline, centerPosX, centerPosY, fontPaint);
                    }
                    else
                    {
                        graphics.DrawText(stampText.Value, centerPosX, centerPosY, fontPaint);
                    }
                }
            }




            AppendEffects(stamp, stampImage);


            edgePaint.Dispose();
            graphics.Dispose();

            return stampImage;
        }

        /// <summary>
        /// 加工処理を実行します
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="stampImage"></param>
        static void AppendEffects(BaseStamp stamp, SKBitmap stampImage)
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
        static void AppendNoise(BaseStamp stamp, SKBitmap stampImage)
        {
            Random rand = new Random();


            for (int x = 0; x < stampImage.Width; x++)
            {
                for (int y = 0; y < stampImage.Height; y++)
                {
                    SKColor color = stampImage.GetPixel(x, y);

                    if (color.Alpha == 0)
                    {
                        continue;
                    }

                    if (1 > rand.Next(5))
                    {
                        var a = (byte)rand.Next(64);

                        stampImage.SetPixel(x, y, new SKColor(color.Red, color.Green, color.Blue, a));
                    }

                }
            }

        }

        /// <summary>
        /// 汚し加工を付与します。
        /// <para><see href="https://jp.freepik.com/photos/grunge"/>Freepik.comのリソースを使用しています</para>
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="stampImage"></param>
        static void AppendGrunge(BaseStamp stamp, SKBitmap stampImage)
        {
            Random rand = new Random();

            Assembly assembly = typeof(IStampImageFactory<>).Assembly;
            var stream = assembly.GetManifestResourceStream(@"StampImages.Core.Resource.effect_grunge.jpg");
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                using (var grungeBitmap = SKBitmap.Decode(ms.ToArray()))
                {

                    // 配列へ展開


                    int xInit = rand.Next(grungeBitmap.Width);
                    int yInit = rand.Next(grungeBitmap.Height);

                    int grungeLength = grungeBitmap.Width * grungeBitmap.Height;

                    for (int x = xInit; x < xInit + stampImage.Width; x++)
                    {
                        for (int y = yInit; y < yInit + stampImage.Height; y++)
                        {
                            SKColor stampColor = stampImage.GetPixel(x - xInit, y - yInit);
                            SKColor grungeColor = grungeBitmap.GetPixel(x % grungeBitmap.Width, y % grungeBitmap.Height);

                            // RGB
                            byte b = grungeColor.Blue;
                            byte g = grungeColor.Green;
                            byte r = grungeColor.Red;

                            if (stampColor.Alpha != 0)
                            {
                                var a = (byte)((r + g + b) / 3);
                                stampImage.SetPixel((x - xInit), (y - yInit), new SKColor(stampColor.Red, stampColor.Green, stampColor.Blue, a));
                            }

                        }
                    }
                }

            }
        }



        /// <inheritdoc />
        public void Dispose()
        {
            Config.Dispose();
        }

    }




}
