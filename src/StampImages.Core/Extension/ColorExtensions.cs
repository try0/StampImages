using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// <see cref="Color"/> 拡張
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// RGBを反転させた<see cref="Color"/>を取得します。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GetInvertColor(this Color color)
        {
            return Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
        }
    }
}
