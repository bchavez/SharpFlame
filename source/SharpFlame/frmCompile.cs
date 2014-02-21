#region

using System;
using System.Diagnostics;
using System.Windows.Forms;
using NLog;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping;
using SharpFlame.Mapping.IO.Wz;
using SharpFlame.Mapping.Objects;
using SharpFlame.Maths;

#endregion

namespace SharpFlame
{
    public partial class frmCompile
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private clsMap Map;

        private frmCompile(clsMap Map)
        {
            InitializeComponent();

            Icon = App.ProgramIcon;

            this.Map = Map;
            Map.CompileScreen = this;

            UpdateControls();
        }

        public static frmCompile Create(clsMap Map)
        {
            if ( Map == null )
            {
                Debugger.Break();
                return null;
            }

            if ( Map.CompileScreen != null )
            {
                Debugger.Break();
                return null;
            }

            return new frmCompile(Map);
        }

        private void UpdateControls()
        {
            txtName.Text = Map.InterfaceOptions.CompileName;

            txtMultiPlayers.Text = Map.InterfaceOptions.CompileMultiPlayers;
            txtAuthor.Text = Map.InterfaceOptions.CompileMultiAuthor;
            cboLicense.Text = Map.InterfaceOptions.CompileMultiLicense;

            cboCampType.SelectedIndex = Map.InterfaceOptions.CampaignGameType;

            cbxAutoScrollLimits.Checked = Map.InterfaceOptions.AutoScrollLimits;
            AutoScrollLimits_Update();
            txtScrollMinX.Text = Map.InterfaceOptions.ScrollMin.X.ToStringInvariant();
            txtScrollMinY.Text = Map.InterfaceOptions.ScrollMin.Y.ToStringInvariant();
            txtScrollMaxX.Text = Map.InterfaceOptions.ScrollMax.X.ToStringInvariant();
            txtScrollMaxY.Text = Map.InterfaceOptions.ScrollMax.Y.ToStringInvariant();
        }

        private void SaveToMap()
        {
            Map.InterfaceOptions.CompileName = txtName.Text;

            Map.InterfaceOptions.CompileMultiPlayers = txtMultiPlayers.Text;
            Map.InterfaceOptions.CompileMultiAuthor = txtAuthor.Text;
            Map.InterfaceOptions.CompileMultiLicense = cboLicense.Text;

            Map.InterfaceOptions.CampaignGameType = cboCampType.SelectedIndex;

            var Invalid = false;

            try
            {
                Map.InterfaceOptions.ScrollMin.X = int.Parse(txtScrollMinX.Text);
            }
            catch ( Exception )
            {
                Invalid = true;
                Map.InterfaceOptions.ScrollMin.X = 0;
            }
            try
            {
                Map.InterfaceOptions.ScrollMin.Y = int.Parse(txtScrollMinY.Text);
            }
            catch ( Exception )
            {
                Invalid = true;
                Map.InterfaceOptions.ScrollMin.Y = 0;
            }
            try
            {
                Map.InterfaceOptions.ScrollMax.X = uint.Parse(txtScrollMaxX.Text);
            }
            catch ( Exception )
            {
                Invalid = true;
                Map.InterfaceOptions.ScrollMax.X = 0;
            }
            try
            {
                Map.InterfaceOptions.ScrollMax.Y = uint.Parse(txtScrollMaxY.Text);
            }
            catch ( Exception )
            {
                Invalid = true;
                Map.InterfaceOptions.ScrollMax.Y = 0;
            }
            Map.InterfaceOptions.AutoScrollLimits = cbxAutoScrollLimits.Checked || Invalid;

            Map.SetChanged();

            UpdateControls(); //display to show any changes
        }

