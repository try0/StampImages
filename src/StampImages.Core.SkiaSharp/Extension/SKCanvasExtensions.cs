using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;

using System.Runtime.InteropServices;

namespace StampImages.Core.SkiaSharp
{
    /// <summary>
    /// <see cref="SKCanvas"/> 拡張
    /// </summary>
    public static class SKCanvasExtensions
    {
        // TODO ポインター操作で高速化

        /// <summary>
        /// <see cref="SKCanvas.DrawText(string, float, float, SKPaint)"/>で描画した文字列を測定します。
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <param name="paint"></param>
        /// <returns></returns>
        public static RectangleF MeasureDrawedString(this SKCanvas graphics, string text, Size size, SKPaint paint)
        {
            if (string.IsNullOrEmpty(text))
            {
                return RectangleF.Empty;
            }

            using (var tmpBitmap = new SKBitmap(size.Width, size.Height))
            using (SKCanvas tmpGraphics = new SKCanvas(tmpBitmap))
            {

                tmpGraphics.Clear(SKColors.White);

                // 背景色保持
                Color backgroundColor = Color.White;
                byte bgR = backgroundColor.R;
                byte bgG = backgroundColor.G;
                byte bgB = backgroundColor.B;


                // 実際に文字列を描画する
                tmpGraphics.DrawText(text, size.Width / 2, size.Height / 2, paint);


                // 配列へ展開


                // 左側
                int leftPos = -1;
                for (int x = 0; x < tmpBitmap.Width; x++)
                {
                    for (int y = 0; y < tmpBitmap.Height; y++)
                    {
                        SKColor color = tmpBitmap.GetPixel(x, y);

                        // RGB
                        byte b = color.Blue;
                        byte g = color.Green;
                        byte r = color.Red;

                        bool isSameColor = r == bgR && g == bgG && b == bgB;
                        if (!isSameColor)
                        {
                            leftPos = x;
                            break;
                        }
                    }
                    if (0 <= leftPos)
                    {
                        break;
                    }
                }

                // 右側
                int rightPos = -1;
                for (int x = tmpBitmap.Width - 1; leftPos < x; x--)
                {
                    for (int y = 0; y < tmpBitmap.Height; y++)
                    {
                        SKColor color = tmpBitmap.GetPixel(x, y);

                        // RGB
                        byte b = color.Blue;
                        byte g = color.Green;
                        byte r = color.Red;

                        bool isSameColor = r == bgR && g == bgG && b == bgB;
                        if (!isSameColor)
                        {
                            rightPos = x;
                            break;
                        }
                    }
                    if (0 <= rightPos)
                    {
                        break;
                    }
                }
                if (rightPos < 0)
                {
                    rightPos = leftPos;
                }

                // 上部
                int topPos = -1;
                for (int y = 0; y < tmpBitmap.Height; y++)
                {
                    for (int x = leftPos; x <= rightPos; x++)
                    {
                        SKColor color = tmpBitmap.GetPixel(x, y);

                        // RGB
                        byte b = color.Blue;
                        byte g = color.Green;
                        byte r = color.Red;

                        bool isSameColor = r == bgR && g == bgG && b == bgB;
                        if (!isSameColor)
                        {
                            topPos = y;
                            break;
                        }
                    }
                    if (0 <= topPos)
                    {
                        break;
                    }
                }

                // 下部
                int bottomPos = -1;
                for (int y = tmpBitmap.Height - 1; topPos < y; y--)
                {
                    for (int x = leftPos; x <= rightPos; x++)
                    {
                        SKColor color = tmpBitmap.GetPixel(x, y);

                        // RGB
                        byte b = color.Blue;
                        byte g = color.Green;
                        byte r = color.Red;


                        bool isSameColor = r == bgR && g == bgG && b == bgB;
                        if (!isSameColor)
                        {
                            bottomPos = y;
                            break;
                        }
                    }
                    if (0 <= bottomPos)
                    {
                        break;
                    }
                }
                if (bottomPos < 0)
                {
                    bottomPos = topPos;
                }

                int stringX = leftPos;
                int stringY = topPos;
                int stringWidth = rightPos - leftPos;
                int stringHeight = bottomPos - topPos;

                return new Rectangle(stringX, stringY, stringWidth, stringHeight);
            }
        }

        // 縦書き用のオプションないっぽい？　見つけられなかった

        /// <summary>
        /// 複数行でテキストを描画します。
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="text"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="paint"></param>
        public static void DrawMutiLineText(this SKCanvas canvas, string text, float cx, float cy, SKPaint paint)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            IList<string> lines = new List<string>();
            foreach (string line in text.Split('\n'))
            {
                lines.Add(line);
            }

            // 一文字目初期値
            float halfTextHeight = paint.TextSize / 2;
            cy -= halfTextHeight * (lines.Count - 1);

            foreach (string line in lines)
            {
                canvas.DrawText(line, cx, cy, paint);
                cy += paint.TextSize;
            }

        }
    }
}
