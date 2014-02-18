using System;

namespace SharpFlame.Core.Domain
{
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

