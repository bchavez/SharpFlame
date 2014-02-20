#region

using System;

#endregion

namespace SharpFlame.Mapping.IO
{
    public struct sFMEUnit
    {
        public string Code;
        public UInt32 ID;
        public byte LNDType;
        public string Name;
        public byte Player;
        public UInt16 Rotation;
        public int SavePriority;
        public UInt32 X;
        public UInt32 Y;
        public UInt32 Z;
    }
}