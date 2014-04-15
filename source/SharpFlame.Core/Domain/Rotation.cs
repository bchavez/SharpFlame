using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpFlame.Core.Domain
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("Direction {Direction}, Pitch {Pitch}, Roll {Roll}")]
    public struct Rotation {
        public UInt16 Direction;
        public UInt16 Pitch;
        public UInt16 Roll;

        public Rotation(UInt16 direction, UInt16 pitch, UInt16 roll) {
            Direction = direction;
            Pitch = pitch;
            Roll = roll;
        }

        // Parses: 19136, 4288, 0
        public static Rotation FromString(string text) {
            var split = text.Split (new string[]{", "}, StringSplitOptions.None);

            if (split.Count() != 3) {
                throw new Exception(string.Format("\"{0}\" is not a valid Rotation.", text));
            }

            var result = new Rotation (UInt16.Parse (split [0]),
                                       UInt16.Parse (split [1]),
                                       UInt16.Parse (split [2]));
            return result;
        }

        public override string ToString() {
            return string.Format ("{0}, {1}, {2}", Direction, Pitch, Roll);
        }
    }
}

