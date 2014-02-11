using FluentValidation.Attributes;
using SharpFlame.Core.Parsers.Validators;

namespace SharpFlame.Core.Parsers.Pie
{
    [Validator(typeof(LevelValidator))]
    public class Level
    {
        public int LevelNumber { get; set; }
        public Point[] Points { get; set; }
        public Polygon[] Polygons { get; set; }
        internal int PointsCount { get; set; }
        internal int PolygonsCount { get; set; }
        internal int? ConnectorCount { get; set; }
        public Connector[] Connectors { get; set; }
    }
}