using System;
using SharpFlame.Collections;
using SharpFlame.Mapping.Objects;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectFlattenTerrain : SimpleListTool<clsUnit>
    {
        private clsUnit Unit;

        public void ActionPerform()
        {
            clsMap Map = Unit.MapLink.Source;
            sXY_int VertexPos = new sXY_int();
            int X = 0;
            int Y = 0;
            double Total = 0;
            byte Average = 0;
            sXY_int Footprint = Unit.TypeBase.get_GetFootprintSelected(Unit.Rotation);
            sXY_int Start = new sXY_int();
            sXY_int Finish = new sXY_int();
            int Samples = 0;

            Map.GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, Start, Finish);

            for ( Y = Start.Y; Y <= Finish.Y + 1; Y++ )
            {
                VertexPos.Y = Y;
                for ( X = Start.X; X <= Finish.X + 1; X++ )
                {
                    VertexPos.X = X;

                    Total += Map.Terrain.Vertices[VertexPos.X, VertexPos.Y].Height;
                    Samples++;
                }
            }

            if ( Samples >= 1 )
            {
                Average = (byte)(MathUtil.Clamp_int((int)(Total / Samples), Byte.MinValue, Byte.MaxValue));
                for ( Y = Start.Y; Y <= Finish.Y + 1; Y++ )
                {
                    VertexPos.Y = Y;
                    for ( X = Start.X; X <= Finish.X + 1; X++ )
                    {
                        VertexPos.X = X;

                        Map.Terrain.Vertices[VertexPos.X, VertexPos.Y].Height = Average;
                        Map.SectorGraphicsChanges.VertexAndNormalsChanged(VertexPos);
                        Map.SectorUnitHeightsChanges.VertexChanged(VertexPos);
                        Map.SectorTerrainUndoChanges.VertexChanged(VertexPos);
                    }
                }
            }
        }

        public void SetItem(clsUnit Item)
        {
            Unit = Item;
        }
    }
}