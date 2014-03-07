#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Ionic.Zlib;
using NLog;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Core;
using SharpFlame.Core.Interfaces.Mapping.IO;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Domain;
using SharpFlame.Mapping.Tiles;

#endregion

namespace SharpFlame.Mapping.IO.FMap
{
    public class FMapSaver : IIOSaver
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly Map map;

        public FMapSaver(Map newMap)
        {
            map = newMap;
        }

        public Result Save(string path, bool overwrite, bool compress)
        {
            var returnResult = new Result(string.Format("Writing FMap to \"{0}\"", path), false);
            logger.Info(string.Format("Writing FMap to \"{0}\"", path));

            if ( !overwrite )
            {
                if ( File.Exists(path) )
                {
                    returnResult.ProblemAdd("The file already exists");
                    return returnResult;
                }
            }
            try
            {
                using ( var zip = new ZipOutputStream(path) )
                {
                    // Set encoding
                    zip.AlternateEncoding = Encoding.GetEncoding("UTF-8");
                    zip.AlternateEncodingUsage = ZipOption.Always;

                    // Set compression
                    if ( compress )
                    {
                        zip.CompressionLevel = CompressionLevel.BestCompression;
                    }
                    else
                    {
                        zip.CompressionMethod = CompressionMethod.None;
                    }

                    var binaryWriter = new BinaryWriter(zip, App.UTF8Encoding);
                    var streamWriter = new StreamWriter(zip, App.UTF8Encoding);

                    zip.PutNextEntry("Info.ini");
                    var infoIniWriter = new IniWriter(streamWriter);
                    returnResult.Add(serialize_FMap_Info(infoIniWriter));
                    streamWriter.Flush();

                    zip.PutNextEntry("VertexHeight.dat");
                    returnResult.Add(serialize_FMap_VertexHeight(binaryWriter));
                    binaryWriter.Flush();

                    zip.PutNextEntry("VertexTerrain.dat");
                    returnResult.Add(serialize_FMap_VertexTerrain(binaryWriter));
                    binaryWriter.Flush();

                    zip.PutNextEntry("TileTexture.dat");
                    returnResult.Add(serialize_FMap_TileTexture(binaryWriter));
                    binaryWriter.Flush();

                    zip.PutNextEntry("TileOrientation.dat");
                    returnResult.Add(serialize_FMap_TileOrientation(binaryWriter));
                    binaryWriter.Flush();

                    zip.PutNextEntry("TileCliff.dat");
                    returnResult.Add(serialize_FMap_TileCliff(binaryWriter));
                    binaryWriter.Flush();

                    zip.PutNextEntry("Roads.dat");
                    returnResult.Add(serialize_FMap_Roads(binaryWriter));
                    binaryWriter.Flush();

                    zip.PutNextEntry("Objects.ini");
                    var objectsIniWriter = new IniWriter(streamWriter);
                    returnResult.Add(serialize_FMap_Objects(objectsIniWriter));
                    streamWriter.Flush();

                    zip.PutNextEntry("Gateways.ini");
                    var gatewaysIniWriter = new IniWriter(streamWriter);
                    returnResult.Add(serialize_FMap_Gateways(gatewaysIniWriter));
                    streamWriter.Flush();

                    zip.PutNextEntry("TileTypes.dat");
                    returnResult.Add(serialize_FMap_TileTypes(binaryWriter));
                    binaryWriter.Flush();

                    zip.PutNextEntry("ScriptLabels.ini");
                    var scriptLabelsIniWriter = new IniWriter(streamWriter);
                    returnResult.Add(serialize_WZ_LabelsINI(scriptLabelsIniWriter));
                    streamWriter.Flush();

                    streamWriter.Close();
                    binaryWriter.Close();
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            return returnResult;
        }

        private Result serialize_WZ_LabelsINI(IniWriter file)
        {
            var returnResult = new Result("Serializing labels INI", false);
            logger.Info("Serializing labels INI");

            try
            {
                foreach ( var scriptPosition in map.ScriptPositions )
                {
                    scriptPosition.WriteWZ(file);
                }
                foreach ( var scriptArea in map.ScriptAreas )
                {
                    scriptArea.WriteWZ(file);
                }
            }
            catch ( Exception ex )
            {
                returnResult.WarningAdd(ex.Message);
                logger.ErrorException("Got an exception", ex);
            }

            return returnResult;
        }

        private Result serialize_FMap_Info(IniWriter file)
        {
            var ReturnResult = new Result("Serializing general map info", false);
            logger.Info("Serializing general map info");

            try
            {
                if ( map.Tileset == null )
                {
                }
                else if ( map.Tileset == App.Tileset_Arizona )
                {
                    file.AddProperty("Tileset", "Arizona");
                }
                else if ( map.Tileset == App.Tileset_Urban )
                {
                    file.AddProperty("Tileset", "Urban");
                }
                else if ( map.Tileset == App.Tileset_Rockies )
                {
                    file.AddProperty("Tileset", "Rockies");
                }

                file.AddProperty("Size", map.Terrain.TileSize.X.ToStringInvariant() + ", " + map.Terrain.TileSize.Y.ToStringInvariant());

                file.AddProperty("AutoScrollLimits", map.InterfaceOptions.AutoScrollLimits.ToStringInvariant());
                file.AddProperty("ScrollMinX", map.InterfaceOptions.ScrollMin.X.ToStringInvariant());
                file.AddProperty("ScrollMinY", map.InterfaceOptions.ScrollMin.Y.ToStringInvariant());
                file.AddProperty("ScrollMaxX", map.InterfaceOptions.ScrollMax.X.ToStringInvariant());
                file.AddProperty("ScrollMaxY", map.InterfaceOptions.ScrollMax.Y.ToStringInvariant());

                file.AddProperty("Name", map.InterfaceOptions.CompileName);
                file.AddProperty("Players", map.InterfaceOptions.CompileMultiPlayers);
                file.AddProperty("Author", map.InterfaceOptions.CompileMultiAuthor);
                file.AddProperty("License", map.InterfaceOptions.CompileMultiLicense);
                if ( map.InterfaceOptions.CampaignGameType >= 0 )
                {
                    file.AddProperty("CampType", map.InterfaceOptions.CampaignGameType.ToStringInvariant());
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            return ReturnResult;
        }

        private Result serialize_FMap_VertexHeight(BinaryWriter file)
        {
            var ReturnResult = new Result("Serializing vertex heights", false);
            logger.Info("Serializing vertex heights");
            var X = 0;
            var Y = 0;

            try
            {
                for ( Y = 0; Y <= map.Terrain.TileSize.Y; Y++ )
                {
                    for ( X = 0; X <= map.Terrain.TileSize.X; X++ )
                    {
                        file.Write(map.Terrain.Vertices[X, Y].Height);
                    }
                }
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
            }

            return ReturnResult;
        }

        private Result serialize_FMap_VertexTerrain(BinaryWriter file)
        {
            var ReturnResult = new Result("Serializing vertex terrain", false);
            logger.Info("Serializing vertex terrain");

            var X = 0;
            var Y = 0;
            var ErrorCount = 0;
            var Value = 0;

            try
            {
                for ( Y = 0; Y <= map.Terrain.TileSize.Y; Y++ )
                {
                    for ( X = 0; X <= map.Terrain.TileSize.X; X++ )
                    {
                        if ( map.Terrain.Vertices[X, Y].Terrain == null )
                        {
                            Value = 0;
                        }
                        else if ( map.Terrain.Vertices[X, Y].Terrain.Num < 0 )
                        {
                            ErrorCount++;
                            Value = 0;
                        }
                        else
                        {
                            Value = Convert.ToInt32(map.Terrain.Vertices[X, Y].Terrain.Num + 1);
                            if ( Value > 255 )
                            {
                                ErrorCount++;
                                Value = 0;
                            }
                        }
                        file.Write((byte)Value);
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

        private Result serialize_FMap_TileTexture(BinaryWriter file)
        {
            var ReturnResult = new Result("Serializing tile textures", false);
            logger.Info("Serializing tile textures");

            var X = 0;
            var Y = 0;
            var ErrorCount = 0;
            var Value = 0;

            try
            {
                for ( Y = 0; Y <= map.Terrain.TileSize.Y - 1; Y++ )
                {
                    for ( X = 0; X <= map.Terrain.TileSize.X - 1; X++ )
                    {
                        Value = Convert.ToInt32(map.Terrain.Tiles[X, Y].Texture.TextureNum + 1);
                        if ( Value < 0 | Value > 255 )
                        {
                            ErrorCount++;
                            Value = 0;
                        }
                        file.Write((byte)Value);
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

        private Result serialize_FMap_TileOrientation(BinaryWriter file)
        {
            var returnResult = new Result("Serializing tile orientations", false);
            logger.Info("Serializing tile orientations");
            var x = 0;
            var y = 0;
            var value = 0;

            try
            {
                for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                    {
                        value = 0;
                        if ( map.Terrain.Tiles[x, y].Texture.Orientation.SwitchedAxes )
                        {
                            value += 8;
                        }
                        if ( map.Terrain.Tiles[x, y].Texture.Orientation.XFlip )
                        {
                            value += 4;
                        }
                        if ( map.Terrain.Tiles[x, y].Texture.Orientation.YFlip )
                        {
                            value += 2;
                        }
                        if ( map.Terrain.Tiles[x, y].Tri )
                        {
                            value++;
                        }
                        file.Write((byte)value);
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
            }

            return returnResult;
        }

        private Result serialize_FMap_TileCliff(BinaryWriter file)
        {
            var returnResult = new Result("Serializing tile cliffs", false);
            logger.Info("Serializing tile cliffs");

            var x = 0;
            var y = 0;
            var value = 0;
            var downSideValue = 0;
            var errorCount = 0;

            try
            {
                for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                    {
                        value = 0;
                        if ( map.Terrain.Tiles[x, y].Tri )
                        {
                            if ( map.Terrain.Tiles[x, y].TriTopLeftIsCliff )
                            {
                                value += 2;
                            }
                            if ( map.Terrain.Tiles[x, y].TriBottomRightIsCliff )
                            {
                                value++;
                            }
                        }
                        else
                        {
                            if ( map.Terrain.Tiles[x, y].TriBottomLeftIsCliff )
                            {
                                value += 2;
                            }
                            if ( map.Terrain.Tiles[x, y].TriTopRightIsCliff )
                            {
                                value++;
                            }
                        }
                        if ( map.Terrain.Tiles[x, y].Terrain_IsCliff )
                        {
                            value += 4;
                        }
                        if ( TileUtil.IdenticalTileDirections(map.Terrain.Tiles[x, y].DownSide, TileUtil.None) )
                        {
                            downSideValue = 0;
                        }
                        else if ( TileUtil.IdenticalTileDirections(map.Terrain.Tiles[x, y].DownSide, TileUtil.Top) )
                        {
                            downSideValue = 1;
                        }
                        else if ( TileUtil.IdenticalTileDirections(map.Terrain.Tiles[x, y].DownSide, TileUtil.Left) )
                        {
                            downSideValue = 2;
                        }
                        else if ( TileUtil.IdenticalTileDirections(map.Terrain.Tiles[x, y].DownSide, TileUtil.Right) )
                        {
                            downSideValue = 3;
                        }
                        else if ( TileUtil.IdenticalTileDirections(map.Terrain.Tiles[x, y].DownSide, TileUtil.Bottom) )
                        {
                            downSideValue = 4;
                        }
                        else
                        {
                            errorCount++;
                            downSideValue = 0;
                        }
                        value += downSideValue * 8;
                        file.Write((byte)value);
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
            }

            if ( errorCount > 0 )
            {
                returnResult.WarningAdd(errorCount + " tiles had an invalid cliff down side.");
            }

            return returnResult;
        }

        private Result serialize_FMap_Roads(BinaryWriter file)
        {
            var returnResult = new Result("Serializing roads", false);
            logger.Info("Serializing roads");

            var x = 0;
            var y = 0;
            var value = 0;
            var errorCount = 0;

            try
            {
                for ( y = 0; y <= map.Terrain.TileSize.Y; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                    {
                        if ( map.Terrain.SideH[x, y].Road == null )
                        {
                            value = 0;
                        }
                        else if ( map.Terrain.SideH[x, y].Road.Num < 0 )
                        {
                            errorCount++;
                            value = 0;
                        }
                        else
                        {
                            value = Convert.ToInt32(map.Terrain.SideH[x, y].Road.Num + 1);
                            if ( value > 255 )
                            {
                                errorCount++;
                                value = 0;
                            }
                        }
                        file.Write((byte)value);
                    }
                }
                for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X; x++ )
                    {
                        if ( map.Terrain.SideV[x, y].Road == null )
                        {
                            value = 0;
                        }
                        else if ( map.Terrain.SideV[x, y].Road.Num < 0 )
                        {
                            errorCount++;
                            value = 0;
                        }
                        else
                        {
                            value = Convert.ToInt32(map.Terrain.SideV[x, y].Road.Num + 1);
                            if ( value > 255 )
                            {
                                errorCount++;
                                value = 0;
                            }
                        }
                        file.Write((byte)value);
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
            }

            if ( errorCount > 0 )
            {
                returnResult.WarningAdd(errorCount + " sides had an invalid road number.");
            }

            return returnResult;
        }

        private Result serialize_FMap_Objects(IniWriter file)
        {
            var returnResult = new Result("Serializing objects", false);
            logger.Info("Serializing objects");

            var droid = default(DroidDesign);
            var warningCount = 0;
            string text = null;

            try
            {
                var a = 0;
                foreach (var unit in map.Units) 
                {
                    file.AddSection(a.ToStringInvariant());
                    switch ( unit.TypeBase.Type )
                    {
                        case UnitType.Feature:
                        file.AddProperty("Type", "Feature, " + ((FeatureTypeBase)unit.TypeBase).Code);
                        break;
                        case UnitType.PlayerStructure:
                        var structureTypeBase = (StructureTypeBase)unit.TypeBase;
                        file.AddProperty("Type", "Structure, " + structureTypeBase.Code);
                        if ( structureTypeBase.WallLink.IsConnected )
                        {
                            file.AddProperty("WallType", structureTypeBase.WallLink.ArrayPosition.ToStringInvariant());
                        }
                        break;
                        case UnitType.PlayerDroid:
                        droid = (DroidDesign)unit.TypeBase;
                        if ( droid.IsTemplate )
                        {
                            file.AddProperty("Type", "DroidTemplate, " + ((DroidTemplate)unit.TypeBase).Code);
                        }
                        else
                        {
                            file.AddProperty("Type", "DroidDesign");
                            if ( droid.TemplateDroidType != null )
                            {
                                file.AddProperty("DroidType", droid.TemplateDroidType.TemplateCode);
                            }
                            if ( droid.Body != null )
                            {
                                file.AddProperty("Body", droid.Body.Code);
                            }
                            if ( droid.Propulsion != null )
                            {
                                file.AddProperty("Propulsion", droid.Propulsion.Code);
                            }
                            file.AddProperty("TurretCount", droid.TurretCount.ToStringInvariant());
                            if ( droid.Turret1 != null )
                            {
                                if ( droid.Turret1.GetTurretTypeName(ref text) )
                                {
                                    file.AddProperty ("Turret1", string.Format ("{0}, {1}", text, droid.Turret1.Code));
                                }
                            }
                            if ( droid.Turret2 != null )
                            {
                                if ( droid.Turret2.GetTurretTypeName(ref text) )
                                {
                                    file.AddProperty ("Turret2", string.Format ("{0}, {1}", text, droid.Turret2.Code));
                                }
                            }
                            if ( droid.Turret3 != null )
                            {
                                if ( droid.Turret3.GetTurretTypeName(ref text) )
                                {
                                    file.AddProperty ("Turret3", string.Format ("{0}, {1}", text, droid.Turret3.Code));
                                }
                            }
                        }
                        break;
                        default:
                        warningCount++;
                        break;
                    }
                    file.AddProperty("ID", unit.ID.ToStringInvariant());
                    file.AddProperty("Priority", unit.SavePriority.ToStringInvariant());
                    file.AddProperty("Pos", unit.Pos.Horizontal.X.ToStringInvariant() + ", " + unit.Pos.Horizontal.Y.ToStringInvariant());
                    file.AddProperty("Heading", unit.Rotation.ToStringInvariant());
                    file.AddProperty("UnitGroup", unit.UnitGroup.GetFMapINIPlayerText());
                    if ( unit.Health < 1.0D )
                    {
                        file.AddProperty("Health", unit.Health.ToStringInvariant());
                    }
                    if ( unit.Label != null )
                    {
                        file.AddProperty("ScriptLabel", unit.Label);
                    }

                    a++;
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
            }

            if ( warningCount > 0 )
            {
                returnResult.WarningAdd("Error: " + Convert.ToString(warningCount) + " units were of an unhandled type.");
            }

            return returnResult;
        }

        private Result serialize_FMap_Gateways(IniWriter File)
        {
            var returnResult = new Result("Serializing gateways", false);
            logger.Info("Serializing gateways");

            try
            {
                var a = 0;
                foreach ( var gateway in map.Gateways)
                {
                    File.AddSection(a.ToStringInvariant());
                    File.AddProperty("AX", gateway.PosA.X.ToStringInvariant());
                    File.AddProperty("AY", gateway.PosA.Y.ToStringInvariant());
                    File.AddProperty("BX", gateway.PosB.X.ToStringInvariant());
                    File.AddProperty("BY", gateway.PosB.Y.ToStringInvariant());
                    a++;
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
            }

            return returnResult;
        }

        private Result serialize_FMap_TileTypes(BinaryWriter file)
        {
            var returnResult = new Result("Serializing tile types", false);
            logger.Info("Serializing tile types");
            var a = 0;

            try
            {
                if ( map.Tileset != null )
                {
                    for ( a = 0; a <= map.Tileset.Tiles.Count - 1; a++ )
                    {
                        file.Write(map.TileTypeNum[a]);
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
            }

            return returnResult;
        }
    }
}

