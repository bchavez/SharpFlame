namespace FlaME
{
    using System;

    public class clsBrain : clsTurret
    {
        public modLists.ConnectedListLink<clsBrain, clsObjectData> ObjectDataLink;
        public clsWeapon Weapon;

        public clsBrain()
        {
            this.ObjectDataLink = new modLists.ConnectedListLink<clsBrain, clsObjectData>(this);
            base.TurretType = clsTurret.enumTurretType.Brain;
        }
    }
}

