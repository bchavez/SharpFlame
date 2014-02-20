namespace SharpFlame.Mapping.Tools
{
    public class clsApplyCliffTriangleRemove : clsAction
    {
        private bool CliffChanged;
        private clsTerrain Terrain;
        public bool Triangle;

        public override void ActionPerform()
        {
            Terrain = Map.Terrain;

            CliffChanged = false;
            if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri )
            {
                if ( Triangle )
                {
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;
                        CliffChanged = true;
                    }
                }
                else
                {
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
                        CliffChanged = true;
                    }
                }
                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff ||
                                                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff;
            }
            else
            {
                if ( Triangle )
                {
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;
                        CliffChanged = true;
                    }
                }
                else
                {
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
                        CliffChanged = true;
                    }
                }
                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff ||
                                                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff;
            }

            if ( !CliffChanged )
            {
                return;
            }

            Map.AutoTextureChanges.TileChanged(PosNum);
            Map.SectorGraphicsChanges.TileChanged(PosNum);
            Map.SectorTerrainUndoChanges.TileChanged(PosNum);
        }
    }
}