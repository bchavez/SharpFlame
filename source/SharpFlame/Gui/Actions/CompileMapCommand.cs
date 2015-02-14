using System;
using Eto.Forms;
using Ninject;
using NLog;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Domain;
using SharpFlame.Gui.Dialogs;
using SharpFlame.Gui.Forms;
using SharpFlame.Mapping;
using SharpFlame.Mapping.IO.Wz;
using SharpFlame.Mapping.Objects;
using SharpFlame.Maths;

namespace SharpFlame.Gui.Actions
{
    public class CompileMapCommand : Command
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly MainForm form;
        //hack to get reference.
        private Map Map
        {
            get
            {
                return form.MainMapView.MainMap;
            }
        }

        public CompileMapCommand(MainForm form)
        {
            this.form = form;
            ID = "compile";
            MenuText = "&Compile Map ...";
            ToolBarText = "&Compile Map ...";
            ToolTip = "Compile Map";
            Shortcut = Keys.C | Application.Instance.CommonModifier;
        }

        protected override void OnExecuted(EventArgs e)
        {
            base.OnExecuted(e);
            
            if( Map == null )
                return;
            if( Map.CompileScreen != null )
                return;

            var compileMap = new CompileMapDialog();
            Map.CompileScreen = compileMap;
            compileMap.DataContext = Map.InterfaceOptions;

            var options = compileMap.ShowModal();

            Map.SetChanged();

            if( options == null )
            {
                Map.CompileScreen = null;
                //canceled.
                return;
            }

            if( options.CompileType == CompileType.Campaign )
            {
                CompileCampaign();
            }
            else
            {
                CompileMultiPlayer();
            }
        }

