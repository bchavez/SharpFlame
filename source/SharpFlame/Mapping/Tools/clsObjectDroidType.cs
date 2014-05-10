

using SharpFlame.Domain;


namespace SharpFlame.Mapping.Tools
{
    public class clsObjectDroidType : clsObjectComponent
    {
        public DroidDesign.clsTemplateDroidType DroidType;

        protected override void ChangeComponent()
        {
            NewDroidType.TemplateDroidType = DroidType;
        }
    }
}