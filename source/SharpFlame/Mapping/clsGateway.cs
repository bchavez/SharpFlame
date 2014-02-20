#region

using SharpFlame.Collections;
using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Mapping
{
    public class clsGateway
    {
        public ConnectedListLink<clsGateway, clsMap> MapLink;
        public XYInt PosA;
        public XYInt PosB;

        public clsGateway()
        {
            MapLink = new ConnectedListLink<clsGateway, clsMap>(this);
        }

        public bool IsOffMap()
        {
            var TerrainSize = MapLink.Source.Terrain.TileSize;

            return PosA.X < 0
                   | PosA.X >= TerrainSize.X
                   | PosA.Y < 0
                   | PosA.Y >= TerrainSize.Y
                   | PosB.X < 0
                   | PosB.X >= TerrainSize.X
                   | PosB.Y < 0
                   | PosB.Y >= TerrainSize.Y;
        }

        public void Deallocate()
        {
            MapLink.Deallocate();
        }
    }
}