#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping.Format.WZFormat
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