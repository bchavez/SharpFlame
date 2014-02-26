#region

using System;
using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Old.Mapping.IO.LND
{
    public class LNDObject
    {
        public string Code;
        public UInt32 ID;
        public string Name;
        public int PlayerNum;
        public XYZDouble Pos;
        public XYZInt Rotation;
        public int TypeNum;
    }
}