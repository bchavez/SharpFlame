using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpFlame.Core.Domain
{
    [StructLayout(LayoutKind.Sequential)]
    public class XYZInt {
        public Int32 X;
        public Int32 Y;
        public Int32 Z;

        public XYZInt (Int32 x, Int32 y, Int32 z) {
            X = x;
            Y = y;
            Z = z;
        }

        // Parses: 19136, 4288, 0
        public static XYZInt FromString(string text) {
            var split = text.Split (new string[]{", "}, StringSplitOptions.None);

            if (split.Count() != 3) {
                throw new Exception(string.Format("\"{0}\" is not a valid XYZ Position.", text));
            }

            var result = new XYZInt (Int32.Parse (split [0]),
                                     Int32.Parse (split [1]),
                                     Int32.Parse (split [2]));
            return result;
        }

        public override string ToString() {
            return string.Format ("{0}, {1}, {2}", X, Y, Z);
        }

        public void Add_dbl( XYZDouble XYZ )
        {
            X += (int)XYZ.X;
            Y += (int)XYZ.Y;
            Z += (int)XYZ.Z;
        }

        public void Set_dbl( XYZDouble XYZ )
        {
            X = (int)XYZ.X;
            Y = (int)XYZ.Y;
            Z = (int)XYZ.Z;
        }
    }
}