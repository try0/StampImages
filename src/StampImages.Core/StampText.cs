using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace StampImages.Core
{
    /// <summary>
    /// スタンプテキスト
    /// </summary>
    public class StampText : IDisposable
    {
        /// <summary>
        /// デフォルトフォントを取得します。
        /// <para>MS UI Gothic</para>
        /// </summary>
        /// <returns></returns>
        public static FontFamily GetDefaultFontFamily() => new FontFamily("MS UI Gothic");


        /// <summary>
        /// 出力対象文字列
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// emSize
        /// </summary>
        public float Size { get; set; } = 27;
        /// <summary>
        /// フォント
        /// </summary>
        public FontFamily FontFamily { get; set; } = GetDefaultFontFamily();
        /// <summary>
        /// Descent領域を無視するか否か
        /// </summary>
        public bool IsIgnoreFontDescent { get; set; } = CultureInfo.CurrentUICulture.ToString().ToLower().Contains("ja");


        /// <summary>
        /// コンストラクター
        /// </summary>
        public StampText()
        {
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="value"></param>
        public StampText(string value)
        {
            Value = value;
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            if (FontFamily != null)
            {
                FontFamily.Dispose();
            }
        }


    }
}
