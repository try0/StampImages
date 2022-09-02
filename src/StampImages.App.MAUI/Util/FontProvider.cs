#if ANDROID
using Android.Content;
using Android.Graphics.Fonts;
using Android.Views;

#elif IOS
using UIKit;

#elif MACCATALYST
using UIKit;

#elif WINDOWS
using Microsoft.Graphics.Canvas.Text;

#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StampImages.App.MAUI.Util
{
    public class FontProvider
    {
        private List<string> fonts = new List<string>();
        private List<string> files = new List<string>();

        public List<string> GetInstalledFontFamilyNames()
        {

            if (fonts.Count > 0)
            {
                return fonts;
            }


#if ANDROID
            Java.IO.File temp = new Java.IO.File("/system/fonts/");
            var names = temp.ListFiles().Select(f => f.Name).Where(fn => fn.EndsWith(".ttf")).Select(fn => fn.Substring(0, fn.LastIndexOf(".ttf")));
            fonts.AddRange(names);

#elif IOS
            fonts.AddRange(UIFont.FamilyNames);

#elif MACCATALYST
            fonts.AddRange(UIFont.FamilyNames);

#elif WINDOWS
            fonts.AddRange(CanvasTextFormat.GetSystemFontFamilies());

#else

#endif

            return fonts;
        }

        public string GetFilePath(string fontName)
        {
#if ANDROID
            Java.IO.File temp = new Java.IO.File("/system/fonts/");
            return temp.ListFiles().Where(f => f.Name == fontName + ".ttf").Select(f => f.ToPath().ToAbsolutePath().ToString()).FirstOrDefault();
#else
            throw new Exception("Unsupported");
#endif

        }

    }
}
