namespace FlaME
{
    using System;

    public class clsSensor : clsTurret
    {
        public enumLocation Location;
        public modLists.ConnectedListLink<clsSensor, clsObjectData> ObjectDataLink;

        public clsSensor()
        {
            this.ObjectDataLink = new modLists.ConnectedListLink<clsSensor, clsObjectData>(this);
            this.Location = enumLocation.Unspecified;
            base.TurretType = clsTurret.enumTurretType.Sensor;
        }

        public enum enumLocation
        {
            Unspecified,
            Turret,
            Invisible
        }
    }
}

