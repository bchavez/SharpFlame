namespace SharpFlame
{
    public abstract class clsComponent
    {
        public bool IsUnknown = false;

        public string Name = "Unknown";
        public string Code;

        public enum enumComponentType
        {
            Unspecified,
            Body,
            Propulsion,
            Turret
        }

        public enumComponentType ComponentType = enumComponentType.Unspecified;

        public bool Designable;
    }

    public class clsBody : clsComponent
    {
        public modLists.ConnectedListLink<clsBody, clsObjectData> ObjectDataLink;

        public clsUnitType.clsAttachment Attachment = new clsUnitType.clsAttachment();
        public int Hitpoints;

        public clsBody()
        {
            ObjectDataLink = new modLists.ConnectedListLink<clsBody, clsObjectData>(this);


            ComponentType = enumComponentType.Body;
        }
    }

    public class clsPropulsion : clsComponent
    {
        public modLists.ConnectedListLink<clsPropulsion, clsObjectData> ObjectDataLink;

        public struct sBody
        {
            public clsUnitType.clsAttachment LeftAttachment;
            public clsUnitType.clsAttachment RightAttachment;
        }

        public sBody[] Bodies = new sBody[0];
        public int HitPoints;

        public clsPropulsion(int BodyCount)
        {
            ObjectDataLink = new modLists.ConnectedListLink<clsPropulsion, clsObjectData>(this);


            ComponentType = enumComponentType.Propulsion;

            int A = 0;

            Bodies = new sBody[BodyCount - 1 + 1];
            for ( A = 0; A <= BodyCount - 1; A++ )
            {
                Bodies[A].LeftAttachment = new clsUnitType.clsAttachment();
                Bodies[A].RightAttachment = new clsUnitType.clsAttachment();
            }
        }
    }

    public class clsTurret : clsComponent
    {
        public clsTurret()
        {
            TurretObjectDataLink = new modLists.ConnectedListLink<clsTurret, clsObjectData>(this);
        }

        public modLists.ConnectedListLink<clsTurret, clsObjectData> TurretObjectDataLink;

        public clsUnitType.clsAttachment Attachment = new clsUnitType.clsAttachment();
        public int HitPoints;

        public enum enumTurretType
        {
            Unknown,
            Weapon,
            Construct,
            Repair,
            Sensor,
            Brain,
            ECM
        }

        public enumTurretType TurretType = enumTurretType.Unknown;

        public bool GetTurretTypeName(ref string Result)
        {
            switch ( TurretType )
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
                default:
                    Result = null;
                    return false;
            }
        }
    }

    public class clsWeapon : clsTurret
    {
        public modLists.ConnectedListLink<clsWeapon, clsObjectData> ObjectDataLink;

        public clsWeapon()
        {
            ObjectDataLink = new modLists.ConnectedListLink<clsWeapon, clsObjectData>(this);


            TurretType = enumTurretType.Weapon;
        }
    }

    public class clsConstruct : clsTurret
    {
        public modLists.ConnectedListLink<clsConstruct, clsObjectData> ObjectDataLink;

        public clsConstruct()
        {
            ObjectDataLink = new modLists.ConnectedListLink<clsConstruct, clsObjectData>(this);


            TurretType = enumTurretType.Construct;
        }
    }

    public class clsRepair : clsTurret
    {
        public modLists.ConnectedListLink<clsRepair, clsObjectData> ObjectDataLink;

        public clsRepair()
        {
            ObjectDataLink = new modLists.ConnectedListLink<clsRepair, clsObjectData>(this);


            TurretType = enumTurretType.Repair;
        }
    }

    public class clsSensor : clsTurret
    {
        public modLists.ConnectedListLink<clsSensor, clsObjectData> ObjectDataLink;

        public enum enumLocation
        {
            Unspecified,
            Turret,
            Invisible
        }

        public enumLocation Location = enumLocation.Unspecified;

        public clsSensor()
        {
            ObjectDataLink = new modLists.ConnectedListLink<clsSensor, clsObjectData>(this);


            TurretType = enumTurretType.Sensor;
        }
    }

    public class clsBrain : clsTurret
    {
        public modLists.ConnectedListLink<clsBrain, clsObjectData> ObjectDataLink;

        public clsWeapon Weapon;

        public clsBrain()
        {
            ObjectDataLink = new modLists.ConnectedListLink<clsBrain, clsObjectData>(this);


            TurretType = enumTurretType.Brain;
        }
    }

    public class clsECM : clsTurret
    {
        public modLists.ConnectedListLink<clsECM, clsObjectData> ObjectDataLink;

        public clsECM()
        {
            ObjectDataLink = new modLists.ConnectedListLink<clsECM, clsObjectData>(this);


            TurretType = enumTurretType.ECM;
        }
    }
}