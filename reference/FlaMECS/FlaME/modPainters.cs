namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;

    [StandardModule]
    internal sealed class modPainters
    {
        public static void CreatePainterArizona()
        {
            modProgram.Painter_Arizona = new clsPainter();
            clsPainter.clsTerrain newTerrain = new clsPainter.clsTerrain {
                Name = "Red"
            };
            modProgram.Painter_Arizona.Terrain_Add(newTerrain);
            clsPainter.clsTerrain terrain7 = new clsPainter.clsTerrain {
                Name = "Yellow"
            };
            modProgram.Painter_Arizona.Terrain_Add(terrain7);
            clsPainter.clsTerrain terrain5 = new clsPainter.clsTerrain {
                Name = "Sand"
            };
            modProgram.Painter_Arizona.Terrain_Add(terrain5);
            clsPainter.clsTerrain terrain = new clsPainter.clsTerrain {
                Name = "Brown"
            };
            modProgram.Painter_Arizona.Terrain_Add(terrain);
            clsPainter.clsTerrain terrain3 = new clsPainter.clsTerrain {
                Name = "Green"
            };
            modProgram.Painter_Arizona.Terrain_Add(terrain3);
            clsPainter.clsTerrain terrain2 = new clsPainter.clsTerrain {
                Name = "Concrete"
            };
            modProgram.Painter_Arizona.Terrain_Add(terrain2);
            clsPainter.clsTerrain terrain6 = new clsPainter.clsTerrain {
                Name = "Water"
            };
            modProgram.Painter_Arizona.Terrain_Add(terrain6);
            newTerrain.Tiles.Tile_Add(0x30, TileOrientation.TileDirection_None, 1);
            newTerrain.Tiles.Tile_Add(0x35, TileOrientation.TileDirection_None, 1);
            newTerrain.Tiles.Tile_Add(0x36, TileOrientation.TileDirection_None, 1);
            newTerrain.Tiles.Tile_Add(0x4c, TileOrientation.TileDirection_None, 1);
            terrain7.Tiles.Tile_Add(9, TileOrientation.TileDirection_None, 1);
            terrain7.Tiles.Tile_Add(11, TileOrientation.TileDirection_None, 1);
            terrain5.Tiles.Tile_Add(12, TileOrientation.TileDirection_None, 1);
            terrain.Tiles.Tile_Add(5, TileOrientation.TileDirection_None, 1);
            terrain.Tiles.Tile_Add(6, TileOrientation.TileDirection_None, 1);
            terrain.Tiles.Tile_Add(7, TileOrientation.TileDirection_None, 1);
            terrain.Tiles.Tile_Add(8, TileOrientation.TileDirection_None, 1);
            terrain3.Tiles.Tile_Add(0x17, TileOrientation.TileDirection_None, 1);
            terrain2.Tiles.Tile_Add(0x16, TileOrientation.TileDirection_None, 1);
            terrain6.Tiles.Tile_Add(0x11, TileOrientation.TileDirection_None, 1);
            clsPainter.clsCliff_Brush newBrush = new clsPainter.clsCliff_Brush {
                Name = "Red Cliff",
                Terrain_Inner = newTerrain,
                Terrain_Outer = newTerrain
            };
            newBrush.Tiles_Straight.Tile_Add(0x2e, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Straight.Tile_Add(0x47, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Corner_In.Tile_Add(0x2d, TileOrientation.TileDirection_TopRight, 1);
            newBrush.Tiles_Corner_In.Tile_Add(0x4b, TileOrientation.TileDirection_TopLeft, 1);
            newBrush.Tiles_Corner_Out.Tile_Add(0x2d, TileOrientation.TileDirection_BottomLeft, 1);
            newBrush.Tiles_Corner_Out.Tile_Add(0x4b, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.CliffBrush_Add(newBrush);
            clsPainter.clsTransition_Brush brush = new clsPainter.clsTransition_Brush {
                Name = "Water->Sand",
                Terrain_Inner = terrain6,
                Terrain_Outer = terrain5
            };
            brush.Tiles_Straight.Tile_Add(14, TileOrientation.TileDirection_Bottom, 1);
            brush.Tiles_Corner_In.Tile_Add(0x10, TileOrientation.TileDirection_BottomLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(15, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Water->Green",
                Terrain_Inner = terrain6,
                Terrain_Outer = terrain3
            };
            brush.Tiles_Straight.Tile_Add(0x1f, TileOrientation.TileDirection_Top, 1);
            brush.Tiles_Corner_In.Tile_Add(0x21, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x20, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Yellow->Red",
                Terrain_Inner = terrain7,
                Terrain_Outer = newTerrain
            };
            brush.Tiles_Straight.Tile_Add(0x1b, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(0x1c, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x1d, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Sand->Red",
                Terrain_Inner = terrain5,
                Terrain_Outer = newTerrain
            };
            brush.Tiles_Straight.Tile_Add(0x2b, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Corner_In.Tile_Add(0x2a, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x29, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Sand->Yellow",
                Terrain_Inner = terrain5,
                Terrain_Outer = terrain7
            };
            brush.Tiles_Straight.Tile_Add(10, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Corner_In.Tile_Add(1, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Brown->Red",
                Terrain_Inner = terrain,
                Terrain_Outer = newTerrain
            };
            brush.Tiles_Straight.Tile_Add(0x22, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Corner_In.Tile_Add(0x24, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x23, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Brown->Yellow",
                Terrain_Inner = terrain,
                Terrain_Outer = terrain7
            };
            brush.Tiles_Straight.Tile_Add(0x26, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Corner_In.Tile_Add(0x27, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(40, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Brown->Sand",
                Terrain_Inner = terrain,
                Terrain_Outer = terrain5
            };
            brush.Tiles_Straight.Tile_Add(2, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Corner_In.Tile_Add(3, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(4, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Brown->Green",
                Terrain_Inner = terrain,
                Terrain_Outer = terrain3
            };
            brush.Tiles_Straight.Tile_Add(0x18, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Corner_In.Tile_Add(0x1a, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x19, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Concrete->Red",
                Terrain_Inner = terrain2,
                Terrain_Outer = newTerrain
            };
            brush.Tiles_Straight.Tile_Add(0x15, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(0x13, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(20, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(brush);
            clsPainter.clsRoad newRoad = new clsPainter.clsRoad();
            newRoad = new clsPainter.clsRoad {
                Name = "Road"
            };
            modProgram.Painter_Arizona.Road_Add(newRoad);
            clsPainter.clsRoad road2 = new clsPainter.clsRoad();
            road2 = new clsPainter.clsRoad {
                Name = "Track"
            };
            modProgram.Painter_Arizona.Road_Add(road2);
            clsPainter.clsRoad_Brush newRoadBrush = new clsPainter.clsRoad_Brush {
                Road = newRoad,
                Terrain = newTerrain
            };
            newRoadBrush.Tile_TIntersection.Tile_Add(0x39, TileOrientation.TileDirection_Bottom, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x3b, TileOrientation.TileDirection_Left, 1);
            newRoadBrush.Tile_End.Tile_Add(0x2f, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Arizona.RoadBrush_Add(newRoadBrush);
            newRoadBrush = new clsPainter.clsRoad_Brush {
                Road = road2,
                Terrain = newTerrain
            };
            newRoadBrush.Tile_CrossIntersection.Tile_Add(0x49, TileOrientation.TileDirection_None, 1);
            newRoadBrush.Tile_TIntersection.Tile_Add(0x48, TileOrientation.TileDirection_Right, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x31, TileOrientation.TileDirection_Top, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x33, TileOrientation.TileDirection_Top, 2);
            newRoadBrush.Tile_Corner_In.Tile_Add(50, TileOrientation.TileDirection_BottomRight, 1);
            newRoadBrush.Tile_End.Tile_Add(0x34, TileOrientation.TileDirection_Bottom, 1);
            modProgram.Painter_Arizona.RoadBrush_Add(newRoadBrush);
            clsGeneratorTileset tileset = modGenerator.Generator_TilesetArizona;
            tileset.OldTextureLayers.Layers = new modProgram.sLayerList.clsLayer[0];
            tileset.OldTextureLayers.LayerCount = 0;
            tileset = null;
            modProgram.sLayerList.clsLayer newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset2 = modGenerator.Generator_TilesetArizona;
            tileset2.OldTextureLayers.Layer_Insert(tileset2.OldTextureLayers.LayerCount, newLayer);
            tileset2 = null;
            modProgram.sLayerList.clsLayer layer2 = newLayer;
            layer2.Terrain = newTerrain;
            layer2.HeightMax = 255f;
            layer2.SlopeMax = 1.570796f;
            layer2.Scale = 0f;
            layer2.Density = 1f;
            layer2 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset3 = modGenerator.Generator_TilesetArizona;
            tileset3.OldTextureLayers.Layer_Insert(tileset3.OldTextureLayers.LayerCount, newLayer);
            tileset3 = null;
            modProgram.sLayerList.clsLayer layer3 = newLayer;
            layer3.Terrain = terrain5;
            layer3.HeightMax = -1f;
            layer3.SlopeMax = -1f;
            layer3.Scale = 0f;
            layer3.Density = 1f;
            layer3 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset4 = modGenerator.Generator_TilesetArizona;
            tileset4.OldTextureLayers.Layer_Insert(tileset4.OldTextureLayers.LayerCount, newLayer);
            tileset4 = null;
            modProgram.sLayerList.clsLayer layer4 = newLayer;
            layer4.WithinLayer = 1;
            layer4.Terrain = terrain6;
            layer4.HeightMax = 255f;
            layer4.SlopeMax = 1.570796f;
            layer4.Scale = 0f;
            layer4.Density = 1f;
            layer4 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset5 = modGenerator.Generator_TilesetArizona;
            tileset5.OldTextureLayers.Layer_Insert(tileset5.OldTextureLayers.LayerCount, newLayer);
            tileset5 = null;
            modProgram.sLayerList.clsLayer layer5 = newLayer;
            layer5.AvoidLayers[1] = true;
            layer5.Terrain = terrain;
            layer5.HeightMax = 255f;
            layer5.SlopeMax = -1f;
            layer5.Scale = 3f;
            layer5.Density = 0.35f;
            layer5 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset6 = modGenerator.Generator_TilesetArizona;
            tileset6.OldTextureLayers.Layer_Insert(tileset6.OldTextureLayers.LayerCount, newLayer);
            tileset6 = null;
            modProgram.sLayerList.clsLayer layer6 = newLayer;
            layer6.AvoidLayers[1] = true;
            layer6.AvoidLayers[3] = true;
            layer6.Terrain = terrain7;
            layer6.HeightMax = 255f;
            layer6.SlopeMax = -1f;
            layer6.Scale = 2f;
            layer6.Density = 0.6f;
            layer6 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset7 = modGenerator.Generator_TilesetArizona;
            tileset7.OldTextureLayers.Layer_Insert(tileset7.OldTextureLayers.LayerCount, newLayer);
            tileset7 = null;
            modProgram.sLayerList.clsLayer layer7 = newLayer;
            layer7.AvoidLayers[1] = true;
            layer7.WithinLayer = 4;
            layer7.Terrain = terrain5;
            layer7.HeightMax = 255f;
            layer7.SlopeMax = 1.570796f;
            layer7.Scale = 1f;
            layer7.Density = 0.5f;
            layer7 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset8 = modGenerator.Generator_TilesetArizona;
            tileset8.OldTextureLayers.Layer_Insert(tileset8.OldTextureLayers.LayerCount, newLayer);
            tileset8 = null;
            modProgram.sLayerList.clsLayer layer8 = newLayer;
            layer8.AvoidLayers[1] = true;
            layer8.WithinLayer = 3;
            layer8.Terrain = terrain3;
            layer8.HeightMax = 255f;
            layer8.SlopeMax = 1.570796f;
            layer8.Scale = 2f;
            layer8.Density = 0.4f;
            layer8 = null;
        }

        public static void CreatePainterRockies()
        {
            modProgram.Painter_Rockies = new clsPainter();
            clsPainter.clsTerrain newTerrain = new clsPainter.clsTerrain {
                Name = "Grass"
            };
            modProgram.Painter_Rockies.Terrain_Add(newTerrain);
            clsPainter.clsTerrain terrain5 = new clsPainter.clsTerrain {
                Name = "Gravel"
            };
            modProgram.Painter_Rockies.Terrain_Add(terrain5);
            clsPainter.clsTerrain terrain2 = new clsPainter.clsTerrain {
                Name = "Dirt"
            };
            modProgram.Painter_Rockies.Terrain_Add(terrain2);
            clsPainter.clsTerrain terrain4 = new clsPainter.clsTerrain {
                Name = "Grass Snow"
            };
            modProgram.Painter_Rockies.Terrain_Add(terrain4);
            clsPainter.clsTerrain terrain6 = new clsPainter.clsTerrain {
                Name = "Gravel Snow"
            };
            modProgram.Painter_Rockies.Terrain_Add(terrain6);
            clsPainter.clsTerrain terrain7 = new clsPainter.clsTerrain {
                Name = "Snow"
            };
            modProgram.Painter_Rockies.Terrain_Add(terrain7);
            clsPainter.clsTerrain terrain = new clsPainter.clsTerrain {
                Name = "Concrete"
            };
            modProgram.Painter_Rockies.Terrain_Add(terrain);
            clsPainter.clsTerrain terrain8 = new clsPainter.clsTerrain {
                Name = "Water"
            };
            modProgram.Painter_Rockies.Terrain_Add(terrain8);
            newTerrain.Tiles.Tile_Add(0, TileOrientation.TileDirection_None, 1);
            terrain5.Tiles.Tile_Add(5, TileOrientation.TileDirection_None, 1);
            terrain5.Tiles.Tile_Add(6, TileOrientation.TileDirection_None, 1);
            terrain5.Tiles.Tile_Add(7, TileOrientation.TileDirection_None, 1);
            terrain2.Tiles.Tile_Add(0x35, TileOrientation.TileDirection_None, 1);
            terrain4.Tiles.Tile_Add(0x17, TileOrientation.TileDirection_None, 1);
            terrain6.Tiles.Tile_Add(0x29, TileOrientation.TileDirection_None, 1);
            terrain7.Tiles.Tile_Add(0x40, TileOrientation.TileDirection_None, 1);
            terrain.Tiles.Tile_Add(0x16, TileOrientation.TileDirection_None, 1);
            terrain8.Tiles.Tile_Add(0x11, TileOrientation.TileDirection_None, 1);
            clsPainter.clsCliff_Brush newBrush = new clsPainter.clsCliff_Brush {
                Name = "Gravel Cliff",
                Terrain_Inner = terrain5,
                Terrain_Outer = terrain5
            };
            newBrush.Tiles_Straight.Tile_Add(0x2e, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Straight.Tile_Add(0x47, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Corner_In.Tile_Add(0x2d, TileOrientation.TileDirection_TopRight, 1);
            newBrush.Tiles_Corner_Out.Tile_Add(0x2d, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(newBrush);
            newBrush = new clsPainter.clsCliff_Brush {
                Name = "Gravel Snow -> Gravel Cliff",
                Terrain_Inner = terrain6,
                Terrain_Outer = terrain5
            };
            newBrush.Tiles_Straight.Tile_Add(0x1d, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Corner_In.Tile_Add(9, TileOrientation.TileDirection_TopLeft, 1);
            newBrush.Tiles_Corner_Out.Tile_Add(0x2a, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(newBrush);
            newBrush = new clsPainter.clsCliff_Brush {
                Name = "Snow -> Gravel Cliff",
                Terrain_Inner = terrain7,
                Terrain_Outer = terrain5
            };
            newBrush.Tiles_Straight.Tile_Add(0x44, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Corner_In.Tile_Add(0x3f, TileOrientation.TileDirection_TopLeft, 1);
            newBrush.Tiles_Corner_Out.Tile_Add(0x2a, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(newBrush);
            newBrush = new clsPainter.clsCliff_Brush {
                Name = "Gravel Snow Cliff",
                Terrain_Inner = terrain6,
                Terrain_Outer = terrain6
            };
            newBrush.Tiles_Straight.Tile_Add(0x2c, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Corner_In.Tile_Add(9, TileOrientation.TileDirection_TopLeft, 1);
            newBrush.Tiles_Corner_Out.Tile_Add(9, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(newBrush);
            newBrush = new clsPainter.clsCliff_Brush {
                Name = "Snow -> Gravel Snow Cliff",
                Terrain_Inner = terrain7,
                Terrain_Outer = terrain6
            };
            newBrush.Tiles_Straight.Tile_Add(0x4e, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Corner_In.Tile_Add(0x3f, TileOrientation.TileDirection_TopLeft, 1);
            newBrush.Tiles_Corner_Out.Tile_Add(9, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(newBrush);
            newBrush = new clsPainter.clsCliff_Brush {
                Name = "Snow -> Snow Cliff",
                Terrain_Inner = terrain7,
                Terrain_Outer = terrain7
            };
            newBrush.Tiles_Straight.Tile_Add(0x4e, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Corner_In.Tile_Add(0x3f, TileOrientation.TileDirection_TopLeft, 1);
            newBrush.Tiles_Corner_Out.Tile_Add(0x3f, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(newBrush);
            clsPainter.clsTransition_Brush brush = new clsPainter.clsTransition_Brush {
                Name = "Water -> Grass",
                Terrain_Inner = terrain8,
                Terrain_Outer = newTerrain
            };
            brush.Tiles_Straight.Tile_Add(14, TileOrientation.TileDirection_Bottom, 1);
            brush.Tiles_Corner_In.Tile_Add(0x10, TileOrientation.TileDirection_BottomLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(15, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Water -> Gravel",
                Terrain_Inner = terrain8,
                Terrain_Outer = terrain5
            };
            brush.Tiles_Straight.Tile_Add(0x1f, TileOrientation.TileDirection_Top, 1);
            brush.Tiles_Corner_In.Tile_Add(0x20, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x21, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Grass -> Gravel",
                Terrain_Inner = newTerrain,
                Terrain_Outer = terrain5
            };
            brush.Tiles_Straight.Tile_Add(2, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Corner_In.Tile_Add(3, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(4, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Grass -> Grass Snow",
                Terrain_Inner = newTerrain,
                Terrain_Outer = terrain4
            };
            brush.Tiles_Straight.Tile_Add(0x1a, TileOrientation.TileDirection_Top, 1);
            brush.Tiles_Corner_In.Tile_Add(0x19, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x18, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Grass -> Dirt",
                Terrain_Inner = newTerrain,
                Terrain_Outer = terrain2
            };
            brush.Tiles_Straight.Tile_Add(0x22, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(0x23, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x24, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Gravel Snow -> Gravel",
                Terrain_Inner = terrain6,
                Terrain_Outer = terrain5
            };
            brush.Tiles_Straight.Tile_Add(12, TileOrientation.TileDirection_Bottom, 1);
            brush.Tiles_Corner_In.Tile_Add(10, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(11, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Snow -> Gravel Snow",
                Terrain_Inner = terrain7,
                Terrain_Outer = terrain6
            };
            brush.Tiles_Straight.Tile_Add(0x43, TileOrientation.TileDirection_Bottom, 1);
            brush.Tiles_Corner_In.Tile_Add(0x41, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x42, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Concrete -> Dirt",
                Terrain_Inner = terrain,
                Terrain_Outer = terrain2
            };
            brush.Tiles_Straight.Tile_Add(0x15, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(0x13, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(20, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Gravel -> Dirt",
                Terrain_Inner = terrain5,
                Terrain_Outer = terrain2
            };
            brush.Tiles_Straight.Tile_Add(0x26, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Corner_In.Tile_Add(40, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x27, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(brush);
            clsPainter.clsRoad newRoad = new clsPainter.clsRoad {
                Name = "Road"
            };
            modProgram.Painter_Rockies.Road_Add(newRoad);
            clsPainter.clsRoad_Brush newRoadBrush = new clsPainter.clsRoad_Brush {
                Road = newRoad,
                Terrain = terrain2
            };
            newRoadBrush.Tile_TIntersection.Tile_Add(13, TileOrientation.TileDirection_Bottom, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x3b, TileOrientation.TileDirection_Left, 1);
            newRoadBrush.Tile_End.Tile_Add(60, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Rockies.RoadBrush_Add(newRoadBrush);
            clsPainter.clsRoad road2 = new clsPainter.clsRoad {
                Name = "Track"
            };
            modProgram.Painter_Rockies.Road_Add(road2);
            newRoadBrush = new clsPainter.clsRoad_Brush {
                Road = road2,
                Terrain = terrain2
            };
            newRoadBrush.Tile_TIntersection.Tile_Add(0x48, TileOrientation.TileDirection_Right, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x31, TileOrientation.TileDirection_Top, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x33, TileOrientation.TileDirection_Top, 2);
            newRoadBrush.Tile_Corner_In.Tile_Add(50, TileOrientation.TileDirection_BottomRight, 1);
            newRoadBrush.Tile_End.Tile_Add(0x34, TileOrientation.TileDirection_Bottom, 1);
            modProgram.Painter_Rockies.RoadBrush_Add(newRoadBrush);
            clsGeneratorTileset tileset = modGenerator.Generator_TilesetRockies;
            tileset.OldTextureLayers.Layers = new modProgram.sLayerList.clsLayer[0];
            tileset.OldTextureLayers.LayerCount = 0;
            tileset = null;
            modProgram.sLayerList.clsLayer newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset2 = modGenerator.Generator_TilesetRockies;
            tileset2.OldTextureLayers.Layer_Insert(tileset2.OldTextureLayers.LayerCount, newLayer);
            tileset2 = null;
            modProgram.sLayerList.clsLayer layer2 = newLayer;
            layer2.Terrain = terrain5;
            layer2.HeightMax = 255f;
            layer2.SlopeMax = 1.570796f;
            layer2.Scale = 0f;
            layer2.Density = 1f;
            layer2 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset3 = modGenerator.Generator_TilesetRockies;
            tileset3.OldTextureLayers.Layer_Insert(tileset3.OldTextureLayers.LayerCount, newLayer);
            tileset3 = null;
            modProgram.sLayerList.clsLayer layer3 = newLayer;
            layer3.Terrain = terrain8;
            layer3.HeightMax = -1f;
            layer3.SlopeMax = -1f;
            layer3.Scale = 0f;
            layer3.Density = 1f;
            layer3 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset4 = modGenerator.Generator_TilesetRockies;
            tileset4.OldTextureLayers.Layer_Insert(tileset4.OldTextureLayers.LayerCount, newLayer);
            tileset4 = null;
            modProgram.sLayerList.clsLayer layer4 = newLayer;
            layer4.AvoidLayers[1] = true;
            layer4.Terrain = newTerrain;
            layer4.HeightMax = 60f;
            layer4.SlopeMax = -1f;
            layer4.Scale = 0f;
            layer4.Density = 1f;
            layer4 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset5 = modGenerator.Generator_TilesetRockies;
            tileset5.OldTextureLayers.Layer_Insert(tileset5.OldTextureLayers.LayerCount, newLayer);
            tileset5 = null;
            modProgram.sLayerList.clsLayer layer5 = newLayer;
            layer5.AvoidLayers[1] = true;
            layer5.AvoidLayers[3] = true;
            layer5.Terrain = terrain6;
            layer5.HeightMin = 150f;
            layer5.HeightMax = 255f;
            layer5.SlopeMax = 1.570796f;
            layer5.Scale = 0f;
            layer5.Density = 1f;
            layer5 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset6 = modGenerator.Generator_TilesetRockies;
            tileset6.OldTextureLayers.Layer_Insert(tileset6.OldTextureLayers.LayerCount, newLayer);
            tileset6 = null;
            modProgram.sLayerList.clsLayer layer6 = newLayer;
            layer6.WithinLayer = 3;
            layer6.AvoidLayers[1] = true;
            layer6.Terrain = terrain7;
            layer6.HeightMin = 200f;
            layer6.HeightMax = 255f;
            layer6.SlopeMax = 1.570796f;
            layer6.Scale = 0f;
            layer6.Density = 1f;
            layer6 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset7 = modGenerator.Generator_TilesetRockies;
            tileset7.OldTextureLayers.Layer_Insert(tileset7.OldTextureLayers.LayerCount, newLayer);
            tileset7 = null;
            modProgram.sLayerList.clsLayer layer7 = newLayer;
            layer7.WithinLayer = 3;
            layer7.AvoidLayers[4] = true;
            layer7.Terrain = terrain7;
            layer7.HeightMin = 150f;
            layer7.HeightMax = 255f;
            layer7.SlopeMax = -1f;
            layer7.Scale = 1.5f;
            layer7.Density = 0.45f;
            layer7 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset8 = modGenerator.Generator_TilesetRockies;
            tileset8.OldTextureLayers.Layer_Insert(tileset8.OldTextureLayers.LayerCount, newLayer);
            tileset8 = null;
            modProgram.sLayerList.clsLayer layer8 = newLayer;
            layer8.AvoidLayers[1] = true;
            layer8.AvoidLayers[2] = true;
            layer8.AvoidLayers[3] = true;
            layer8.Terrain = terrain6;
            layer8.HeightMin = 0f;
            layer8.HeightMax = 255f;
            layer8.SlopeMax = -1f;
            layer8.Scale = 1.5f;
            layer8.Density = 0.45f;
            layer8 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset9 = modGenerator.Generator_TilesetRockies;
            tileset9.OldTextureLayers.Layer_Insert(tileset9.OldTextureLayers.LayerCount, newLayer);
            tileset9 = null;
            modProgram.sLayerList.clsLayer layer9 = newLayer;
            layer9.AvoidLayers[1] = true;
            layer9.WithinLayer = 2;
            layer9.Terrain = terrain2;
            layer9.HeightMin = 0f;
            layer9.HeightMax = 255f;
            layer9.SlopeMax = 1.570796f;
            layer9.Scale = 1f;
            layer9.Density = 0.3f;
            layer9 = null;
        }

        public static void CreatePainterUrban()
        {
            modProgram.Painter_Urban = new clsPainter();
            clsPainter.clsTerrain newTerrain = new clsPainter.clsTerrain {
                Name = "Green"
            };
            modProgram.Painter_Urban.Terrain_Add(newTerrain);
            clsPainter.clsTerrain terrain = new clsPainter.clsTerrain {
                Name = "Blue"
            };
            modProgram.Painter_Urban.Terrain_Add(terrain);
            clsPainter.clsTerrain terrain3 = new clsPainter.clsTerrain {
                Name = "Gray"
            };
            modProgram.Painter_Urban.Terrain_Add(terrain3);
            clsPainter.clsTerrain terrain5 = new clsPainter.clsTerrain {
                Name = "Orange"
            };
            modProgram.Painter_Urban.Terrain_Add(terrain5);
            clsPainter.clsTerrain terrain2 = new clsPainter.clsTerrain {
                Name = "Concrete"
            };
            modProgram.Painter_Urban.Terrain_Add(terrain2);
            clsPainter.clsTerrain terrain6 = new clsPainter.clsTerrain {
                Name = "Water"
            };
            modProgram.Painter_Urban.Terrain_Add(terrain6);
            newTerrain.Tiles.Tile_Add(50, TileOrientation.TileDirection_None, 1);
            terrain.Tiles.Tile_Add(0, TileOrientation.TileDirection_None, 14);
            terrain.Tiles.Tile_Add(2, TileOrientation.TileDirection_None, 1);
            terrain3.Tiles.Tile_Add(5, TileOrientation.TileDirection_None, 1);
            terrain3.Tiles.Tile_Add(7, TileOrientation.TileDirection_None, 4);
            terrain3.Tiles.Tile_Add(8, TileOrientation.TileDirection_None, 4);
            terrain3.Tiles.Tile_Add(0x4e, TileOrientation.TileDirection_None, 4);
            terrain5.Tiles.Tile_Add(0x1f, TileOrientation.TileDirection_None, 1);
            terrain5.Tiles.Tile_Add(0x16, TileOrientation.TileDirection_None, 50);
            terrain2.Tiles.Tile_Add(0x33, TileOrientation.TileDirection_None, 200);
            terrain6.Tiles.Tile_Add(0x11, TileOrientation.TileDirection_None, 1);
            clsPainter.clsCliff_Brush newBrush = new clsPainter.clsCliff_Brush {
                Name = "Cliff",
                Terrain_Inner = terrain3,
                Terrain_Outer = terrain3
            };
            newBrush.Tiles_Straight.Tile_Add(0x45, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Straight.Tile_Add(70, TileOrientation.TileDirection_Bottom, 1);
            newBrush.Tiles_Corner_In.Tile_Add(0x44, TileOrientation.TileDirection_TopRight, 1);
            newBrush.Tiles_Corner_Out.Tile_Add(0x44, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Urban.CliffBrush_Add(newBrush);
            clsPainter.clsTransition_Brush brush = new clsPainter.clsTransition_Brush {
                Name = "Water->Gray",
                Terrain_Inner = terrain6,
                Terrain_Outer = terrain3
            };
            brush.Tiles_Straight.Tile_Add(0x17, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Straight.Tile_Add(0x18, TileOrientation.TileDirection_Top, 1);
            brush.Tiles_Corner_In.Tile_Add(0x19, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x1a, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Water->Concrete",
                Terrain_Inner = terrain6,
                Terrain_Outer = terrain2
            };
            brush.Tiles_Straight.Tile_Add(13, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Straight.Tile_Add(14, TileOrientation.TileDirection_Bottom, 1);
            brush.Tiles_Corner_In.Tile_Add(0x10, TileOrientation.TileDirection_BottomLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(15, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Gray->Blue",
                Terrain_Inner = terrain3,
                Terrain_Outer = terrain
            };
            brush.Tiles_Straight.Tile_Add(6, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Corner_In.Tile_Add(4, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(3, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Concrete->Gray",
                Terrain_Inner = terrain2,
                Terrain_Outer = terrain3
            };
            brush.Tiles_Straight.Tile_Add(9, TileOrientation.TileDirection_Left, 1);
            brush.Tiles_Straight.Tile_Add(0x1b, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(30, TileOrientation.TileDirection_BottomLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(10, TileOrientation.TileDirection_BottomLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x1d, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Orange->Blue",
                Terrain_Inner = terrain5,
                Terrain_Outer = terrain
            };
            brush.Tiles_Straight.Tile_Add(0x21, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(0x22, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x23, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Orange->Green",
                Terrain_Inner = terrain5,
                Terrain_Outer = newTerrain
            };
            brush.Tiles_Straight.Tile_Add(0x27, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(0x26, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x25, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Orange->Gray",
                Terrain_Inner = terrain5,
                Terrain_Outer = terrain3
            };
            brush.Tiles_Straight.Tile_Add(60, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(0x49, TileOrientation.TileDirection_TopLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x48, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Orange->Concrete",
                Terrain_Inner = terrain5,
                Terrain_Outer = terrain2
            };
            brush.Tiles_Straight.Tile_Add(0x47, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(0x4c, TileOrientation.TileDirection_BottomRight, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x4b, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(brush);
            brush = new clsPainter.clsTransition_Brush {
                Name = "Gray->Green",
                Terrain_Inner = terrain3,
                Terrain_Outer = newTerrain
            };
            brush.Tiles_Straight.Tile_Add(0x4d, TileOrientation.TileDirection_Right, 1);
            brush.Tiles_Corner_In.Tile_Add(0x3a, TileOrientation.TileDirection_BottomLeft, 1);
            brush.Tiles_Corner_Out.Tile_Add(0x4f, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(brush);
            clsPainter.clsRoad newRoad = new clsPainter.clsRoad {
                Name = "Road"
            };
            modProgram.Painter_Urban.Road_Add(newRoad);
            clsPainter.clsRoad_Brush newRoadBrush = new clsPainter.clsRoad_Brush {
                Road = newRoad,
                Terrain = newTerrain
            };
            newRoadBrush.Tile_CrossIntersection.Tile_Add(0x31, TileOrientation.TileDirection_None, 1);
            newRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x2a, TileOrientation.TileDirection_Left, 1);
            newRoadBrush.Tile_End.Tile_Add(0x2d, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(newRoadBrush);
            newRoadBrush = new clsPainter.clsRoad_Brush {
                Road = newRoad,
                Terrain = terrain
            };
            newRoadBrush.Tile_CrossIntersection.Tile_Add(0x31, TileOrientation.TileDirection_None, 1);
            newRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x2a, TileOrientation.TileDirection_Left, 1);
            newRoadBrush.Tile_End.Tile_Add(0x29, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(newRoadBrush);
            newRoadBrush = new clsPainter.clsRoad_Brush {
                Road = newRoad,
                Terrain = terrain3
            };
            newRoadBrush.Tile_CrossIntersection.Tile_Add(0x31, TileOrientation.TileDirection_None, 1);
            newRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x2a, TileOrientation.TileDirection_Left, 1);
            newRoadBrush.Tile_End.Tile_Add(0x2b, TileOrientation.TileDirection_Left, 1);
            newRoadBrush.Tile_End.Tile_Add(0x2c, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(newRoadBrush);
            newRoadBrush = new clsPainter.clsRoad_Brush {
                Road = newRoad,
                Terrain = terrain5
            };
            newRoadBrush.Tile_CrossIntersection.Tile_Add(0x31, TileOrientation.TileDirection_None, 1);
            newRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x2a, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(newRoadBrush);
            newRoadBrush = new clsPainter.clsRoad_Brush {
                Road = newRoad,
                Terrain = terrain2
            };
            newRoadBrush.Tile_CrossIntersection.Tile_Add(0x31, TileOrientation.TileDirection_None, 1);
            newRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            newRoadBrush.Tile_Straight.Tile_Add(0x2a, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(newRoadBrush);
            clsGeneratorTileset tileset = modGenerator.Generator_TilesetUrban;
            tileset.OldTextureLayers.Layers = new modProgram.sLayerList.clsLayer[0];
            tileset.OldTextureLayers.LayerCount = 0;
            tileset = null;
            modProgram.sLayerList.clsLayer newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset2 = modGenerator.Generator_TilesetUrban;
            tileset2.OldTextureLayers.Layer_Insert(tileset2.OldTextureLayers.LayerCount, newLayer);
            tileset2 = null;
            modProgram.sLayerList.clsLayer layer2 = newLayer;
            layer2.Terrain = terrain3;
            layer2.HeightMax = 255f;
            layer2.SlopeMax = 1.570796f;
            layer2.Scale = 0f;
            layer2.Density = 1f;
            layer2 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset3 = modGenerator.Generator_TilesetUrban;
            tileset3.OldTextureLayers.Layer_Insert(tileset3.OldTextureLayers.LayerCount, newLayer);
            tileset3 = null;
            modProgram.sLayerList.clsLayer layer3 = newLayer;
            layer3.Terrain = terrain6;
            layer3.HeightMax = -1f;
            layer3.SlopeMax = -1f;
            layer3.Scale = 0f;
            layer3.Density = 1f;
            layer3 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset4 = modGenerator.Generator_TilesetUrban;
            tileset4.OldTextureLayers.Layer_Insert(tileset4.OldTextureLayers.LayerCount, newLayer);
            tileset4 = null;
            modProgram.sLayerList.clsLayer layer4 = newLayer;
            layer4.AvoidLayers[1] = true;
            layer4.Terrain = terrain;
            layer4.HeightMax = 255f;
            layer4.SlopeMax = -1f;
            layer4.Scale = 3f;
            layer4.Density = 0.3f;
            layer4 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset5 = modGenerator.Generator_TilesetUrban;
            tileset5.OldTextureLayers.Layer_Insert(tileset5.OldTextureLayers.LayerCount, newLayer);
            tileset5 = null;
            modProgram.sLayerList.clsLayer layer5 = newLayer;
            layer5.AvoidLayers[1] = true;
            layer5.AvoidLayers[2] = true;
            layer5.Terrain = terrain5;
            layer5.HeightMax = 255f;
            layer5.SlopeMax = -1f;
            layer5.Scale = 2.5f;
            layer5.Density = 0.4f;
            layer5 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset6 = modGenerator.Generator_TilesetUrban;
            tileset6.OldTextureLayers.Layer_Insert(tileset6.OldTextureLayers.LayerCount, newLayer);
            tileset6 = null;
            modProgram.sLayerList.clsLayer layer6 = newLayer;
            layer6.AvoidLayers[1] = true;
            layer6.AvoidLayers[2] = true;
            layer6.AvoidLayers[3] = true;
            layer6.Terrain = terrain2;
            layer6.HeightMax = 255f;
            layer6.SlopeMax = -1f;
            layer6.Scale = 1.5f;
            layer6.Density = 0.6f;
            layer6 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset7 = modGenerator.Generator_TilesetUrban;
            tileset7.OldTextureLayers.Layer_Insert(tileset7.OldTextureLayers.LayerCount, newLayer);
            tileset7 = null;
            modProgram.sLayerList.clsLayer layer7 = newLayer;
            layer7.AvoidLayers[1] = true;
            layer7.AvoidLayers[2] = true;
            layer7.AvoidLayers[3] = true;
            layer7.AvoidLayers[4] = true;
            layer7.Terrain = newTerrain;
            layer7.HeightMax = 255f;
            layer7.SlopeMax = -1f;
            layer7.Scale = 2.5f;
            layer7.Density = 0.6f;
            layer7 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset8 = modGenerator.Generator_TilesetUrban;
            tileset8.OldTextureLayers.Layer_Insert(tileset8.OldTextureLayers.LayerCount, newLayer);
            tileset8 = null;
            modProgram.sLayerList.clsLayer layer8 = newLayer;
            layer8.WithinLayer = 2;
            layer8.Terrain = terrain5;
            layer8.HeightMax = 255f;
            layer8.SlopeMax = 1.570796f;
            layer8.Scale = 1.5f;
            layer8.Density = 0.5f;
            layer8 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset9 = modGenerator.Generator_TilesetUrban;
            tileset9.OldTextureLayers.Layer_Insert(tileset9.OldTextureLayers.LayerCount, newLayer);
            tileset9 = null;
            modProgram.sLayerList.clsLayer layer9 = newLayer;
            layer9.WithinLayer = 3;
            layer9.Terrain = terrain;
            layer9.HeightMax = 255f;
            layer9.SlopeMax = 1.570796f;
            layer9.Scale = 1.5f;
            layer9.Density = 0.5f;
            layer9 = null;
            newLayer = new modProgram.sLayerList.clsLayer();
            clsGeneratorTileset tileset10 = modGenerator.Generator_TilesetUrban;
            tileset10.OldTextureLayers.Layer_Insert(tileset10.OldTextureLayers.LayerCount, newLayer);
            tileset10 = null;
            modProgram.sLayerList.clsLayer layer10 = newLayer;
            layer10.WithinLayer = 3;
            layer10.AvoidLayers[7] = true;
            layer10.Terrain = newTerrain;
            layer10.HeightMax = 255f;
            layer10.SlopeMax = 1.570796f;
            layer10.Scale = 1.5f;
            layer10.Density = 0.5f;
            layer10 = null;
        }
    }
}

