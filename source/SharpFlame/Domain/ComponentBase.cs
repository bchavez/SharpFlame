

using SharpFlame.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.Domain.ObjData;


namespace SharpFlame.Domain
{
    public abstract class ComponentBase
    {
        public string Code;

        public ComponentType ComponentType = ComponentType.Unspecified;

        public bool Designable;
        public bool IsUnknown = false;

        public string Name = "Unknown";
    }

    public enum ComponentType
    {
        Unspecified,
        Body,
        Propulsion,
        Turret
    }

    public class Body : ComponentBase
    {
        public clsAttachment Attachment = new clsAttachment();
        public int Hitpoints;
        public ConnectedListItem<Body, ObjectData> ObjectDataLink;

        public Body()
        {
            ObjectDataLink = new ConnectedListItem<Body, ObjectData>(this);

            ComponentType = ComponentType.Body;
        }
    }

    public class Propulsion : ComponentBase
    {
        public sBody[] Bodies = new sBody[0];
        public int HitPoints;
        public ConnectedListItem<Propulsion, ObjectData> ObjectDataLink;

        public Propulsion(int BodyCount)
        {
            ObjectDataLink = new ConnectedListItem<Propulsion, ObjectData>(this);


            ComponentType = ComponentType.Propulsion;

            var A = 0;

            Bodies = new sBody[BodyCount];
            for ( A = 0; A <= BodyCount - 1; A++ )
            {
                Bodies[A].LeftAttachment = new clsAttachment();
                Bodies[A].RightAttachment = new clsAttachment();
            }
        }

        public struct sBody
        {
            public clsAttachment LeftAttachment;
            public clsAttachment RightAttachment;
        }
    }

    public class Turret : ComponentBase
    {
        public clsAttachment Attachment = new clsAttachment();
        public int HitPoints;
        public ConnectedListItem<Turret, ObjectData> TurretObjectDataLink;

        public TurretType TurretType = TurretType.Unknown;

        public Turret()
        {
            TurretObjectDataLink = new ConnectedListItem<Turret, ObjectData>(this);
        }

        public bool GetTurretTypeName(ref string Result)
        {
            switch ( TurretType )
            {
                case TurretType.Weapon:
                    Result = "Weapon";
                    return true;
                case TurretType.Construct:
                    Result = "Construct";
                    return true;
                case TurretType.Repair:
                    Result = "Repair";
                    return true;
                case TurretType.Sensor:
                    Result = "Sensor";
                    return true;
                case TurretType.Brain:
                    Result = "Brain";
                    return true;
                case TurretType.ECM:
                    Result = "ECM";
                    return true;
                default:
                    Result = null;
                    return false;
            }
        }
    }

    public enum TurretType
    {
        Unknown,
        Weapon,
        Construct,
        Repair,
        Sensor,
        Brain,
        ECM
    }

    public class Weapon : Turret
    {
        public ConnectedListItem<Weapon, ObjectData> ObjectDataLink;

        public Weapon()
        {
            ObjectDataLink = new ConnectedListItem<Weapon, ObjectData>(this);


            TurretType = TurretType.Weapon;
        }
    }

    public class Construct : Turret
    {
        public ConnectedListItem<Construct, ObjectData> ObjectDataLink;

        public Construct()
        {
            ObjectDataLink = new ConnectedListItem<Construct, ObjectData>(this);


            TurretType = TurretType.Construct;
        }
    }

    public class Repair : Turret
    {
        public ConnectedListItem<Repair, ObjectData> ObjectDataLink;

        public Repair()
        {
            ObjectDataLink = new ConnectedListItem<Repair, ObjectData>(this);


            TurretType = TurretType.Repair;
        }
    }

    public class Sensor : Turret
    {
        public SensorLocationType Location = SensorLocationType.Unspecified;
        public ConnectedListItem<Sensor, ObjectData> ObjectDataLink;

        public Sensor()
        {
            ObjectDataLink = new ConnectedListItem<Sensor, ObjectData>(this);


            TurretType = TurretType.Sensor;
        }
    }

    public enum SensorLocationType
    {
        Unspecified,
        Turret,
        Invisible
    }

    public class Brain : Turret
    {
        public ConnectedListItem<Brain, ObjectData> ObjectDataLink;

        public Weapon Weapon;

        public Brain()
        {
            ObjectDataLink = new ConnectedListItem<Brain, ObjectData>(this);


            TurretType = TurretType.Brain;
        }
    }

    public class Ecm : Turret
    {
        public ConnectedListItem<Ecm, ObjectData> ObjectDataLink;

        public Ecm()
        {
            ObjectDataLink = new ConnectedListItem<Ecm, ObjectData>(this);


            TurretType = TurretType.ECM;
        }
    }
}