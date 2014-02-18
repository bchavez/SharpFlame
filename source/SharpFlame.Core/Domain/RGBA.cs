using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpFlame.Core.Domain
{
    [StructLayout(LayoutKind.Sequential)]
    public class RGBA {
        public Double R;
        public Double G;
        public Double B;
        public Double A;

        public RGBA (Double r, Double g, Double b, Double a) {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        // Parses: 1, 0.25, 0.25, 0.5
        public static RGBA FromString(string text) {
            var split = text.Split (new string[]{", "}, StringSplitOptions.None);

            if (split.Count() != 4) {
                throw new Exception(string.Format("\"{0}\" is not a valid RGBA.", text));
            }

            var result = new RGBA (double.Parse (split[0], CultureInfo.InvariantCulture),
                                   double.Parse (split[1], CultureInfo.InvariantCulture),
                                   double.Parse (split[2], CultureInfo.InvariantCulture),
                                   double.Parse (split[3], CultureInfo.InvariantCulture));
            return result;
        }

        public override string ToString() {
            return string.Format ("{0}, {1}, {2}, {3}", R, G, B, A);
        }
    }
}