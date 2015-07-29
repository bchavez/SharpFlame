

using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;


namespace SharpFlame.Mapping
{
    public class Gateway
    {
        public ConnectedListItem<Gateway, Map> MapLink;
        public XYInt PosA;
        public XYInt PosB;

        public Gateway()
        {
            MapLink = new ConnectedListItem<Gateway, Map>(this);
        }

        public bool IsOffMap()
        {
            var terrainSize = MapLink.Owner.Terrain.TileSize;

            return PosA.X < 0
                   | PosA.X >= terrainSize.X
                   | PosA.Y < 0
                   | PosA.Y >= terrainSize.Y
                   | PosB.X < 0
                   | PosB.X >= terrainSize.X
                   | PosB.Y < 0
                   | PosB.Y >= terrainSize.Y;
        }

        public void Deallocate()
        {
            MapLink.Deallocate();
        }
    }
}