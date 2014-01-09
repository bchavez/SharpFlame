using SharpFlame.Collections;

namespace SharpFlame.Mapping
{
    public class clsUndo
    {
        public string Name;
        public SimpleList<clsShadowSector> ChangedSectors = new SimpleList<clsShadowSector>();
        public SimpleList<clsUnitChange> UnitChanges = new SimpleList<clsUnitChange>();
        public SimpleList<clsGatewayChange> GatewayChanges = new SimpleList<clsGatewayChange>();
    }
}