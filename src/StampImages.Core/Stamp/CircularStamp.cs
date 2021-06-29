using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// 丸いのスタンプ
    /// </summary>
    public class CircularStamp : BaseStamp
    {
        /// <summary>
        /// テキスト
        /// </summary>
        public StampText Text { get; set; }

        /// <summary>
        /// テキスト向き
        /// </summary>
        public TextOrientationType TextOrientationType { get; set; } = TextOrientationType.HORIZONTAL;


        /// <summary>
        /// コンストラクター
        /// </summary>
        public CircularStamp() : base()
        {
        }

        public override void SetFontFamily(FontFamily fontfamily)
        {
            if (Text != null)
            {
                Text.FontFamily = fontfamily;
            }
        }

    }
}
