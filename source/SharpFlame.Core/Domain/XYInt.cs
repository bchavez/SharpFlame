using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpFlame.Core.Domain
{
    [DebuggerDisplay("X={X}, Y={Y}")]
    [StructLayout(LayoutKind.Sequential)]
    public struct XYInt {
        public Int32 X;
        public Int32 Y;

        public XYInt(int x, int y) {
            X = x;
            Y = y;
        }

        // Parses: 19136, 4288
        public static XYInt FromString(string text) {
            var split = text.Split (new string[]{", "}, StringSplitOptions.None);

            if (split.Count() != 2) {
                throw new Exception(string.Format("\"{0}\" is not a valid XY Position.", text));
            }

            var result = new XYInt (Int32.Parse (split [0]), 
                                     Int32.Parse (split [1]));
            return result;
        }

        public override string ToString() {
            return string.Format ("{0}, {1}", X, Y);
        }

        public static implicit operator XYInt(XYZInt input) {
            if (input == null) {
                return new XYInt (0, 0);
            }
            return new XYInt (input.X, input.Y);
        }

        public override bool Equals (object obj)
        {
            if (obj == null) {
                return false;
            }

            return base.Equals (obj);
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }

        public static bool operator ==( XYInt a, XYInt b )
        {
            return a.Equals(b);
        }

        public static bool operator !=( XYInt a, XYInt b )
        {
            return ( a.X != b.X ) || ( a.Y != b.Y );
        }

        public static XYInt operator +( XYInt a, XYInt b )
        {
            return new XYInt(a.X + b.X,
                             a.Y + b.Y);
        }

        public static XYInt operator -( XYInt a, XYInt b )
        {
            return new XYInt(a.X - b.X, 
                             a.Y - b.Y);
        }

        public static XYInt Min( XYInt a, XYInt b )
        {
            return new XYInt(Math.Min( a.X, b.X ),
                             Math.Min( a.Y, b.Y ));
        }

        public static XYInt Max( XYInt a, XYInt b )
        {
            return new XYInt(Math.Max( a.X, b.X ),
                             Math.Max( a.Y, b.Y ));

        }

        public bool IsInRange( XYInt Minimum, XYInt Maximum )
        {
            return X >= Minimum.X & X <= Maximum.X
                & Y >= Minimum.Y & Y <= Maximum.Y;
        }

        public static XYDouble operator *( XYInt a, double b )
        {
            return new XYDouble (a.X * b,
                                 a.Y * b);
        }

        public static XYDouble operator /( XYInt a, double b )
        {
            return new XYDouble (a.X / b, 
                                 a.Y / b);
        }

        public XYDouble ToDoubles() {
            return new XYDouble (X, Y);
        }
    }
}

