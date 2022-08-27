using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// スタンプテキスト
    /// </summary>
    public class StampText
    {
        public static readonly string DEFAULT_FONT_FAMILY = "MS UI Gothic";

        /// <summary>
        /// 出力対象文字列
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// emSize
        /// </summary>
        public float Size { get; set; } = 27;
        /// <summary>
        /// フォント
        /// </summary>
        public string FontFamily { get; set; } = DEFAULT_FONT_FAMILY;


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

    }
}
