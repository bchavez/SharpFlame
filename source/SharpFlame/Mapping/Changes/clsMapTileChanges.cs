#region

using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Mapping.Changes
{
    public abstract class clsMapTileChanges : clsPointChanges
    {
        public Map Map;
        public clsTerrain Terrain;

        public clsMapTileChanges(Map map, XYInt pointsize) : base(pointsize)
        {
            Map = map;
            Terrain = map.Terrain;
        }

        public void Deallocate()
        {
            Map = null;
        }

        public abstract void TileChanged(XYInt num);

        public void VertexChanged(XYInt num)
        {
            if ( num.X > 0 )
            {
                if ( num.Y > 0 )
                {
                    TileChanged(new XYInt(num.X - 1, num.Y - 1));
                }
                if ( num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new XYInt(num.X - 1, num.Y));
                }
            }
            if ( num.X < Terrain.TileSize.X )
            {
                if ( num.Y > 0 )
                {
                    TileChanged(new XYInt(num.X, num.Y - 1));
                }
                if ( num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(num);
                }
            }
        }

        public void VertexAndNormalsChanged(XYInt num)
        {
            if ( num.X > 1 )
            {
                if ( num.Y > 0 )
                {
                    TileChanged(new XYInt(num.X - 2, num.Y - 1));
                }
                if ( num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new XYInt(num.X - 2, num.Y));
                }
            }
            if ( num.X > 0 )
            {
                if ( num.Y > 1 )
                {
                    TileChanged(new XYInt(num.X - 1, num.Y - 2));
                }
                if ( num.Y > 0 )
                {
                    TileChanged(new XYInt(num.X - 1, num.Y - 1));
                }
                if ( num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new XYInt(num.X - 1, num.Y));
                }
                if ( num.Y < Terrain.TileSize.Y - 1 )
                {
                    TileChanged(new XYInt(num.X - 1, num.Y + 1));
                }
            }
            if ( num.X < Terrain.TileSize.X )
            {
                if ( num.Y > 1 )
                {
                    TileChanged(new XYInt(num.X, num.Y - 2));
                }
                if ( num.Y > 0 )
                {
                    TileChanged(new XYInt(num.X, num.Y - 1));
                }
                if ( num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(num);
                }
                if ( num.Y < Terrain.TileSize.Y - 1 )
                {
                    TileChanged(new XYInt(num.X, num.Y + 1));
                }
            }
            if ( num.X < Terrain.TileSize.X - 1 )
            {
                if ( num.Y > 0 )
                {
                    TileChanged(new XYInt(num.X + 1, num.Y - 1));
                }
                if ( num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new XYInt(num.X + 1, num.Y));
                }
            }
        }

        public void SideHChanged(XYInt num)
        {
            if ( num.Y > 0 )
            {
                TileChanged(new XYInt(num.X, num.Y - 1));
            }
            if ( num.Y < Map.Terrain.TileSize.Y )
            {
                TileChanged(num);
            }
        }

        public void SideVChanged(XYInt num)
        {
            if ( num.X > 0 )
            {
                TileChanged(new XYInt(num.X - 1, num.Y));
            }
            if ( num.X < Map.Terrain.TileSize.X )
            {
                TileChanged(num);
            }
        }
    }
}