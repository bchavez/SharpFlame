using System;
using System.Collections.Generic;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using SharpFlame.Mapping.Objects;
using SharpFlame.Util;

namespace SharpFlame.Mapping.Wz
{
    public class IniStructures
    {
        public class Structure
        {
            public UInt32 ID;
            public string Code;
            public clsUnitGroup UnitGroup;
            public clsWorldPos Pos;
            public sWZAngle Rotation;
            public int ModuleCount;
            public int HealthPercent;
            public int WallType;
        }

        public List<Structure> Structures;
        public int StructureCount {
            get { return Structures.Count; }
        }

        public IniStructures()
        {
            Structures = new List<Structure> ();
        }
    }
}