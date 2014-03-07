#region

using System;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsApplyAutoTri : clsAction
    {
        private bool NewTri;
        private double difA;
        private double difB;

        public override void ActionPerform()
        {
            //tri set to the direction where the diagonal edge will be the flattest, so that cliff edges are level
            difA = Math.Abs((Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) - Map.Terrain.Vertices[PosNum.X, PosNum.Y].Height);
            difB = Math.Abs((Map.Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) - Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height);
            if ( difA == difB )
            {
                NewTri = App.Random.Next() < 0.5F;
            }
            else
            {
                NewTri = difA > difB;
            }
            Map.Terrain.Tiles[PosNum.X, PosNum.Y].Tri = NewTri;

            Map.SectorGraphicsChanges.TileChanged(PosNum);
            Map.SectorUnitHeightsChanges.TileChanged(PosNum);
            Map.SectorTerrainUndoChanges.TileChanged(PosNum);
        }
    }
}