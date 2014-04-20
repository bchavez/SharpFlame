

using System;
using System.IO;



namespace SharpFlame.Core.Extensions
{
    public static class ExtensionsForString
    {
        /*
         * Returns a string with x chars from the left
         */

        public static string Left(this string input, int len)
        {
            return input.Substring(0, Math.Min(input.Length, len));
        }

        /*
         * Returns a string with x chars from the right
         */

        public static string Right(this string input, int len)
        {
            if ( input.Length <= len )
            {
                return input;
            }
            return input.Substring(input.Length - len, len);
        }

        public static string Format2(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string CombinePathWith(this string path1, string path2, bool endWithPathSeparator = false)
        {
            if ( endWithPathSeparator )
                return Path.Combine(path1, path2) + Path.DirectorySeparatorChar;

            return Path.Combine(path1, path2);
        }
    }

    public static class ExtensionsForDouble
    {
        public static int ToInt(this double d)
        {
            return Convert.ToInt32(d);
        }

        public static int ToInt(this decimal d)
        {
            return Convert.ToInt32(d);
        }

        public static int ToInt(this float f)
        {
            return Convert.ToInt32(f);
        }

        public static double Floor(this double d)
        {
            return Math.Floor(d);
        }

        public static long ToLong(this double d)
        {
            return Convert.ToInt64(d);
        }
    }

    public static class PrintUtil
    {
        public static string Print(Eto.Drawing.Size size)
        {
            return size.ToString();
        }
    }
}