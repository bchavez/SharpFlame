using System;
using SharpFlame.Core.Domain;

namespace SharpFlame.Maths
{
    public sealed class MathUtil
    {
        public const double RadOf1Deg = Math.PI / 180.0D;
        public const double RadOf90Deg = Math.PI / 2.0D;
        public const double RadOf360Deg = Math.PI * 2.0D;

        public const double RootTwo = 1.4142135623730951D;

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
            public XYInt Pos;
        }

        public static sIntersectPos GetLinesIntersectBetween(XYInt A1, XYInt A2, XYInt B1, XYInt B2)
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

        public static XYInt PointGetClosestPosOnLine(XYInt LinePointA, XYInt LinePointB, XYInt Point)
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
                    XYInt Result = new XYInt();
                    Result.X = LinePointA.X + (int)(adifx * ar);
                    Result.Y = LinePointA.Y + (int)(adify * ar);
                    return Result;
                }
            }
        }

        public static void ReorderXY(XYInt A, XYInt B, ref XYInt Lesser, ref XYInt Greater)
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