using SharpFlame.Maths;

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectRotationOffset : clsObjectAction
    {
        public int Offset;

        private sXY_int NewPos;

        protected override void _ActionPerform()
        {
            ResultUnit.Rotation = Unit.Rotation + Offset;
            if ( ResultUnit.Rotation < 0 )
            {
                ResultUnit.Rotation += 360;
            }
            else if ( ResultUnit.Rotation >= 360 )
            {
                ResultUnit.Rotation -= 360;
            }
        }
    }
}