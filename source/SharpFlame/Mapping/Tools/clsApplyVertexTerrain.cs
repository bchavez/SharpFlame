#region

using SharpFlame.Painters;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsApplyVertexTerrain : clsAction
    {
        private clsTerrain Terrain;
        public Terrain VertexTerrain;

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