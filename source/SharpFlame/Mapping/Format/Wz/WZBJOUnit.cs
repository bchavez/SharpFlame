#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;

#endregion

namespace SharpFlame.Mapping.Format.Wz
{
    public class clsWZBJOUnit
    {
        public string Code;
        public UInt32 ID;
        public UnitType ObjectType;
        public UInt32 Player;
        public WorldPos Pos;
        public UInt32 Rotation;
    }
}