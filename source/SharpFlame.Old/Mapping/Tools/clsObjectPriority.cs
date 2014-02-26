namespace SharpFlame.Old.Mapping.Tools
{
    public class clsObjectPriority : clsObjectAction
    {
        public int Priority;

        protected override void _ActionPerform()
        {
            ResultUnit.SavePriority = Priority;
        }
    }
}