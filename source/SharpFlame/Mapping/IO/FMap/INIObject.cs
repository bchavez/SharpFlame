#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;

#endregion

namespace SharpFlame.Mapping.IO.FMap
{
    public class INIObject
    {
        public string BodyCode;
        public string Code;
        public bool GotAltitude;
        public double Heading;
        public double Health;
        public UInt32 ID;
        public bool IsTemplate;
        public string Label;
        public XYInt Pos;
        public int Priority;
        public string PropulsionCode;
        public DroidDesign.clsTemplateDroidType TemplateDroidType;
        public string[] TurretCodes;
        public int TurretCount;
        public TurretType[] TurretTypes;
        public UnitType Type;
        public string UnitGroup;
        public int WallType;

		public INIObject()
		{
			Pos = new XYInt (0, 0);
		}
    }
}