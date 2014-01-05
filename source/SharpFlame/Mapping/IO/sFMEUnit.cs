using System;

namespace SharpFlame.Mapping.IO
{
    public struct sFMEUnit
    {
        public string Code;
        public UInt32 ID;
        public int SavePriority;
        public byte LNDType;
        public UInt32 X;
        public UInt32 Y;
        public UInt32 Z;
        public UInt16 Rotation;
        public string Name;
        public byte Player;
    }
}