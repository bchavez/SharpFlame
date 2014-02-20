using System;
using System.IO;
using NLog;
using SharpFlame.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Maths;
using SharpFlame.Mapping.Tiles;


namespace SharpFlame.Mapping.Format.LND
{
    public class LND
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly clsMap map;

        public LND(clsMap newMap)
        {
            map = newMap;
        }

        public clsResult Load(string path)
        {
            var returnResult =
                new clsResult("Loading LND from \"{0}\"".Format2(path), false);
            logger.Info("Loading LND from \"{0}\"".Format2(path));
            try
            {
                var strTemp = "";
                var strTemp2 = "";
                var X = 0;
                var Y = 0;
                var A = 0;
                var B = 0;
                var Tile_Num = 0;
                // SimpleList<string> LineData = default(SimpleList<string>);
                var Line_Num = 0;
                LNDTile[] LNDTile = null;
                var LNDObjects = new SimpleList<LNDObject>();
                var UnitAdd = new clsUnitAdd();

                UnitAdd.Map = map;

                var Reader = default(BinaryReader);
                try
                {
                    Reader = new BinaryReader(new FileStream(path, FileMode.Open), App.UTF8Encoding);
                }
                catch ( Exception ex )
                {
                    returnResult.ProblemAdd(ex.Message);
                    return returnResult;
                }
                var LineData = IOUtil.BytesToLinesRemoveComments(Reader);
                Reader.Close();

                Array.Resize(ref LNDTile, LineData.Count);

                var strTemp3 = "";
                var GotTiles = default(bool);
                var GotObjects = default(bool);
                var GotGates = default(bool);
                var GotTileTypes = default(bool);
                var LNDTileType = new byte[0];
                var ObjectText = new string[11];
                var GateText = new string[4];
                var TileTypeText = new string[256];
                var LNDTileTypeCount = 0;
                var LNDGates = new SimpleList<clsGateway>();
                var Gateway = default(clsGateway);
                var C = 0;
                var D = 0;
                var GotText = default(bool);
                var FlipX = default(bool);
                var FlipZ = default(bool);
                byte Rotation = 0;
                var NewTileSize = new XYInt();
                double dblTemp = 0;

                Line_Num = 0;
                while ( Line_Num < LineData.Count )
                {
                    strTemp = LineData[Line_Num];

                    A = strTemp.IndexOf("TileWidth ") + 1;
                    if ( A == 0 )
                    {
                    }

                    A = strTemp.IndexOf("TileHeight ") + 1;
                    if ( A == 0 )
                    {
                    }

                    A = strTemp.IndexOf("MapWidth ") + 1;
                    if ( A == 0 )
                    {
                    }
                    else
                    {
                        IOUtil.InvariantParse(strTemp.Substring(strTemp.Length - (strTemp.Length - (A + 8)), strTemp.Length - (A + 8)), ref NewTileSize.X);
                        goto LineDone;
                    }

                    A = strTemp.IndexOf("MapHeight ") + 1;
                    if ( A == 0 )
                    {
                    }
                    else
                    {
                        IOUtil.InvariantParse(strTemp.Substring(strTemp.Length - (strTemp.Length - (A + 9)), strTemp.Length - (A + 9)), ref NewTileSize.Y);
                        goto LineDone;
                    }

                    A = strTemp.IndexOf("Textures {") + 1;
                    if ( A == 0 )
                    {
                    }
                    else
                    {
                        Line_Num++;
                        strTemp = LineData[Line_Num];

                        strTemp2 = strTemp.ToLower();
                        if ( strTemp2.IndexOf("tertilesc1") + 1 > 0 )
                        {
                            map.Tileset = App.Tileset_Arizona;
                        }
                        if ( strTemp2.IndexOf("tertilesc2") + 1 > 0 )
                        {
                            map.Tileset = App.Tileset_Urban;
                        }
                        if ( strTemp2.IndexOf("tertilesc3") + 1 > 0 )
                        {
                            map.Tileset = App.Tileset_Rockies;
                        }

                        goto LineDone;
                    }

                    A = strTemp.IndexOf("Tiles {") + 1;
                    if ( A == 0 || GotTiles )
                    {
                    }
                    else
                    {
                        Line_Num++;
                        while ( Line_Num < LineData.Count )
                        {
                            strTemp = LineData[Line_Num];

                            A = strTemp.IndexOf("}") + 1;
                            if ( A == 0 )
                            {
                                A = strTemp.IndexOf("TID ") + 1;
                                if ( A == 0 )
                                {
                                    returnResult.ProblemAdd("Tile ID missing");
                                    return returnResult;
                                }
                                strTemp2 = strTemp.Substring(strTemp.Length - (strTemp.Length - A - 3), strTemp.Length - A - 3);
                                A = strTemp2.IndexOf(" ") + 1;
                                if ( A > 0 )
                                {
                                    strTemp2 = strTemp2.Substring(0, A - 1);
                                }
                                var temp_Result = LNDTile[Tile_Num].TID;
                                IOUtil.InvariantParse(strTemp2, ref temp_Result);

                                A = strTemp.IndexOf("VF ") + 1;
                                if ( A == 0 )
                                {
                                    returnResult.ProblemAdd("Tile VF missing");
                                    return returnResult;
                                }
                                strTemp2 = strTemp.Substring(strTemp.Length - (strTemp.Length - A - 2), strTemp.Length - A - 2);
                                A = strTemp2.IndexOf(" ") + 1;
                                if ( A > 0 )
                                {
                                    strTemp2 = strTemp2.Substring(0, A - 1);
                                }
                                var temp_Result2 = LNDTile[Tile_Num].VF;
                                IOUtil.InvariantParse(strTemp2, ref temp_Result2);

                                A = strTemp.IndexOf("TF ") + 1;
                                if ( A == 0 )
                                {
                                    returnResult.ProblemAdd("Tile TF missing");
                                    return returnResult;
                                }
                                strTemp2 = strTemp.Substring(strTemp.Length - (strTemp.Length - A - 2), strTemp.Length - A - 2);
                                A = strTemp2.IndexOf(" ") + 1;
                                if ( A > 0 )
                                {
                                    strTemp2 = strTemp2.Substring(0, A - 1);
                                }
                                var temp_Result3 = LNDTile[Tile_Num].TF;
                                IOUtil.InvariantParse(strTemp2, ref temp_Result3);

                                A = strTemp.IndexOf(" F ") + 1;
                                if ( A == 0 )
                                {
                                    returnResult.ProblemAdd("Tile flip missing");
                                    return returnResult;
                                }
                                strTemp2 = strTemp.Substring(strTemp.Length - A - 2, strTemp.Length - A - 2);
                                A = strTemp2.IndexOf(" ");
                                if ( A > 0 )
                                {
                                    strTemp2 = strTemp2.Substring(0, A);
                                }
                                var temp_Result4 = LNDTile[Tile_Num].F;
                                IOUtil.InvariantParse(strTemp2, ref temp_Result4);

                                A = strTemp.IndexOf(" VH ") + 1;
                                if ( A == 0 )
                                {
                                    returnResult.ProblemAdd("Tile height is missing");
                                    return returnResult;
                                }
                                strTemp3 = strTemp.Substring(strTemp.Length - (strTemp.Length - A - 3), strTemp.Length - A - 3);
                                for ( A = 0; A <= 2; A++ )
                                {
                                    B = strTemp3.IndexOf(" ") + 1;
                                    if ( B == 0 )
                                    {
                                        returnResult.ProblemAdd("A tile height value is missing");
                                        return returnResult;
                                    }
                                    strTemp2 = strTemp3.Substring(0, B - 1);
                                    strTemp3 = strTemp3.Substring(strTemp3.Length - (strTemp3.Length - B), strTemp3.Length - B);

                                    if ( A == 0 )
                                    {
                                        var temp_Result5 = LNDTile[Tile_Num].Vertex0Height;
                                        IOUtil.InvariantParse(strTemp2, ref temp_Result5);
                                    }
                                    else if ( A == 1 )
                                    {
                                        var temp_Result6 = LNDTile[Tile_Num].Vertex1Height;
                                        IOUtil.InvariantParse(strTemp2, ref temp_Result6);
                                    }
                                    else if ( A == 2 )
                                    {
                                        var temp_Result7 = LNDTile[Tile_Num].Vertex2Height;
                                        IOUtil.InvariantParse(strTemp2, ref temp_Result7);
                                    }
                                }
                                var temp_Result8 = LNDTile[Tile_Num].Vertex3Height;
                                IOUtil.InvariantParse(strTemp3, ref temp_Result8);

                                Tile_Num++;
                            }
                            else
                            {
                                GotTiles = true;
                                goto LineDone;
                            }

                            Line_Num++;
                        }

                        GotTiles = true;
                        goto LineDone;
                    }

                    A = strTemp.IndexOf("Objects {") + 1;
                    if ( A == 0 || GotObjects )
                    {
                    }
                    else
                    {
                        Line_Num++;
                        while ( Line_Num < LineData.Count )
                        {
                            strTemp = LineData[Line_Num];

                            A = strTemp.IndexOf("}") + 1;
                            if ( A == 0 )
                            {
                                C = 0;
                                ObjectText[0] = "";
                                GotText = false;
                                for ( B = 0; B <= strTemp.Length - 1; B++ )
                                {
                                    if ( strTemp[B] != ' ' && strTemp[B] != '\t' )
                                    {
                                        GotText = true;
                                        ObjectText[C] += strTemp[B].ToString();
                                    }
                                    else
                                    {
                                        if ( GotText )
                                        {
                                            C++;
                                            if ( C == 11 )
                                            {
                                                returnResult.ProblemAdd("Too many fields for an object, or a space at the end.");
                                                return returnResult;
                                            }
                                            ObjectText[C] = "";
                                            GotText = false;
                                        }
                                    }
                                }

                                var NewObject = new LNDObject();
                                IOUtil.InvariantParse(ObjectText[0], ref NewObject.ID);
                                IOUtil.InvariantParse(ObjectText[1], ref NewObject.TypeNum);
                                NewObject.Code = ObjectText[2].Substring(1, ObjectText[2].Length - 2); //remove quotes
                                IOUtil.InvariantParse(ObjectText[3], ref NewObject.PlayerNum);
                                NewObject.Name = ObjectText[4].Substring(1, ObjectText[4].Length - 2); //remove quotes
                                IOUtil.InvariantParse(ObjectText[5], ref NewObject.Pos.X);
                                IOUtil.InvariantParse(ObjectText[6], ref NewObject.Pos.Y);
                                IOUtil.InvariantParse(ObjectText[7], ref NewObject.Pos.Z);
                                if ( IOUtil.InvariantParse(ObjectText[8], ref dblTemp) )
                                {
                                    NewObject.Rotation.X = (int)(MathUtil.Clamp_dbl(dblTemp, 0.0D, 359.0D));
                                }
                                if ( IOUtil.InvariantParse(ObjectText[9], ref dblTemp) )
                                {
                                    NewObject.Rotation.Y = (int)(MathUtil.Clamp_dbl(dblTemp, 0.0D, 359.0D));
                                }
                                if ( IOUtil.InvariantParse(ObjectText[10], ref dblTemp) )
                                {
                                    NewObject.Rotation.Z = (int)(MathUtil.Clamp_dbl(dblTemp, 0.0D, 359.0D));
                                }
                                LNDObjects.Add(NewObject);
                            }
                            else
                            {
                                GotObjects = true;
                                goto LineDone;
                            }

                            Line_Num++;
                        }

                        GotObjects = true;
                        goto LineDone;
                    }

                    A = strTemp.IndexOf("Gates {") + 1;
                    if ( A == 0 || GotGates )
                    {
                    }
                    else
                    {
                        Line_Num++;
                        while ( Line_Num < LineData.Count )
                        {
                            strTemp = LineData[Line_Num];

                            A = strTemp.IndexOf("}") + 1;
                            if ( A == 0 )
                            {
                                C = 0;
                                GateText[0] = "";
                                GotText = false;
                                for ( B = 0; B <= strTemp.Length - 1; B++ )
                                {
                                    if ( strTemp[B] != ' ' && strTemp[B] != '\t' )
                                    {
                                        GotText = true;
                                        GateText[C] += strTemp[B].ToString();
                                    }
                                    else
                                    {
                                        if ( GotText )
                                        {
                                            C++;
                                            if ( C == 4 )
                                            {
                                                returnResult.ProblemAdd("Too many fields for a gateway, or a space at the end.");
                                                return returnResult;
                                            }
                                            GateText[C] = "";
                                            GotText = false;
                                        }
                                    }
                                }

                                Gateway = new clsGateway();
                                IOUtil.InvariantParse(GateText[0], ref Gateway.PosA.X);
                                Gateway.PosA.X = Math.Max(Gateway.PosA.X, 0);
                                IOUtil.InvariantParse(GateText[1], ref Gateway.PosA.Y);
                                Gateway.PosA.Y = Math.Max(Gateway.PosA.Y, 0);
                                IOUtil.InvariantParse(GateText[2], ref Gateway.PosB.X);
                                Gateway.PosB.X = Math.Max(Gateway.PosB.X, 0);
                                IOUtil.InvariantParse(GateText[3], ref Gateway.PosB.Y);
                                Gateway.PosB.Y = Math.Max(Gateway.PosB.Y, 0);
                                LNDGates.Add(Gateway);
                            }
                            else
                            {
                                GotGates = true;
                                goto LineDone;
                            }

                            Line_Num++;
                        }

                        GotGates = true;
                        goto LineDone;
                    }

                    A = strTemp.IndexOf("Tiles {") + 1;
                    if ( A == 0 || GotTileTypes || !GotTiles )
                    {
                    }
                    else
                    {
                        Line_Num++;
                        while ( Line_Num < LineData.Count )
                        {
                            strTemp = LineData[Line_Num];

                            A = strTemp.IndexOf("}") + 1;
                            if ( A == 0 )
                            {
                                C = 0;
                                TileTypeText[0] = "";
                                GotText = false;
                                for ( B = 0; B <= strTemp.Length - 1; B++ )
                                {
                                    if ( strTemp[B] != ' ' && strTemp[B] != '\t' )
                                    {
                                        GotText = true;
                                        TileTypeText[C] += strTemp[B].ToString();
                                    }
                                    else
                                    {
                                        if ( GotText )
                                        {
                                            C++;
                                            if ( C == 256 )
                                            {
                                                returnResult.ProblemAdd("Too many fields for tile types.");
                                                return returnResult;
                                            }
                                            TileTypeText[C] = "";
                                            GotText = false;
                                        }
                                    }
                                }

                                if ( TileTypeText[C] == "" || TileTypeText[C] == " " )
                                {
                                    C--;
                                }

                                for ( D = 0; D <= C; D++ )
                                {
                                    Array.Resize(ref LNDTileType, LNDTileTypeCount + 1);
                                    LNDTileType[LNDTileTypeCount] = Math.Min(byte.Parse(TileTypeText[D]), (byte)11);
                                    LNDTileTypeCount++;
                                }
                            }
                            else
                            {
                                GotTileTypes = true;
                                goto LineDone;
                            }

                            Line_Num++;
                        }

                        GotTileTypes = true;
                    }

                    LineDone:
                        Line_Num++;
                }

                Array.Resize(ref LNDTile, Tile_Num);

                map.SetPainterToDefaults();

                if ( NewTileSize.X < 1 | NewTileSize.Y < 1 )
                {
                    returnResult.ProblemAdd("The LND\'s terrain dimensions are missing or invalid.");
                    return returnResult;
                }

                map.TerrainBlank(NewTileSize);
                map.TileType_Reset();

                for ( Y = 0; Y <= map.Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= map.Terrain.TileSize.X - 1; X++ )
                    {
                        Tile_Num = Y * map.Terrain.TileSize.X + X;
                        //lnd uses different order! (3 = 2, 2 = 3), this program goes left to right, lnd goes clockwise around each tile
                        map.Terrain.Vertices[X, Y].Height = (byte)(LNDTile[Tile_Num].Vertex0Height);
                    }
                }

                for ( Y = 0; Y <= map.Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= map.Terrain.TileSize.X - 1; X++ )
                    {
                        Tile_Num = Y * map.Terrain.TileSize.X + X;

                        map.Terrain.Tiles[X, Y].Texture.TextureNum = LNDTile[Tile_Num].TID - 1;

                        //ignore higher values
                        A = Convert.ToInt32((LNDTile[Tile_Num].F / 64.0D));
                        LNDTile[Tile_Num].F = (short)(LNDTile[Tile_Num].F - A * 64);

                        A = (int)((LNDTile[Tile_Num].F / 16.0D));
                        LNDTile[Tile_Num].F = (short)(LNDTile[Tile_Num].F - A * 16);
                        if ( A < 0 | A > 3 )
                        {
                            returnResult.ProblemAdd("Invalid flip value.");
                            return returnResult;
                        }
                        Rotation = (byte)A;

                        A = (int)((LNDTile[Tile_Num].F / 8.0D));
                        LNDTile[Tile_Num].F -= (short)(A * 8);
                        FlipZ = A == 1;

                        A = (int)((LNDTile[Tile_Num].F / 4.0D));
                        LNDTile[Tile_Num].F -= (short)(A * 4);
                        FlipX = A == 1;

                        A = Convert.ToInt32((LNDTile[Tile_Num].F / 2.0D));
                        LNDTile[Tile_Num].F -= (short)(A * 2);
                        map.Terrain.Tiles[X, Y].Tri = A == 1;

                        //vf, tf, ignore

                        TileUtil.OldOrientation_To_TileOrientation(Rotation, FlipX, FlipZ, ref map.Terrain.Tiles[X, Y].Texture.Orientation);
                    }
                }

