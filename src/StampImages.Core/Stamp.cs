using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// スタンプ
    /// </summary>
    public class Stamp : IDisposable
    {
        /// <summary>
        /// 朱色
        /// </summary>
        public static readonly Color DEFAULT_STAMP_COLOR = Color.FromArgb(239, 69, 74);

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
        /// スタンプ設定
        /// </summary>
        public StampOption Option { get; set; } = new StampOption();


        public void SetFont(Font font)
        {
            if (TopText != null)
            {
                TopText.Font = font;
            }
            if (MiddleText != null)
            {
                MiddleText.Font = font;
            }
            if (BottomText != null)
            {
                BottomText.Font = font;
            }
        }

        public void SetBrush(Brush brush)
        {
            if (TopText != null)
            {
                TopText.Brush = brush;
            }
            if (MiddleText != null)
            {
                MiddleText.Brush = brush;
            }
            if (BottomText != null)
            {
                BottomText.Brush = brush;
            }
        }

        public void Dispose()
        {
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

            if (Option != null)
            {
                Option.Dispose();
            }
        }
    }
}
