#region

using SharpFlame.Domain;

#endregion

namespace SharpFlame.Mapping.Tools
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