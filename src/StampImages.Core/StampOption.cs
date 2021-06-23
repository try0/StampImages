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
        public Pen Pen { get; set; } = new Pen(Color.Red)
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

        public int DoubleStampEdgeOffset { get; set; } = 10;

        public void Dispose()
        {
            Pen.Dispose();
        }
    }
}
