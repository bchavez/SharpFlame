namespace FlaME
{
    using System;

    public class clsECM : clsTurret
    {
        public modLists.ConnectedListLink<clsECM, clsObjectData> ObjectDataLink;

        public clsECM()
        {
            this.ObjectDataLink = new modLists.ConnectedListLink<clsECM, clsObjectData>(this);
            base.TurretType = clsTurret.enumTurretType.ECM;
        }
    }
}

