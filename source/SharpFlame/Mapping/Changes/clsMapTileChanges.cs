using SharpFlame.Maths;

namespace SharpFlame.Mapping.Changes
{
    public abstract class clsMapTileChanges : clsPointChanges
    {
        public clsMap Map;
        public clsMap.clsTerrain Terrain;

        public clsMapTileChanges(clsMap Map, sXY_int PointSize) : base(PointSize)
        {
            this.Map = Map;
            Terrain = Map.Terrain;
        }

        public void Deallocate()
        {
            Map = null;
        }

        public abstract void TileChanged(sXY_int Num);

        public void VertexChanged(sXY_int Num)
        {
            if ( Num.X > 0 )
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new sXY_int(Num.X - 1, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new sXY_int(Num.X - 1, Num.Y));
                }
            }
            if ( Num.X < Terrain.TileSize.X )
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new sXY_int(Num.X, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(Num);
                }
            }
        }

        public void VertexAndNormalsChanged(sXY_int Num)
        {
            if ( Num.X > 1 )
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new sXY_int(Num.X - 2, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new sXY_int(Num.X - 2, Num.Y));
                }
            }
            if ( Num.X > 0 )
            {
                if ( Num.Y > 1 )
                {
                    TileChanged(new sXY_int(Num.X - 1, Num.Y - 2));
                }
                if ( Num.Y > 0 )
                {
                    TileChanged(new sXY_int(Num.X - 1, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new sXY_int(Num.X - 1, Num.Y));
                }
                if ( Num.Y < Terrain.TileSize.Y - 1 )
                {
                    TileChanged(new sXY_int(Num.X - 1, Num.Y + 1));
                }
            }
            if ( Num.X < Terrain.TileSize.X )
            {
                if ( Num.Y > 1 )
                {
                    TileChanged(new sXY_int(Num.X, Num.Y - 2));
                }
                if ( Num.Y > 0 )
                {
                    TileChanged(new sXY_int(Num.X, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(Num);
                }
                if ( Num.Y < Terrain.TileSize.Y - 1 )
                {
                    TileChanged(new sXY_int(Num.X, Num.Y + 1));
                }
            }
            if ( Num.X < Terrain.TileSize.X - 1 )
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new sXY_int(Num.X + 1, Num.Y - 1));
                }
                if ( Num.Y < Terrain.TileSize.Y )
                {
                    TileChanged(new sXY_int(Num.X + 1, Num.Y));
                }
            }
        }

        public void SideHChanged(sXY_int Num)
        {
            if ( Num.Y > 0 )
            {
                TileChanged(new sXY_int(Num.X, Num.Y - 1));
            }
            if ( Num.Y < Map.Terrain.TileSize.Y )
            {
                TileChanged(Num);
            }
        }

        public void SideVChanged(sXY_int Num)
        {
            if ( Num.X > 0 )
            {
                TileChanged(new sXY_int(Num.X - 1, Num.Y));
            }
            if ( Num.X < Map.Terrain.TileSize.X )
            {
                TileChanged(Num);
            }
        }
    }
}