        private Result ValidateMap_UnitPositions()
        {
            var Result = new Result("Validate unit positions", false);
            logger.Info("Validate unit positions");

            //check unit positions

            var TileHasUnit = new bool[Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y];
            var tileStructureTypeBase = new StructureTypeBase[Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y];
            var tileFeatureTypeBase = new FeatureTypeBase[Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y];
            var TileObjectGroup = new clsUnitGroup[Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y];

            var StartPos = new XYInt();
            var FinishPos = new XYInt();
            var CentrePos = new XYInt();
            StructureType StructureTypeType;
            StructureTypeBase structureTypeBase;
            var Footprint = new XYInt();
            var UnitIsStructureModule = new bool[Map.Units.Count];
            bool IsValid;

            foreach( var unit in Map.Units )
            {
                if( unit.TypeBase.Type == UnitType.PlayerStructure )
                {
                    structureTypeBase = (StructureTypeBase)unit.TypeBase;
                    StructureTypeType = structureTypeBase.StructureType;
                    UnitIsStructureModule[unit.MapLink.ArrayPosition] = structureTypeBase.IsModule() |
                                                                        StructureTypeType == StructureType.ResourceExtractor;
                }
            }
            //check and store non-module units first. modules need to check for the underlying unit.
            foreach( var unit in Map.Units )
            {
                if( !UnitIsStructureModule[unit.MapLink.ArrayPosition] )
                {
                    Footprint = unit.TypeBase.GetGetFootprintSelected(unit.Rotation);
                    Map.GetFootprintTileRange(unit.Pos.Horizontal, Footprint, ref StartPos, ref FinishPos);
                    if( StartPos.X < 0 | FinishPos.X >= Map.Terrain.TileSize.X
                         | StartPos.Y < 0 | FinishPos.Y >= Map.Terrain.TileSize.Y )
                    {
                        var resultItem = modResults.CreateResultProblemGotoForObject(unit);
                        resultItem.Text = string.Format("Unit off map at position {0}.", unit.GetPosText());
                        Result.ItemAdd(resultItem);
                    }
                    else
                    {
                        for( var y = StartPos.Y; y <= FinishPos.Y; y++ )
                        {
                            for( var x = StartPos.X; x <= FinishPos.X; x++ )
                            {
                                if( TileHasUnit[x, y] )
                                {
                                    var resultItem = modResults.CreateResultProblemGotoForObject(unit);
                                    logger.Info("Bad overlap of {0} on tile {1}, {2}.", unit.TypeBase.GetDisplayTextName(), x, y);
                                    resultItem.Text = string.Format("Bad unit overlap of {0} on tile {1}, {2}.", unit.TypeBase.GetDisplayTextName(), x, y);
                                    Result.ItemAdd(resultItem);
                                }
                                else
                                {
                                    logger.Debug("{0} on X:{1}, Y:{2} tile.", unit.TypeBase.GetDisplayTextName(), x, y);

                                    TileHasUnit[x, y] = true;
                                    if( unit.TypeBase.Type == UnitType.PlayerStructure )
                                    {
                                        tileStructureTypeBase[x, y] = (StructureTypeBase)unit.TypeBase;
                                    }
                                    else if( unit.TypeBase.Type == UnitType.Feature )
                                    {
                                        tileFeatureTypeBase[x, y] = (FeatureTypeBase)unit.TypeBase;
                                    }
                                    TileObjectGroup[x, y] = unit.UnitGroup;
                                }
                            }
                        }
                    }
                }
            }
            //check modules and extractors
            foreach( var unit in Map.Units )
            {
                if( UnitIsStructureModule[unit.MapLink.ArrayPosition] )
                {
                    StructureTypeType = ( (StructureTypeBase)unit.TypeBase ).StructureType;
                    CentrePos.X = ( unit.Pos.Horizontal.X / Constants.TerrainGridSpacing );
                    CentrePos.Y = unit.Pos.Horizontal.Y / Constants.TerrainGridSpacing;
                    if( CentrePos.X < 0 | CentrePos.X >= Map.Terrain.TileSize.X
                         | CentrePos.Y < 0 | CentrePos.Y >= Map.Terrain.TileSize.Y )
                    {
                        var resultItem = modResults.CreateResultProblemGotoForObject(unit);
                        resultItem.Text = "Module off map at position " + unit.GetPosText() + ".";
                        Result.ItemAdd(resultItem);
                    }
                    else
                    {
                        if( tileStructureTypeBase[CentrePos.X, CentrePos.Y] != null )
                        {
                            if( TileObjectGroup[CentrePos.X, CentrePos.Y] == unit.UnitGroup )
                            {
                                if( StructureTypeType == StructureType.FactoryModule )
                                {
                                    if( tileStructureTypeBase[CentrePos.X, CentrePos.Y].StructureType == StructureType.Factory
                                         | tileStructureTypeBase[CentrePos.X, CentrePos.Y].StructureType == StructureType.VTOLFactory )
                                    {
                                        IsValid = true;
                                    }
                                    else
                                    {
                                        IsValid = false;
                                    }
                                }
                                else if( StructureTypeType == StructureType.PowerModule )
                                {
                                    if( tileStructureTypeBase[CentrePos.X, CentrePos.Y].StructureType == StructureType.PowerGenerator )
                                    {
                                        IsValid = true;
                                    }
                                    else
                                    {
                                        IsValid = false;
                                    }
                                }
                                else if( StructureTypeType == StructureType.ResearchModule )
                                {
                                    if( tileStructureTypeBase[CentrePos.X, CentrePos.Y].StructureType == StructureType.Research )
                                    {
                                        IsValid = true;
                                    }
                                    else
                                    {
                                        IsValid = false;
                                    }
                                }
                                else
                                {
                                    IsValid = false;
                                }
                            }
                            else
                            {
                                IsValid = false;
                            }
                        }
                        else if( tileFeatureTypeBase[CentrePos.X, CentrePos.Y] != null )
                        {
                            if( StructureTypeType == StructureType.ResourceExtractor )
                            {
                                if( tileFeatureTypeBase[CentrePos.X, CentrePos.Y].FeatureType == FeatureType.OilResource )
                                {
                                    IsValid = true;
                                }
                                else
                                {
                                    IsValid = false;
                                }
                            }
                            else
                            {
                                IsValid = false;
                            }
                        }
                        else if( StructureTypeType == StructureType.ResourceExtractor )
                        {
                            IsValid = true;
                        }
                        else
                        {
                            IsValid = false;
                        }
                        if( !IsValid )
                        {
                            var resultItem = modResults.CreateResultProblemGotoForObject(unit);
                            resultItem.Text = "Bad module on tile " + Convert.ToString(CentrePos.X) + ", " + Convert.ToString(CentrePos.Y) + ".";
                            Result.ItemAdd(resultItem);
                        }
                    }
                }
            }

            return Result;
        }

