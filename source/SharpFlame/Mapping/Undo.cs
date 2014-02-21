#region

using SharpFlame.Collections;
using SharpFlame.Core.Collections;

#endregion

namespace SharpFlame.Mapping
{
    public class Undo
    {
        public SimpleList<clsShadowSector> ChangedSectors = new SimpleList<clsShadowSector>();
        public SimpleList<GatewayChange> GatewayChanges = new SimpleList<GatewayChange>();
        public string Name;
        public SimpleList<clsUnitChange> UnitChanges = new SimpleList<clsUnitChange>();
    }
}