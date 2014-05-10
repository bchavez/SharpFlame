

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Objects;


namespace SharpFlame.Mapping.IO.Wz
{
    public class IniDroid
    {
        public string Body;
        public string Brain;
        public string Construct;
        public int DroidType;
        public string ECM;
        public int HealthPercent;
        public UInt32 ID;
        public XYZInt Pos;
        public string Propulsion;
        public string Repair;
        public Rotation Rotation;
        public string Sensor;
        public string Template;
        public clsUnitGroup UnitGroup;
        public int WeaponCount;
        public string[] Weapons;
    }
}