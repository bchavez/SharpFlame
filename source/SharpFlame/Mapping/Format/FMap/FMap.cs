#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Ionic.Zlib;
using NLog;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;
using IniReader = SharpFlame.FileIO.Ini.IniReader;
using Section = SharpFlame.FileIO.Ini.Section;

#endregion

namespace SharpFlame.Mapping.Format.FMap
{
    public class FMap
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly clsMap map;

        public FMap(clsMap newMap)
        {
            map = newMap;
        }

        public clsResult Load(string path)
        {
            var returnResult = new clsResult(string.Format("Loading FMap from \"{0}\"", path), false);
            logger.Info(string.Format("Loading FMap from \"{0}\"", path));

            using ( var zip = ZipFile.Read(path) )
            {
                /*
                 * Info.ini loading
                 */
                var infoIniEntry = zip["info.ini"]; // Case insensetive.
                if ( infoIniEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"info.ini\".");
                    return returnResult;
                }

                FMapInfo resultInfo = null;
                using ( Stream s = infoIniEntry.OpenReader() )
                {
                    var reader = new StreamReader(s);
                    returnResult.Add(read_FMap_Info(reader, ref resultInfo));
                    reader.Close();
                    if ( returnResult.HasProblems )
                    {
                        return returnResult;
                    }
                }

                var newTerrainSize = resultInfo.TerrainSize;
                map.Tileset = resultInfo.Tileset;

                if ( newTerrainSize.X <= 0 | newTerrainSize.X > Constants.MapMaxSize )
                {
                    returnResult.ProblemAdd(string.Format("Map width of {0} is not valid.", newTerrainSize.X));
                }
                if ( newTerrainSize.Y <= 0 | newTerrainSize.Y > Constants.MapMaxSize )
                {
                    returnResult.ProblemAdd(string.Format("Map height of {0} is not valid.", newTerrainSize.Y));
                }
                if ( returnResult.HasProblems )
                {
                    return returnResult;
                }

                map.SetPainterToDefaults(); //depends on tileset. must be called before loading the terrains.
                map.TerrainBlank(newTerrainSize);
                map.TileType_Reset();

                // vertexheight.dat
                var vhEntry = zip["vertexheight.dat"]; // Case insensetive.
                if ( vhEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"vertexheight.dat\".");
                }
                else
                {
                    using ( Stream s = vhEntry.OpenReader() )
                    {
                        var reader = new BinaryReader(s);
                        returnResult.Add(read_FMap_VertexHeight(reader));
                        reader.Close();
                    }
                }

