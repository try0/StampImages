using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace StampImages.Core.SkiaSharp
{
    /// <summary>
    /// ファクトリーコンフィグ
    /// </summary>
    public class StampImageFactoryConfig : IStampImageFactoryConfig
    {
        public Func<string, SKTypeface> SKTypefaceProvider { get; set; } = fontFamilyName => SKTypeface.FromFamilyName(fontFamilyName);

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
