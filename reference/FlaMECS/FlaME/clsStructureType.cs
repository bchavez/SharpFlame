namespace FlaME
{
    using System;

    public class clsStructureType : clsUnitType
    {
        public clsUnitType.clsAttachment BaseAttachment;
        public string Code;
        public modMath.sXY_int Footprint;
        public string Name;
        public clsModel StructureBasePlate;
        public enumStructureType StructureType;
        public modLists.ConnectedListLink<clsStructureType, clsObjectData> StructureType_ObjectDataLink;
        public modLists.ConnectedListLink<clsStructureType, clsWallType> WallLink;

        public clsStructureType()
        {
            this.StructureType_ObjectDataLink = new modLists.ConnectedListLink<clsStructureType, clsObjectData>(this);
            this.Code = "";
            this.Name = "Unknown";
            this.StructureType = enumStructureType.Unknown;
            this.WallLink = new modLists.ConnectedListLink<clsStructureType, clsWallType>(this);
            this.BaseAttachment = new clsUnitType.clsAttachment();
            base.Type = clsUnitType.enumType.PlayerStructure;
        }

        public override string GetName()
        {
            return this.Name;
        }

        public bool IsModule()
        {
            return (((this.StructureType == enumStructureType.FactoryModule) | (this.StructureType == enumStructureType.PowerModule)) | (this.StructureType == enumStructureType.ResearchModule));
        }

        protected override void TypeGLDraw()
        {
            if (this.BaseAttachment != null)
            {
                this.BaseAttachment.GLDraw();
            }
            if (this.StructureBasePlate != null)
            {
                this.StructureBasePlate.GLDraw();
            }
        }

        public enum enumStructureType : byte
        {
            Command = 7,
            CornerWall = 3,
            CyborgFactory = 5,
            Defense = 9,
            Demolish = 1,
            DOOR = 15,
            Factory = 4,
            FactoryModule = 14,
            HQ = 8,
            MissileSilo = 0x13,
            PowerGenerator = 10,
            PowerModule = 11,
            RearmPad = 0x12,
            RepairFacility = 0x10,
            Research = 12,
            ResearchModule = 13,
            ResourceExtractor = 20,
            SatUplink = 0x11,
            Unknown = 0,
            VTOLFactory = 6,
            Wall = 2
        }
    }
}

