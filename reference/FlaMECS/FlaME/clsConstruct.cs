namespace FlaME
{
    using System;

    public class clsConstruct : clsTurret
    {
        public modLists.ConnectedListLink<clsConstruct, clsObjectData> ObjectDataLink;

        public clsConstruct()
        {
            this.ObjectDataLink = new modLists.ConnectedListLink<clsConstruct, clsObjectData>(this);
            base.TurretType = clsTurret.enumTurretType.Construct;
        }
    }
}

