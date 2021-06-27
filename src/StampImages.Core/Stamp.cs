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
        /// スタンプ縁タイプ
        /// </summary>
        public enum StampEdgeType
        {
            SINGLE,
            DOUBLE
        }

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
        /// 出力画像サイズ
        /// </summary>
        public Size Size { get; set; } = new Size(256, 256);

        /// <summary>
        /// スタンプカラー
        /// </summary>
        public Color Color { get; set; } = DEFAULT_STAMP_COLOR;

        /// <summary>
        /// 縁
        /// </summary>
        public StampEdgeType EdgeType { get; set; } = StampEdgeType.SINGLE;

        /// <summary>
        /// 縁ライン幅
        /// </summary>
        public int EdgeWidth { get; set; } = 5;

        /// <summary>
        /// エリア分割ライン
        /// </summary>
        public int DividerWidth { get; set; } = 5;

        /// <summary>
        /// 分割ラインから上段、下段の文字列までの間隔
        /// </summary>
        public int TopBottomTextOffset { get; set; } = 10;


        /// <summary>
        /// 2重円間隔調整池
        /// </summary>
        public int DoubleEdgeOffset { get; set; } = 10;

        /// <summary>
        /// 回転角度
        /// </summary>
        public int RotationAngle { get; set; } = 0;

        /// <summary>
        /// ノイズ
        /// </summary>
        public bool IsAppendNoise { get; set; } = false;


        public void SetFontFamily(FontFamily fontfamily)
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
        }
    }
}
