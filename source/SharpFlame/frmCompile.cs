using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using SharpFlame.FileIO;
using SharpFlame.MathExtra;

namespace SharpFlame
{
    public partial class frmCompile
    {
        private clsMap Map;

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

        private frmCompile(clsMap Map)
        {
            InitializeComponent();

            Icon = App.ProgramIcon;

            this.Map = Map;
            Map.CompileScreen = this;

            UpdateControls();
        }

        private void UpdateControls()
        {
            txtName.Text = Map.InterfaceOptions.CompileName;

            txtMultiPlayers.Text = Map.InterfaceOptions.CompileMultiPlayers;
            cbxLevFormat.Checked = Map.InterfaceOptions.CompileMultiXPlayers;
            txtAuthor.Text = Map.InterfaceOptions.CompileMultiAuthor;
            cboLicense.Text = Map.InterfaceOptions.CompileMultiLicense;

            cboCampType.SelectedIndex = Map.InterfaceOptions.CampaignGameType;

            cbxAutoScrollLimits.Checked = Map.InterfaceOptions.AutoScrollLimits;
            AutoScrollLimits_Update();
            txtScrollMinX.Text = IOUtil.InvariantToString_int(Map.InterfaceOptions.ScrollMin.X);
            txtScrollMinY.Text = IOUtil.InvariantToString_int(Map.InterfaceOptions.ScrollMin.Y);
            txtScrollMaxX.Text = IOUtil.InvariantToString_uint(Map.InterfaceOptions.ScrollMax.X);
            txtScrollMaxY.Text = IOUtil.InvariantToString_uint(Map.InterfaceOptions.ScrollMax.Y);
        }

