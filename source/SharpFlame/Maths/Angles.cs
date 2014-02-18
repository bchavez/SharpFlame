using System;
using System.Runtime.InteropServices;
using SharpFlame.Core.Domain;

namespace SharpFlame.Maths
{
    public sealed class Angles
    {
        public const double Pi = 3.1415926535897931;
        public const double RadOf1Deg = 0.017453292519943295;
        public const double RadOf360Deg = 6.2831853071795862;
        public const double RadOf90Deg = 1.5707963267948966;

        public static double AngleClamp(double Angle)
        {
            double num = Angle;
            if (num < -3.1415926535897931)
            {
                return (num + 6.2831853071795862);
            }
            if (num >= 3.1415926535897931)
            {
                num -= 6.2831853071795862;
            }
            return num;
        }

        public static double AngleClamp360(double Angle)
        {
            double num = Angle;
            if (num < 0.0)
            {
                return (num + 6.2831853071795862);
            }
            if (num >= 6.2831853071795862)
            {
                num -= 6.2831853071795862;
            }
            return num;
        }

        internal static double AngleClampUnlimited(double Angle)
        {
            int num2 = (int)Math.Round((Angle + 3.1415926535897931) / 6.2831853071795862);
            return (Angle - (num2 * 6.2831853071795862));
        }

        public static double GetAngle(XYDouble Length)
        {
            return Math.Atan2(Length.Y, Length.X);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AnglePY
        {
            public double Pitch;
            public double Yaw;
            public AnglePY(double Pitch, double Yaw)
            {
                this = new Angles.AnglePY();
                this.Pitch = Pitch;
                this.Yaw = Yaw;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AngleRPY
        {
            public double Roll;
            public double Pitch;
            public double Yaw;
            public Angles.AnglePY PY
            {
                get
                {
                    return new Angles.AnglePY(this.Pitch, this.Yaw);
                }
                set
                {
                    this.Pitch = value.Pitch;
                    this.Yaw = value.Yaw;
                }
            }
            public AngleRPY(double Roll, double Pitch, double Yaw)
            {
                this = new Angles.AngleRPY();
                this.Roll = Roll;
                this.Pitch = Pitch;
                this.Yaw = Yaw;
            }
        }
    }
}

