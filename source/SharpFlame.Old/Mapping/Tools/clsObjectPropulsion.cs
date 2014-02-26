#region

using SharpFlame.Old.Domain;
using SharpFlame.Old.Domain;

#endregion

namespace SharpFlame.Old.Mapping.Tools
{
    public class clsObjectPropulsion : clsObjectComponent
    {
        public Propulsion Propulsion;

        protected override void ChangeComponent()
        {
            NewDroidType.Propulsion = Propulsion;
        }
    }
}