using System;
using System.Runtime.InteropServices;

namespace Matrix3D
{
    public sealed class Position
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct XY_dbl
        {
            public double X;
            public double Y;
            public static Position.XY_dbl operator +(Position.XY_dbl ValueA, Position.XY_dbl ValueB)
            {
                Position.XY_dbl _dbl;
                _dbl.X = ValueA.X + ValueB.X;
                _dbl.Y = ValueA.Y + ValueB.Y;
                return _dbl;
            }

            public static Position.XY_dbl operator -(Position.XY_dbl ValueA, Position.XY_dbl ValueB)
            {
                Position.XY_dbl _dbl;
                _dbl.X = ValueA.X - ValueB.X;
                _dbl.Y = ValueA.Y - ValueB.Y;
                return _dbl;
            }

            public static Position.XY_dbl operator *(Position.XY_dbl ValueA, double ValueB)
            {
                Position.XY_dbl _dbl;
                _dbl.X = ValueA.X * ValueB;
                _dbl.Y = ValueA.Y * ValueB;
                return _dbl;
            }

            public static Position.XY_dbl operator *(double ValueA, Position.XY_dbl ValueB)
            {
                return (Position.XY_dbl) (ValueB * ValueA);
            }

            public static Position.XY_dbl operator /(Position.XY_dbl ValueA, double ValueB)
            {
                Position.XY_dbl _dbl;
                _dbl.X = ValueA.X / ValueB;
                _dbl.Y = ValueA.Y / ValueB;
                return _dbl;
            }

            public static Position.XY_dbl operator /(double ValueA, Position.XY_dbl ValueB)
            {
                Position.XY_dbl _dbl;
                _dbl.X = ValueA / ValueB.X;
                _dbl.Y = ValueA / ValueB.Y;
                return _dbl;
            }

            public XY_dbl(double X, double Y)
            {
                this = new Position.XY_dbl();
                this.X = X;
                this.Y = Y;
            }

            public double GetMagnitude()
            {
                return Math.Sqrt((this.X * this.X) + (this.Y * this.Y));
            }

            public double GetAngle()
            {
                return Math.Atan2(this.Y, this.X);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XYZ_dbl
        {
            public double X;
            public double Y;
            public double Z;
            public static Position.XYZ_dbl operator +(Position.XYZ_dbl ValueA, Position.XYZ_dbl ValueB)
            {
                Position.XYZ_dbl _dbl;
                _dbl.X = ValueA.X + ValueB.X;
                _dbl.Y = ValueA.Y + ValueB.Y;
                _dbl.Z = ValueA.Z + ValueB.Z;
                return _dbl;
            }

            public static Position.XYZ_dbl operator -(Position.XYZ_dbl ValueA, Position.XYZ_dbl ValueB)
            {
                Position.XYZ_dbl _dbl;
                _dbl.X = ValueA.X - ValueB.X;
                _dbl.Y = ValueA.Y - ValueB.Y;
                _dbl.Z = ValueA.Z - ValueB.Z;
                return _dbl;
            }

            public static Position.XYZ_dbl operator *(Position.XYZ_dbl ValueA, Position.XYZ_dbl ValueB)
            {
                Position.XYZ_dbl _dbl;
                _dbl.X = ValueA.X * ValueB.X;
                _dbl.Y = ValueA.Y * ValueB.Y;
                _dbl.Z = ValueA.Z * ValueB.Z;
                return _dbl;
            }

            public static Position.XYZ_dbl operator *(Position.XYZ_dbl ValueA, double ValueB)
            {
                Position.XYZ_dbl _dbl;
                _dbl.X = ValueA.X * ValueB;
                _dbl.Y = ValueA.Y * ValueB;
                _dbl.Z = ValueA.Z * ValueB;
                return _dbl;
            }

            public static Position.XYZ_dbl operator *(double ValueA, Position.XYZ_dbl ValueB)
            {
                return (Position.XYZ_dbl) (ValueB * ValueA);
            }

            public static Position.XYZ_dbl operator /(Position.XYZ_dbl ValueA, Position.XYZ_dbl ValueB)
            {
                Position.XYZ_dbl _dbl;
                _dbl.X = ValueA.X / ValueB.X;
                _dbl.Y = ValueA.Y / ValueB.Y;
                _dbl.Z = ValueA.Z / ValueB.Z;
                return _dbl;
            }

            public static Position.XYZ_dbl operator /(Position.XYZ_dbl ValueA, double ValueB)
            {
                Position.XYZ_dbl _dbl;
                _dbl.X = ValueA.X / ValueB;
                _dbl.Y = ValueA.Y / ValueB;
                _dbl.Z = ValueA.Z / ValueB;
                return _dbl;
            }

            public static Position.XYZ_dbl operator /(double ValueA, Position.XYZ_dbl ValueB)
            {
                Position.XYZ_dbl _dbl;
                _dbl.X = ValueA / ValueB.X;
                _dbl.Y = ValueA / ValueB.Y;
                _dbl.Z = ValueA / ValueB.Z;
                return _dbl;
            }

            public XYZ_dbl(double X, double Y, double Z)
            {
                this = new Position.XYZ_dbl();
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }

            public double GetMagnitude()
            {
                return Math.Sqrt(((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z));
            }
        }
    }
}