        private Result ValidateMap_Multiplayer(int PlayerCount)
        {
            var Result = new Result("Validate for multiplayer", false);
            logger.Info("Validate for multiplayer");

            if( PlayerCount < 2 | PlayerCount > Constants.PlayerCountMax )
            {
                Result.ProblemAdd("Unable to evaluate for multiplayer due to bad number of players.");
                return Result;
            }

            //check HQs, Trucks and unit counts

            var PlayerHQCount = new int[Constants.PlayerCountMax];
            var Player23TruckCount = new int[Constants.PlayerCountMax];
            var PlayerMasterTruckCount = new int[Constants.PlayerCountMax];
            var DroidType = default(DroidDesign);
            StructureTypeBase structureTypeBase;
            var UnusedPlayerUnitWarningCount = 0;
            var Unit = default(Unit);

            foreach( var tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if( Unit.UnitGroup == Map.ScavengerUnitGroup )
                {
                }
                else
                {
                    if( Unit.TypeBase.Type == UnitType.PlayerDroid )
                    {
                        DroidType = (DroidDesign)Unit.TypeBase;
                        if( DroidType.Body != null && DroidType.Propulsion != null && DroidType.Turret1 != null && DroidType.TurretCount == 1 )
                        {
                            if( DroidType.Turret1.TurretType == TurretType.Construct )
                            {
                                PlayerMasterTruckCount[Unit.UnitGroup.WZ_StartPos]++;
                                if( DroidType.IsTemplate )
                                {
                                    Player23TruckCount[Unit.UnitGroup.WZ_StartPos]++;
                                }
                            }
                        }
                    }
                    else if( Unit.TypeBase.Type == UnitType.PlayerStructure )
                    {
                        structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                        if( structureTypeBase.Code == "A0CommandCentre" )
                        {
                            PlayerHQCount[Unit.UnitGroup.WZ_StartPos]++;
                        }
                    }
                }
                if( Unit.TypeBase.Type != UnitType.Feature )
                {
                    if( Unit.UnitGroup.WZ_StartPos >= PlayerCount )
                    {
                        if( UnusedPlayerUnitWarningCount < 32 )
                        {
                            UnusedPlayerUnitWarningCount++;
                            var resultItem = modResults.CreateResultProblemGotoForObject(Unit);
                            resultItem.Text = string.Format("An unused player ({0}) has a unit at {1}.", Unit.UnitGroup.WZ_StartPos, Unit.GetPosText());
                            Result.ItemAdd(resultItem);
                        }
                    }
                }
            }

            for( var A = 0; A <= PlayerCount - 1; A++ )
            {
                if( PlayerHQCount[A] == 0 )
                {
                    Result.ProblemAdd("There is no Command Centre for player " + Convert.ToString(A) + ".");
                }
                if( PlayerMasterTruckCount[A] == 0 )
                {
                    Result.ProblemAdd("There are no constructor units for player " + Convert.ToString(A) + ".");
                }
                else if( Player23TruckCount[A] == 0 )
                {
                    Result.WarningAdd("All constructor units for player " + Convert.ToString(A) + " will only exist in master.");
                }
            }

            return Result;
        }