        public void btnCompile_Click(Object sender, EventArgs e)
        {
            var ReturnResult = new clsResult("Compile multiplayer", false);
            logger.Info("Compile multiplayer");
            var A = 0;

            SaveToMap();

            var MapName = "";
            var License = cboLicense.Text;
            var PlayerCount = 0;
            if ( !IOUtil.InvariantParse(txtMultiPlayers.Text, ref PlayerCount) )
            {
                PlayerCount = 0;
            }
            if ( PlayerCount < 2 | PlayerCount > Constants.PlayerCountMax )
            {
                ReturnResult.ProblemAdd(string.Format("The number of players must be from 2 to {0}", Constants.PlayerCountMax));
            }

            A = ValidateMap_WaterTris();
            if ( A > 0 )
            {
                ReturnResult.WarningAdd(string.Format("{0} water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.", A));
            }

            ReturnResult.Add(ValidateMap());
            ReturnResult.Add(ValidateMap_UnitPositions());
            ReturnResult.Add(ValidateMap_Multiplayer(PlayerCount));

            MapName = txtName.Text;
            var CurrentChar = (char)0;
            for ( A = 0; A <= MapName.Length - 1; A++ )
            {
                CurrentChar = MapName[A];
                if (
                    !((CurrentChar >= 'a' && CurrentChar <= 'z') || (CurrentChar >= 'A' && CurrentChar <= 'Z') ||
                      (A >= 1 && ((CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '-' || CurrentChar == '_'))) )
                {
                    break;
                }
            }
            if ( A < MapName.Length )
            {
                ReturnResult.ProblemAdd("The map\'s name must contain only letters, numbers, underscores and hyphens, and must begin with a letter.");
            }
            if ( MapName.Length < 1 | MapName.Length > 16 )
            {
                ReturnResult.ProblemAdd("Map name must be from 1 to 16 characters.");
            }
            if ( string.IsNullOrEmpty(License) )
            {
                ReturnResult.ProblemAdd("Enter a valid license.");
            }
            if ( ReturnResult.HasProblems )
            {
                App.ShowWarnings(ReturnResult);
                return;
            }
            var CompileMultiDialog = new SaveFileDialog();
            if ( Map.PathInfo != null )
            {
                CompileMultiDialog.InitialDirectory = Map.PathInfo.Path;
            }
            CompileMultiDialog.FileName = PlayerCount + "c-" + MapName;
            CompileMultiDialog.Filter = "WZ Files (*.wz)|*.wz";
            if ( CompileMultiDialog.ShowDialog(this) != DialogResult.OK )
            {
                return;
            }

            SetScrollLimits(ref Map.InterfaceOptions.ScrollMin, ref Map.InterfaceOptions.ScrollMax);           
            Map.InterfaceOptions.CompileName = MapName;
            Map.InterfaceOptions.CompileMultiAuthor = txtAuthor.Text;
            Map.InterfaceOptions.CompileMultiPlayers = PlayerCount.ToString();
            Map.InterfaceOptions.CompileMultiLicense = License;
            Map.InterfaceOptions.CompileType = clsInterfaceOptions.EnumCompileType.Multiplayer;
            var wzFormat = new WzSaver(Map);
            ReturnResult.Add(wzFormat.Save(CompileMultiDialog.FileName, true, true));
            App.ShowWarnings(ReturnResult);
            if ( !ReturnResult.HasWarnings )
            {
                Close();
            }
        }

        public void frmCompile_FormClosed(object sender, FormClosedEventArgs e)
        {
            Map.CompileScreen = null;
            Map = null;
        }

        public void frmCompile_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveToMap();
        }

