using System;
using System.IO;
using NLog;
using SharpFlame.Collections;
using SharpFlame.Core.Extensions;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Objects;
using SharpFlame.Maths;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Interfaces.Mapping.IO;
using SharpFlame.Domain;
using SharpFlame.Mapping.Tiles;

namespace SharpFlame.Mapping.IO.LND
{
    public class LNDSaver: IIOSaver
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly Map map;

        public LNDSaver(Map newMap)
        {
            map = newMap;
        }

        public Result Save(string path, bool overwrite, bool compress = false) // Compress is ignored.
        {
            var returnResult =
                new Result("Writing LND to \"{0}\"".Format2(path), false);

            logger.Info("Writing LND to \"{0}\"".Format2(path));

            if ( System.IO.File.Exists(path) )
            {
                if ( overwrite )
                {
                    System.IO.File.Delete(path);
                }
                else
                {
                    returnResult.ProblemAdd("The selected file already exists.");
                    return returnResult;
                }
            }

            StreamWriter File = null;

            try
            {
                var text = "";
                var endChar = '\n';
                var quote = '\"';;
                var a = 0;
                var x = 0;
                var y = 0;
                byte flip = 0;
                var b = 0;
                var vf = 0;
                var tf = 0;
                var c = 0;
                byte rotation = 0;
                var flipX = default(bool);

                File = new StreamWriter(new FileStream(path, FileMode.CreateNew), App.UTF8Encoding);

                if ( map.Tileset == App.Tileset_Arizona )
                {
                    text = "DataSet WarzoneDataC1.eds" + Convert.ToString(endChar);
                }
                else if ( map.Tileset == App.Tileset_Urban )
                {
                    text = "DataSet WarzoneDataC2.eds" + Convert.ToString(endChar);
                }
                else if ( map.Tileset == App.Tileset_Rockies )
                {
                    text = "DataSet WarzoneDataC3.eds" + Convert.ToString(endChar);
                }
                else
                {
                    text = "DataSet " + Convert.ToString(endChar);
                }
                File.Write(text);
                text = "GrdLand {" + Convert.ToString(endChar);
                File.Write(text);
                text = "    Version 4" + Convert.ToString(endChar);
                File.Write(text);
                text = "    3DPosition 0.000000 3072.000000 0.000000" + Convert.ToString(endChar);
                File.Write(text);
                text = "    3DRotation 80.000000 0.000000 0.000000" + Convert.ToString(endChar);
                File.Write(text);
                text = "    2DPosition 0 0" + Convert.ToString(endChar);
                File.Write(text);
                text = "    CustomSnap 16 16" + Convert.ToString(endChar);
                File.Write(text);
                text = "    SnapMode 0" + Convert.ToString(endChar);
                File.Write(text);
                text = "    Gravity 1" + Convert.ToString(endChar);
                File.Write(text);
                text = "    HeightScale " + map.HeightMultiplier.ToStringInvariant() + Convert.ToString(endChar);
                File.Write(text);
                text = "    MapWidth " + map.Terrain.TileSize.X.ToStringInvariant() + Convert.ToString(endChar);
                File.Write(text);
                text = "    MapHeight " + map.Terrain.TileSize.Y.ToStringInvariant() + Convert.ToString(endChar);
                File.Write(text);
                text = "    TileWidth 128" + Convert.ToString(endChar);
                File.Write(text);
                text = "    TileHeight 128" + Convert.ToString(endChar);
                File.Write(text);
                text = "    SeaLevel 0" + Convert.ToString(endChar);
                File.Write(text);
                text = "    TextureWidth 64" + Convert.ToString(endChar);
                File.Write(text);
                text = "    TextureHeight 64" + Convert.ToString(endChar);
                File.Write(text);
                text = "    NumTextures 1" + Convert.ToString(endChar);
                File.Write(text);
                text = "    Textures {" + Convert.ToString(endChar);
                File.Write(text);
                if ( map.Tileset == App.Tileset_Arizona )
                {
                    text = "        texpages\\tertilesc1.pcx" + Convert.ToString(endChar);
                }
                else if ( map.Tileset == App.Tileset_Urban )
                {
                    text = "        texpages\\tertilesc2.pcx" + Convert.ToString(endChar);
                }
                else if ( map.Tileset == App.Tileset_Rockies )
                {
                    text = "        texpages\\tertilesc3.pcx" + Convert.ToString(endChar);
                }
                else
                {
                    text = "        " + Convert.ToString(endChar);
                }
                File.Write(text);
                text = "    }" + Convert.ToString(endChar);
                File.Write(text);
                text = "    NumTiles " + (map.Terrain.TileSize.X * map.Terrain.TileSize.Y).ToStringInvariant() + Convert.ToString(endChar);
                File.Write(text);
                text = "    Tiles {" + Convert.ToString(endChar);
                File.Write(text);
                for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                    {
                        TileUtil.TileOrientation_To_OldOrientation(map.Terrain.Tiles[x, y].Texture.Orientation, ref rotation, ref flipX);
                        flip = 0;
                        if ( map.Terrain.Tiles[x, y].Tri )
                        {
                            flip += 2;
                        }
                        if ( flipX )
                        {
                            flip += 4;
                        }
                        flip += (byte)(rotation * 16);

                        if ( map.Terrain.Tiles[x, y].Tri )
                        {
                            vf = 1;
                        }
                        else
                        {
                            vf = 0;
                        }
                        if ( flipX )
                        {
                            tf = 1;
                        }
                        else
                        {
                            tf = 0;
                        }

                        text = "        TID " + (map.Terrain.Tiles[x, y].Texture.TextureNum + 1) + " VF " + vf.ToStringInvariant() + " TF " +
                            tf.ToStringInvariant() + " F " + ((int)flip).ToStringInvariant() + " VH " +
                                Convert.ToByte(map.Terrain.Vertices[x, y].Height).ToStringInvariant() + " " +
                                map.Terrain.Vertices[x + 1, y].Height.ToStringInvariant() + " " + Convert.ToString(map.Terrain.Vertices[x + 1, y + 1].Height) +
                                " " + Convert.ToByte(map.Terrain.Vertices[x, y + 1].Height).ToStringInvariant() + Convert.ToString(endChar);
                        File.Write(text);
                    }
                }
                text = "    }" + Convert.ToString(endChar);
                File.Write(text);
                text = "}" + Convert.ToString(endChar);
                File.Write(text);
                text = "ObjectList {" + Convert.ToString(endChar);
                File.Write(text);
                text = "    Version 3" + Convert.ToString(endChar);
                File.Write(text);
                if ( map.Tileset == App.Tileset_Arizona )
                {
                    text = "    FeatureSet WarzoneDataC1.eds" + Convert.ToString(endChar);
                }
                else if ( map.Tileset == App.Tileset_Urban )
                {
                    text = "    FeatureSet WarzoneDataC2.eds" + Convert.ToString(endChar);
                }
                else if ( map.Tileset == App.Tileset_Rockies )
                {
                    text = "    FeatureSet WarzoneDataC3.eds" + Convert.ToString(endChar);
                }
                else
                {
                    text = "    FeatureSet " + Convert.ToString(endChar);
                }
                File.Write(text);
                text = "    NumObjects " + map.Units.Count.ToStringInvariant() + Convert.ToString(endChar);
                File.Write(text);
                text = "    Objects {" + Convert.ToString(endChar);
                File.Write(text);
                var XYZ_int = new XYZInt(0, 0, 0);
                string Code = null;
                var CustomDroidCount = 0;
                foreach ( var unit in map.Units )
                {
                    switch ( unit.TypeBase.Type )
                    {
                        case UnitType.Feature:
                        b = 0;
                        break;
                        case UnitType.PlayerStructure:
                        b = 1;
                        break;
                        case UnitType.PlayerDroid:
                        if ( ((DroidDesign)unit.TypeBase).IsTemplate )
                        {
                            b = 2;
                        }
                        else
                        {
                            b = -1;
                        }
                        break;
                        default:
                        b = -1;
                        returnResult.WarningAdd("Unit type classification not accounted for.");
                        break;
                    }
                    XYZ_int = lndPos_From_MapPos(map.Units[a].Pos.Horizontal);
                    if ( b >= 0 )
                    {
                        if ( unit.TypeBase.GetCode(ref Code) )
                        {
                            text = "        " + unit.ID.ToStringInvariant() + " " + Convert.ToString(b) + " " + Convert.ToString(quote) +
                                Code + Convert.ToString(quote) + " " + unit.UnitGroup.GetLNDPlayerText() + " " + Convert.ToString(quote) + "NONAME" +
                                    Convert.ToString(quote) + " " + XYZ_int.X.ToStringInvariant() + ".00 " + XYZ_int.Y.ToStringInvariant() +
                                    ".00 " + XYZ_int.Z.ToStringInvariant() + ".00 0.00 " + unit.Rotation.ToStringInvariant() + ".00 0.00" +
                                    Convert.ToString(endChar);
                            File.Write(text);
                        }
                        else
                        {
                            returnResult.WarningAdd("Error. Code not found.");
                        }
                    }
                    else
                    {
                        CustomDroidCount++;
                    }
                }
                text = "    }" + Convert.ToString(endChar);
                File.Write(text);
                text = "}" + Convert.ToString(endChar);
                File.Write(text);
                text = "ScrollLimits {" + Convert.ToString(endChar);
                File.Write(text);
                text = "    Version 1" + Convert.ToString(endChar);
                File.Write(text);
                text = "    NumLimits 1" + Convert.ToString(endChar);
                File.Write(text);
                text = "    Limits {" + Convert.ToString(endChar);
                File.Write(text);
                text = "        " + Convert.ToString(quote) + "Entire Map" + Convert.ToString(quote) + " 0 0 0 " +
                    map.Terrain.TileSize.X.ToStringInvariant() + " " + map.Terrain.TileSize.Y.ToStringInvariant() + Convert.ToString(endChar);
                File.Write(text);
                text = "    }" + Convert.ToString(endChar);
                File.Write(text);
                text = "}" + Convert.ToString(endChar);
                File.Write(text);
                text = "Gateways {" + Convert.ToString(endChar);
                File.Write(text);
                text = "    Version 1" + Convert.ToString(endChar);
                File.Write(text);
                text = "    NumGateways " + map.Gateways.Count.ToStringInvariant() + Convert.ToString(endChar);
                File.Write(text);
                text = "    Gates {" + Convert.ToString(endChar);
                File.Write(text);
                foreach ( var gateway in map.Gateways )
                {
                    text = "        " + gateway.PosA.X.ToStringInvariant() + " " + gateway.PosA.Y.ToStringInvariant() + " " +
                        gateway.PosB.X.ToStringInvariant() + " " + gateway.PosB.Y.ToStringInvariant() + Convert.ToString(endChar);
                    File.Write(text);
                }
                text = "    }" + Convert.ToString(endChar);
                File.Write(text);
                text = "}" + Convert.ToString(endChar);
                File.Write(text);
                text = "TileTypes {" + Convert.ToString(endChar);
                File.Write(text);
                text = "    NumTiles " + Convert.ToString(map.Tileset.Tiles.Count) + Convert.ToString(endChar);
                File.Write(text);
                text = "    Tiles {" + Convert.ToString(endChar);
                File.Write(text);
                for ( a = 0; a <= ((int)(Math.Ceiling(Convert.ToDecimal((map.Tileset.Tiles.Count + 1) / 16.0D)))) - 1; a++ )
                    //+1 because the first number is not a tile type
                {
                    text = "        ";
                    c = a * 16 - 1; //-1 because the first number is not a tile type
                    for ( b = 0; b <= Math.Min(16, map.Tileset.Tiles.Count - c) - 1; b++ )
                    {
                        if ( c + b < 0 )
                        {
                            text = text + "2 ";
                        }
                        else
                        {
                            text = text + map.TileTypeNum[c + b].ToStringInvariant() + " ";
                        }
                    }
                    text = text + Convert.ToString(endChar);
                    File.Write(text);
                }
                text = "    }" + Convert.ToString(endChar);
                File.Write(text);
                text = "}" + Convert.ToString(endChar);
                File.Write(text);
                text = "TileFlags {" + Convert.ToString(endChar);
                File.Write(text);
                text = "    NumTiles 90" + Convert.ToString(endChar);
                File.Write(text);
                text = "    Flags {" + Convert.ToString(endChar);
                File.Write(text);
                text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(endChar);
                File.Write(text);
                text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(endChar);
                File.Write(text);
                text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(endChar);
                File.Write(text);
                text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(endChar);
                File.Write(text);
                text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(endChar);
                File.Write(text);
                text = "        0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(endChar);
                File.Write(text);
                text = "    }" + Convert.ToString(endChar);
                File.Write(text);
                text = "}" + Convert.ToString(endChar);
                File.Write(text);
                text = "Brushes {" + Convert.ToString(endChar);
                File.Write(text);
                text = "    Version 2" + Convert.ToString(endChar);
                File.Write(text);
                text = "    NumEdgeBrushes 0" + Convert.ToString(endChar);
                File.Write(text);
                text = "    NumUserBrushes 0" + Convert.ToString(endChar);
                File.Write(text);
                text = "    EdgeBrushes {" + Convert.ToString(endChar);
                File.Write(text);
                text = "    }" + Convert.ToString(endChar);
                File.Write(text);
                text = "}" + Convert.ToString(endChar);
                File.Write(text);
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
            }
            if ( File != null )
            {
                File.Close();
            }

            return returnResult;
        }

        private XYZInt lndPos_From_MapPos(XYInt Horizontal)
        {
            return new XYZInt(Horizontal.X - (int)(map.Terrain.TileSize.X * Constants.TerrainGridSpacing / 2.0D),
                              ((int)(map.Terrain.TileSize.Y * Constants.TerrainGridSpacing / 2.0D)) - Horizontal.Y,
                              (int)(map.GetTerrainHeight(Horizontal)));
        }
    }
}

