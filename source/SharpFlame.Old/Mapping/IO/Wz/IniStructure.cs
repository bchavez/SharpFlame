#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Old.Mapping.Objects;

#endregion

namespace SharpFlame.Old.Mapping.IO.Wz
{
    public class IniStructure
    {
        public string Code;
        public int HealthPercent;
        public UInt32 ID;
        public int ModuleCount;
        public XYZInt Pos;
        public Rotation Rotation;
        public clsUnitGroup UnitGroup;
        public int WallType;
    }
}