        private clsResult ValidateMap_UnitPositions()
        {
            var Result = new clsResult("Validate unit positions", false);
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

            foreach ( var unit in Map.Units )
            {
                if ( unit.TypeBase.Type == UnitType.PlayerStructure )
                {
                    structureTypeBase = (StructureTypeBase)unit.TypeBase;
                    StructureTypeType = structureTypeBase.StructureType;
                    UnitIsStructureModule[unit.MapLink.ArrayPosition] = structureTypeBase.IsModule() |
                                                                        StructureTypeType == StructureType.ResourceExtractor;
                }
            }
            //check and store non-module units first. modules need to check for the underlying unit.
            foreach ( var unit in Map.Units )
            {
                if ( !UnitIsStructureModule[unit.MapLink.ArrayPosition] )
                {
                    Footprint = unit.TypeBase.GetGetFootprintSelected(unit.Rotation);
                    Map.GetFootprintTileRange(unit.Pos.Horizontal, Footprint, ref StartPos, ref FinishPos);
                    if ( StartPos.X < 0 | FinishPos.X >= Map.Terrain.TileSize.X
                         | StartPos.Y < 0 | FinishPos.Y >= Map.Terrain.TileSize.Y )
                    {
                        var resultItem = modResults.CreateResultProblemGotoForObject(unit);
                        resultItem.Text = string.Format("Unit off map at position {0}.", unit.GetPosText());
                        Result.ItemAdd(resultItem);
                    }
                    else
                    {
                        for ( var y = StartPos.Y; y <= FinishPos.Y; y++ )
                        {
                            for ( var x = StartPos.X; x <= FinishPos.X; x++ )
                            {
                                if ( TileHasUnit[x, y] )
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
                                    if ( unit.TypeBase.Type == UnitType.PlayerStructure )
                                    {
                                        tileStructureTypeBase[x, y] = (StructureTypeBase)unit.TypeBase;
                                    }
                                    else if ( unit.TypeBase.Type == UnitType.Feature )
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
            foreach ( var unit in Map.Units )
            {
                if ( UnitIsStructureModule[unit.MapLink.ArrayPosition] )
                {
                    StructureTypeType = ((StructureTypeBase)unit.TypeBase).StructureType;
                    CentrePos.X = (unit.Pos.Horizontal.X / Constants.TerrainGridSpacing);
                    CentrePos.Y = unit.Pos.Horizontal.Y / Constants.TerrainGridSpacing;
                    if ( CentrePos.X < 0 | CentrePos.X >= Map.Terrain.TileSize.X
                         | CentrePos.Y < 0 | CentrePos.Y >= Map.Terrain.TileSize.Y )
                    {
                        var resultItem = modResults.CreateResultProblemGotoForObject(unit);
                        resultItem.Text = "Module off map at position " + unit.GetPosText() + ".";
                        Result.ItemAdd(resultItem);
                    }
                    else
                    {
                        if ( tileStructureTypeBase[CentrePos.X, CentrePos.Y] != null )
                        {
                            if ( TileObjectGroup[CentrePos.X, CentrePos.Y] == unit.UnitGroup )
                            {
                                if ( StructureTypeType == StructureType.FactoryModule )
                                {
                                    if ( tileStructureTypeBase[CentrePos.X, CentrePos.Y].StructureType == StructureType.Factory
                                         | tileStructureTypeBase[CentrePos.X, CentrePos.Y].StructureType == StructureType.VTOLFactory )
                                    {
                                        IsValid = true;
                                    }
                                    else
                                    {
                                        IsValid = false;
                                    }
                                }
                                else if ( StructureTypeType == StructureType.PowerModule )
                                {
                                    if ( tileStructureTypeBase[CentrePos.X, CentrePos.Y].StructureType == StructureType.PowerGenerator )
                                    {
                                        IsValid = true;
                                    }
                                    else
                                    {
                                        IsValid = false;
                                    }
                                }
                                else if ( StructureTypeType == StructureType.ResearchModule )
                                {
                                    if ( tileStructureTypeBase[CentrePos.X, CentrePos.Y].StructureType == StructureType.Research )
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
                        else if ( tileFeatureTypeBase[CentrePos.X, CentrePos.Y] != null )
                        {
                            if ( StructureTypeType == StructureType.ResourceExtractor )
                            {
                                if ( tileFeatureTypeBase[CentrePos.X, CentrePos.Y].FeatureType == FeatureTypeBase.enumFeatureType.OilResource )
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
                        else if ( StructureTypeType == StructureType.ResourceExtractor )
                        {
                            IsValid = true;
                        }
                        else
                        {
                            IsValid = false;
                        }
                        if ( !IsValid )
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

        private clsResult ValidateMap_Multiplayer(int PlayerCount)
        {
            var Result = new clsResult("Validate for multiplayer", false);
            logger.Info("Validate for multiplayer");

            if ( PlayerCount < 2 | PlayerCount > Constants.PlayerCountMax )
            {
                Result.ProblemAdd("Unable to evaluate for multiplayer due to bad number of players.");
                return Result;
            }

            //check HQs, Trucks and unit counts

            var PlayerHQCount = new int[Constants.PlayerCountMax];
            var Player23TruckCount = new int[Constants.PlayerCountMax];
            var PlayerMasterTruckCount = new int[Constants.PlayerCountMax];
            var ScavPlayerNum = 0;
            var DroidType = default(DroidDesign);
            StructureTypeBase structureTypeBase;
            var UnusedPlayerUnitWarningCount = 0;
            var Unit = default(clsUnit);

            ScavPlayerNum = Math.Max(PlayerCount, 7);

            foreach ( var tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.UnitGroup == Map.ScavengerUnitGroup )
                {
                }
                else
                {
                    if ( Unit.TypeBase.Type == UnitType.PlayerDroid )
                    {
                        DroidType = (DroidDesign)Unit.TypeBase;
                        if ( DroidType.Body != null && DroidType.Propulsion != null && DroidType.Turret1 != null && DroidType.TurretCount == 1 )
                        {
                            if ( DroidType.Turret1.TurretType == enumTurretType.Construct )
                            {
                                PlayerMasterTruckCount[Unit.UnitGroup.WZ_StartPos]++;
                                if ( DroidType.IsTemplate )
                                {
                                    Player23TruckCount[Unit.UnitGroup.WZ_StartPos]++;
                                }
                            }
                        }
                    }
                    else if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                    {
                        structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                        if ( structureTypeBase.Code == "A0CommandCentre" )
                        {
                            PlayerHQCount[Unit.UnitGroup.WZ_StartPos]++;
                        }
                    }
                }
                if ( Unit.TypeBase.Type != UnitType.Feature )
                {
                    if ( Unit.UnitGroup.WZ_StartPos >= PlayerCount )
                    {
                        if ( UnusedPlayerUnitWarningCount < 32 )
                        {
                            UnusedPlayerUnitWarningCount++;
                            var resultItem = modResults.CreateResultProblemGotoForObject(Unit);
                            resultItem.Text = string.Format("An unused player ({0}) has a unit at {1}.", Unit.UnitGroup.WZ_StartPos, Unit.GetPosText());
                            Result.ItemAdd(resultItem);
                        }
                    }
                }
            }

            for ( var A = 0; A <= PlayerCount - 1; A++ )
            {
                if ( PlayerHQCount[A] == 0 )
                {
                    Result.ProblemAdd("There is no Command Centre for player " + Convert.ToString(A) + ".");
                }
                if ( PlayerMasterTruckCount[A] == 0 )
                {
                    Result.ProblemAdd("There are no constructor units for player " + Convert.ToString(A) + ".");
                }
                else if ( Player23TruckCount[A] == 0 )
                {
                    Result.WarningAdd("All constructor units for player " + Convert.ToString(A) + " will only exist in master.");
                }
            }

            return Result;
        }

        private clsResult ValidateMap()
        {
            var ReturnResult = new clsResult("Validate map", false);
            logger.Info("Validate map");

            if ( Map.Terrain.TileSize.X > Constants.WzMapMaxSize )
            {
                ReturnResult.WarningAdd("Map width is too large. The maximum is " + Convert.ToString(Constants.WzMapMaxSize) + ".");
            }
            if ( Map.Terrain.TileSize.Y > Constants.WzMapMaxSize )
            {
                ReturnResult.WarningAdd("Map height is too large. The maximum is " + Convert.ToString(Constants.WzMapMaxSize) + ".");
            }

            if ( Map.Tileset == null )
            {
                ReturnResult.ProblemAdd("No tileset selected.");
            }

            var PlayerStructureTypeCount = new int[Constants.PlayerCountMax, App.ObjectData.StructureTypes.Count];
            var ScavStructureTypeCount = new int[App.ObjectData.StructureTypes.Count];
            var structureTypeBase = default(StructureTypeBase);
            var Unit = default(clsUnit);

            foreach ( var tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                {
                    structureTypeBase = (StructureTypeBase)Unit.TypeBase;
                    if ( Unit.UnitGroup == Map.ScavengerUnitGroup )
                    {
                        ScavStructureTypeCount[structureTypeBase.StructureType_ObjectDataLink.ArrayPosition]++;
                    }
                    else
                    {
                        PlayerStructureTypeCount[Unit.UnitGroup.WZ_StartPos, structureTypeBase.StructureType_ObjectDataLink.ArrayPosition]++;
                    }
                }
            }

            foreach ( var tempLoopVar_StructureType in App.ObjectData.StructureTypes )
            {
                structureTypeBase = tempLoopVar_StructureType;
                var StructureTypeNum = structureTypeBase.StructureType_ObjectDataLink.ArrayPosition;
                var PlayerNum = 0;
                for ( PlayerNum = 0; PlayerNum <= Constants.PlayerCountMax - 1; PlayerNum++ )
                {
                    if ( PlayerStructureTypeCount[PlayerNum, StructureTypeNum] > 255 )
                    {
                        ReturnResult.ProblemAdd("Player {0} has to  many ({1}) of structure \"{2}\"" +
                                                ". The limit is 255 of any one structure type.".Format2(
                                                    PlayerNum, PlayerStructureTypeCount[PlayerNum, StructureTypeNum],
                                                    structureTypeBase.Code)
                            );
                    }
                }
                if ( ScavStructureTypeCount[StructureTypeNum] > 255 )
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

            if ( Map.Tileset == null )
            {
                return 0;
            }

            for ( Y = 0; Y <= Map.Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X - 1; X++ )
                {
                    if ( Map.Terrain.Tiles[X, Y].Tri )
                    {
                        if ( Map.Terrain.Tiles[X, Y].Texture.TextureNum >= 0 && Map.Terrain.Tiles[X, Y].Texture.TextureNum < Map.Tileset.TileCount )
                        {
                            if ( Map.Tileset.Tiles[Map.Terrain.Tiles[X, Y].Texture.TextureNum].DefaultType == Constants.TileTypeNumWater )
                            {
                                Count++;
                            }
                        }
                    }
                }
            }
            return Count;
        }

        public void btnCompileCampaign_Click(Object sender, EventArgs e)
        {
            var ReturnResult = new clsResult("Compile campaign", false);
            logger.Info("Compile campaign");
            var A = 0;

            SaveToMap();

            A = ValidateMap_WaterTris();
            if ( A > 0 )
            {
                ReturnResult.WarningAdd(A + " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.");
            }

            ReturnResult.Add(ValidateMap());
            ReturnResult.Add(ValidateMap_UnitPositions());

            var MapName = "";
            var TypeNum = 0;

            MapName = txtName.Text;
            if ( MapName.Length < 1 )
            {
                ReturnResult.ProblemAdd("Enter a name for the campaign files.");
            }
            TypeNum = cboCampType.SelectedIndex;
            if ( TypeNum < 0 | TypeNum > 2 )
            {
                ReturnResult.ProblemAdd("Select a campaign type.");
            }
            if ( ReturnResult.HasProblems )
            {
                App.ShowWarnings(ReturnResult);
                return;
            }
            var CompileCampDialog = new FolderBrowserDialog();
            if ( CompileCampDialog.ShowDialog(this) != DialogResult.OK )
            {
                return;
            }
            SetScrollLimits(ref Map.InterfaceOptions.ScrollMin, ref Map.InterfaceOptions.ScrollMax);           
            Map.InterfaceOptions.CompileName = MapName;
            Map.InterfaceOptions.CompileType = clsInterfaceOptions.EnumCompileType.Campaign;
            Map.InterfaceOptions.CampaignGameType = TypeNum;

            var wzFormat = new WzSaver(Map);
            ReturnResult.Add(wzFormat.Save(CompileCampDialog.SelectedPath, false, true));
            App.ShowWarnings(ReturnResult);
            if ( !ReturnResult.HasWarnings )
            {
                Close();
            }
        }

        public void AutoScrollLimits_Update()
        {
            if ( cbxAutoScrollLimits.Checked )
            {
                txtScrollMinX.Enabled = false;
                txtScrollMaxX.Enabled = false;
                txtScrollMinY.Enabled = false;
                txtScrollMaxY.Enabled = false;
            }
            else
            {
                txtScrollMinX.Enabled = true;
                txtScrollMaxX.Enabled = true;
                txtScrollMinY.Enabled = true;
                txtScrollMaxY.Enabled = true;
            }
        }

        public void cbxAutoScrollLimits_CheckedChanged(Object sender, EventArgs e)
        {
            if ( !cbxAutoScrollLimits.Enabled )
            {
                return;
            }

            AutoScrollLimits_Update();
        }

        private void SetScrollLimits(ref XYInt Min, ref sXY_uint Max)
        {
            Min.X = 0;
            Min.Y = 0;
            Max.X = (uint)Map.Terrain.TileSize.X;
            Max.Y = (uint)Map.Terrain.TileSize.Y;
            if ( !cbxAutoScrollLimits.Checked )
            {
                IOUtil.InvariantParse(txtScrollMinX.Text, ref Min.X);
                IOUtil.InvariantParse(txtScrollMinY.Text, ref Min.Y);
                IOUtil.InvariantParse(txtScrollMaxX.Text, ref Max.X);
                IOUtil.InvariantParse(txtScrollMaxY.Text, ref Max.Y);
            }
        }
    }
}