using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// 四角形のスタンプ
    /// </summary>
    public class RectangleStamp : BaseStamp
    {
        /// <summary>
        /// テキスト
        /// </summary>
        public StampText Text { get; set; }

        /// <summary>
        /// テキスト向き
        /// </summary>
        public TextOrientationType TextOrientationType { get; set; } = TextOrientationType.Horizontal;

        /// <summary>
        /// コーナーRadius
        /// </summary>
        public int EdgeRadius { get; set; } = 20;


        /// <summary>
        /// コンストラクター
        /// </summary>
        public RectangleStamp() : base()
        {
        }

        /// <inheritdoc />
        public override void SetFontFamily(string fontfamily)
        {
            if (Text != null)
            {
                Text.FontFamily = fontfamily;
            }
        }

    }
}
