namespace SharpFlame.Mapping.Tools
{
    public class clsApplyVertexTerrain : clsAction
    {
        public Painters.Terrain VertexTerrain;

        private clsMap.clsTerrain Terrain;

        public override void ActionPerform()
        {
            Terrain = Map.Terrain;

            if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain != VertexTerrain )
            {
                Terrain.Vertices[PosNum.X, PosNum.Y].Terrain = VertexTerrain;
                Map.SectorGraphicsChanges.VertexChanged(PosNum);
                Map.SectorTerrainUndoChanges.VertexChanged(PosNum);
                Map.AutoTextureChanges.VertexChanged(PosNum);
            }
        }
    }
}