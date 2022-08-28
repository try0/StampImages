using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace StampImages.Core
{
    /// <summary>
    /// <see cref="Size"/> 拡張
    /// </summary>
    public static class SizeExtensions
    {

        /// <summary>
        /// 回転後のサイズを覆うサイズを取得します。
        /// </summary>
        /// <param name="size"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Size GetRotatedContainerSize(this Size size, int angle)
        {
            if (angle == 0)
            {
                return size;
            }

            angle %= 90;

            int width = (int)Math.Abs(size.Width * Math.Sin(StampUtils.ConvertToRadian(90 - angle)) + Math.Abs(size.Height * Math.Sin(StampUtils.ConvertToRadian(angle)))); 
            int height = (int)Math.Abs(size.Height * Math.Sin(StampUtils.ConvertToRadian(90 - angle)) + Math.Abs(size.Width * Math.Sin(StampUtils.ConvertToRadian(angle))));


            return new Size(width, height);

        }
    }
}
