using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualBasic;
using SharpFlame.MathExtra;

namespace SharpFlame
{
    public partial class clsMap
    {
        public class clsFMap_INIObjects : clsINIRead.clsSectionTranslator
        {
            public struct sObject
            {
                public UInt32 ID;
                public clsUnitType.enumType Type;
                public bool IsTemplate;
                public string Code;
                public string UnitGroup;
                public bool GotAltitude;
                public clsXY_int Pos;
                public double Heading;
                public double Health;
                public clsDroidDesign.clsTemplateDroidType TemplateDroidType;
                public string BodyCode;
                public string PropulsionCode;
                public clsTurret.enumTurretType[] TurretTypes;
                public string[] TurretCodes;
                public int TurretCount;
                public int Priority;
                public string Label;
                public int WallType;
            }

            public sObject[] Objects;
            public int ObjectCount;

            public clsFMap_INIObjects(int NewObjectCount)
            {
                int A = 0;
                int B = 0;

                ObjectCount = NewObjectCount;
                Objects = new sObject[ObjectCount];
                for ( A = 0; A <= ObjectCount - 1; A++ )
                {
                    Objects[A].Type = clsUnitType.enumType.Unspecified;
                    Objects[A].Health = 1.0D;
                    Objects[A].WallType = -1;
                    Objects[A].TurretCodes = new string[3];
                    Objects[A].TurretTypes = new clsTurret.enumTurretType[3];
                    for ( B = 0; B <= modProgram.MaxDroidWeapons - 1; B++ )
                    {
                        Objects[A].TurretTypes[B] = clsTurret.enumTurretType.Unknown;
                    }
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                if ( (string)INIProperty.Name == "type" )
                {
                    string[] CommaText = null;
                    int CommaTextCount = 0;
                    int A = 0;
                    CommaText = INIProperty.Value.Split(',');
                    CommaTextCount = CommaText.GetUpperBound(0) + 1;
                    if ( CommaTextCount < 1 )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    for ( A = 0; A <= CommaTextCount - 1; A++ )
                    {
                        CommaText[A] = Convert.ToString(CommaText[A].Trim());
                    }
                    switch ( CommaText[0].ToLower() )
                    {
                        case "feature":
                            Objects[INISectionNum].Type = clsUnitType.enumType.Feature;
                            Objects[INISectionNum].Code = CommaText[1];
                            break;
                        case "structure":
                            Objects[INISectionNum].Type = clsUnitType.enumType.PlayerStructure;
                            Objects[INISectionNum].Code = CommaText[1];
                            break;
                        case "droidtemplate":
                            Objects[INISectionNum].Type = clsUnitType.enumType.PlayerDroid;
                            Objects[INISectionNum].IsTemplate = true;
                            Objects[INISectionNum].Code = CommaText[1];
                            break;
                        case "droiddesign":
                            Objects[INISectionNum].Type = clsUnitType.enumType.PlayerDroid;
                            break;
                        default:
                            return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "droidtype" )
                {
                    clsDroidDesign.clsTemplateDroidType DroidType = modProgram.GetTemplateDroidTypeFromTemplateCode(Convert.ToString(INIProperty.Value));
                    if ( DroidType == null )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Objects[INISectionNum].TemplateDroidType = DroidType;
                }
                else if ( (string)INIProperty.Name == "body" )
                {
                    Objects[INISectionNum].BodyCode = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "propulsion" )
                {
                    Objects[INISectionNum].PropulsionCode = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "turretcount" )
                {
                    int NewTurretCount = 0;
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref NewTurretCount) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( NewTurretCount < 0 | NewTurretCount > modProgram.MaxDroidWeapons )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Objects[INISectionNum].TurretCount = NewTurretCount;
                }
                else if ( (string)INIProperty.Name == "turret1" )
                {
                    string[] CommaText = null;
                    int CommaTextCount = 0;
                    int A = 0;
                    CommaText = INIProperty.Value.Split(',');
                    CommaTextCount = CommaText.GetUpperBound(0) + 1;
                    if ( CommaTextCount < 2 )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    for ( A = 0; A <= CommaTextCount - 1; A++ )
                    {
                        CommaText[A] = Convert.ToString(CommaText[A].Trim());
                    }
                    clsTurret.enumTurretType TurretType = default(clsTurret.enumTurretType);
                    TurretType = modProgram.GetTurretTypeFromName(CommaText[0]);
                    if ( TurretType != clsTurret.enumTurretType.Unknown )
                    {
                        Objects[INISectionNum].TurretTypes[0] = TurretType;
                        Objects[INISectionNum].TurretCodes[0] = CommaText[1];
                    }
                }
                else if ( (string)INIProperty.Name == "turret2" )
                {
                    string[] CommaText = null;
                    int CommaTextCount = 0;
                    int A = 0;
                    CommaText = INIProperty.Value.Split(',');
                    CommaTextCount = CommaText.GetUpperBound(0) + 1;
                    if ( CommaTextCount < 2 )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    for ( A = 0; A <= CommaTextCount - 1; A++ )
                    {
                        CommaText[A] = Convert.ToString(CommaText[A].Trim());
                    }
                    clsTurret.enumTurretType TurretType = default(clsTurret.enumTurretType);
                    TurretType = modProgram.GetTurretTypeFromName(CommaText[0]);
                    if ( TurretType != clsTurret.enumTurretType.Unknown )
                    {
                        Objects[INISectionNum].TurretTypes[1] = TurretType;
                        Objects[INISectionNum].TurretCodes[1] = CommaText[1];
                    }
                }
                else if ( (string)INIProperty.Name == "turret3" )
                {
                    string[] CommaText = null;
                    int CommaTextCount = 0;
                    int A = 0;
                    CommaText = INIProperty.Value.Split(',');
                    CommaTextCount = CommaText.GetUpperBound(0) + 1;
                    if ( CommaTextCount < 2 )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    for ( A = 0; A <= CommaTextCount - 1; A++ )
                    {
                        CommaText[A] = Convert.ToString(CommaText[A].Trim());
                    }
                    clsTurret.enumTurretType TurretType = default(clsTurret.enumTurretType);
                    TurretType = modProgram.GetTurretTypeFromName(CommaText[0]);
                    if ( TurretType != clsTurret.enumTurretType.Unknown )
                    {
                        Objects[INISectionNum].TurretTypes[2] = TurretType;
                        Objects[INISectionNum].TurretCodes[2] = CommaText[1];
                    }
                }
                else if ( (string)INIProperty.Name == "id" )
                {
                    if ( !modIO.InvariantParse_uint(Convert.ToString(INIProperty.Value), Objects[INISectionNum].ID) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "priority" )
                {
                    Int32 temp_Result = Objects[INISectionNum].Priority;
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref temp_Result) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "pos" )
                {
                    string[] CommaText = null;
                    int CommaTextCount = 0;
                    int A = 0;
                    CommaText = INIProperty.Value.Split(',');
                    CommaTextCount = CommaText.GetUpperBound(0) + 1;
                    if ( CommaTextCount < 2 )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    for ( A = 0; A <= CommaTextCount - 1; A++ )
                    {
                        CommaText[A] = Convert.ToString(CommaText[A].Trim());
                    }
                    sXY_int Pos = new sXY_int();
                    if ( !modIO.InvariantParse_int(CommaText[0], ref Pos.X) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !modIO.InvariantParse_int(CommaText[1], ref Pos.Y) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    try
                    {
                        Objects[INISectionNum].Pos = new clsXY_int(Pos);
                    }
                    catch ( Exception )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "heading" )
                {
                    double dblTemp = 0;
                    if ( !modIO.InvariantParse_dbl(Convert.ToString(INIProperty.Value), ref dblTemp) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( dblTemp < 0.0D | dblTemp >= 360.0D )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Objects[INISectionNum].Heading = dblTemp;
                }
                else if ( (string)INIProperty.Name == "unitgroup" )
                {
                    Objects[INISectionNum].UnitGroup = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "health" )
                {
                    double NewHealth = 0;
                    if ( !modIO.InvariantParse_dbl(Convert.ToString(INIProperty.Value), ref NewHealth) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( NewHealth < 0.0D | NewHealth >= 1.0D )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    Objects[INISectionNum].Health = NewHealth;
                }
                else if ( (string)INIProperty.Name == "walltype" )
                {
                    int WallType = -1;
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref WallType) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( WallType >= 0 & WallType <= 3 )
                    {
                        Objects[INISectionNum].WallType = WallType;
                    }
                }
                else if ( (string)INIProperty.Name == "scriptlabel" )
                {
                    Objects[INISectionNum].Label = Convert.ToString(INIProperty.Value);
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.NameUnknown;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }
        }

        public clsResult Write_FMap(string Path, bool Overwrite, bool Compress)
        {
            clsResult ReturnResult =
                new clsResult("Writing FMap to " + Convert.ToString(ControlChars.Quote) + Path + Convert.ToString(ControlChars.Quote));

            if ( !Overwrite )
            {
                if ( File.Exists(Path) )
                {
                    ReturnResult.ProblemAdd("The file already exists");
                    return ReturnResult;
                }
            }

            FileStream FileStream = default(FileStream);
            try
            {
                FileStream = File.Create(Path);
            }
            catch ( Exception )
            {
                ReturnResult.ProblemAdd("Unable to create file");
                return ReturnResult;
            }

            ZipOutputStream WZStream = new ZipOutputStream(FileStream);
            WZStream.UseZip64 = UseZip64.Off;
            if ( Compress )
            {
                WZStream.SetLevel(9);
            }
            else
            {
                WZStream.SetLevel(0);
            }

            BinaryWriter BinaryWriter = new BinaryWriter(WZStream, modProgram.UTF8Encoding);
            StreamWriter StreamWriter = new StreamWriter(WZStream, modProgram.UTF8Encoding);
            ZipEntry ZipEntry = default(ZipEntry);
            string ZipPath = "";

            ZipPath = "Info.ini";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                clsINIWrite INI_Info = new clsINIWrite();
                INI_Info.File = StreamWriter;
                ReturnResult.Add(Serialize_FMap_Info(INI_Info));

                StreamWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "VertexHeight.dat";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                ReturnResult.Add(Serialize_FMap_VertexHeight(BinaryWriter));

                BinaryWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "VertexTerrain.dat";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                ReturnResult.Add(Serialize_FMap_VertexTerrain(BinaryWriter));

                BinaryWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "TileTexture.dat";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                ReturnResult.Add(Serialize_FMap_TileTexture(BinaryWriter));

                BinaryWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "TileOrientation.dat";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                ReturnResult.Add(Serialize_FMap_TileOrientation(BinaryWriter));

                BinaryWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "TileCliff.dat";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                ReturnResult.Add(Serialize_FMap_TileCliff(BinaryWriter));

                BinaryWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "Roads.dat";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                ReturnResult.Add(Serialize_FMap_Roads(BinaryWriter));

                BinaryWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "Objects.ini";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                clsINIWrite INI_Objects = new clsINIWrite();
                INI_Objects.File = StreamWriter;
                ReturnResult.Add(Serialize_FMap_Objects(INI_Objects));

                StreamWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "Gateways.ini";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                clsINIWrite INI_Gateways = new clsINIWrite();
                INI_Gateways.File = StreamWriter;
                ReturnResult.Add(Serialize_FMap_Gateways(INI_Gateways));

                StreamWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "TileTypes.dat";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                ReturnResult.Add(Serialize_FMap_TileTypes(BinaryWriter));

                BinaryWriter.Flush();
                WZStream.CloseEntry();
            }

            ZipPath = "ScriptLabels.ini";
            ZipEntry = modIO.ZipMakeEntry(WZStream, ZipPath, ReturnResult);
            if ( ZipEntry != null )
            {
                clsINIWrite INI_ScriptLabels = new clsINIWrite();
                INI_ScriptLabels.File = StreamWriter;
                ReturnResult.Add(Serialize_WZ_LabelsINI(INI_ScriptLabels, -1));

                StreamWriter.Flush();
                WZStream.CloseEntry();
            }

            WZStream.Finish();
            WZStream.Close();
            BinaryWriter.Close();
            return ReturnResult;
        }

        public clsResult Serialize_FMap_Info(clsINIWrite File)
        {
            clsResult ReturnResult = new clsResult("Serializing general map info");

            try
            {
                if ( Tileset == null )
                {
                }
                else if ( Tileset == modProgram.Tileset_Arizona )
                {
                    File.Property_Append("Tileset", "Arizona");
                }
                else if ( Tileset == modProgram.Tileset_Urban )
                {
                    File.Property_Append("Tileset", "Urban");
                }
                else if ( Tileset == modProgram.Tileset_Rockies )
                {
                    File.Property_Append("Tileset", "Rockies");
                }

                File.Property_Append("Size", modIO.InvariantToString_int(Terrain.TileSize.X) + ", " + modIO.InvariantToString_int(Terrain.TileSize.Y));

                File.Property_Append("AutoScrollLimits", modIO.InvariantToString_bool(InterfaceOptions.AutoScrollLimits));
                File.Property_Append("ScrollMinX", modIO.InvariantToString_int(InterfaceOptions.ScrollMin.X));
                File.Property_Append("ScrollMinY", modIO.InvariantToString_int(InterfaceOptions.ScrollMin.Y));
                File.Property_Append("ScrollMaxX", modIO.InvariantToString_uint(InterfaceOptions.ScrollMax.X));
                File.Property_Append("ScrollMaxY", modIO.InvariantToString_uint(InterfaceOptions.ScrollMax.Y));

                File.Property_Append("Name", InterfaceOptions.CompileName);
                File.Property_Append("Players", InterfaceOptions.CompileMultiPlayers);
                File.Property_Append("XPlayerLev", modIO.InvariantToString_bool(InterfaceOptions.CompileMultiXPlayers));
                File.Property_Append("Author", InterfaceOptions.CompileMultiAuthor);
                File.Property_Append("License", InterfaceOptions.CompileMultiLicense);
                if ( InterfaceOptions.CampaignGameType >= 0 )
                {
                    File.Property_Append("CampType", modIO.InvariantToString_int(InterfaceOptions.CampaignGameType));
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            return ReturnResult;
        }

        public clsResult Serialize_FMap_VertexHeight(BinaryWriter File)
        {
            clsResult ReturnResult = new clsResult("Serializing vertex heights");
            int X = 0;
            int Y = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X; X++ )
                    {
                        File.Write(Terrain.Vertices[X, Y].Height);
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            return ReturnResult;
        }

        public clsResult Serialize_FMap_VertexTerrain(BinaryWriter File)
        {
            clsResult ReturnResult = new clsResult("Serializing vertex terrain");

            int X = 0;
            int Y = 0;
            int ErrorCount = 0;
            int Value = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X; X++ )
                    {
                        if ( Terrain.Vertices[X, Y].Terrain == null )
                        {
                            Value = 0;
                        }
                        else if ( Terrain.Vertices[X, Y].Terrain.Num < 0 )
                        {
                            ErrorCount++;
                            Value = 0;
                        }
                        else
                        {
                            Value = Convert.ToInt32(Terrain.Vertices[X, Y].Terrain.Num + 1);
                            if ( Value > 255 )
                            {
                                ErrorCount++;
                                Value = 0;
                            }
                        }
                        File.Write((byte)Value);
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            if ( ErrorCount > 0 )
            {
                ReturnResult.WarningAdd(ErrorCount + " vertices had an invalid painted terrain number.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_FMap_TileTexture(BinaryWriter File)
        {
            clsResult ReturnResult = new clsResult("Serializing tile textures");

            int X = 0;
            int Y = 0;
            int ErrorCount = 0;
            int Value = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        Value = Convert.ToInt32(Terrain.Tiles[X, Y].Texture.TextureNum + 1);
                        if ( Value < 0 | Value > 255 )
                        {
                            ErrorCount++;
                            Value = 0;
                        }
                        File.Write((byte)Value);
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            if ( ErrorCount > 0 )
            {
                ReturnResult.WarningAdd(ErrorCount + " tiles had an invalid texture number.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_FMap_TileOrientation(BinaryWriter File)
        {
            clsResult ReturnResult = new clsResult("Serializing tile orientations");
            int X = 0;
            int Y = 0;
            int Value = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        Value = 0;
                        if ( Terrain.Tiles[X, Y].Texture.Orientation.SwitchedAxes )
                        {
                            Value += 8;
                        }
                        if ( Terrain.Tiles[X, Y].Texture.Orientation.ResultXFlip )
                        {
                            Value += 4;
                        }
                        if ( Terrain.Tiles[X, Y].Texture.Orientation.ResultYFlip )
                        {
                            Value += 2;
                        }
                        if ( Terrain.Tiles[X, Y].Tri )
                        {
                            Value++;
                        }
                        File.Write((byte)Value);
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            return ReturnResult;
        }

        public clsResult Serialize_FMap_TileCliff(BinaryWriter File)
        {
            clsResult ReturnResult = new clsResult("Serializing tile cliffs");

            int X = 0;
            int Y = 0;
            int Value = 0;
            int DownSideValue = 0;
            int ErrorCount = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        Value = 0;
                        if ( Terrain.Tiles[X, Y].Tri )
                        {
                            if ( Terrain.Tiles[X, Y].TriTopLeftIsCliff )
                            {
                                Value += 2;
                            }
                            if ( Terrain.Tiles[X, Y].TriBottomRightIsCliff )
                            {
                                Value++;
                            }
                        }
                        else
                        {
                            if ( Terrain.Tiles[X, Y].TriBottomLeftIsCliff )
                            {
                                Value += 2;
                            }
                            if ( Terrain.Tiles[X, Y].TriTopRightIsCliff )
                            {
                                Value++;
                            }
                        }
                        if ( Terrain.Tiles[X, Y].Terrain_IsCliff )
                        {
                            Value += 4;
                        }
                        if ( TileOrientation.IdenticalTileDirections(Terrain.Tiles[X, Y].DownSide, TileOrientation.TileDirection_None) )
                        {
                            DownSideValue = 0;
                        }
                        else if ( TileOrientation.IdenticalTileDirections(Terrain.Tiles[X, Y].DownSide, TileOrientation.TileDirection_Top) )
                        {
                            DownSideValue = 1;
                        }
                        else if ( TileOrientation.IdenticalTileDirections(Terrain.Tiles[X, Y].DownSide, TileOrientation.TileDirection_Left) )
                        {
                            DownSideValue = 2;
                        }
                        else if ( TileOrientation.IdenticalTileDirections(Terrain.Tiles[X, Y].DownSide, TileOrientation.TileDirection_Right) )
                        {
                            DownSideValue = 3;
                        }
                        else if ( TileOrientation.IdenticalTileDirections(Terrain.Tiles[X, Y].DownSide, TileOrientation.TileDirection_Bottom) )
                        {
                            DownSideValue = 4;
                        }
                        else
                        {
                            ErrorCount++;
                            DownSideValue = 0;
                        }
                        Value += DownSideValue * 8;
                        File.Write((byte)Value);
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            if ( ErrorCount > 0 )
            {
                ReturnResult.WarningAdd(ErrorCount + " tiles had an invalid cliff down side.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_FMap_Roads(BinaryWriter File)
        {
            clsResult ReturnResult = new clsResult("Serializing roads");

            int X = 0;
            int Y = 0;
            int Value = 0;
            int ErrorCount = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        if ( Terrain.SideH[X, Y].Road == null )
                        {
                            Value = 0;
                        }
                        else if ( Terrain.SideH[X, Y].Road.Num < 0 )
                        {
                            ErrorCount++;
                            Value = 0;
                        }
                        else
                        {
                            Value = Convert.ToInt32(Terrain.SideH[X, Y].Road.Num + 1);
                            if ( Value > 255 )
                            {
                                ErrorCount++;
                                Value = 0;
                            }
                        }
                        File.Write((byte)Value);
                    }
                }
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X; X++ )
                    {
                        if ( Terrain.SideV[X, Y].Road == null )
                        {
                            Value = 0;
                        }
                        else if ( Terrain.SideV[X, Y].Road.Num < 0 )
                        {
                            ErrorCount++;
                            Value = 0;
                        }
                        else
                        {
                            Value = Convert.ToInt32(Terrain.SideV[X, Y].Road.Num + 1);
                            if ( Value > 255 )
                            {
                                ErrorCount++;
                                Value = 0;
                            }
                        }
                        File.Write((byte)Value);
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            if ( ErrorCount > 0 )
            {
                ReturnResult.WarningAdd(ErrorCount + " sides had an invalid road number.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_FMap_Objects(clsINIWrite File)
        {
            clsResult ReturnResult = new clsResult("Serializing objects");

            int A = 0;
            clsUnit Unit = default(clsUnit);
            clsDroidDesign Droid = default(clsDroidDesign);
            int WarningCount = 0;
            string Text = null;

            try
            {
                for ( A = 0; A <= Units.Count - 1; A++ )
                {
                    Unit = Units[A];
                    File.SectionName_Append(modIO.InvariantToString_int(A));
                    switch ( Unit.Type.Type )
                    {
                        case clsUnitType.enumType.Feature:
                            File.Property_Append("Type", "Feature, " + ((clsFeatureType)Unit.Type).Code);
                            break;
                        case clsUnitType.enumType.PlayerStructure:
                            clsStructureType StructureType = (clsStructureType)Unit.Type;
                            File.Property_Append("Type", "Structure, " + StructureType.Code);
                            if ( StructureType.WallLink.IsConnected )
                            {
                                File.Property_Append("WallType", modIO.InvariantToString_int(StructureType.WallLink.ArrayPosition));
                            }
                            break;
                        case clsUnitType.enumType.PlayerDroid:
                            Droid = (clsDroidDesign)Unit.Type;
                            if ( Droid.IsTemplate )
                            {
                                File.Property_Append("Type", "DroidTemplate, " + ((clsDroidTemplate)Unit.Type).Code);
                            }
                            else
                            {
                                File.Property_Append("Type", "DroidDesign");
                                if ( Droid.TemplateDroidType != null )
                                {
                                    File.Property_Append("DroidType", Droid.TemplateDroidType.TemplateCode);
                                }
                                if ( Droid.Body != null )
                                {
                                    File.Property_Append("Body", Droid.Body.Code);
                                }
                                if ( Droid.Propulsion != null )
                                {
                                    File.Property_Append("Propulsion", Droid.Propulsion.Code);
                                }
                                File.Property_Append("TurretCount", modIO.InvariantToString_byte(Droid.TurretCount));
                                if ( Droid.Turret1 != null )
                                {
                                    if ( Droid.Turret1.GetTurretTypeName(ref Text) )
                                    {
                                        File.Property_Append("Turret1", Text + ", " + Droid.Turret1.Code);
                                    }
                                }
                                if ( Droid.Turret2 != null )
                                {
                                    if ( Droid.Turret2.GetTurretTypeName(ref Text) )
                                    {
                                        File.Property_Append("Turret2", Text + ", " + Droid.Turret2.Code);
                                    }
                                }
                                if ( Droid.Turret3 != null )
                                {
                                    if ( Droid.Turret3.GetTurretTypeName(ref Text) )
                                    {
                                        File.Property_Append("Turret3", Text + ", " + Droid.Turret3.Code);
                                    }
                                }
                            }
                            break;
                        default:
                            WarningCount++;
                            break;
                    }
                    File.Property_Append("ID", modIO.InvariantToString_uint(Unit.ID));
                    File.Property_Append("Priority", modIO.InvariantToString_int(Unit.SavePriority));
                    File.Property_Append("Pos", modIO.InvariantToString_int(Unit.Pos.Horizontal.X) + ", " + modIO.InvariantToString_int(Unit.Pos.Horizontal.Y));
                    File.Property_Append("Heading", modIO.InvariantToString_int(Unit.Rotation));
                    File.Property_Append("UnitGroup", Unit.UnitGroup.GetFMapINIPlayerText());
                    if ( Unit.Health < 1.0D )
                    {
                        File.Property_Append("Health", modIO.InvariantToString_dbl(Unit.Health));
                    }
                    if ( Unit.Label != null )
                    {
                        File.Property_Append("ScriptLabel", Unit.Label);
                    }
                    File.Gap_Append();
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            if ( WarningCount > 0 )
            {
                ReturnResult.WarningAdd("Error: " + Convert.ToString(WarningCount) + " units were of an unhandled type.");
            }

            return ReturnResult;
        }

        public clsResult Serialize_FMap_Gateways(clsINIWrite File)
        {
            clsResult ReturnResult = new clsResult("Serializing gateways");
            int A = 0;
            clsGateway Gateway = default(clsGateway);

            try
            {
                for ( A = 0; A <= Gateways.Count - 1; A++ )
                {
                    Gateway = Gateways[A];
                    File.SectionName_Append(modIO.InvariantToString_int(A));
                    File.Property_Append("AX", modIO.InvariantToString_int(Gateway.PosA.X));
                    File.Property_Append("AY", modIO.InvariantToString_int(Gateway.PosA.Y));
                    File.Property_Append("BX", modIO.InvariantToString_int(Gateway.PosB.X));
                    File.Property_Append("BY", modIO.InvariantToString_int(Gateway.PosB.Y));
                    File.Gap_Append();
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            return ReturnResult;
        }

        public clsResult Serialize_FMap_TileTypes(BinaryWriter File)
        {
            clsResult ReturnResult = new clsResult("Serializing tile types");
            int A = 0;

            try
            {
                if ( Tileset != null )
                {
                    for ( A = 0; A <= Tileset.TileCount - 1; A++ )
                    {
                        File.Write(Tile_TypeNum[A]);
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            return ReturnResult;
        }

        public clsResult Load_FMap(string Path)
        {
            clsResult ReturnResult =
                new clsResult("Loading FMap from " + Convert.ToString(ControlChars.Quote) + Path + Convert.ToString(ControlChars.Quote));

            clsZipStreamEntry ZipSearchResult = default(clsZipStreamEntry);
            string FindPath = "";

            clsFMapInfo ResultInfo = null;

            FindPath = "info.ini";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.ProblemAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
                return ReturnResult;
            }
            else
            {
                StreamReader Info_StreamReader = new StreamReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_Info(Info_StreamReader, ref ResultInfo));
                Info_StreamReader.Close();
                if ( ReturnResult.HasProblems )
                {
                    return ReturnResult;
                }
            }

            sXY_int NewTerrainSize = ResultInfo.TerrainSize;
            Tileset = ResultInfo.Tileset;

            if ( NewTerrainSize.X <= 0 | NewTerrainSize.X > modProgram.MapMaxSize )
            {
                ReturnResult.ProblemAdd("Map width of " + Convert.ToString(NewTerrainSize.X) + " is not valid.");
            }
            if ( NewTerrainSize.Y <= 0 | NewTerrainSize.Y > modProgram.MapMaxSize )
            {
                ReturnResult.ProblemAdd("Map height of " + Convert.ToString(NewTerrainSize.Y) + " is not valid.");
            }
            if ( ReturnResult.HasProblems )
            {
                return ReturnResult;
            }

            SetPainterToDefaults(); //depends on tileset. must be called before loading the terrains.
            TerrainBlank(NewTerrainSize);
            TileType_Reset();

            FindPath = "vertexheight.dat";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.WarningAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
            }
            else
            {
                BinaryReader VertexHeight_Reader = new BinaryReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_VertexHeight(VertexHeight_Reader));
                VertexHeight_Reader.Close();
            }

            FindPath = "vertexterrain.dat";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.WarningAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
            }
            else
            {
                BinaryReader VertexTerrain_Reader = new BinaryReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_VertexTerrain(VertexTerrain_Reader));
                VertexTerrain_Reader.Close();
            }

            FindPath = "tiletexture.dat";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.WarningAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
            }
            else
            {
                BinaryReader TileTexture_Reader = new BinaryReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_TileTexture(TileTexture_Reader));
                TileTexture_Reader.Close();
            }

            FindPath = "tileorientation.dat";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.WarningAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
            }
            else
            {
                BinaryReader TileOrientation_Reader = new BinaryReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_TileOrientation(TileOrientation_Reader));
                TileOrientation_Reader.Close();
            }

            FindPath = "tilecliff.dat";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.WarningAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
            }
            else
            {
                BinaryReader TileCliff_Reader = new BinaryReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_TileCliff(TileCliff_Reader));
                TileCliff_Reader.Close();
            }

            FindPath = "roads.dat";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.WarningAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
            }
            else
            {
                BinaryReader Roads_Reader = new BinaryReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_Roads(Roads_Reader));
                Roads_Reader.Close();
            }

            FindPath = "objects.ini";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.WarningAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
            }
            else
            {
                StreamReader Objects_Reader = new StreamReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_Objects(Objects_Reader));
                Objects_Reader.Close();
            }

            FindPath = "gateways.ini";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.WarningAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
            }
            else
            {
                StreamReader Gateway_Reader = new StreamReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_Gateways(Gateway_Reader));
                Gateway_Reader.Close();
            }

            FindPath = "tiletypes.dat";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
                ReturnResult.WarningAdd("Unable to find file " + Convert.ToString(ControlChars.Quote) + FindPath + Convert.ToString(ControlChars.Quote) +
                                        ".");
            }
            else
            {
                BinaryReader TileTypes_Reader = new BinaryReader(ZipSearchResult.Stream);
                ReturnResult.Add(Read_FMap_TileTypes(TileTypes_Reader));
                TileTypes_Reader.Close();
            }

            FindPath = "scriptlabels.ini";
            ZipSearchResult = modIO.FindZipEntryFromPath(Path, FindPath);
            if ( ZipSearchResult == null )
            {
            }
            else
            {
                clsResult Result = new clsResult("Reading labels");
                clsINIRead LabelsINI = new clsINIRead();
                StreamReader LabelsINI_Reader = new StreamReader(ZipSearchResult.Stream);
                Result.Take(LabelsINI.ReadFile(LabelsINI_Reader));
                LabelsINI_Reader.Close();
                Result.Take(Read_WZ_Labels(LabelsINI, true));
                ReturnResult.Add(Result);
            }

            InterfaceOptions = ResultInfo.InterfaceOptions;

            return ReturnResult;
        }

        public class clsFMapInfo : clsINIRead.clsTranslator
        {
            public sXY_int TerrainSize = new sXY_int(-1, -1);
            public clsInterfaceOptions InterfaceOptions = new clsInterfaceOptions();
            public clsTileset Tileset;

            public override clsINIRead.enumTranslatorResult Translate(clsINIRead.clsSection.sProperty INIProperty)
            {
                if ( INIProperty.Name == "tileset" )
                {
                    if ( INIProperty.Value.ToLower() == "arizona" )
                    {
                        Tileset = modProgram.Tileset_Arizona;
                    }
                    else if ( INIProperty.Value.ToLower() == "urban" )
                    {
                        Tileset = modProgram.Tileset_Urban;
                    }
                    else if ( INIProperty.Value.ToLower() == "rockies" )
                    {
                        Tileset = modProgram.Tileset_Rockies;
                    }
                    else
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "size" )
                {
                    string[] CommaText = INIProperty.Value.Split(',');
                    if ( CommaText.GetUpperBound(0) + 1 < 2 )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    int A = 0;
                    for ( A = 0; A <= CommaText.GetUpperBound(0); A++ )
                    {
                        CommaText[A] = CommaText[A].Trim();
                    }
                    sXY_int NewSize = new sXY_int();
                    if ( !modIO.InvariantParse_int(CommaText[0], ref NewSize.X) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !modIO.InvariantParse_int(CommaText[1], ref NewSize.Y) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( NewSize.X < 1 | NewSize.Y < 1 | NewSize.X > modProgram.MapMaxSize | NewSize.Y > modProgram.MapMaxSize )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    TerrainSize = NewSize;
                }
                else if ( (string)INIProperty.Name == "autoscrolllimits" )
                {
                    if ( !modIO.InvariantParse_bool(Convert.ToString(INIProperty.Value), ref InterfaceOptions.AutoScrollLimits) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "scrollminx" )
                {
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref InterfaceOptions.ScrollMin.X) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "scrollminy" )
                {
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref InterfaceOptions.ScrollMin.Y) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "scrollmaxx" )
                {
                    if ( !modIO.InvariantParse_uint(Convert.ToString(INIProperty.Value), InterfaceOptions.ScrollMax.X) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "scrollmaxy" )
                {
                    if ( !modIO.InvariantParse_uint(Convert.ToString(INIProperty.Value), InterfaceOptions.ScrollMax.Y) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "name" )
                {
                    InterfaceOptions.CompileName = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "players" )
                {
                    InterfaceOptions.CompileMultiPlayers = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "xplayerlev" )
                {
                    if ( !modIO.InvariantParse_bool(Convert.ToString(INIProperty.Value), ref InterfaceOptions.CompileMultiXPlayers) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "author" )
                {
                    InterfaceOptions.CompileMultiAuthor = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "license" )
                {
                    InterfaceOptions.CompileMultiLicense = Convert.ToString(INIProperty.Value);
                }
                else if ( (string)INIProperty.Name == "camptime" )
                {
                    //allow and ignore
                }
                else if ( (string)INIProperty.Name == "camptype" )
                {
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref InterfaceOptions.CampaignGameType) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.NameUnknown;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }
        }

        private clsResult Read_FMap_Info(StreamReader File, ref clsFMapInfo ResultInfo)
        {
            clsResult ReturnResult = new clsResult("Read general map info");

            clsINIRead.clsSection InfoINI = new clsINIRead.clsSection();
            ReturnResult.Take(InfoINI.ReadFile(File));

            ResultInfo = new clsFMapInfo();
            ReturnResult.Take(InfoINI.Translate(ResultInfo));

            if ( ResultInfo.TerrainSize.X < 0 | ResultInfo.TerrainSize.Y < 0 )
            {
                ReturnResult.ProblemAdd("Map size was not specified or was invalid.");
            }

            return ReturnResult;
        }

        private clsResult Read_FMap_VertexHeight(BinaryReader File)
        {
            clsResult ReturnResult = new clsResult("Reading vertex heights");

            int X = 0;
            int Y = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X; X++ )
                    {
                        Terrain.Vertices[X, Y].Height = File.ReadByte();
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            if ( File.PeekChar() >= 0 )
            {
                ReturnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return ReturnResult;
        }

        private clsResult Read_FMap_VertexTerrain(BinaryReader File)
        {
            clsResult ReturnResult = new clsResult("Reading vertex terrain");

            int X = 0;
            int Y = 0;
            int Value = 0;
            byte byteTemp = 0;
            int WarningCount = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X; X++ )
                    {
                        byteTemp = File.ReadByte();
                        Value = (byteTemp) - 1;
                        if ( Value < 0 )
                        {
                            Terrain.Vertices[X, Y].Terrain = null;
                        }
                        else if ( Value >= Painter.TerrainCount )
                        {
                            if ( WarningCount < 16 )
                            {
                                ReturnResult.WarningAdd("Painted terrain at vertex " + Convert.ToString(X) + ", " + Convert.ToString(Y) +
                                                        " was invalid.");
                            }
                            WarningCount++;
                            Terrain.Vertices[X, Y].Terrain = null;
                        }
                        else
                        {
                            Terrain.Vertices[X, Y].Terrain = Painter.Terrains[Value];
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            if ( WarningCount > 0 )
            {
                ReturnResult.WarningAdd(WarningCount + " painted terrain vertices were invalid.");
            }

            if ( File.PeekChar() >= 0 )
            {
                ReturnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return ReturnResult;
        }

        public clsResult Read_FMap_TileTexture(BinaryReader File)
        {
            clsResult ReturnResult = new clsResult("Reading tile textures");

            int X = 0;
            int Y = 0;
            byte byteTemp = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        byteTemp = File.ReadByte();
                        Terrain.Tiles[X, Y].Texture.TextureNum = (byteTemp) - 1;
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            if ( File.PeekChar() >= 0 )
            {
                ReturnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return ReturnResult;
        }

        public clsResult Read_FMap_TileOrientation(BinaryReader File)
        {
            clsResult ReturnResult = new clsResult("Reading tile orientations");

            int X = 0;
            int Y = 0;
            int Value = 0;
            int PartValue = 0;
            int WarningCount = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        Value = File.ReadByte();

                        PartValue = (int)(Math.Floor((double)(Value / 16)));
                        if ( PartValue > 0 )
                        {
                            if ( WarningCount < 16 )
                            {
                                ReturnResult.WarningAdd("Unknown bits used for tile " + Convert.ToString(X) + ", " + Convert.ToString(Y) + ".");
                            }
                            WarningCount++;
                        }
                        Value -= PartValue * 16;

                        PartValue = (int)(Conversion.Int(Value / 8.0D));
                        Terrain.Tiles[X, Y].Texture.Orientation.SwitchedAxes = PartValue > 0;
                        Value -= PartValue * 8;

                        PartValue = (int)(Conversion.Int(Value / 4.0D));
                        Terrain.Tiles[X, Y].Texture.Orientation.ResultXFlip = PartValue > 0;
                        Value -= PartValue * 4;

                        PartValue = (int)(Conversion.Int(Value / 2.0D));
                        Terrain.Tiles[X, Y].Texture.Orientation.ResultYFlip = PartValue > 0;
                        Value -= PartValue * 2;

                        PartValue = Value;
                        Terrain.Tiles[X, Y].Tri = PartValue > 0;
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            if ( WarningCount > 0 )
            {
                ReturnResult.WarningAdd(WarningCount + " tiles had unknown bits used.");
            }

            if ( File.PeekChar() >= 0 )
            {
                ReturnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return ReturnResult;
        }

        public clsResult Read_FMap_TileCliff(BinaryReader File)
        {
            clsResult ReturnResult = new clsResult("Reading tile cliffs");

            int X = 0;
            int Y = 0;
            int Value = 0;
            int PartValue = 0;
            int DownSideWarningCount = 0;
            int WarningCount = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        Value = File.ReadByte();

                        PartValue = (int)(Conversion.Int(Value / 64.0D));
                        if ( PartValue > 0 )
                        {
                            if ( WarningCount < 16 )
                            {
                                ReturnResult.WarningAdd("Unknown bits used for tile " + Convert.ToString(X) + ", " + Convert.ToString(Y) + ".");
                            }
                            WarningCount++;
                        }
                        Value -= PartValue * 64;

                        PartValue = (int)(Conversion.Int(Value / 8.0D));
                        switch ( PartValue )
                        {
                            case 0:
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_None;
                                break;
                            case 1:
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_Top;
                                break;
                            case 2:
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_Left;
                                break;
                            case 3:
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_Right;
                                break;
                            case 4:
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_Bottom;
                                break;
                            default:
                                Terrain.Tiles[X, Y].DownSide = TileOrientation.TileDirection_None;
                                if ( DownSideWarningCount < 16 )
                                {
                                    ReturnResult.WarningAdd("Down side value for tile " + Convert.ToString(X) + ", " + Convert.ToString(Y) +
                                                            " was invalid.");
                                }
                                DownSideWarningCount++;
                                break;
                        }
                        Value -= PartValue * 8;

                        PartValue = (int)(Conversion.Int(Value / 4.0D));
                        Terrain.Tiles[X, Y].Terrain_IsCliff = PartValue > 0;
                        Value -= PartValue * 4;

                        PartValue = (int)(Conversion.Int(Value / 2.0D));
                        if ( Terrain.Tiles[X, Y].Tri )
                        {
                            Terrain.Tiles[X, Y].TriTopLeftIsCliff = PartValue > 0;
                        }
                        else
                        {
                            Terrain.Tiles[X, Y].TriBottomLeftIsCliff = PartValue > 0;
                        }
                        Value -= PartValue * 2;

                        PartValue = Value;
                        if ( Terrain.Tiles[X, Y].Tri )
                        {
                            Terrain.Tiles[X, Y].TriBottomRightIsCliff = PartValue > 0;
                        }
                        else
                        {
                            Terrain.Tiles[X, Y].TriTopRightIsCliff = PartValue > 0;
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            if ( WarningCount > 0 )
            {
                ReturnResult.WarningAdd(WarningCount + " tiles had unknown bits used.");
            }
            if ( DownSideWarningCount > 0 )
            {
                ReturnResult.WarningAdd(DownSideWarningCount + " tiles had invalid down side values.");
            }

            if ( File.PeekChar() >= 0 )
            {
                ReturnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return ReturnResult;
        }

        public clsResult Read_FMap_Roads(BinaryReader File)
        {
            clsResult ReturnResult = new clsResult("Reading roads");

            int X = 0;
            int Y = 0;
            int Value = 0;
            int WarningCount = 0;

            try
            {
                for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                    {
                        Value = File.ReadByte() - 1;
                        if ( Value < 0 )
                        {
                            Terrain.SideH[X, Y].Road = null;
                        }
                        else if ( Value >= Painter.RoadCount )
                        {
                            if ( WarningCount < 16 )
                            {
                                ReturnResult.WarningAdd("Invalid road value for horizontal side " + Convert.ToString(X) + ", " + Convert.ToString(Y) +
                                                        ".");
                            }
                            WarningCount++;
                            Terrain.SideH[X, Y].Road = null;
                        }
                        else
                        {
                            Terrain.SideH[X, Y].Road = Painter.Roads[Value];
                        }
                    }
                }
                for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Terrain.TileSize.X; X++ )
                    {
                        Value = File.ReadByte() - 1;
                        if ( Value < 0 )
                        {
                            Terrain.SideV[X, Y].Road = null;
                        }
                        else if ( Value >= Painter.RoadCount )
                        {
                            if ( WarningCount < 16 )
                            {
                                ReturnResult.WarningAdd("Invalid road value for vertical side " + Convert.ToString(X) + ", " + Convert.ToString(Y) +
                                                        ".");
                            }
                            WarningCount++;
                            Terrain.SideV[X, Y].Road = null;
                        }
                        else
                        {
                            Terrain.SideV[X, Y].Road = Painter.Roads[Value];
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            if ( WarningCount > 0 )
            {
                ReturnResult.WarningAdd(WarningCount + " sides had an invalid road value.");
            }

            if ( File.PeekChar() >= 0 )
            {
                ReturnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return ReturnResult;
        }

        private clsResult Read_FMap_Objects(StreamReader File)
        {
            clsResult ReturnResult = new clsResult("Reading objects");

            int A = 0;

            clsINIRead ObjectsINI = new clsINIRead();
            ReturnResult.Take(ObjectsINI.ReadFile(File));

            clsFMap_INIObjects INIObjects = new clsFMap_INIObjects(ObjectsINI.Sections.Count);
            ReturnResult.Take(ObjectsINI.Translate(INIObjects));

            int DroidComponentUnknownCount = 0;
            int ObjectTypeMissingCount = 0;
            int ObjectPlayerNumInvalidCount = 0;
            int ObjectPosInvalidCount = 0;
            int DesignTypeUnspecifiedCount = 0;
            int UnknownUnitTypeCount = 0;
            int MaxUnknownUnitTypeWarningCount = 16;

            clsDroidDesign DroidDesign = default(clsDroidDesign);
            clsUnit NewObject = default(clsUnit);
            clsUnitAdd UnitAdd = new clsUnitAdd();
            clsUnitType UnitType = default(clsUnitType);
            bool IsDesign = default(bool);
            clsUnitGroup UnitGroup = default(clsUnitGroup);
            sXY_int ZeroPos = new sXY_int(0, 0);
            UInt32 AvailableID = 0;

            UnitAdd.Map = this;

            AvailableID = 1U;
            for ( A = 0; A <= INIObjects.ObjectCount - 1; A++ )
            {
                if ( INIObjects.Objects[A].ID >= AvailableID )
                {
                    AvailableID = INIObjects.Objects[A].ID + 1U;
                }
            }
            for ( A = 0; A <= INIObjects.ObjectCount - 1; A++ )
            {
                if ( INIObjects.Objects[A].Pos == null )
                {
                    ObjectPosInvalidCount++;
                }
                else if ( !modProgram.PosIsWithinTileArea(INIObjects.Objects[A].Pos.XY, ZeroPos, Terrain.TileSize) )
                {
                    ObjectPosInvalidCount++;
                }
                else
                {
                    UnitType = null;
                    if ( INIObjects.Objects[A].Type != clsUnitType.enumType.Unspecified )
                    {
                        IsDesign = false;
                        if ( INIObjects.Objects[A].Type == clsUnitType.enumType.PlayerDroid )
                        {
                            if ( !INIObjects.Objects[A].IsTemplate )
                            {
                                IsDesign = true;
                            }
                        }
                        if ( IsDesign )
                        {
                            DroidDesign = new clsDroidDesign();
                            DroidDesign.TemplateDroidType = INIObjects.Objects[A].TemplateDroidType;
                            if ( DroidDesign.TemplateDroidType == null )
                            {
                                DroidDesign.TemplateDroidType = modProgram.TemplateDroidType_Droid;
                                DesignTypeUnspecifiedCount++;
                            }
                            if ( INIObjects.Objects[A].BodyCode != "" )
                            {
                                DroidDesign.Body = modProgram.ObjectData.FindOrCreateBody(Convert.ToString(INIObjects.Objects[A].BodyCode));
                                if ( DroidDesign.Body.IsUnknown )
                                {
                                    DroidComponentUnknownCount++;
                                }
                            }
                            if ( INIObjects.Objects[A].PropulsionCode != "" )
                            {
                                DroidDesign.Propulsion = modProgram.ObjectData.FindOrCreatePropulsion(INIObjects.Objects[A].PropulsionCode);
                                if ( DroidDesign.Propulsion.IsUnknown )
                                {
                                    DroidComponentUnknownCount++;
                                }
                            }
                            DroidDesign.TurretCount = (byte)(INIObjects.Objects[A].TurretCount);
                            if ( INIObjects.Objects[A].TurretCodes[0] != "" )
                            {
                                DroidDesign.Turret1 = modProgram.ObjectData.FindOrCreateTurret(INIObjects.Objects[A].TurretTypes[0],
                                    Convert.ToString(INIObjects.Objects[A].TurretCodes[0]));
                                if ( DroidDesign.Turret1.IsUnknown )
                                {
                                    DroidComponentUnknownCount++;
                                }
                            }
                            if ( INIObjects.Objects[A].TurretCodes[1] != "" )
                            {
                                DroidDesign.Turret2 = modProgram.ObjectData.FindOrCreateTurret(INIObjects.Objects[A].TurretTypes[1],
                                    Convert.ToString(INIObjects.Objects[A].TurretCodes[1]));
                                if ( DroidDesign.Turret2.IsUnknown )
                                {
                                    DroidComponentUnknownCount++;
                                }
                            }
                            if ( INIObjects.Objects[A].TurretCodes[2] != "" )
                            {
                                DroidDesign.Turret3 = modProgram.ObjectData.FindOrCreateTurret(INIObjects.Objects[A].TurretTypes[2],
                                    Convert.ToString(INIObjects.Objects[A].TurretCodes[2]));
                                if ( DroidDesign.Turret3.IsUnknown )
                                {
                                    DroidComponentUnknownCount++;
                                }
                            }
                            DroidDesign.UpdateAttachments();
                            UnitType = DroidDesign;
                        }
                        else
                        {
                            UnitType = modProgram.ObjectData.FindOrCreateUnitType(INIObjects.Objects[A].Code, INIObjects.Objects[A].Type, INIObjects.Objects[A].WallType);
                            if ( UnitType.IsUnknown )
                            {
                                if ( UnknownUnitTypeCount < MaxUnknownUnitTypeWarningCount )
                                {
                                    ReturnResult.WarningAdd(ControlChars.Quote + INIObjects.Objects[A].Code + Convert.ToString(ControlChars.Quote) +
                                                            " is not a loaded object.");
                                }
                                UnknownUnitTypeCount++;
                            }
                        }
                    }
                    if ( UnitType == null )
                    {
                        ObjectTypeMissingCount++;
                    }
                    else
                    {
                        NewObject = new clsUnit();
                        NewObject.Type = UnitType;
                        NewObject.Pos.Horizontal.X = INIObjects.Objects[A].Pos.X;
                        NewObject.Pos.Horizontal.Y = INIObjects.Objects[A].Pos.Y;
                        NewObject.Health = INIObjects.Objects[A].Health;
                        NewObject.SavePriority = INIObjects.Objects[A].Priority;
                        NewObject.Rotation = (int)(INIObjects.Objects[A].Heading);
                        if ( NewObject.Rotation >= 360 )
                        {
                            NewObject.Rotation -= 360;
                        }
                        if ( INIObjects.Objects[A].UnitGroup == null || INIObjects.Objects[A].UnitGroup == "" )
                        {
                            if ( INIObjects.Objects[A].Type != clsUnitType.enumType.Feature )
                            {
                                ObjectPlayerNumInvalidCount++;
                            }
                            NewObject.UnitGroup = ScavengerUnitGroup;
                        }
                        else
                        {
                            if ( INIObjects.Objects[A].UnitGroup.ToLower() == "scavenger" )
                            {
                                NewObject.UnitGroup = ScavengerUnitGroup;
                            }
                            else
                            {
                                UInt32 PlayerNum = 0;
                                try
                                {
                                    if ( !modIO.InvariantParse_uint(INIObjects.Objects[A].UnitGroup, PlayerNum) )
                                    {
                                        throw (new Exception());
                                    }
                                    if ( PlayerNum < modProgram.PlayerCountMax )
                                    {
                                        UnitGroup = UnitGroups[Convert.ToInt32(PlayerNum)];
                                    }
                                    else
                                    {
                                        UnitGroup = ScavengerUnitGroup;
                                        ObjectPlayerNumInvalidCount++;
                                    }
                                }
                                catch ( Exception )
                                {
                                    ObjectPlayerNumInvalidCount++;
                                    UnitGroup = ScavengerUnitGroup;
                                }
                                NewObject.UnitGroup = UnitGroup;
                            }
                        }
                        if ( INIObjects.Objects[A].ID == 0U )
                        {
                            INIObjects.Objects[A].ID = AvailableID;
                            modProgram.ZeroIDWarning(NewObject, INIObjects.Objects[A].ID, ReturnResult);
                        }
                        UnitAdd.NewUnit = NewObject;
                        UnitAdd.ID = INIObjects.Objects[A].ID;
                        UnitAdd.Label = INIObjects.Objects[A].Label;
                        UnitAdd.Perform();
                        modProgram.ErrorIDChange(INIObjects.Objects[A].ID, NewObject, "Read_FMap_Objects");
                        if ( AvailableID == INIObjects.Objects[A].ID )
                        {
                            AvailableID = NewObject.ID + 1U;
                        }
                    }
                }
            }

            if ( UnknownUnitTypeCount > MaxUnknownUnitTypeWarningCount )
            {
                ReturnResult.WarningAdd(UnknownUnitTypeCount + " objects were not in the loaded object data.");
            }
            if ( ObjectTypeMissingCount > 0 )
            {
                ReturnResult.WarningAdd(ObjectTypeMissingCount + " objects did not specify a type and were ignored.");
            }
            if ( DroidComponentUnknownCount > 0 )
            {
                ReturnResult.WarningAdd(DroidComponentUnknownCount + " components used by droids were loaded as unknowns.");
            }
            if ( ObjectPlayerNumInvalidCount > 0 )
            {
                ReturnResult.WarningAdd(ObjectPlayerNumInvalidCount + " objects had an invalid player number and were set to player 0.");
            }
            if ( ObjectPosInvalidCount > 0 )
            {
                ReturnResult.WarningAdd(ObjectPosInvalidCount + " objects had a position that was off-map and were ignored.");
            }
            if ( DesignTypeUnspecifiedCount > 0 )
            {
                ReturnResult.WarningAdd(DesignTypeUnspecifiedCount + " designed droids did not specify a template droid type and were set to droid.");
            }

            return ReturnResult;
        }

        public class clsFMap_INIGateways : clsINIRead.clsSectionTranslator
        {
            public struct sGateway
            {
                public sXY_int PosA;
                public sXY_int PosB;
            }

            public sGateway[] Gateways;
            public int GatewayCount;

            public clsFMap_INIGateways(int NewGatewayCount)
            {
                int A = 0;

                GatewayCount = NewGatewayCount;
                Gateways = new sGateway[GatewayCount];
                for ( A = 0; A <= GatewayCount - 1; A++ )
                {
                    Gateways[A].PosA.X = -1;
                    Gateways[A].PosA.Y = -1;
                    Gateways[A].PosB.X = -1;
                    Gateways[A].PosB.Y = -1;
                }
            }

            public override clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty)
            {
                if ( (string)INIProperty.Name == "ax" )
                {
                    int temp_Result = Gateways[INISectionNum].PosA.X;
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref temp_Result) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "ay" )
                {
                    Int32 temp_Result2 = Gateways[INISectionNum].PosA.Y;
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref temp_Result2) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "bx" )
                {
                    Int32 temp_Result3 = Gateways[INISectionNum].PosB.X;
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref temp_Result3) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else if ( (string)INIProperty.Name == "by" )
                {
                    int temp_Result4 = Gateways[INISectionNum].PosB.Y;
                    if ( !modIO.InvariantParse_int(Convert.ToString(INIProperty.Value), ref temp_Result4) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.NameUnknown;
                }
                return clsINIRead.enumTranslatorResult.Translated;
            }
        }

        public clsResult Read_FMap_Gateways(StreamReader File)
        {
            clsResult ReturnResult = new clsResult("Reading gateways");

            clsINIRead GatewaysINI = new clsINIRead();
            ReturnResult.Take(GatewaysINI.ReadFile(File));

            clsFMap_INIGateways INIGateways = new clsFMap_INIGateways(GatewaysINI.Sections.Count);
            ReturnResult.Take(GatewaysINI.Translate(INIGateways));

            int A = 0;
            int InvalidGatewayCount = 0;

            for ( A = 0; A <= INIGateways.GatewayCount - 1; A++ )
            {
                if ( GatewayCreate(INIGateways.Gateways[A].PosA, INIGateways.Gateways[A].PosB) == null )
                {
                    InvalidGatewayCount++;
                }
            }

            if ( InvalidGatewayCount > 0 )
            {
                ReturnResult.WarningAdd(InvalidGatewayCount + " gateways were invalid.");
            }

            return ReturnResult;
        }

        public clsResult Read_FMap_TileTypes(BinaryReader File)
        {
            clsResult ReturnResult = new clsResult("Reading tile types");

            int A = 0;
            byte byteTemp = 0;
            int InvalidTypeCount = 0;

            try
            {
                if ( Tileset != null )
                {
                    for ( A = 0; A <= Tileset.TileCount - 1; A++ )
                    {
                        byteTemp = File.ReadByte();
                        if ( byteTemp >= modProgram.TileTypes.Count )
                        {
                            InvalidTypeCount++;
                        }
                        else
                        {
                            Tile_TypeNum[A] = byteTemp;
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            if ( InvalidTypeCount > 0 )
            {
                ReturnResult.WarningAdd(InvalidTypeCount + " tile types were invalid.");
            }

            if ( File.PeekChar() >= 0 )
            {
                ReturnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return ReturnResult;
        }
    }
}