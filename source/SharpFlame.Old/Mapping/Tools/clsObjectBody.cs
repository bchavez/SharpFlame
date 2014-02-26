#region

using SharpFlame.Old.Domain;

#endregion

namespace SharpFlame.Old.Mapping.Tools
{
    public class clsObjectBody : clsObjectComponent
    {
        public Body Body;

        protected override void ChangeComponent()
        {
            NewDroidType.Body = Body;
        }
    }
}