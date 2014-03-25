using System;
using System.Linq;
using System.Runtime.InteropServices;
using SharpFlame.Core.Extensions;

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

        public void AddDbl( XYZDouble xyz )
        {
            X += xyz.X.ToInt();
            Y += xyz.Y.ToInt();
            Z += xyz.Z.ToInt();
        }

        public void SetDbl( XYZDouble xyz )
        {
            X = xyz.X.ToInt();
            Y = xyz.Y.ToInt();
            Z = xyz.Z.ToInt();
        }
    }
}