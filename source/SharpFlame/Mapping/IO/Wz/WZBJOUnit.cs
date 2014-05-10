

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;


namespace SharpFlame.Mapping.IO.Wz
{
    public class WZBJOUnit
    {
        public string Code;
        public UInt32 ID;
        public UnitType ObjectType;
        public UInt32 Player;
        public WorldPos Pos;
        public UInt32 Rotation;
    }
}