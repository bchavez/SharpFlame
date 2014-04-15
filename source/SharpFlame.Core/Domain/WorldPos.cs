using System.Diagnostics;

namespace SharpFlame.Core.Domain
{
    [DebuggerDisplay("Horiz.X {Horizontal.X}, Horz.Y {Horizontal.Y}, Alt {Altitude}")]
    public struct WorldPos
    {
        public XYInt Horizontal;
        public int Altitude;

        public WorldPos( XYInt horizontal, int altitude )
        {
            Horizontal = horizontal;
            Altitude = altitude;
        }
    }
}