        private Result ValidateMap()
        {
            var ReturnResult = new Result("Validate map", false);
            logger.Info("Validate map");

            if( Map.Terrain.TileSize.X > Constants.WzMapMaxSize )
            {
                ReturnResult.WarningAdd("Map width is too large. The maximum is " + Convert.ToString(Constants.WzMapMaxSize) + ".");
            }
            if( Map.Terrain.TileSize.Y > Constants.WzMapMaxSize )
            {
                ReturnResult.WarningAdd("Map height is too large. The maximum is " + Convert.ToString(Constants.WzMapMaxSize) + ".");
            }

            if( Map.Tileset == null )
            {
                ReturnResult.ProblemAdd("No tileset selected.");
            }

            var PlayerStructureTypeCount = new int[Constants.PlayerCountMax, App.ObjectData.StructureTypes.Count];
            var ScavStructureTypeCount = new int[App.ObjectData.StructureTypes.Count];
            var structureTypeBase = default(StructureTypeBase);
            var Unit = default(Unit);

            foreach( var tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if( Unit.TypeBase.Type == UnitType.PlayerStructure )
                {
                    structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                    if( Unit.UnitGroup == Map.ScavengerUnitGroup )
                    {
                        ScavStructureTypeCount[structureTypeBase.StructureType_ObjectDataLink.ArrayPosition]++;
                    }
                    else
                    {
                        PlayerStructureTypeCount[Unit.UnitGroup.WZ_StartPos, structureTypeBase.StructureType_ObjectDataLink.ArrayPosition]++;
                    }
                }
            }

            foreach( var tempLoopVar_StructureType in App.ObjectData.StructureTypes )
            {
                structureTypeBase = tempLoopVar_StructureType;
                var StructureTypeNum = structureTypeBase.StructureType_ObjectDataLink.ArrayPosition;
                var PlayerNum = 0;
                for( PlayerNum = 0; PlayerNum <= Constants.PlayerCountMax - 1; PlayerNum++ )
                {
                    if( PlayerStructureTypeCount[PlayerNum, StructureTypeNum] > 255 )
                    {
                        ReturnResult.ProblemAdd("Player {0} has to  many ({1}) of structure \"{2}\"" +
                                                ". The limit is 255 of any one structure type.".Format2(
                                                    PlayerNum, PlayerStructureTypeCount[PlayerNum, StructureTypeNum],
                                                    structureTypeBase.Code)
                            );
                    }
                }
                if( ScavStructureTypeCount[StructureTypeNum] > 255 )
                {
                    ReturnResult.ProblemAdd("Scavengers have to many ({0}) of structure \"{1}\"" +
                                            ". The limit is 255 of any one structure type.".Format2
                                                (ScavStructureTypeCount[StructureTypeNum], structureTypeBase.Code)
                        );
                }
            }

            return ReturnResult;
        }

        private int ValidateMap_WaterTris()
        {
            var X = 0;
            var Y = 0;
            var Count = 0;

            if( Map.Tileset == null )
            {
                return 0;
            }

            for( Y = 0; Y <= Map.Terrain.TileSize.Y - 1; Y++ )
            {
                for( X = 0; X <= Map.Terrain.TileSize.X - 1; X++ )
                {
                    if( Map.Terrain.Tiles[X, Y].Tri )
                    {
                        if( Map.Terrain.Tiles[X, Y].Texture.TextureNum >= 0 && Map.Terrain.Tiles[X, Y].Texture.TextureNum < Map.Tileset.Tiles.Count )
                        {
                            if( Map.Tileset.Tiles[Map.Terrain.Tiles[X, Y].Texture.TextureNum].DefaultType == Constants.TileTypeNumWater )
                            {
                                Count++;
                            }
                        }
                    }
                }
            }
            return Count;
        }

