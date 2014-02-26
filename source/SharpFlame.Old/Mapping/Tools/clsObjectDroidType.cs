#region

using SharpFlame.Old.Domain;

#endregion

namespace SharpFlame.Old.Mapping.Tools
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