using SharpFlame.Maths;

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectPosOffset : clsObjectAction
    {
        public sXY_int Offset;

        private sXY_int NewPos;

        protected override void _ActionPerform()
        {
            NewPos.X = Unit.Pos.Horizontal.X + Offset.X;
            NewPos.Y = Unit.Pos.Horizontal.Y + Offset.Y;
            ResultUnit.Pos = Map.TileAlignedPosFromMapPos(NewPos, ResultUnit.TypeBase.get_GetFootprintSelected(Unit.Rotation));
        }
    }
}