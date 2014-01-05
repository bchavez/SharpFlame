using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;
using SharpFlame.Collections;
using SharpFlame.Collections.Specialized;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;
using SharpFlame.Painters;

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public void Rotate(TileOrientation Orientation, App.enumObjectRotateMode ObjectRotateMode)
        {
            int X = 0;
            int Y = 0;
            sXY_int Pos = new sXY_int();
            sXY_int RotatedPos = new sXY_int();
            sXY_int NewTerrainPosA = TileUtil.GetRotatedPos(Orientation, new sXY_int(0, 0), Terrain.TileSize);
            sXY_int NewTerrainPosB = TileUtil.GetRotatedPos(Orientation, Terrain.TileSize, Terrain.TileSize);
            sXY_int VertexLimits = new sXY_int(Math.Max(NewTerrainPosA.X, NewTerrainPosB.X), Math.Max(NewTerrainPosA.Y, NewTerrainPosB.Y));
            clsTerrain NewTerrain = new clsTerrain(VertexLimits);
            sXY_int NewTileLimits = new sXY_int(NewTerrain.TileSize.X - 1, NewTerrain.TileSize.Y - 1);
            sXY_int NewSideHLimits = new sXY_int(NewTerrain.TileSize.X - 1, NewTerrain.TileSize.Y);
            sXY_int NewSideVLimits = new sXY_int(NewTerrain.TileSize.X, NewTerrain.TileSize.Y - 1);
            sXY_int OldTileLimits = new sXY_int(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1);
            sXY_int OldPosLimits = new sXY_int(Terrain.TileSize.X * App.TerrainGridSpacing, Terrain.TileSize.Y * App.TerrainGridSpacing);
            TileOrientation ReverseOrientation = new TileOrientation();
            TileDirection TriDirection = new TileDirection();

            ReverseOrientation = Orientation;
            ReverseOrientation.Reverse();

            for ( Y = 0; Y <= NewTerrain.TileSize.Y; Y++ )
            {
                Pos.Y = Y;
                for ( X = 0; X <= NewTerrain.TileSize.X; X++ )
                {
                    Pos.X = X;
                    RotatedPos = TileUtil.GetRotatedPos(ReverseOrientation, Pos, VertexLimits);
                    NewTerrain.Vertices[X, Y].Height = Terrain.Vertices[RotatedPos.X, RotatedPos.Y].Height;
                    NewTerrain.Vertices[X, Y].Terrain = Terrain.Vertices[RotatedPos.X, RotatedPos.Y].Terrain;
                }
            }
            for ( Y = 0; Y <= NewTerrain.TileSize.Y - 1; Y++ )
            {
                Pos.Y = Y;
                for ( X = 0; X <= NewTerrain.TileSize.X - 1; X++ )
                {
                    Pos.X = X;
                    RotatedPos = TileUtil.GetRotatedPos(ReverseOrientation, Pos, NewTileLimits);
                    NewTerrain.Tiles[X, Y].Texture = Terrain.Tiles[RotatedPos.X, RotatedPos.Y].Texture;
                    NewTerrain.Tiles[X, Y].Texture.Orientation = NewTerrain.Tiles[X, Y].Texture.Orientation.GetRotated(Orientation);
                    NewTerrain.Tiles[X, Y].DownSide = Terrain.Tiles[RotatedPos.X, RotatedPos.Y].DownSide;
                    NewTerrain.Tiles[X, Y].DownSide = NewTerrain.Tiles[X, Y].DownSide.GetRotated(Orientation);
                    if ( Terrain.Tiles[RotatedPos.X, RotatedPos.Y].Tri )
                    {
                        TriDirection = TileUtil.TopLeft;
                    }
                    else
                    {
                        TriDirection = TileUtil.TopRight;
                    }
                    TriDirection = TriDirection.GetRotated(Orientation);
                    NewTerrain.Tiles[X, Y].Tri =
                        Convert.ToBoolean(TileUtil.IdenticalTileDirections(TriDirection, TileUtil.TopLeft) ||
                                                 TileUtil.IdenticalTileDirections(TriDirection, TileUtil.BottomRight));
                    if ( Terrain.Tiles[RotatedPos.X, RotatedPos.Y].Tri )
                    {
                        if ( Terrain.Tiles[RotatedPos.X, RotatedPos.Y].TriTopLeftIsCliff )
                        {
                            TileUtil.RotateDirection(TileUtil.TopLeft, Orientation, ref TriDirection);
                            NewTerrain.Tiles[X, Y].TriCliffAddDirection(TriDirection);
                        }
                        if ( Terrain.Tiles[RotatedPos.X, RotatedPos.Y].TriBottomRightIsCliff )
                        {
                            TileUtil.RotateDirection(TileUtil.BottomRight, Orientation, ref TriDirection);
                            NewTerrain.Tiles[X, Y].TriCliffAddDirection(TriDirection);
                        }
                    }
                    else
                    {
                        if ( Terrain.Tiles[RotatedPos.X, RotatedPos.Y].TriTopRightIsCliff )
                        {
                            TileUtil.RotateDirection(TileUtil.TopRight, Orientation, ref TriDirection);
                            NewTerrain.Tiles[X, Y].TriCliffAddDirection(TriDirection);
                        }
                        if ( Terrain.Tiles[RotatedPos.X, RotatedPos.Y].TriBottomLeftIsCliff )
                        {
                            TileUtil.RotateDirection(TileUtil.BottomLeft, Orientation, ref TriDirection);
                            NewTerrain.Tiles[X, Y].TriCliffAddDirection(TriDirection);
                        }
                    }
                    NewTerrain.Tiles[X, Y].Terrain_IsCliff = Terrain.Tiles[RotatedPos.X, RotatedPos.Y].Terrain_IsCliff;
                }
            }
            if ( Orientation.SwitchedAxes )
            {
                for ( Y = 0; Y <= NewTerrain.TileSize.Y; Y++ )
                {
                    Pos.Y = Y;
                    for ( X = 0; X <= NewTerrain.TileSize.X - 1; X++ )
                    {
                        Pos.X = X;
                        RotatedPos = TileUtil.GetRotatedPos(ReverseOrientation, Pos, NewSideHLimits);
                        NewTerrain.SideH[X, Y].Road = Terrain.SideV[RotatedPos.X, RotatedPos.Y].Road;
                    }
                }
                for ( Y = 0; Y <= NewTerrain.TileSize.Y - 1; Y++ )
                {
                    Pos.Y = Y;
                    for ( X = 0; X <= NewTerrain.TileSize.X; X++ )
                    {
                        Pos.X = X;
                        RotatedPos = TileUtil.GetRotatedPos(ReverseOrientation, Pos, NewSideVLimits);
                        NewTerrain.SideV[X, Y].Road = Terrain.SideH[RotatedPos.X, RotatedPos.Y].Road;
                    }
                }
            }
            else
            {
                for ( Y = 0; Y <= NewTerrain.TileSize.Y; Y++ )
                {
                    Pos.Y = Y;
                    for ( X = 0; X <= NewTerrain.TileSize.X - 1; X++ )
                    {
                        Pos.X = X;
                        RotatedPos = TileUtil.GetRotatedPos(ReverseOrientation, Pos, NewSideHLimits);
                        NewTerrain.SideH[X, Y].Road = Terrain.SideH[RotatedPos.X, RotatedPos.Y].Road;
                    }
                }
                for ( Y = 0; Y <= NewTerrain.TileSize.Y - 1; Y++ )
                {
                    Pos.Y = Y;
                    for ( X = 0; X <= NewTerrain.TileSize.X; X++ )
                    {
                        Pos.X = X;
                        RotatedPos = TileUtil.GetRotatedPos(ReverseOrientation, Pos, NewSideVLimits);
                        NewTerrain.SideV[X, Y].Road = Terrain.SideV[RotatedPos.X, RotatedPos.Y].Road;
                    }
                }
            }

            clsUnit Unit = default(clsUnit);
            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                Unit.Sectors.Clear();
                if ( ObjectRotateMode == App.enumObjectRotateMode.All )
                {
                    Unit.Rotation =
                        (int)
                            (MathUtil.AngleClamp(MathUtil.RadOf360Deg -
                                                TileUtil.GetRotatedAngle(Orientation,
                                                    MathUtil.AngleClamp(MathUtil.RadOf360Deg - Unit.Rotation * MathUtil.RadOf1Deg))) / MathUtil.RadOf1Deg);
                    if ( Unit.Rotation < 0 )
                    {
                        Unit.Rotation += 360;
                    }
                }
                else if ( ObjectRotateMode == App.enumObjectRotateMode.Walls )
                {
                    if ( Unit.Type.Type == clsUnitType.enumType.PlayerStructure )
                    {
                        if ( ((clsStructureType)Unit.Type).StructureType == clsStructureType.enumStructureType.Wall )
                        {
                            Unit.Rotation =
                                (int)
                                    (MathUtil.AngleClamp(MathUtil.RadOf360Deg -
                                                        TileUtil.GetRotatedAngle(Orientation,
                                                            MathUtil.AngleClamp(MathUtil.RadOf360Deg - Unit.Rotation * MathUtil.RadOf1Deg))) / MathUtil.RadOf1Deg);
                            if ( Unit.Rotation < 0 )
                            {
                                Unit.Rotation += 360;
                            }
                            //If Unit.Rotation = 180 Then
                            //    Unit.Rotation = 0
                            //ElseIf Unit.Rotation = 270 Then
                            //    Unit.Rotation = 90
                            //End If
                        }
                    }
                }
                Unit.Pos.Horizontal = TileUtil.GetRotatedPos(Orientation, Unit.Pos.Horizontal, OldPosLimits);
            }

            sXY_int ZeroPos = new sXY_int(0, 0);

            int Position = 0;
            foreach ( clsUnit tempLoopVar_Unit in Units.GetItemsAsSimpleList() )
            {
                Unit = tempLoopVar_Unit;
                if ( !App.PosIsWithinTileArea(Unit.Pos.Horizontal, ZeroPos, NewTerrain.TileSize) )
                {
                    Position = Unit.MapLink.ArrayPosition;
                    UnitRemove(Position);
                }
            }

            Terrain = NewTerrain;

            clsGateway Gateway = default(clsGateway);
            foreach ( clsGateway tempLoopVar_Gateway in Gateways.GetItemsAsSimpleClassList() )
            {
                Gateway = tempLoopVar_Gateway;
                GatewayCreate(TileUtil.GetRotatedPos(Orientation, Gateway.PosA, OldTileLimits),
                    TileUtil.GetRotatedPos(Orientation, Gateway.PosB, OldTileLimits));
                Gateway.Deallocate();
            }

            if ( _ReadyForUserInput )
            {
                CancelUserInput();
                InitializeUserInput();
            }
        }

        public void RandomizeHeights(int LevelCount)
        {
            clsHeightmap hmSource = new clsHeightmap();
            clsHeightmap hmA = new clsHeightmap();
            clsHeightmap hmB = new clsHeightmap();
            int IntervalCount = 0;
            clsHeightmap.sHeights AlterationLevels = new clsHeightmap.sHeights();
            sHeightmaps hmAlteration = new sHeightmaps();
            float LevelHeight = 0;
            double HeightRange = 0;
            int Level = 0;
            double IntervalHeight = 0;
            double Variation = 0;
            int X = 0;
            int Y = 0;

            IntervalCount = LevelCount - 1;

            AlterationLevels.Heights = new float[IntervalCount + 1];
            clsHeightmap.sMinMax MinMax = new clsHeightmap.sMinMax();
            hmAlteration.Heightmaps = new clsHeightmap[IntervalCount + 1];
            hmSource.HeightData.Height = new long[Terrain.TileSize.Y + 1, Terrain.TileSize.X + 1];
            hmSource.HeightData.SizeX = Terrain.TileSize.X + 1;
            hmSource.HeightData.SizeY = Terrain.TileSize.Y + 1;
            for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X; X++ )
                {
                    hmSource.HeightData.Height[Y, X] = Convert.ToInt32(Terrain.Vertices[X, Y].Height / hmSource.HeightScale);
                }
            }
            hmSource.MinMaxGet(MinMax);
            HeightRange = 255.0D;
            IntervalHeight = HeightRange / IntervalCount;
            Variation = IntervalHeight / 4.0D;
            for ( Level = 0; Level <= IntervalCount; Level++ )
            {
                LevelHeight =
                    Convert.ToSingle(Convert.ToDouble(MinMax.Min + Convert.ToInt32(Level * MinMax.Max / IntervalCount)) * hmSource.HeightScale);
                AlterationLevels.Heights[Level] = LevelHeight;
                hmB.GenerateNewOfSize(Terrain.TileSize.Y + 1, Terrain.TileSize.X + 1, 2.0F, 10000.0D);
                hmAlteration.Heightmaps[Level] = new clsHeightmap();
                hmAlteration.Heightmaps[Level].Rescale(hmB, LevelHeight - Variation, LevelHeight + Variation);
            }
            hmA.FadeMultiple(hmSource, hmAlteration, AlterationLevels);
            hmB.Rescale(hmA, Math.Max(Convert.ToDouble(Convert.ToDouble(MinMax.Min * hmSource.HeightScale) - Variation), 0.0D),
                Math.Min(Convert.ToDouble(Convert.ToDouble(MinMax.Max * hmSource.HeightScale) + Variation), 255.9D));
            for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X; X++ )
                {
                    Terrain.Vertices[X, Y].Height = Convert.ToByte(Conversion.Int(hmB.HeightData.Height[Y, X] * hmB.HeightScale));
                }
            }
        }

        public void LevelWater()
        {
            int X = 0;
            int Y = 0;
            int TextureNum = 0;

            if ( Tileset == null )
            {
                return;
            }

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    TextureNum = Terrain.Tiles[X, Y].Texture.TextureNum;
                    if ( TextureNum >= 0 & TextureNum < Tileset.TileCount )
                    {
                        if ( Tileset.Tiles[TextureNum].Default_Type == App.TileTypeNum_Water )
                        {
                            Terrain.Vertices[X, Y].Height = (byte)0;
                            Terrain.Vertices[X + 1, Y].Height = (byte)0;
                            Terrain.Vertices[X, Y + 1].Height = (byte)0;
                            Terrain.Vertices[X + 1, Y + 1].Height = (byte)0;
                        }
                    }
                }
            }
        }

        public struct sGenerateMasterTerrainArgs
        {
            public clsGeneratorTileset Tileset;
            public int LevelCount;

            public class clsLayer
            {
                public int WithinLayer;
                public bool[] AvoidLayers;
                public int TileNum;
                public BooleanMap Terrainmap;
                public float TerrainmapScale;
                public float TerrainmapDensity;
                public float HeightMin;
                public float HeightMax;
                public bool IsCliff;
            }

            public clsLayer[] Layers;
            public int LayerCount;
            public BooleanMap Watermap;
        }

        public void GenerateMasterTerrain(sGenerateMasterTerrainArgs Args)
        {
            int X = 0;
            int Y = 0;
            int A = 0;
            int[,] TerrainType = null;
            float[,] Slope = null;

            int TerrainNum = 0;

            BooleanMap bmA = new BooleanMap();
            int Layer_Num = 0;
            BooleanMap[] LayerResult = new BooleanMap[Args.LayerCount];
            BooleanMap bmB = new BooleanMap();
            double BestSlope = 0;
            double CurrentSlope = 0;
            clsHeightmap hmB = new clsHeightmap();
            clsHeightmap hmC = new clsHeightmap();

            double difA = 0;
            double difB = 0;
            bool NewTri = default(bool);
            double CliffSlope = Math.Atan(255.0D * Constants.DefaultHeightMultiplier / (2.0D * (Args.LevelCount - 1.0D) * App.TerrainGridSpacing)) -
                                MathUtil.RadOf1Deg; //divided by 2 due to the terrain height randomization

            Tileset = Args.Tileset.Tileset;

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    difA = Math.Abs((Terrain.Vertices[X + 1, Y + 1].Height) - Terrain.Vertices[X, Y].Height);
                    difB = Math.Abs((Terrain.Vertices[X, Y + 1].Height) - Terrain.Vertices[X + 1, Y].Height);
                    if ( difA == difB )
                    {
                        if ( VBMath.Rnd() >= 0.5F )
                        {
                            NewTri = false;
                        }
                        else
                        {
                            NewTri = true;
                        }
                    }
                    else if ( difA < difB )
                    {
                        NewTri = false;
                    }
                    else
                    {
                        NewTri = true;
                    }
                    if ( !(Terrain.Tiles[X, Y].Tri == NewTri) )
                    {
                        Terrain.Tiles[X, Y].Tri = NewTri;
                    }
                }
            }

            for ( A = 0; A <= Args.LayerCount - 1; A++ )
            {
                Args.Layers[A].Terrainmap = new BooleanMap();
                if ( Args.Layers[A].TerrainmapDensity == 1.0F )
                {
                    Args.Layers[A].Terrainmap.ValueData.Value = new bool[Terrain.TileSize.Y, Terrain.TileSize.X];
                    Args.Layers[A].Terrainmap.ValueData.Size = Terrain.TileSize;
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            Args.Layers[A].Terrainmap.ValueData.Value[Y, X] = true;
                        }
                    }
                }
                else
                {
                    hmB.GenerateNewOfSize(Terrain.TileSize.Y, Terrain.TileSize.X, Convert.ToSingle(Args.Layers[A].TerrainmapScale), 1.0D);
                    hmC.Rescale(hmB, 0.0D, 1.0D);
                    Args.Layers[A].Terrainmap.Convert_Heightmap(hmC, (int)((1.0F - Args.Layers[A].TerrainmapDensity) / hmC.HeightScale));
                }
            }

            sXY_int Pos = new sXY_int();

            TerrainType = new int[Terrain.TileSize.X, Terrain.TileSize.Y];
            Slope = new float[Terrain.TileSize.X, Terrain.TileSize.Y];
            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    //get slope
                    BestSlope = 0.0D;

                    Pos.X = (int)((X + 0.25D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((Y + 0.25D) * App.TerrainGridSpacing);
                    CurrentSlope = GetTerrainSlopeAngle(Pos);
                    if ( CurrentSlope > BestSlope )
                    {
                        BestSlope = CurrentSlope;
                    }

                    Pos.X = (int)((X + 0.75D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((Y + 0.25D) * App.TerrainGridSpacing);
                    CurrentSlope = GetTerrainSlopeAngle(Pos);
                    if ( CurrentSlope > BestSlope )
                    {
                        BestSlope = CurrentSlope;
                    }

                    Pos.X = (int)((X + 0.25D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((Y + 0.75D) * App.TerrainGridSpacing);
                    CurrentSlope = GetTerrainSlopeAngle(Pos);
                    if ( CurrentSlope > BestSlope )
                    {
                        BestSlope = CurrentSlope;
                    }

                    Pos.X = (int)((X + 0.75D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((Y + 0.75D) * App.TerrainGridSpacing);
                    CurrentSlope = GetTerrainSlopeAngle(Pos);
                    if ( CurrentSlope > BestSlope )
                    {
                        BestSlope = CurrentSlope;
                    }

                    Slope[X, Y] = (float)BestSlope;
                }
            }
            for ( Layer_Num = 0; Layer_Num <= Args.LayerCount - 1; Layer_Num++ )
            {
                TerrainNum = Args.Layers[Layer_Num].TileNum;
                if ( TerrainNum >= 0 )
                {
                    //do other layer constraints
                    LayerResult[Layer_Num] = new BooleanMap();
                    LayerResult[Layer_Num].Copy(Args.Layers[Layer_Num].Terrainmap);
                    if ( Args.Layers[Layer_Num].WithinLayer >= 0 )
                    {
                        if ( Args.Layers[Layer_Num].WithinLayer < Layer_Num )
                        {
                            bmA.Within(LayerResult[Layer_Num], LayerResult[Args.Layers[Layer_Num].WithinLayer]);
                            LayerResult[Layer_Num].ValueData = bmA.ValueData;
                            bmA.ValueData = new BooleanMapDataValue();
                        }
                    }
                    for ( A = 0; A <= Layer_Num - 1; A++ )
                    {
                        if ( Args.Layers[Layer_Num].AvoidLayers[A] )
                        {
                            bmA.Expand_One_Tile(LayerResult[A]);
                            bmB.Remove(LayerResult[Layer_Num], bmA);
                            LayerResult[Layer_Num].ValueData = bmB.ValueData;
                            bmB.ValueData = new BooleanMapDataValue();
                        }
                    }
                    //do height and slope constraints
                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            if ( LayerResult[Layer_Num].ValueData.Value[Y, X] )
                            {
                                if ( Terrain.Vertices[X, Y].Height < Args.Layers[Layer_Num].HeightMin
                                     || Terrain.Vertices[X, Y].Height > Args.Layers[Layer_Num].HeightMax )
                                {
                                    LayerResult[Layer_Num].ValueData.Value[Y, X] = false;
                                }
                                if ( Args.Layers[Layer_Num].IsCliff )
                                {
                                    if ( LayerResult[Layer_Num].ValueData.Value[Y, X] )
                                    {
                                        if ( Slope[X, Y] < CliffSlope )
                                        {
                                            LayerResult[Layer_Num].ValueData.Value[Y, X] = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                        {
                            if ( LayerResult[Layer_Num].ValueData.Value[Y, X] )
                            {
                                TerrainType[X, Y] = TerrainNum;
                            }
                        }
                    }
                }
            }

            //set water tiles

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    if ( Args.Watermap.ValueData.Value[Y, X] )
                    {
                        if ( Slope[X, Y] < CliffSlope )
                        {
                            TerrainType[X, Y] = 17;
                        }
                    }
                }
            }

            //set border tiles to cliffs
            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= 2; X++ )
                {
                    TerrainType[X, Y] = Args.Tileset.BorderTextureNum;
                }
                for ( X = Terrain.TileSize.X - 4; X <= Terrain.TileSize.X - 1; X++ )
                {
                    TerrainType[X, Y] = Args.Tileset.BorderTextureNum;
                }
            }
            for ( X = 3; X <= Terrain.TileSize.X - 5; X++ )
            {
                for ( Y = 0; Y <= 2; Y++ )
                {
                    TerrainType[X, Y] = Args.Tileset.BorderTextureNum;
                }
                for ( Y = Terrain.TileSize.Y - 4; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    TerrainType[X, Y] = Args.Tileset.BorderTextureNum;
                }
            }

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    Terrain.Tiles[X, Y].Texture.TextureNum = TerrainType[X, Y];
                }
            }
        }

        public void RandomizeTileOrientations()
        {
            int X = 0;
            int Y = 0;

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    Terrain.Tiles[X, Y].Texture.Orientation = new TileOrientation(VBMath.Rnd() >= 0.5F, VBMath.Rnd() >= 0.5F, VBMath.Rnd() >= 0.5F);
                }
            }
            SectorTerrainUndoChanges.SetAllChanged();
            SectorGraphicsChanges.SetAllChanged();
        }

        public void MapTexturer(App.sLayerList LayerList)
        {
            int X = 0;
            int Y = 0;
            int A = 0;
            Painters.Terrain[,] TerrainType = null;
            float[,] Slope = null;
            Painters.Terrain tmpTerrain = default(Painters.Terrain);
            BooleanMap bmA = new BooleanMap();
            BooleanMap bmB = new BooleanMap();
            int LayerNum = 0;
            BooleanMap[] LayerResult = new BooleanMap[LayerList.LayerCount];
            double BestSlope = 0;
            double CurrentSlope = 0;
            bool AllowSlope = default(bool);
            sXY_int Pos = new sXY_int();

            TerrainType = new Painters.Terrain[Terrain.TileSize.X + 1, Terrain.TileSize.Y + 1];
            Slope = new float[Terrain.TileSize.X, Terrain.TileSize.Y];
            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    //get slope
                    BestSlope = 0.0D;

                    Pos.X = (int)((X + 0.25D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((Y + 0.25D) * App.TerrainGridSpacing);
                    CurrentSlope = GetTerrainSlopeAngle(Pos);
                    if ( CurrentSlope > BestSlope )
                    {
                        BestSlope = CurrentSlope;
                    }

                    Pos.X = (int)((X + 0.75D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((Y + 0.25D) * App.TerrainGridSpacing);
                    CurrentSlope = GetTerrainSlopeAngle(Pos);
                    if ( CurrentSlope > BestSlope )
                    {
                        BestSlope = CurrentSlope;
                    }

                    Pos.X = (int)((X + 0.25D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((Y + 0.75D) * App.TerrainGridSpacing);
                    CurrentSlope = GetTerrainSlopeAngle(Pos);
                    if ( CurrentSlope > BestSlope )
                    {
                        BestSlope = CurrentSlope;
                    }

                    Pos.X = (int)((X + 0.75D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((Y + 0.75D) * App.TerrainGridSpacing);
                    CurrentSlope = GetTerrainSlopeAngle(Pos);
                    if ( CurrentSlope > BestSlope )
                    {
                        BestSlope = CurrentSlope;
                    }

                    Slope[X, Y] = (float)BestSlope;
                }
            }
            for ( LayerNum = 0; LayerNum <= LayerList.LayerCount - 1; LayerNum++ )
            {
                tmpTerrain = LayerList.Layers[LayerNum].Terrain;
                if ( tmpTerrain != null )
                {
                    //do other layer constraints
                    LayerResult[LayerNum] = new BooleanMap();
                    LayerResult[LayerNum].Copy(LayerList.Layers[LayerNum].Terrainmap);
                    if ( LayerList.Layers[LayerNum].WithinLayer >= 0 )
                    {
                        if ( LayerList.Layers[LayerNum].WithinLayer < LayerNum )
                        {
                            bmA.Within(LayerResult[LayerNum], LayerResult[LayerList.Layers[LayerNum].WithinLayer]);
                            LayerResult[LayerNum].ValueData = bmA.ValueData;
                            bmA.ValueData = new BooleanMapDataValue();
                        }
                    }
                    for ( A = 0; A <= LayerNum - 1; A++ )
                    {
                        if ( LayerList.Layers[LayerNum].AvoidLayers[A] )
                        {
                            bmA.Expand_One_Tile(LayerResult[A]);
                            bmB.Remove(LayerResult[LayerNum], bmA);
                            LayerResult[LayerNum].ValueData = bmB.ValueData;
                            bmB.ValueData = new BooleanMapDataValue();
                        }
                    }
                    //do height and slope constraints
                    for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X; X++ )
                        {
                            if ( LayerResult[LayerNum].ValueData.Value[Y, X] )
                            {
                                if ( Terrain.Vertices[X, Y].Height < LayerList.Layers[LayerNum].HeightMin
                                     || Terrain.Vertices[X, Y].Height > LayerList.Layers[LayerNum].HeightMax )
                                {
                                    LayerResult[LayerNum].ValueData.Value[Y, X] = false;
                                }
                                if ( LayerResult[LayerNum].ValueData.Value[Y, X] )
                                {
                                    AllowSlope = true;
                                    if ( X > 0 )
                                    {
                                        if ( Y > 0 )
                                        {
                                            if ( Slope[X - 1, Y - 1] < LayerList.Layers[LayerNum].SlopeMin
                                                 || Slope[X - 1, Y - 1] > LayerList.Layers[LayerNum].SlopeMax )
                                            {
                                                AllowSlope = false;
                                            }
                                        }
                                        if ( Y < Terrain.TileSize.Y )
                                        {
                                            if ( Slope[X - 1, Y] < LayerList.Layers[LayerNum].SlopeMin
                                                 || Slope[X - 1, Y] > LayerList.Layers[LayerNum].SlopeMax )
                                            {
                                                AllowSlope = false;
                                            }
                                        }
                                    }
                                    if ( X < Terrain.TileSize.X )
                                    {
                                        if ( Y > 0 )
                                        {
                                            if ( Slope[X, Y - 1] < LayerList.Layers[LayerNum].SlopeMin
                                                 || Slope[X, Y - 1] > LayerList.Layers[LayerNum].SlopeMax )
                                            {
                                                AllowSlope = false;
                                            }
                                        }
                                        if ( Y < Terrain.TileSize.Y )
                                        {
                                            if ( Slope[X, Y] < LayerList.Layers[LayerNum].SlopeMin
                                                 || Slope[X, Y] > LayerList.Layers[LayerNum].SlopeMax )
                                            {
                                                AllowSlope = false;
                                            }
                                        }
                                    }
                                    if ( !AllowSlope )
                                    {
                                        LayerResult[LayerNum].ValueData.Value[Y, X] = false;
                                    }
                                }
                            }
                        }
                    }

                    LayerResult[LayerNum].Remove_Diagonals();

                    for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
                    {
                        for ( X = 0; X <= Terrain.TileSize.X; X++ )
                        {
                            if ( LayerResult[LayerNum].ValueData.Value[Y, X] )
                            {
                                TerrainType[X, Y] = tmpTerrain;
                            }
                        }
                    }
                }
            }

            //set vertex terrain by terrain map
            for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X; X++ )
                {
                    if ( TerrainType[X, Y] != null )
                    {
                        Terrain.Vertices[X, Y].Terrain = TerrainType[X, Y];
                    }
                }
            }
            AutoTextureChanges.SetAllChanged();
            UpdateAutoTextures();
        }

        public BooleanMap GenerateTerrainMap(float Scale, float Density)
        {
            BooleanMap ReturnResult = default(BooleanMap);
            clsHeightmap hmB = new clsHeightmap();
            clsHeightmap hmC = new clsHeightmap();

            hmB.GenerateNewOfSize(Terrain.TileSize.Y + 1, Terrain.TileSize.X + 1, Scale, 1.0D);
            hmC.Rescale(hmB, 0.0D, 1.0D);
            ReturnResult = new BooleanMap();
            ReturnResult.Convert_Heightmap(hmC, (int)((1.0D - Density) / hmC.HeightScale));
            return ReturnResult;
        }

        public void WaterTriCorrection()
        {
            if ( Tileset == null )
            {
                return;
            }

            int X = 0;
            int Y = 0;
            sXY_int TileNum = new sXY_int();

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                TileNum.Y = Y;
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    TileNum.X = X;
                    if ( Terrain.Tiles[X, Y].Tri )
                    {
                        if ( Terrain.Tiles[X, Y].Texture.TextureNum >= 0 )
                        {
                            if ( Tileset.Tiles[Terrain.Tiles[X, Y].Texture.TextureNum].Default_Type == App.TileTypeNum_Water )
                            {
                                Terrain.Tiles[X, Y].Tri = false;
                                SectorGraphicsChanges.TileChanged(TileNum);
                                SectorTerrainUndoChanges.TileChanged(TileNum);
                            }
                        }
                    }
                }
            }
        }

        public abstract class clsAction
        {
            public clsMap Map;
            public sXY_int PosNum;
            public bool UseEffect;
            public double Effect;

            public abstract void ActionPerform();
        }

        public abstract class clsObjectAction : SimpleListTool<clsUnit>
        {
            public clsMap Map;
            public clsUnit Unit;
            private SimpleClassList<clsUnit> _ResultUnits = new SimpleClassList<clsUnit>();
            public bool ActionPerformed;

            protected clsUnit ResultUnit;

            public SimpleClassList<clsUnit> ResultUnits
            {
                get { return _ResultUnits; }
            }

            protected virtual void ActionCondition()
            {
            }

            public void ActionPerform()
            {
                ResultUnit = null;
                ActionPerformed = false;
                if ( Unit == null )
                {
                    Debugger.Break();
                    return;
                }
                ActionPerformed = true;
                ActionCondition();
                if ( !ActionPerformed )
                {
                    return;
                }
                ResultUnit = new clsUnit(Unit, Map);
                _ActionPerform();
                if ( ResultUnit == null )
                {
                    ResultUnit = Unit;
                }
                else
                {
                    _ResultUnits.Add(ResultUnit);
                    Map.UnitSwap(Unit, ResultUnit);
                }
            }

            protected abstract void _ActionPerform();

            public void SetItem(clsUnit Item)
            {
                Unit = Item;
            }
        }

        public class clsApplyCliff : clsAction
        {
            public double Angle;
            public bool SetTris;

            private int RandomNum;
            private double DifA;
            private double DifB;
            private double HeightA;
            private double HeightB;
            private double TriTopLeftMaxSlope;
            private double TriTopRightMaxSlope;
            private double TriBottomLeftMaxSlope;
            private double TriBottomRightMaxSlope;
            private bool CliffChanged;
            private bool TriChanged;
            private bool NewVal;
            private clsTerrain Terrain;
            private sXY_int Pos;

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                HeightA = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height) / 2.0D);
                HeightB = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) / 2.0D);
                DifA = HeightB - HeightA;
                HeightA = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) / 2.0D);
                HeightB = Convert.ToDouble(((Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) / 2.0D);
                DifB = HeightB - HeightA;
                if ( Math.Abs(DifA) == Math.Abs(DifB) )
                {
                    RandomNum = (int)(Conversion.Int(VBMath.Rnd() * 4.0F));
                    if ( RandomNum == 0 )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Top;
                    }
                    else if ( RandomNum == 1 )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Right;
                    }
                    else if ( RandomNum == 2 )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Bottom;
                    }
                    else
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Left;
                    }
                }
                else if ( Math.Abs(DifA) > Math.Abs(DifB) )
                {
                    if ( DifA < 0.0D )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Bottom;
                    }
                    else
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Top;
                    }
                }
                else
                {
                    if ( DifB < 0.0D )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Right;
                    }
                    else
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Left;
                    }
                }

                CliffChanged = false;
                TriChanged = false;

                if ( SetTris )
                {
                    DifA = Math.Abs((Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) - Terrain.Vertices[PosNum.X, PosNum.Y].Height);
                    DifB = Math.Abs((Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) - Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height);
                    if ( DifA == DifB )
                    {
                        if ( VBMath.Rnd() >= 0.5F )
                        {
                            NewVal = false;
                        }
                        else
                        {
                            NewVal = true;
                        }
                    }
                    else if ( DifA < DifB )
                    {
                        NewVal = false;
                    }
                    else
                    {
                        NewVal = true;
                    }
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri != NewVal )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].Tri = NewVal;
                        TriChanged = true;
                    }
                }

                if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri )
                {
                    Pos.X = (int)((PosNum.X + 0.25D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((PosNum.Y + 0.25D) * App.TerrainGridSpacing);
                    TriTopLeftMaxSlope = Map.GetTerrainSlopeAngle(Pos);
                    Pos.X = (int)((PosNum.X + 0.75D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((PosNum.Y + 0.75D) * App.TerrainGridSpacing);
                    TriBottomRightMaxSlope = Map.GetTerrainSlopeAngle(Pos);

                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;
                        CliffChanged = true;
                    }
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
                        CliffChanged = true;
                    }

                    NewVal = TriTopLeftMaxSlope >= Angle;
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff != NewVal )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = NewVal;
                        CliffChanged = true;
                    }

                    NewVal = TriBottomRightMaxSlope >= Angle;
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff != NewVal )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = NewVal;
                        CliffChanged = true;
                    }

                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff || Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                    }
                    else
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = false;
                    }
                }
                else
                {
                    Pos.X = (int)((PosNum.X + 0.75D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((PosNum.Y + 0.25D) * App.TerrainGridSpacing);
                    TriTopRightMaxSlope = Map.GetTerrainSlopeAngle(Pos);
                    Pos.X = (int)((PosNum.X + 0.25D) * App.TerrainGridSpacing);
                    Pos.Y = (int)((PosNum.Y + 0.75D) * App.TerrainGridSpacing);
                    TriBottomLeftMaxSlope = Map.GetTerrainSlopeAngle(Pos);

                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;
                        CliffChanged = true;
                    }
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
                        CliffChanged = true;
                    }

                    NewVal = TriTopRightMaxSlope >= Angle;
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff != NewVal )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = NewVal;
                        CliffChanged = true;
                    }

                    NewVal = TriBottomLeftMaxSlope >= Angle;
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff != NewVal )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = NewVal;
                        CliffChanged = true;
                    }

                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff || Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                    }
                    else
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = false;
                    }
                }

                if ( CliffChanged )
                {
                    Map.AutoTextureChanges.TileChanged(PosNum);
                }
                if ( TriChanged || CliffChanged )
                {
                    Map.SectorGraphicsChanges.TileChanged(PosNum);
                    Map.SectorTerrainUndoChanges.TileChanged(PosNum);
                }
            }
        }

        public class clsApplyCliffRemove : clsAction
        {
            private clsTerrain Terrain;

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                if ( Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff || Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff ||
                     Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff || Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff ||
                     Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = false;
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;

                    Map.AutoTextureChanges.TileChanged(PosNum);
                    Map.SectorGraphicsChanges.TileChanged(PosNum);
                    Map.SectorTerrainUndoChanges.TileChanged(PosNum);
                }
            }
        }

        public class clsApplyCliffTriangle : clsAction
        {
            public bool Triangle;

            private clsTerrain Terrain;
            private bool CliffChanged;

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;

                CliffChanged = false;
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri )
                {
                    if ( Triangle )
                    {
                        if ( !Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                            CliffChanged = true;
                        }
                    }
                    else
                    {
                        if ( !Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff )
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                            CliffChanged = true;
                        }
                    }
                }
                else
                {
                    if ( Triangle )
                    {
                        if ( !Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                            CliffChanged = true;
                        }
                    }
                    else
                    {
                        if ( !Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                            CliffChanged = true;
                        }
                    }
                }

                if ( !CliffChanged )
                {
                    return;
                }

                double HeightA = 0;
                double HeightB = 0;
                double difA = 0;
                double difB = 0;
                int A;

                Map.AutoTextureChanges.TileChanged(PosNum);
                Map.SectorGraphicsChanges.TileChanged(PosNum);
                Map.SectorTerrainUndoChanges.TileChanged(PosNum);

                HeightA = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height) / 2.0D);
                HeightB = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) / 2.0D);
                difA = HeightB - HeightA;
                HeightA = Convert.ToDouble(((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) / 2.0D);
                HeightB = Convert.ToDouble(((Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height) + Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) / 2.0D);
                difB = HeightB - HeightA;
                if ( Math.Abs(difA) == Math.Abs(difB) )
                {
                    A = (int)(Conversion.Int(VBMath.Rnd() * 4.0F));
                    if ( A == 0 )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Top;
                    }
                    else if ( A == 1 )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Right;
                    }
                    else if ( A == 2 )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Bottom;
                    }
                    else
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Left;
                    }
                }
                else if ( Math.Abs(difA) > Math.Abs(difB) )
                {
                    if ( difA < 0.0D )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Bottom;
                    }
                    else
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Top;
                    }
                }
                else
                {
                    if ( difB < 0.0D )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Right;
                    }
                    else
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.Left;
                    }
                }
            }
        }

        public class clsApplyCliffTriangleRemove : clsAction
        {
            public bool Triangle;

            private clsTerrain Terrain;
            private bool CliffChanged;

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                CliffChanged = false;
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri )
                {
                    if ( Triangle )
                    {
                        if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;
                            CliffChanged = true;
                        }
                    }
                    else
                    {
                        if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff )
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
                            CliffChanged = true;
                        }
                    }
                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff ||
                                                                        Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff;
                }
                else
                {
                    if ( Triangle )
                    {
                        if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;
                            CliffChanged = true;
                        }
                    }
                    else
                    {
                        if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                        {
                            Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
                            CliffChanged = true;
                        }
                    }
                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff ||
                                                                        Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff;
                }

                if ( !CliffChanged )
                {
                    return;
                }

                Map.AutoTextureChanges.TileChanged(PosNum);
                Map.SectorGraphicsChanges.TileChanged(PosNum);
                Map.SectorTerrainUndoChanges.TileChanged(PosNum);
            }
        }

        public class clsApplyHeightChange : clsAction
        {
            public double Rate;

            private clsTerrain Terrain;

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                Terrain.Vertices[PosNum.X, PosNum.Y].Height =
                    (byte)(MathUtil.Clamp_int((Terrain.Vertices[PosNum.X, PosNum.Y].Height) + (int)(Rate * Effect), byte.MinValue, byte.MaxValue));

                Map.SectorGraphicsChanges.VertexAndNormalsChanged(PosNum);
                Map.SectorUnitHeightsChanges.VertexChanged(PosNum);
                Map.SectorTerrainUndoChanges.VertexChanged(PosNum);
            }
        }

        public class clsApplyHeightSet : clsAction
        {
            public byte Height;

            private clsTerrain Terrain;

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Height != Height )
                {
                    Terrain.Vertices[PosNum.X, PosNum.Y].Height = Height;
                    Map.SectorGraphicsChanges.VertexAndNormalsChanged(PosNum);
                    Map.SectorUnitHeightsChanges.VertexChanged(PosNum);
                    Map.SectorTerrainUndoChanges.VertexChanged(PosNum);
                }
            }
        }

        public class clsApplyHeightSmoothing : clsAction
        {
            public double Ratio;
            public sXY_int Offset;
            public sXY_int AreaTileSize;

            private byte[,] NewHeight;
            private bool Started;
            private int TempHeight;
            private int Samples;
            private int LimitX;
            private int LimitY;
            private int XNum;
            private sXY_int VertexNum;
            private clsTerrain Terrain;

            public void Start()
            {
                int X = 0;
                int Y = 0;

                Terrain = Map.Terrain;

                NewHeight = new byte[AreaTileSize.X + 1, AreaTileSize.Y + 1];
                for ( Y = 0; Y <= AreaTileSize.Y; Y++ )
                {
                    for ( X = 0; X <= AreaTileSize.X; X++ )
                    {
                        NewHeight[X, Y] = Terrain.Vertices[Offset.X + X, Offset.Y + Y].Height;
                    }
                }

                Started = true;
            }

            public void Finish()
            {
                if ( !Started )
                {
                    Debugger.Break();
                    return;
                }

                int X = 0;
                int Y = 0;

                Terrain = Map.Terrain;

                for ( Y = 0; Y <= AreaTileSize.Y; Y++ )
                {
                    VertexNum.Y = Offset.Y + Y;
                    for ( X = 0; X <= AreaTileSize.X; X++ )
                    {
                        VertexNum.X = Offset.X + X;
                        Terrain.Vertices[VertexNum.X, VertexNum.Y].Height = NewHeight[X, Y];

                        Map.SectorGraphicsChanges.VertexAndNormalsChanged(VertexNum);
                        Map.SectorUnitHeightsChanges.VertexChanged(VertexNum);
                        Map.SectorTerrainUndoChanges.VertexChanged(VertexNum);
                    }
                }

                Started = false;
            }

            public override void ActionPerform()
            {
                if ( !Started )
                {
                    Debugger.Break();
                    return;
                }

                int X = 0;
                int Y = 0;
                int X2 = 0;
                int Y2 = 0;

                Terrain = Map.Terrain;

                LimitX = Terrain.TileSize.X;
                LimitY = Terrain.TileSize.Y;
                TempHeight = 0;
                Samples = 0;
                for ( Y = MathUtil.Clamp_int(App.SmoothRadius.Tiles.YMin + PosNum.Y, 0, LimitY) - PosNum.Y;
                    Y <= MathUtil.Clamp_int(App.SmoothRadius.Tiles.YMax + PosNum.Y, 0, LimitY) - PosNum.Y;
                    Y++ )
                {
                    Y2 = PosNum.Y + Y;
                    XNum = Y - App.SmoothRadius.Tiles.YMin;
                    for ( X = MathUtil.Clamp_int(Convert.ToInt32(App.SmoothRadius.Tiles.XMin[XNum] + PosNum.X), 0, LimitX) - PosNum.X;
                        X <= MathUtil.Clamp_int(Convert.ToInt32(App.SmoothRadius.Tiles.XMax[XNum] + PosNum.X), 0, LimitX) - PosNum.X;
                        X++ )
                    {
                        X2 = PosNum.X + X;
                        TempHeight += Terrain.Vertices[X2, Y2].Height;
                        Samples++;
                    }
                }
                NewHeight[PosNum.X - Offset.X, PosNum.Y - Offset.Y] =
                    Math.Min((byte)(Convert.ToInt32(Terrain.Vertices[PosNum.X, PosNum.Y].Height * (1.0D - Ratio) + TempHeight / Samples * Ratio)), byte.MaxValue);
            }
        }

        public class clsApplyRoadRemove : clsAction
        {
            private clsTerrain Terrain;

            private void ToolPerformSideH(sXY_int SideNum)
            {
                Terrain = Map.Terrain;

                if ( Terrain.SideH[SideNum.X, SideNum.Y].Road != null )
                {
                    Terrain.SideH[SideNum.X, SideNum.Y].Road = null;
                    Map.AutoTextureChanges.SideHChanged(SideNum);
                    Map.SectorGraphicsChanges.SideHChanged(SideNum);
                    Map.SectorTerrainUndoChanges.SideHChanged(SideNum);
                }
            }

            private void ToolPerformSideV(sXY_int SideNum)
            {
                Terrain = Map.Terrain;

                if ( Terrain.SideV[SideNum.X, SideNum.Y].Road != null )
                {
                    Terrain.SideV[SideNum.X, SideNum.Y].Road = null;
                    Map.AutoTextureChanges.SideVChanged(SideNum);
                    Map.SectorGraphicsChanges.SideVChanged(SideNum);
                    Map.SectorTerrainUndoChanges.SideVChanged(SideNum);
                }
            }

            public override void ActionPerform()
            {
                ToolPerformSideH(PosNum);
                ToolPerformSideH(new sXY_int(PosNum.X, PosNum.Y + 1));
                ToolPerformSideV(PosNum);
                ToolPerformSideV(new sXY_int(PosNum.X + 1, PosNum.Y));
            }
        }

        public class clsApplyVertexTerrain : clsAction
        {
            public Painters.Terrain VertexTerrain;

            private clsTerrain Terrain;

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain != VertexTerrain )
                {
                    Terrain.Vertices[PosNum.X, PosNum.Y].Terrain = VertexTerrain;
                    Map.SectorGraphicsChanges.VertexChanged(PosNum);
                    Map.SectorTerrainUndoChanges.VertexChanged(PosNum);
                    Map.AutoTextureChanges.VertexChanged(PosNum);
                }
            }
        }

        public class clsApplyTexture : clsAction
        {
            public int TextureNum;
            public bool SetTexture;
            public TileOrientation Orientation;
            public bool SetOrientation;
            public bool RandomOrientation;
            public App.enumTextureTerrainAction TerrainAction;

            private clsTerrain Terrain;

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;

                if ( SetTexture )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].Texture.TextureNum = TextureNum;
                }
                if ( SetOrientation )
                {
                    if ( RandomOrientation )
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].Texture.Orientation = new TileOrientation(VBMath.Rnd() < 0.5F, VBMath.Rnd() < 0.5F,
                            VBMath.Rnd() < 0.5F);
                    }
                    else
                    {
                        Terrain.Tiles[PosNum.X, PosNum.Y].Texture.Orientation = Orientation;
                    }
                }

                Map.TileTextureChangeTerrainAction(PosNum, TerrainAction);

                Map.SectorGraphicsChanges.TileChanged(PosNum);
                Map.SectorTerrainUndoChanges.TileChanged(PosNum);
            }
        }

        public class clsApplyVertexTerrainInterpret : clsAction
        {
            private int[] TerrainCount;
            private TileDirection VertexDirection;
            private Painter Painter;
            private Painters.Terrain PainterTerrainA;
            private Painters.Terrain PainterTerrainB;
            private clsTerrain.Tile.sTexture Texture;
            private TileDirection ResultDirection;
            private TileOrientationChance PainterTexture;
            private TileDirection OppositeDirection;
            private int BestNum;
            private int BestCount;
            private clsTerrain.Tile Tile;
            private clsTerrain Terrain;

            private void ToolPerformTile()
            {
                int PainterBrushNum = 0;
                int A = 0;

                for ( PainterBrushNum = 0; PainterBrushNum <= Painter.TerrainCount - 1; PainterBrushNum++ )
                {
                    PainterTerrainA = Painter.Terrains[PainterBrushNum];
                    for ( A = 0; A <= PainterTerrainA.Tiles.TileCount - 1; A++ )
                    {
                        PainterTexture = PainterTerrainA.Tiles.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                    }
                }
                for ( PainterBrushNum = 0; PainterBrushNum <= Painter.TransitionBrushCount - 1; PainterBrushNum++ )
                {
                    PainterTerrainA = Painter.TransitionBrushes[PainterBrushNum].Terrain_Inner;
                    PainterTerrainB = Painter.TransitionBrushes[PainterBrushNum].Terrain_Outer;
                    for ( A = 0; A <= Painter.TransitionBrushes[PainterBrushNum].Tiles_Straight.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.TransitionBrushes[PainterBrushNum].Tiles_Straight.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            if ( TileUtil.DirectionsOnSameSide(VertexDirection, ResultDirection) )
                            {
                                TerrainCount[PainterTerrainB.Num]++;
                            }
                            else
                            {
                                TerrainCount[PainterTerrainA.Num]++;
                            }
                        }
                    }
                    for ( A = 0; A <= Painter.TransitionBrushes[PainterBrushNum].Tiles_Corner_In.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.TransitionBrushes[PainterBrushNum].Tiles_Corner_In.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            if ( TileUtil.IdenticalTileDirections(VertexDirection, ResultDirection) )
                            {
                                TerrainCount[PainterTerrainB.Num]++;
                            }
                            else
                            {
                                TerrainCount[PainterTerrainA.Num]++;
                            }
                        }
                    }
                    for ( A = 0; A <= Painter.TransitionBrushes[PainterBrushNum].Tiles_Corner_Out.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.TransitionBrushes[PainterBrushNum].Tiles_Corner_Out.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            OppositeDirection = PainterTexture.Direction;
                            OppositeDirection.FlipX();
                            OppositeDirection.FlipY();
                            TileUtil.RotateDirection(OppositeDirection, Texture.Orientation, ref ResultDirection);
                            if ( TileUtil.IdenticalTileDirections(VertexDirection, ResultDirection) )
                            {
                                TerrainCount[PainterTerrainA.Num]++;
                            }
                            else
                            {
                                TerrainCount[PainterTerrainB.Num]++;
                            }
                        }
                    }
                }

                for ( PainterBrushNum = 0; PainterBrushNum <= Painter.CliffBrushCount - 1; PainterBrushNum++ )
                {
                    PainterTerrainA = Painter.CliffBrushes[PainterBrushNum].Terrain_Inner;
                    PainterTerrainB = Painter.CliffBrushes[PainterBrushNum].Terrain_Outer;
                    for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Straight.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Straight.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            if ( TileUtil.DirectionsOnSameSide(VertexDirection, ResultDirection) )
                            {
                                TerrainCount[PainterTerrainB.Num]++;
                            }
                            else
                            {
                                TerrainCount[PainterTerrainA.Num]++;
                            }
                        }
                    }
                    for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_In.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_In.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            if ( TileUtil.IdenticalTileDirections(VertexDirection, ResultDirection) )
                            {
                                TerrainCount[PainterTerrainA.Num]++;
                            }
                            else
                            {
                                TerrainCount[PainterTerrainB.Num]++;
                            }
                        }
                    }
                    for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_Out.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_Out.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            OppositeDirection = PainterTexture.Direction;
                            OppositeDirection.FlipX();
                            OppositeDirection.FlipY();
                            TileUtil.RotateDirection(OppositeDirection, Texture.Orientation, ref ResultDirection);
                            if ( TileUtil.IdenticalTileDirections(VertexDirection, ResultDirection) )
                            {
                                TerrainCount[PainterTerrainA.Num]++;
                            }
                            else
                            {
                                TerrainCount[PainterTerrainB.Num]++;
                            }
                        }
                    }
                }

                for ( PainterBrushNum = 0; PainterBrushNum <= Painter.RoadBrushCount - 1; PainterBrushNum++ )
                {
                    PainterTerrainA = Painter.RoadBrushes[PainterBrushNum].Terrain;
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_Corner_In.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_Corner_In.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                    }
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_CrossIntersection.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_CrossIntersection.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                    }
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_End.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_End.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                    }
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_Straight.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_Straight.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                    }
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_TIntersection.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_TIntersection.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TerrainCount[PainterTerrainA.Num]++;
                        }
                    }
                }
            }

            public override void ActionPerform()
            {
                int A = 0;

                Terrain = Map.Terrain;

                Painter = Map.Painter;
                TerrainCount = new int[Painter.TerrainCount];

                if ( PosNum.Y > 0 )
                {
                    if ( PosNum.X > 0 )
                    {
                        VertexDirection = TileUtil.BottomRight;
                        Tile = Terrain.Tiles[PosNum.X - 1, PosNum.Y - 1];
                        Texture = Tile.Texture;
                        ToolPerformTile();
                    }
                    if ( PosNum.X < Terrain.TileSize.X )
                    {
                        VertexDirection = TileUtil.BottomLeft;
                        Tile = Terrain.Tiles[PosNum.X, PosNum.Y - 1];
                        Texture = Tile.Texture;
                        ToolPerformTile();
                    }
                }
                if ( PosNum.Y < Terrain.TileSize.Y )
                {
                    if ( PosNum.X > 0 )
                    {
                        VertexDirection = TileUtil.TopRight;
                        Tile = Terrain.Tiles[PosNum.X - 1, PosNum.Y];
                        Texture = Tile.Texture;
                        ToolPerformTile();
                    }
                    if ( PosNum.X < Terrain.TileSize.X )
                    {
                        VertexDirection = TileUtil.TopLeft;
                        Tile = Terrain.Tiles[PosNum.X, PosNum.Y];
                        Texture = Tile.Texture;
                        ToolPerformTile();
                    }
                }

                BestNum = -1;
                BestCount = 0;
                for ( A = 0; A <= Painter.TerrainCount - 1; A++ )
                {
                    if ( TerrainCount[A] > BestCount )
                    {
                        BestNum = A;
                        BestCount = TerrainCount[A];
                    }
                }
                if ( BestCount > 0 )
                {
                    Terrain.Vertices[PosNum.X, PosNum.Y].Terrain = Painter.Terrains[BestNum];
                }
                else
                {
                    Terrain.Vertices[PosNum.X, PosNum.Y].Terrain = null;
                }

                Map.SectorTerrainUndoChanges.VertexChanged(PosNum);
            }
        }

        public class clsApplyTileTerrainInterpret : clsAction
        {
            private Painter Painter;
            private Painters.Terrain PainterTerrainA;
            private Painters.Terrain PainterTerrainB;
            private clsTerrain.Tile.sTexture Texture;
            private TileDirection ResultDirection;
            private TileOrientationChance PainterTexture;
            private TileDirection OppositeDirection;
            private clsTerrain.Tile Tile;
            private clsTerrain Terrain;

            public override void ActionPerform()
            {
                int PainterBrushNum = 0;
                int A = 0;

                Terrain = Map.Terrain;

                Painter = Map.Painter;

                Tile = Terrain.Tiles[PosNum.X, PosNum.Y];
                Texture = Tile.Texture;

                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = false;
                Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = TileUtil.None;

                for ( PainterBrushNum = 0; PainterBrushNum <= Painter.CliffBrushCount - 1; PainterBrushNum++ )
                {
                    PainterTerrainA = Painter.CliffBrushes[PainterBrushNum].Terrain_Inner;
                    PainterTerrainB = Painter.CliffBrushes[PainterBrushNum].Terrain_Outer;
                    for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Straight.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Straight.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            if ( Tile.Tri )
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                            }
                            else
                            {
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                                Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                            }
                            Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            Terrain.Tiles[PosNum.X, PosNum.Y].DownSide = ResultDirection;
                        }
                    }
                    for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_In.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_In.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            if ( Tile.Tri )
                            {
                                if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.TopLeft) )
                                {
                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                                }
                                else if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.BottomRight) )
                                {
                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                                }
                            }
                            else
                            {
                                if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.TopRight) )
                                {
                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                                }
                                else if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.BottomLeft) )
                                {
                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                                }
                            }
                        }
                    }
                    for ( A = 0; A <= Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_Out.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.CliffBrushes[PainterBrushNum].Tiles_Corner_Out.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            OppositeDirection = PainterTexture.Direction;
                            OppositeDirection.FlipX();
                            OppositeDirection.FlipY();
                            TileUtil.RotateDirection(OppositeDirection, Texture.Orientation, ref ResultDirection);
                            if ( Tile.Tri )
                            {
                                if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.TopLeft) )
                                {
                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff = true;
                                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                                }
                                else if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.BottomRight) )
                                {
                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff = true;
                                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                                }
                            }
                            else
                            {
                                if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.TopRight) )
                                {
                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff = true;
                                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                                }
                                else if ( TileUtil.IdenticalTileDirections(ResultDirection, TileUtil.BottomLeft) )
                                {
                                    Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff = true;
                                    Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff = true;
                                }
                            }
                        }
                    }
                }

                Map.SectorTerrainUndoChanges.TileChanged(PosNum);
            }
        }

        public abstract class clsApplySideTerrainInterpret : clsAction
        {
            protected Painter Painter;
            protected Painters.Terrain PainterTerrain;
            protected Road PainterRoad;
            protected clsTerrain.Tile.sTexture Texture;
            protected TileDirection ResultDirection;
            protected TileOrientationChance PainterTexture;
            protected TileDirection OppositeDirection;
            protected clsTerrain.Tile Tile;
            protected int[] RoadCount;
            protected TileDirection SideDirection;
            protected int BestNum;
            protected int BestCount;
            protected clsTerrain Terrain;

            protected void ToolPerformTile()
            {
                int PainterBrushNum = 0;
                int A = 0;

                for ( PainterBrushNum = 0; PainterBrushNum <= Painter.RoadBrushCount - 1; PainterBrushNum++ )
                {
                    PainterRoad = Painter.RoadBrushes[PainterBrushNum].Road;
                    PainterTerrain = Painter.RoadBrushes[PainterBrushNum].Terrain;
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_Corner_In.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_Corner_In.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            if ( TileUtil.DirectionsOnSameSide(SideDirection, ResultDirection) )
                            {
                                RoadCount[PainterRoad.Num]++;
                            }
                        }
                    }
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_CrossIntersection.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_CrossIntersection.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            RoadCount[PainterRoad.Num]++;
                        }
                    }
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_End.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_End.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            if ( TileUtil.IdenticalTileDirections(SideDirection, ResultDirection) )
                            {
                                RoadCount[PainterRoad.Num]++;
                            }
                        }
                    }
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_Straight.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_Straight.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            if ( TileUtil.DirectionsAreInLine(SideDirection, ResultDirection) )
                            {
                                RoadCount[PainterRoad.Num]++;
                            }
                        }
                    }
                    for ( A = 0; A <= Painter.RoadBrushes[PainterBrushNum].Tile_TIntersection.TileCount - 1; A++ )
                    {
                        PainterTexture = Painter.RoadBrushes[PainterBrushNum].Tile_TIntersection.Tiles[A];
                        if ( PainterTexture.TextureNum == Texture.TextureNum )
                        {
                            TileUtil.RotateDirection(PainterTexture.Direction, Texture.Orientation, ref ResultDirection);
                            if ( !TileUtil.DirectionsOnSameSide(SideDirection, ResultDirection) )
                            {
                                RoadCount[PainterRoad.Num]++;
                            }
                        }
                    }
                }
            }

            public override void ActionPerform()
            {
                Terrain = Map.Terrain;

                Painter = Map.Painter;
                RoadCount = new int[Painter.RoadCount];
            }
        }

        public class clsApplySideHTerrainInterpret : clsApplySideTerrainInterpret
        {
            public override void ActionPerform()
            {
                base.ActionPerform();

                int A = 0;

                if ( PosNum.Y > 0 )
                {
                    SideDirection = TileUtil.Bottom;
                    Tile = Terrain.Tiles[PosNum.X, PosNum.Y - 1];
                    Texture = Tile.Texture;
                    ToolPerformTile();
                }
                if ( PosNum.Y < Terrain.TileSize.Y )
                {
                    SideDirection = TileUtil.Top;
                    Tile = Terrain.Tiles[PosNum.X, PosNum.Y];
                    Texture = Tile.Texture;
                    ToolPerformTile();
                }

                BestNum = -1;
                BestCount = 0;
                for ( A = 0; A <= Painter.RoadCount - 1; A++ )
                {
                    if ( RoadCount[A] > BestCount )
                    {
                        BestNum = A;
                        BestCount = RoadCount[A];
                    }
                }
                if ( BestCount > 0 )
                {
                    Terrain.SideH[PosNum.X, PosNum.Y].Road = Painter.Roads[BestNum];
                }
                else
                {
                    Terrain.SideH[PosNum.X, PosNum.Y].Road = null;
                }

                Map.SectorTerrainUndoChanges.SideHChanged(PosNum);
            }
        }

        public class clsApplySideVTerrainInterpret : clsApplySideTerrainInterpret
        {
            public override void ActionPerform()
            {
                base.ActionPerform();

                int A = 0;

                if ( PosNum.X > 0 )
                {
                    SideDirection = TileUtil.Right;
                    Tile = Terrain.Tiles[PosNum.X - 1, PosNum.Y];
                    Texture = Tile.Texture;
                    ToolPerformTile();
                }
                if ( PosNum.X < Terrain.TileSize.X )
                {
                    SideDirection = TileUtil.Left;
                    Tile = Terrain.Tiles[PosNum.X, PosNum.Y];
                    Texture = Tile.Texture;
                    ToolPerformTile();
                }

                BestNum = -1;
                BestCount = 0;
                for ( A = 0; A <= Painter.RoadCount - 1; A++ )
                {
                    if ( RoadCount[A] > BestCount )
                    {
                        BestNum = A;
                        BestCount = RoadCount[A];
                    }
                }
                if ( BestCount > 0 )
                {
                    Terrain.SideV[PosNum.X, PosNum.Y].Road = Painter.Roads[BestNum];
                }
                else
                {
                    Terrain.SideV[PosNum.X, PosNum.Y].Road = null;
                }

                Map.SectorTerrainUndoChanges.SideVChanged(PosNum);
            }
        }

        public class clsApplyAutoTri : clsAction
        {
            private double difA;
            private double difB;
            private bool NewTri;

            public override void ActionPerform()
            {
                //tri set to the direction where the diagonal edge will be the flattest, so that cliff edges are level
                difA = Math.Abs((Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height) - Map.Terrain.Vertices[PosNum.X, PosNum.Y].Height);
                difB = Math.Abs((Map.Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height) - Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height);
                if ( difA == difB )
                {
                    NewTri = VBMath.Rnd() < 0.5F;
                }
                else
                {
                    NewTri = difA > difB;
                }
                Map.Terrain.Tiles[PosNum.X, PosNum.Y].Tri = NewTri;

                Map.SectorGraphicsChanges.TileChanged(PosNum);
                Map.SectorUnitHeightsChanges.TileChanged(PosNum);
                Map.SectorTerrainUndoChanges.TileChanged(PosNum);
            }
        }

        public class clsObjectRotation : clsObjectAction
        {
            public int Angle;

            protected override void _ActionPerform()
            {
                ResultUnit.Rotation = Angle;
            }
        }

        public class clsObjectUnitGroup : clsObjectAction
        {
            public clsUnitGroup UnitGroup;

            protected override void _ActionPerform()
            {
                ResultUnit.UnitGroup = UnitGroup;
            }
        }

        public class clsObjectPriority : clsObjectAction
        {
            public int Priority;

            protected override void _ActionPerform()
            {
                ResultUnit.SavePriority = Priority;
            }
        }

        public class clsObjectHealth : clsObjectAction
        {
            public double Health;

            protected override void _ActionPerform()
            {
                ResultUnit.Health = Health;
            }
        }

        public class clsObjectPosOffset : clsObjectAction
        {
            public sXY_int Offset;

            private sXY_int NewPos;

            protected override void _ActionPerform()
            {
                NewPos.X = Unit.Pos.Horizontal.X + Offset.X;
                NewPos.Y = Unit.Pos.Horizontal.Y + Offset.Y;
                ResultUnit.Pos = Map.TileAlignedPosFromMapPos(NewPos, ResultUnit.Type.get_GetFootprintSelected(Unit.Rotation));
            }
        }

        public class clsObjectRotationOffset : clsObjectAction
        {
            public int Offset;

            private sXY_int NewPos;

            protected override void _ActionPerform()
            {
                ResultUnit.Rotation = Unit.Rotation + Offset;
                if ( ResultUnit.Rotation < 0 )
                {
                    ResultUnit.Rotation += 360;
                }
                else if ( ResultUnit.Rotation >= 360 )
                {
                    ResultUnit.Rotation -= 360;
                }
            }
        }

        public class clsObjectTemplateToDesign : clsObjectAction
        {
            private clsDroidDesign OldDroidType;
            private clsDroidDesign NewDroidType;

            protected override void ActionCondition()
            {
                base.ActionCondition();

                if ( Unit.Type.Type == clsUnitType.enumType.PlayerDroid )
                {
                    OldDroidType = (clsDroidDesign)Unit.Type;
                    ActionPerformed = OldDroidType.IsTemplate;
                }
                else
                {
                    OldDroidType = null;
                    ActionPerformed = false;
                }
            }

            protected override void _ActionPerform()
            {
                NewDroidType = new clsDroidDesign();
                ResultUnit.Type = NewDroidType;
                NewDroidType.CopyDesign(OldDroidType);
                NewDroidType.UpdateAttachments();
            }
        }

        public abstract class clsObjectComponent : clsObjectAction
        {
            private clsDroidDesign OldDroidType;
            protected clsDroidDesign NewDroidType;

            protected abstract void ChangeComponent();

            protected override void ActionCondition()
            {
                base.ActionCondition();

                if ( Unit.Type.Type == clsUnitType.enumType.PlayerDroid )
                {
                    OldDroidType = (clsDroidDesign)Unit.Type;
                    ActionPerformed = !OldDroidType.IsTemplate;
                }
                else
                {
                    OldDroidType = null;
                    ActionPerformed = false;
                }
            }

            protected override void _ActionPerform()
            {
                NewDroidType = new clsDroidDesign();
                ResultUnit.Type = NewDroidType;
                NewDroidType.CopyDesign(OldDroidType);

                ChangeComponent();

                NewDroidType.UpdateAttachments();
            }
        }

        public class clsObjectAlignment : clsObjectAction
        {
            protected override void _ActionPerform()
            {
                ResultUnit.Pos = Unit.MapLink.Source.TileAlignedPosFromMapPos(Unit.Pos.Horizontal, Unit.Type.get_GetFootprintNew(Unit.Rotation));
            }
        }

        public class clsObjectBody : clsObjectComponent
        {
            public clsBody Body;

            protected override void ChangeComponent()
            {
                NewDroidType.Body = Body;
            }
        }

        public class clsObjectPropulsion : clsObjectComponent
        {
            public clsPropulsion Propulsion;

            protected override void ChangeComponent()
            {
                NewDroidType.Propulsion = Propulsion;
            }
        }

        public class clsObjectTurret : clsObjectComponent
        {
            public clsTurret Turret;
            public int TurretNum;

            protected override void ChangeComponent()
            {
                switch ( TurretNum )
                {
                    case 0:
                        NewDroidType.Turret1 = Turret;
                        break;
                    case 1:
                        NewDroidType.Turret2 = Turret;
                        break;
                    case 2:
                        NewDroidType.Turret3 = Turret;
                        break;
                }
            }
        }

        public class clsObjectTurretCount : clsObjectComponent
        {
            public byte TurretCount;

            protected override void ChangeComponent()
            {
                NewDroidType.TurretCount = TurretCount;
            }
        }

        public class clsObjectDroidType : clsObjectComponent
        {
            public clsDroidDesign.clsTemplateDroidType DroidType;

            protected override void ChangeComponent()
            {
                NewDroidType.TemplateDroidType = DroidType;
            }
        }

        public class clsObjectSelect : SimpleListTool<clsUnit>
        {
            private clsUnit Unit;

            public void ActionPerform()
            {
                Unit.MapSelect();
            }

            public void SetItem(clsUnit Item)
            {
                Unit = Item;
            }
        }

        public class clsObjectPriorityOrderList : SimpleListTool<clsUnit>
        {
            private SimpleClassList<clsUnit> _Result = new SimpleClassList<clsUnit>();

            public SimpleClassList<clsUnit> Result
            {
                get { return _Result; }
            }

            private clsUnit Unit;

            public clsObjectPriorityOrderList()
            {
                _Result.MaintainOrder = true;
            }

            public void ActionPerform()
            {
                int A = 0;

                for ( A = 0; A <= _Result.Count - 1; A++ )
                {
                    if ( Unit.SavePriority > _Result[A].SavePriority )
                    {
                        break;
                    }
                }
                _Result.Insert(Unit, A);
            }

            public void SetItem(clsUnit Item)
            {
                Unit = Item;
            }
        }

        public class clsObjectFlattenTerrain : SimpleListTool<clsUnit>
        {
            private clsUnit Unit;

            public void ActionPerform()
            {
                clsMap Map = Unit.MapLink.Source;
                sXY_int VertexPos = new sXY_int();
                int X = 0;
                int Y = 0;
                double Total = 0;
                byte Average = 0;
                sXY_int Footprint = Unit.Type.get_GetFootprintSelected(Unit.Rotation);
                sXY_int Start = new sXY_int();
                sXY_int Finish = new sXY_int();
                int Samples = 0;

                Map.GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Footprint, Start, Finish);

                for ( Y = Start.Y; Y <= Finish.Y + 1; Y++ )
                {
                    VertexPos.Y = Y;
                    for ( X = Start.X; X <= Finish.X + 1; X++ )
                    {
                        VertexPos.X = X;

                        Total += Map.Terrain.Vertices[VertexPos.X, VertexPos.Y].Height;
                        Samples++;
                    }
                }

                if ( Samples >= 1 )
                {
                    Average = (byte)(MathUtil.Clamp_int((int)(Total / Samples), byte.MinValue, byte.MaxValue));
                    for ( Y = Start.Y; Y <= Finish.Y + 1; Y++ )
                    {
                        VertexPos.Y = Y;
                        for ( X = Start.X; X <= Finish.X + 1; X++ )
                        {
                            VertexPos.X = X;

                            Map.Terrain.Vertices[VertexPos.X, VertexPos.Y].Height = Average;
                            Map.SectorGraphicsChanges.VertexAndNormalsChanged(VertexPos);
                            Map.SectorUnitHeightsChanges.VertexChanged(VertexPos);
                            Map.SectorTerrainUndoChanges.VertexChanged(VertexPos);
                        }
                    }
                }
            }

            public void SetItem(clsUnit Item)
            {
                Unit = Item;
            }
        }

        public class clsStructureWriteWZ : SimpleListTool<clsUnit>
        {
            public BinaryWriter File;
            public sWrite_WZ_Args.enumCompileType CompileType;
            public int PlayerCount;

            private clsUnit Unit;

            private clsStructureType StructureType;
            private byte[] StruZeroBytesA = new byte[12];
            private byte[] StruZeroBytesB = new byte[40];

            public void ActionPerform()
            {
                if ( CompileType == sWrite_WZ_Args.enumCompileType.Unspecified )
                {
                    Debugger.Break();
                    return;
                }

                StructureType = (clsStructureType)Unit.Type;
                IOUtil.WriteTextOfLength(File, 40, StructureType.Code);
                File.Write(Unit.ID);
                File.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.X));
                File.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.Y));
                File.Write(Convert.ToBoolean((uint)Unit.Pos.Altitude));
                File.Write(Convert.ToBoolean((uint)Unit.Rotation));
                switch ( CompileType )
                {
                    case sWrite_WZ_Args.enumCompileType.Multiplayer:
                        File.Write(Unit.GetBJOMultiplayerPlayerNum(PlayerCount));
                        break;
                    case sWrite_WZ_Args.enumCompileType.Campaign:
                        File.Write(Unit.GetBJOCampaignPlayerNum());
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
                File.Write(StruZeroBytesA);
                File.Write((byte)1);
                File.Write((byte)26);
                File.Write((byte)127);
                File.Write((byte)0);
                File.Write(StruZeroBytesB);
            }

            public void SetItem(clsUnit Item)
            {
                Unit = Item;
            }
        }
    }
}