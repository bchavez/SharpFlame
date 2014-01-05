
Module modPainters

    Public Sub CreatePainterArizona()
        Dim NewBrushCliff As clsPainter.clsCliff_Brush
        Dim NewBrush As clsPainter.clsTransition_Brush
        Dim NewRoadBrush As clsPainter.clsRoad_Brush

        Painter_Arizona = New clsPainter

        'arizona

        Dim Terrain_Red As New clsPainter.clsTerrain
        Terrain_Red.Name = "Red"
        Painter_Arizona.Terrain_Add(Terrain_Red)

        Dim Terrain_Yellow As New clsPainter.clsTerrain
        Terrain_Yellow.Name = "Yellow"
        Painter_Arizona.Terrain_Add(Terrain_Yellow)

        Dim Terrain_Sand As New clsPainter.clsTerrain
        Terrain_Sand.Name = "Sand"
        Painter_Arizona.Terrain_Add(Terrain_Sand)

        Dim Terrain_Brown As New clsPainter.clsTerrain
        Terrain_Brown.Name = "Brown"
        Painter_Arizona.Terrain_Add(Terrain_Brown)

        Dim Terrain_Green As New clsPainter.clsTerrain
        Terrain_Green.Name = "Green"
        Painter_Arizona.Terrain_Add(Terrain_Green)

        Dim Terrain_Concrete As New clsPainter.clsTerrain
        Terrain_Concrete.Name = "Concrete"
        Painter_Arizona.Terrain_Add(Terrain_Concrete)

        Dim Terrain_Water As New clsPainter.clsTerrain
        Terrain_Water.Name = "Water"
        Painter_Arizona.Terrain_Add(Terrain_Water)

        'red centre brush
        Terrain_Red.Tiles.Tile_Add(48, TileDirection_None, 1)
        Terrain_Red.Tiles.Tile_Add(53, TileDirection_None, 1)
        Terrain_Red.Tiles.Tile_Add(54, TileDirection_None, 1)
        Terrain_Red.Tiles.Tile_Add(76, TileDirection_None, 1)
        'yellow centre brushTerrain_yellow
        Terrain_Yellow.Tiles.Tile_Add(9, TileDirection_None, 1)
        Terrain_Yellow.Tiles.Tile_Add(11, TileDirection_None, 1)
        'sand centre brush
        Terrain_Sand.Tiles.Tile_Add(12, TileDirection_None, 1)
        'brown centre brush
        Terrain_Brown.Tiles.Tile_Add(5, TileDirection_None, 1)
        Terrain_Brown.Tiles.Tile_Add(6, TileDirection_None, 1)
        Terrain_Brown.Tiles.Tile_Add(7, TileDirection_None, 1)
        Terrain_Brown.Tiles.Tile_Add(8, TileDirection_None, 1)
        'green centre brush
        Terrain_Green.Tiles.Tile_Add(23, TileDirection_None, 1)
        'concrete centre brush
        Terrain_Concrete.Tiles.Tile_Add(22, TileDirection_None, 1)
        'water centre brush
        Terrain_Water.Tiles.Tile_Add(17, TileDirection_None, 1)
        'red cliff brush
        NewBrushCliff = New clsPainter.clsCliff_Brush
        NewBrushCliff.Name = "Red Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Red
        NewBrushCliff.Terrain_Outer = Terrain_Red
        NewBrushCliff.Tiles_Straight.Tile_Add(46, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Straight.Tile_Add(71, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(45, TileDirection_TopRight, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(75, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(45, TileDirection_BottomLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(75, TileDirection_BottomRight, 1)
        Painter_Arizona.CliffBrush_Add(NewBrushCliff)
        'water to sand transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Water->Sand"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Sand
        NewBrush.Tiles_Straight.Tile_Add(14, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(16, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(15, TileDirection_BottomLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'water to green transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Water->Green"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Green
        NewBrush.Tiles_Straight.Tile_Add(31, TileDirection_Top, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(33, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(32, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'yellow to red transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Yellow->Red"
        NewBrush.Terrain_Inner = Terrain_Yellow
        NewBrush.Terrain_Outer = Terrain_Red
        NewBrush.Tiles_Straight.Tile_Add(27, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(28, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(29, TileDirection_BottomRight, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'sand to red transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Sand->Red"
        NewBrush.Terrain_Inner = Terrain_Sand
        NewBrush.Terrain_Outer = Terrain_Red
        NewBrush.Tiles_Straight.Tile_Add(43, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(42, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(41, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'sand to yellow transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Sand->Yellow"
        NewBrush.Terrain_Inner = Terrain_Sand
        NewBrush.Terrain_Outer = Terrain_Yellow
        NewBrush.Tiles_Straight.Tile_Add(10, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(1, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(0, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'brown to red transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Brown->Red"
        NewBrush.Terrain_Inner = Terrain_Brown
        NewBrush.Terrain_Outer = Terrain_Red
        NewBrush.Tiles_Straight.Tile_Add(34, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(36, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(35, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'brown to yellow transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Brown->Yellow"
        NewBrush.Terrain_Inner = Terrain_Brown
        NewBrush.Terrain_Outer = Terrain_Yellow
        NewBrush.Tiles_Straight.Tile_Add(38, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(39, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(40, TileDirection_BottomRight, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'brown to sand transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Brown->Sand"
        NewBrush.Terrain_Inner = Terrain_Brown
        NewBrush.Terrain_Outer = Terrain_Sand
        NewBrush.Tiles_Straight.Tile_Add(2, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(3, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(4, TileDirection_BottomRight, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'brown to green transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Brown->Green"
        NewBrush.Terrain_Inner = Terrain_Brown
        NewBrush.Terrain_Outer = Terrain_Green
        NewBrush.Tiles_Straight.Tile_Add(24, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(26, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(25, TileDirection_TopLeft, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)
        'concrete to red transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Concrete->Red"
        NewBrush.Terrain_Inner = Terrain_Concrete
        NewBrush.Terrain_Outer = Terrain_Red
        NewBrush.Tiles_Straight.Tile_Add(21, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(19, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(20, TileDirection_BottomRight, 1)
        Painter_Arizona.TransitionBrush_Add(NewBrush)


        Dim Road_Road As New clsPainter.clsRoad

        Road_Road = New clsPainter.clsRoad
        Road_Road.Name = "Road"
        Painter_Arizona.Road_Add(Road_Road)

        Dim Road_Track As New clsPainter.clsRoad

        Road_Track = New clsPainter.clsRoad
        Road_Track.Name = "Track"
        Painter_Arizona.Road_Add(Road_Track)

        'road
        NewRoadBrush = New clsPainter.clsRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Red
        NewRoadBrush.Tile_TIntersection.Tile_Add(57, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(59, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(47, TileDirection_Left, 1)
        Painter_Arizona.RoadBrush_Add(NewRoadBrush)
        'track
        NewRoadBrush = New clsPainter.clsRoad_Brush
        NewRoadBrush.Road = Road_Track
        NewRoadBrush.Terrain = Terrain_Red
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(73, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(72, TileDirection_Right, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(49, TileDirection_Top, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(51, TileDirection_Top, 2)
        NewRoadBrush.Tile_Corner_In.Tile_Add(50, TileDirection_BottomRight, 1)
        NewRoadBrush.Tile_End.Tile_Add(52, TileDirection_Bottom, 1)
        Painter_Arizona.RoadBrush_Add(NewRoadBrush)

        With Generator_TilesetArizona
            ReDim .OldTextureLayers.Layers(-1)
            .OldTextureLayers.LayerCount = 0
        End With

        Dim NewLayer As sLayerList.clsLayer

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Red
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Sand
            .HeightMax = -1.0F 'signals water distribution
            .SlopeMax = -1.0F 'signals water distribution
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 1
            .Terrain = Terrain_Water
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .Terrain = Terrain_Brown
            .HeightMax = 255.0F
            .SlopeMax = -1.0F 'signals to use cliff angle
            .Scale = 3.0F
            .Density = 0.35F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(3) = True
            .Terrain = Terrain_Yellow
            .HeightMax = 255.0F
            .SlopeMax = -1.0F 'signals to use cliff angle
            .Scale = 2.0F
            .Density = 0.6F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .WithinLayer = 4
            .Terrain = Terrain_Sand
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.0F
            .Density = 0.5F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetArizona
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .WithinLayer = 3
            .Terrain = Terrain_Green
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 2.0F
            .Density = 0.4F
        End With
    End Sub

    Public Sub CreatePainterUrban()
        Dim NewBrushCliff As clsPainter.clsCliff_Brush
        Dim NewBrush As clsPainter.clsTransition_Brush
        Dim NewRoadBrush As clsPainter.clsRoad_Brush

        'urban

        Painter_Urban = New clsPainter

        Dim Terrain_Green As New clsPainter.clsTerrain
        Terrain_Green.Name = "Green"
        Painter_Urban.Terrain_Add(Terrain_Green)

        Dim Terrain_Blue As New clsPainter.clsTerrain
        Terrain_Blue.Name = "Blue"
        Painter_Urban.Terrain_Add(Terrain_Blue)

        Dim Terrain_Gray As New clsPainter.clsTerrain
        Terrain_Gray.Name = "Gray"
        Painter_Urban.Terrain_Add(Terrain_Gray)

        Dim Terrain_Orange As New clsPainter.clsTerrain
        Terrain_Orange.Name = "Orange"
        Painter_Urban.Terrain_Add(Terrain_Orange)

        Dim Terrain_Concrete As New clsPainter.clsTerrain
        Terrain_Concrete.Name = "Concrete"
        Painter_Urban.Terrain_Add(Terrain_Concrete)

        Dim Terrain_Water As New clsPainter.clsTerrain
        Terrain_Water.Name = "Water"
        Painter_Urban.Terrain_Add(Terrain_Water)

        'green centre brush
        Terrain_Green.Tiles.Tile_Add(50, TileDirection_None, 1)
        'blue centre brush
        Terrain_Blue.Tiles.Tile_Add(0, TileDirection_None, 14)
        Terrain_Blue.Tiles.Tile_Add(2, TileDirection_None, 1) 'line
        'gray centre brush
        Terrain_Gray.Tiles.Tile_Add(5, TileDirection_None, 1)
        Terrain_Gray.Tiles.Tile_Add(7, TileDirection_None, 4)
        Terrain_Gray.Tiles.Tile_Add(8, TileDirection_None, 4)
        Terrain_Gray.Tiles.Tile_Add(78, TileDirection_None, 4)
        'orange centre brush
        Terrain_Orange.Tiles.Tile_Add(31, TileDirection_None, 1) 'pipe
        Terrain_Orange.Tiles.Tile_Add(22, TileDirection_None, 50)
        'concrete centre brush
        Terrain_Concrete.Tiles.Tile_Add(51, TileDirection_None, 200)
        'water centre brush
        Terrain_Water.Tiles.Tile_Add(17, TileDirection_None, 1)
        'cliff brush
        NewBrushCliff = New clsPainter.clsCliff_Brush
        NewBrushCliff.Name = "Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Gray
        NewBrushCliff.Terrain_Outer = Terrain_Gray
        NewBrushCliff.Tiles_Straight.Tile_Add(69, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Straight.Tile_Add(70, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(68, TileDirection_TopRight, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(68, TileDirection_BottomLeft, 1)
        Painter_Urban.CliffBrush_Add(NewBrushCliff)
        'water to gray transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Water->Gray"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Gray
        NewBrush.Tiles_Straight.Tile_Add(23, TileDirection_Left, 1)
        NewBrush.Tiles_Straight.Tile_Add(24, TileDirection_Top, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(25, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(26, TileDirection_TopLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'water to concrete transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Water->Concrete"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Concrete
        NewBrush.Tiles_Straight.Tile_Add(13, TileDirection_Left, 1)
        NewBrush.Tiles_Straight.Tile_Add(14, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(16, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(15, TileDirection_BottomLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'gray to blue transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Gray->Blue"
        NewBrush.Terrain_Inner = Terrain_Gray
        NewBrush.Terrain_Outer = Terrain_Blue
        NewBrush.Tiles_Straight.Tile_Add(6, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(4, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(3, TileDirection_BottomRight, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'concrete to gray transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Concrete->Gray"
        NewBrush.Terrain_Inner = Terrain_Concrete
        NewBrush.Terrain_Outer = Terrain_Gray
        NewBrush.Tiles_Straight.Tile_Add(9, TileDirection_Left, 1)
        NewBrush.Tiles_Straight.Tile_Add(27, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(30, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(10, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(29, TileDirection_BottomLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'orange to blue transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Orange->Blue"
        NewBrush.Terrain_Inner = Terrain_Orange
        NewBrush.Terrain_Outer = Terrain_Blue
        NewBrush.Tiles_Straight.Tile_Add(33, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(34, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(35, TileDirection_BottomRight, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'orange to green transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Orange->Green"
        NewBrush.Terrain_Inner = Terrain_Orange
        NewBrush.Terrain_Outer = Terrain_Green
        NewBrush.Tiles_Straight.Tile_Add(39, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(38, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(37, TileDirection_TopLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'orange to gray transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Orange->Gray"
        NewBrush.Terrain_Inner = Terrain_Orange
        NewBrush.Terrain_Outer = Terrain_Gray
        NewBrush.Tiles_Straight.Tile_Add(60, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(73, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(72, TileDirection_TopLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'orange to concrete transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Orange->Concrete"
        NewBrush.Terrain_Inner = Terrain_Orange
        NewBrush.Terrain_Outer = Terrain_Concrete
        NewBrush.Tiles_Straight.Tile_Add(71, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(76, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(75, TileDirection_BottomRight, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)
        'gray to green transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Gray->Green"
        NewBrush.Terrain_Inner = Terrain_Gray
        NewBrush.Terrain_Outer = Terrain_Green
        NewBrush.Tiles_Straight.Tile_Add(77, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(58, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(79, TileDirection_BottomLeft, 1)
        Painter_Urban.TransitionBrush_Add(NewBrush)

        'road
        Dim Road_Road As New clsPainter.clsRoad
        Road_Road.Name = "Road"
        Painter_Urban.Road_Add(Road_Road)
        'road green
        NewRoadBrush = New clsPainter.clsRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Green
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(45, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)
        'road blue
        NewRoadBrush = New clsPainter.clsRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Blue
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(41, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)
        'road gray
        NewRoadBrush = New clsPainter.clsRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Gray
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(43, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(44, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)
        'road orange
        NewRoadBrush = New clsPainter.clsRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Orange
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)
        'road concrete
        NewRoadBrush = New clsPainter.clsRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Concrete
        NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
        NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
        Painter_Urban.RoadBrush_Add(NewRoadBrush)

        With Generator_TilesetUrban
            ReDim .OldTextureLayers.Layers(-1)
            .OldTextureLayers.LayerCount = 0
        End With

        Dim NewLayer As sLayerList.clsLayer

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Gray
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Water
            .HeightMax = -1.0F
            .SlopeMax = -1.0F
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .Terrain = Terrain_Blue
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 3.0F
            .Density = 0.3F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(2) = True
            .Terrain = Terrain_Orange
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 2.5F
            .Density = 0.4F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(2) = True
            .AvoidLayers(3) = True
            .Terrain = Terrain_Concrete
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 1.5F
            .Density = 0.6F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(2) = True
            .AvoidLayers(3) = True
            .AvoidLayers(4) = True
            .Terrain = Terrain_Green
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 2.5F
            .Density = 0.6F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 2
            .Terrain = Terrain_Orange
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.5F
            .Density = 0.5F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 3
            .Terrain = Terrain_Blue
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.5F
            .Density = 0.5F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetUrban
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 3
            .AvoidLayers(7) = True
            .Terrain = Terrain_Green
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.5F
            .Density = 0.5F
        End With
    End Sub

    Public Sub CreatePainterRockies()
        Dim NewBrushCliff As clsPainter.clsCliff_Brush
        Dim NewBrush As clsPainter.clsTransition_Brush
        Dim NewRoadBrush As clsPainter.clsRoad_Brush

        Painter_Rockies = New clsPainter

        Dim Terrain_Grass As New clsPainter.clsTerrain
        Terrain_Grass.Name = "Grass"
        Painter_Rockies.Terrain_Add(Terrain_Grass)

        Dim Terrain_Gravel As New clsPainter.clsTerrain
        Terrain_Gravel.Name = "Gravel"
        Painter_Rockies.Terrain_Add(Terrain_Gravel)

        Dim Terrain_Dirt As New clsPainter.clsTerrain
        Terrain_Dirt.Name = "Dirt"
        Painter_Rockies.Terrain_Add(Terrain_Dirt)

        Dim Terrain_GrassSnow As New clsPainter.clsTerrain
        Terrain_GrassSnow.Name = "Grass Snow"
        Painter_Rockies.Terrain_Add(Terrain_GrassSnow)

        Dim Terrain_GravelSnow As New clsPainter.clsTerrain
        Terrain_GravelSnow.Name = "Gravel Snow"
        Painter_Rockies.Terrain_Add(Terrain_GravelSnow)

        Dim Terrain_Snow As New clsPainter.clsTerrain
        Terrain_Snow.Name = "Snow"
        Painter_Rockies.Terrain_Add(Terrain_Snow)

        Dim Terrain_Concrete As New clsPainter.clsTerrain
        Terrain_Concrete.Name = "Concrete"
        Painter_Rockies.Terrain_Add(Terrain_Concrete)

        Dim Terrain_Water As New clsPainter.clsTerrain
        Terrain_Water.Name = "Water"
        Painter_Rockies.Terrain_Add(Terrain_Water)

        'grass centre brush
        Terrain_Grass.Tiles.Tile_Add(0, TileDirection_None, 1)
        'gravel centre brush
        Terrain_Gravel.Tiles.Tile_Add(5, TileDirection_None, 1)
        Terrain_Gravel.Tiles.Tile_Add(6, TileDirection_None, 1)
        Terrain_Gravel.Tiles.Tile_Add(7, TileDirection_None, 1)
        'dirt centre brush
        Terrain_Dirt.Tiles.Tile_Add(53, TileDirection_None, 1)
        'grass snow centre brush
        Terrain_GrassSnow.Tiles.Tile_Add(23, TileDirection_None, 1)
        'gravel snow centre brush
        Terrain_GravelSnow.Tiles.Tile_Add(41, TileDirection_None, 1)
        'snow centre brush
        Terrain_Snow.Tiles.Tile_Add(64, TileDirection_None, 1)
        'concrete centre brush
        Terrain_Concrete.Tiles.Tile_Add(22, TileDirection_None, 1)
        'water centre brush
        Terrain_Water.Tiles.Tile_Add(17, TileDirection_None, 1)
        'gravel to gravel cliff brush
        NewBrushCliff = New clsPainter.clsCliff_Brush
        NewBrushCliff.Name = "Gravel Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Gravel
        NewBrushCliff.Terrain_Outer = Terrain_Gravel
        NewBrushCliff.Tiles_Straight.Tile_Add(46, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Straight.Tile_Add(71, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(45, TileDirection_TopRight, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(45, TileDirection_BottomLeft, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'gravel snow to gravel cliff brush
        NewBrushCliff = New clsPainter.clsCliff_Brush
        NewBrushCliff.Name = "Gravel Snow -> Gravel Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_GravelSnow
        NewBrushCliff.Terrain_Outer = Terrain_Gravel
        NewBrushCliff.Tiles_Straight.Tile_Add(29, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(9, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(42, TileDirection_BottomLeft, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'snow to gravel cliff brush
        NewBrushCliff = New clsPainter.clsCliff_Brush
        NewBrushCliff.Name = "Snow -> Gravel Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Snow
        NewBrushCliff.Terrain_Outer = Terrain_Gravel
        NewBrushCliff.Tiles_Straight.Tile_Add(68, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(42, TileDirection_BottomLeft, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'gravel snow cliff brush
        NewBrushCliff = New clsPainter.clsCliff_Brush
        NewBrushCliff.Name = "Gravel Snow Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_GravelSnow
        NewBrushCliff.Terrain_Outer = Terrain_GravelSnow
        NewBrushCliff.Tiles_Straight.Tile_Add(44, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(9, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(9, TileDirection_BottomRight, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'snow to gravel snow cliff brush
        NewBrushCliff = New clsPainter.clsCliff_Brush
        NewBrushCliff.Name = "Snow -> Gravel Snow Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Snow
        NewBrushCliff.Terrain_Outer = Terrain_GravelSnow
        NewBrushCliff.Tiles_Straight.Tile_Add(78, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(9, TileDirection_BottomRight, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'snow to snow cliff brush
        NewBrushCliff = New clsPainter.clsCliff_Brush
        NewBrushCliff.Name = "Snow -> Snow Cliff"
        NewBrushCliff.Terrain_Inner = Terrain_Snow
        NewBrushCliff.Terrain_Outer = Terrain_Snow
        NewBrushCliff.Tiles_Straight.Tile_Add(78, TileDirection_Bottom, 1)
        NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileDirection_TopLeft, 1)
        NewBrushCliff.Tiles_Corner_Out.Tile_Add(63, TileDirection_BottomRight, 1)
        Painter_Rockies.CliffBrush_Add(NewBrushCliff)
        'water to grass transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Water -> Grass"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Grass
        NewBrush.Tiles_Straight.Tile_Add(14, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(16, TileDirection_BottomLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(15, TileDirection_BottomLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'water to gravel transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Water -> Gravel"
        NewBrush.Terrain_Inner = Terrain_Water
        NewBrush.Terrain_Outer = Terrain_Gravel
        NewBrush.Tiles_Straight.Tile_Add(31, TileDirection_Top, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(32, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(33, TileDirection_TopLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'grass to gravel transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Grass -> Gravel"
        NewBrush.Terrain_Inner = Terrain_Grass
        NewBrush.Terrain_Outer = Terrain_Gravel
        NewBrush.Tiles_Straight.Tile_Add(2, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(3, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(4, TileDirection_TopLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'grass to grass snow transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Grass -> Grass Snow"
        NewBrush.Terrain_Inner = Terrain_Grass
        NewBrush.Terrain_Outer = Terrain_GrassSnow
        NewBrush.Tiles_Straight.Tile_Add(26, TileDirection_Top, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(25, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(24, TileDirection_TopLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'grass to dirt transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Grass -> Dirt"
        NewBrush.Terrain_Inner = Terrain_Grass
        NewBrush.Terrain_Outer = Terrain_Dirt
        NewBrush.Tiles_Straight.Tile_Add(34, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(35, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(36, TileDirection_BottomRight, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'gravel snow to gravel transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Gravel Snow -> Gravel"
        NewBrush.Terrain_Inner = Terrain_GravelSnow
        NewBrush.Terrain_Outer = Terrain_Gravel
        NewBrush.Tiles_Straight.Tile_Add(12, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(10, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(11, TileDirection_BottomRight, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'snow to gravel snow transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Snow -> Gravel Snow"
        NewBrush.Terrain_Inner = Terrain_Snow
        NewBrush.Terrain_Outer = Terrain_GravelSnow
        NewBrush.Tiles_Straight.Tile_Add(67, TileDirection_Bottom, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(65, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(66, TileDirection_BottomRight, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'concrete to dirt transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Concrete -> Dirt"
        NewBrush.Terrain_Inner = Terrain_Concrete
        NewBrush.Terrain_Outer = Terrain_Dirt
        NewBrush.Tiles_Straight.Tile_Add(21, TileDirection_Right, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(19, TileDirection_BottomRight, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(20, TileDirection_BottomRight, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'gravel to dirt transition brush
        NewBrush = New clsPainter.clsTransition_Brush
        NewBrush.Name = "Gravel -> Dirt"
        NewBrush.Terrain_Inner = Terrain_Gravel
        NewBrush.Terrain_Outer = Terrain_Dirt
        NewBrush.Tiles_Straight.Tile_Add(38, TileDirection_Left, 1)
        NewBrush.Tiles_Corner_In.Tile_Add(40, TileDirection_TopLeft, 1)
        NewBrush.Tiles_Corner_Out.Tile_Add(39, TileDirection_TopLeft, 1)
        Painter_Rockies.TransitionBrush_Add(NewBrush)
        'road
        Dim Road_Road As New clsPainter.clsRoad
        Road_Road.Name = "Road"
        Painter_Rockies.Road_Add(Road_Road)
        'road brown
        NewRoadBrush = New clsPainter.clsRoad_Brush
        NewRoadBrush.Road = Road_Road
        NewRoadBrush.Terrain = Terrain_Dirt
        NewRoadBrush.Tile_TIntersection.Tile_Add(13, TileDirection_Bottom, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(59, TileDirection_Left, 1)
        NewRoadBrush.Tile_End.Tile_Add(60, TileDirection_Left, 1)
        Painter_Rockies.RoadBrush_Add(NewRoadBrush)
        'track
        Dim Road_Track As New clsPainter.clsRoad
        Road_Track.Name = "Track"
        Painter_Rockies.Road_Add(Road_Track)
        'track brown
        NewRoadBrush = New clsPainter.clsRoad_Brush
        NewRoadBrush.Road = Road_Track
        NewRoadBrush.Terrain = Terrain_Dirt
        NewRoadBrush.Tile_TIntersection.Tile_Add(72, TileDirection_Right, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(49, TileDirection_Top, 1)
        NewRoadBrush.Tile_Straight.Tile_Add(51, TileDirection_Top, 2)
        NewRoadBrush.Tile_Corner_In.Tile_Add(50, TileDirection_BottomRight, 1)
        NewRoadBrush.Tile_End.Tile_Add(52, TileDirection_Bottom, 1)
        Painter_Rockies.RoadBrush_Add(NewRoadBrush)

        With Generator_TilesetRockies
            ReDim .OldTextureLayers.Layers(-1)
            .OldTextureLayers.LayerCount = 0
        End With

        Dim NewLayer As sLayerList.clsLayer

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Gravel
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .Terrain = Terrain_Water
            .HeightMax = -1.0F
            .SlopeMax = -1.0F
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .Terrain = Terrain_Grass
            .HeightMax = 60.0F
            .SlopeMax = -1.0F
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(3) = True
            .Terrain = Terrain_GravelSnow
            .HeightMin = 150.0F
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 3
            .AvoidLayers(1) = True
            .Terrain = Terrain_Snow
            .HeightMin = 200.0F
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 0.0F
            .Density = 1.0F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .WithinLayer = 3
            .AvoidLayers(4) = True
            .Terrain = Terrain_Snow
            .HeightMin = 150.0F
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 1.5F
            .Density = 0.45F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .AvoidLayers(2) = True
            .AvoidLayers(3) = True
            .Terrain = Terrain_GravelSnow
            .HeightMin = 0.0F
            .HeightMax = 255.0F
            .SlopeMax = -1.0F
            .Scale = 1.5F
            .Density = 0.45F
        End With

        NewLayer = New sLayerList.clsLayer
        With Generator_TilesetRockies
            .OldTextureLayers.Layer_Insert(.OldTextureLayers.LayerCount, NewLayer)
        End With
        With NewLayer
            .AvoidLayers(1) = True
            .WithinLayer = 2
            .Terrain = Terrain_Dirt
            .HeightMin = 0.0F
            .HeightMax = 255.0F
            .SlopeMax = RadOf90Deg
            .Scale = 1.0F
            .Density = 0.3F
        End With
    End Sub
End Module
