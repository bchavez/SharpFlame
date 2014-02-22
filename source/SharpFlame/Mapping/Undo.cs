#region

using SharpFlame.Collections;
using SharpFlame.Core.Collections;

#endregion

namespace SharpFlame.Mapping
{
    public class Undo
    {
        public SimpleList<ShadowSector> ChangedSectors = new SimpleList<ShadowSector>();
        public SimpleList<GatewayChange> GatewayChanges = new SimpleList<GatewayChange>();
        public string Name;
        public SimpleList<UnitChange> UnitChanges = new SimpleList<UnitChange>();
    }
}