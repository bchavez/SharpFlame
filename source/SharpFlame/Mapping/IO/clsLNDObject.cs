using System;
using SharpFlame.Maths;
using Matrix3D;

namespace SharpFlame.Mapping.IO
{
    public class clsLNDObject
    {
        public UInt32 ID;
        public int TypeNum;
        public string Code;
        public int PlayerNum;
        public string Name;
        public Position.XYZ_dbl Pos;
        public sXYZ_int Rotation;
    }
}