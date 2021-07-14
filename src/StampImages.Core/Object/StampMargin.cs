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
        public int TopBottom { get; set; }

        /// <summary>
        /// 左右スペース
        /// </summary>
        public int LeftRight { get; set; }

        /// <summary>
        /// 余白情報を生成します。
        /// </summary>
        public StampMargin()
        {
            TopBottom = 0;
            LeftRight = 0;
        }

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
