namespace SharpFlame.Mapping.Tools
{
    public class clsObjectHealth : clsObjectAction
    {
        public double Health;

        protected override void _ActionPerform()
        {
            ResultUnit.Health = Health;
        }
    }
}