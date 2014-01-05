using System;
using Matrix3D;

namespace SharpFlame
{
    public sealed class modMath
    {
        public const double RadOf1Deg = Math.PI / 180.0D;
        public const double RadOf90Deg = Math.PI / 2.0D;
        public const double RadOf360Deg = Math.PI * 2.0D;

        public const double RootTwo = 1.4142135623730951D;

        public struct sXY_int
        {
            public int X;
            public int Y;

            public sXY_int(int X, int Y)
            {
                this.X = X;
                this.Y = Y;
            }

            public static bool operator ==(sXY_int a, sXY_int b)
            {
                return (a.X == b.X) && (a.Y == b.Y);
            }

            public static bool operator !=(sXY_int a, sXY_int b)
            {
                return (a.X != b.X) || (a.Y != b.Y);
            }

            public static sXY_int operator +(sXY_int a, sXY_int b)
            {
                sXY_int result = new sXY_int();

                result.X = a.X + b.X;
                result.Y = a.Y + b.Y;

                return result;
            }

            public static sXY_int operator -(sXY_int a, sXY_int b)
            {
                sXY_int result = new sXY_int();

                result.X = a.X - b.X;
                result.Y = a.Y - b.Y;

                return result;
            }

            public static Position.XY_dbl operator *(sXY_int a, double b)
            {
                Position.XY_dbl result = default(Position.XY_dbl);

                result.X = a.X * b;
                result.Y = a.Y * b;

                return result;
            }

            public static Position.XY_dbl operator /(sXY_int a, double b)
            {
                Position.XY_dbl result = default(Position.XY_dbl);

                result.X = a.X / b;
                result.Y = a.Y / b;

                return result;
            }

            public Position.XY_dbl ToDoubles()
            {
                Position.XY_dbl result = default(Position.XY_dbl);

                result.X = X;
                result.Y = Y;

                return result;
            }

            public static sXY_int Min(sXY_int a, sXY_int b)
            {
                sXY_int result = new sXY_int();

                result.X = Math.Min(a.X, b.X);
                result.Y = Math.Min(a.Y, b.Y);

                return result;
            }

            public static sXY_int Max(sXY_int a, sXY_int b)
            {
                sXY_int result = new sXY_int();

                result.X = Math.Max(a.X, b.X);
                result.Y = Math.Max(a.Y, b.Y);

                return result;
            }

            public bool IsInRange(sXY_int Minimum, sXY_int Maximum)
            {
                return X >= Minimum.X & X <= Maximum.X
                       & Y >= Minimum.Y & Y <= Maximum.Y;
            }
        }

        public class clsXY_int
        {
            public sXY_int XY;

            public int X
            {
                get { return XY.X; }
                set { XY.X = value; }
            }

            public int Y
            {
                get { return XY.Y; }
                set { XY.Y = value; }
            }

            public clsXY_int(sXY_int XY)
            {
                this.XY = XY;
            }
        }

        public struct sXY_uint
        {
            public UInt32 X;
            public UInt32 Y;

            public sXY_uint(UInt32 X, UInt32 Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }

        public struct sXY_sng
        {
            public float X;
            public float Y;

            public sXY_sng(float X, float Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }

        public struct sXYZ_int
        {
            public int X;
            public int Y;
            public int Z;

            public sXYZ_int(int X, int Y, int Z)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }

            public void Add_dbl(Position.XYZ_dbl XYZ)
            {
                X += (int)XYZ.X;
                Y += (int)XYZ.Y;
                Z += (int)XYZ.Z;
            }

            public void Set_dbl(Position.XYZ_dbl XYZ)
            {
                X = (int)XYZ.X;
                Y = (int)XYZ.Y;
                Z = (int)XYZ.Z;
            }
        }

        public struct sXYZ_sng
        {
            public float X;
            public float Y;
            public float Z;

            public sXYZ_sng(float X, float Y, float Z)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }
        }

        public static double AngleClamp(double Angle)
        {
            double ReturnResult = 0;

            ReturnResult = Angle;
            if ( ReturnResult < - Math.PI )
            {
                ReturnResult += RadOf360Deg;
            }
            else if ( ReturnResult >= Math.PI )
            {
                ReturnResult -= RadOf360Deg;
            }
            return ReturnResult;
        }

