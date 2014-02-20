#region

using SharpFlame.Domain;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public abstract class clsObjectComponent : clsObjectAction
    {
        protected DroidDesign NewDroidType;
        private DroidDesign OldDroidType;

        protected abstract void ChangeComponent();

        protected override void ActionCondition()
        {
            base.ActionCondition();

            if ( Unit.TypeBase.Type == UnitType.PlayerDroid )
            {
                OldDroidType = (DroidDesign)Unit.TypeBase;
                ActionPerformed = !OldDroidType.IsTemplate;
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

            ChangeComponent();

            NewDroidType.UpdateAttachments();
        }
    }
}