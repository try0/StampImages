using System;
using System.Collections.Generic;
using System.Text;

namespace StampImages
{
    internal static class StampUtils
    {
        public static bool IsDebug()
        {
            return false;
        }

        public static void RequiredArgument(object obj, string argname)
        {

            if (obj == null)
            {
                throw new ArgumentNullException($"{argname} is required");
            }

        }

        public static double ConvertToRadian(double angle)
        {
            return angle * (Math.PI / 180);
        }
    }
}
