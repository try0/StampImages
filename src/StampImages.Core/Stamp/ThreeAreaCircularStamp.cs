using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// スタンプ
    /// </summary>
    public class ThreeAreaCircularStamp : BaseStamp
    {
        /// <summary>
        /// 上段テキスト
        /// </summary>
        public StampText TopText { get; set; }

        /// <summary>
        /// 中段テキスト
        /// </summary>
        public StampText MiddleText { get; set; }

        /// <summary>
        /// 下段テキスト
        /// </summary>
        public StampText BottomText { get; set; }

        /// <summary>
        /// エリア分割ライン
        /// </summary>
        public int DividerWidth { get; set; } = 5;

        /// <summary>
        /// 分割ラインから上段、下段の文字列までの間隔
        /// </summary>
        public int TopBottomTextOffset { get; set; } = 10;


        public override void SetFontFamily(FontFamily fontfamily)
        {
            if (TopText != null)
            {
                TopText.FontFamily = fontfamily;
            }
            if (MiddleText != null)
            {
                MiddleText.FontFamily = fontfamily;
            }
            if (BottomText != null)
            {
                BottomText.FontFamily = fontfamily;
            }
        }


        public override void Dispose()
        {
            base.Dispose();

            if (TopText != null)
            {
                TopText.Dispose();
            }
            if (MiddleText != null)
            {
                MiddleText.Dispose();
            }
            if (BottomText != null)
            {
                BottomText.Dispose();
            }
        }
    }
}