                var newUnit = default(clsUnit);
                var xyzInt = new XYZInt(0, 0, 0);
                var newTypeBase = default(UnitTypeBase);
                UInt32 availableID = 0;

                availableID = 1U;
                foreach ( var currentObject in LNDObjects )
                {
                    if ( currentObject.ID >= availableID )
                    {
                        availableID = currentObject.ID + 1U;
                    }
                }
                foreach ( var currentObject in LNDObjects )
                {
                    switch ( currentObject.TypeNum )
                    {
                        case 0:
                        newTypeBase = App.ObjectData.FindOrCreateUnitType(currentObject.Code, UnitType.Feature, -1);
                        break;
                        case 1:
                        newTypeBase = App.ObjectData.FindOrCreateUnitType(currentObject.Code, UnitType.PlayerStructure, -1);
                        break;
                        case 2:
                        newTypeBase = App.ObjectData.FindOrCreateUnitType(currentObject.Code, UnitType.PlayerDroid, -1);
                        break;
                        default:
                        newTypeBase = null;
                        break;
                    }
                    if ( newTypeBase != null )
                    {
                        newUnit = new clsUnit();
                        newUnit.TypeBase = newTypeBase;
                        if ( currentObject.PlayerNum < 0 | currentObject.PlayerNum >= Constants.PlayerCountMax )
                        {
                            newUnit.UnitGroup = map.ScavengerUnitGroup;
                        }
                        else
                        {
                            newUnit.UnitGroup = map.UnitGroups[currentObject.PlayerNum];
                        }
                        xyzInt.X = (int)currentObject.Pos.X;
                        xyzInt.Y = (int)currentObject.Pos.Y;
                        xyzInt.Z = (int)currentObject.Pos.Z;
                        newUnit.Pos = mapPos_From_LNDPos(xyzInt);
                        newUnit.Rotation = currentObject.Rotation.Y;
                        if ( currentObject.ID == 0U )
                        {
                            currentObject.ID = availableID;
                            App.ZeroIDWarning(newUnit, currentObject.ID, returnResult);
                        }
                        UnitAdd.NewUnit = newUnit;
                        UnitAdd.ID = currentObject.ID;
                        UnitAdd.Perform();
                        App.ErrorIDChange(currentObject.ID, newUnit, "Load_LND");
                        if ( availableID == currentObject.ID )
                        {
                            availableID = newUnit.ID + 1U;
                        }
                    }
                }

