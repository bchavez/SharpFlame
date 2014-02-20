#region

using System;
using System.Windows.Forms;
using SharpFlame.Collections.Specialized;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.Generators;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;

#endregion

namespace SharpFlame
{
    public partial class frmGenerator
    {
        private readonly clsGenerateMap Generator = new clsGenerateMap();
        private readonly frmMain _Owner;

        private int PlayerCount = 4;
        private bool StopTrying;

        public frmGenerator(frmMain Owner)
        {
            InitializeComponent();

            _Owner = Owner;
        }

        private int ValidateTextbox(TextBox TextBoxToValidate, double Min, double Max, double Multiplier)
        {
            double dblTemp = 0;
            var Result = 0;

            if ( !IOUtil.InvariantParse(TextBoxToValidate.Text, ref dblTemp) )
            {
                return 0;
            }
            Result = (int)(MathUtil.Clamp_dbl(dblTemp, Min, Max) * Multiplier);
            TextBoxToValidate.Text = ((float)(Result / Multiplier)).ToStringInvariant();
            return Result;
        }

        public void btnGenerateLayout_Click(Object sender, EventArgs e)
        {
            lstResult.Items.Clear();
            btnGenerateLayout.Enabled = false;
            lstResult_AddText("Generating layout.");
            Application.DoEvents();

            StopTrying = false;

            var LoopCount = 0;

            Generator.ClearLayout();

            Generator.GenerateTileset = null;
            Generator.Map = null;

            Generator.TopLeftPlayerCount = PlayerCount;

            switch ( cboSymmetry.SelectedIndex )
            {
                case 0: //none
                    Generator.SymmetryBlockCountXY.X = 1;
                    Generator.SymmetryBlockCountXY.Y = 1;
                    Generator.SymmetryBlockCount = 1;
                    Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(Generator.SymmetryBlockCount - 1) + 1];
                    Generator.SymmetryBlocks[0].XYNum = new XYInt(0, 0);
                    Generator.SymmetryBlocks[0].Orientation = new TileOrientation(false, false, false);
                    Generator.SymmetryIsRotational = false;
                    break;
                case 1: //h rotation
                    Generator.SymmetryBlockCountXY.X = 2;
                    Generator.SymmetryBlockCountXY.Y = 1;
                    Generator.SymmetryBlockCount = 2;
                    Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(Generator.SymmetryBlockCount - 1) + 1];
                    Generator.SymmetryBlocks[0].XYNum = new XYInt(0, 0);
                    Generator.SymmetryBlocks[0].Orientation = new TileOrientation(false, false, false);
                    Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    Generator.SymmetryBlocks[1].XYNum = new XYInt(1, 0);
                    Generator.SymmetryBlocks[1].Orientation = new TileOrientation(true, true, false);
                    Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    Generator.SymmetryIsRotational = true;
                    break;
                case 2: //v rotation
                    Generator.SymmetryBlockCountXY.X = 1;
                    Generator.SymmetryBlockCountXY.Y = 2;
                    Generator.SymmetryBlockCount = 2;
                    Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(Generator.SymmetryBlockCount - 1) + 1];
                    Generator.SymmetryBlocks[0].XYNum = new XYInt(0, 0);
                    Generator.SymmetryBlocks[0].Orientation = new TileOrientation(false, false, false);
                    Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    Generator.SymmetryBlocks[1].XYNum = new XYInt(0, 1);
                    Generator.SymmetryBlocks[1].Orientation = new TileOrientation(true, true, false);
                    Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    Generator.SymmetryIsRotational = true;
                    break;
                case 3: //h flip
                    Generator.SymmetryBlockCountXY.X = 2;
                    Generator.SymmetryBlockCountXY.Y = 1;
                    Generator.SymmetryBlockCount = 2;
                    Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(Generator.SymmetryBlockCount - 1) + 1];
                    Generator.SymmetryBlocks[0].XYNum = new XYInt(0, 0);
                    Generator.SymmetryBlocks[0].Orientation = new TileOrientation(false, false, false);
                    Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    Generator.SymmetryBlocks[1].XYNum = new XYInt(1, 0);
                    Generator.SymmetryBlocks[1].Orientation = new TileOrientation(true, false, false);
                    Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    Generator.SymmetryIsRotational = false;
                    break;
                case 4: //v flip
                    Generator.SymmetryBlockCountXY.X = 1;
                    Generator.SymmetryBlockCountXY.Y = 2;
                    Generator.SymmetryBlockCount = 2;
                    Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(Generator.SymmetryBlockCount - 1) + 1];
                    Generator.SymmetryBlocks[0].XYNum = new XYInt(0, 0);
                    Generator.SymmetryBlocks[0].Orientation = new TileOrientation(false, false, false);
                    Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    Generator.SymmetryBlocks[1].XYNum = new XYInt(0, 1);
                    Generator.SymmetryBlocks[1].Orientation = new TileOrientation(false, true, false);
                    Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    Generator.SymmetryIsRotational = false;
                    Generator.SymmetryIsRotational = false;
                    break;
                case 5: //4x rotation
                    Generator.SymmetryBlockCountXY.X = 2;
                    Generator.SymmetryBlockCountXY.Y = 2;
                    Generator.SymmetryBlockCount = 4;
                    Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(Generator.SymmetryBlockCount - 1) + 1];
                    Generator.SymmetryBlocks[0].XYNum = new XYInt(0, 0);
                    Generator.SymmetryBlocks[0].Orientation = new TileOrientation(false, false, false);
                    Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    Generator.SymmetryBlocks[0].ReflectToNum[1] = 2;
                    Generator.SymmetryBlocks[1].XYNum = new XYInt(1, 0);
                    Generator.SymmetryBlocks[1].Orientation = new TileOrientation(true, false, true);
                    Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[1].ReflectToNum[0] = 3;
                    Generator.SymmetryBlocks[1].ReflectToNum[1] = 0;
                    Generator.SymmetryBlocks[2].XYNum = new XYInt(0, 1);
                    Generator.SymmetryBlocks[2].Orientation = new TileOrientation(false, true, true);
                    Generator.SymmetryBlocks[2].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[2].ReflectToNum[0] = 0;
                    Generator.SymmetryBlocks[2].ReflectToNum[1] = 3;
                    Generator.SymmetryBlocks[3].XYNum = new XYInt(1, 1);
                    Generator.SymmetryBlocks[3].Orientation = new TileOrientation(true, true, false);
                    Generator.SymmetryBlocks[3].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[3].ReflectToNum[0] = 2;
                    Generator.SymmetryBlocks[3].ReflectToNum[1] = 1;
                    Generator.SymmetryIsRotational = true;
                    break;
                case 6: //hv flip
                    Generator.SymmetryBlockCountXY.X = 2;
                    Generator.SymmetryBlockCountXY.Y = 2;
                    Generator.SymmetryBlockCount = 4;
                    Generator.SymmetryBlocks = new clsGenerateMap.sSymmetryBlock[(Generator.SymmetryBlockCount - 1) + 1];
                    Generator.SymmetryBlocks[0].XYNum = new XYInt(0, 0);
                    Generator.SymmetryBlocks[0].Orientation = new TileOrientation(false, false, false);
                    Generator.SymmetryBlocks[0].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[0].ReflectToNum[0] = 1;
                    Generator.SymmetryBlocks[0].ReflectToNum[1] = 2;
                    Generator.SymmetryBlocks[1].XYNum = new XYInt(1, 0);
                    Generator.SymmetryBlocks[1].Orientation = new TileOrientation(true, false, false);
                    Generator.SymmetryBlocks[1].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[1].ReflectToNum[0] = 0;
                    Generator.SymmetryBlocks[1].ReflectToNum[1] = 3;
                    Generator.SymmetryBlocks[2].XYNum = new XYInt(0, 1);
                    Generator.SymmetryBlocks[2].Orientation = new TileOrientation(false, true, false);
                    Generator.SymmetryBlocks[2].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[2].ReflectToNum[0] = 3;
                    Generator.SymmetryBlocks[2].ReflectToNum[1] = 0;
                    Generator.SymmetryBlocks[3].XYNum = new XYInt(1, 1);
                    Generator.SymmetryBlocks[3].Orientation = new TileOrientation(true, true, false);
                    Generator.SymmetryBlocks[3].ReflectToNum = new int[(((int)Math.Round(Generator.SymmetryBlockCount / 2.0)) - 1) + 1];
                    Generator.SymmetryBlocks[3].ReflectToNum[0] = 2;
                    Generator.SymmetryBlocks[3].ReflectToNum[1] = 1;
                    Generator.SymmetryIsRotational = false;
                    break;
                default:
                    MessageBox.Show("Select symmetry");
                    btnGenerateLayout.Enabled = true;
                    return;
            }

            if ( Generator.TopLeftPlayerCount * Generator.SymmetryBlockCount < 2 )
            {
                MessageBox.Show("That configuration only produces 1 player.");
                btnGenerateLayout.Enabled = true;
                return;
            }
            if ( Generator.TopLeftPlayerCount * Generator.SymmetryBlockCount > 10 )
            {
                MessageBox.Show("That configuration produces more than 10 players.");
                btnGenerateLayout.Enabled = true;
                return;
            }

            Generator.TileSize.X = ValidateTextbox(txtWidth, 48.0D, 250.0D, 1.0D);
            Generator.TileSize.Y = ValidateTextbox(txtHeight, 48.0D, 250.0D, 1.0D);
            if ( Generator.SymmetryBlockCount == 4 )
            {
                if ( Generator.TileSize.X != Generator.TileSize.Y && Generator.SymmetryIsRotational )
                {
                    MessageBox.Show("Width and height must be equal if map is rotated on two axes.");
                    btnGenerateLayout.Enabled = true;
                    return;
                }
            }
            Generator.PlayerBasePos = new XYInt[Generator.TopLeftPlayerCount];
            var BaseMin = 12.0D;
            var BaseMax =
                new XYDouble(Math.Min((double)Generator.TileSize.X / Generator.SymmetryBlockCountXY.X, Generator.TileSize.X - 12.0D),
                    Math.Min((double)Generator.TileSize.Y / Generator.SymmetryBlockCountXY.Y, Generator.TileSize.Y - 12.0D));
            Generator.PlayerBasePos[0] = new XYInt(ValidateTextbox(txt1x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                ValidateTextbox(txt1y, BaseMin, BaseMax.X, Constants.TerrainGridSpacing));
            if ( Generator.TopLeftPlayerCount >= 2 )
            {
                Generator.PlayerBasePos[1] = new XYInt(ValidateTextbox(txt2x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                    ValidateTextbox(txt2y, BaseMin, BaseMax.Y, Constants.TerrainGridSpacing));
                if ( Generator.TopLeftPlayerCount >= 3 )
                {
                    Generator.PlayerBasePos[2] = new XYInt(ValidateTextbox(txt3x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                        ValidateTextbox(txt3y, BaseMin, BaseMax.Y, Constants.TerrainGridSpacing));
                    if ( Generator.TopLeftPlayerCount >= 4 )
                    {
                        Generator.PlayerBasePos[3] = new XYInt(ValidateTextbox(txt4x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                            ValidateTextbox(txt4y, BaseMin, BaseMax.Y, Constants.TerrainGridSpacing));
                        if ( Generator.TopLeftPlayerCount >= 5 )
                        {
                            Generator.PlayerBasePos[4] = new XYInt(ValidateTextbox(txt5x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                                ValidateTextbox(txt5y, BaseMin, BaseMax.Y, Constants.TerrainGridSpacing));
                            if ( Generator.TopLeftPlayerCount >= 6 )
                            {
                                Generator.PlayerBasePos[5] = new XYInt(ValidateTextbox(txt6x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                                    ValidateTextbox(txt6y, BaseMin, BaseMax.Y, Constants.TerrainGridSpacing));
                                if ( Generator.TopLeftPlayerCount >= 7 )
                                {
                                    Generator.PlayerBasePos[6] = new XYInt(ValidateTextbox(txt7x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                                        ValidateTextbox(txt7y, BaseMin, BaseMax.Y, Constants.TerrainGridSpacing));
                                    if ( Generator.TopLeftPlayerCount >= 8 )
                                    {
                                        Generator.PlayerBasePos[7] = new XYInt(ValidateTextbox(txt8x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                                            ValidateTextbox(txt8y, BaseMin, BaseMax.Y, Constants.TerrainGridSpacing));
                                        if ( Generator.TopLeftPlayerCount >= 9 )
                                        {
                                            Generator.PlayerBasePos[8] = new XYInt(
                                                ValidateTextbox(txt9x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                                                ValidateTextbox(txt9y, BaseMin, BaseMax.Y, Constants.TerrainGridSpacing));
                                            if ( Generator.TopLeftPlayerCount >= 10 )
                                            {
                                                Generator.PlayerBasePos[9] =
                                                    new XYInt(ValidateTextbox(txt10x, BaseMin, BaseMax.X, Constants.TerrainGridSpacing),
                                                        ValidateTextbox(txt10y, BaseMin, BaseMax.Y, Constants.TerrainGridSpacing));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Generator.LevelCount = ValidateTextbox(txtLevels, 3.0D, 5.0D, 1.0D);
            Generator.BaseLevel = ValidateTextbox(txtBaseLevel, -1.0D, Generator.LevelCount - 1, 1.0D);
            Generator.JitterScale = 1;
            Generator.MaxLevelTransition = 2;
            Generator.PassagesChance = ValidateTextbox(txtLevelFrequency, 0.0D, 100.0D, 1.0D);
            Generator.VariationChance = ValidateTextbox(txtVariation, 0.0D, 100.0D, 1.0D);
            Generator.FlatsChance = ValidateTextbox(txtFlatness, 0.0D, 100.0D, 1.0D);
            Generator.BaseFlatArea = ValidateTextbox(txtBaseArea, 1.0D, 16.0D, 1.0D);
            Generator.NodeScale = 4.0F;
            Generator.WaterSpawnQuantity = ValidateTextbox(txtWaterQuantity, 0.0D, 9999.0D, 1.0D);
            Generator.TotalWaterQuantity = ValidateTextbox(txtConnectedWater, 0.0D, 9999.0D, 1.0D);

            Application.DoEvents();
            LoopCount = 0;
            var Result = default(clsResult);
            do
            {
                Result = new clsResult("", false);
                Result = Generator.GenerateLayout();
                if ( !Result.HasProblems )
                {
                    var HeightsResult = FinishHeights();
                    Result.Add(HeightsResult);
                    if ( !HeightsResult.HasProblems )
                    {
                        lstResult_AddResult(Result);
                        lstResult_AddText("Done.");
                        btnGenerateLayout.Enabled = true;
                        break;
                    }
                }
                LoopCount++;
                lstResult_AddText("Attempt " + Convert.ToString(LoopCount) + " failed.");
                Application.DoEvents();
                if ( StopTrying )
                {
                    Generator.Map = null;
                    lstResult_AddResult(Result);
                    lstResult_AddText("Stopped.");
                    btnGenerateLayout.Enabled = true;
                    return;
                }
                lstResult_AddResult(Result);
                lstResult_AddText("Retrying...");
                Application.DoEvents();
                Generator.ClearLayout();
            } while ( true );
            lstResult_AddResult(Result);
        }

        private clsResult FinishHeights()
        {
            var ReturnResult = new clsResult("", false);

            ReturnResult.Take(Generator.GenerateLayoutTerrain());
            if ( ReturnResult.HasProblems )
            {
                return ReturnResult;
            }

            Generator.Map.RandomizeHeights(Generator.LevelCount);

            Generator.Map.InterfaceOptions = new clsInterfaceOptions();
            Generator.Map.InterfaceOptions.CompileMultiPlayers = Generator.GetTotalPlayerCount.ToStringInvariant();

            _Owner.NewMainMap(Generator.Map);

            return ReturnResult;
        }

        private clsResult FinishTextures()
        {
            var ReturnResult = new clsResult("", false);

            if ( cbxMasterTexture.Checked )
            {
                switch ( cboTileset.SelectedIndex )
                {
                    case 0:
                        Generator.GenerateTileset = DefaultGenerator.Generator_TilesetArizona;
                        DefaultGenerator.TerrainStyle_Arizona.Watermap = Generator.GetWaterMap();
                        DefaultGenerator.TerrainStyle_Arizona.LevelCount = Generator.LevelCount;
                        Generator.Map.GenerateMasterTerrain(ref DefaultGenerator.TerrainStyle_Arizona);
                        DefaultGenerator.TerrainStyle_Arizona.Watermap = null;
                        break;
                    case 1:
                        Generator.GenerateTileset = DefaultGenerator.Generator_TilesetUrban;
                        DefaultGenerator.TerrainStyle_Urban.Watermap = Generator.GetWaterMap();
                        DefaultGenerator.TerrainStyle_Urban.LevelCount = Generator.LevelCount;
                        Generator.Map.GenerateMasterTerrain(ref DefaultGenerator.TerrainStyle_Urban);
                        DefaultGenerator.TerrainStyle_Urban.Watermap = null;
                        break;
                    case 2:
                        Generator.GenerateTileset = DefaultGenerator.Generator_TilesetRockies;
                        DefaultGenerator.TerrainStyle_Rockies.Watermap = Generator.GetWaterMap();
                        DefaultGenerator.TerrainStyle_Rockies.LevelCount = Generator.LevelCount;
                        Generator.Map.GenerateMasterTerrain(ref DefaultGenerator.TerrainStyle_Rockies);
                        DefaultGenerator.TerrainStyle_Rockies.Watermap = null;
                        break;
                    default:
                        ReturnResult.ProblemAdd("Error: bad tileset selection.");
                        btnGenerateLayout.Enabled = true;
                        return ReturnResult;
                }
                Generator.Map.TileType_Reset();
                Generator.Map.SetPainterToDefaults();
            }
            else
            {
                switch ( cboTileset.SelectedIndex )
                {
                    case 0:
                        Generator.Map.Tileset = App.Tileset_Arizona;
                        Generator.GenerateTileset = DefaultGenerator.Generator_TilesetArizona;
                        break;
                    case 1:
                        Generator.Map.Tileset = App.Tileset_Urban;
                        Generator.GenerateTileset = DefaultGenerator.Generator_TilesetUrban;
                        break;
                    case 2:
                        Generator.Map.Tileset = App.Tileset_Rockies;
                        Generator.GenerateTileset = DefaultGenerator.Generator_TilesetRockies;
                        break;
                    default:
                        ReturnResult.ProblemAdd("Error: bad tileset selection.");
                        btnGenerateLayout.Enabled = true;
                        return ReturnResult;
                }
                Generator.Map.TileType_Reset();
                Generator.Map.SetPainterToDefaults();
                var CliffAngle = Math.Atan(255.0D * Generator.Map.HeightMultiplier / (2.0D * (Generator.LevelCount - 1.0D) * Constants.TerrainGridSpacing)) -
                                 MathUtil.RadOf1Deg;
                var tmpBrush = new clsBrush((Math.Max(Generator.Map.Terrain.TileSize.X, Generator.Map.Terrain.TileSize.Y)) * 1.1D, clsBrush.enumShape.Square);
                var ApplyCliff = new clsApplyCliff();
                ApplyCliff.Map = Generator.Map;
                ApplyCliff.Angle = CliffAngle;
                ApplyCliff.SetTris = true;
                var Alignments = new clsBrush.sPosNum();
                Alignments.Normal = new XYInt((int)(Generator.Map.Terrain.TileSize.X / 2.0D),
                    (int)(Generator.Map.Terrain.TileSize.Y / 2.0D));
                Alignments.Alignment = Alignments.Normal;
                tmpBrush.PerformActionMapTiles(ApplyCliff, Alignments);
                bool[] RevertSlope = null;
                bool[] RevertHeight = null;
                var WaterMap = new BooleanMap();
                var bmTemp = new BooleanMap();
                var A = 0;
                WaterMap = Generator.GetWaterMap();
                RevertSlope = new bool[Generator.GenerateTileset.OldTextureLayers.LayerCount];
                RevertHeight = new bool[Generator.GenerateTileset.OldTextureLayers.LayerCount];
                for ( A = 0; A <= Generator.GenerateTileset.OldTextureLayers.LayerCount - 1; A++ )
                {
                    var with_2 = Generator.GenerateTileset.OldTextureLayers.Layers[A];
                    with_2.Terrainmap = Generator.Map.GenerateTerrainMap(with_2.Scale, with_2.Density);
                    if ( with_2.SlopeMax < 0.0F )
                    {
                        with_2.SlopeMax = (float)(CliffAngle - MathUtil.RadOf1Deg);
                        if ( with_2.HeightMax < 0.0F )
                        {
                            with_2.HeightMax = 255.0F;
                            bmTemp.Within(with_2.Terrainmap, WaterMap);
                            with_2.Terrainmap.ValueData = bmTemp.ValueData;
                            bmTemp.ValueData = new BooleanMapDataValue();
                            RevertHeight[A] = true;
                        }
                        RevertSlope[A] = true;
                    }
                }
                Generator.Map.MapTexturer(ref Generator.GenerateTileset.OldTextureLayers);
                for ( A = 0; A <= Generator.GenerateTileset.OldTextureLayers.LayerCount - 1; A++ )
                {
                    var with_3 = Generator.GenerateTileset.OldTextureLayers.Layers[A];
                    with_3.Terrainmap = null;
                    if ( RevertSlope[A] )
                    {
                        with_3.SlopeMax = -1.0F;
                    }
                    if ( RevertHeight[A] )
                    {
                        with_3.HeightMax = -1.0F;
                    }
                }
            }

            Generator.Map.LevelWater();

            Generator.Map.WaterTriCorrection();

            Generator.Map.SectorGraphicsChanges.SetAllChanged();
            Generator.Map.SectorUnitHeightsChanges.SetAllChanged();

            Generator.Map.Update();

            Generator.Map.UndoStepCreate("Generated Textures");

            if ( Generator.Map == _Owner.MainMap )
            {
                Program.frmMainInstance.PainterTerrains_Refresh(-1, -1);
                Program.frmMainInstance.MainMapTilesetChanged();
            }

            return ReturnResult;
        }

        public void btnGenerateObjects_Click(Object sender, EventArgs e)
        {
            if ( Generator.Map == null || Generator.GenerateTileset == null )
            {
                return;
            }
            if ( !Generator.Map.frmMainLink.IsConnected )
            {
                return;
            }

            Generator.BaseOilCount = ValidateTextbox(txtBaseOil, 0.0D, 16.0D, 1.0D);
            Generator.ExtraOilCount = ValidateTextbox(txtOilElsewhere, 0.0D, 9999.0D, 1.0D);
            Generator.ExtraOilClusterSizeMax = ValidateTextbox(txtOilClusterMax, 0.0D, 99.0D, 1.0D);
            Generator.ExtraOilClusterSizeMin = ValidateTextbox(txtOilClusterMin, 0.0D, Generator.ExtraOilClusterSizeMax, 1.0D);
            Generator.OilDispersion = ValidateTextbox(txtOilDispersion, 0.0D, 9999.0D, 1.0D) / 100.0F;
            Generator.OilAtATime = ValidateTextbox(txtOilAtATime, 1.0D, 2.0D, 1.0D);

            Generator.FeatureClusterChance = ValidateTextbox(txtFClusterChance, 0.0D, 100.0D, 1.0D) / 100.0F;
            Generator.FeatureClusterMaxUnits = ValidateTextbox(txtFClusterMax, 0.0D, 99.0D, 1.0D);
            Generator.FeatureClusterMinUnits = ValidateTextbox(txtFClusterMin, 0.0D, Generator.FeatureClusterMaxUnits, 1.0D);
            Generator.FeatureScatterCount = ValidateTextbox(txtFScatterChance, 0.0D, 99999.0D, 1.0D);
            Generator.FeatureScatterGap = ValidateTextbox(txtFScatterGap, 0.0D, 99999.0D, 1.0D);
            Generator.BaseTruckCount = ValidateTextbox(txtTrucks, 0.0D, 15.0D, 1.0D);

            Generator.GenerateTilePathMap();

            Generator.TerrainBlockPaths();

            Generator.BlockEdgeTiles();

            Generator.GenerateGateways();

            lstResult_AddText("Generating objects.");
            var Result = new clsResult("", false);
            Result.Take(Generator.GenerateOil());
            Result.Take(Generator.GenerateUnits());
            lstResult_AddResult(Result);
            if ( Result.HasProblems )
            {
                lstResult_AddText("Failed.");
            }
            else
            {
                lstResult_AddText("Done.");
            }

            Generator.Map.SectorGraphicsChanges.SetAllChanged();
            Generator.Map.Update();
            Generator.Map.UndoStepCreate("Generator objects");
        }

        public void btnGenerateRamps_Click(Object sender, EventArgs e)
        {
            if ( Generator.Map == null )
            {
                return;
            }

            Generator.MaxDisconnectionDist = ValidateTextbox(txtRampDistance, 0.0D, 99999.0D, Constants.TerrainGridSpacing);
            Generator.RampBase = ValidateTextbox(txtRampBase, 10.0D, 1000.0D, 10.0D) / 1000.0D;

            var Result = new clsResult("", false);

            lstResult_AddText("Generating ramps.");
            Result = Generator.GenerateRamps();
            if ( !Result.HasProblems )
            {
                Result.Add(FinishHeights());
            }
            lstResult_AddResult(Result);
            if ( Result.HasProblems )
            {
                lstResult_AddText("Failed.");
            }
            lstResult_AddText("Done.");
        }

        public void btnGenerateTextures_Click(Object sender, EventArgs e)
        {
            if ( Generator.Map == null )
            {
                return;
            }
            if ( !Generator.Map.frmMainLink.IsConnected )
            {
                return;
            }

            lstResult_AddResult(FinishTextures());
            Program.frmMainInstance.View_DrawViewLater();
        }

        public void frmGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        public void frmWZMapGen_Load(object sender, EventArgs e)
        {
            cboTileset.SelectedIndex = 0;
            cboSymmetry.SelectedIndex = 0;
        }

        public void rdoPlayer2_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer2.Checked )
            {
                PlayerCount = 2;
                rdoPlayer1.Checked = false;
                rdoPlayer3.Checked = false;
                rdoPlayer4.Checked = false;
                rdoPlayer5.Checked = false;
                rdoPlayer6.Checked = false;
                rdoPlayer7.Checked = false;
                rdoPlayer8.Checked = false;
                rdoPlayer9.Checked = false;
                rdoPlayer10.Checked = false;
            }
        }

        public void rdoPlayer3_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer3.Checked )
            {
                PlayerCount = 3;
                rdoPlayer1.Checked = false;
                rdoPlayer2.Checked = false;
                rdoPlayer4.Checked = false;
                rdoPlayer5.Checked = false;
                rdoPlayer6.Checked = false;
                rdoPlayer7.Checked = false;
                rdoPlayer8.Checked = false;
                rdoPlayer9.Checked = false;
                rdoPlayer10.Checked = false;
            }
        }

        public void rdoPlayer4_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer4.Checked )
            {
                PlayerCount = 4;
                rdoPlayer1.Checked = false;
                rdoPlayer2.Checked = false;
                rdoPlayer3.Checked = false;
                rdoPlayer5.Checked = false;
                rdoPlayer6.Checked = false;
                rdoPlayer7.Checked = false;
                rdoPlayer8.Checked = false;
                rdoPlayer9.Checked = false;
                rdoPlayer10.Checked = false;
            }
        }

        public void rdoPlayer5_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer5.Checked )
            {
                PlayerCount = 5;
                rdoPlayer1.Checked = false;
                rdoPlayer2.Checked = false;
                rdoPlayer3.Checked = false;
                rdoPlayer4.Checked = false;
                rdoPlayer6.Checked = false;
                rdoPlayer7.Checked = false;
                rdoPlayer8.Checked = false;
                rdoPlayer9.Checked = false;
                rdoPlayer10.Checked = false;
            }
        }

        public void rdoPlayer6_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer6.Checked )
            {
                PlayerCount = 6;
                rdoPlayer1.Checked = false;
                rdoPlayer2.Checked = false;
                rdoPlayer3.Checked = false;
                rdoPlayer4.Checked = false;
                rdoPlayer5.Checked = false;
                rdoPlayer7.Checked = false;
                rdoPlayer8.Checked = false;
                rdoPlayer9.Checked = false;
                rdoPlayer10.Checked = false;
            }
        }

        public void rdoPlayer7_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer7.Checked )
            {
                PlayerCount = 7;
                rdoPlayer1.Checked = false;
                rdoPlayer2.Checked = false;
                rdoPlayer3.Checked = false;
                rdoPlayer4.Checked = false;
                rdoPlayer5.Checked = false;
                rdoPlayer6.Checked = false;
                rdoPlayer8.Checked = false;
                rdoPlayer9.Checked = false;
                rdoPlayer10.Checked = false;
            }
        }

        public void rdoPlayer8_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer8.Checked )
            {
                PlayerCount = 8;
                rdoPlayer1.Checked = false;
                rdoPlayer2.Checked = false;
                rdoPlayer3.Checked = false;
                rdoPlayer4.Checked = false;
                rdoPlayer5.Checked = false;
                rdoPlayer6.Checked = false;
                rdoPlayer7.Checked = false;
                rdoPlayer9.Checked = false;
                rdoPlayer10.Checked = false;
            }
        }

        public void btnStop_Click(Object sender, EventArgs e)
        {
            StopTrying = true;
        }

        public void rdoPlayer1_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer1.Checked )
            {
                PlayerCount = 1;
                rdoPlayer2.Checked = false;
                rdoPlayer3.Checked = false;
                rdoPlayer4.Checked = false;
                rdoPlayer5.Checked = false;
                rdoPlayer6.Checked = false;
                rdoPlayer7.Checked = false;
                rdoPlayer8.Checked = false;
                rdoPlayer9.Checked = false;
                rdoPlayer10.Checked = false;
            }
        }

        public void rdoPlayer9_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer9.Checked )
            {
                PlayerCount = 9;
                rdoPlayer1.Checked = false;
                rdoPlayer2.Checked = false;
                rdoPlayer3.Checked = false;
                rdoPlayer4.Checked = false;
                rdoPlayer5.Checked = false;
                rdoPlayer6.Checked = false;
                rdoPlayer7.Checked = false;
                rdoPlayer8.Checked = false;
                rdoPlayer10.Checked = false;
            }
        }

        public void rdoPlayer10_CheckedChanged(Object sender, EventArgs e)
        {
            if ( rdoPlayer10.Checked )
            {
                PlayerCount = 10;
                rdoPlayer1.Checked = false;
                rdoPlayer2.Checked = false;
                rdoPlayer3.Checked = false;
                rdoPlayer4.Checked = false;
                rdoPlayer5.Checked = false;
                rdoPlayer6.Checked = false;
                rdoPlayer7.Checked = false;
                rdoPlayer8.Checked = false;
                rdoPlayer9.Checked = false;
            }
        }

        private void lstResult_AddResult(clsResult result)
        {
            lstResult.SelectedIndex = lstResult.Items.Count - 1;
        }

        private void lstResult_AddText(string Text)
        {
            lstResult.Items.Add(Text);
            lstResult.SelectedIndex = lstResult.Items.Count - 1;
        }
    }
}