#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;

#endregion

namespace SharpFlame.Maths
{
    public sealed class MathUtil
    {
        public const double RadOf1Deg = Math.PI / 180.0D;
        public const double RadOf90Deg = Math.PI / 2.0D;
        public const double RadOf360Deg = Math.PI * 2.0D;

        public const double RootTwo = 1.4142135623730951D;

        public static double AngleClamp(double angle)
        {
            double returnResult = 0;

            returnResult = angle;
            if ( returnResult < - Math.PI )
            {
                returnResult += RadOf360Deg;
            }
            else if ( returnResult >= Math.PI )
            {
                returnResult -= RadOf360Deg;
            }
            return returnResult;
        }

        public static double ClampDbl(double amount, double minimum, double maximum)
        {
            double returnResult = 0;

            returnResult = amount;
            if ( returnResult < minimum )
            {
                returnResult = minimum;
            }
            else if ( returnResult > maximum )
            {
                returnResult = maximum;
            }
            return returnResult;
        }

        public static float ClampSng(float amount, float minimum, float maximum)
        {
            float returnResult = 0;

            returnResult = amount;
            if ( returnResult < minimum )
            {
                returnResult = minimum;
            }
            else if ( returnResult > maximum )
            {
                returnResult = maximum;
            }
            return returnResult;
        }

        public static int ClampInt(int amount, int minimum, int maximum)
        {
            var returnResult = 0;

            returnResult = amount;
            if ( returnResult < minimum )
            {
                returnResult = minimum;
            }
            else if ( returnResult > maximum )
            {
                returnResult = maximum;
            }
            return returnResult;
        }

        public static sIntersectPos GetLinesIntersectBetween(XYInt a1, XYInt a2, XYInt b1, XYInt b2)
        {
            var result = new sIntersectPos();

            if ( (a1.X == a2.X & a1.Y == a2.Y) || (b1.X == b2.X & b1.Y == b2.Y) )
            {
                result.Exists = false;
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

                y1dif = b1.Y - a1.Y;
                x1dif = b1.X - a1.X;
                adifx = a2.X - a1.X;
                adify = a2.Y - a1.Y;
                bdifx = b2.X - b1.X;
                bdify = b2.Y - b1.Y;
                m = adifx * bdify - adify * bdifx;
                if ( m == 0.0D )
                {
                    result.Exists = false;
                }
                else
                {
                    ar = (x1dif * bdify - y1dif * bdifx) / m;
                    br = (x1dif * adify - y1dif * adifx) / m;
                    if ( ar <= 0.0D | ar >= 1.0D | br <= 0.0D | br >= 1.0D )
                    {
                        result.Exists = false;
                    }
                    else
                    {
                        result.Pos.X = a1.X + (ar * adifx).ToInt();
                        result.Pos.Y = a1.Y + (ar * adify).ToInt();
                        result.Exists = true;
                    }
                }
            }
            return result;
        }

        public static XYInt PointGetClosestPosOnLine(XYInt linePointA, XYInt linePointB, XYInt point)
        {
            double x1dif = point.X - linePointA.X;
            double y1dif = point.Y - linePointA.Y;
            double adifx = linePointB.X - linePointA.X;
            double adify = linePointB.Y - linePointA.Y;
            double m = 0;

            m = adifx * adifx + adify * adify;
            if ( m == 0.0D )
            {
                return linePointA;
            }
            double ar = 0;
            ar = (x1dif * adifx + y1dif * adify) / m;
            if ( ar <= 0.0D )
            {
                return linePointA;
            }
            if ( ar >= 1.0D )
            {
                return linePointB;
            }
            var result = new XYInt();
            result.X = linePointA.X + (adifx * ar).ToInt();
            result.Y = linePointA.Y + (adify * ar).ToInt();
            return result;
        }

        public static void ReorderXY(XYInt a, XYInt b, ref XYInt lesser, ref XYInt greater)
        {
            if ( a.X <= b.X )
            {
                lesser.X = a.X;
                greater.X = b.X;
            }
            else
            {
                lesser.X = b.X;
                greater.X = a.X;
            }
            if ( a.Y <= b.Y )
            {
                lesser.Y = a.Y;
                greater.Y = b.Y;
            }
            else
            {
                lesser.Y = b.Y;
                greater.Y = a.Y;
            }
        }

        public struct sIntersectPos
        {
            public bool Exists;
            public XYInt Pos;
        }
    }
}