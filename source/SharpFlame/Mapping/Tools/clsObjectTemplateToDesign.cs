using SharpFlame.Domain;

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectTemplateToDesign : clsObjectAction
    {
        private DroidDesign OldDroidType;
        private DroidDesign NewDroidType;

        protected override void ActionCondition()
        {
            base.ActionCondition();

            if ( Unit.TypeBase.Type == UnitType.PlayerDroid )
            {
                OldDroidType = (DroidDesign)Unit.TypeBase;
                ActionPerformed = OldDroidType.IsTemplate;
            }
            else
            {
                OldDroidType = null;
                ActionPerformed = false;
            }
        }

        protected override void _ActionPerform()
        {
            NewDroidType = new DroidDesign();
            ResultUnit.TypeBase = NewDroidType;
            NewDroidType.CopyDesign(OldDroidType);
            NewDroidType.UpdateAttachments();
        }
    }
}