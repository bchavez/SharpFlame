namespace FlaME
{
    using System;

    public class clsFeatureType : clsUnitType
    {
        public clsUnitType.clsAttachment BaseAttachment;
        public string Code;
        public enumFeatureType FeatureType;
        public modLists.ConnectedListLink<clsFeatureType, clsObjectData> FeatureType_ObjectDataLink;
        public modMath.sXY_int Footprint;
        public string Name;

        public clsFeatureType()
        {
            this.FeatureType_ObjectDataLink = new modLists.ConnectedListLink<clsFeatureType, clsObjectData>(this);
            this.Code = "";
            this.Name = "Unknown";
            this.FeatureType = enumFeatureType.Unknown;
            base.Type = clsUnitType.enumType.Feature;
        }

        public override string GetName()
        {
            return this.Name;
        }

        protected override void TypeGLDraw()
        {
            if (this.BaseAttachment != null)
            {
                this.BaseAttachment.GLDraw();
            }
        }

        public enum enumFeatureType : byte
        {
            OilResource = 1,
            Unknown = 0
        }
    }
}

