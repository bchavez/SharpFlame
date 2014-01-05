namespace FlaME
{
    using System;

    public class clsRepair : clsTurret
    {
        public modLists.ConnectedListLink<clsRepair, clsObjectData> ObjectDataLink;

        public clsRepair()
        {
            this.ObjectDataLink = new modLists.ConnectedListLink<clsRepair, clsObjectData>(this);
            base.TurretType = clsTurret.enumTurretType.Repair;
        }
    }
}

