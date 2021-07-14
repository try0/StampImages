using System;
using System.Collections.Generic;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// 余白情報
    /// </summary>
    public class StampMargin
    {
        /// <summary>
        /// 上下スペース
        /// </summary>
        public int TopBottom { get; private set; }

        /// <summary>
        /// 左右スペース
        /// </summary>
        public int LeftRight { get; private set; }

        /// <summary>
        /// 余白情報を生成します。
        /// </summary>
        /// <param name="topBottom"></param>
        /// <param name="leftRight"></param>
        public StampMargin(int topBottom, int leftRight)
        {
            TopBottom = topBottom;
            LeftRight = leftRight;
        }
    }
}
