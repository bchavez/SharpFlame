using SharpFlame.Domain;

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectTurret : clsObjectComponent
    {
        public Turret Turret;
        public int TurretNum;

        protected override void ChangeComponent()
        {
            switch ( TurretNum )
            {
                case 0:
                    NewDroidType.Turret1 = Turret;
                    break;
                case 1:
                    NewDroidType.Turret2 = Turret;
                    break;
                case 2:
                    NewDroidType.Turret3 = Turret;
                    break;
            }
        }
    }
}