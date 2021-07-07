using StampImages.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace StampImages.WebApp.Blazor.Models
{
    /// <summary>
    /// スタンプモデル
    /// </summary>
    public class StampModel
    {
        /// <summary>
        /// 上段テキスト
        /// </summary>
        public string TopText { get; set; } = "StampImages";

        /// <summary>
        /// 中段テキスト
        /// </summary>
        public string MiddleText { get; set; } = DateTime.Now.ToString("yyyy.MM.dd");

        /// <summary>
        /// 下段テキスト
        /// </summary>
        public string BottomText { get; set; } = DateTime.Now.ToString("HH:mm:ss");

        /// <summary>
        /// 上段テキストフォントサイズ
        /// </summary>
        public int TopFontSize { get; set; } = 23;

        /// <summary>
        /// 中段テキストフォントサイズ
        /// </summary>
        public int MiddleFontSize { get; set; } = 30;

        /// <summary>
        /// 下段テキストフォントサイズ
        /// </summary>
        public int BottomFontSize { get; set; } = 27;

        /// <summary>
        /// スタンプカラー
        /// </summary>
        public string ColorHexValue { get; set; } = ColorTranslator.ToHtml(BaseStamp.DEFAULT_STAMP_COLOR);

        /// <summary>
        /// スタンプカラー
        /// </summary>
        public Color Color => ColorTranslator.FromHtml(ColorHexValue);

        /// <summary>
        /// エッジタイプ
        /// </summary>
        public StampEdgeType EdgeType { get; set; } = StampEdgeType.Single;

        public StampModel()
        {
        }


    }
}
