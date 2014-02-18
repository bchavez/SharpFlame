using System;
using System.Runtime.InteropServices;

namespace SharpFlame.Core.Domain
{
    [StructLayout(LayoutKind.Sequential)]
    public struct XYDouble
    {
        public double X;
        public double Y;
        public static XYDouble operator +(XYDouble ValueA, XYDouble ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA.X + ValueB.X;
            _dbl.Y = ValueA.Y + ValueB.Y;
            return _dbl;
        }

        public static XYDouble operator -(XYDouble ValueA, XYDouble ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA.X - ValueB.X;
            _dbl.Y = ValueA.Y - ValueB.Y;
            return _dbl;
        }

        public static XYDouble operator *(XYDouble ValueA, double ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA.X * ValueB;
            _dbl.Y = ValueA.Y * ValueB;
            return _dbl;
        }

        public static XYDouble operator *(double ValueA, XYDouble ValueB)
        {
            return (XYDouble) (ValueB * ValueA);
        }

        public static XYDouble operator /(XYDouble ValueA, double ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA.X / ValueB;
            _dbl.Y = ValueA.Y / ValueB;
            return _dbl;
        }

        public static XYDouble operator /(double ValueA, XYDouble ValueB)
        {
            XYDouble _dbl;
            _dbl.X = ValueA / ValueB.X;
            _dbl.Y = ValueA / ValueB.Y;
            return _dbl;
        }

        public XYDouble(double X, double Y)
        {
            this = new XYDouble();
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
}