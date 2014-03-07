namespace SharpFlame.Mapping.Tools
{
    public class clsObjectTurretCount : clsObjectComponent
    {
        public byte TurretCount;

        protected override void ChangeComponent()
        {
            NewDroidType.TurretCount = TurretCount;
        }
    }
}