                // vertexterrain.dat
                var vtEntry = zip["vertexterrain.dat"]; // Case insensetive.
                if ( vtEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"vertexterrain.dat\".");
                }
                else
                {
                    using ( Stream s = vtEntry.OpenReader() )
                    {
                        var reader = new BinaryReader(s);
                        returnResult.Add(read_FMap_VertexTerrain(reader));
                        reader.Close();
                    }
                }

                // tiletexture.dat
                var ttEntry = zip["tiletexture.dat"]; // Case insensetive.
                if ( vtEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"tiletexture.dat\".");
                }
                else
                {
                    using ( Stream s = ttEntry.OpenReader() )
                    {
                        var reader = new BinaryReader(s);
                        returnResult.Add(read_FMap_TileTexture(reader));
                        reader.Close();
                    }
                }

                // tileorientation.dat
                var toEntry = zip["tileorientation.dat"]; // Case insensetive.
                if ( toEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"tileorientation.dat\".");
                }
                else
                {
                    using ( Stream s = toEntry.OpenReader() )
                    {
                        var reader = new BinaryReader(s);
                        returnResult.Add(read_FMap_TileOrientation(reader));
                        reader.Close();
                    }
                }

                // tilecliff.dat
                var tcEntry = zip["tilecliff.dat"]; // Case insensetive.
                if ( tcEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"tilecliff.dat\".");
                }
                else
                {
                    using ( Stream s = tcEntry.OpenReader() )
                    {
                        var reader = new BinaryReader(s);
                        returnResult.Add(read_FMap_TileCliff(reader));
                        reader.Close();
                    }
                }

                // roads.dat
                var roEntry = zip["roads.dat"]; // Case insensetive.
                if ( roEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"roads.dat\".");
                }
                else
                {
                    using ( Stream s = roEntry.OpenReader() )
                    {
                        var reader = new BinaryReader(s);
                        returnResult.Add(read_FMap_Roads(reader));
                        reader.Close();
                    }
                }

                // objects.ini
                var obEntry = zip["objects.ini"]; // Case insensetive.
                if ( obEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"objects.ini\".");
                }
                else
                {
                    using ( Stream s = obEntry.OpenReader() )
                    {
                        var reader = new StreamReader(s);
                        returnResult.Add(read_FMap_Objects(reader));
                        reader.Close();
                    }
                }

                // gateways.ini
                var gaEntry = zip["gateways.ini"]; // Case insensetive.
                if ( gaEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"gateways.ini\".");
                    return returnResult;
                }
                using ( Stream s = gaEntry.OpenReader() )
                {
                    var reader = new StreamReader(s);
                    returnResult.Add(read_FMap_Gateways(reader));
                    reader.Close();
                }

                // tiletypes.dat
                var tileTypesEntry = zip["tiletypes.dat"]; // Case insensetive.
                if ( tileTypesEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"tiletypes.dat\".");
                }
                else
                {
                    using ( Stream s = tileTypesEntry.OpenReader() )
                    {
                        var reader = new BinaryReader(s);
                        returnResult.Add(read_FMap_TileTypes(reader));
                        reader.Close();
                    }
                }

                // scriptlabels.ini
                var scriptLabelsEntry = zip["scriptlabels.ini"]; // Case insensetive.
                if ( scriptLabelsEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"scriptlabels.ini\".");
                    return returnResult;
                }
                if ( scriptLabelsEntry != null )
                {
                    using ( var reader = new StreamReader(scriptLabelsEntry.OpenReader()) )
                    {
                        var text = reader.ReadToEnd();
                        returnResult.Add(read_INI_Labels(text));
                    }
                }

                map.InterfaceOptions = resultInfo.InterfaceOptions;
            }

            return returnResult;
        }

        public clsResult Save(string path, bool overwrite, bool compress)
        {
            var returnResult = new clsResult(string.Format("Writing FMap to \"{0}\"", path), false);
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

        private clsResult serialize_WZ_LabelsINI(IniWriter file)
        {
            var returnResult = new clsResult("Serializing labels INI", false);
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

        private clsResult serialize_FMap_Info(IniWriter file)
        {
            var ReturnResult = new clsResult("Serializing general map info", false);
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
                file.AddProperty("XPlayerLev", map.InterfaceOptions.CompileMultiXPlayers.ToStringInvariant());
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

        private clsResult serialize_FMap_VertexHeight(BinaryWriter file)
        {
            var ReturnResult = new clsResult("Serializing vertex heights", false);
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

        private clsResult serialize_FMap_VertexTerrain(BinaryWriter file)
        {
            var ReturnResult = new clsResult("Serializing vertex terrain", false);
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

        private clsResult serialize_FMap_TileTexture(BinaryWriter file)
        {
            var ReturnResult = new clsResult("Serializing tile textures", false);
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

        private clsResult serialize_FMap_TileOrientation(BinaryWriter file)
        {
            var returnResult = new clsResult("Serializing tile orientations", false);
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
                        if ( map.Terrain.Tiles[x, y].Texture.Orientation.ResultXFlip )
                        {
                            value += 4;
                        }
                        if ( map.Terrain.Tiles[x, y].Texture.Orientation.ResultYFlip )
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

        private clsResult serialize_FMap_TileCliff(BinaryWriter file)
        {
            var returnResult = new clsResult("Serializing tile cliffs", false);
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

        private clsResult serialize_FMap_Roads(BinaryWriter file)
        {
            var returnResult = new clsResult("Serializing roads", false);
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

        private clsResult serialize_FMap_Objects(IniWriter file)
        {
            var returnResult = new clsResult("Serializing objects", false);
            logger.Info("Serializing objects");

            var a = 0;
            var droid = default(DroidDesign);
            var warningCount = 0;
            string text = null;

            try
            {
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

        private clsResult serialize_FMap_Gateways(IniWriter File)
        {
            var returnResult = new clsResult("Serializing gateways", false);
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

        private clsResult serialize_FMap_TileTypes(BinaryWriter file)
        {
            var returnResult = new clsResult("Serializing tile types", false);
            logger.Info("Serializing tile types");
            var a = 0;

            try
            {
                if ( map.Tileset != null )
                {
                    for ( a = 0; a <= map.Tileset.TileCount - 1; a++ )
                    {
                        file.Write(map.Tile_TypeNum[a]);
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
            }

            return returnResult;
        }

        private clsResult read_FMap_Info(StreamReader file, ref FMapInfo resultInfo)
        {
            var returnResult = new clsResult("Read general map info", false);
            logger.Info("Read general map info");

            var infoINI = new Section();
            returnResult.Take(infoINI.ReadFile(file));

            resultInfo = new FMapInfo();
            returnResult.Take(infoINI.Translate(resultInfo));

            if ( resultInfo.TerrainSize.X < 0 | resultInfo.TerrainSize.Y < 0 )
            {
                returnResult.ProblemAdd("Map size was not specified or was invalid.");
            }

            return returnResult;
        }

        private clsResult read_FMap_VertexHeight(BinaryReader file)
        {
            var returnResult = new clsResult("Reading vertex heights", false);
            logger.Info("Reading vertex heights");

            var x = 0;
            var y = 0;

            try
            {
                for ( y = 0; y <= map.Terrain.TileSize.Y; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X; x++ )
                    {
                        map.Terrain.Vertices[x, y].Height = file.ReadByte();
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            if ( file.PeekChar() >= 0 )
            {
                returnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return returnResult;
        }

        private clsResult read_FMap_VertexTerrain(BinaryReader file)
        {
            var returnResult = new clsResult("Reading vertex terrain", false);
            logger.Info("Reading vertex terrain");

            var x = 0;
            var y = 0;
            var value = 0;
            byte byteTemp = 0;
            var warningCount = 0;

            try
            {
                for ( y = 0; y <= map.Terrain.TileSize.Y; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X; x++ )
                    {
                        byteTemp = file.ReadByte();
                        value = (byteTemp) - 1;
                        if ( value < 0 )
                        {
                            map.Terrain.Vertices[x, y].Terrain = null;
                        }
                        else if ( value >= map.Painter.TerrainCount )
                        {
                            if ( warningCount < 16 )
                            {
                                returnResult.WarningAdd("Painted terrain at vertex " + Convert.ToString(x) + ", " + Convert.ToString(y) +
                                                        " was invalid.");
                            }
                            warningCount++;
                            map.Terrain.Vertices[x, y].Terrain = null;
                        }
                        else
                        {
                            map.Terrain.Vertices[x, y].Terrain = map.Painter.Terrains[value];
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            if ( warningCount > 0 )
            {
                returnResult.WarningAdd(warningCount + " painted terrain vertices were invalid.");
            }

            if ( file.PeekChar() >= 0 )
            {
                returnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return returnResult;
        }

        private clsResult read_FMap_TileTexture(BinaryReader file)
        {
            var returnResult = new clsResult("Reading tile textures", false);
            logger.Info("Reading tile textures");

            var x = 0;
            var y = 0;
            byte byteTemp = 0;

            try
            {
                for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                    {
                        byteTemp = file.ReadByte();
                        map.Terrain.Tiles[x, y].Texture.TextureNum = (byteTemp) - 1;
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            if ( file.PeekChar() >= 0 )
            {
                returnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return returnResult;
        }

        private clsResult read_FMap_TileOrientation(BinaryReader file)
        {
            var returnResult = new clsResult("Reading tile orientations", false);
            logger.Info("Reading tile orientations");

            var x = 0;
            var y = 0;
            var value = 0;
            var partValue = 0;
            var warningCount = 0;

            try
            {
                for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                    {
                        value = file.ReadByte();

                        partValue = (int)(Math.Floor(((double)value / 16)));
                        if ( partValue > 0 )
                        {
                            if ( warningCount < 16 )
                            {
                                returnResult.WarningAdd("Unknown bits used for tile " + Convert.ToString(x) + ", " + Convert.ToString(y) + ".");
                            }
                            warningCount++;
                        }
                        value -= partValue * 16;

                        partValue = (int)(value / 8.0D);
                        map.Terrain.Tiles[x, y].Texture.Orientation.SwitchedAxes = partValue > 0;
                        value -= partValue * 8;

                        partValue = (int)(value / 4.0D);
                        map.Terrain.Tiles[x, y].Texture.Orientation.ResultXFlip = partValue > 0;
                        value -= partValue * 4;

                        partValue = (int)(value / 2.0D);
                        map.Terrain.Tiles[x, y].Texture.Orientation.ResultYFlip = partValue > 0;
                        value -= partValue * 2;

                        partValue = value;
                        map.Terrain.Tiles[x, y].Tri = partValue > 0;
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            if ( warningCount > 0 )
            {
                returnResult.WarningAdd(warningCount + " tiles had unknown bits used.");
            }

            if ( file.PeekChar() >= 0 )
            {
                returnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return returnResult;
        }

        private clsResult read_FMap_TileCliff(BinaryReader file)
        {
            var returnResult = new clsResult("Reading tile cliffs", false);
            logger.Info("Reading tile cliffs");

            var x = 0;
            var y = 0;
            var value = 0;
            var partValue = 0;
            var downSideWarningCount = 0;
            var warningCount = 0;

            try
            {
                for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                    {
                        value = file.ReadByte();

                        partValue = (int)(value / 64.0D);
                        if ( partValue > 0 )
                        {
                            if ( warningCount < 16 )
                            {
                                returnResult.WarningAdd("Unknown bits used for tile " + Convert.ToString(x) + ", " + Convert.ToString(y) + ".");
                            }
                            warningCount++;
                        }
                        value -= partValue * 64;

                        partValue = (int)(value / 8.0D);
                        switch ( partValue )
                        {
                        case 0:
                            map.Terrain.Tiles[x, y].DownSide = TileUtil.None;
                            break;
                        case 1:
                            map.Terrain.Tiles[x, y].DownSide = TileUtil.Top;
                            break;
                        case 2:
                            map.Terrain.Tiles[x, y].DownSide = TileUtil.Left;
                            break;
                        case 3:
                            map.Terrain.Tiles[x, y].DownSide = TileUtil.Right;
                            break;
                        case 4:
                            map.Terrain.Tiles[x, y].DownSide = TileUtil.Bottom;
                            break;
                        default:
                            map.Terrain.Tiles[x, y].DownSide = TileUtil.None;
                            if ( downSideWarningCount < 16 )
                            {
                                returnResult.WarningAdd("Down side value for tile " + Convert.ToString(x) + ", " + Convert.ToString(y) +
                                                        " was invalid.");
                            }
                            downSideWarningCount++;
                            break;
                        }
                        value -= partValue * 8;

                        partValue = (int)(value / 4.0D);
                        map.Terrain.Tiles[x, y].Terrain_IsCliff = partValue > 0;
                        value -= partValue * 4;

                        partValue = (int)(value / 2.0D);
                        if ( map.Terrain.Tiles[x, y].Tri )
                        {
                            map.Terrain.Tiles[x, y].TriTopLeftIsCliff = partValue > 0;
                        }
                        else
                        {
                            map.Terrain.Tiles[x, y].TriBottomLeftIsCliff = partValue > 0;
                        }
                        value -= partValue * 2;

                        partValue = value;
                        if ( map.Terrain.Tiles[x, y].Tri )
                        {
                            map.Terrain.Tiles[x, y].TriBottomRightIsCliff = partValue > 0;
                        }
                        else
                        {
                            map.Terrain.Tiles[x, y].TriTopRightIsCliff = partValue > 0;
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            if ( warningCount > 0 )
            {
                returnResult.WarningAdd(warningCount + " tiles had unknown bits used.");
            }
            if ( downSideWarningCount > 0 )
            {
                returnResult.WarningAdd(downSideWarningCount + " tiles had invalid down side values.");
            }

            if ( file.PeekChar() >= 0 )
            {
                returnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return returnResult;
        }

        private clsResult read_FMap_Roads(BinaryReader file)
        {
            var returnResult = new clsResult("Reading roads", false);
            logger.Info("Reading roads");

            var x = 0;
            var y = 0;
            var value = 0;
            var warningCount = 0;

            try
            {
                for ( y = 0; y <= map.Terrain.TileSize.Y; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X - 1; x++ )
                    {
                        value = file.ReadByte() - 1;
                        if ( value < 0 )
                        {
                            map.Terrain.SideH[x, y].Road = null;
                        }
                        else if ( value >= map.Painter.RoadCount )
                        {
                            if ( warningCount < 16 )
                            {
                                returnResult.WarningAdd("Invalid road value for horizontal side " + Convert.ToString(x) + ", " + Convert.ToString(y) +
                                                        ".");
                            }
                            warningCount++;
                            map.Terrain.SideH[x, y].Road = null;
                        }
                        else
                        {
                            map.Terrain.SideH[x, y].Road = map.Painter.Roads[value];
                        }
                    }
                }
                for ( y = 0; y <= map.Terrain.TileSize.Y - 1; y++ )
                {
                    for ( x = 0; x <= map.Terrain.TileSize.X; x++ )
                    {
                        value = file.ReadByte() - 1;
                        if ( value < 0 )
                        {
                            map.Terrain.SideV[x, y].Road = null;
                        }
                        else if ( value >= map.Painter.RoadCount )
                        {
                            if ( warningCount < 16 )
                            {
                                returnResult.WarningAdd("Invalid road value for vertical side " + Convert.ToString(x) + ", " + Convert.ToString(y) +
                                                        ".");
                            }
                            warningCount++;
                            map.Terrain.SideV[x, y].Road = null;
                        }
                        else
                        {
                            map.Terrain.SideV[x, y].Road = map.Painter.Roads[value];
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            if ( warningCount > 0 )
            {
                returnResult.WarningAdd(warningCount + " sides had an invalid road value.");
            }

            if ( file.PeekChar() >= 0 )
            {
                returnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return returnResult;
        }

        private clsResult read_FMap_Objects(StreamReader file)
        {
            var returnResult = new clsResult("Reading objects", false);
            logger.Info("Reading objects");

            var a = 0;

            var objectsINI = new IniReader();
            returnResult.Take(objectsINI.ReadFile(file));

            var iniObjects = new FMapIniObjects(objectsINI.Sections.Count);
            returnResult.Take(objectsINI.Translate(iniObjects));

            var droidComponentUnknownCount = 0;
            var objectTypeMissingCount = 0;
            var objectPlayerNumInvalidCount = 0;
            var objectPosInvalidCount = 0;
            var designTypeUnspecifiedCount = 0;
            var unknownUnitTypeCount = 0;
            var maxUnknownUnitTypeWarningCount = 16;

            var droidDesign = default(DroidDesign);
            var newObject = default(clsUnit);
            var unitAdd = new clsUnitAdd();
            var unitTypeBase = default(UnitTypeBase);
            var isDesign = default(bool);
            var unitGroup = default(clsUnitGroup);
            var zeroPos = new XYInt(0, 0);
            UInt32 availableID = 0;

            unitAdd.Map = map;

            availableID = 1U;
            for ( a = 0; a <= iniObjects.ObjectCount - 1; a++ )
            {
                if ( iniObjects.Objects[a].ID >= availableID )
                {
                    availableID = iniObjects.Objects[a].ID + 1U;
                }
            }
            for ( a = 0; a <= iniObjects.ObjectCount - 1; a++ )
            {
                if ( iniObjects.Objects[a].Pos == null )
                {
                    objectPosInvalidCount++;
                }
                else if ( !App.PosIsWithinTileArea(iniObjects.Objects[a].Pos, zeroPos, map.Terrain.TileSize) )
                {
                    objectPosInvalidCount++;
                }
                else
                {
                    unitTypeBase = null;
                    if ( iniObjects.Objects[a].Type != UnitType.Unspecified )
                    {
                        isDesign = false;
                        if ( iniObjects.Objects[a].Type == UnitType.PlayerDroid )
                        {
                            if ( !iniObjects.Objects[a].IsTemplate )
                            {
                                isDesign = true;
                            }
                        }
                        if ( isDesign )
                        {
                            droidDesign = new DroidDesign();
                            droidDesign.TemplateDroidType = iniObjects.Objects[a].TemplateDroidType;
                            if ( droidDesign.TemplateDroidType == null )
                            {
                                droidDesign.TemplateDroidType = App.TemplateDroidType_Droid;
                                designTypeUnspecifiedCount++;
                            }
                            if ( iniObjects.Objects[a].BodyCode != "" )
                            {
                                droidDesign.Body = App.ObjectData.FindOrCreateBody(Convert.ToString(iniObjects.Objects[a].BodyCode));
                                if ( droidDesign.Body.IsUnknown )
                                {
                                    droidComponentUnknownCount++;
                                }
                            }
                            if ( iniObjects.Objects[a].PropulsionCode != "" )
                            {
                                droidDesign.Propulsion = App.ObjectData.FindOrCreatePropulsion(iniObjects.Objects[a].PropulsionCode);
                                if ( droidDesign.Propulsion.IsUnknown )
                                {
                                    droidComponentUnknownCount++;
                                }
                            }
                            droidDesign.TurretCount = (byte)(iniObjects.Objects[a].TurretCount);
                            if ( iniObjects.Objects[a].TurretCodes[0] != "" )
                            {
                                droidDesign.Turret1 = App.ObjectData.FindOrCreateTurret(iniObjects.Objects[a].TurretTypes[0],
                                    Convert.ToString(iniObjects.Objects[a].TurretCodes[0]));
                                if ( droidDesign.Turret1.IsUnknown )
                                {
                                    droidComponentUnknownCount++;
                                }
                            }
                            if ( iniObjects.Objects[a].TurretCodes[1] != "" )
                            {
                                droidDesign.Turret2 = App.ObjectData.FindOrCreateTurret(iniObjects.Objects[a].TurretTypes[1],
                                    Convert.ToString(iniObjects.Objects[a].TurretCodes[1]));
                                if ( droidDesign.Turret2.IsUnknown )
                                {
                                    droidComponentUnknownCount++;
                                }
                            }
                            if ( iniObjects.Objects[a].TurretCodes[2] != "" )
                            {
                                droidDesign.Turret3 = App.ObjectData.FindOrCreateTurret(iniObjects.Objects[a].TurretTypes[2],
                                    Convert.ToString(iniObjects.Objects[a].TurretCodes[2]));
                                if ( droidDesign.Turret3.IsUnknown )
                                {
                                    droidComponentUnknownCount++;
                                }
                            }
                            droidDesign.UpdateAttachments();
                            unitTypeBase = droidDesign;
                        }
                        else
                        {
                            unitTypeBase = App.ObjectData.FindOrCreateUnitType(iniObjects.Objects[a].Code, iniObjects.Objects[a].Type, iniObjects.Objects[a].WallType);
                            if ( unitTypeBase.IsUnknown )
                            {
                                if ( unknownUnitTypeCount < maxUnknownUnitTypeWarningCount )
                                {
                                    returnResult.WarningAdd("\"{0}\" is ont a loaded object.".Format2(iniObjects.Objects[a].Code));
                                }
                                unknownUnitTypeCount++;
                            }
                        }
                    }
                    if ( unitTypeBase == null )
                    {
                        objectTypeMissingCount++;
                    }
                    else
                    {
                        newObject = new clsUnit();
                        newObject.TypeBase = unitTypeBase;
                        newObject.Pos.Horizontal.X = iniObjects.Objects[a].Pos.X;
                        newObject.Pos.Horizontal.Y = iniObjects.Objects[a].Pos.Y;
                        newObject.Health = iniObjects.Objects[a].Health;
                        newObject.SavePriority = iniObjects.Objects[a].Priority;
                        newObject.Rotation = (int)(iniObjects.Objects[a].Heading);
                        if ( newObject.Rotation >= 360 )
                        {
                            newObject.Rotation -= 360;
                        }
                        if ( iniObjects.Objects[a].UnitGroup == null || iniObjects.Objects[a].UnitGroup == "" )
                        {
                            if ( iniObjects.Objects[a].Type != UnitType.Feature )
                            {
                                objectPlayerNumInvalidCount++;
                            }
                            newObject.UnitGroup = map.ScavengerUnitGroup;
                        }
                        else
                        {
                            if ( iniObjects.Objects[a].UnitGroup.ToLower() == "scavenger" )
                            {
                                newObject.UnitGroup = map.ScavengerUnitGroup;
                            }
                            else
                            {
                                UInt32 PlayerNum = 0;
                                try
                                {
                                    if ( !IOUtil.InvariantParse(iniObjects.Objects[a].UnitGroup, ref PlayerNum) )
                                    {
                                        throw (new Exception());
                                    }
                                    if ( PlayerNum < Constants.PlayerCountMax )
                                    {
                                        unitGroup = map.UnitGroups[Convert.ToInt32(PlayerNum)];
                                    }
                                    else
                                    {
                                        unitGroup = map.ScavengerUnitGroup;
                                        objectPlayerNumInvalidCount++;
                                    }
                                }
                                catch ( Exception )
                                {
                                    objectPlayerNumInvalidCount++;
                                    unitGroup = map.ScavengerUnitGroup;
                                }
                                newObject.UnitGroup = unitGroup;
                            }
                        }
                        if ( iniObjects.Objects[a].ID == 0U )
                        {
                            iniObjects.Objects[a].ID = availableID;
                            App.ZeroIDWarning(newObject, iniObjects.Objects[a].ID, returnResult);
                        }
                        unitAdd.NewUnit = newObject;
                        unitAdd.ID = iniObjects.Objects[a].ID;
                        unitAdd.Label = iniObjects.Objects[a].Label;
                        unitAdd.Perform();
                        App.ErrorIDChange(iniObjects.Objects[a].ID, newObject, "Read_FMap_Objects");
                        if ( availableID == iniObjects.Objects[a].ID )
                        {
                            availableID = newObject.ID + 1U;
                        }
                    }
                }
            }

            if ( unknownUnitTypeCount > maxUnknownUnitTypeWarningCount )
            {
                returnResult.WarningAdd(unknownUnitTypeCount + " objects were not in the loaded object data.");
            }
            if ( objectTypeMissingCount > 0 )
            {
                returnResult.WarningAdd(objectTypeMissingCount + " objects did not specify a type and were ignored.");
            }
            if ( droidComponentUnknownCount > 0 )
            {
                returnResult.WarningAdd(droidComponentUnknownCount + " components used by droids were loaded as unknowns.");
            }
            if ( objectPlayerNumInvalidCount > 0 )
            {
                returnResult.WarningAdd(objectPlayerNumInvalidCount + " objects had an invalid player number and were set to player 0.");
            }
            if ( objectPosInvalidCount > 0 )
            {
                returnResult.WarningAdd(objectPosInvalidCount + " objects had a position that was off-map and were ignored.");
            }
            if ( designTypeUnspecifiedCount > 0 )
            {
                returnResult.WarningAdd(designTypeUnspecifiedCount + " designed droids did not specify a template droid type and were set to droid.");
            }

            return returnResult;
        }

        private clsResult read_FMap_Gateways(StreamReader file)
        {
            var ReturnResult = new clsResult("Reading gateways", false);
            logger.Info("Reading gateways");

            var GatewaysINI = new IniReader();
            ReturnResult.Take(GatewaysINI.ReadFile(file));

            var INIGateways = new FMapIniGateways(GatewaysINI.Sections.Count);
            ReturnResult.Take(GatewaysINI.Translate(INIGateways));

            var A = 0;
            var InvalidGatewayCount = 0;

            for ( A = 0; A <= INIGateways.GatewayCount - 1; A++ )
            {
                if ( map.GatewayCreate(INIGateways.Gateways[A].PosA, INIGateways.Gateways[A].PosB) == null )
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

        private clsResult read_FMap_TileTypes(BinaryReader file)
        {
            var returnResult = new clsResult("Reading tile types", false);
            logger.Info("Reading tile types");

            var a = 0;
            byte byteTemp = 0;
            var invalidTypeCount = 0;

            try
            {
                if ( map.Tileset != null )
                {
                    for ( a = 0; a <= map.Tileset.TileCount - 1; a++ )
                    {
                        byteTemp = file.ReadByte();
                        if ( byteTemp >= App.TileTypes.Count )
                        {
                            invalidTypeCount++;
                        }
                        else
                        {
                            map.Tile_TypeNum[a] = byteTemp;
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                returnResult.ProblemAdd(ex.Message);
                return returnResult;
            }

            if ( invalidTypeCount > 0 )
            {
                returnResult.WarningAdd(invalidTypeCount + " tile types were invalid.");
            }

            if ( file.PeekChar() >= 0 )
            {
                returnResult.WarningAdd("There were unread bytes at the end of the file.");
            }

            return returnResult;
        }

        private clsResult read_INI_Labels(string iniText)
        {
            var resultObject = new clsResult("Reading labels", false);
            logger.Info("Reading labels.");

            var typeNum = 0;
            var NewPosition = default(clsScriptPosition);
            var NewArea = default(clsScriptArea);
            var nameText = "";
            var strLabel = "";
            var strPosA = "";
            var strPosB = "";
            var idText = "";
            UInt32 idNum = 0;
            XYInt xyIntA = null;
            XYInt xyIntB = null;

            var failedCount = 0;
            var modifiedCount = 0;

            try
            {
                var iniSections = Core.Parsers.Ini.IniReader.ReadString(iniText);
                foreach ( var iniSection in iniSections )
                {
                    var idx = iniSection.Name.IndexOf('_');
                    if ( idx > 0 )
                    {
                        nameText = iniSection.Name.Substring(0, idx);
                    }
                    else
                    {
                        nameText = iniSection.Name;
                    }
                    switch ( nameText )
                    {
                        case "position":
                            typeNum = 0;
                            break;
                        case "area":
                            typeNum = 1;
                            break;
                        case "object":
                            typeNum = int.MaxValue;
                            failedCount++;
                            continue;
                        default:
                            typeNum = int.MaxValue;
                            failedCount++;
                            continue;
                    }

                    // Raised an exception if nothing was found
                    try
                    {
                        strLabel = iniSection.Data.Where(d => d.Name == "label").First().Data;
                    }
                    catch ( Exception ex )
                    {
                        resultObject.WarningAdd(string.Format("Failed to parse \"label\", error was: {0}", ex.Message));
                        logger.WarnException("Failed to parse \"label\", error was", ex);
                        failedCount++;
                        continue;
                    }
                    strLabel = strLabel.Replace("\"", "");

                    switch ( typeNum )
                    {
                    case 0: //position
                        strPosA = iniSection.Data.Where(d => d.Name == "pos").First().Data;
                        if ( strPosA == null )
                        {
                            failedCount++;
                            continue;
                        }
                        try
                        {
                            xyIntA = XYInt.FromString(strPosA);
                            NewPosition = new clsScriptPosition(map);
                            NewPosition.PosX = xyIntA.X;
                            NewPosition.PosY = xyIntA.Y;
                            NewPosition.SetLabel(strLabel);
                            if ( NewPosition.Label != strLabel ||
                                 NewPosition.PosX != xyIntA.X || NewPosition.PosY != xyIntA.Y )
                            {
                                modifiedCount++;
                            }
                        }
                        catch ( Exception ex )
                        {
                            resultObject.WarningAdd(string.Format("Failed to parse \"pos\", error was: {0}", ex.Message));
                            logger.WarnException("Failed to parse \"pos\", error was", ex);
                            failedCount++;
                        }
                        break;
                    case 1: //area
                        try
                        {
                            strPosA = iniSection.Data.Where(d => d.Name == "pos1").First().Data;
                            strPosB = iniSection.Data.Where(d => d.Name == "pos2").First().Data;

                            xyIntA = XYInt.FromString(strPosA);
                            xyIntB = XYInt.FromString(strPosA);
                            NewArea = new clsScriptArea(map);
                            NewArea.SetPositions(xyIntA, xyIntB);
                            NewArea.SetLabel(strLabel);
                            if ( NewArea.Label != strLabel || NewArea.PosAX != xyIntA.X | NewArea.PosAY != xyIntA.Y
                                 | NewArea.PosBX != xyIntB.X | NewArea.PosBY != xyIntB.Y )
                            {
                                modifiedCount++;
                            }
                        }
                        catch ( Exception ex )
                        {
                            Debugger.Break();
                            resultObject.WarningAdd(string.Format("Failed to parse \"pos1\" or \"pos2\", error was: {0}", ex.Message));
                            logger.WarnException("Failed to parse \"pos1\" or \"pos2\".", ex);
                            failedCount++;
                        }
                        break;
                    case 2: //object
                        idText = iniSection.Data.Where(d => d.Name == "id").First().Data;
                        if ( IOUtil.InvariantParse(idText, ref idNum) )
                        {
                            var Unit = map.IDUsage(idNum);
                            if ( Unit != null )
                            {
                                if ( !Unit.SetLabel(strLabel).Success )
                                {
                                    failedCount++;
                                }
                            }
                            else
                            {
                                failedCount++;
                            }
                        }
                        break;
                    default:
                        resultObject.WarningAdd("Error! Bad type number for script label.");
                        break;
                    }
                }
            }
            catch ( Exception ex )
            {
                Debugger.Break();
                logger.ErrorException("Got exception while reading labels.ini", ex);
                resultObject.ProblemAdd(string.Format("Got exception: {0}", ex.Message), false);
                return resultObject;
            }

            if ( failedCount > 0 )
            {
                resultObject.WarningAdd(string.Format("Unable to translate {0} script labels.", failedCount));
            }
            if ( modifiedCount > 0 )
            {
                resultObject.WarningAdd(string.Format("{0} script labels had invalid values and were modified.", modifiedCount));
            }

            return resultObject;
        }
    }
}