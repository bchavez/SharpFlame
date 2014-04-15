#region

using System;
using SharpFlame.Core.Extensions;
using SharpFlame.Maths;

#endregion

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
                (byte)(MathUtil.ClampInt((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + (Rate * Effect).ToInt(), Byte.MinValue, Byte.MaxValue));

            Map.SectorGraphicsChanges.VertexAndNormalsChanged(PosNum);
            Map.SectorUnitHeightsChanges.VertexChanged(PosNum);
            Map.SectorTerrainUndoChanges.VertexChanged(PosNum);
        }
    }
}