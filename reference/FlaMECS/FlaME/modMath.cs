namespace FlaME
{
    using Matrix3D;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Runtime.InteropServices;

    [StandardModule]
    public sealed class modMath
    {
        public const double RadOf1Deg = 0.017453292519943295;
        public const double RadOf360Deg = 6.2831853071795862;
        public const double RadOf90Deg = 1.5707963267948966;
        public const double RootTwo = 1.4142135623730951;

        public static double AngleClamp(double Angle)
        {
            double num2 = Angle;
            if (num2 < -3.1415926535897931)
            {
                return (num2 + 6.2831853071795862);
            }
            if (num2 >= 3.1415926535897931)
            {
                num2 -= 6.2831853071795862;
            }
            return num2;
        }

        public static double Clamp_dbl(double Amount, double Minimum, double Maximum)
        {
            double num2 = Amount;
            if (num2 < Minimum)
            {
                return Minimum;
            }
            if (num2 > Maximum)
            {
                num2 = Maximum;
            }
            return num2;
        }

        public static int Clamp_int(int Amount, int Minimum, int Maximum)
        {
            int num2 = Amount;
            if (num2 < Minimum)
            {
                return Minimum;
            }
            if (num2 > Maximum)
            {
                num2 = Maximum;
            }
            return num2;
        }

        public static float Clamp_sng(float Amount, float Minimum, float Maximum)
        {
            float num2 = Amount;
            if (num2 < Minimum)
            {
                return Minimum;
            }
            if (num2 > Maximum)
            {
                num2 = Maximum;
            }
            return num2;
        }

        public static sIntersectPos GetLinesIntersectBetween(sXY_int A1, sXY_int A2, sXY_int B1, sXY_int B2)
        {
            sIntersectPos pos2;
            if (((A1.X == A2.X) & (A1.Y == A2.Y)) | ((B1.X == B2.X) & (B1.Y == B2.Y)))
            {
                pos2.Exists = false;
                return pos2;
            }
            double num9 = B1.Y - A1.Y;
            double num8 = B1.X - A1.X;
            double num = A2.X - A1.X;
            double num2 = A2.Y - A1.Y;
            double num4 = B2.X - B1.X;
            double num5 = B2.Y - B1.Y;
            double num7 = (num * num5) - (num2 * num4);
            if (num7 == 0.0)
            {
                pos2.Exists = false;
                return pos2;
            }
            double num3 = ((num8 * num5) - (num9 * num4)) / num7;
            double num6 = ((num8 * num2) - (num9 * num)) / num7;
            if ((((num3 <= 0.0) | (num3 >= 1.0)) | (num6 <= 0.0)) | (num6 >= 1.0))
            {
                pos2.Exists = false;
                return pos2;
            }
            pos2.Pos.X = A1.X + ((int) Math.Round((double) (num3 * num)));
            pos2.Pos.Y = A1.Y + ((int) Math.Round((double) (num3 * num2)));
            pos2.Exists = true;
            return pos2;
        }

        public static sXY_int PointGetClosestPosOnLine(sXY_int LinePointA, sXY_int LinePointB, sXY_int Point)
        {
            sXY_int _int2;
            double num4 = Point.X - LinePointA.X;
            double num5 = Point.Y - LinePointA.Y;
            double num = LinePointB.X - LinePointA.X;
            double num2 = LinePointB.Y - LinePointA.Y;
            double num3 = (num * num) + (num2 * num2);
            if (num3 == 0.0)
            {
                return LinePointA;
            }
            double num6 = ((num4 * num) + (num5 * num2)) / num3;
            if (num6 <= 0.0)
            {
                return LinePointA;
            }
            if (num6 >= 1.0)
            {
                return LinePointB;
            }
            _int2.X = LinePointA.X + ((int) Math.Round((double) (num * num6)));
            _int2.Y = LinePointA.Y + ((int) Math.Round((double) (num2 * num6)));
            return _int2;
        }

        public static void ReorderXY(sXY_int A, sXY_int B, ref sXY_int Lesser, ref sXY_int Greater)
        {
            if (A.X <= B.X)
            {
                Lesser.X = A.X;
                Greater.X = B.X;
            }
            else
            {
                Lesser.X = B.X;
                Greater.X = A.X;
            }
            if (A.Y <= B.Y)
            {
                Lesser.Y = A.Y;
                Greater.Y = B.Y;
            }
            else
            {
                Lesser.Y = B.Y;
                Greater.Y = A.Y;
            }
        }

        public class clsXY_int
        {
            public modMath.sXY_int XY;

            public clsXY_int(modMath.sXY_int XY)
            {
                this.XY = XY;
            }

            public int X
            {
                get
                {
                    return this.XY.X;
                }
                set
                {
                    this.XY.X = value;
                }
            }

            public int Y
            {
                get
                {
                    return this.XY.Y;
                }
                set
                {
                    this.XY.Y = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sIntersectPos
        {
            public bool Exists;
            public modMath.sXY_int Pos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sXY_int
        {
            public int X;
            public int Y;
            public sXY_int(int X, int Y)
            {
                this = new modMath.sXY_int();
                this.X = X;
                this.Y = Y;
            }

            public static bool operator ==(modMath.sXY_int a, modMath.sXY_int b)
            {
                return ((a.X == b.X) & (a.Y == b.Y));
            }

            public static bool operator !=(modMath.sXY_int a, modMath.sXY_int b)
            {
                return ((a.X != b.X) | (a.Y != b.Y));
            }

            public static modMath.sXY_int operator +(modMath.sXY_int a, modMath.sXY_int b)
            {
                modMath.sXY_int _int;
                _int.X = a.X + b.X;
                _int.Y = a.Y + b.Y;
                return _int;
            }

            public static modMath.sXY_int operator -(modMath.sXY_int a, modMath.sXY_int b)
            {
                modMath.sXY_int _int;
                _int.X = a.X - b.X;
                _int.Y = a.Y - b.Y;
                return _int;
            }

            public static Position.XY_dbl operator *(modMath.sXY_int a, double b)
            {
                Position.XY_dbl _dbl;
                _dbl.X = a.X * b;
                _dbl.Y = a.Y * b;
                return _dbl;
            }

            public static Position.XY_dbl operator /(modMath.sXY_int a, double b)
            {
                Position.XY_dbl _dbl;
                _dbl.X = ((double) a.X) / b;
                _dbl.Y = ((double) a.Y) / b;
                return _dbl;
            }

            public Position.XY_dbl ToDoubles()
            {
                Position.XY_dbl _dbl;
                _dbl.X = this.X;
                _dbl.Y = this.Y;
                return _dbl;
            }

            public static modMath.sXY_int Min(modMath.sXY_int a, modMath.sXY_int b)
            {
                modMath.sXY_int _int2;
                _int2.X = Math.Min(a.X, b.X);
                _int2.Y = Math.Min(a.Y, b.Y);
                return _int2;
            }

            public static modMath.sXY_int Max(modMath.sXY_int a, modMath.sXY_int b)
            {
                modMath.sXY_int _int2;
                _int2.X = Math.Max(a.X, b.X);
                _int2.Y = Math.Max(a.Y, b.Y);
                return _int2;
            }

            public bool IsInRange(modMath.sXY_int Minimum, modMath.sXY_int Maximum)
            {
                return ((((this.X >= Minimum.X) & (this.X <= Maximum.X)) & (this.Y >= Minimum.Y)) & (this.Y <= Maximum.Y));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sXY_sng
        {
            public float X;
            public float Y;
            public sXY_sng(float X, float Y)
            {
                this = new modMath.sXY_sng();
                this.X = X;
                this.Y = Y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sXY_uint
        {
            public uint X;
            public uint Y;
            public sXY_uint(uint X, uint Y)
            {
                this = new modMath.sXY_uint();
                this.X = X;
                this.Y = Y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sXYZ_int
        {
            public int X;
            public int Y;
            public int Z;
            public sXYZ_int(int X, int Y, int Z)
            {
                this = new modMath.sXYZ_int();
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }

            public void Add_dbl(Position.XYZ_dbl XYZ)
            {
                this.X += (int) Math.Round(XYZ.X);
                this.Y += (int) Math.Round(XYZ.Y);
                this.Z += (int) Math.Round(XYZ.Z);
            }

            public void Set_dbl(Position.XYZ_dbl XYZ)
            {
                this.X = (int) Math.Round(XYZ.X);
                this.Y = (int) Math.Round(XYZ.Y);
                this.Z = (int) Math.Round(XYZ.Z);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sXYZ_sng
        {
            public float X;
            public float Y;
            public float Z;
            public sXYZ_sng(float X, float Y, float Z)
            {
                this = new modMath.sXYZ_sng();
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }
        }
    }
}

