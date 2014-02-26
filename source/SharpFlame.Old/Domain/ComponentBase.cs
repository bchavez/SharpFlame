#region

using SharpFlame.Old.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.Old.Domain.ObjData;

#endregion

namespace SharpFlame.Old.Domain
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
        public ConnectedListLink<Body, clsObjectData> ObjectDataLink;

        public Body()
        {
            ObjectDataLink = new ConnectedListLink<Body, clsObjectData>(this);

            ComponentType = ComponentType.Body;
        }
    }

    public class Propulsion : ComponentBase
    {
        public sBody[] Bodies = new sBody[0];
        public int HitPoints;
        public ConnectedListLink<Propulsion, clsObjectData> ObjectDataLink;

        public Propulsion(int BodyCount)
        {
            ObjectDataLink = new ConnectedListLink<Propulsion, clsObjectData>(this);


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
        public ConnectedListLink<Turret, clsObjectData> TurretObjectDataLink;

        public TurretType TurretType = TurretType.Unknown;

        public Turret()
        {
            TurretObjectDataLink = new ConnectedListLink<Turret, clsObjectData>(this);
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
        public ConnectedListLink<Weapon, clsObjectData> ObjectDataLink;

        public Weapon()
        {
            ObjectDataLink = new ConnectedListLink<Weapon, clsObjectData>(this);


            TurretType = TurretType.Weapon;
        }
    }

    public class Construct : Turret
    {
        public ConnectedListLink<Construct, clsObjectData> ObjectDataLink;

        public Construct()
        {
            ObjectDataLink = new ConnectedListLink<Construct, clsObjectData>(this);


            TurretType = TurretType.Construct;
        }
    }

    public class Repair : Turret
    {
        public ConnectedListLink<Repair, clsObjectData> ObjectDataLink;

        public Repair()
        {
            ObjectDataLink = new ConnectedListLink<Repair, clsObjectData>(this);


            TurretType = TurretType.Repair;
        }
    }

    public class Sensor : Turret
    {
        public SensorLocationType Location = SensorLocationType.Unspecified;
        public ConnectedListLink<Sensor, clsObjectData> ObjectDataLink;

        public Sensor()
        {
            ObjectDataLink = new ConnectedListLink<Sensor, clsObjectData>(this);


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
        public ConnectedListLink<Brain, clsObjectData> ObjectDataLink;

        public Weapon Weapon;

        public Brain()
        {
            ObjectDataLink = new ConnectedListLink<Brain, clsObjectData>(this);


            TurretType = TurretType.Brain;
        }
    }

    public class Ecm : Turret
    {
        public ConnectedListLink<Ecm, clsObjectData> ObjectDataLink;

        public Ecm()
        {
            ObjectDataLink = new ConnectedListLink<Ecm, clsObjectData>(this);


            TurretType = TurretType.ECM;
        }
    }
}