        private void SaveToMap()
        {
            Map.InterfaceOptions.CompileName = txtName.Text;

            Map.InterfaceOptions.CompileMultiPlayers = txtMultiPlayers.Text;
            Map.InterfaceOptions.CompileMultiXPlayers = cbxLevFormat.Checked;
            Map.InterfaceOptions.CompileMultiAuthor = txtAuthor.Text;
            Map.InterfaceOptions.CompileMultiLicense = cboLicense.Text;

            Map.InterfaceOptions.CampaignGameType = cboCampType.SelectedIndex;

            bool Invalid = false;

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
            clsResult ReturnResult = new clsResult("Compile multiplayer");
            int A = 0;

            SaveToMap();

            string MapName = "";
            string License = cboLicense.Text;
            int PlayerCount = 0;
            if ( !IOUtil.InvariantParse_int(txtMultiPlayers.Text, ref PlayerCount) )
            {
                PlayerCount = 0;
            }
            bool IsXPlayerFormat = cbxLevFormat.Checked;
            if ( PlayerCount < 2 | PlayerCount > 10 )
            {
                ReturnResult.ProblemAdd("The number of players must be from 2 to " + Convert.ToString(Constants.PlayerCountMax));
            }
            if ( !IsXPlayerFormat )
            {
                if ( PlayerCount != 2 & PlayerCount != 4 & PlayerCount != 8 )
                {
                    ReturnResult.ProblemAdd("You must enable support for this number of players.");
                }
            }

            A = ValidateMap_WaterTris();
            if ( A > 0 )
            {
                ReturnResult.WarningAdd(A + " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.");
            }

            ReturnResult.Add(ValidateMap());
            ReturnResult.Add(ValidateMap_UnitPositions());
            ReturnResult.Add(ValidateMap_Multiplayer(PlayerCount, IsXPlayerFormat));

            MapName = txtName.Text;
            char CurrentChar = (char)0;
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
            SaveFileDialog CompileMultiDialog = new SaveFileDialog();
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
            clsMap.sWrite_WZ_Args WriteWZArgs = new clsMap.sWrite_WZ_Args();
            WriteWZArgs.MapName = MapName;
            WriteWZArgs.Path = CompileMultiDialog.FileName;
            WriteWZArgs.Overwrite = true;
            SetScrollLimits(WriteWZArgs.ScrollMin, WriteWZArgs.ScrollMax);
            WriteWZArgs.Multiplayer = new clsMap.sWrite_WZ_Args.clsMultiplayer();
            WriteWZArgs.Multiplayer.AuthorName = txtAuthor.Text;
            WriteWZArgs.Multiplayer.PlayerCount = PlayerCount;
            WriteWZArgs.Multiplayer.IsBetaPlayerFormat = IsXPlayerFormat;
            WriteWZArgs.Multiplayer.License = License;
            WriteWZArgs.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Multiplayer;
            ReturnResult.Add(Map.Write_WZ(WriteWZArgs));
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
            clsResult Result = new clsResult("Validate unit positions");

            //check unit positions

            bool[,] TileHasUnit = new bool[Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y];
            clsStructureType[,] TileStructureType = new clsStructureType[Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y];
            clsFeatureType[,] TileFeatureType = new clsFeatureType[Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y];
            clsMap.clsUnitGroup[,] TileObjectGroup = new clsMap.clsUnitGroup[Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y];
            int X = 0;
            int Y = 0;
            sXY_int StartPos = new sXY_int();
            sXY_int FinishPos = new sXY_int();
            sXY_int CentrePos = new sXY_int();
            clsStructureType.enumStructureType StructureTypeType;
            clsStructureType StructureType = default(clsStructureType);
            sXY_int Footprint = new sXY_int();
            bool[] UnitIsStructureModule = new bool[Map.Units.Count];
            bool IsValid = default(bool);
            clsMap.clsUnit Unit = default(clsMap.clsUnit);
            foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                {
                    StructureType = (clsStructureType)Unit.Type;
                    StructureTypeType = StructureType.StructureType;
                    UnitIsStructureModule[Unit.MapLink.ArrayPosition] = StructureType.IsModule() |
                                                                        StructureTypeType == clsStructureType.enumStructureType.ResourceExtractor;
                }
            }
            //check and store non-module units first. modules need to check for the underlying unit.
            foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( !UnitIsStructureModule[Unit.MapLink.ArrayPosition] )
                {
                    Footprint = Unit.Type.get_GetFootprintSelected(Unit.Rotation);
                    Map.GetFootprintTileRange(Unit.Pos.Horizontal, Footprint, StartPos, FinishPos);
                    if ( StartPos.X < 0 | FinishPos.X >= Map.Terrain.TileSize.X
                         | StartPos.Y < 0 | FinishPos.Y >= Map.Terrain.TileSize.Y )
                    {
                        clsResultProblemGoto<clsResultItemPosGoto> resultItem = modResults.CreateResultProblemGotoForObject(Unit);
                        resultItem.Text = "Unit off map at position " + Unit.GetPosText() + ".";
                        Result.ItemAdd(resultItem);
                    }
                    else
                    {
                        for ( Y = StartPos.Y; Y <= FinishPos.Y; Y++ )
                        {
                            for ( X = StartPos.X; X <= FinishPos.X; X++ )
                            {
                                if ( TileHasUnit[X, Y] )
                                {
                                    clsResultProblemGoto<clsResultItemPosGoto> resultItem = modResults.CreateResultProblemGotoForObject(Unit);
                                    resultItem.Text = "Bad unit overlap on tile " + Convert.ToString(X) + ", " + Convert.ToString(Y) + ".";
                                    Result.ItemAdd(resultItem);
                                }
                                else
                                {
                                    TileHasUnit[X, Y] = true;
                                    if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                                    {
                                        TileStructureType[X, Y] = (clsStructureType)Unit.Type;
                                    }
                                    else if ( Unit.Type.Type == clsUnitType.enumType.Feature )
                                    {
                                        TileFeatureType[X, Y] = (clsFeatureType)Unit.Type;
                                    }
                                    TileObjectGroup[X, Y] = Unit.UnitGroup;
                                }
                            }
                        }
                    }
                }
            }
            //check modules and extractors
            foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( UnitIsStructureModule[Unit.MapLink.ArrayPosition] )
                {
                    StructureTypeType = ((clsStructureType)Unit.Type).StructureType;
                    CentrePos.X = Conversion.Int(Unit.Pos.Horizontal.X / App.TerrainGridSpacing);
                    CentrePos.Y = (int)(Conversion.Int(Unit.Pos.Horizontal.Y / App.TerrainGridSpacing));
                    if ( CentrePos.X < 0 | CentrePos.X >= Map.Terrain.TileSize.X
                         | CentrePos.Y < 0 | CentrePos.Y >= Map.Terrain.TileSize.Y )
                    {
                        clsResultProblemGoto<clsResultItemPosGoto> resultItem = modResults.CreateResultProblemGotoForObject(Unit);
                        resultItem.Text = "Module off map at position " + Unit.GetPosText() + ".";
                        Result.ItemAdd(resultItem);
                    }
                    else
                    {
                        if ( TileStructureType[CentrePos.X, CentrePos.Y] != null )
                        {
                            if ( TileObjectGroup[CentrePos.X, CentrePos.Y] == Unit.UnitGroup )
                            {
                                if ( StructureTypeType == clsStructureType.enumStructureType.FactoryModule )
                                {
                                    if ( TileStructureType[CentrePos.X, CentrePos.Y].StructureType == clsStructureType.enumStructureType.Factory
                                         | TileStructureType[CentrePos.X, CentrePos.Y].StructureType == clsStructureType.enumStructureType.VTOLFactory )
                                    {
                                        IsValid = true;
                                    }
                                    else
                                    {
                                        IsValid = false;
                                    }
                                }
                                else if ( StructureTypeType == clsStructureType.enumStructureType.PowerModule )
                                {
                                    if ( TileStructureType[CentrePos.X, CentrePos.Y].StructureType == clsStructureType.enumStructureType.PowerGenerator )
                                    {
                                        IsValid = true;
                                    }
                                    else
                                    {
                                        IsValid = false;
                                    }
                                }
                                else if ( StructureTypeType == clsStructureType.enumStructureType.ResearchModule )
                                {
                                    if ( TileStructureType[CentrePos.X, CentrePos.Y].StructureType == clsStructureType.enumStructureType.Research )
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
                        else if ( TileFeatureType[CentrePos.X, CentrePos.Y] != null )
                        {
                            if ( StructureTypeType == clsStructureType.enumStructureType.ResourceExtractor )
                            {
                                if ( TileFeatureType[CentrePos.X, CentrePos.Y].FeatureType == clsFeatureType.enumFeatureType.OilResource )
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
                        else if ( StructureTypeType == clsStructureType.enumStructureType.ResourceExtractor )
                        {
                            IsValid = true;
                        }
                        else
                        {
                            IsValid = false;
                        }
                        if ( !IsValid )
                        {
                            clsResultProblemGoto<clsResultItemPosGoto> resultItem = modResults.CreateResultProblemGotoForObject(Unit);
                            resultItem.Text = "Bad module on tile " + Convert.ToString(CentrePos.X) + ", " + Convert.ToString(CentrePos.Y) + ".";
                            Result.ItemAdd(resultItem);
                        }
                    }
                }
            }

            return Result;
        }

        private clsResult ValidateMap_Multiplayer(int PlayerCount, bool IsXPlayerFormat)
        {
            clsResult Result = new clsResult("Validate for multiplayer");

            if ( PlayerCount < 2 | PlayerCount > Constants.PlayerCountMax )
            {
                Result.ProblemAdd("Unable to evaluate for multiplayer due to bad number of players.");
                return Result;
            }

            //check HQs, Trucks and unit counts

            int[] PlayerHQCount = new int[Constants.PlayerCountMax];
            int[] Player23TruckCount = new int[Constants.PlayerCountMax];
            int[] PlayerMasterTruckCount = new int[Constants.PlayerCountMax];
            int ScavPlayerNum = 0;
            int ScavObjectCount = 0;
            clsDroidDesign DroidType = default(clsDroidDesign);
            clsStructureType StructureType;
            int UnusedPlayerUnitWarningCount = 0;
            clsMap.clsUnit Unit = default(clsMap.clsUnit);

            ScavPlayerNum = Math.Max(PlayerCount, 7);

            foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.UnitGroup == Map.ScavengerUnitGroup )
                {
                }
                else
                {
                    if ( Unit.Type.Type == clsUnitType.enumType.PlayerDroid )
                    {
                        DroidType = (clsDroidDesign)Unit.Type;
                        if ( DroidType.Body != null && DroidType.Propulsion != null && DroidType.Turret1 != null && DroidType.TurretCount == 1 )
                        {
                            if ( DroidType.Turret1.TurretType == clsTurret.enumTurretType.Construct )
                            {
                                PlayerMasterTruckCount[Unit.UnitGroup.WZ_StartPos]++;
                                if ( DroidType.IsTemplate )
                                {
                                    Player23TruckCount[Unit.UnitGroup.WZ_StartPos]++;
                                }
                            }
                        }
                    }
                    else if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                    {
                        StructureType = (clsStructureType)Unit.Type;
                        if ( StructureType.Code == "A0CommandCentre" )
                        {
                            PlayerHQCount[Unit.UnitGroup.WZ_StartPos]++;
                        }
                    }
                }
                if ( Unit.Type.Type != clsUnitType.enumType.Feature )
                {
                    if ( Unit.UnitGroup.WZ_StartPos == ScavPlayerNum || Unit.UnitGroup == Map.ScavengerUnitGroup )
                    {
                        ScavObjectCount++;
                    }
                    else if ( Unit.UnitGroup.WZ_StartPos >= PlayerCount )
                    {
                        if ( UnusedPlayerUnitWarningCount < 32 )
                        {
                            UnusedPlayerUnitWarningCount++;
                            clsResultProblemGoto<clsResultItemPosGoto> resultItem = modResults.CreateResultProblemGotoForObject(Unit);
                            resultItem.Text = "An unused player (" + Convert.ToString(Unit.UnitGroup.WZ_StartPos) + ") has a unit at " + Unit.GetPosText() + ".";
                            Result.ItemAdd(resultItem);
                        }
                    }
                }
            }

            if ( ScavPlayerNum <= 7 || IsXPlayerFormat )
            {
            }
            else if ( ScavObjectCount > 0 ) //only counts non-features
            {
                Result.ProblemAdd("Scavengers are not supported on a map with this number of players without enabling X player support.");
            }

            for ( int A = 0; A <= PlayerCount - 1; A++ )
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
            clsResult ReturnResult = new clsResult("Validate map");

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

            int[,] PlayerStructureTypeCount = new int[Constants.PlayerCountMax, App.ObjectData.StructureTypes.Count];
            int[] ScavStructureTypeCount = new int[App.ObjectData.StructureTypes.Count];
            clsStructureType StructureType = default(clsStructureType);
            clsMap.clsUnit Unit = default(clsMap.clsUnit);

            foreach ( clsMap.clsUnit tempLoopVar_Unit in Map.Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                {
                    StructureType = (clsStructureType)Unit.Type;
                    if ( Unit.UnitGroup == Map.ScavengerUnitGroup )
                    {
                        ScavStructureTypeCount[StructureType.StructureType_ObjectDataLink.ArrayPosition]++;
                    }
                    else
                    {
                        PlayerStructureTypeCount[Unit.UnitGroup.WZ_StartPos, StructureType.StructureType_ObjectDataLink.ArrayPosition]++;
                    }
                }
            }

            foreach ( clsStructureType tempLoopVar_StructureType in App.ObjectData.StructureTypes )
            {
                StructureType = tempLoopVar_StructureType;
                int StructureTypeNum = StructureType.StructureType_ObjectDataLink.ArrayPosition;
                int PlayerNum = 0;
                for ( PlayerNum = 0; PlayerNum <= Constants.PlayerCountMax - 1; PlayerNum++ )
                {
                    if ( PlayerStructureTypeCount[PlayerNum, StructureTypeNum] > 255 )
                    {
                        ReturnResult.ProblemAdd("Player " + Convert.ToString(PlayerNum) + " has too many (" +
                                                Convert.ToString(PlayerStructureTypeCount[PlayerNum, StructureTypeNum]) + ") of structure " +
                                                Convert.ToString(ControlChars.Quote) + StructureType.Code + Convert.ToString(ControlChars.Quote) +
                                                ". The limit is 255 of any one structure type.");
                    }
                }
                if ( ScavStructureTypeCount[StructureTypeNum] > 255 )
                {
                    ReturnResult.ProblemAdd("Scavengers have too many (" + Convert.ToString(ScavStructureTypeCount[StructureTypeNum]) + ") of structure " +
                                            Convert.ToString(ControlChars.Quote) + StructureType.Code + Convert.ToString(ControlChars.Quote) +
                                            ". The limit is 255 of any one structure type.");
                }
            }

            return ReturnResult;
        }

        private int ValidateMap_WaterTris()
        {
            int X = 0;
            int Y = 0;
            int Count = 0;

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
                            if ( Map.Tileset.Tiles[Map.Terrain.Tiles[X, Y].Texture.TextureNum].Default_Type == App.TileTypeNum_Water )
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
            clsResult ReturnResult = new clsResult("Compile campaign");
            int A = 0;

            SaveToMap();

            A = ValidateMap_WaterTris();
            if ( A > 0 )
            {
                ReturnResult.WarningAdd(A + " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.");
            }

            ReturnResult.Add(ValidateMap());
            ReturnResult.Add(ValidateMap_UnitPositions());

            string MapName = "";
            int TypeNum = 0;

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
            FolderBrowserDialog CompileCampDialog = new FolderBrowserDialog();
            if ( CompileCampDialog.ShowDialog(this) != DialogResult.OK )
            {
                return;
            }
            clsMap.sWrite_WZ_Args WriteWZArgs = new clsMap.sWrite_WZ_Args();
            WriteWZArgs.MapName = MapName;
            WriteWZArgs.Path = CompileCampDialog.SelectedPath;
            WriteWZArgs.Overwrite = false;
            SetScrollLimits(WriteWZArgs.ScrollMin, WriteWZArgs.ScrollMax);
            WriteWZArgs.Campaign = new clsMap.sWrite_WZ_Args.clsCampaign();
            WriteWZArgs.Campaign.GAMType = (uint)TypeNum;
            WriteWZArgs.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Campaign;
            ReturnResult.Add(Map.Write_WZ(WriteWZArgs));
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

        private void SetScrollLimits(sXY_int Min, sXY_uint Max)
        {
            Min.X = 0;
            Min.Y = 0;
            Max.X = (uint)Map.Terrain.TileSize.X;
            Max.Y = (uint)Map.Terrain.TileSize.Y;
            if ( !cbxAutoScrollLimits.Checked )
            {
                IOUtil.InvariantParse_int(txtScrollMinX.Text, ref Min.X);
                IOUtil.InvariantParse_int(txtScrollMinY.Text, ref Min.Y);
                IOUtil.InvariantParse_uint(txtScrollMaxX.Text, Max.X);
                IOUtil.InvariantParse_uint(txtScrollMaxY.Text, Max.Y);
            }
        }
    }
}