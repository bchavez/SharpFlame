using System;

namespace SharpFlame.Core.Parsers.Lev
{
    public class Level
    {
        public string Name { get; internal set; }
        public int Players { get; internal set; }
        public int Type { get; internal set; }
        public string Dataset { get; internal set; }
        public string Game { get; internal set; }
        public string[] Data { get; internal set; }
    }
}