                foreach ( var tempLoopVar_Gateway in LNDGates )
                {
                    Gateway = tempLoopVar_Gateway;
                    map.GatewayCreate(Gateway.PosA, Gateway.PosB);
                }

                if ( map.Tileset != null )
                {
                    for ( A = 0; A <= Math.Min(LNDTileTypeCount - 1, map.Tileset.TileCount) - 1; A++ )
                    {
                        map.Tile_TypeNum[A] = LNDTileType[A + 1]; //lnd value 0 is ignored
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            return returnResult;
        }

        public clsResult Save(string path, bool overwrite, bool compress = false) // Compress is ignored.
        {
            var returnResult =
                new clsResult("Writing LND to \"{0}\"".Format2(path), false);

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
                text = "    NumTiles " + Convert.ToString(map.Tileset.TileCount) + Convert.ToString(endChar);
                File.Write(text);
                text = "    Tiles {" + Convert.ToString(endChar);
                File.Write(text);
                for ( a = 0; a <= ((int)(Math.Ceiling(Convert.ToDecimal((map.Tileset.TileCount + 1) / 16.0D)))) - 1; a++ )
                    //+1 because the first number is not a tile type
                {
                    text = "        ";
                    c = a * 16 - 1; //-1 because the first number is not a tile type
                    for ( b = 0; b <= Math.Min(16, map.Tileset.TileCount - c) - 1; b++ )
                    {
                        if ( c + b < 0 )
                        {
                            text = text + "2 ";
                        }
                        else
                        {
                            text = text + map.Tile_TypeNum[c + b].ToStringInvariant() + " ";
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

        private WorldPos mapPos_From_LNDPos(XYZInt Pos)
        {
            var Result = new WorldPos();

            Result.Horizontal.X = Pos.X + (int)(map.Terrain.TileSize.X * Constants.TerrainGridSpacing / 2.0D);
            Result.Horizontal.Y = ((int)(map.Terrain.TileSize.Y * Constants.TerrainGridSpacing / 2.0D)) - Pos.Z;
            Result.Altitude = (int)(map.GetTerrainHeight(Result.Horizontal));

            return Result;
        }       
    }
}

