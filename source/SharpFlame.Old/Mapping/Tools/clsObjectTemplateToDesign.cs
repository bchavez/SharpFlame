#region

using SharpFlame.Old.Domain;

#endregion

namespace SharpFlame.Old.Mapping.Tools
{
    public class clsObjectTemplateToDesign : clsObjectAction
    {
        private DroidDesign NewDroidType;
        private DroidDesign OldDroidType;

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