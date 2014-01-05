namespace SharpFlame
{
    public partial class clsMap
    {
        public class clsPointChanges
        {
            public bool[,] PointIsChanged;
            public modLists.SimpleList<modMath.clsXY_int> ChangedPoints = new modLists.SimpleList<modMath.clsXY_int>();

            public clsPointChanges(modMath.sXY_int PointSize)
            {
                PointIsChanged = new bool[PointSize.X, PointSize.Y];
                ChangedPoints.MinSize = PointSize.X * PointSize.Y;
                ChangedPoints.Clear();
            }

            public void Changed(modMath.sXY_int Num)
            {
                if ( !PointIsChanged[Num.X, Num.Y] )
                {
                    PointIsChanged[Num.X, Num.Y] = true;
                    ChangedPoints.Add(new modMath.clsXY_int(Num));
                }
            }

            public void SetAllChanged()
            {
                int X = 0;
                int Y = 0;
                modMath.sXY_int Num = new modMath.sXY_int();

                for ( Y = 0; Y <= PointIsChanged.GetUpperBound(1); Y++ )
                {
                    Num.Y = Y;
                    for ( X = 0; X <= PointIsChanged.GetUpperBound(0); X++ )
                    {
                        Num.X = X;
                        Changed(Num);
                    }
                }
            }

            public void Clear()
            {
                modMath.clsXY_int Point = default(modMath.clsXY_int);

                foreach ( modMath.clsXY_int tempLoopVar_Point in ChangedPoints )
                {
                    Point = tempLoopVar_Point;
                    PointIsChanged[Point.X, Point.Y] = false;
                }
                ChangedPoints.Clear();
            }

            public void PerformTool(clsMap.clsAction Tool)
            {
                modMath.clsXY_int Point = default(modMath.clsXY_int);

                foreach ( modMath.clsXY_int tempLoopVar_Point in ChangedPoints )
                {
                    Point = tempLoopVar_Point;
                    Tool.PosNum = Point.XY;
                    Tool.ActionPerform();
                }
            }
        }

        public abstract class clsMapTileChanges : clsPointChanges
        {
            public clsMap Map;
            public clsTerrain Terrain;

            public clsMapTileChanges(clsMap Map, modMath.sXY_int PointSize) : base(PointSize)
            {
                this.Map = Map;
                this.Terrain = Map.Terrain;
            }

            public void Deallocate()
            {
                Map = null;
            }

            public abstract void TileChanged(modMath.sXY_int Num);

            public void VertexChanged(modMath.sXY_int Num)
            {
                if ( Num.X > 0 )
                {
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X - 1, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(new modMath.sXY_int(Num.X - 1, Num.Y));
                    }
                }
                if ( Num.X < Terrain.TileSize.X )
                {
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(Num);
                    }
                }
            }

            public void VertexAndNormalsChanged(modMath.sXY_int Num)
            {
                if ( Num.X > 1 )
                {
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X - 2, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(new modMath.sXY_int(Num.X - 2, Num.Y));
                    }
                }
                if ( Num.X > 0 )
                {
                    if ( Num.Y > 1 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X - 1, Num.Y - 2));
                    }
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X - 1, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(new modMath.sXY_int(Num.X - 1, Num.Y));
                    }
                    if ( Num.Y < Terrain.TileSize.Y - 1 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X - 1, Num.Y + 1));
                    }
                }
                if ( Num.X < Terrain.TileSize.X )
                {
                    if ( Num.Y > 1 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X, Num.Y - 2));
                    }
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(Num);
                    }
                    if ( Num.Y < Terrain.TileSize.Y - 1 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X, Num.Y + 1));
                    }
                }
                if ( Num.X < Terrain.TileSize.X - 1 )
                {
                    if ( Num.Y > 0 )
                    {
                        TileChanged(new modMath.sXY_int(Num.X + 1, Num.Y - 1));
                    }
                    if ( Num.Y < Terrain.TileSize.Y )
                    {
                        TileChanged(new modMath.sXY_int(Num.X + 1, Num.Y));
                    }
                }
            }

            public void SideHChanged(modMath.sXY_int Num)
            {
                if ( Num.Y > 0 )
                {
                    TileChanged(new modMath.sXY_int(Num.X, Num.Y - 1));
                }
                if ( Num.Y < Map.Terrain.TileSize.Y )
                {
                    TileChanged(Num);
                }
            }

            public void SideVChanged(modMath.sXY_int Num)
            {
                if ( Num.X > 0 )
                {
                    TileChanged(new modMath.sXY_int(Num.X - 1, Num.Y));
                }
                if ( Num.X < Map.Terrain.TileSize.X )
                {
                    TileChanged(Num);
                }
            }
        }

        public class clsSectorChanges : clsMap.clsMapTileChanges
        {
            public clsSectorChanges(clsMap Map) : base(Map, Map.SectorCount)
            {
            }

            public override void TileChanged(modMath.sXY_int Num)
            {
                modMath.sXY_int SectorNum = new modMath.sXY_int();

                SectorNum = Map.GetTileSectorNum(Num);
                Changed(SectorNum);
            }
        }

        public class clsAutoTextureChanges : clsMap.clsMapTileChanges
        {
            public clsAutoTextureChanges(clsMap Map) : base(Map, Map.Terrain.TileSize)
            {
            }

            public override void TileChanged(modMath.sXY_int Num)
            {
                Changed(Num);
            }
        }

        public clsSectorChanges SectorGraphicsChanges;
        public clsSectorChanges SectorUnitHeightsChanges;
        public clsSectorChanges SectorTerrainUndoChanges;
        public clsAutoTextureChanges AutoTextureChanges;

        public class clsTerrainUpdate
        {
            public clsPointChanges Vertices;
            public clsPointChanges Tiles;
            public clsPointChanges SidesH;
            public clsPointChanges SidesV;

            public void Deallocate()
            {
            }

            public clsTerrainUpdate(modMath.sXY_int TileSize)
            {
                Vertices = new clsPointChanges(new modMath.sXY_int(TileSize.X + 1, TileSize.Y + 1));
                Tiles = new clsPointChanges(new modMath.sXY_int(TileSize.X, TileSize.Y));
                SidesH = new clsPointChanges(new modMath.sXY_int(TileSize.X, TileSize.Y + 1));
                SidesV = new clsPointChanges(new modMath.sXY_int(TileSize.X + 1, TileSize.Y));
            }

            public void SetAllChanged()
            {
                Vertices.SetAllChanged();
                Tiles.SetAllChanged();
                SidesH.SetAllChanged();
                SidesV.SetAllChanged();
            }

            public void ClearAll()
            {
                Vertices.Clear();
                Tiles.Clear();
                SidesH.Clear();
                SidesV.Clear();
            }
        }

        public clsTerrainUpdate TerrainInterpretChanges;
    }
}