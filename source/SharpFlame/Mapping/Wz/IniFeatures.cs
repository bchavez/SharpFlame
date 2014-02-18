using System;
using System.Collections.Generic;
using SharpFlame.Util;
using SharpFlame.Core.Domain;

namespace SharpFlame.Mapping.Wz
{
    public class IniFeatures
    {
        public class Feature
        {
            public UInt32 ID;
            public string Code;
            public XYZInt Pos;
			public Rotation Rotation;
            public int HealthPercent;
        }

        public List<Feature>Features;
        public int FeatureCount {
            get { return Features.Count; }
        }

        public IniFeatures()
        {
            Features = new List<Feature> ();
        }
    }
}