using System;
using Matrix3D;

namespace SharpFlame.Maths
{
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

    public struct sXYZ_int
    {
        public int X;
        public int Y;
        public int Z;

        public sXYZ_int( int X, int Y, int Z )
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public void Add_dbl( Position.XYZ_dbl XYZ )
        {
            X += (int)XYZ.X;
            Y += (int)XYZ.Y;
            Z += (int)XYZ.Z;
        }

        public void Set_dbl( Position.XYZ_dbl XYZ )
        {
            X = (int)XYZ.X;
            Y = (int)XYZ.Y;
            Z = (int)XYZ.Z;
        }
    }

    public struct sXY_sng
    {
        public float X;
        public float Y;

        public sXY_sng( float X, float Y )
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public struct sXY_uint
    {
        public UInt32 X;
        public UInt32 Y;

        public sXY_uint( UInt32 X, UInt32 Y )
        {
            this.X = X;
            this.Y = Y;
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

        public clsXY_int( sXY_int XY )
        {
            this.XY = XY;
        }
    }

    public struct sXY_int
    {
        public int X;
        public int Y;

        public sXY_int( int X, int Y )
        {
            this.X = X;
            this.Y = Y;
        }

        public static bool operator ==( sXY_int a, sXY_int b )
        {
            return ( a.X == b.X ) && ( a.Y == b.Y );
        }

        public static bool operator !=( sXY_int a, sXY_int b )
        {
            return ( a.X != b.X ) || ( a.Y != b.Y );
        }

        public static sXY_int operator +( sXY_int a, sXY_int b )
        {
            sXY_int result = new sXY_int();

            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;

            return result;
        }

        public static sXY_int operator -( sXY_int a, sXY_int b )
        {
            sXY_int result = new sXY_int();

            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;

            return result;
        }

        public static Position.XY_dbl operator *( sXY_int a, double b )
        {
            Position.XY_dbl result = default( Position.XY_dbl );

            result.X = a.X * b;
            result.Y = a.Y * b;

            return result;
        }

        public static Position.XY_dbl operator /( sXY_int a, double b )
        {
            Position.XY_dbl result = default( Position.XY_dbl );

            result.X = a.X / b;
            result.Y = a.Y / b;

            return result;
        }

        public Position.XY_dbl ToDoubles()
        {
            Position.XY_dbl result = default( Position.XY_dbl );

            result.X = X;
            result.Y = Y;

            return result;
        }

        public static sXY_int Min( sXY_int a, sXY_int b )
        {
            sXY_int result = new sXY_int();

            result.X = Math.Min( a.X, b.X );
            result.Y = Math.Min( a.Y, b.Y );

            return result;
        }

        public static sXY_int Max( sXY_int a, sXY_int b )
        {
            sXY_int result = new sXY_int();

            result.X = Math.Max( a.X, b.X );
            result.Y = Math.Max( a.Y, b.Y );

            return result;
        }

        public bool IsInRange( sXY_int Minimum, sXY_int Maximum )
        {
            return X >= Minimum.X & X <= Maximum.X
                   & Y >= Minimum.Y & Y <= Maximum.Y;
        }
    }
}