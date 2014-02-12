using System;
using System.Globalization;
using System.IO;

namespace SharpFlame.FileIO
{
    static internal class ExtensionsForIO
    {
        public static string ToStringInvariant(this bool Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this byte Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this short Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this int Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this UInt32 Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this float Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this double Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
