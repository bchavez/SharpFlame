namespace SharpFlame.Mapping.Tools
{
    public class clsApplyHeightSet : clsAction
    {
        public byte Height;

        private clsMap.clsTerrain Terrain;

        public override void ActionPerform()
        {
            Terrain = Map.Terrain;

            if ( Terrain.Vertices[PosNum.X, PosNum.Y].Height != Height )
            {
                Terrain.Vertices[PosNum.X, PosNum.Y].Height = Height;
                Map.SectorGraphicsChanges.VertexAndNormalsChanged(PosNum);
                Map.SectorUnitHeightsChanges.VertexChanged(PosNum);
                Map.SectorTerrainUndoChanges.VertexChanged(PosNum);
            }
        }
    }
}