namespace SharpFlame.Core.Parsers.Pie
{
    public class Polygon
    {
        public PolygonFlags Flags { get; set; }
        public uint PointCount { get; set; }
        public float P1 { get; set; }
        public float P2 { get; set; }
        public float P3 { get; set; }
        public float Frames { get; set; }
        public float PlaybackRate { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public TexCoord[] TexCoords { get; set; }
    }
}