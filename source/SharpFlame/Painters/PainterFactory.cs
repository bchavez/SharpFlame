#region

using SharpFlame.Generators;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Painters
{
    internal sealed class PainterFactory
    {
        public static void CreatePainterArizona()
        {
            CliffBrush NewBrushCliff;
            TransitionBrush NewBrush;
            RoadBrush NewRoadBrush;

            App.Painter_Arizona = new Painter();

            //arizona

            var Terrain_Red = new Terrain();
            Terrain_Red.Name = "Red";
            App.Painter_Arizona.AddTerrain(Terrain_Red);

            var Terrain_Yellow = new Terrain();
            Terrain_Yellow.Name = "Yellow";
            App.Painter_Arizona.AddTerrain(Terrain_Yellow);

            var Terrain_Sand = new Terrain();
            Terrain_Sand.Name = "Sand";
            App.Painter_Arizona.AddTerrain(Terrain_Sand);

            var Terrain_Brown = new Terrain();
            Terrain_Brown.Name = "Brown";
            App.Painter_Arizona.AddTerrain(Terrain_Brown);

            var Terrain_Green = new Terrain();
            Terrain_Green.Name = "Green";
            App.Painter_Arizona.AddTerrain(Terrain_Green);

            var Terrain_Concrete = new Terrain();
            Terrain_Concrete.Name = "Concrete";
            App.Painter_Arizona.AddTerrain(Terrain_Concrete);

            var Terrain_Water = new Terrain();
            Terrain_Water.Name = "Water";
            App.Painter_Arizona.AddTerrain(Terrain_Water);

            //red centre brush
            Terrain_Red.Tiles.TileAdd(48, TileUtil.None, 1);
            Terrain_Red.Tiles.TileAdd(53, TileUtil.None, 1);
            Terrain_Red.Tiles.TileAdd(54, TileUtil.None, 1);
            Terrain_Red.Tiles.TileAdd(76, TileUtil.None, 1);
            //yellow centre brushTerrain_yellow
            Terrain_Yellow.Tiles.TileAdd(9, TileUtil.None, 1);
            Terrain_Yellow.Tiles.TileAdd(11, TileUtil.None, 1);
            //sand centre brush
            Terrain_Sand.Tiles.TileAdd(12, TileUtil.None, 1);
            //brown centre brush
            Terrain_Brown.Tiles.TileAdd(5, TileUtil.None, 1);
            Terrain_Brown.Tiles.TileAdd(6, TileUtil.None, 1);
            Terrain_Brown.Tiles.TileAdd(7, TileUtil.None, 1);
            Terrain_Brown.Tiles.TileAdd(8, TileUtil.None, 1);
            //green centre brush
            Terrain_Green.Tiles.TileAdd(23, TileUtil.None, 1);
            //concrete centre brush
            Terrain_Concrete.Tiles.TileAdd(22, TileUtil.None, 1);
            //water centre brush
            Terrain_Water.Tiles.TileAdd(17, TileUtil.None, 1);
            //red cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Red Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Red;
            NewBrushCliff.Terrain_Outer = Terrain_Red;
            NewBrushCliff.Tiles_Straight.TileAdd(46, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Straight.TileAdd(71, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.TileAdd(45, TileUtil.TopRight, 1);
            NewBrushCliff.Tiles_Corner_In.TileAdd(75, TileUtil.TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.TileAdd(45, TileUtil.BottomLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.TileAdd(75, TileUtil.BottomRight, 1);
            App.Painter_Arizona.AddBrush(NewBrushCliff);
            //water to sand transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water->Sand";
            NewBrush.TerrainInner = Terrain_Water;
            NewBrush.TerrainOuter = Terrain_Sand;
            NewBrush.TilesStraight.TileAdd(14, TileUtil.Bottom, 1);
            NewBrush.TilesCornerIn.TileAdd(16, TileUtil.BottomLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(15, TileUtil.BottomLeft, 1);
            App.Painter_Arizona.AddBrush(NewBrush);
            //water to green transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water->Green";
            NewBrush.TerrainInner = Terrain_Water;
            NewBrush.TerrainOuter = Terrain_Green;
            NewBrush.TilesStraight.TileAdd(31, TileUtil.Top, 1);
            NewBrush.TilesCornerIn.TileAdd(33, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(32, TileUtil.TopLeft, 1);
            App.Painter_Arizona.AddBrush(NewBrush);
            //yellow to red transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Yellow->Red";
            NewBrush.TerrainInner = Terrain_Yellow;
            NewBrush.TerrainOuter = Terrain_Red;
            NewBrush.TilesStraight.TileAdd(27, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(28, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(29, TileUtil.BottomRight, 1);
            App.Painter_Arizona.AddBrush(NewBrush);
            //sand to red transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Sand->Red";
            NewBrush.TerrainInner = Terrain_Sand;
            NewBrush.TerrainOuter = Terrain_Red;
            NewBrush.TilesStraight.TileAdd(43, TileUtil.Left, 1);
            NewBrush.TilesCornerIn.TileAdd(42, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(41, TileUtil.TopLeft, 1);
            App.Painter_Arizona.AddBrush(NewBrush);
            //sand to yellow transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Sand->Yellow";
            NewBrush.TerrainInner = Terrain_Sand;
            NewBrush.TerrainOuter = Terrain_Yellow;
            NewBrush.TilesStraight.TileAdd(10, TileUtil.Left, 1);
            NewBrush.TilesCornerIn.TileAdd(1, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(0, TileUtil.TopLeft, 1);
            App.Painter_Arizona.AddBrush(NewBrush);
            //brown to red transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Brown->Red";
            NewBrush.TerrainInner = Terrain_Brown;
            NewBrush.TerrainOuter = Terrain_Red;
            NewBrush.TilesStraight.TileAdd(34, TileUtil.Left, 1);
            NewBrush.TilesCornerIn.TileAdd(36, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(35, TileUtil.TopLeft, 1);
            App.Painter_Arizona.AddBrush(NewBrush);
            //brown to yellow transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Brown->Yellow";
            NewBrush.TerrainInner = Terrain_Brown;
            NewBrush.TerrainOuter = Terrain_Yellow;
            NewBrush.TilesStraight.TileAdd(38, TileUtil.Left, 1);
            NewBrush.TilesCornerIn.TileAdd(39, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(40, TileUtil.BottomRight, 1);
            App.Painter_Arizona.AddBrush(NewBrush);
            //brown to sand transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Brown->Sand";
            NewBrush.TerrainInner = Terrain_Brown;
            NewBrush.TerrainOuter = Terrain_Sand;
            NewBrush.TilesStraight.TileAdd(2, TileUtil.Left, 1);
            NewBrush.TilesCornerIn.TileAdd(3, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(4, TileUtil.BottomRight, 1);
            App.Painter_Arizona.AddBrush(NewBrush);
            //brown to green transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Brown->Green";
            NewBrush.TerrainInner = Terrain_Brown;
            NewBrush.TerrainOuter = Terrain_Green;
            NewBrush.TilesStraight.TileAdd(24, TileUtil.Left, 1);
            NewBrush.TilesCornerIn.TileAdd(26, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(25, TileUtil.TopLeft, 1);
            App.Painter_Arizona.AddBrush(NewBrush);
            //concrete to red transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Concrete->Red";
            NewBrush.TerrainInner = Terrain_Concrete;
            NewBrush.TerrainOuter = Terrain_Red;
            NewBrush.TilesStraight.TileAdd(21, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(19, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(20, TileUtil.BottomRight, 1);
            App.Painter_Arizona.AddBrush(NewBrush);


            var Road_Road = new Road();

            Road_Road = new Road();
            Road_Road.Name = "Road";
            App.Painter_Arizona.AddRoad(Road_Road);

            var Road_Track = new Road();

            Road_Track = new Road();
            Road_Track.Name = "Track";
            App.Painter_Arizona.AddRoad(Road_Track);

            //road
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Red;
            NewRoadBrush.Tile_TIntersection.TileAdd(57, TileUtil.Bottom, 1);
            NewRoadBrush.Tile_Straight.TileAdd(59, TileUtil.Left, 1);
            NewRoadBrush.Tile_End.TileAdd(47, TileUtil.Left, 1);
            App.Painter_Arizona.AddBrush(NewRoadBrush);
            //track
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Track;
            NewRoadBrush.Terrain = Terrain_Red;
            NewRoadBrush.Tile_CrossIntersection.TileAdd(73, TileUtil.None, 1);
            NewRoadBrush.Tile_TIntersection.TileAdd(72, TileUtil.Right, 1);
            NewRoadBrush.Tile_Straight.TileAdd(49, TileUtil.Top, 1);
            NewRoadBrush.Tile_Straight.TileAdd(51, TileUtil.Top, 2);
            NewRoadBrush.Tile_Corner_In.TileAdd(50, TileUtil.BottomRight, 1);
            NewRoadBrush.Tile_End.TileAdd(52, TileUtil.Bottom, 1);
            App.Painter_Arizona.AddBrush(NewRoadBrush);

            DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.Layers = new clsLayer[0];
            DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount = 0;

            var NewLayer = default(clsLayer);

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Red;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Sand;
            NewLayer.HeightMax = -1.0F; //signals water distribution
            NewLayer.SlopeMax = -1.0F; //signals water distribution
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 1;
            NewLayer.Terrain = Terrain_Water;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.Terrain = Terrain_Brown;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F; //signals to use cliff angle
            NewLayer.Scale = 3.0F;
            NewLayer.Density = 0.35F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.Terrain = Terrain_Yellow;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F; //signals to use cliff angle
            NewLayer.Scale = 2.0F;
            NewLayer.Density = 0.6F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.WithinLayer = 4;
            NewLayer.Terrain = Terrain_Sand;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 1.0F;
            NewLayer.Density = 0.5F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetArizona.OldTextureLayers.LayerCount, NewLayer);
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
            var NewBrushCliff = default(CliffBrush);
            var NewBrush = default(TransitionBrush);
            var NewRoadBrush = default(RoadBrush);

            //urban

            App.Painter_Urban = new Painter();

            var Terrain_Green = new Terrain();
            Terrain_Green.Name = "Green";
            App.Painter_Urban.AddTerrain(Terrain_Green);

            var Terrain_Blue = new Terrain();
            Terrain_Blue.Name = "Blue";
            App.Painter_Urban.AddTerrain(Terrain_Blue);

            var Terrain_Gray = new Terrain();
            Terrain_Gray.Name = "Gray";
            App.Painter_Urban.AddTerrain(Terrain_Gray);

            var Terrain_Orange = new Terrain();
            Terrain_Orange.Name = "Orange";
            App.Painter_Urban.AddTerrain(Terrain_Orange);

            var Terrain_Concrete = new Terrain();
            Terrain_Concrete.Name = "Concrete";
            App.Painter_Urban.AddTerrain(Terrain_Concrete);

            var Terrain_Water = new Terrain();
            Terrain_Water.Name = "Water";
            App.Painter_Urban.AddTerrain(Terrain_Water);

            //green centre brush
            Terrain_Green.Tiles.TileAdd(50, TileUtil.None, 1);
            //blue centre brush
            Terrain_Blue.Tiles.TileAdd(0, TileUtil.None, 14);
            Terrain_Blue.Tiles.TileAdd(2, TileUtil.None, 1); //line
            //gray centre brush
            Terrain_Gray.Tiles.TileAdd(5, TileUtil.None, 1);
            Terrain_Gray.Tiles.TileAdd(7, TileUtil.None, 4);
            Terrain_Gray.Tiles.TileAdd(8, TileUtil.None, 4);
            Terrain_Gray.Tiles.TileAdd(78, TileUtil.None, 4);
            //orange centre brush
            Terrain_Orange.Tiles.TileAdd(31, TileUtil.None, 1); //pipe
            Terrain_Orange.Tiles.TileAdd(22, TileUtil.None, 50);
            //concrete centre brush
            Terrain_Concrete.Tiles.TileAdd(51, TileUtil.None, 200);
            //water centre brush
            Terrain_Water.Tiles.TileAdd(17, TileUtil.None, 1);
            //cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Gray;
            NewBrushCliff.Terrain_Outer = Terrain_Gray;
            NewBrushCliff.Tiles_Straight.TileAdd(69, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Straight.TileAdd(70, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.TileAdd(68, TileUtil.TopRight, 1);
            NewBrushCliff.Tiles_Corner_Out.TileAdd(68, TileUtil.BottomLeft, 1);
            App.Painter_Urban.AddBrush(NewBrushCliff);
            //water to gray transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water->Gray";
            NewBrush.TerrainInner = Terrain_Water;
            NewBrush.TerrainOuter = Terrain_Gray;
            NewBrush.TilesStraight.TileAdd(23, TileUtil.Left, 1);
            NewBrush.TilesStraight.TileAdd(24, TileUtil.Top, 1);
            NewBrush.TilesCornerIn.TileAdd(25, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(26, TileUtil.TopLeft, 1);
            App.Painter_Urban.AddBrush(NewBrush);
            //water to concrete transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water->Concrete";
            NewBrush.TerrainInner = Terrain_Water;
            NewBrush.TerrainOuter = Terrain_Concrete;
            NewBrush.TilesStraight.TileAdd(13, TileUtil.Left, 1);
            NewBrush.TilesStraight.TileAdd(14, TileUtil.Bottom, 1);
            NewBrush.TilesCornerIn.TileAdd(16, TileUtil.BottomLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(15, TileUtil.BottomLeft, 1);
            App.Painter_Urban.AddBrush(NewBrush);
            //gray to blue transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Gray->Blue";
            NewBrush.TerrainInner = Terrain_Gray;
            NewBrush.TerrainOuter = Terrain_Blue;
            NewBrush.TilesStraight.TileAdd(6, TileUtil.Left, 1);
            NewBrush.TilesCornerIn.TileAdd(4, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(3, TileUtil.BottomRight, 1);
            App.Painter_Urban.AddBrush(NewBrush);
            //concrete to gray transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Concrete->Gray";
            NewBrush.TerrainInner = Terrain_Concrete;
            NewBrush.TerrainOuter = Terrain_Gray;
            NewBrush.TilesStraight.TileAdd(9, TileUtil.Left, 1);
            NewBrush.TilesStraight.TileAdd(27, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(30, TileUtil.BottomLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(10, TileUtil.BottomLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(29, TileUtil.BottomLeft, 1);
            App.Painter_Urban.AddBrush(NewBrush);
            //orange to blue transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Orange->Blue";
            NewBrush.TerrainInner = Terrain_Orange;
            NewBrush.TerrainOuter = Terrain_Blue;
            NewBrush.TilesStraight.TileAdd(33, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(34, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(35, TileUtil.BottomRight, 1);
            App.Painter_Urban.AddBrush(NewBrush);
            //orange to green transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Orange->Green";
            NewBrush.TerrainInner = Terrain_Orange;
            NewBrush.TerrainOuter = Terrain_Green;
            NewBrush.TilesStraight.TileAdd(39, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(38, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(37, TileUtil.TopLeft, 1);
            App.Painter_Urban.AddBrush(NewBrush);
            //orange to gray transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Orange->Gray";
            NewBrush.TerrainInner = Terrain_Orange;
            NewBrush.TerrainOuter = Terrain_Gray;
            NewBrush.TilesStraight.TileAdd(60, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(73, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(72, TileUtil.TopLeft, 1);
            App.Painter_Urban.AddBrush(NewBrush);
            //orange to concrete transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Orange->Concrete";
            NewBrush.TerrainInner = Terrain_Orange;
            NewBrush.TerrainOuter = Terrain_Concrete;
            NewBrush.TilesStraight.TileAdd(71, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(76, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(75, TileUtil.BottomRight, 1);
            App.Painter_Urban.AddBrush(NewBrush);
            //gray to green transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Gray->Green";
            NewBrush.TerrainInner = Terrain_Gray;
            NewBrush.TerrainOuter = Terrain_Green;
            NewBrush.TilesStraight.TileAdd(77, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(58, TileUtil.BottomLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(79, TileUtil.BottomLeft, 1);
            App.Painter_Urban.AddBrush(NewBrush);

            //road
            var Road_Road = new Road();
            Road_Road.Name = "Road";
            App.Painter_Urban.AddRoad(Road_Road);
            //road green
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Green;
            NewRoadBrush.Tile_CrossIntersection.TileAdd(49, TileUtil.None, 1);
            NewRoadBrush.Tile_TIntersection.TileAdd(40, TileUtil.Bottom, 1);
            NewRoadBrush.Tile_Straight.TileAdd(42, TileUtil.Left, 1);
            NewRoadBrush.Tile_End.TileAdd(45, TileUtil.Left, 1);
            App.Painter_Urban.AddBrush(NewRoadBrush);
            //road blue
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Blue;
            NewRoadBrush.Tile_CrossIntersection.TileAdd(49, TileUtil.None, 1);
            NewRoadBrush.Tile_TIntersection.TileAdd(40, TileUtil.Bottom, 1);
            NewRoadBrush.Tile_Straight.TileAdd(42, TileUtil.Left, 1);
            NewRoadBrush.Tile_End.TileAdd(41, TileUtil.Left, 1);
            App.Painter_Urban.AddBrush(NewRoadBrush);
            //road gray
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Gray;
            NewRoadBrush.Tile_CrossIntersection.TileAdd(49, TileUtil.None, 1);
            NewRoadBrush.Tile_TIntersection.TileAdd(40, TileUtil.Bottom, 1);
            NewRoadBrush.Tile_Straight.TileAdd(42, TileUtil.Left, 1);
            NewRoadBrush.Tile_End.TileAdd(43, TileUtil.Left, 1);
            NewRoadBrush.Tile_End.TileAdd(44, TileUtil.Left, 1);
            App.Painter_Urban.AddBrush(NewRoadBrush);
            //road orange
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Orange;
            NewRoadBrush.Tile_CrossIntersection.TileAdd(49, TileUtil.None, 1);
            NewRoadBrush.Tile_TIntersection.TileAdd(40, TileUtil.Bottom, 1);
            NewRoadBrush.Tile_Straight.TileAdd(42, TileUtil.Left, 1);
            App.Painter_Urban.AddBrush(NewRoadBrush);
            //road concrete
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Concrete;
            NewRoadBrush.Tile_CrossIntersection.TileAdd(49, TileUtil.None, 1);
            NewRoadBrush.Tile_TIntersection.TileAdd(40, TileUtil.Bottom, 1);
            NewRoadBrush.Tile_Straight.TileAdd(42, TileUtil.Left, 1);
            App.Painter_Urban.AddBrush(NewRoadBrush);

            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.Layers = new clsLayer[0];
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount = 0;

            var NewLayer = default(clsLayer);

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Gray;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Water;
            NewLayer.HeightMax = -1.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.Terrain = Terrain_Blue;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 3.0F;
            NewLayer.Density = 0.3F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[2] = true;
            NewLayer.Terrain = Terrain_Orange;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 2.5F;
            NewLayer.Density = 0.4F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[2] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.Terrain = Terrain_Concrete;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.6F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[2] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.AvoidLayers[4] = true;
            NewLayer.Terrain = Terrain_Green;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 2.5F;
            NewLayer.Density = 0.6F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 2;
            NewLayer.Terrain = Terrain_Orange;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.5F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 3;
            NewLayer.Terrain = Terrain_Blue;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.5F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetUrban.OldTextureLayers.LayerCount, NewLayer);
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
            var NewBrushCliff = default(CliffBrush);
            var NewBrush = default(TransitionBrush);
            var NewRoadBrush = default(RoadBrush);

            App.Painter_Rockies = new Painter();

            var Terrain_Grass = new Terrain();
            Terrain_Grass.Name = "Grass";
            App.Painter_Rockies.AddTerrain(Terrain_Grass);

            var Terrain_Gravel = new Terrain();
            Terrain_Gravel.Name = "Gravel";
            App.Painter_Rockies.AddTerrain(Terrain_Gravel);

            var Terrain_Dirt = new Terrain();
            Terrain_Dirt.Name = "Dirt";
            App.Painter_Rockies.AddTerrain(Terrain_Dirt);

            var Terrain_GrassSnow = new Terrain();
            Terrain_GrassSnow.Name = "Grass Snow";
            App.Painter_Rockies.AddTerrain(Terrain_GrassSnow);

            var Terrain_GravelSnow = new Terrain();
            Terrain_GravelSnow.Name = "Gravel Snow";
            App.Painter_Rockies.AddTerrain(Terrain_GravelSnow);

            var Terrain_Snow = new Terrain();
            Terrain_Snow.Name = "Snow";
            App.Painter_Rockies.AddTerrain(Terrain_Snow);

            var Terrain_Concrete = new Terrain();
            Terrain_Concrete.Name = "Concrete";
            App.Painter_Rockies.AddTerrain(Terrain_Concrete);

            var Terrain_Water = new Terrain();
            Terrain_Water.Name = "Water";
            App.Painter_Rockies.AddTerrain(Terrain_Water);

            //grass centre brush
            Terrain_Grass.Tiles.TileAdd(0, TileUtil.None, 1);
            //gravel centre brush
            Terrain_Gravel.Tiles.TileAdd(5, TileUtil.None, 1);
            Terrain_Gravel.Tiles.TileAdd(6, TileUtil.None, 1);
            Terrain_Gravel.Tiles.TileAdd(7, TileUtil.None, 1);
            //dirt centre brush
            Terrain_Dirt.Tiles.TileAdd(53, TileUtil.None, 1);
            //grass snow centre brush
            Terrain_GrassSnow.Tiles.TileAdd(23, TileUtil.None, 1);
            //gravel snow centre brush
            Terrain_GravelSnow.Tiles.TileAdd(41, TileUtil.None, 1);
            //snow centre brush
            Terrain_Snow.Tiles.TileAdd(64, TileUtil.None, 1);
            //concrete centre brush
            Terrain_Concrete.Tiles.TileAdd(22, TileUtil.None, 1);
            //water centre brush
            Terrain_Water.Tiles.TileAdd(17, TileUtil.None, 1);
            //gravel to gravel cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Gravel Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Gravel;
            NewBrushCliff.Terrain_Outer = Terrain_Gravel;
            NewBrushCliff.Tiles_Straight.TileAdd(46, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Straight.TileAdd(71, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.TileAdd(45, TileUtil.TopRight, 1);
            NewBrushCliff.Tiles_Corner_Out.TileAdd(45, TileUtil.BottomLeft, 1);
            App.Painter_Rockies.AddBrush(NewBrushCliff);
            //gravel snow to gravel cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Gravel Snow -> Gravel Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_GravelSnow;
            NewBrushCliff.Terrain_Outer = Terrain_Gravel;
            NewBrushCliff.Tiles_Straight.TileAdd(29, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.TileAdd(9, TileUtil.TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.TileAdd(42, TileUtil.BottomLeft, 1);
            App.Painter_Rockies.AddBrush(NewBrushCliff);
            //snow to gravel cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Snow -> Gravel Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Snow;
            NewBrushCliff.Terrain_Outer = Terrain_Gravel;
            NewBrushCliff.Tiles_Straight.TileAdd(68, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.TileAdd(63, TileUtil.TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.TileAdd(42, TileUtil.BottomLeft, 1);
            App.Painter_Rockies.AddBrush(NewBrushCliff);
            //gravel snow cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Gravel Snow Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_GravelSnow;
            NewBrushCliff.Terrain_Outer = Terrain_GravelSnow;
            NewBrushCliff.Tiles_Straight.TileAdd(44, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.TileAdd(9, TileUtil.TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.TileAdd(9, TileUtil.BottomRight, 1);
            App.Painter_Rockies.AddBrush(NewBrushCliff);
            //snow to gravel snow cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Snow -> Gravel Snow Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Snow;
            NewBrushCliff.Terrain_Outer = Terrain_GravelSnow;
            NewBrushCliff.Tiles_Straight.TileAdd(78, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.TileAdd(63, TileUtil.TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.TileAdd(9, TileUtil.BottomRight, 1);
            App.Painter_Rockies.AddBrush(NewBrushCliff);
            //snow to snow cliff brush
            NewBrushCliff = new CliffBrush();
            NewBrushCliff.Name = "Snow -> Snow Cliff";
            NewBrushCliff.Terrain_Inner = Terrain_Snow;
            NewBrushCliff.Terrain_Outer = Terrain_Snow;
            NewBrushCliff.Tiles_Straight.TileAdd(78, TileUtil.Bottom, 1);
            NewBrushCliff.Tiles_Corner_In.TileAdd(63, TileUtil.TopLeft, 1);
            NewBrushCliff.Tiles_Corner_Out.TileAdd(63, TileUtil.BottomRight, 1);
            App.Painter_Rockies.AddBrush(NewBrushCliff);
            //water to grass transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water -> Grass";
            NewBrush.TerrainInner = Terrain_Water;
            NewBrush.TerrainOuter = Terrain_Grass;
            NewBrush.TilesStraight.TileAdd(14, TileUtil.Bottom, 1);
            NewBrush.TilesCornerIn.TileAdd(16, TileUtil.BottomLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(15, TileUtil.BottomLeft, 1);
            App.Painter_Rockies.AddBrush(NewBrush);
            //water to gravel transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Water -> Gravel";
            NewBrush.TerrainInner = Terrain_Water;
            NewBrush.TerrainOuter = Terrain_Gravel;
            NewBrush.TilesStraight.TileAdd(31, TileUtil.Top, 1);
            NewBrush.TilesCornerIn.TileAdd(32, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(33, TileUtil.TopLeft, 1);
            App.Painter_Rockies.AddBrush(NewBrush);
            //grass to gravel transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Grass -> Gravel";
            NewBrush.TerrainInner = Terrain_Grass;
            NewBrush.TerrainOuter = Terrain_Gravel;
            NewBrush.TilesStraight.TileAdd(2, TileUtil.Left, 1);
            NewBrush.TilesCornerIn.TileAdd(3, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(4, TileUtil.TopLeft, 1);
            App.Painter_Rockies.AddBrush(NewBrush);
            //grass to grass snow transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Grass -> Grass Snow";
            NewBrush.TerrainInner = Terrain_Grass;
            NewBrush.TerrainOuter = Terrain_GrassSnow;
            NewBrush.TilesStraight.TileAdd(26, TileUtil.Top, 1);
            NewBrush.TilesCornerIn.TileAdd(25, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(24, TileUtil.TopLeft, 1);
            App.Painter_Rockies.AddBrush(NewBrush);
            //grass to dirt transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Grass -> Dirt";
            NewBrush.TerrainInner = Terrain_Grass;
            NewBrush.TerrainOuter = Terrain_Dirt;
            NewBrush.TilesStraight.TileAdd(34, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(35, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(36, TileUtil.BottomRight, 1);
            App.Painter_Rockies.AddBrush(NewBrush);
            //gravel snow to gravel transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Gravel Snow -> Gravel";
            NewBrush.TerrainInner = Terrain_GravelSnow;
            NewBrush.TerrainOuter = Terrain_Gravel;
            NewBrush.TilesStraight.TileAdd(12, TileUtil.Bottom, 1);
            NewBrush.TilesCornerIn.TileAdd(10, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(11, TileUtil.BottomRight, 1);
            App.Painter_Rockies.AddBrush(NewBrush);
            //snow to gravel snow transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Snow -> Gravel Snow";
            NewBrush.TerrainInner = Terrain_Snow;
            NewBrush.TerrainOuter = Terrain_GravelSnow;
            NewBrush.TilesStraight.TileAdd(67, TileUtil.Bottom, 1);
            NewBrush.TilesCornerIn.TileAdd(65, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(66, TileUtil.BottomRight, 1);
            App.Painter_Rockies.AddBrush(NewBrush);
            //concrete to dirt transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Concrete -> Dirt";
            NewBrush.TerrainInner = Terrain_Concrete;
            NewBrush.TerrainOuter = Terrain_Dirt;
            NewBrush.TilesStraight.TileAdd(21, TileUtil.Right, 1);
            NewBrush.TilesCornerIn.TileAdd(19, TileUtil.BottomRight, 1);
            NewBrush.TilesCornerOut.TileAdd(20, TileUtil.BottomRight, 1);
            App.Painter_Rockies.AddBrush(NewBrush);
            //gravel to dirt transition brush
            NewBrush = new TransitionBrush();
            NewBrush.Name = "Gravel -> Dirt";
            NewBrush.TerrainInner = Terrain_Gravel;
            NewBrush.TerrainOuter = Terrain_Dirt;
            NewBrush.TilesStraight.TileAdd(38, TileUtil.Left, 1);
            NewBrush.TilesCornerIn.TileAdd(40, TileUtil.TopLeft, 1);
            NewBrush.TilesCornerOut.TileAdd(39, TileUtil.TopLeft, 1);
            App.Painter_Rockies.AddBrush(NewBrush);
            //road
            var Road_Road = new Road();
            Road_Road.Name = "Road";
            App.Painter_Rockies.AddRoad(Road_Road);
            //road brown
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Road;
            NewRoadBrush.Terrain = Terrain_Dirt;
            NewRoadBrush.Tile_TIntersection.TileAdd(13, TileUtil.Bottom, 1);
            NewRoadBrush.Tile_Straight.TileAdd(59, TileUtil.Left, 1);
            NewRoadBrush.Tile_End.TileAdd(60, TileUtil.Left, 1);
            App.Painter_Rockies.AddBrush(NewRoadBrush);
            //track
            var Road_Track = new Road();
            Road_Track.Name = "Track";
            App.Painter_Rockies.AddRoad(Road_Track);
            //track brown
            NewRoadBrush = new RoadBrush();
            NewRoadBrush.Road = Road_Track;
            NewRoadBrush.Terrain = Terrain_Dirt;
            NewRoadBrush.Tile_TIntersection.TileAdd(72, TileUtil.Right, 1);
            NewRoadBrush.Tile_Straight.TileAdd(49, TileUtil.Top, 1);
            NewRoadBrush.Tile_Straight.TileAdd(51, TileUtil.Top, 2);
            NewRoadBrush.Tile_Corner_In.TileAdd(50, TileUtil.BottomRight, 1);
            NewRoadBrush.Tile_End.TileAdd(52, TileUtil.Bottom, 1);
            App.Painter_Rockies.AddBrush(NewRoadBrush);

            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.Layers = new clsLayer[0];
            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount = 0;

            var NewLayer = default(clsLayer);

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Gravel;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.Terrain = Terrain_Water;
            NewLayer.HeightMax = -1.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.Terrain = Terrain_Grass;
            NewLayer.HeightMax = 60.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.Terrain = Terrain_GravelSnow;
            NewLayer.HeightMin = 150.0F;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 3;
            NewLayer.AvoidLayers[1] = true;
            NewLayer.Terrain = Terrain_Snow;
            NewLayer.HeightMin = 200.0F;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = (float)MathUtil.RadOf90Deg;
            NewLayer.Scale = 0.0F;
            NewLayer.Density = 1.0F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.WithinLayer = 3;
            NewLayer.AvoidLayers[4] = true;
            NewLayer.Terrain = Terrain_Snow;
            NewLayer.HeightMin = 150.0F;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.45F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
            NewLayer.AvoidLayers[1] = true;
            NewLayer.AvoidLayers[2] = true;
            NewLayer.AvoidLayers[3] = true;
            NewLayer.Terrain = Terrain_GravelSnow;
            NewLayer.HeightMin = 0.0F;
            NewLayer.HeightMax = 255.0F;
            NewLayer.SlopeMax = -1.0F;
            NewLayer.Scale = 1.5F;
            NewLayer.Density = 0.45F;

            NewLayer = new clsLayer();
            DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerInsert(DefaultGenerator.Generator_TilesetRockies.OldTextureLayers.LayerCount, NewLayer);
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