namespace SharpFlame.Old.Mapping.Tools
{
    public class clsApplyCliffRemove : clsAction
    {
        private clsTerrain Terrain;

        public override void ActionPerform()
        {
            Terrain = Map.Terrain;

            if ( Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff || Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff ||
                 Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff || Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff ||
                 Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
            {
                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;

                Map.AutoTextureChanges.TileChanged(PosNum);
                Map.SectorGraphicsChanges.TileChanged(PosNum);
                Map.SectorTerrainUndoChanges.TileChanged(PosNum);
            }
        }
    }
}