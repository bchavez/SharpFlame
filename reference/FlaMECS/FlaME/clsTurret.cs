namespace FlaME
{
    using System;

    public class clsTurret : clsComponent
    {
        public clsUnitType.clsAttachment Attachment;
        public int HitPoints;
        public modLists.ConnectedListLink<clsTurret, clsObjectData> TurretObjectDataLink;
        public enumTurretType TurretType;

        public clsTurret()
        {
            this.TurretObjectDataLink = new modLists.ConnectedListLink<clsTurret, clsObjectData>(this);
            this.Attachment = new clsUnitType.clsAttachment();
            this.TurretType = enumTurretType.Unknown;
        }

        public bool GetTurretTypeName(ref string Result)
        {
            switch (this.TurretType)
            {
                case enumTurretType.Weapon:
                    Result = "Weapon";
                    return true;

                case enumTurretType.Construct:
                    Result = "Construct";
                    return true;

                case enumTurretType.Repair:
                    Result = "Repair";
                    return true;

                case enumTurretType.Sensor:
                    Result = "Sensor";
                    return true;

                case enumTurretType.Brain:
                    Result = "Brain";
                    return true;

                case enumTurretType.ECM:
                    Result = "ECM";
                    return true;
            }
            Result = null;
            return false;
        }

        public enum enumTurretType : byte
        {
            Brain = 5,
            Construct = 2,
            ECM = 6,
            Repair = 3,
            Sensor = 4,
            Unknown = 0,
            Weapon = 1
        }
    }
}

