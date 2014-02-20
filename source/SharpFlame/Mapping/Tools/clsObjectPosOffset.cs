#region

using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectPosOffset : clsObjectAction
    {
        private XYInt NewPos;
        public XYInt Offset;

        protected override void _ActionPerform()
        {
            NewPos.X = Unit.Pos.Horizontal.X + Offset.X;
            NewPos.Y = Unit.Pos.Horizontal.Y + Offset.Y;
            ResultUnit.Pos = Map.TileAlignedPosFromMapPos(NewPos, ResultUnit.TypeBase.get_GetFootprintSelected(Unit.Rotation));
        }
    }
}