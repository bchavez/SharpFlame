using SharpFlame.Maths;

namespace SharpFlame.Util
{
    public struct sWorldPos
    {
        public sXY_int Horizontal;
        public int Altitude;

        public sWorldPos( sXY_int NewHorizontal, int NewAltitude )
        {
            Horizontal = NewHorizontal;
            Altitude = NewAltitude;
        }
    }
}