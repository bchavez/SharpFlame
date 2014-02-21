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
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Core.Interfaces.Mapping.IO;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Mapping.Tiles;

#endregion

namespace SharpFlame.Mapping.IO.FMap
{
    public class FMapLoader : IIOLoader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly Map map;

        public FMapLoader(Map newMap)
        {
            map = newMap;
        }

        public Result Load(string path)
        {
            var returnResult = new Result(string.Format("Loading FMap from \"{0}\"", path), false);
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
                using (var reader = new StreamReader(infoIniEntry.OpenReader()))
                {
                    var text = reader.ReadToEnd ();
                    resultInfo = new FMapInfo ();
                    returnResult.Add(read_FMap_Info(text, ref resultInfo));
                    if (returnResult.HasProblems)
                    {
                        return returnResult;
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
                }              

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
                    using ( var reader = new StreamReader(obEntry.OpenReader()) )
                    {
                        var text = reader.ReadToEnd();
                        returnResult.Add(read_FMap_Objects(text));
                    }
                }

                // gateways.ini
                var gaEntry = zip["gateways.ini"]; // Case insensetive.
                if ( gaEntry == null )
                {
                    returnResult.ProblemAdd("Unable to find file \"gateways.ini\".");
                    return returnResult;
                }
                using ( var reader = new StreamReader(gaEntry.OpenReader()) )
                {
                    var text = reader.ReadToEnd();
                    returnResult.Add(read_FMap_Gateways(text));
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

        private Result read_FMap_Info(string text, ref FMapInfo resultInfo)
        {
            var returnResult = new Result("Read general map info", false);
            logger.Info("Read general map info");

            var infoIni = IniReader.ReadString (text);

            var invalid = true;
            foreach (var iniSection in infoIni) {
                invalid = false;

                foreach (var iniToken in iniSection.Data) {
                    if (invalid) {
                        break;
                    }

                    try {
                        switch (iniToken.Name.ToLower()) {
                        case "tileset":
                            switch (iniToken.Data.ToLower()) {
                            case "arizona":
                                resultInfo.Tileset = App.Tileset_Arizona;
                                break;
                            case "urban":
                                resultInfo.Tileset = App.Tileset_Urban;
                                break;
                            case "rockies":
                                resultInfo.Tileset = App.Tileset_Rockies;
                                break;
                            default:
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            break;
                        case "size":
                            resultInfo.TerrainSize = XYInt.FromString(iniToken.Data);
                            if (resultInfo.TerrainSize.X < 1 || resultInfo.TerrainSize.Y < 1 || resultInfo.TerrainSize.X > Constants.MapMaxSize || resultInfo.TerrainSize.Y > Constants.MapMaxSize) {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            break;
                        case "autoscrolllimits":
                            resultInfo.InterfaceOptions.AutoScrollLimits = bool.Parse(iniToken.Data);
                            break;
                        case "scrollminx":
                            resultInfo.InterfaceOptions.ScrollMin.X = int.Parse(iniToken.Data);
                            break;
                        case "scrollminy":
                            resultInfo.InterfaceOptions.ScrollMin.Y = int.Parse(iniToken.Data);
                            break;
                        case "scrollmaxx":
                            resultInfo.InterfaceOptions.ScrollMax.X = uint.Parse(iniToken.Data);
                            break;
                        case "scrollmaxy":
                            resultInfo.InterfaceOptions.ScrollMax.Y = uint.Parse(iniToken.Data);
                            break;
                        case "name":
                            resultInfo.InterfaceOptions.CompileName = iniToken.Data;
                            break;
                        case "players":
                            resultInfo.InterfaceOptions.CompileMultiAuthor = iniToken.Data;
                            break;
                        case "xplayerlev":
                            // ignored.
                            break;
                        case "author":
                            resultInfo.InterfaceOptions.CompileMultiAuthor = iniToken.Data;
                            break;
                        case "license":
                            resultInfo.InterfaceOptions.CompileMultiLicense = iniToken.Data;
                            break;
                        case "camptime":
                            // ignore
                            break;
                        case "camptype":
                            resultInfo.InterfaceOptions.CampaignGameType = int.Parse(iniToken.Data);
                            break;
                        }
                    }
                    catch (Exception ex) {
                        invalid = true;
                        returnResult.WarningAdd(
                            string.Format("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                        logger.WarnException(string.Format("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);
                    }
                }
            }

            if ( resultInfo.TerrainSize.X < 0 | resultInfo.TerrainSize.Y < 0 )
            {
                returnResult.ProblemAdd("Map size was not specified or was invalid.");
            }

            return returnResult;
        }

        private Result read_FMap_VertexHeight(BinaryReader file)
        {
            var returnResult = new Result("Reading vertex heights", false);
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

        private Result read_FMap_VertexTerrain(BinaryReader file)
        {
            var returnResult = new Result("Reading vertex terrain", false);
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

        private Result read_FMap_TileTexture(BinaryReader file)
        {
            var returnResult = new Result("Reading tile textures", false);
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

        private Result read_FMap_TileOrientation(BinaryReader file)
        {
            var returnResult = new Result("Reading tile orientations", false);
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

        private Result read_FMap_TileCliff(BinaryReader file)
        {
            var returnResult = new Result("Reading tile cliffs", false);
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

        private Result read_FMap_Roads(BinaryReader file)
        {
            var returnResult = new Result("Reading roads", false);
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

        private Result read_FMap_Objects(string text)
        {
            var returnResult = new Result("Reading objects", false);
            logger.Info("Reading objects");

            var objectsINI = IniReader.ReadString(text);

            var droidComponentUnknownCount = 0;
            var objectTypeMissingCount = 0;
            var objectPlayerNumInvalidCount = 0;
            var objectPosInvalidCount = 0;
            var designTypeUnspecifiedCount = 0;
            var unknownUnitTypeCount = 0;
            var maxUnknownUnitTypeWarningCount = 16;

            var droidDesign = default(DroidDesign);
            var newObject = default(Unit);
            var unitAdd = new clsUnitAdd();
            var unitTypeBase = default(UnitTypeBase);
            var isDesign = default(bool);
            var unitGroup = default(clsUnitGroup);
            var zeroPos = new XYInt(0, 0);
            UInt32 availableID = 0;

            unitAdd.Map = map;

            INIObject iniObject = default(INIObject);
            var invalid = true;
            var iniObjects = new List<INIObject> ();
           
            foreach (var iniSection in objectsINI) {
                iniObject = new INIObject ();
                iniObject.Type = UnitType.Unspecified;
                iniObject.Health = 1.0D;
                iniObject.WallType = -1;
                iniObject.TurretCodes = new string[Constants.MaxDroidWeapons];
                iniObject.TurretTypes = new TurretType[Constants.MaxDroidWeapons];
                for (var i = 0; i < Constants.MaxDroidWeapons; i++) {
                    iniObject.TurretTypes [i] = TurretType.Unknown;
                    iniObject.TurretCodes [i] = "";
                }
                invalid = false;
                foreach (var iniToken in iniSection.Data) {
                    if (invalid)
                    {
                        break;
                    }

                    try {
                        switch (iniToken.Name.ToLower()) {
                        case "type":
                            var typeTokens = iniToken.Data.Split (new string[] { ", " }, StringSplitOptions.None);
                            if (typeTokens.Length < 1) {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} type \"{1}\" is not valid.", iniSection.Name, iniToken.Data));
                                logger.Warn ("#{0} type \"{1}\" is not valid.", iniSection.Name, iniToken.Data);
                                continue;
                            }
                            switch (typeTokens [0].ToLower ()) {
                            case "feature":
                                iniObject.Type = UnitType.Feature;
                                iniObject.Code = typeTokens [1];
                                break;
                            case "structure":
                                iniObject.Type = UnitType.PlayerStructure;
                                iniObject.Code = typeTokens [1];
                                break;
                            case "droidtemplate":
                                iniObject.Type = UnitType.PlayerDroid;
                                iniObject.IsTemplate = true;
                                iniObject.Code = typeTokens [1];
                                break;
                            case "droiddesign":
                                iniObject.Type = UnitType.PlayerDroid;
                                break;
                            default:
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            break;
                        case "droidtype":
                            var droidType = App.GetTemplateDroidTypeFromTemplateCode (iniToken.Data);
                            if (droidType == null) {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            break;
                        case "body":
                            iniObject.BodyCode = iniToken.Data;
                            break;
                        case "propulsion":
                            iniObject.PropulsionCode = iniToken.Data;
                            break;
                        case "turrentcount":
                            iniObject.TurretCount = int.Parse(iniToken.Data);
                            if (iniObject.TurretCount < 0 || iniObject.TurretCount > Constants.MaxDroidWeapons) {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            break;
                        case "turret1":
                            var turret1Tokens = iniToken.Data.Split (new string[] { ", " }, StringSplitOptions.None);
                            if (turret1Tokens.Length < 2) {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            iniObject.TurretTypes[0] = App.GetTurretTypeFromName(turret1Tokens[0]);
                            iniObject.TurretCodes[1] = turret1Tokens[1];
                            break;
                        case "turret2":
                            var turret2Tokens = iniToken.Data.Split (new string[] { ", " }, StringSplitOptions.None);
                            if (turret2Tokens.Length < 2) {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            iniObject.TurretTypes[0] = App.GetTurretTypeFromName(turret2Tokens[0]);
                            iniObject.TurretCodes[1] = turret2Tokens[1];
                            break;
                        case "turret3":
                            var turret3Tokens = iniToken.Data.Split (new string[] { ", " }, StringSplitOptions.None);
                            if (turret3Tokens.Length < 2) {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            iniObject.TurretTypes[0] = App.GetTurretTypeFromName(turret3Tokens[0]);
                            iniObject.TurretCodes[1] = turret3Tokens[1];
                            break;
                        case "id":
                            iniObject.ID = uint.Parse(iniToken.Data);
                            break;
                        case "priority":
                            iniObject.Priority = int.Parse(iniToken.Data);
                            break;
                        case "pos":
                            iniObject.Pos = XYInt.FromString(iniToken.Data);
                            break;
                        case "heading":
                            if ( !IOUtil.InvariantParse(iniToken.Data, ref iniObject.Heading) )
                            {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            break;
                        case "unitgroup":
                            iniObject.UnitGroup = iniToken.Data;
                            break;
                        case "health":
                            if ( !IOUtil.InvariantParse(iniToken.Data, ref iniObject.Health) )
                            {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            break;
                        case "walltype":
                            if ( !IOUtil.InvariantParse(iniToken.Data, ref iniObject.WallType) )
                            {
                                invalid = true;
                                returnResult.WarningAdd (string.Format ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data));
                                logger.Warn ("#{0} {1} \"{2}\" is not valid.", iniSection.Name, iniToken.Name, iniToken.Data);
                                continue;
                            }
                            if (iniObject.WallType < 0 || iniObject.WallType > 3) {
                                iniObject.WallType = -1;
                            }
                            break;
                        case "scriptlabel":
                            iniObject.Label = iniToken.Data;
                            break;
                        default:
                            returnResult.WarningAdd(string.Format("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data), false);
                            logger.Warn("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                            break;
                        }
                    }
                    catch (Exception ex) {
                        invalid = true;
                        returnResult.WarningAdd(
                            string.Format("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                        logger.WarnException(string.Format("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);
                    }
                }

                if (!invalid) {
                    iniObjects.Add (iniObject);
                }
            }

            try {
                availableID = iniObjects.Max (w => w.ID) + 10;
                foreach (var iniObject2 in iniObjects) 
                {
                    if ( iniObject2.Pos == null )
                    {
                        objectPosInvalidCount++;
                        continue;
                    }
                    else if ( !App.PosIsWithinTileArea(iniObject2.Pos, zeroPos, map.Terrain.TileSize) )
                    {
                        objectPosInvalidCount++;
                        continue;
                    }

                    unitTypeBase = null;
                    if ( iniObject2.Type != UnitType.Unspecified )
                    {
                        isDesign = false;
                        if ( iniObject2.Type == UnitType.PlayerDroid )
                        {
                            if ( !iniObject2.IsTemplate )
                            {
                                isDesign = true;
                            }
                        }
                        if ( isDesign )
                        {
                            droidDesign = new DroidDesign();
                            droidDesign.TemplateDroidType = iniObject2.TemplateDroidType;
                            if ( droidDesign.TemplateDroidType == null )
                            {
                                droidDesign.TemplateDroidType = App.TemplateDroidType_Droid;
                                designTypeUnspecifiedCount++;
                            }
                            if ( iniObject2.BodyCode != null )
                            {
                                droidDesign.Body = App.ObjectData.FindOrCreateBody(Convert.ToString(iniObject2.BodyCode));
                                if ( droidDesign.Body.IsUnknown )
                                {
                                    droidComponentUnknownCount++;
                                }
                            }
                            if ( iniObject2.PropulsionCode != null )
                            {
                                droidDesign.Propulsion = App.ObjectData.FindOrCreatePropulsion(iniObject2.PropulsionCode);
                                if ( droidDesign.Propulsion.IsUnknown )
                                {
                                    droidComponentUnknownCount++;
                                }
                            }
                            droidDesign.TurretCount = (byte)(iniObject2.TurretCount);
                            if ( iniObject2.TurretCodes[0] != "" )
                            {
                                droidDesign.Turret1 = App.ObjectData.FindOrCreateTurret(iniObject2.TurretTypes[0],
                                    Convert.ToString(iniObject2.TurretCodes[0]));
                                if ( droidDesign.Turret1.IsUnknown )
                                {
                                    droidComponentUnknownCount++;
                                }
                            }
                            if ( iniObject2.TurretCodes[1] != "" )
                            {
                                droidDesign.Turret2 = App.ObjectData.FindOrCreateTurret(iniObject2.TurretTypes[1],
                                    Convert.ToString(iniObject2.TurretCodes[1]));
                                if ( droidDesign.Turret2.IsUnknown )
                                {
                                    droidComponentUnknownCount++;
                                }
                            }
                            if ( iniObject2.TurretCodes[2] != "" )
                            {
                                droidDesign.Turret3 = App.ObjectData.FindOrCreateTurret(iniObject2.TurretTypes[2],
                                    Convert.ToString(iniObject2.TurretCodes[2]));
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
                            unitTypeBase = App.ObjectData.FindOrCreateUnitType(iniObject2.Code, iniObject2.Type, iniObject2.WallType);
                            if ( unitTypeBase.IsUnknown )
                            {
                                if ( unknownUnitTypeCount < maxUnknownUnitTypeWarningCount )
                                {
                                    returnResult.WarningAdd("\"{0}\" is not a loaded object.".Format2(iniObject2.Code));
                                }
                                unknownUnitTypeCount++;
                            }
                        }
                    }
                    if ( unitTypeBase == null )
                    {
                        objectTypeMissingCount++;
                        continue;
                    }

                    newObject = new Unit();
                    newObject.TypeBase = unitTypeBase;
                    newObject.Pos.Horizontal.X = iniObject2.Pos.X;
                    newObject.Pos.Horizontal.Y = iniObject2.Pos.Y;
                    newObject.Health = iniObject2.Health;
                    newObject.SavePriority = iniObject2.Priority;
                    newObject.Rotation = (int)(iniObject2.Heading);
                    if ( newObject.Rotation >= 360 )
                    {
                        newObject.Rotation -= 360;
                    }
                    if ( iniObject2.UnitGroup == null || iniObject2.UnitGroup == "" )
                    {
                        if ( iniObject2.Type != UnitType.Feature )
                        {
                            objectPlayerNumInvalidCount++;
                        }
                        newObject.UnitGroup = map.ScavengerUnitGroup;
                    }
                    else
                    {
                        if ( iniObject2.UnitGroup.ToLower() == "scavenger" )
                        {
                            newObject.UnitGroup = map.ScavengerUnitGroup;
                        }
                        else
                        {
                            UInt32 PlayerNum = 0;
                            try
                            {
                                if ( !IOUtil.InvariantParse(iniObject2.UnitGroup, ref PlayerNum) )
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
                    if ( iniObject2.ID == 0U )
                    {
                        // iniObject2.ID = availableID;
                        App.ZeroIDWarning(newObject, iniObject2.ID, returnResult);
                    }
                    unitAdd.NewUnit = newObject;
                    unitAdd.ID = iniObject2.ID;
                    unitAdd.Label = iniObject2.Label;
                    unitAdd.Perform();
                    App.ErrorIDChange(iniObject2.ID, newObject, "Read_FMap_Objects");
                    if ( availableID == iniObject2.ID )
                    {
                        availableID = newObject.ID + 1U;
                    }
                }

                if ( unknownUnitTypeCount > maxUnknownUnitTypeWarningCount )
                {
                    returnResult.WarningAdd (string.Format ("{0} objects were not in the loaded object data.", unknownUnitTypeCount));
                }
                if ( objectTypeMissingCount > 0 )
                {
                    returnResult.WarningAdd (string.Format ("{0} objects did not specify a type and were ignored.", objectTypeMissingCount));
                }
                if ( droidComponentUnknownCount > 0 )
                {
                    returnResult.WarningAdd (string.Format ("{0} components used by droids were loaded as unknowns.", droidComponentUnknownCount));
                }
                if ( objectPlayerNumInvalidCount > 0 )
                {
                    returnResult.WarningAdd (string.Format ("{0} objects had an invalid player number and were set to player 0.", objectPlayerNumInvalidCount));
                }
                if ( objectPosInvalidCount > 0 )
                {
                    returnResult.WarningAdd (string.Format ("{0} objects had a position that was off-map and were ignored.", objectPosInvalidCount));
                }
                if ( designTypeUnspecifiedCount > 0 )
                {
                    returnResult.WarningAdd (string.Format ("{0} designed droids did not specify a template droid type and were set to droid.", designTypeUnspecifiedCount));
                }
            }
            catch (Exception ex) {
                Debugger.Break ();
                returnResult.ProblemAdd (string.Format("Got a exeption: {0}", ex.Message));
                logger.ErrorException ("Got exception", ex);
            }

            return returnResult;
        }

        private Result read_FMap_Gateways(string text)
        {
            var returnResult = new Result("Reading gateways", false);
            logger.Info("Reading gateways");

            var invalid = true;
            var ini = IniReader.ReadString (text);
            var iniGateway = default(INIGateway);
            var iniGateways = new List<INIGateway> ();
            foreach (var iniSection in ini) {
                invalid = false;
                iniGateway = new INIGateway ();           
                foreach (var iniToken in iniSection.Data) {
                    if (invalid) {
                        break;
                    }

                    try {
                        switch (iniToken.Name) {
                        case "ax":
                            iniGateway.PosA.X = int.Parse(iniToken.Data);
                            break;
                        case "ay":
                            iniGateway.PosA.Y = int.Parse(iniToken.Data);
                            break;
                        case "by":
                            iniGateway.PosB.Y = int.Parse(iniToken.Data);
                            break;
                        case "bx":
                            iniGateway.PosB.X = int.Parse(iniToken.Data);
                            break;
                        default:
                            returnResult.WarningAdd(string.Format("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data), false);
                            logger.Warn("Found an invalid key: {0} = {1}", iniToken.Name, iniToken.Data);
                            break;
                        }
                    }
                    catch (Exception ex) {
                        invalid = true;
                        returnResult.WarningAdd(
                            string.Format("#{0} invalid {2}: \"{3}\", got exception: {2}", iniSection.Name, iniToken.Name, iniToken.Data, ex.Message), false);
                        logger.WarnException(string.Format("#{0} invalid {2} \"{1}\"", iniSection.Name, iniToken.Name, iniToken.Data), ex);
                    }
                }

                if (!invalid) {
                    iniGateways.Add(iniGateway);
                }
            }


            var invalidGatewayCount = 0;

            foreach (var iniGateway2 in iniGateways) {
                if ( map.GatewayCreate(iniGateway2.PosA, iniGateway2.PosB) == null ) {
                    invalidGatewayCount++;
                }
            }

            if ( invalidGatewayCount > 0 )
            {
                returnResult.WarningAdd(invalidGatewayCount + " gateways were invalid.");
            }

            return returnResult;
        }

        private Result read_FMap_TileTypes(BinaryReader file)
        {
            var returnResult = new Result("Reading tile types", false);
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
                            map.TileTypeNum[a] = byteTemp;
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

        private Result read_INI_Labels(string iniText)
        {
            var resultObject = new Result("Reading labels", false);
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