using System;
using System.Collections.Generic;
using System.Drawing;

namespace StampImages.Core
{
    /// <summary>
    /// スタンプ
    /// </summary>
    public abstract partial class BaseStamp : IDisposable
    {

        /// <summary>
        /// 朱色
        /// </summary>
        public static readonly Color DEFAULT_STAMP_COLOR = Color.FromArgb(239, 69, 74);

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
        /// 2重円間隔調整池
        /// </summary>
        public int DoubleEdgeOffset { get; set; } = 10;

        /// <summary>
        /// 回転角度
        /// </summary>
        public int RotationAngle { get; set; } = 0;

        /// <summary>
        /// エフェクトリスト
        /// </summary>
        public ICollection<StampEffectType> EffectTypes { get; set; } = new List<StampEffectType>();

        /// <summary>
        /// 保持する<see cref="StampText"/>のフォントを一括で指定します。
        /// </summary>
        /// <param name="fontFamily"></param>
        public abstract void SetFontFamily(FontFamily fontFamily);

        public virtual void Dispose()
        {
        }
    }
}
