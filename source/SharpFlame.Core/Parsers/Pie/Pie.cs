using FluentValidation.Attributes;
using SharpFlame.Core.Parsers.Validators;

namespace SharpFlame.Core.Parsers.Pie
{
    [Validator(typeof(PieValidator))]
    public class Pie
    {
        public string FileName { get; set; }
        public int Version { get; set; }
        public int Type { get; set; }
        public Texture Texture { get; set; }
        public Level[] Levels { get; set; }
        internal int LevelCount { get; set; }
    }
}