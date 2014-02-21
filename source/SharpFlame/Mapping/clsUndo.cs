#region

using SharpFlame.Collections;
using SharpFlame.Core.Collections;

#endregion

namespace SharpFlame.Mapping
{
    public class clsUndo
    {
        public SimpleList<clsShadowSector> ChangedSectors = new SimpleList<clsShadowSector>();
        public SimpleList<clsGatewayChange> GatewayChanges = new SimpleList<clsGatewayChange>();
        public string Name;
        public SimpleList<clsUnitChange> UnitChanges = new SimpleList<clsUnitChange>();
    }
}