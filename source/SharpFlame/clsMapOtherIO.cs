using System;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.VisualBasic;
using SharpFlame.Bitmaps;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO;
using SharpFlame.MathExtra;

namespace SharpFlame
{
    public partial class clsMap
    {
        public struct sFMEUnit
        {
            public string Code;
            public UInt32 ID;
            public int SavePriority;
            public byte LNDType;
            public UInt32 X;
            public UInt32 Y;
            public UInt32 Z;
            public UInt16 Rotation;
            public string Name;
            public byte Player;
        }

        public clsResult Load_FME(string Path)
        {
            clsResult ReturnResult =
                new clsResult("Loading FME from " + Convert.ToString(ControlChars.Quote) + Path + Convert.ToString(ControlChars.Quote));

            BinaryReader File = default(BinaryReader);

            try
            {
                File = new BinaryReader(new FileStream(Path, FileMode.Open));
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }
            ReturnResult.Take(Read_FME(File));
            File.Close();

            return ReturnResult;
        }

        private clsResult Read_FME(BinaryReader File)
        {
            clsResult ReturnResult = new clsResult("Reading FME");

            UInt32 Version = 0;

            clsInterfaceOptions ResultInfo = new clsInterfaceOptions();

            clsUnitAdd UnitAdd = new clsUnitAdd();
            UnitAdd.Map = this;

            try
            {
                Version = File.ReadUInt32();

                if ( Version <= 4U )
                {
                    ReturnResult.ProblemAdd("Version " + Convert.ToString(Version) + " is not supported.");
                    return ReturnResult;
                }
                else if ( Version == 5U || Version == 6U || Version == 7U )
                {
                    byte byteTemp = 0;

                    //tileset
                    byteTemp = File.ReadByte();
                    if ( byteTemp == 0 )
                    {
                        Tileset = null;
                    }
                    else if ( byteTemp == 1 )
                    {
                        Tileset = modProgram.Tileset_Arizona;
                    }
                    else if ( byteTemp == 2 )
                    {
                        Tileset = modProgram.Tileset_Urban;
                    }
                    else if ( byteTemp == 3 )
                    {
                        Tileset = modProgram.Tileset_Rockies;
                    }
                    else
                    {
                        ReturnResult.WarningAdd("Tileset value out of range.");
                        Tileset = null;
                    }

                    SetPainterToDefaults(); //depends on tileset. must be called before loading the terrains.

                    UInt16 MapWidth = 0;
                    UInt16 MapHeight = 0;

                    MapWidth = File.ReadUInt16();
                    MapHeight = File.ReadUInt16();

                    if ( MapWidth < 1U || MapHeight < 1U || MapWidth > modProgram.MapMaxSize || MapHeight > modProgram.MapMaxSize )
                    {
                        ReturnResult.ProblemAdd("Map size is invalid.");
                        return ReturnResult;
                    }

                    TerrainBlank(new sXY_int(MapWidth, MapHeight));
                    TileType_Reset();

                    int X = 0;
                    int Y = 0;
                    int A = 0;
                    int B = 0;
                    int intTemp = 0;
                    int WarningCount = 0;

                    WarningCount = 0;
                    for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X; X++ )
                        {
                            Terrain.Vertices[X, Y].Height = File.ReadByte();
                            byteTemp = File.ReadByte();
                            intTemp = (byteTemp) - 1;
                            if ( intTemp < 0 )
                            {
                                Terrain.Vertices[X, Y].Terrain = null;
                            }
                            else if ( intTemp >= Painter.TerrainCount )
                            {
                                WarningCount++;
                                Terrain.Vertices[X, Y].Terrain = null;
                            }
                            else
                            {
                                Terrain.Vertices[X, Y].Terrain = Painter.Terrains[intTemp];
                            }
                        }
                    }
                    if ( WarningCount > 0 )
                    {
                        ReturnResult.WarningAdd(WarningCount + " painted ground vertices were out of range.");
                    }
                    WarningCount = 0;
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            byteTemp = File.ReadByte();
                            Terrain.Tiles[X, Y].Texture.TextureNum = (byteTemp) - 1;

                            byteTemp = File.ReadByte();

                            intTemp = 128;
                            A = (int)(Conversion.Int(byteTemp / intTemp));
                            byteTemp -= (byte)(A * intTemp);
                            Terrain.Tiles[X, Y].Terrain_IsCliff = A == 1;

                            intTemp = 64;
                            A = (int)(Conversion.Int(byteTemp / intTemp));
                            byteTemp -= (byte)(A * intTemp);
                            Terrain.Tiles[X, Y].Texture.Orientation.SwitchedAxes = A == 1;

                            intTemp = 32;
                            A = (int)(Conversion.Int(byteTemp / intTemp));
                            byteTemp -= (byte)(A * intTemp);
                            Terrain.Tiles[X, Y].Texture.Orientation.ResultXFlip = A == 1;

                            intTemp = 16;
                            A = (int)(Conversion.Int(byteTemp / intTemp));
                            byteTemp -= (byte)(A * intTemp);
                            Terrain.Tiles[X, Y].Texture.Orientation.ResultYFlip = A == 1;

                            intTemp = 4;
                            A = (int)(Conversion.Int(byteTemp / intTemp));
                            byteTemp -= (byte)(A * intTemp);
                            Terrain.Tiles[X, Y].Tri = A == 1;

                            intTemp = 2;
                            A = (int)(Conversion.Int(byteTemp / intTemp));
                            byteTemp -= (byte)(A * intTemp);
                            if ( Terrain.Tiles[X, Y].Tri )
                            {
                                Terrain.Tiles[X, Y].TriTopLeftIsCliff = A == 1;
                            }
                            else
                            {
                                Terrain.Tiles[X, Y].TriBottomLeftIsCliff = A == 1;
                            }

                            intTemp = 1;
                            A = (int)(Conversion.Int(byteTemp / intTemp));
                            byteTemp -= (byte)(A * intTemp);
                            if ( Terrain.Tiles[X, Y].Tri )
                            {
                                Terrain.Tiles[X, Y].TriBottomRightIsCliff = A == 1;
                            }
                            else
                            {
                                Terrain.Tiles[X, Y].TriTopRightIsCliff = A == 1;
                            }

                            //attributes2
                            byteTemp = File.ReadByte();

                            if ( byteTemp == ((byte)0) )
                            {
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_None;
                            }
                            else if ( byteTemp == ((byte)1) )
                            {
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_Top;
                            }
                            else if ( byteTemp == ((byte)2) )
                            {
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_Left;
                            }
                            else if ( byteTemp == ((byte)3) )
                            {
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_Right;
                            }
                            else if ( byteTemp == ((byte)4) )
                            {
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_Bottom;
                            }
                            else
                            {
                                WarningCount++;
                            }
                        }
                    }
                    if ( WarningCount > 0 )
                    {
                        ReturnResult.WarningAdd(WarningCount + " tile cliff down-sides were out of range.");
                    }
                    WarningCount = 0;
                    for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            byteTemp = File.ReadByte();
                            intTemp = (byteTemp) - 1;
                            if ( intTemp < 0 )
                            {
                                Terrain.SideH[X, Y].Road = null;
                            }
                            else if ( intTemp >= Painter.RoadCount )
                            {
                                WarningCount++;
                                Terrain.SideH[X, Y].Road = null;
                            }
                            else
                            {
                                Terrain.SideH[X, Y].Road = Painter.Roads[intTemp];
                            }
                        }
                    }
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X; X++ )
                        {
                            byteTemp = File.ReadByte();
                            intTemp = (byteTemp) - 1;
                            if ( intTemp < 0 )
                            {
                                Terrain.SideV[X, Y].Road = null;
                            }
                            else if ( intTemp >= Painter.RoadCount )
                            {
                                WarningCount++;
                                Terrain.SideV[X, Y].Road = null;
                            }
                            else
                            {
                                Terrain.SideV[X, Y].Road = Painter.Roads[intTemp];
                            }
                        }
                    }
                    if ( WarningCount > 0 )
                    {
                        ReturnResult.WarningAdd(WarningCount + " roads were out of range.");
                    }
                    UInt32 TempUnitCount = 0;
                    TempUnitCount = File.ReadUInt32();
                    sFMEUnit[] TempUnit = new sFMEUnit[(Convert.ToInt32(TempUnitCount))];
                    for ( A = 0; A <= (Convert.ToInt32(TempUnitCount)) - 1; A++ )
                    {
                        TempUnit[A].Code = new string(File.ReadChars(40));
                        B = Strings.InStr(TempUnit[A].Code, Convert.ToString('\0'), (CompareMethod)0);
                        if ( B > 0 )
                        {
                            TempUnit[A].Code = Strings.Left(TempUnit[A].Code, B - 1);
                        }
                        TempUnit[A].LNDType = File.ReadByte();
                        TempUnit[A].ID = File.ReadUInt32();
                        if ( Version == 6U )
                        {
                            TempUnit[A].SavePriority = File.ReadInt32();
                        }
                        TempUnit[A].X = File.ReadUInt32();
                        TempUnit[A].Z = File.ReadUInt32();
                        TempUnit[A].Y = File.ReadUInt32();
                        TempUnit[A].Rotation = File.ReadUInt16();
                        TempUnit[A].Name = IOUtil.ReadOldText(File);
                        TempUnit[A].Player = File.ReadByte();
                    }

                    clsUnit NewUnit = default(clsUnit);
                    clsUnitType UnitType = null;
                    UInt32 AvailableID = 0;

                    AvailableID = 1U;
                    for ( A = 0; A <= (Convert.ToInt32(TempUnitCount)) - 1; A++ )
                    {
                        if ( TempUnit[A].ID >= AvailableID )
                        {
                            AvailableID = TempUnit[A].ID + 1U;
                        }
                    }
                    WarningCount = 0;
                    for ( A = 0; A <= (Convert.ToInt32(TempUnitCount)) - 1; A++ )
                    {
                        if ( TempUnit[A].LNDType == ((byte)0) )
                        {
                            UnitType = modProgram.ObjectData.FindOrCreateUnitType(TempUnit[A].Code, clsUnitType.enumType.Feature, -1);
                        }
                        else if ( TempUnit[A].LNDType == ((byte)1) )
                        {
                            UnitType = modProgram.ObjectData.FindOrCreateUnitType(TempUnit[A].Code, clsUnitType.enumType.PlayerStructure, -1);
                        }
                        else if ( TempUnit[A].LNDType == ((byte)2) )
                        {
                            UnitType = modProgram.ObjectData.FindOrCreateUnitType(Convert.ToString(TempUnit[A].Code), clsUnitType.enumType.PlayerDroid, -1);
                        }
                        else
                        {
                            UnitType = null;
                        }
                        if ( UnitType != null )
                        {
                            NewUnit = new clsUnit();
                            NewUnit.Type = UnitType;
                            NewUnit.ID = TempUnit[A].ID;
                            NewUnit.SavePriority = TempUnit[A].SavePriority;
                            //NewUnit.Name = TempUnit(A).Name
                            if ( TempUnit[A].Player >= modProgram.PlayerCountMax )
                            {
                                NewUnit.UnitGroup = ScavengerUnitGroup;
                            }
                            else
                            {
                                NewUnit.UnitGroup = UnitGroups[TempUnit[A].Player];
                            }
                            NewUnit.Pos.Horizontal.X = Convert.ToInt32(TempUnit[A].X);
                            //NewUnit.Pos.Altitude = TempUnit(A).Y
                            NewUnit.Pos.Horizontal.Y = Convert.ToInt32(TempUnit[A].Z);
                            NewUnit.Rotation = Math.Min(Convert.ToInt32(TempUnit[A].Rotation), 359);
                            if ( TempUnit[A].ID == 0U )
                            {
                                TempUnit[A].ID = AvailableID;
                                modProgram.ZeroIDWarning(NewUnit, TempUnit[A].ID, ReturnResult);
                            }
                            UnitAdd.ID = TempUnit[A].ID;
                            UnitAdd.NewUnit = NewUnit;
                            UnitAdd.Perform();
                            modProgram.ErrorIDChange(TempUnit[A].ID, NewUnit, "Read_FMEv5+");
                            if ( AvailableID == TempUnit[A].ID )
                            {
                                AvailableID = NewUnit.ID + 1U;
                            }
                        }
                        else
                        {
                            WarningCount++;
                        }
                    }
                    if ( WarningCount > 0 )
                    {
                        ReturnResult.WarningAdd(WarningCount + " types of units were invalid. That many units were ignored.");
                    }

                    UInt32 NewGatewayCount = 0;
                    sXY_int NewGateStart = new sXY_int();
                    sXY_int NewGateFinish = new sXY_int();

                    NewGatewayCount = File.ReadUInt32();
                    WarningCount = 0;
                    for ( A = 0; A <= (Convert.ToInt32(NewGatewayCount)) - 1; A++ )
                    {
                        NewGateStart.X = File.ReadUInt16();
                        NewGateStart.Y = File.ReadUInt16();
                        NewGateFinish.X = File.ReadUInt16();
                        NewGateFinish.Y = File.ReadUInt16();
                        if ( GatewayCreate(NewGateStart, NewGateFinish) == null )
                        {
                            WarningCount++;
                        }
                    }
                    if ( WarningCount > 0 )
                    {
                        ReturnResult.WarningAdd(WarningCount + " gateways were invalid.");
                    }

                    if ( Tileset != null )
                    {
                        for ( A = 0; A <= Tileset.TileCount - 1; A++ )
                        {
                            byteTemp = File.ReadByte();
                            Tile_TypeNum[A] = byteTemp;
                        }
                    }

                    //scroll limits
                    ResultInfo.ScrollMin.X = File.ReadInt32();
                    ResultInfo.ScrollMin.Y = File.ReadInt32();
                    ResultInfo.ScrollMax.X = File.ReadUInt32();
                    ResultInfo.ScrollMax.Y = File.ReadUInt32();

                    //other compile info

                    string strTemp = null;

                    ResultInfo.CompileName = IOUtil.ReadOldText(File);
                    byteTemp = File.ReadByte();
                    if ( byteTemp == ((byte)0) )
                    {
                        //no compile type
                    }
                    else if ( byteTemp == ((byte)1) )
                    {
                        //compile multi
                    }
                    else if ( byteTemp == ((byte)2) )
                    {
                        //compile campaign
                    }
                    else
                    {
                        //error
                    }
                    ResultInfo.CompileMultiPlayers = IOUtil.ReadOldText(File);
                    byteTemp = File.ReadByte();
                    if ( byteTemp == ((byte)0) )
                    {
                        ResultInfo.CompileMultiXPlayers = false;
                    }
                    else if ( byteTemp == ((byte)1) )
                    {
                        ResultInfo.CompileMultiXPlayers = true;
                    }
                    else
                    {
                        ReturnResult.WarningAdd("Compile player format out of range.");
                    }
                    ResultInfo.CompileMultiAuthor = IOUtil.ReadOldText(File);
                    ResultInfo.CompileMultiLicense = IOUtil.ReadOldText(File);
                    strTemp = IOUtil.ReadOldText(File); //game time
                    ResultInfo.CampaignGameType = File.ReadInt32();
                    if ( ResultInfo.CampaignGameType < -1 | ResultInfo.CampaignGameType >= modProgram.GameTypeCount )
                    {
                        ReturnResult.WarningAdd("Compile campaign type out of range.");
                        ResultInfo.CampaignGameType = -1;
                    }

                    if ( File.PeekChar() >= 0 )
                    {
                        ReturnResult.WarningAdd("There were unread bytes at the end of the file.");
                    }
                }
                else
                {
                    ReturnResult.ProblemAdd("File version number not recognised.");
                }

                InterfaceOptions = ResultInfo;
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd("Read error: " + ex.Message);
            }

            return ReturnResult;
        }

        public struct sLNDTile
        {
            public short Vertex0Height;
            public short Vertex1Height;
            public short Vertex2Height;
            public short Vertex3Height;
            public short TID;
            public short VF;
            public short TF;
            public short F;
        }

        public class clsLNDObject
        {
            public UInt32 ID;
            public int TypeNum;
            public string Code;
            public int PlayerNum;
            public string Name;
            public sXYZ_sng Pos;
            public sXYZ_int Rotation;
        }

        public clsResult Load_LND(string Path)
        {
            clsResult ReturnResult =
                new clsResult("Loading LND from " + Convert.ToString(ControlChars.Quote) + Path + Convert.ToString(ControlChars.Quote));

            try
            {
                string strTemp = "";
                string strTemp2 = "";
                int X = 0;
                int Y = 0;
                int A = 0;
                int B = 0;
                int Tile_Num = 0;
                SimpleList<string> LineData = default(SimpleList<string>);
                int Line_Num = 0;
                sLNDTile[] LNDTile = null;
                SimpleList<clsLNDObject> LNDObjects = new SimpleList<clsLNDObject>();
                clsUnitAdd UnitAdd = new clsUnitAdd();

                UnitAdd.Map = this;

                BinaryReader Reader = default(BinaryReader);
                try
                {
                    Reader = new BinaryReader(new FileStream(Path, FileMode.Open), modProgram.UTF8Encoding);
                }
                catch ( Exception ex )
                {
                    ReturnResult.ProblemAdd(ex.Message);
                    return ReturnResult;
                }
                LineData = IOUtil.BytesToLinesRemoveComments(Reader);
                Reader.Close();

                Array.Resize(ref LNDTile, LineData.Count);

                string strTemp3 = "";
                bool GotTiles = default(bool);
                bool GotObjects = default(bool);
                bool GotGates = default(bool);
                bool GotTileTypes = default(bool);
                byte[] LNDTileType = new byte[0];
                string[] ObjectText = new string[11];
                string[] GateText = new string[4];
                string[] TileTypeText = new string[256];
                int LNDTileTypeCount = 0;
                SimpleList<clsGateway> LNDGates = new SimpleList<clsGateway>();
                clsGateway Gateway = default(clsGateway);
                int C = 0;
                int D = 0;
                bool GotText = default(bool);
                bool FlipX = default(bool);
                bool FlipZ = default(bool);
                byte Rotation = 0;
                sXY_int NewTileSize = new sXY_int();
                double dblTemp = 0;

                Line_Num = 0;
                while ( Line_Num < LineData.Count )
                {
                    strTemp = LineData[Line_Num];

                    A = strTemp.IndexOf("TileWidth ") + 1;
                    if ( A == 0 )
                    {
                    }
                    else
                    {
                    }

                    A = strTemp.IndexOf("TileHeight ") + 1;
                    if ( A == 0 )
                    {
                    }
                    else
                    {
                    }

                    A = strTemp.IndexOf("MapWidth ") + 1;
                    if ( A == 0 )
                    {
                    }
                    else
                    {
                        IOUtil.InvariantParse_int(strTemp.Substring(strTemp.Length - (strTemp.Length - (A + 8)), strTemp.Length - (A + 8)), ref NewTileSize.X);
                        goto LineDone;
                    }

                    A = strTemp.IndexOf("MapHeight ") + 1;
                    if ( A == 0 )
                    {
                    }
                    else
                    {
                        IOUtil.InvariantParse_int(strTemp.Substring(strTemp.Length - (strTemp.Length - (A + 9)), strTemp.Length - (A + 9)), ref NewTileSize.Y);
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
                            Tileset = modProgram.Tileset_Arizona;

                            goto LineDone;
                        }
                        else if ( strTemp2.IndexOf("tertilesc2") + 1 > 0 )
                        {
                            Tileset = modProgram.Tileset_Urban;

                            goto LineDone;
                        }
                        else if ( strTemp2.IndexOf("tertilesc3") + 1 > 0 )
                        {
                            Tileset = modProgram.Tileset_Rockies;

                            goto LineDone;
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
                                    ReturnResult.ProblemAdd("Tile ID missing");
                                    return ReturnResult;
                                }
                                else
                                {
                                    strTemp2 = strTemp.Substring(strTemp.Length - (strTemp.Length - A - 3), strTemp.Length - A - 3);
                                    A = strTemp2.IndexOf(" ") + 1;
                                    if ( A > 0 )
                                    {
                                        strTemp2 = strTemp2.Substring(0, A - 1);
                                    }
                                    Int16 temp_Result = LNDTile[Tile_Num].TID;
                                    IOUtil.InvariantParse_short(strTemp2, ref temp_Result);
                                }

                                A = strTemp.IndexOf("VF ") + 1;
                                if ( A == 0 )
                                {
                                    ReturnResult.ProblemAdd("Tile VF missing");
                                    return ReturnResult;
                                }
                                else
                                {
                                    strTemp2 = strTemp.Substring(strTemp.Length - (strTemp.Length - A - 2), strTemp.Length - A - 2);
                                    A = strTemp2.IndexOf(" ") + 1;
                                    if ( A > 0 )
                                    {
                                        strTemp2 = strTemp2.Substring(0, A - 1);
                                    }
                                    Int16 temp_Result2 = LNDTile[Tile_Num].VF;
                                    IOUtil.InvariantParse_short(strTemp2, ref temp_Result2);
                                }

                                A = strTemp.IndexOf("TF ") + 1;
                                if ( A == 0 )
                                {
                                    ReturnResult.ProblemAdd("Tile TF missing");
                                    return ReturnResult;
                                }
                                else
                                {
                                    strTemp2 = strTemp.Substring(strTemp.Length - (strTemp.Length - A - 2), strTemp.Length - A - 2);
                                    A = strTemp2.IndexOf(" ") + 1;
                                    if ( A > 0 )
                                    {
                                        strTemp2 = strTemp2.Substring(0, A - 1);
                                    }
                                    Int16 temp_Result3 = LNDTile[Tile_Num].TF;
                                    IOUtil.InvariantParse_short(strTemp2, ref temp_Result3);
                                }

                                A = strTemp.IndexOf(" F ") + 1;
                                if ( A == 0 )
                                {
                                    ReturnResult.ProblemAdd("Tile flip missing");
                                    return ReturnResult;
                                }
                                else
                                {
                                    strTemp2 = Strings.Right(strTemp, strTemp.Length - A - 2);
                                    A = strTemp2.IndexOf(" ") + 1;
                                    if ( A > 0 )
                                    {
                                        strTemp2 = strTemp2.Substring(0, A - 1);
                                    }
                                    short temp_Result4 = LNDTile[Tile_Num].F;
                                    IOUtil.InvariantParse_short(strTemp2, ref temp_Result4);
                                }

                                A = strTemp.IndexOf(" VH ") + 1;
                                if ( A == 0 )
                                {
                                    ReturnResult.ProblemAdd("Tile height is missing");
                                    return ReturnResult;
                                }
                                else
                                {
                                    strTemp3 = strTemp.Substring(strTemp.Length - (strTemp.Length - A - 3), strTemp.Length - A - 3);
                                    for ( A = 0; A <= 2; A++ )
                                    {
                                        B = strTemp3.IndexOf(" ") + 1;
                                        if ( B == 0 )
                                        {
                                            ReturnResult.ProblemAdd("A tile height value is missing");
                                            return ReturnResult;
                                        }
                                        strTemp2 = strTemp3.Substring(0, B - 1);
                                        strTemp3 = strTemp3.Substring(strTemp3.Length - (strTemp3.Length - B), strTemp3.Length - B);

                                        if ( A == 0 )
                                        {
                                            short temp_Result5 = LNDTile[Tile_Num].Vertex0Height;
                                            IOUtil.InvariantParse_short(strTemp2, ref temp_Result5);
                                        }
                                        else if ( A == 1 )
                                        {
                                            short temp_Result6 = LNDTile[Tile_Num].Vertex1Height;
                                            IOUtil.InvariantParse_short(strTemp2, ref temp_Result6);
                                        }
                                        else if ( A == 2 )
                                        {
                                            Int16 temp_Result7 = LNDTile[Tile_Num].Vertex2Height;
                                            IOUtil.InvariantParse_short(strTemp2, ref temp_Result7);
                                        }
                                    }
                                    short temp_Result8 = LNDTile[Tile_Num].Vertex3Height;
                                    IOUtil.InvariantParse_short(strTemp3, ref temp_Result8);
                                }

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
                                                ReturnResult.ProblemAdd("Too many fields for an object, or a space at the end.");
                                                return ReturnResult;
                                            }
                                            ObjectText[C] = "";
                                            GotText = false;
                                        }
                                    }
                                }

                                clsLNDObject NewObject = new clsLNDObject();
                                IOUtil.InvariantParse_uint(ObjectText[0], NewObject.ID);
                                IOUtil.InvariantParse_int(ObjectText[1], ref NewObject.TypeNum);
                                NewObject.Code = ObjectText[2].Substring(1, ObjectText[2].Length - 2); //remove quotes
                                IOUtil.InvariantParse_int(ObjectText[3], ref NewObject.PlayerNum);
                                NewObject.Name = ObjectText[4].Substring(1, ObjectText[4].Length - 2); //remove quotes
                                IOUtil.InvariantParse_sng(ObjectText[5], ref NewObject.Pos.X);
                                IOUtil.InvariantParse_sng(ObjectText[6], ref NewObject.Pos.Y);
                                IOUtil.InvariantParse_sng(ObjectText[7], ref NewObject.Pos.Z);
                                if ( IOUtil.InvariantParse_dbl(ObjectText[8], ref dblTemp) )
                                {
                                    NewObject.Rotation.X = (int)(MathUtil.Clamp_dbl(dblTemp, 0.0D, 359.0D));
                                }
                                if ( IOUtil.InvariantParse_dbl(ObjectText[9], ref dblTemp) )
                                {
                                    NewObject.Rotation.Y = (int)(MathUtil.Clamp_dbl(dblTemp, 0.0D, 359.0D));
                                }
                                if ( IOUtil.InvariantParse_dbl(ObjectText[10], ref dblTemp) )
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
                                                ReturnResult.ProblemAdd("Too many fields for a gateway, or a space at the end.");
                                                return ReturnResult;
                                            }
                                            GateText[C] = "";
                                            GotText = false;
                                        }
                                    }
                                }

                                Gateway = new clsGateway();
                                IOUtil.InvariantParse_int(GateText[0], ref Gateway.PosA.X);
                                Gateway.PosA.X = Math.Max(Gateway.PosA.X, 0);
                                IOUtil.InvariantParse_int(GateText[1], ref Gateway.PosA.Y);
                                Gateway.PosA.Y = Math.Max(Gateway.PosA.Y, 0);
                                IOUtil.InvariantParse_int(GateText[2], ref Gateway.PosB.X);
                                Gateway.PosB.X = Math.Max(Gateway.PosB.X, 0);
                                IOUtil.InvariantParse_int(GateText[3], ref Gateway.PosB.Y);
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
                                                ReturnResult.ProblemAdd("Too many fields for tile types.");
                                                return ReturnResult;
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
                        goto LineDone;
                    }

                    LineDone:
                    Line_Num++;
                }

                Array.Resize(ref LNDTile, Tile_Num);

                SetPainterToDefaults();

                if ( NewTileSize.X < 1 | NewTileSize.Y < 1 )
                {
                    ReturnResult.ProblemAdd("The LND\'s terrain dimensions are missing or invalid.");
                    return ReturnResult;
                }

                TerrainBlank(NewTileSize);
                TileType_Reset();

                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        Tile_Num = Y * Terrain.TileSize.X + X;
                        //lnd uses different order! (3 = 2, 2 = 3), this program goes left to right, lnd goes clockwise around each tile
                        Terrain.Vertices[X, Y].Height = (byte)(LNDTile[Tile_Num].Vertex0Height);
                    }
                }

                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        Tile_Num = Y * Terrain.TileSize.X + X;

                        Terrain.Tiles[X, Y].Texture.TextureNum = LNDTile[Tile_Num].TID - 1;

                        //ignore higher values
                        A = Convert.ToInt32(Conversion.Int(LNDTile[Tile_Num].F / 64.0D));
                        LNDTile[Tile_Num].F = (short)(LNDTile[Tile_Num].F - A * 64);

                        A = (int)(Conversion.Int(LNDTile[Tile_Num].F / 16.0D));
                        LNDTile[Tile_Num].F = (short)(LNDTile[Tile_Num].F - A * 16);
                        if ( A < 0 | A > 3 )
                        {
                            ReturnResult.ProblemAdd("Invalid flip value.");
                            return ReturnResult;
                        }
                        Rotation = (byte)A;

                        A = (int)(Conversion.Int(LNDTile[Tile_Num].F / 8.0D));
                        LNDTile[Tile_Num].F -= (short)(A * 8);
                        FlipZ = A == 1;

                        A = (int)(Conversion.Int(LNDTile[Tile_Num].F / 4.0D));
                        LNDTile[Tile_Num].F -= (short)(A * 4);
                        FlipX = A == 1;

                        A = Convert.ToInt32(Conversion.Int(LNDTile[Tile_Num].F / 2.0D));
                        LNDTile[Tile_Num].F -= (short)(A * 2);
                        Terrain.Tiles[X, Y].Tri = A == 1;

                        //vf, tf, ignore

                        TileOrientation.OldOrientation_To_TileOrientation(Rotation, FlipX, FlipZ, Terrain.Tiles[X, Y].Texture.Orientation);
                    }
                }

                clsUnit NewUnit = default(clsUnit);
                sXYZ_int XYZ_int = new sXYZ_int();
                clsUnitType NewType = default(clsUnitType);
                UInt32 AvailableID = 0;
                clsLNDObject CurrentObject = default(clsLNDObject);

                AvailableID = 1U;
                foreach ( clsLNDObject tempLoopVar_CurrentObject in LNDObjects )
                {
                    CurrentObject = tempLoopVar_CurrentObject;
                    if ( CurrentObject.ID >= AvailableID )
                    {
                        AvailableID = CurrentObject.ID + 1U;
                    }
                }
                foreach ( clsLNDObject tempLoopVar_CurrentObject in LNDObjects )
                {
                    CurrentObject = tempLoopVar_CurrentObject;
                    switch ( CurrentObject.TypeNum )
                    {
                        case 0:
                            NewType = modProgram.ObjectData.FindOrCreateUnitType(CurrentObject.Code, clsUnitType.enumType.Feature, -1);
                            break;
                        case 1:
                            NewType = modProgram.ObjectData.FindOrCreateUnitType(CurrentObject.Code, clsUnitType.enumType.PlayerStructure, -1);
                            break;
                        case 2:
                            NewType = modProgram.ObjectData.FindOrCreateUnitType(CurrentObject.Code, clsUnitType.enumType.PlayerDroid, -1);
                            break;
                        default:
                            NewType = null;
                            break;
                    }
                    if ( NewType != null )
                    {
                        NewUnit = new clsUnit();
                        NewUnit.Type = NewType;
                        if ( CurrentObject.PlayerNum < 0 | CurrentObject.PlayerNum >= modProgram.PlayerCountMax )
                        {
                            NewUnit.UnitGroup = ScavengerUnitGroup;
                        }
                        else
                        {
                            NewUnit.UnitGroup = UnitGroups[CurrentObject.PlayerNum];
                        }
                        XYZ_int.X = (int)CurrentObject.Pos.X;
                        XYZ_int.Y = (int)CurrentObject.Pos.Y;
                        XYZ_int.Z = (int)CurrentObject.Pos.Z;
                        NewUnit.Pos = MapPos_From_LNDPos(XYZ_int);
                        NewUnit.Rotation = CurrentObject.Rotation.Y;
                        if ( CurrentObject.ID == 0U )
                        {
                            CurrentObject.ID = AvailableID;
                            modProgram.ZeroIDWarning(NewUnit, CurrentObject.ID, ReturnResult);
                        }
                        UnitAdd.NewUnit = NewUnit;
                        UnitAdd.ID = CurrentObject.ID;
                        UnitAdd.Perform();
                        modProgram.ErrorIDChange(CurrentObject.ID, NewUnit, "Load_LND");
                        if ( AvailableID == CurrentObject.ID )
                        {
                            AvailableID = NewUnit.ID + 1U;
                        }
                    }
                }

                foreach ( clsGateway tempLoopVar_Gateway in LNDGates )
                {
                    Gateway = tempLoopVar_Gateway;
                    GatewayCreate(Gateway.PosA, Gateway.PosB);
                }

                if ( Tileset != null )
                {
                    for ( A = 0; A <= Math.Min(LNDTileTypeCount - 1, Tileset.TileCount) - 1; A++ )
                    {
                        Tile_TypeNum[A] = LNDTileType[A + 1]; //lnd value 0 is ignored
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            return ReturnResult;
        }

        public sXYZ_int LNDPos_From_MapPos(sXY_int Horizontal)
        {
            sXYZ_int Result = new sXYZ_int();

            Result.X = Horizontal.X - (int)(Terrain.TileSize.X * modProgram.TerrainGridSpacing / 2.0D);
            Result.Z = ((int)(Terrain.TileSize.Y * modProgram.TerrainGridSpacing / 2.0D)) - Horizontal.Y;
            Result.Y = (int)(GetTerrainHeight(Horizontal));

            return Result;
        }

        public modProgram.sWorldPos MapPos_From_LNDPos(sXYZ_int Pos)
        {
            modProgram.sWorldPos Result = new modProgram.sWorldPos();

            Result.Horizontal.X = Pos.X + (int)(Terrain.TileSize.X * modProgram.TerrainGridSpacing / 2.0D);
            Result.Horizontal.Y = ((int)(Terrain.TileSize.Y * modProgram.TerrainGridSpacing / 2.0D)) - Pos.Z;
            Result.Altitude = (int)(GetTerrainHeight(Result.Horizontal));

            return Result;
        }

        public clsResult Write_LND(string Path, bool Overwrite)
        {
            clsResult ReturnResult =
                new clsResult("Writing LND to " + Convert.ToString(ControlChars.Quote) + Path + Convert.ToString(ControlChars.Quote));

            if ( System.IO.File.Exists(Path) )
            {
                if ( Overwrite )
                {
                    System.IO.File.Delete(Path);
                }
                else
                {
                    ReturnResult.ProblemAdd("The selected file already exists.");
                    return ReturnResult;
                }
            }

            StreamWriter File = null;

            try
            {
                string Text = "";
                char EndChar = (char)0;
                char Quote = (char)0;
                int A = 0;
                int X = 0;
                int Y = 0;
                byte Flip = 0;
                int B = 0;
                int VF = 0;
                int TF = 0;
                int C = 0;
                byte Rotation = 0;
                bool FlipX = default(bool);

                Quote = ControlChars.Quote;
                EndChar = '\n';

                File = new StreamWriter(new FileStream(Path, FileMode.CreateNew), new UTF8Encoding(false, false));

                if ( Tileset == modProgram.Tileset_Arizona )
                {
                    Text = "DataSet WarzoneDataC1.eds" + Convert.ToString(EndChar);
                }
                else if ( Tileset == modProgram.Tileset_Urban )
                {
                    Text = "DataSet WarzoneDataC2.eds" + Convert.ToString(EndChar);
                }
                else if ( Tileset == modProgram.Tileset_Rockies )
                {
                    Text = "DataSet WarzoneDataC3.eds" + Convert.ToString(EndChar);
                }
                else
                {
                    Text = "DataSet " + Convert.ToString(EndChar);
                }
                File.Write(Text);
                Text = "GrdLand {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Version 4" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    3DPosition 0.000000 3072.000000 0.000000" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    3DRotation 80.000000 0.000000 0.000000" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    2DPosition 0 0" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    CustomSnap 16 16" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    SnapMode 0" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Gravity 1" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    HeightScale " + IOUtil.InvariantToString_int(HeightMultiplier) + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    MapWidth " + IOUtil.InvariantToString_int(Terrain.TileSize.X) + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    MapHeight " + IOUtil.InvariantToString_int(Terrain.TileSize.Y) + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    TileWidth 128" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    TileHeight 128" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    SeaLevel 0" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    TextureWidth 64" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    TextureHeight 64" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    NumTextures 1" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Textures {" + Convert.ToString(EndChar);
                File.Write(Text);
                if ( Tileset == modProgram.Tileset_Arizona )
                {
                    Text = "        texpages\\tertilesc1.pcx" + Convert.ToString(EndChar);
                }
                else if ( Tileset == modProgram.Tileset_Urban )
                {
                    Text = "        texpages\\tertilesc2.pcx" + Convert.ToString(EndChar);
                }
                else if ( Tileset == modProgram.Tileset_Rockies )
                {
                    Text = "        texpages\\tertilesc3.pcx" + Convert.ToString(EndChar);
                }
                else
                {
                    Text = "        " + Convert.ToString(EndChar);
                }
                File.Write(Text);
                Text = "    }" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    NumTiles " + IOUtil.InvariantToString_int(Terrain.TileSize.X * Terrain.TileSize.Y) + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Tiles {" + Convert.ToString(EndChar);
                File.Write(Text);
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        TileOrientation.TileOrientation_To_OldOrientation(Terrain.Tiles[X, Y].Texture.Orientation, ref Rotation, ref FlipX);
                        Flip = (byte)0;
                        if ( Terrain.Tiles[X, Y].Tri )
                        {
                            Flip += (byte)2;
                        }
                        if ( FlipX )
                        {
                            Flip += (byte)4;
                        }
                        Flip += (byte)(Rotation * 16);

                        if ( Terrain.Tiles[X, Y].Tri )
                        {
                            VF = 1;
                        }
                        else
                        {
                            VF = 0;
                        }
                        if ( FlipX )
                        {
                            TF = 1;
                        }
                        else
                        {
                            TF = 0;
                        }

                        Text = "        TID " + (Terrain.Tiles[X, Y].Texture.TextureNum + 1) + " VF " + IOUtil.InvariantToString_int(VF) + " TF " +
                               IOUtil.InvariantToString_int(TF) + " F " + IOUtil.InvariantToString_int(Flip) + " VH " +
                               IOUtil.InvariantToString_byte(Convert.ToByte(Terrain.Vertices[X, Y].Height)) + " " +
                               IOUtil.InvariantToString_byte(Terrain.Vertices[X + 1, Y].Height) + " " + Convert.ToString(Terrain.Vertices[X + 1, Y + 1].Height) +
                               " " + IOUtil.InvariantToString_byte(Convert.ToByte(Terrain.Vertices[X, Y + 1].Height)) + Convert.ToString(EndChar);
                        File.Write(Text);
                    }
                }
                Text = "    }" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "}" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "ObjectList {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Version 3" + Convert.ToString(EndChar);
                File.Write(Text);
                if ( Tileset == modProgram.Tileset_Arizona )
                {
                    Text = "	FeatureSet WarzoneDataC1.eds" + Convert.ToString(EndChar);
                }
                else if ( Tileset == modProgram.Tileset_Urban )
                {
                    Text = "	FeatureSet WarzoneDataC2.eds" + Convert.ToString(EndChar);
                }
                else if ( Tileset == modProgram.Tileset_Rockies )
                {
                    Text = "	FeatureSet WarzoneDataC3.eds" + Convert.ToString(EndChar);
                }
                else
                {
                    Text = "	FeatureSet " + Convert.ToString(EndChar);
                }
                File.Write(Text);
                Text = "    NumObjects " + IOUtil.InvariantToString_int(Units.Count) + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Objects {" + Convert.ToString(EndChar);
                File.Write(Text);
                sXYZ_int XYZ_int = new sXYZ_int();
                string Code = null;
                int CustomDroidCount = 0;
                clsUnit Unit = default(clsUnit);
                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    switch ( Unit.Type.Type )
                    {
                        case clsUnitType.enumType.Feature:
                            B = 0;
                            break;
                        case clsUnitType.enumType.PlayerStructure:
                            B = 1;
                            break;
                        case clsUnitType.enumType.PlayerDroid:
                            if ( ((clsDroidDesign)Unit.Type).IsTemplate )
                            {
                                B = 2;
                            }
                            else
                            {
                                B = -1;
                            }
                            break;
                        default:
                            B = -1;
                            ReturnResult.WarningAdd("Unit type classification not accounted for.");
                            break;
                    }
                    XYZ_int = LNDPos_From_MapPos(Units[A].Pos.Horizontal);
                    if ( B >= 0 )
                    {
                        if ( Unit.Type.GetCode(ref Code) )
                        {
                            Text = "        " + IOUtil.InvariantToString_uint(Unit.ID) + " " + Convert.ToString(B) + " " + Convert.ToString(Quote) +
                                   Code + Convert.ToString(Quote) + " " + Unit.UnitGroup.GetLNDPlayerText() + " " + Convert.ToString(Quote) + "NONAME" +
                                   Convert.ToString(Quote) + " " + IOUtil.InvariantToString_int(XYZ_int.X) + ".00 " + IOUtil.InvariantToString_int(XYZ_int.Y) +
                                   ".00 " + IOUtil.InvariantToString_int(XYZ_int.Z) + ".00 0.00 " + IOUtil.InvariantToString_int(Unit.Rotation) + ".00 0.00" +
                                   Convert.ToString(EndChar);
                            File.Write(Text);
                        }
                        else
                        {
                            ReturnResult.WarningAdd("Error. Code not found.");
                        }
                    }
                    else
                    {
                        CustomDroidCount++;
                    }
                }
                Text = "    }" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "}" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "ScrollLimits {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Version 1" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    NumLimits 1" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Limits {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "        " + Convert.ToString(Quote) + "Entire Map" + Convert.ToString(Quote) + " 0 0 0 " +
                       IOUtil.InvariantToString_int(Terrain.TileSize.X) + " " + IOUtil.InvariantToString_int(Terrain.TileSize.Y) + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    }" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "}" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "Gateways {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Version 1" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    NumGateways " + IOUtil.InvariantToString_int(Gateways.Count) + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Gates {" + Convert.ToString(EndChar);
                File.Write(Text);
                clsGateway Gateway = default(clsGateway);
                foreach ( clsGateway tempLoopVar_Gateway in Gateways )
                {
                    Gateway = tempLoopVar_Gateway;
                    Text = "        " + IOUtil.InvariantToString_int(Gateway.PosA.X) + " " + IOUtil.InvariantToString_int(Gateway.PosA.Y) + " " +
                           IOUtil.InvariantToString_int(Gateway.PosB.X) + " " + IOUtil.InvariantToString_int(Gateway.PosB.Y) + Convert.ToString(EndChar);
                    File.Write(Text);
                }
                Text = "    }" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "}" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "TileTypes {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    NumTiles " + Convert.ToString(Tileset.TileCount) + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Tiles {" + Convert.ToString(EndChar);
                File.Write(Text);
                for ( A = 0; A <= ((int)(Math.Ceiling(Convert.ToDecimal((Tileset.TileCount + 1) / 16.0D)))) - 1; A++ )
                    //+1 because the first number is not a tile type
                {
                    Text = "        ";
                    C = A * 16 - 1; //-1 because the first number is not a tile type
                    for ( B = 0; B <= Math.Min(16, Tileset.TileCount - C) - 1; B++ )
                    {
                        if ( C + B < 0 )
                        {
                            Text = Text + "2 ";
                        }
                        else
                        {
                            Text = Text + IOUtil.InvariantToString_byte(Tile_TypeNum[C + B]) + " ";
                        }
                    }
                    Text = Text + Convert.ToString(EndChar);
                    File.Write(Text);
                }
                Text = "    }" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "}" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "TileFlags {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    NumTiles 90" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Flags {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "        0 0 0 0 0 0 0 0 0 0 " + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    }" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "}" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "Brushes {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    Version 2" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    NumEdgeBrushes 0" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    NumUserBrushes 0" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    EdgeBrushes {" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "    }" + Convert.ToString(EndChar);
                File.Write(Text);
                Text = "}" + Convert.ToString(EndChar);
                File.Write(Text);
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }
            if ( File != null )
            {
                File.Close();
            }

            return ReturnResult;
        }

        public modProgram.sResult Write_MinimapFile(string Path, bool Overwrite)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            int X = 0;
            int Y = 0;

            Bitmap MinimapBitmap = new Bitmap(Terrain.TileSize.X, Terrain.TileSize.Y);

            clsMinimapTexture Texture = new clsMinimapTexture(new sXY_int(Terrain.TileSize.X, Terrain.TileSize.Y));
            MinimapTextureFill(Texture);

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    MinimapBitmap.SetPixel(X, Y,
                        ColorTranslator.FromOle(
                            ColorUtil.OSRGB((int)(MathUtil.Clamp_sng(Convert.ToSingle(Texture.get_Pixels(X, Y).Red * 255.0F), 0.0F, 255.0F)),
                                (int)(MathUtil.Clamp_sng(Convert.ToSingle(Texture.get_Pixels(X, Y).Green * 255.0F), 0.0F, 255.0F)),
                                (int)(MathUtil.Clamp_sng(Convert.ToSingle(Texture.get_Pixels(X, Y).Blue * 255.0F), 0.0F, 255.0F)))));
                }
            }

            ReturnResult = BitmapUtil.SaveBitmap(Path, Overwrite, MinimapBitmap);

            return ReturnResult;
        }

        public modProgram.sResult Write_Heightmap(string Path, bool Overwrite)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            Bitmap HeightmapBitmap = new Bitmap(Terrain.TileSize.X + 1, Terrain.TileSize.Y + 1);
            int X = 0;
            int Y = 0;

            for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X; X++ )
                {
                    HeightmapBitmap.SetPixel(X, Y,
                        ColorTranslator.FromOle(ColorUtil.OSRGB(Convert.ToInt32(Terrain.Vertices[X, Y].Height), Terrain.Vertices[X, Y].Height,
                            Terrain.Vertices[X, Y].Height)));
                }
            }

            ReturnResult = BitmapUtil.SaveBitmap(Path, Overwrite, HeightmapBitmap);
            return ReturnResult;
        }

        public modProgram.sResult Write_TTP(string Path, bool Overwrite)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            if ( File.Exists(Path) )
            {
                if ( Overwrite )
                {
                    File.Delete(Path);
                }
                else
                {
                    ReturnResult.Problem = "File already exists.";
                    return ReturnResult;
                }
            }

            BinaryWriter File_TTP = default(BinaryWriter);

            try
            {
                File_TTP = new BinaryWriter(new FileStream(Path, FileMode.CreateNew), modProgram.ASCIIEncoding);
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
                return ReturnResult;
            }

            int A = 0;

            IOUtil.WriteText(File_TTP, false, "ttyp");

            File_TTP.Write(8U);
            if ( Tileset == null )
            {
                File_TTP.Write(0U);
            }
            else
            {
                File_TTP.Write(Convert.ToBoolean((uint)Tileset.TileCount));
                for ( A = 0; A <= Tileset.TileCount - 1; A++ )
                {
                    File_TTP.Write(Convert.ToBoolean(Tile_TypeNum[A]));
                }
            }
            File_TTP.Close();

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public modProgram.sResult Load_TTP(string Path)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";
            BinaryReader File = default(BinaryReader);

            try
            {
                File = new BinaryReader(new FileStream(Path, FileMode.Open));
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
                return ReturnResult;
            }
            ReturnResult = Read_TTP(File);
            File.Close();

            return ReturnResult;
        }

        public modProgram.sResult Write_FME(string Path, bool Overwrite, byte ScavengerPlayerNum)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            if ( System.IO.File.Exists(Path) )
            {
                if ( Overwrite )
                {
                    System.IO.File.Delete(Path);
                }
                else
                {
                    ReturnResult.Problem = "The selected file already exists.";
                    return ReturnResult;
                }
            }

            BinaryWriter File = null;

            try
            {
                File = new BinaryWriter(new FileStream(Path, FileMode.CreateNew));

                int X = 0;
                int Z = 0;

                File.Write(6U);

                if ( Tileset == null )
                {
                    File.Write((byte)0);
                }
                else if ( Tileset == modProgram.Tileset_Arizona )
                {
                    File.Write((byte)1);
                }
                else if ( Tileset == modProgram.Tileset_Urban )
                {
                    File.Write((byte)2);
                }
                else if ( Tileset == modProgram.Tileset_Rockies )
                {
                    File.Write((byte)3);
                }

                File.Write(Convert.ToBoolean((ushort)Terrain.TileSize.X));
                File.Write(Convert.ToBoolean((ushort)Terrain.TileSize.Y));

                byte TileAttributes = 0;
                byte DownSideData = 0;

                for ( Z = 0; Z <= Terrain.TileSize.Y; Z++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X; X++ )
                    {
                        File.Write(Terrain.Vertices[X, Z].Height);
                        if ( Terrain.Vertices[X, Z].Terrain == null )
                        {
                            File.Write((byte)0);
                        }
                        else if ( Terrain.Vertices[X, Z].Terrain.Num < 0 )
                        {
                            ReturnResult.Problem = "Terrain number out of range.";
                            return ReturnResult;
                        }
                        else
                        {
                            File.Write(Convert.ToByte(Terrain.Vertices[X, Z].Terrain.Num + 1));
                        }
                    }
                }
                for ( Z = 0; Z <= Terrain.TileSize.Y - 1; Z++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        File.Write(Convert.ToByte(Terrain.Tiles[X, Z].Texture.TextureNum + 1));

                        TileAttributes = (byte)0;
                        if ( Terrain.Tiles[X, Z].Terrain_IsCliff )
                        {
                            TileAttributes += (byte)128;
                        }
                        if ( Terrain.Tiles[X, Z].Texture.Orientation.SwitchedAxes )
                        {
                            TileAttributes += (byte)64;
                        }
                        if ( Terrain.Tiles[X, Z].Texture.Orientation.ResultXFlip )
                        {
                            TileAttributes += (byte)32;
                        }
                        if ( Terrain.Tiles[X, Z].Texture.Orientation.ResultYFlip )
                        {
                            TileAttributes += (byte)16;
                        }
                        //8 is free
                        if ( Terrain.Tiles[X, Z].Tri )
                        {
                            TileAttributes += (byte)4;
                            if ( Terrain.Tiles[X, Z].TriTopLeftIsCliff )
                            {
                                TileAttributes += (byte)2;
                            }
                            if ( Terrain.Tiles[X, Z].TriBottomRightIsCliff )
                            {
                                TileAttributes += (byte)1;
                            }
                        }
                        else
                        {
                            if ( Terrain.Tiles[X, Z].TriBottomLeftIsCliff )
                            {
                                TileAttributes += (byte)2;
                            }
                            if ( Terrain.Tiles[X, Z].TriTopRightIsCliff )
                            {
                                TileAttributes += (byte)1;
                            }
                        }
                        File.Write(TileAttributes);
                        if ( TileOrientation.IdenticalTileDirections(Terrain.Tiles[X, Z].DownSide, TileOrientation.TileDirection_Top) )
                        {
                            DownSideData = (byte)1;
                        }
                        else if ( TileOrientation.IdenticalTileDirections(Terrain.Tiles[X, Z].DownSide, TileOrientation.TileDirection_Left) )
                        {
                            DownSideData = (byte)2;
                        }
                        else if ( TileOrientation.IdenticalTileDirections(Terrain.Tiles[X, Z].DownSide, TileOrientation.TileDirection_Right) )
                        {
                            DownSideData = (byte)3;
                        }
                        else if ( TileOrientation.IdenticalTileDirections(Terrain.Tiles[X, Z].DownSide, TileOrientation.TileDirection_Bottom) )
                        {
                            DownSideData = (byte)4;
                        }
                        else
                        {
                            DownSideData = (byte)0;
                        }
                        File.Write(DownSideData);
                    }
                }
                for ( Z = 0; Z <= Terrain.TileSize.Y; Z++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        if ( Terrain.SideH[X, Z].Road == null )
                        {
                            File.Write((byte)0);
                        }
                        else if ( Terrain.SideH[X, Z].Road.Num < 0 )
                        {
                            ReturnResult.Problem = "Road number out of range.";
                            return ReturnResult;
                        }
                        else
                        {
                            File.Write(Convert.ToByte(Terrain.SideH[X, Z].Road.Num + 1));
                        }
                    }
                }
                for ( Z = 0; Z <= Terrain.TileSize.Y - 1; Z++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X; X++ )
                    {
                        if ( Terrain.SideV[X, Z].Road == null )
                        {
                            File.Write((byte)0);
                        }
                        else if ( Terrain.SideV[X, Z].Road.Num < 0 )
                        {
                            ReturnResult.Problem = "Road number out of range.";
                            return ReturnResult;
                        }
                        else
                        {
                            File.Write(Convert.ToByte(Terrain.SideV[X, Z].Road.Num + 1));
                        }
                    }
                }

                clsUnit[] OutputUnits = new clsUnit[Units.Count];
                string[] OutputUnitCode = new string[Units.Count];
                int OutputUnitCount = 0;
                clsUnit Unit = default(clsUnit);
                int A = 0;

                foreach ( clsUnit tempLoopVar_Unit in Units )
                {
                    Unit = tempLoopVar_Unit;
                    if ( Unit.Type.GetCode(ref OutputUnitCode[OutputUnitCount]) )
                    {
                        OutputUnits[OutputUnitCount] = Unit;
                        OutputUnitCount++;
                    }
                }

                File.Write(Convert.ToBoolean((uint)OutputUnitCount));

                for ( A = 0; A <= OutputUnitCount - 1; A++ )
                {
                    Unit = OutputUnits[A];
                    IOUtil.WriteTextOfLength(File, 40, OutputUnitCode[A]);
                    switch ( Unit.Type.Type )
                    {
                        case clsUnitType.enumType.Feature:
                            File.Write((byte)0);
                            break;
                        case clsUnitType.enumType.PlayerStructure:
                            File.Write((byte)1);
                            break;
                        case clsUnitType.enumType.PlayerDroid:
                            File.Write((byte)2);
                            break;
                    }
                    File.Write(Unit.ID);
                    File.Write(Unit.SavePriority);
                    File.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.X));
                    File.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.Y));
                    File.Write(Convert.ToBoolean((uint)Unit.Pos.Altitude));
                    File.Write(Convert.ToBoolean((ushort)Unit.Rotation));
                    IOUtil.WriteText(File, true, "");
                    if ( Unit.UnitGroup == ScavengerUnitGroup )
                    {
                        File.Write(ScavengerPlayerNum);
                    }
                    else
                    {
                        File.Write((byte)Unit.UnitGroup.WZ_StartPos);
                    }
                }

                File.Write(Convert.ToBoolean((uint)Gateways.Count));

                clsGateway Gateway = default(clsGateway);
                foreach ( clsGateway tempLoopVar_Gateway in Gateways )
                {
                    Gateway = tempLoopVar_Gateway;
                    File.Write(Convert.ToBoolean((ushort)Gateway.PosA.X));
                    File.Write(Convert.ToBoolean((ushort)Gateway.PosA.Y));
                    File.Write(Convert.ToBoolean((ushort)Gateway.PosB.X));
                    File.Write(Convert.ToBoolean((ushort)Gateway.PosB.Y));
                }

                if ( Tileset != null )
                {
                    for ( A = 0; A <= Tileset.TileCount - 1; A++ )
                    {
                        File.Write(Tile_TypeNum[A]);
                    }
                }

                //scroll limits
                File.Write(InterfaceOptions.ScrollMin.X);
                File.Write(InterfaceOptions.ScrollMin.Y);
                File.Write(InterfaceOptions.ScrollMax.X);
                File.Write(InterfaceOptions.ScrollMax.Y);

                //other compile info
                IOUtil.WriteText(File, true, InterfaceOptions.CompileName);
                File.Write((byte)0); //multiplayer/campaign. 0 = neither
                IOUtil.WriteText(File, true, InterfaceOptions.CompileMultiPlayers);
                File.Write(InterfaceOptions.CompileMultiXPlayers);
                IOUtil.WriteText(File, true, InterfaceOptions.CompileMultiAuthor);
                IOUtil.WriteText(File, true, InterfaceOptions.CompileMultiLicense);
                IOUtil.WriteText(File, true, "0"); //game time
                int intTemp = InterfaceOptions.CampaignGameType;
                File.Write(intTemp);
            }
            catch ( Exception ex )
            {
                ReturnResult.Problem = ex.Message;
                return ReturnResult;
            }

            if ( File != null )
            {
                File.Close();
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }
    }
}