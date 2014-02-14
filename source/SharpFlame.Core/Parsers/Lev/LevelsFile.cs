using System;
using System.Collections.Generic;

namespace SharpFlame.Core.Parsers.Lev
{
    public class LevelsFile
    {
        public IEnumerable<Campaign> Campaigns { get; internal set; }
        public IEnumerable<Level> Levels { get; internal set; }
    }
}

