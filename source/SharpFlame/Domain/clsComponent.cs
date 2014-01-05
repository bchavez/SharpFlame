using SharpFlame.Collections;

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
        public ConnectedListLink<clsBody, clsObjectData> ObjectDataLink;

        public clsUnitType.clsAttachment Attachment = new clsUnitType.clsAttachment();
        public int Hitpoints;

        public clsBody()
        {
            ObjectDataLink = new ConnectedListLink<clsBody, clsObjectData>(this);


            ComponentType = enumComponentType.Body;
        }
    }

    public class clsPropulsion : clsComponent
    {
        public ConnectedListLink<clsPropulsion, clsObjectData> ObjectDataLink;

        public struct sBody
        {
            public clsUnitType.clsAttachment LeftAttachment;
            public clsUnitType.clsAttachment RightAttachment;
        }

        public sBody[] Bodies = new sBody[0];
        public int HitPoints;

        public clsPropulsion(int BodyCount)
        {
            ObjectDataLink = new ConnectedListLink<clsPropulsion, clsObjectData>(this);


            ComponentType = enumComponentType.Propulsion;

            int A = 0;

            Bodies = new sBody[BodyCount];
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
            TurretObjectDataLink = new ConnectedListLink<clsTurret, clsObjectData>(this);
        }

        public ConnectedListLink<clsTurret, clsObjectData> TurretObjectDataLink;

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
        public ConnectedListLink<clsWeapon, clsObjectData> ObjectDataLink;

        public clsWeapon()
        {
            ObjectDataLink = new ConnectedListLink<clsWeapon, clsObjectData>(this);


            TurretType = enumTurretType.Weapon;
        }
    }

    public class clsConstruct : clsTurret
    {
        public ConnectedListLink<clsConstruct, clsObjectData> ObjectDataLink;

        public clsConstruct()
        {
            ObjectDataLink = new ConnectedListLink<clsConstruct, clsObjectData>(this);


            TurretType = enumTurretType.Construct;
        }
    }

    public class clsRepair : clsTurret
    {
        public ConnectedListLink<clsRepair, clsObjectData> ObjectDataLink;

        public clsRepair()
        {
            ObjectDataLink = new ConnectedListLink<clsRepair, clsObjectData>(this);


            TurretType = enumTurretType.Repair;
        }
    }

    public class clsSensor : clsTurret
    {
        public ConnectedListLink<clsSensor, clsObjectData> ObjectDataLink;

        public enum enumLocation
        {
            Unspecified,
            Turret,
            Invisible
        }

        public enumLocation Location = enumLocation.Unspecified;

        public clsSensor()
        {
            ObjectDataLink = new ConnectedListLink<clsSensor, clsObjectData>(this);


            TurretType = enumTurretType.Sensor;
        }
    }

    public class clsBrain : clsTurret
    {
        public ConnectedListLink<clsBrain, clsObjectData> ObjectDataLink;

        public clsWeapon Weapon;

        public clsBrain()
        {
            ObjectDataLink = new ConnectedListLink<clsBrain, clsObjectData>(this);


            TurretType = enumTurretType.Brain;
        }
    }

    public class clsECM : clsTurret
    {
        public ConnectedListLink<clsECM, clsObjectData> ObjectDataLink;

        public clsECM()
        {
            ObjectDataLink = new ConnectedListLink<clsECM, clsObjectData>(this);


            TurretType = enumTurretType.ECM;
        }
    }
}