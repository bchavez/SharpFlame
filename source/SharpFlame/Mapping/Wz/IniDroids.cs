using System;
using System.Collections.Generic;
using SharpFlame.Mapping.Objects;
using SharpFlame.Util;

namespace SharpFlame.Mapping.Wz
{
    public class IniDroids
    {
        public class Droid
        {
            public UInt32 ID;
            public string Template;
            public clsUnitGroup UnitGroup;
            public clsWorldPos Pos;
            public sWZAngle Rotation;
            public int HealthPercent;
            public int DroidType;
            public string Body;
            public string Propulsion;
            public string Brain;
            public string Repair;
            public string ECM;
            public string Sensor;
            public string Construct;
            public string[] Weapons;
            public int WeaponCount;
        }

        public List<Droid> Droids;
        public int DroidCount { 
            get { return Droids.Count; }
        }

        public IniDroids() {
            Droids = new List<Droid> ();
        }
    }
}