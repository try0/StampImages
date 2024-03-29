﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace StampImages.Core
{
    /// <summary>
    /// <see cref="Graphics"/> 拡張
    /// </summary>
    public static class GraphicsExtensions
    {

        /// <summary>
        /// <see cref="Graphics.DrawString(string, Font, Brush, float, float, StringFormat)"/>で描画した文字列を測定します。
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="size"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static RectangleF MeasureDrawedString(this Graphics graphics, string text, Font font, Size size, StringFormat format)
        {
            if (string.IsNullOrEmpty(text))
            {
                return RectangleF.Empty;
            }

            using (var tmpBitmap = new Bitmap(size.Width, size.Height, graphics))
            using (Graphics tmpGraphics = Graphics.FromImage(tmpBitmap))
            {

                // 背景色保持
                Color backgroundColor = tmpBitmap.GetPixel(0, 0);
                byte bgR = backgroundColor.R;
                byte bgG = backgroundColor.G;
                byte bgB = backgroundColor.B;


                // 実際に文字列を描画する
                tmpGraphics.DrawString(text, font, Brushes.Red, new RectangleF(0f, 0f, tmpBitmap.Width, tmpBitmap.Height), format);


                // 配列へ展開
                PixelFormat pixelFormat = tmpBitmap.PixelFormat;
                int pixelSize = Image.GetPixelFormatSize(pixelFormat) / 8;
                if (pixelSize < 3 || 4 < pixelSize)
                {
                    throw new ArgumentException();
                }

                BitmapData bmpData =
                    tmpBitmap.LockBits(new Rectangle(0, 0, tmpBitmap.Width, tmpBitmap.Height), ImageLockMode.ReadOnly, pixelFormat);

                IntPtr ptr = bmpData.Scan0;
                int bytes = Math.Abs(bmpData.Stride) * tmpBitmap.Height;
                byte[] rgbValues = new byte[bytes];
                Marshal.Copy(ptr, rgbValues, 0, bytes);

                // 左側
                int leftPos = -1;
                for (int x = 0; x < bmpData.Width; x++)
                {
                    for (int y = 0; y < bmpData.Height; y++)
                    {
                        int pos = y * bmpData.Stride + x * pixelSize;

                        // RGB
                        byte b = rgbValues[pos];
                        byte g = rgbValues[pos + 1];
                        byte r = rgbValues[pos + 2];

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
                for (int x = bmpData.Width - 1; leftPos < x; x--)
                {
                    for (int y = 0; y < bmpData.Height; y++)
                    {
                        int pos = y * bmpData.Stride + x * pixelSize;

                        // RGB
                        byte b = rgbValues[pos];
                        byte g = rgbValues[pos + 1];
                        byte r = rgbValues[pos + 2];

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
                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = leftPos; x <= rightPos; x++)
                    {
                        int pos = y * bmpData.Stride + x * pixelSize;

                        // RGB
                        byte b = rgbValues[pos];
                        byte g = rgbValues[pos + 1];
                        byte r = rgbValues[pos + 2];

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
                for (int y = bmpData.Height - 1; topPos < y; y--)
                {
                    for (int x = leftPos; x <= rightPos; x++)
                    {
                        int pos = y * bmpData.Stride + x * pixelSize;

                        // RGB
                        byte b = rgbValues[pos];
                        byte g = rgbValues[pos + 1];
                        byte r = rgbValues[pos + 2];

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

                tmpBitmap.UnlockBits(bmpData);


                int stringX = leftPos;
                int stringY = topPos;
                int stringWidth = rightPos - leftPos;
                int stringHeight = bottomPos - topPos;

                return new Rectangle(stringX, stringY, stringWidth, stringHeight);
            }
        }

        /// <summary>
        /// 塗りつぶした丸角の四角形を描画します。
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="brush"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        public static void FillRoundedRectangle(this Graphics graphics, SolidBrush brush, int x, int y, int width, int height, int radius)
        {

            if (radius == 0)
            {
                graphics.FillRectangle(brush, x, y, width, height);
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
            graphics.FillPath(brush, path);
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
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, int x, int y, int width, int height, int radius)
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
    }
}
