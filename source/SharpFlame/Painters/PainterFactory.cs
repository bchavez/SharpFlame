using SharpFlame.MathExtra;

namespace SharpFlame.Painters
{
    internal sealed class PainterFactory
    {
        public static void CreatePainterArizona()
        {
            CliffBrush NewBrushCliff;
            TransitionBrush NewBrush;
            RoadBrush NewRoadBrush;

            modProgram.Painter_Arizona = new Painter();

            //arizona

            Terrain Terrain_Red = new Terrain();
            Terrain_Red.Name = "Red";
            modProgram.Painter_Arizona.Terrain_Add(Terrain_Red);

            Terrain Terrain_Yellow = new Terrain();
            Terrain_Yellow.Name = "Yellow";
            modProgram.Painter_Arizona.Terrain_Add(Terrain_Yellow);

            Terrain Terrain_Sand = new Terrain();
            Terrain_Sand.Name = "Sand";
            modProgram.Painter_Arizona.Terrain_Add(Terrain_Sand);

            Terrain Terrain_Brown = new Terrain();
            Terrain_Brown.Name = "Brown";
            modProgram.Painter_Arizona.Terrain_Add(Terrain_Brown);

            Terrain Terrain_Green = new Terrain();
            Terrain_Green.Name = "Green";
            modProgram.Painter_Arizona.Terrain_Add(Terrain_Green);

            Terrain Terrain_Concrete = new Terrain();
            Terrain_Concrete.Name = "Concrete";
            modProgram.Painter_Arizona.Terrain_Add(Terrain_Concrete);

            Terrain Terrain_Water = new Terrain();
            Terrain_Water.Name = "Water";
            modProgram.Painter_Arizona.Terrain_Add(Terrain_Water);

            //red centre brush
            Terrain_Red.Tiles.Tile_Add(48, TileOrientation.TileDirection_None, 1);
            Terrain_Red.Tiles.Tile_Add(53, TileOrientation.TileDirection_None, 1);
            Terrain_Red.Tiles.Tile_Add(54, TileOrientation.TileDirection_None, 1);
            Terrain_Red.Tiles.Tile_Add(76, TileOrientation.TileDirection_None, 1);
            //yellow centre brushTerrain_yellow
            Terrain_Yellow.Tiles.Tile_Add(9, TileOrientation.TileDirection_None, 1);
            Terrain_Yellow.Tiles.Tile_Add(11, TileOrientation.TileDirection_None, 1);
            //sand centre brush
            Terrain_Sand.Tiles.Tile_Add(12, TileOrientation.TileDirection_None, 1);
            //brown centre brush
            Terrain_Brown.Tiles.Tile_Add(5, TileOrientation.TileDirection_None, 1);
            Terrain_Brown.Tiles.Tile_Add(6, TileOrientation.TileDirection_None, 1);
            Terrain_Brown.Tiles.Tile_Add(7, TileOrientation.TileDirection_None, 1);
            Terrain_Brown.Tiles.Tile_Add(8, TileOrientation.TileDirection_None, 1);
            //green centre brush
            Terrain_Green.Tiles.Tile_Add(23, TileOrientation.TileDirection_None, 1);
            //concrete centre brush
            Terrain_Concrete.Tiles.Tile_Add(22, TileOrientation.TileDirection_None, 1);
            //water centre brush
            Terrain_Water.Tiles.Tile_Add(17, TileOrientation.TileDirection_None, 1);
            //red cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Red Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Red;
            NewBrushCliff.Terrain_Outer = Terrain_Red;
            NewBrushCliff.Tiles_Straight.Tile_Add(46, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Straight.Tile_Add(71, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.Tile_Add(45, TileOrientation.TileDirection_TopRight, 1);
            NewBrushCliff.Tiles_Corner_In.Tile_Add(75, TileOrientation.TileDirection_TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(45, TileOrientation.TileDirection_BottomLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(75, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.CliffBrush_Add(NewBrushCliff);
            //water to sand transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water->Sand";
            NewBrush.Terrain_Inner = Terrain_Water;
            NewBrush.Terrain_Outer = Terrain_Sand;
            NewBrush.Tiles_Straight.Tile_Add(14, TileOrientation.TileDirection_Bottom, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(16, TileOrientation.TileDirection_BottomLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(15, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);
            //water to green transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water->Green";
            NewBrush.Terrain_Inner = Terrain_Water;
            NewBrush.Terrain_Outer = Terrain_Green;
            NewBrush.Tiles_Straight.Tile_Add(31, TileOrientation.TileDirection_Top, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(33, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(32, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);
            //yellow to red transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Yellow->Red";
            NewBrush.Terrain_Inner = Terrain_Yellow;
            NewBrush.Terrain_Outer = Terrain_Red;
            NewBrush.Tiles_Straight.Tile_Add(27, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(28, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(29, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);
            //sand to red transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Sand->Red";
            NewBrush.Terrain_Inner = Terrain_Sand;
            NewBrush.Terrain_Outer = Terrain_Red;
            NewBrush.Tiles_Straight.Tile_Add(43, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(42, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(41, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);
            //sand to yellow transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Sand->Yellow";
            NewBrush.Terrain_Inner = Terrain_Sand;
            NewBrush.Terrain_Outer = Terrain_Yellow;
            NewBrush.Tiles_Straight.Tile_Add(10, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(1, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(0, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);
            //brown to red transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Brown->Red";
            NewBrush.Terrain_Inner = Terrain_Brown;
            NewBrush.Terrain_Outer = Terrain_Red;
            NewBrush.Tiles_Straight.Tile_Add(34, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(36, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(35, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);
            //brown to yellow transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Brown->Yellow";
            NewBrush.Terrain_Inner = Terrain_Brown;
            NewBrush.Terrain_Outer = Terrain_Yellow;
            NewBrush.Tiles_Straight.Tile_Add(38, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(39, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(40, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);
            //brown to sand transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Brown->Sand";
            NewBrush.Terrain_Inner = Terrain_Brown;
            NewBrush.Terrain_Outer = Terrain_Sand;
            NewBrush.Tiles_Straight.Tile_Add(2, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(3, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(4, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);
            //brown to green transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Brown->Green";
            NewBrush.Terrain_Inner = Terrain_Brown;
            NewBrush.Terrain_Outer = Terrain_Green;
            NewBrush.Tiles_Straight.Tile_Add(24, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(26, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(25, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);
            //concrete to red transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Concrete->Red";
            NewBrush.Terrain_Inner = Terrain_Concrete;
            NewBrush.Terrain_Outer = Terrain_Red;
            NewBrush.Tiles_Straight.Tile_Add(21, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(19, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(20, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Arizona.TransitionBrush_Add(NewBrush);


            Road Road_Road = new Road();

            Road_Road = new Road();
            Road_Road.Name = "Road";
            modProgram.Painter_Arizona.Road_Add(Road_Road);

            Road Road_Track = new Road();

            Road_Track = new Road();
            Road_Track.Name = "Track";
            modProgram.Painter_Arizona.Road_Add(Road_Track);

            //road
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Red;
            NewRoadBrush.Tile_TIntersection.Tile_Add(57, TileOrientation.TileDirection_Bottom, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(59, TileOrientation.TileDirection_Left, 1);
            NewRoadBrush.Tile_End.Tile_Add(47, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Arizona.RoadBrush_Add(NewRoadBrush);
            //track
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Track;
            NewRoadBrush.Terrain = Terrain_Red;
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(73, TileOrientation.TileDirection_None, 1);
            NewRoadBrush.Tile_TIntersection.Tile_Add(72, TileOrientation.TileDirection_Right, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(49, TileOrientation.TileDirection_Top, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(51, TileOrientation.TileDirection_Top, 2);
            NewRoadBrush.Tile_Corner_In.Tile_Add(50, TileOrientation.TileDirection_BottomRight, 1);
            NewRoadBrush.Tile_End.Tile_Add(52, TileOrientation.TileDirection_Bottom, 1);
            modProgram.Painter_Arizona.RoadBrush_Add(NewRoadBrush);

            modGenerator.Generator_TilesetArizona.OldTextureLayers.Layers = new modProgram.sLayerList.clsLayer[0];
            modGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount = 0;

            modProgram.sLayerList.clsLayer NewLayer = default(modProgram.sLayerList.clsLayer);

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetArizona.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Red;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetArizona.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Sand;
            NewLayer.HeightMax = -1.0F; //signals water distribution
            NewLayer.SlopeMax = -1.0F; //signals water distribution
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetArizona.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 1;
            NewLayer.Terrain = Terrain_Water;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetArizona.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.Terrain = Terrain_Brown;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F; //signals to use cliff angle
            NewLayer.Scale = 3.0F;
            NewLayer.Density = 0.35F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetArizona.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.Terrain = Terrain_Yellow;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F; //signals to use cliff angle
            NewLayer.Scale = 2.0F;
            NewLayer.Density = 0.6F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetArizona.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.WithinLayer = 4;
            NewLayer.Terrain = Terrain_Sand;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 1.0F;
            NewLayer.Density = 0.5F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetArizona.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.WithinLayer = 3;
            NewLayer.Terrain = Terrain_Green;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 2.0F;
            NewLayer.Density = 0.4F;
        }

        public static void CreatePainterUrban()
        {
            CliffBrush NewBrushCliff = default(CliffBrush);
            TransitionBrush NewBrush = default(TransitionBrush);
            RoadBrush NewRoadBrush = default(RoadBrush);

            //urban

            modProgram.Painter_Urban = new Painter();

            Terrain Terrain_Green = new Terrain();
            Terrain_Green.Name = "Green";
            modProgram.Painter_Urban.Terrain_Add(Terrain_Green);

            Terrain Terrain_Blue = new Terrain();
            Terrain_Blue.Name = "Blue";
            modProgram.Painter_Urban.Terrain_Add(Terrain_Blue);

            Terrain Terrain_Gray = new Terrain();
            Terrain_Gray.Name = "Gray";
            modProgram.Painter_Urban.Terrain_Add(Terrain_Gray);

            Terrain Terrain_Orange = new Terrain();
            Terrain_Orange.Name = "Orange";
            modProgram.Painter_Urban.Terrain_Add(Terrain_Orange);

            Terrain Terrain_Concrete = new Terrain();
            Terrain_Concrete.Name = "Concrete";
            modProgram.Painter_Urban.Terrain_Add(Terrain_Concrete);

            Terrain Terrain_Water = new Terrain();
            Terrain_Water.Name = "Water";
            modProgram.Painter_Urban.Terrain_Add(Terrain_Water);

            //green centre brush
            Terrain_Green.Tiles.Tile_Add(50, TileOrientation.TileDirection_None, 1);
            //blue centre brush
            Terrain_Blue.Tiles.Tile_Add(0, TileOrientation.TileDirection_None, 14);
            Terrain_Blue.Tiles.Tile_Add(2, TileOrientation.TileDirection_None, 1); //line
            //gray centre brush
            Terrain_Gray.Tiles.Tile_Add(5, TileOrientation.TileDirection_None, 1);
            Terrain_Gray.Tiles.Tile_Add(7, TileOrientation.TileDirection_None, 4);
            Terrain_Gray.Tiles.Tile_Add(8, TileOrientation.TileDirection_None, 4);
            Terrain_Gray.Tiles.Tile_Add(78, TileOrientation.TileDirection_None, 4);
            //orange centre brush
            Terrain_Orange.Tiles.Tile_Add(31, TileOrientation.TileDirection_None, 1); //pipe
            Terrain_Orange.Tiles.Tile_Add(22, TileOrientation.TileDirection_None, 50);
            //concrete centre brush
            Terrain_Concrete.Tiles.Tile_Add(51, TileOrientation.TileDirection_None, 200);
            //water centre brush
            Terrain_Water.Tiles.Tile_Add(17, TileOrientation.TileDirection_None, 1);
            //cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Gray;
            NewBrushCliff.Terrain_Outer = Terrain_Gray;
            NewBrushCliff.Tiles_Straight.Tile_Add(69, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Straight.Tile_Add(70, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.Tile_Add(68, TileOrientation.TileDirection_TopRight, 1);
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(68, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Urban.CliffBrush_Add(NewBrushCliff);
            //water to gray transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water->Gray";
            NewBrush.Terrain_Inner = Terrain_Water;
            NewBrush.Terrain_Outer = Terrain_Gray;
            NewBrush.Tiles_Straight.Tile_Add(23, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Straight.Tile_Add(24, TileOrientation.TileDirection_Top, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(25, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(26, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(NewBrush);
            //water to concrete transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water->Concrete";
            NewBrush.Terrain_Inner = Terrain_Water;
            NewBrush.Terrain_Outer = Terrain_Concrete;
            NewBrush.Tiles_Straight.Tile_Add(13, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Straight.Tile_Add(14, TileOrientation.TileDirection_Bottom, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(16, TileOrientation.TileDirection_BottomLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(15, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(NewBrush);
            //gray to blue transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Gray->Blue";
            NewBrush.Terrain_Inner = Terrain_Gray;
            NewBrush.Terrain_Outer = Terrain_Blue;
            NewBrush.Tiles_Straight.Tile_Add(6, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(4, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(3, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(NewBrush);
            //concrete to gray transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Concrete->Gray";
            NewBrush.Terrain_Inner = Terrain_Concrete;
            NewBrush.Terrain_Outer = Terrain_Gray;
            NewBrush.Tiles_Straight.Tile_Add(9, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Straight.Tile_Add(27, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(30, TileOrientation.TileDirection_BottomLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(10, TileOrientation.TileDirection_BottomLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(29, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(NewBrush);
            //orange to blue transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Orange->Blue";
            NewBrush.Terrain_Inner = Terrain_Orange;
            NewBrush.Terrain_Outer = Terrain_Blue;
            NewBrush.Tiles_Straight.Tile_Add(33, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(34, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(35, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(NewBrush);
            //orange to green transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Orange->Green";
            NewBrush.Terrain_Inner = Terrain_Orange;
            NewBrush.Terrain_Outer = Terrain_Green;
            NewBrush.Tiles_Straight.Tile_Add(39, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(38, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(37, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(NewBrush);
            //orange to gray transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Orange->Gray";
            NewBrush.Terrain_Inner = Terrain_Orange;
            NewBrush.Terrain_Outer = Terrain_Gray;
            NewBrush.Tiles_Straight.Tile_Add(60, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(73, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(72, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(NewBrush);
            //orange to concrete transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Orange->Concrete";
            NewBrush.Terrain_Inner = Terrain_Orange;
            NewBrush.Terrain_Outer = Terrain_Concrete;
            NewBrush.Tiles_Straight.Tile_Add(71, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(76, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(75, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(NewBrush);
            //gray to green transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Gray->Green";
            NewBrush.Terrain_Inner = Terrain_Gray;
            NewBrush.Terrain_Outer = Terrain_Green;
            NewBrush.Tiles_Straight.Tile_Add(77, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(58, TileOrientation.TileDirection_BottomLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(79, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Urban.TransitionBrush_Add(NewBrush);

            //road
            Road Road_Road = new Road();
            Road_Road.Name = "Road";
            modProgram.Painter_Urban.Road_Add(Road_Road);
            //road green
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Green;
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileOrientation.TileDirection_None, 1);
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileOrientation.TileDirection_Left, 1);
            NewRoadBrush.Tile_End.Tile_Add(45, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(NewRoadBrush);
            //road blue
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Blue;
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileOrientation.TileDirection_None, 1);
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileOrientation.TileDirection_Left, 1);
            NewRoadBrush.Tile_End.Tile_Add(41, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(NewRoadBrush);
            //road gray
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Gray;
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileOrientation.TileDirection_None, 1);
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileOrientation.TileDirection_Left, 1);
            NewRoadBrush.Tile_End.Tile_Add(43, TileOrientation.TileDirection_Left, 1);
            NewRoadBrush.Tile_End.Tile_Add(44, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(NewRoadBrush);
            //road orange
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Orange;
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileOrientation.TileDirection_None, 1);
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(NewRoadBrush);
            //road concrete
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Concrete;
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileOrientation.TileDirection_None, 1);
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileOrientation.TileDirection_Bottom, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Urban.RoadBrush_Add(NewRoadBrush);

            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layers = new modProgram.sLayerList.clsLayer[0];
            modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount = 0;

            modProgram.sLayerList.clsLayer NewLayer = default(modProgram.sLayerList.clsLayer);

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Gray;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Water;
            NewLayer.HeightMax = -1.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.Terrain = Terrain_Blue;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 3.0F;
            NewLayer.Density = 0.3F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[2] = true;
            NewLayer.Terrain = Terrain_Orange;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 2.5F;
            NewLayer.Density = 0.4F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[2] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.Terrain = Terrain_Concrete;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.6F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[2] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.AvoidLayers[4] = true;
            NewLayer.Terrain = Terrain_Green;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 2.5F;
            NewLayer.Density = 0.6F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 2;
            NewLayer.Terrain = Terrain_Orange;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.5F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 3;
            NewLayer.Terrain = Terrain_Blue;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.5F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetUrban.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 3;
            NewLayer.AvoidLayers[7] = true;
            NewLayer.Terrain = Terrain_Green;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.5F;
        }

        public static void CreatePainterRockies()
        {
            CliffBrush NewBrushCliff = default(CliffBrush);
            TransitionBrush NewBrush = default(TransitionBrush);
            RoadBrush NewRoadBrush = default(RoadBrush);

            modProgram.Painter_Rockies = new Painter();

            Terrain Terrain_Grass = new Terrain();
            Terrain_Grass.Name = "Grass";
            modProgram.Painter_Rockies.Terrain_Add(Terrain_Grass);

            Terrain Terrain_Gravel = new Terrain();
            Terrain_Gravel.Name = "Gravel";
            modProgram.Painter_Rockies.Terrain_Add(Terrain_Gravel);

            Terrain Terrain_Dirt = new Terrain();
            Terrain_Dirt.Name = "Dirt";
            modProgram.Painter_Rockies.Terrain_Add(Terrain_Dirt);

            Terrain Terrain_GrassSnow = new Terrain();
            Terrain_GrassSnow.Name = "Grass Snow";
            modProgram.Painter_Rockies.Terrain_Add(Terrain_GrassSnow);

            Terrain Terrain_GravelSnow = new Terrain();
            Terrain_GravelSnow.Name = "Gravel Snow";
            modProgram.Painter_Rockies.Terrain_Add(Terrain_GravelSnow);

            Terrain Terrain_Snow = new Terrain();
            Terrain_Snow.Name = "Snow";
            modProgram.Painter_Rockies.Terrain_Add(Terrain_Snow);

            Terrain Terrain_Concrete = new Terrain();
            Terrain_Concrete.Name = "Concrete";
            modProgram.Painter_Rockies.Terrain_Add(Terrain_Concrete);

            Terrain Terrain_Water = new Terrain();
            Terrain_Water.Name = "Water";
            modProgram.Painter_Rockies.Terrain_Add(Terrain_Water);

            //grass centre brush
            Terrain_Grass.Tiles.Tile_Add(0, TileOrientation.TileDirection_None, 1);
            //gravel centre brush
            Terrain_Gravel.Tiles.Tile_Add(5, TileOrientation.TileDirection_None, 1);
            Terrain_Gravel.Tiles.Tile_Add(6, TileOrientation.TileDirection_None, 1);
            Terrain_Gravel.Tiles.Tile_Add(7, TileOrientation.TileDirection_None, 1);
            //dirt centre brush
            Terrain_Dirt.Tiles.Tile_Add(53, TileOrientation.TileDirection_None, 1);
            //grass snow centre brush
            Terrain_GrassSnow.Tiles.Tile_Add(23, TileOrientation.TileDirection_None, 1);
            //gravel snow centre brush
            Terrain_GravelSnow.Tiles.Tile_Add(41, TileOrientation.TileDirection_None, 1);
            //snow centre brush
            Terrain_Snow.Tiles.Tile_Add(64, TileOrientation.TileDirection_None, 1);
            //concrete centre brush
            Terrain_Concrete.Tiles.Tile_Add(22, TileOrientation.TileDirection_None, 1);
            //water centre brush
            Terrain_Water.Tiles.Tile_Add(17, TileOrientation.TileDirection_None, 1);
            //gravel to gravel cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Gravel Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Gravel;
            NewBrushCliff.Terrain_Outer = Terrain_Gravel;
            NewBrushCliff.Tiles_Straight.Tile_Add(46, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Straight.Tile_Add(71, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.Tile_Add(45, TileOrientation.TileDirection_TopRight, 1);
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(45, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(NewBrushCliff);
            //gravel snow to gravel cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Gravel Snow -> Gravel Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_GravelSnow;
            NewBrushCliff.Terrain_Outer = Terrain_Gravel;
            NewBrushCliff.Tiles_Straight.Tile_Add(29, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.Tile_Add(9, TileOrientation.TileDirection_TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(42, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(NewBrushCliff);
            //snow to gravel cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Snow -> Gravel Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Snow;
            NewBrushCliff.Terrain_Outer = Terrain_Gravel;
            NewBrushCliff.Tiles_Straight.Tile_Add(68, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileOrientation.TileDirection_TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(42, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(NewBrushCliff);
            //gravel snow cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Gravel Snow Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_GravelSnow;
            NewBrushCliff.Terrain_Outer = Terrain_GravelSnow;
            NewBrushCliff.Tiles_Straight.Tile_Add(44, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.Tile_Add(9, TileOrientation.TileDirection_TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(9, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(NewBrushCliff);
            //snow to gravel snow cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Snow -> Gravel Snow Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Snow;
            NewBrushCliff.Terrain_Outer = Terrain_GravelSnow;
            NewBrushCliff.Tiles_Straight.Tile_Add(78, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileOrientation.TileDirection_TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(9, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(NewBrushCliff);
            //snow to snow cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Snow -> Snow Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Snow;
            NewBrushCliff.Terrain_Outer = Terrain_Snow;
            NewBrushCliff.Tiles_Straight.Tile_Add(78, TileOrientation.TileDirection_Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileOrientation.TileDirection_TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(63, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.CliffBrush_Add(NewBrushCliff);
            //water to grass transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water -> Grass";
            NewBrush.Terrain_Inner = Terrain_Water;
            NewBrush.Terrain_Outer = Terrain_Grass;
            NewBrush.Tiles_Straight.Tile_Add(14, TileOrientation.TileDirection_Bottom, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(16, TileOrientation.TileDirection_BottomLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(15, TileOrientation.TileDirection_BottomLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(NewBrush);
            //water to gravel transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water -> Gravel";
            NewBrush.Terrain_Inner = Terrain_Water;
            NewBrush.Terrain_Outer = Terrain_Gravel;
            NewBrush.Tiles_Straight.Tile_Add(31, TileOrientation.TileDirection_Top, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(32, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(33, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(NewBrush);
            //grass to gravel transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Grass -> Gravel";
            NewBrush.Terrain_Inner = Terrain_Grass;
            NewBrush.Terrain_Outer = Terrain_Gravel;
            NewBrush.Tiles_Straight.Tile_Add(2, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(3, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(4, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(NewBrush);
            //grass to grass snow transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Grass -> Grass Snow";
            NewBrush.Terrain_Inner = Terrain_Grass;
            NewBrush.Terrain_Outer = Terrain_GrassSnow;
            NewBrush.Tiles_Straight.Tile_Add(26, TileOrientation.TileDirection_Top, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(25, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(24, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(NewBrush);
            //grass to dirt transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Grass -> Dirt";
            NewBrush.Terrain_Inner = Terrain_Grass;
            NewBrush.Terrain_Outer = Terrain_Dirt;
            NewBrush.Tiles_Straight.Tile_Add(34, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(35, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(36, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(NewBrush);
            //gravel snow to gravel transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Gravel Snow -> Gravel";
            NewBrush.Terrain_Inner = Terrain_GravelSnow;
            NewBrush.Terrain_Outer = Terrain_Gravel;
            NewBrush.Tiles_Straight.Tile_Add(12, TileOrientation.TileDirection_Bottom, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(10, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(11, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(NewBrush);
            //snow to gravel snow transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Snow -> Gravel Snow";
            NewBrush.Terrain_Inner = Terrain_Snow;
            NewBrush.Terrain_Outer = Terrain_GravelSnow;
            NewBrush.Tiles_Straight.Tile_Add(67, TileOrientation.TileDirection_Bottom, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(65, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(66, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(NewBrush);
            //concrete to dirt transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Concrete -> Dirt";
            NewBrush.Terrain_Inner = Terrain_Concrete;
            NewBrush.Terrain_Outer = Terrain_Dirt;
            NewBrush.Tiles_Straight.Tile_Add(21, TileOrientation.TileDirection_Right, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(19, TileOrientation.TileDirection_BottomRight, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(20, TileOrientation.TileDirection_BottomRight, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(NewBrush);
            //gravel to dirt transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Gravel -> Dirt";
            NewBrush.Terrain_Inner = Terrain_Gravel;
            NewBrush.Terrain_Outer = Terrain_Dirt;
            NewBrush.Tiles_Straight.Tile_Add(38, TileOrientation.TileDirection_Left, 1);
            NewBrush.Tiles_Corner_In.Tile_Add(40, TileOrientation.TileDirection_TopLeft, 1);
            NewBrush.Tiles_Corner_Out.Tile_Add(39, TileOrientation.TileDirection_TopLeft, 1);
            modProgram.Painter_Rockies.TransitionBrush_Add(NewBrush);
            //road
            Road Road_Road = new Road();
            Road_Road.Name = "Road";
            modProgram.Painter_Rockies.Road_Add(Road_Road);
            //road brown
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Dirt;
            NewRoadBrush.Tile_TIntersection.Tile_Add(13, TileOrientation.TileDirection_Bottom, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(59, TileOrientation.TileDirection_Left, 1);
            NewRoadBrush.Tile_End.Tile_Add(60, TileOrientation.TileDirection_Left, 1);
            modProgram.Painter_Rockies.RoadBrush_Add(NewRoadBrush);
            //track
            Road Road_Track = new Road();
            Road_Track.Name = "Track";
            modProgram.Painter_Rockies.Road_Add(Road_Track);
            //track brown
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Track;
            NewRoadBrush.Terrain = Terrain_Dirt;
            NewRoadBrush.Tile_TIntersection.Tile_Add(72, TileOrientation.TileDirection_Right, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(49, TileOrientation.TileDirection_Top, 1);
            NewRoadBrush.Tile_Straight.Tile_Add(51, TileOrientation.TileDirection_Top, 2);
            NewRoadBrush.Tile_Corner_In.Tile_Add(50, TileOrientation.TileDirection_BottomRight, 1);
            NewRoadBrush.Tile_End.Tile_Add(52, TileOrientation.TileDirection_Bottom, 1);
            modProgram.Painter_Rockies.RoadBrush_Add(NewRoadBrush);

            modGenerator.Generator_TilesetRockies.OldTextureLayers.Layers = new modProgram.sLayerList.clsLayer[0];
            modGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount = 0;

            modProgram.sLayerList.clsLayer NewLayer = default(modProgram.sLayerList.clsLayer);

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetRockies.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Gravel;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetRockies.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Water;
            NewLayer.HeightMax = -1.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetRockies.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.Terrain = Terrain_Grass;
            NewLayer.HeightMax = 60.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetRockies.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.Terrain = Terrain_GravelSnow;
            NewLayer.HeightMin = 150.0F;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetRockies.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 3;
            NewLayer.AvoidLayers[1] = true;
            NewLayer.Terrain = Terrain_Snow;
            NewLayer.HeightMin = 200.0F;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetRockies.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 3;
            NewLayer.AvoidLayers[4] = true;
            NewLayer.Terrain = Terrain_Snow;
            NewLayer.HeightMin = 150.0F;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.45F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetRockies.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[2] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.Terrain = Terrain_GravelSnow;
            NewLayer.HeightMin = 0.0F;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.45F;

            NewLayer = new modProgram.sLayerList.clsLayer();
            modGenerator.Generator_TilesetRockies.OldTextureLayers.Layer_Insert(modGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.WithinLayer = 2;
            NewLayer.Terrain = Terrain_Dirt;
            NewLayer.HeightMin = 0.0F;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 1.0F;
            NewLayer.Density = 0.3F;
        }
    }
}