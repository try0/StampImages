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
        /// スタンプカラー
        /// </summary>
        public String ColorHexValue { get; set; } = ColorTranslator.ToHtml(BaseStamp.DEFAULT_STAMP_COLOR);

        /// <summary>
        /// スタンプカラー
        /// </summary>
        public Color Color => ColorTranslator.FromHtml(ColorHexValue);

        /// <summary>
        /// エッジタイプ
        /// </summary>
        public StampEdgeType EdgeType { get; set; } = StampEdgeType.SINGLE;

        public StampModel()
        {
        }


    }
}
