using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// 四角形のスタンプ
    /// </summary>
    public class SquareStamp : BaseStamp
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
        /// コーナーRadius
        /// </summary>
        public int EdgeRadius { get; set; } = 20;


        /// <summary>
        /// コンストラクター
        /// </summary>
        public SquareStamp() : base()
        {
        }

        public override void SetFontFamily(FontFamily fontfamily)
        {
            if (Text != null)
            {
                Text.FontFamily = fontfamily;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (Text != null)
            {
                Text.Dispose();
            }
        }
    }
}
