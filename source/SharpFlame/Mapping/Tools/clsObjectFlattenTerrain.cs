using System;
using SharpFlame.Collections;
using SharpFlame.Core.Domain;
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
            XYInt VertexPos = new XYInt();
            int X = 0;
            int Y = 0;
            double Total = 0;
            byte Average = 0;
            XYInt Footprint = Unit.TypeBase.get_GetFootprintSelected(Unit.Rotation);
            XYInt Start = new XYInt();
            XYInt Finish = new XYInt();
            int Samples = 0;

            Map.GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, ref Start, ref Finish);

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