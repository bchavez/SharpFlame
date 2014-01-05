namespace SharpFlame.Mapping.Tools
{
    public class clsObjectAlignment : clsObjectAction
    {
        protected override void _ActionPerform()
        {
            ResultUnit.Pos = Unit.MapLink.Source.TileAlignedPosFromMapPos(Unit.Pos.Horizontal, Unit.TypeBase.get_GetFootprintNew(Unit.Rotation));
        }
    }
}