

using System.Collections.ObjectModel;
using SharpFlame.Collections;
using SharpFlame.Core.Collections;


namespace SharpFlame.Mapping
{
    public class Undo
    {
        public ObservableCollection<ShadowSector> ChangedSectors = new ObservableCollection<ShadowSector>();
        public ObservableCollection<GatewayChange> GatewayChanges = new ObservableCollection<GatewayChange>();
        public string Name;
        public ObservableCollection<UnitChange> UnitChanges = new ObservableCollection<UnitChange>();
    }
}