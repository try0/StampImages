using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// 設定
    /// </summary>
    public class StampOption : IDisposable
    {
        /// <summary>
        /// 出力画像サイズ
        /// </summary>
        public int ImageEdgeSize { get; set; } = 256;

        /// <summary>
        /// 縁、分割ライン描画用のPen
        /// </summary>
        public Pen Pen { get; set; } = new Pen(Stamp.DEFAULT_STAMP_COLOR)
        {
            Width = 5
        };

        /// <summary>
        /// 分割ラインから上段、下段の文字列までの間隔
        /// </summary>
        public int TopBottomTextOffset { get; set; } = 10;

        /// <summary>
        /// 2重円
        /// </summary>
        public bool IsDoubleStampEdge { get; set; } = false;

        /// <summary>
        /// 2重円間隔調整池
        /// </summary>
        public int DoubleStampEdgeOffset { get; set; } = 10;

        /// <summary>
        /// 回転角度
        /// </summary>
        public int RotationAngle { get; set; } = 0;

        /// <summary>
        /// ノイズ
        /// </summary>
        public bool IsAppendNoise { get; set; } = false;

        public void Dispose()
        {
            Pen.Dispose();
        }
    }
}
