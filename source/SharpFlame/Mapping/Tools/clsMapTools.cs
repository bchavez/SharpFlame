using System;
using SharpFlame.Collections.Specialized;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public void Rotate(TileOrientation Orientation, enumObjectRotateMode ObjectRotateMode)
        {
            int X = 0;
            int Y = 0;
            XYInt Pos = new XYInt(0, 0);
            XYInt RotatedPos = new XYInt();
            XYInt NewTerrainPosA = TileUtil.GetRotatedPos(Orientation, new XYInt(0, 0), Terrain.TileSize);
            XYInt NewTerrainPosB = TileUtil.GetRotatedPos(Orientation, Terrain.TileSize, Terrain.TileSize);
            XYInt VertexLimits = new XYInt(Math.Max(NewTerrainPosA.X, NewTerrainPosB.X), Math.Max(NewTerrainPosA.Y, NewTerrainPosB.Y));
            clsTerrain NewTerrain = new clsTerrain(VertexLimits);
            XYInt NewTileLimits = new XYInt(NewTerrain.TileSize.X - 1, NewTerrain.TileSize.Y - 1);
            XYInt NewSideHLimits = new XYInt(NewTerrain.TileSize.X - 1, NewTerrain.TileSize.Y);
            XYInt NewSideVLimits = new XYInt(NewTerrain.TileSize.X, NewTerrain.TileSize.Y - 1);
            XYInt OldTileLimits = new XYInt(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1);
            XYInt OldPosLimits = new XYInt(Terrain.TileSize.X * App.TerrainGridSpacing, Terrain.TileSize.Y * App.TerrainGridSpacing);
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
                if ( ObjectRotateMode == enumObjectRotateMode.All )
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
                else if ( ObjectRotateMode == enumObjectRotateMode.Walls )
                {
                    if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                    {
                        if ( ((StructureTypeBase)Unit.TypeBase).StructureType == StructureTypeBase.enumStructureType.Wall )
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

            XYInt ZeroPos = new XYInt(0, 0);

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
            hmSource.MinMaxGet(ref MinMax);
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
            hmA.FadeMultiple(hmSource, ref hmAlteration, ref AlterationLevels);
            hmB.Rescale(hmA, Math.Max(Convert.ToDouble(Convert.ToDouble(MinMax.Min * hmSource.HeightScale) - Variation), 0.0D),
                Math.Min(Convert.ToDouble(Convert.ToDouble(MinMax.Max * hmSource.HeightScale) + Variation), 255.9D));
            for ( Y = 0; Y <= Terrain.TileSize.Y; Y++ )
            {
                for ( X = 0; X <= Terrain.TileSize.X; X++ )
                {
                    Terrain.Vertices[X, Y].Height = Convert.ToByte((hmB.HeightData.Height[Y, X] * hmB.HeightScale));
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
                        if ( Tileset.Tiles[TextureNum].DefaultType == App.TileTypeNum_Water )
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

        public void GenerateMasterTerrain(ref sGenerateMasterTerrainArgs Args)
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
                        if ( App.Random.Next() >= 0.5F )
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

            XYInt Pos = new XYInt();

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
                    Terrain.Tiles[X, Y].Texture.Orientation = new TileOrientation(App.Random.Next() >= 0.5F, App.Random.Next() >= 0.5F, App.Random.Next() >= 0.5F);
                }
            }
            SectorTerrainUndoChanges.SetAllChanged();
            SectorGraphicsChanges.SetAllChanged();
        }

        public void MapTexturer(ref sLayerList LayerList)
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
            XYInt Pos = new XYInt();

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
            XYInt TileNum = new XYInt();

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
                            if ( Tileset.Tiles[Terrain.Tiles[X, Y].Texture.TextureNum].DefaultType == App.TileTypeNum_Water )
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
    }
}