        private void CompileMultiPlayer()
        {
            var options = Map.InterfaceOptions;

            var result = new Result("Compile multiplayer", false);

            logger.Info("Compile multiplayer");

            var license = options.CompileMultiLicense;
            var playerCount = options.CompileMultiPlayers;
            if( playerCount < 2 | playerCount > Constants.PlayerCountMax )
            {
                result.ProblemAdd(string.Format("The number of players must be from 2 to {0}", Constants.PlayerCountMax));
            }

            var waterErrors = ValidateMap_WaterTris();
            if( waterErrors > 0 )
            {
                result.WarningAdd(string.Format("{0} water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.", waterErrors));
            }

            result.Add(ValidateMap());
            result.Add(ValidateMap_UnitPositions());
            result.Add(ValidateMap_Multiplayer(playerCount));

            var mapName = options.CompileName;
            for( waterErrors = 0; waterErrors <= mapName.Length - 1; waterErrors++ )
            {
                var currentChar = mapName[waterErrors];
                if(
                    !( ( currentChar >= 'a' && currentChar <= 'z' ) || ( currentChar >= 'A' && currentChar <= 'Z' ) ||
                      ( waterErrors >= 1 && ( ( currentChar >= '0' && currentChar <= '9' ) || currentChar == '-' || currentChar == '_' ) ) ) )
                {
                    break;
                }
            }
            if( waterErrors < mapName.Length )
            {
                result.ProblemAdd("The map\'s name must contain only letters, numbers, underscores and hyphens, and must begin with a letter.");
            }
            if( mapName.Length < 1 | mapName.Length > 16 )
            {
                result.ProblemAdd("Map name must be from 1 to 16 characters.");
            }
            if( string.IsNullOrEmpty(license) )
            {
                result.ProblemAdd("Enter a valid license.");
            }
            if( result.HasProblems )
            {
                App.ShowWarnings(result);
                return;
            }
            var saveFileDialog = new SaveFileDialog();
            if( Map.PathInfo != null )
            {
                saveFileDialog.Directory = new Uri(Map.PathInfo.Path);
            }
            saveFileDialog.FileName = playerCount + "c-" + mapName;
            saveFileDialog.Filters = new[] {new FileDialogFilter("WZ Files", ".wz")};
            if( saveFileDialog.ShowDialog(form) != DialogResult.Ok )
            {
                return;
            }

            if( options.AutoScrollLimits )
            {
                SetScrollLimits(ref Map.InterfaceOptions.ScrollMin, ref Map.InterfaceOptions.ScrollMax);
            }

            var wzFormat = App.Kernel.Get<WzSaver>();
            result.Add(wzFormat.Save(saveFileDialog.FileName, Map, true, true));
            App.ShowWarnings(result);
        }

        private void SetScrollLimits(ref XYInt min, ref sXY_uint max)
        {
            min.X = 0;
            min.Y = 0;
            max.X = (uint)Map.Terrain.TileSize.X;
            max.Y = (uint)Map.Terrain.TileSize.Y;
        }


        private void CompileCampaign()
        {
            var options = Map.InterfaceOptions;

            var ReturnResult = new Result("Compile campaign", false);
            logger.Info("Compile campaign");
            var waterTileErrors = 0;

            waterTileErrors = ValidateMap_WaterTris();
            if( waterTileErrors > 0 )
            {
                ReturnResult.WarningAdd(waterTileErrors + " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.");
            }

            ReturnResult.Add(ValidateMap());
            ReturnResult.Add(ValidateMap_UnitPositions());

            var MapName = "";
            var TypeNum = 0;

            MapName = options.CompileName;
            if( MapName.Length < 1 )
            {
                ReturnResult.ProblemAdd("Enter a name for the campaign files.");
            }
            TypeNum = (int)options.CampaignGameType;
            if( TypeNum < 0 | TypeNum > 2 )
            {
                ReturnResult.ProblemAdd("Select a campaign type.");
            }
            if( ReturnResult.HasProblems )
            {
                App.ShowWarnings(ReturnResult);
                return;
            }
            var CompileCampDialog = new SelectFolderDialog();
            if( CompileCampDialog.ShowDialog(form) != DialogResult.Ok )
            {
                return;
            }
            if( options.AutoScrollLimits )
            {
                SetScrollLimits(ref Map.InterfaceOptions.ScrollMin, ref Map.InterfaceOptions.ScrollMax);
            }

            var wzFormat = App.Kernel.Get<WzSaver>();
            ReturnResult.Add(wzFormat.Save(CompileCampDialog.Directory, Map, false, true));
            App.ShowWarnings(ReturnResult);
        }
    }
}