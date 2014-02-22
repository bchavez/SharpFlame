#region

using System;
using System.Runtime.InteropServices;
using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Maths
{
    public sealed class Angles
    {
        public const double Pi = 3.1415926535897931;
        public const double RadOf1Deg = 0.017453292519943295;
        public const double RadOf360Deg = 6.2831853071795862;
        public const double RadOf90Deg = 1.5707963267948966;

        public static double AngleClamp(double angle)
        {
            var num = angle;
            if ( num < -3.1415926535897931 )
            {
                return (num + 6.2831853071795862);
            }
            if ( num >= 3.1415926535897931 )
            {
                num -= 6.2831853071795862;
            }
            return num;
        }

        public static double AngleClamp360(double angle)
        {
            var num = angle;
            if ( num < 0.0 )
            {
                return (num + 6.2831853071795862);
            }
            if ( num >= 6.2831853071795862 )
            {
                num -= 6.2831853071795862;
            }
            return num;
        }

        internal static double AngleClampUnlimited(double angle)
        {
            var num2 = (int)Math.Round((angle + 3.1415926535897931) / 6.2831853071795862);
            return (angle - (num2 * 6.2831853071795862));
        }

        public static double GetAngle(XYDouble length)
        {
            return Math.Atan2(length.Y, length.X);
        }

        /// <summary>
        /// Angle Pitch, Yaw. Values in radians
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct AnglePY
        {
            public double Pitch;

            /// <summary>
            /// With respect to the ViewAngleMatrix, typical values range from -Pi (counter-clockwise) to Pi (clockwise).
            /// Or, if you prefer from zero to 180 deg (clockwise) and zero to -180 (counter-clockwise)
            /// </summary>
            public double Yaw;

            public AnglePY(double pitch, double yaw)
            {
                this = new AnglePY {Pitch = pitch, Yaw = yaw};
            }
        }

        /// <summary>
        /// Angle Roll, Pitch, Yaw. Values in radians.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct AngleRPY
        {
            public double Roll;
            public double Pitch;
            public double Yaw;

            public AnglePY PY
            {
                get
                {
                    return new AnglePY(Pitch, Yaw);
                }
                set
                {
                    Pitch = value.Pitch;
                    Yaw = value.Yaw;
                }
            }

            public AngleRPY(double roll, double pitch, double yaw)
            {
                this = new AngleRPY
                    {
                        Roll = roll,
                        Pitch = pitch,
                        Yaw = yaw
                    };
            }
        }
    }
}