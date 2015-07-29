namespace SharpFlame.Mapping.Tools
{
    public class clsObjectAlignment : clsObjectAction
    {
        protected override void _ActionPerform()
        {
            ResultUnit.Pos = Unit.MapLink.Owner.TileAlignedPosFromMapPos(Unit.Pos.Horizontal, Unit.TypeBase.GetGetFootprintNew(Unit.Rotation));
        }
    }
}