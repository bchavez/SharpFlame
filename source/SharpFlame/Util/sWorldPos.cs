using SharpFlame.Core.Domain;

namespace SharpFlame.Util
{
    public struct sWorldPos
    {
        public XYInt Horizontal;
        public int Altitude;

        public sWorldPos( XYInt NewHorizontal, int NewAltitude )
        {
            Horizontal = NewHorizontal;
            Altitude = NewAltitude;
        }
    }
}