        public static double Clamp_dbl(double Amount, double Minimum, double Maximum)
        {
            double ReturnResult = 0;

            ReturnResult = Amount;
            if ( ReturnResult < Minimum )
            {
                ReturnResult = Minimum;
            }
            else if ( ReturnResult > Maximum )
            {
                ReturnResult = Maximum;
            }
            return ReturnResult;
        }

        public static float Clamp_sng(float Amount, float Minimum, float Maximum)
        {
            float ReturnResult = 0;

            ReturnResult = Amount;
            if ( ReturnResult < Minimum )
            {
                ReturnResult = Minimum;
            }
            else if ( ReturnResult > Maximum )
            {
                ReturnResult = Maximum;
            }
            return ReturnResult;
        }

        public static int Clamp_int(int Amount, int Minimum, int Maximum)
        {
            int ReturnResult = 0;

            ReturnResult = Amount;
            if ( ReturnResult < Minimum )
            {
                ReturnResult = Minimum;
            }
            else if ( ReturnResult > Maximum )
            {
                ReturnResult = Maximum;
            }
            return ReturnResult;
        }

        public struct sIntersectPos
        {
            public bool Exists;
            public sXY_int Pos;
        }

        public static sIntersectPos GetLinesIntersectBetween(sXY_int A1, sXY_int A2, sXY_int B1, sXY_int B2)
        {
            sIntersectPos Result = new sIntersectPos();

            if ( (A1.X == A2.X & A1.Y == A2.Y) || (B1.X == B2.X & B1.Y == B2.Y) )
            {
                Result.Exists = false;
            }
            else
            {
                double y1dif = 0;
                double x1dif = 0;
                double adifx = 0;
                double adify = 0;
                double bdifx = 0;
                double bdify = 0;
                double m = 0;
                double ar = 0;
                double br = 0;

                y1dif = B1.Y - A1.Y;
                x1dif = B1.X - A1.X;
                adifx = A2.X - A1.X;
                adify = A2.Y - A1.Y;
                bdifx = B2.X - B1.X;
                bdify = B2.Y - B1.Y;
                m = adifx * bdify - adify * bdifx;
                if ( m == 0.0D )
                {
                    Result.Exists = false;
                }
                else
                {
                    ar = (x1dif * bdify - y1dif * bdifx) / m;
                    br = (x1dif * adify - y1dif * adifx) / m;
                    if ( ar <= 0.0D | ar >= 1.0D | br <= 0.0D | br >= 1.0D )
                    {
                        Result.Exists = false;
                    }
                    else
                    {
                        Result.Pos.X = A1.X + (int)(ar * adifx);
                        Result.Pos.Y = A1.Y + (int)(ar * adify);
                        Result.Exists = true;
                    }
                }
            }
            return Result;
        }

        public static sXY_int PointGetClosestPosOnLine(sXY_int LinePointA, sXY_int LinePointB, sXY_int Point)
        {
            double x1dif = Point.X - LinePointA.X;
            double y1dif = Point.Y - LinePointA.Y;
            double adifx = LinePointB.X - LinePointA.X;
            double adify = LinePointB.Y - LinePointA.Y;
            double m = 0;

            m = adifx * adifx + adify * adify;
            if ( m == 0.0D )
            {
                return LinePointA;
            }
            else
            {
                double ar = 0;
                ar = (x1dif * adifx + y1dif * adify) / m;
                if ( ar <= 0.0D )
                {
                    return LinePointA;
                }
                else if ( ar >= 1.0D )
                {
                    return LinePointB;
                }
                else
                {
                    sXY_int Result = new sXY_int();
                    Result.X = LinePointA.X + (int)(adifx * ar);
                    Result.Y = LinePointA.Y + (int)(adify * ar);
                    return Result;
                }
            }
        }

        public static void ReorderXY(sXY_int A, sXY_int B, sXY_int Lesser, sXY_int Greater)
        {
            if ( A.X <= B.X )
            {
                Lesser.X = A.X;
                Greater.X = B.X;
            }
            else
            {
                Lesser.X = B.X;
                Greater.X = A.X;
            }
            if ( A.Y <= B.Y )
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
    }
}