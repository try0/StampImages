using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;

namespace StampImages
{
    public class StampText : IDisposable
    {
        /// <summary>
        /// MS UI Gothic
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Font GetDefaultFont(int size)
        {
            return new Font("MS UI Gothic", size);
        }

        /// <summary>
        /// <see cref="SolidBrush"/>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Brush GetDefaultBrush(Color color)
        {
            return new SolidBrush(color);
        }



        /// <summary>
        /// 出力対象文字列
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// フォント
        /// </summary>
        public Font Font { get; set; } = GetDefaultFont(22);

        /// <summary>
        /// ブラシ
        /// </summary>
        public Brush Brush { get; set; } = GetDefaultBrush(Color.Red);

        /// <summary>
        /// コンストラクター
        /// </summary>
        public StampText()
        {
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="value"></param>
        public StampText(string value)
        {
            Value = value;
        }

        public void Dispose()
        {
            if (Font != null)
            {
                Font.Dispose();
            }

            if (Brush != null)
            {
                Brush.Dispose();
            }
        }


    }
}
