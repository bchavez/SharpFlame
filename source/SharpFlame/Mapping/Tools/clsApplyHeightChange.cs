using System;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Tools
{
    public class clsApplyHeightChange : clsAction
    {
        public double Rate;

        private clsTerrain Terrain;

        public override void ActionPerform()
        {
            Terrain = Map.Terrain;

            Terrain.Vertices[PosNum.X, PosNum.Y].Height =
                (byte)(MathUtil.Clamp_int((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + (int)(Rate * Effect), Byte.MinValue, Byte.MaxValue));

            Map.SectorGraphicsChanges.VertexAndNormalsChanged(PosNum);
            Map.SectorUnitHeightsChanges.VertexChanged(PosNum);
            Map.SectorTerrainUndoChanges.VertexChanged(PosNum);
        }
    }
}