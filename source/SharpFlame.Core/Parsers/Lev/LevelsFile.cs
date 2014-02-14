using System;
using System.Collections.Generic;

namespace SharpFlame.Core.Parsers.Lev
{
    public class LevelsFile
    {
        public List<Campaign> Campaigns { get; internal set; }
        public List<Level> Levels { get; internal set; }
    }
}

