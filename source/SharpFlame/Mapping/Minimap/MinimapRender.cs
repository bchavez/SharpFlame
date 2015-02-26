using System;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Domain;
using SharpFlame.Mapping.Objects;
using SharpFlame.Maths;
using SharpFlame.MouseTools;

namespace SharpFlame.Mapping.Minimap
{
    public class MinimapRender
    {
        private static SRgb GetUnitGroupColour(clsUnitGroup colourUnitGroup)
        {
            if( colourUnitGroup.WZ_StartPos < 0 )
            {
                return new SRgb(1.0F, 1.0F, 1.0F);
            }
            return App.PlayerColour[colourUnitGroup.WZ_StartPos].MinimapColour;
        }
        public static void FillTexture(MinimapTexture texture, Map myMap, MinimapOpts miniOpts, Settings.SettingsManager settings)
        {
            if( myMap == null )
            {
                return;
            }

            var terrain = myMap.Terrain;
            var tileset = myMap.Tileset;
            var units = myMap.Units;
            var gateways = myMap.Gateways;

            var low = new XYInt();
            var high = new XYInt();
            var footprint = new XYInt();
            var flag = default(bool);
            var unitMap = new bool[texture.Size.Y, texture.Size.X];
            var sngTexture = new float[texture.Size.Y, texture.Size.X, 3];
            float alpha = 0;
            float antiAlpha = 0;
            var rGBSng = new SRgb();

            if( miniOpts.Textures )
            {
                if( tileset != null )
                {
                    for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
                    {
                        for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                        {
                            if( terrain.Tiles[x, y].Texture.TextureNum >= 0 && terrain.Tiles[x, y].Texture.TextureNum < tileset.Tiles.Count )
                            {
                                sngTexture[y, x, 0] = tileset.Tiles[terrain.Tiles[x, y].Texture.TextureNum].AverageColour.Red;
                                sngTexture[y, x, 1] = tileset.Tiles[terrain.Tiles[x, y].Texture.TextureNum].AverageColour.Green;
                                sngTexture[y, x, 2] = tileset.Tiles[terrain.Tiles[x, y].Texture.TextureNum].AverageColour.Blue;
                            }
                        }
                    }
                }
                if( miniOpts.Heights )
                {
                    float Height = 0;
                    for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
                    {
                        for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                        {
                            Height =
                                Convert.ToSingle(( ( terrain.Vertices[x, y].Height ) + terrain.Vertices[x + 1, y].Height + terrain.Vertices[x, y + 1].Height +
                                                   terrain.Vertices[x + 1, y + 1].Height ) / 1020.0F);
                            sngTexture[y, x, 0] = ( sngTexture[y, x, 0] * 2.0F + Height ) / 3.0F;
                            sngTexture[y, x, 1] = ( sngTexture[y, x, 1] * 2.0F + Height ) / 3.0F;
                            sngTexture[y, x, 2] = ( sngTexture[y, x, 2] * 2.0F + Height ) / 3.0F;
                        }
                    }
                }
            }
            else if( miniOpts.Heights )
            {
                for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
                {
                    for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                    {
                        var height =
                            Convert.ToSingle(( ( terrain.Vertices[x, y].Height ) + terrain.Vertices[x + 1, y].Height + terrain.Vertices[x, y + 1].Height +
                                               terrain.Vertices[x + 1, y + 1].Height ) / 1020.0F);
                        sngTexture[y, x, 0] = height;
                        sngTexture[y, x, 1] = height;
                        sngTexture[y, x, 2] = height;
                    }
                }
            }
            else
            {
                for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
                {
                    for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                    {
                        sngTexture[y, x, 0] = 0.0F;
                        sngTexture[y, x, 1] = 0.0F;
                        sngTexture[y, x, 2] = 0.0F;
                    }
                }
            }
            if( miniOpts.Cliffs )
            {
                if( tileset != null )
                {
                    alpha = settings.MinimapCliffColour.Alpha;
                    antiAlpha = 1.0F - alpha;
                    for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
                    {
                        for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                        {
                            if( terrain.Tiles[x, y].Texture.TextureNum >= 0 && terrain.Tiles[x, y].Texture.TextureNum < tileset.Tiles.Count )
                            {
                                if( tileset.Tiles[terrain.Tiles[x, y].Texture.TextureNum].DefaultType == Constants.TileTypeNumCliff )
                                {
                                    sngTexture[y, x, 0] = sngTexture[y, x, 0] * antiAlpha + settings.MinimapCliffColour.Red * alpha;
                                    sngTexture[y, x, 1] = sngTexture[y, x, 1] * antiAlpha + settings.MinimapCliffColour.Green * alpha;
                                    sngTexture[y, x, 2] = sngTexture[y, x, 2] * antiAlpha + settings.MinimapCliffColour.Blue * alpha;
                                }
                            }
                        }
                    }
                }
            }
            if( miniOpts.Gateways )
            {
                foreach( var gateway in gateways )
                {
                    MathUtil.ReorderXY(gateway.PosA, gateway.PosB, ref low, ref high);
                    for( var y = low.Y; y <= high.Y; y++ )
                    {
                        for( var x = low.X; x <= high.X; x++ )
                        {
                            sngTexture[y, x, 0] = 1.0F;
                            sngTexture[y, x, 1] = 1.0F;
                            sngTexture[y, x, 2] = 0.0F;
                        }
                    }
                }
            }
            if( miniOpts.Objects )
            {
                //units that are not selected
                foreach( var unit in units )
                {
                    flag = true;
                    if( unit.TypeBase.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        flag = false;
                    }
                    else
                    {
                        footprint = unit.TypeBase.GetGetFootprintSelected(unit.Rotation);
                    }
                    if( flag )
                    {
                        myMap.GetFootprintTileRangeClamped(unit.Pos.Horizontal, footprint, ref low, ref high);
                        for( var y = low.Y; y <= high.Y; y++ )
                        {
                            for( var x = low.X; x <= high.X; x++ )
                            {
                                if( !unitMap[y, x] )
                                {
                                    unitMap[y, x] = true;
                                    if( settings.MinimapTeamColours )
                                    {
                                        if( settings.MinimapTeamColoursExceptFeatures & unit.TypeBase.Type == UnitType.Feature )
                                        {
                                            sngTexture[y, x, 0] = App.MinimapFeatureColour.Red;
                                            sngTexture[y, x, 1] = App.MinimapFeatureColour.Green;
                                            sngTexture[y, x, 2] = App.MinimapFeatureColour.Blue;
                                        }
                                        else
                                        {
                                            rGBSng = GetUnitGroupColour(unit.UnitGroup);
                                            sngTexture[y, x, 0] = rGBSng.Red;
                                            sngTexture[y, x, 1] = rGBSng.Green;
                                            sngTexture[y, x, 2] = rGBSng.Blue;
                                        }
                                    }
                                    else
                                    {
                                        sngTexture[y, x, 0] = sngTexture[y, x, 0] * 0.6666667F + 0.333333343F;
                                        sngTexture[y, x, 1] = sngTexture[y, x, 1] * 0.6666667F;
                                        sngTexture[y, x, 2] = sngTexture[y, x, 2] * 0.6666667F;
                                    }
                                }
                            }
                        }
                    }
                }
                //reset unit map
                for( var y = 0; y <= texture.Size.Y - 1; y++ )
                {
                    for( var x = 0; x <= texture.Size.X - 1; x++ )
                    {
                        unitMap[y, x] = false;
                    }
                }
                //units that are selected and highlighted
                alpha = settings.MinimapSelectedObjectsColour.Alpha;
                antiAlpha = 1.0F - alpha;
                foreach( var unit in units )
                {
                    flag = false;
                    if( unit.TypeBase.UnitType_frmMainSelectedLink.IsConnected )
                    {
                        flag = true;
                        footprint = unit.TypeBase.GetGetFootprintSelected(unit.Rotation);
                        footprint.X += 2;
                        footprint.Y += 2;
                    }
                    if( flag )
                    {
                        myMap.GetFootprintTileRangeClamped(unit.Pos.Horizontal, footprint, ref low, ref high);
                        for( var y = low.Y; y <= high.Y; y++ )
                        {
                            for( var x = low.X; x <= high.X; x++ )
                            {
                                if( !unitMap[y, x] )
                                {
                                    unitMap[y, x] = true;
                                    sngTexture[y, x, 0] = sngTexture[y, x, 0] * antiAlpha + settings.MinimapSelectedObjectsColour.Red * alpha;
                                    sngTexture[y, x, 1] = sngTexture[y, x, 1] * antiAlpha + settings.MinimapSelectedObjectsColour.Green * alpha;
                                    sngTexture[y, x, 2] = sngTexture[y, x, 2] * antiAlpha + settings.MinimapSelectedObjectsColour.Blue * alpha;
                                }
                            }
                        }
                    }
                }
            }
            for( var y = 0; y <= terrain.TileSize.Y - 1; y++ )
            {
                for( var x = 0; x <= terrain.TileSize.X - 1; x++ )
                {
                    texture.set(x, y, new SRgba(
                        sngTexture[y, x, 0],
                        sngTexture[y, x, 1],
                        sngTexture[y, x, 2],
                        1.0F));
                }
            }
        }

    }
}