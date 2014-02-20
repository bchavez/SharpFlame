#region

using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Mapping.Changes
{
    public abstract class clsMapTileChanges : clsPointChanges
    {
        public clsMap Map;
        public clsTerrain Terrain;

        public clsMapTileChanges(clsMap Map, XYInt PointSize) : base(PointSize)
        {
            this.Map = Map;
            Terrain = Map.Terrain;
        }

        public void Deallocate()
        {
            Map = null;
        }

        public abstract void TileChanged(XYInt Num);

        public void VertexChanged(XYInt Num)
        {
            if ( Num.X > 0 )
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new XYInt(Num.X - 1, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new XYInt(Num.X - 1, Num.Y));
                }
            }
            if ( Num.X < Terrain.TileSize.X )
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new XYInt(Num.X, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(Num);
                }
            }
        }

        public void VertexAndNormalsChanged(XYInt Num)
        {
            if ( Num.X > 1 )
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new XYInt(Num.X - 2, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new XYInt(Num.X - 2, Num.Y));
                }
            }
            if ( Num.X > 0 )
            {
                if ( Num.Y > 1 )
                {
                    TileChanged(new XYInt(Num.X - 1, Num.Y - 2));
                }
                if ( Num.Y > 0 )
                {
                    TileChanged(new XYInt(Num.X - 1, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new XYInt(Num.X - 1, Num.Y));
                }
                if ( Num.Y < Terrain.TileSize.Y - 1 )
                {
                    TileChanged(new XYInt(Num.X - 1, Num.Y + 1));
                }
            }
            if ( Num.X < Terrain.TileSize.X )
            {
                if ( Num.Y > 1 )
                {
                    TileChanged(new XYInt(Num.X, Num.Y - 2));
                }
                if ( Num.Y > 0 )
                {
                    TileChanged(new XYInt(Num.X, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(Num);
                }
                if ( Num.Y < Terrain.TileSize.Y - 1 )
                {
                    TileChanged(new XYInt(Num.X, Num.Y + 1));
                }
            }
            if ( Num.X < Terrain.TileSize.X - 1 )
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new XYInt(Num.X + 1, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new XYInt(Num.X + 1, Num.Y));
                }
            }
        }

        public void SideHChanged(XYInt Num)
        {
            if ( Num.Y > 0 )
            {
                TileChanged(new XYInt(Num.X, Num.Y - 1));
            }
            if ( Num.Y < Map.Terrain.TileSize.Y )
            {
                TileChanged(Num);
            }
        }

        public void SideVChanged(XYInt Num)
        {
            if ( Num.X > 0 )
            {
                TileChanged(new XYInt(Num.X - 1, Num.Y));
            }
            if ( Num.X < Map.Terrain.TileSize.X )
            {
                TileChanged(Num);
            }
        }
    }
}