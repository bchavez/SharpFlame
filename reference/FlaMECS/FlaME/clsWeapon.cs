namespace FlaME
{
    using System;

    public class clsWeapon : clsTurret
    {
        public modLists.ConnectedListLink<clsWeapon, clsObjectData> ObjectDataLink;

        public clsWeapon()
        {
            this.ObjectDataLink = new modLists.ConnectedListLink<clsWeapon, clsObjectData>(this);
            base.TurretType = clsTurret.enumTurretType.Weapon;
        }
    }
}

