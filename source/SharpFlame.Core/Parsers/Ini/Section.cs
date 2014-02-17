using System;
using System.Collections.Generic;

namespace SharpFlame.Core.Parsers.Ini
{
    public class Section
    {
        public string Name { get; set; }
        public List<Token> Data { get; internal set; }

        public Section() {
        }

        public Section(string name) {
            Name = name;
            Data = new List<Token>();
        }
    }
}

