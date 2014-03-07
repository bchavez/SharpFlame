namespace SharpFlame.Mapping.Tools
{
    public class clsObjectAlignment : clsObjectAction
    {
        protected override void _ActionPerform()
        {
            ResultUnit.Pos = Unit.MapLink.Source.TileAlignedPosFromMapPos(Unit.Pos.Horizontal, Unit.TypeBase.GetGetFootprintNew(Unit.Rotation));
        }
    }
}