using SharpFlame.Domain;

namespace SharpFlame.Mapping.Tools
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