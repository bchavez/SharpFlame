using System;
using SharpFlame.Mapping.Tools;
using SharpFlame.Core.Domain;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawTileOrientation : clsAction
    {
        public override void ActionPerform()
        {
            int X = 0;
            int Y = 0;

            for ( Y = PosNum.Y * Constants.SectorTileSize; Y <= Math.Min((PosNum.Y + 1) * Constants.SectorTileSize - 1, Map.Terrain.TileSize.Y - 1); Y++ )
            {
                for ( X = PosNum.X * Constants.SectorTileSize; X <= Math.Min((PosNum.X + 1) * Constants.SectorTileSize - 1, Map.Terrain.TileSize.X - 1); X++ )
                {
                    Map.DrawTileOrientation(new XYInt(X, Y));
                }
            }
        }
    }
}