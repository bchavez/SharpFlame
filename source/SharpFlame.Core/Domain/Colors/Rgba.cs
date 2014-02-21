using System;
using System.Globalization;
using System.Linq;

namespace SharpFlame.Core.Domain.Colors
{
    public class Rgba : Rgb
    {
        public float Alpha;

        public Rgba( float red, float green, float blue, float alpha )
            : base( red, green, blue )
        {
            Alpha = alpha;
        }

        public Rgba( Rgba copyItem )
            : base( copyItem.Red, copyItem.Green, copyItem.Blue )
        {
            Alpha = copyItem.Alpha;
        }

        // Parses: 1, 0.25, 0.25, 0.5
        public static Rgba FromString(string text)
        {
            var split = text.Split(new string[] { ", " }, StringSplitOptions.None);

            if (split.Count() != 4)
            {
                throw new Exception(string.Format("\"{0}\" is not a valid RGBA.", text));
            }

            return new Rgba(float.Parse(split[0], CultureInfo.InvariantCulture),
                float.Parse(split[1], CultureInfo.InvariantCulture),
                float.Parse(split[2], CultureInfo.InvariantCulture),
                float.Parse(split[3], CultureInfo.InvariantCulture));
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", Red, Green, Blue, Alpha);
        }
    }
}