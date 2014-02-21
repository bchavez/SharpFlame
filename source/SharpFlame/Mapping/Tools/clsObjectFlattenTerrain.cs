#region

using System;
using SharpFlame.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Objects;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectFlattenTerrain : ISimpleListTool<clsUnit>
    {
        private clsUnit Unit;

        public void ActionPerform()
        {
            var map = Unit.MapLink.Source;
            var vertexPos = new XYInt();
            var x = 0;
            var y = 0;
            double total = 0;
            var footprint = Unit.TypeBase.GetGetFootprintSelected(Unit.Rotation);
            var start = new XYInt();
            var finish = new XYInt();
            var samples = 0;

            map.GetFootprintTileRangeClamped(Unit.Pos.Horizontal, footprint, ref start, ref finish);

            for ( y = start.Y; y <= finish.Y + 1; y++ )
            {
                vertexPos.Y = y;
                for ( x = start.X; x <= finish.X + 1; x++ )
                {
                    vertexPos.X = x;

                    total += map.Terrain.Vertices[vertexPos.X, vertexPos.Y].Height;
                    samples++;
                }
            }

            if ( samples >= 1 )
            {
                var average = (byte)(MathUtil.ClampInt((int)(total / samples), Byte.MinValue, Byte.MaxValue));
                for ( y = start.Y; y <= finish.Y + 1; y++ )
                {
                    vertexPos.Y = y;
                    for ( x = start.X; x <= finish.X + 1; x++ )
                    {
                        vertexPos.X = x;

                        map.Terrain.Vertices[vertexPos.X, vertexPos.Y].Height = average;
                        map.SectorGraphicsChanges.VertexAndNormalsChanged(vertexPos);
                        map.SectorUnitHeightsChanges.VertexChanged(vertexPos);
                        map.SectorTerrainUndoChanges.VertexChanged(vertexPos);
                    }
                }
            }
        }

        public void SetItem(clsUnit item)
        {
            Unit = item;
        }
    }
}