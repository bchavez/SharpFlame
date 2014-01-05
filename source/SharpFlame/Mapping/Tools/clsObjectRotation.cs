namespace SharpFlame.Mapping.Tools
{
    public class clsObjectRotation : clsObjectAction
    {
        public int Angle;

        protected override void _ActionPerform()
        {
            ResultUnit.Rotation = Angle;
        }
    }
}