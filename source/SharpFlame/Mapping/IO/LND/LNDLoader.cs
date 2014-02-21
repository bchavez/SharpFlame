using System;
using System.IO;
using NLog;
using SharpFlame.Collections;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Core.Interfaces.Mapping.IO;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.IO.LND
{
    public class LNDLoader : IIOLoader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly clsMap map;

        public LNDLoader(clsMap newMap)
        {
            map = newMap;
        }

        public Result Load(string path)
        {
            var returnResult =
                new Result("Loading LND from \"{0}\"".Format2(path), false);
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
                                    NewObject.Rotation.X = (int)(MathUtil.ClampDbl(dblTemp, 0.0D, 359.0D));
                                }
                                if ( IOUtil.InvariantParse(ObjectText[9], ref dblTemp) )
                                {
                                    NewObject.Rotation.Y = (int)(MathUtil.ClampDbl(dblTemp, 0.0D, 359.0D));
                                }
                                if ( IOUtil.InvariantParse(ObjectText[10], ref dblTemp) )
                                {
                                    NewObject.Rotation.Z = (int)(MathUtil.ClampDbl(dblTemp, 0.0D, 359.0D));
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

