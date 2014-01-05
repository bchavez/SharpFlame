
Public Module modGenerator

    Public Generator_TilesetArizona As clsGeneratorTileset
    Public Generator_TilesetUrban As clsGeneratorTileset
    Public Generator_TilesetRockies As clsGeneratorTileset

    Public UnitType_OilResource As clsUnitType
    Public UnitType_CommandCentre As clsUnitType
    Public UnitType_Truck As clsUnitType
    Public UnitType_Factory As clsUnitType
    Public UnitType_FactoryModule As clsUnitType
    Public UnitType_CyborgFactory As clsUnitType
    Public UnitType_ResearchFacility As clsUnitType
    Public UnitType_ResearchModule As clsUnitType
    Public UnitType_PowerGenerator As clsUnitType
    Public UnitType_PowerModule As clsUnitType
    Public UnitType_Derrick As clsUnitType

    Public UnitType_Boulder1 As clsUnitType
    Public UnitType_Boulder2 As clsUnitType
    Public UnitType_Boulder3 As clsUnitType
    Public UnitType_Ruin1 As clsUnitType
    Public UnitType_Ruin3 As clsUnitType
    Public UnitType_Ruin4 As clsUnitType
    Public UnitType_Ruin5 As clsUnitType

    Public UnitType_5Trees As clsUnitType
    Public UnitType_4Trees As clsUnitType
    Public UnitType_1Tree As clsUnitType
    Public UnitType_5TreesSnowy As clsUnitType
    Public UnitType_4TreesSnowy As clsUnitType
    Public UnitType_1TreeSnowy As clsUnitType

    Public UnitType_Highrise1 As clsUnitType
    Public UnitType_Highrise2 As clsUnitType
    Public UnitType_Highrise3 As clsUnitType
    Public UnitType_HalfHighrise As clsUnitType
    Public UnitType_HighriseStump1 As clsUnitType
    Public UnitType_HighriseStump2 As clsUnitType
    Public UnitType_BuildingStump As clsUnitType
    Public UnitType_SmallBuilding1 As clsUnitType
    Public UnitType_SmallBuilding2 As clsUnitType
    Public UnitType_SmallBuilding3 As clsUnitType

    Public UnitType_LogCabin1 As clsUnitType
    Public UnitType_LogCabin2 As clsUnitType
    Public UnitType_LogCabin3 As clsUnitType
    Public UnitType_LogCabin4 As clsUnitType
    Public UnitType_LogCabin5 As clsUnitType

    Public UnitType_Crane As clsUnitType

    Public UnitType_OilDrum As clsUnitType

    Public TerrainStyle_Arizona As clsMap.sGenerateMasterTerrainArgs
    Public TerrainStyle_Urban As clsMap.sGenerateMasterTerrainArgs
    Public TerrainStyle_Rockies As clsMap.sGenerateMasterTerrainArgs

    Public Function GetUnitTypeFromCode(Code As String) As clsUnitType
        Dim UnitType As clsUnitType

        For Each UnitType In ObjectData.UnitTypes
            Dim UnitCode As String = Nothing
            If UnitType.GetCode(UnitCode) Then
                If UnitCode = Code Then
                    Return UnitType
                End If
            End If
        Next
        Return Nothing
    End Function

    Public Sub CreateGeneratorTilesets()

        Generator_TilesetArizona = New clsGeneratorTileset
        Generator_TilesetArizona.Tileset = Tileset_Arizona
        Generator_TilesetUrban = New clsGeneratorTileset
        Generator_TilesetUrban.Tileset = Tileset_Urban
        Generator_TilesetRockies = New clsGeneratorTileset
        Generator_TilesetRockies.Tileset = Tileset_Rockies

        UnitType_OilResource = GetUnitTypeFromCode("OilResource")
        UnitType_CommandCentre = GetUnitTypeFromCode("A0CommandCentre")
        UnitType_Truck = GetUnitTypeFromCode("ConstructionDroid")
        UnitType_Factory = GetUnitTypeFromCode("A0LightFactory")
        UnitType_FactoryModule = GetUnitTypeFromCode("A0FacMod1")
        UnitType_CyborgFactory = GetUnitTypeFromCode("A0CyborgFactory")
        UnitType_ResearchFacility = GetUnitTypeFromCode("A0ResearchFacility")
        UnitType_ResearchModule = GetUnitTypeFromCode("A0ResearchModule1")
        UnitType_PowerGenerator = GetUnitTypeFromCode("A0PowerGenerator")
        UnitType_PowerModule = GetUnitTypeFromCode("A0PowMod1")
        UnitType_Derrick = GetUnitTypeFromCode("A0ResourceExtractor")
        UnitType_Boulder1 = GetUnitTypeFromCode("Boulder1")
        UnitType_Boulder2 = GetUnitTypeFromCode("Boulder2")
        UnitType_Boulder3 = GetUnitTypeFromCode("Boulder3")
        UnitType_Ruin1 = GetUnitTypeFromCode("Ruin1")
        UnitType_Ruin3 = GetUnitTypeFromCode("Ruin3")
        UnitType_Ruin4 = GetUnitTypeFromCode("Ruin4")
        UnitType_Ruin5 = GetUnitTypeFromCode("Ruin5")
        UnitType_5Trees = GetUnitTypeFromCode("Tree1")
        UnitType_4Trees = GetUnitTypeFromCode("Tree2")
        UnitType_1Tree = GetUnitTypeFromCode("Tree3")
        UnitType_5TreesSnowy = GetUnitTypeFromCode("TreeSnow1")
        UnitType_4TreesSnowy = GetUnitTypeFromCode("TreeSnow2")
        UnitType_1TreeSnowy = GetUnitTypeFromCode("TreeSnow3")
        UnitType_Highrise1 = GetUnitTypeFromCode("building1")
        UnitType_Highrise2 = GetUnitTypeFromCode("building2")
        UnitType_Highrise3 = GetUnitTypeFromCode("building3")
        UnitType_HalfHighrise = GetUnitTypeFromCode("building11")
        UnitType_HighriseStump1 = GetUnitTypeFromCode("building7")
        UnitType_HighriseStump2 = GetUnitTypeFromCode("building8")
        UnitType_BuildingStump = GetUnitTypeFromCode("WreckedBuilding9")
        UnitType_SmallBuilding1 = GetUnitTypeFromCode("building10")
        UnitType_SmallBuilding2 = GetUnitTypeFromCode("building12")
        UnitType_SmallBuilding3 = GetUnitTypeFromCode("WreckedBuilding17")
        UnitType_LogCabin1 = GetUnitTypeFromCode("LogCabin1")
        UnitType_LogCabin2 = GetUnitTypeFromCode("LogCabin2")
        UnitType_LogCabin3 = GetUnitTypeFromCode("LogCabin3")
        UnitType_LogCabin4 = GetUnitTypeFromCode("LogCabin4")
        UnitType_LogCabin5 = GetUnitTypeFromCode("LogCabin5")
        UnitType_Crane = GetUnitTypeFromCode("Crane")
        UnitType_OilDrum = GetUnitTypeFromCode("OilDrum")

        Generator_TilesetArizona.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Boulder1, 1))
        Generator_TilesetArizona.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Boulder2, 1))
        Generator_TilesetArizona.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Boulder3, 1))

        Generator_TilesetArizona.ClusteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Ruin1, 1))
        Generator_TilesetArizona.ClusteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Ruin3, 1))
        Generator_TilesetArizona.ClusteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Ruin4, 1))
        Generator_TilesetArizona.ClusteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Ruin5, 1))

        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Highrise1, 3))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Highrise2, 3))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Highrise3, 3))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_HalfHighrise, 1))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_SmallBuilding1, 3))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_SmallBuilding2, 3))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_SmallBuilding3, 3))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_HighriseStump1, 1))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_HighriseStump2, 1))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_BuildingStump, 1))
        Generator_TilesetUrban.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_Crane, 2))

        Generator_TilesetRockies.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_5Trees, 1))
        Generator_TilesetRockies.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_4Trees, 1))
        Generator_TilesetRockies.ScatteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_1Tree, 2))
        'Tileset_Rockies.ScatteredUnit_Add(New clsWZTileset.sUnitChance(UnitType_5TreesSnowy, 1))
        'Tileset_Rockies.ScatteredUnit_Add(New clsWZTileset.sUnitChance(UnitType_4TreesSnowy, 1))
        'Tileset_Rockies.ScatteredUnit_Add(New clsWZTileset.sUnitChance(UnitType_1TreeSnowy, 2))

        Generator_TilesetRockies.ClusteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_LogCabin1, 3))
        Generator_TilesetRockies.ClusteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_LogCabin2, 1))
        Generator_TilesetRockies.ClusteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_LogCabin3, 1))
        Generator_TilesetRockies.ClusteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_LogCabin4, 1))
        Generator_TilesetRockies.ClusteredUnit_Add(New clsGeneratorTileset.sUnitChance(UnitType_LogCabin5, 3))

        Dim Num As Integer = 0

        'terrain arizona

        TerrainStyle_Arizona.LayerCount = 5

        Num = 0

        ReDim Preserve TerrainStyle_Arizona.Layers(Num)

        TerrainStyle_Arizona.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Arizona.Layers(Num).TileNum = 48 'red
        TerrainStyle_Arizona.Layers(Num).TerrainmapDensity = 1.0F
        TerrainStyle_Arizona.Layers(Num).TerrainmapScale = 0.0F
        TerrainStyle_Arizona.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Arizona.Layers(Num).AvoidLayers(TerrainStyle_Arizona.LayerCount - 1)
        TerrainStyle_Arizona.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Arizona.Layers(Num)

        TerrainStyle_Arizona.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Arizona.Layers(Num).TileNum = 11 'yellow
        TerrainStyle_Arizona.Layers(Num).TerrainmapDensity = 0.5F
        TerrainStyle_Arizona.Layers(Num).TerrainmapScale = 2.0F
        TerrainStyle_Arizona.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Arizona.Layers(Num).AvoidLayers(TerrainStyle_Arizona.LayerCount - 1)
        TerrainStyle_Arizona.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Arizona.Layers(Num)

        TerrainStyle_Arizona.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Arizona.Layers(Num).TileNum = 5 'brown
        TerrainStyle_Arizona.Layers(Num).TerrainmapDensity = 0.4F
        TerrainStyle_Arizona.Layers(Num).TerrainmapScale = 1.5F
        TerrainStyle_Arizona.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Arizona.Layers(Num).AvoidLayers(TerrainStyle_Arizona.LayerCount - 1)
        TerrainStyle_Arizona.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Arizona.Layers(Num)

        TerrainStyle_Arizona.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Arizona.Layers(Num).TileNum = 23 'green
        TerrainStyle_Arizona.Layers(Num).TerrainmapDensity = 0.75F
        TerrainStyle_Arizona.Layers(Num).TerrainmapScale = 1.5F
        TerrainStyle_Arizona.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Arizona.Layers(Num).AvoidLayers(TerrainStyle_Arizona.LayerCount - 1)
        TerrainStyle_Arizona.Layers(Num).WithinLayer = Num - 1

        Num += 1
        ReDim Preserve TerrainStyle_Arizona.Layers(Num)

        TerrainStyle_Arizona.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Arizona.Layers(Num).TileNum = 18 'cliff
        TerrainStyle_Arizona.Layers(Num).TerrainmapDensity = 1.0F
        TerrainStyle_Arizona.Layers(Num).TerrainmapScale = 0.0F
        TerrainStyle_Arizona.Layers(Num).HeightMax = 256.0F
        TerrainStyle_Arizona.Layers(Num).IsCliff = True
        ReDim TerrainStyle_Arizona.Layers(Num).AvoidLayers(TerrainStyle_Arizona.LayerCount - 1)
        TerrainStyle_Arizona.Layers(Num).WithinLayer = -1

        TerrainStyle_Arizona.Tileset = Generator_TilesetArizona

        Generator_TilesetArizona.BorderTextureNum = 18

        'terrain urban

        TerrainStyle_Urban.LayerCount = 6

        Num = 0

        ReDim Preserve TerrainStyle_Urban.Layers(Num)

        TerrainStyle_Urban.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Urban.Layers(Num).TileNum = 7
        TerrainStyle_Urban.Layers(Num).TerrainmapDensity = 1.0F
        TerrainStyle_Urban.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Urban.Layers(Num).AvoidLayers(TerrainStyle_Urban.LayerCount - 1)
        TerrainStyle_Urban.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Urban.Layers(Num)

        TerrainStyle_Urban.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Urban.Layers(Num).TileNum = 0
        TerrainStyle_Urban.Layers(Num).TerrainmapDensity = 0.5F
        TerrainStyle_Urban.Layers(Num).TerrainmapScale = 1.5F
        TerrainStyle_Urban.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Urban.Layers(Num).AvoidLayers(TerrainStyle_Urban.LayerCount - 1)
        TerrainStyle_Urban.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Urban.Layers(Num)

        TerrainStyle_Urban.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Urban.Layers(Num).TileNum = 22
        TerrainStyle_Urban.Layers(Num).TerrainmapDensity = 0.333F
        TerrainStyle_Urban.Layers(Num).TerrainmapScale = 1.5F
        TerrainStyle_Urban.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Urban.Layers(Num).AvoidLayers(TerrainStyle_Urban.LayerCount - 1)
        TerrainStyle_Urban.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Urban.Layers(Num)

        TerrainStyle_Urban.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Urban.Layers(Num).TileNum = 50
        TerrainStyle_Urban.Layers(Num).TerrainmapDensity = 0.333F
        TerrainStyle_Urban.Layers(Num).TerrainmapScale = 1.5F
        TerrainStyle_Urban.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Urban.Layers(Num).AvoidLayers(TerrainStyle_Urban.LayerCount - 1)
        TerrainStyle_Urban.Layers(Num).WithinLayer = -1

        'Num += 1
        'ReDim Preserve TerrainStyle_Urban.Layers(Num)

        'TerrainStyle_Urban.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        'TerrainStyle_Urban.Layers(Num).TileNum = 19
        'TerrainStyle_Urban.Layers(Num).TerrainmapDensity = 0.25F
        'TerrainStyle_Urban.Layers(Num).TerrainmapScale = 1.5F
        'TerrainStyle_Urban.Layers(Num).HeightMax = 256.0F
        'ReDim TerrainStyle_Urban.Layers(Num).AvoidLayers(TerrainStyle_Urban.LayerCount - 1)
        'TerrainStyle_Urban.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Urban.Layers(Num)

        TerrainStyle_Urban.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Urban.Layers(Num).TileNum = 51
        TerrainStyle_Urban.Layers(Num).TerrainmapDensity = 0.4F
        TerrainStyle_Urban.Layers(Num).TerrainmapScale = 1.5F
        TerrainStyle_Urban.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Urban.Layers(Num).AvoidLayers(TerrainStyle_Urban.LayerCount - 1)
        TerrainStyle_Urban.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Urban.Layers(Num)

        TerrainStyle_Urban.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Urban.Layers(Num).TileNum = 70
        TerrainStyle_Urban.Layers(Num).TerrainmapDensity = 1.0F
        TerrainStyle_Urban.Layers(Num).TerrainmapScale = 0.0F
        TerrainStyle_Urban.Layers(Num).HeightMax = 256.0F
        TerrainStyle_Urban.Layers(Num).IsCliff = True
        ReDim TerrainStyle_Urban.Layers(Num).AvoidLayers(TerrainStyle_Urban.LayerCount - 1)
        TerrainStyle_Urban.Layers(Num).WithinLayer = -1

        TerrainStyle_Urban.Tileset = Generator_TilesetUrban

        Generator_TilesetUrban.BorderTextureNum = 70

        'terrain rockies

        TerrainStyle_Rockies.LayerCount = 7

        Num = 0

        ReDim Preserve TerrainStyle_Rockies.Layers(Num)

        TerrainStyle_Rockies.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Rockies.Layers(Num).TileNum = 0 'green
        TerrainStyle_Rockies.Layers(Num).TerrainmapDensity = 1.0F
        TerrainStyle_Rockies.Layers(Num).TerrainmapScale = 0.0F
        TerrainStyle_Rockies.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Rockies.Layers(Num).AvoidLayers(TerrainStyle_Rockies.LayerCount - 1)
        TerrainStyle_Rockies.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Rockies.Layers(Num)

        TerrainStyle_Rockies.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Rockies.Layers(Num).TileNum = 53 'brown
        TerrainStyle_Rockies.Layers(Num).TerrainmapDensity = 0.4F
        TerrainStyle_Rockies.Layers(Num).TerrainmapScale = 1.5F
        TerrainStyle_Rockies.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Rockies.Layers(Num).AvoidLayers(TerrainStyle_Rockies.LayerCount - 1)
        TerrainStyle_Rockies.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Rockies.Layers(Num)

        TerrainStyle_Rockies.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Rockies.Layers(Num).TileNum = 23 'green & snow
        TerrainStyle_Rockies.Layers(Num).TerrainmapDensity = 0.333F
        TerrainStyle_Rockies.Layers(Num).TerrainmapScale = 1.5F
        TerrainStyle_Rockies.Layers(Num).HeightMin = 85.0F
        TerrainStyle_Rockies.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Rockies.Layers(Num).AvoidLayers(TerrainStyle_Rockies.LayerCount - 1)
        TerrainStyle_Rockies.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Rockies.Layers(Num)

        TerrainStyle_Rockies.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Rockies.Layers(Num).TileNum = 64 'snow
        TerrainStyle_Rockies.Layers(Num).TerrainmapDensity = 0.5F
        TerrainStyle_Rockies.Layers(Num).TerrainmapScale = 1.0F
        TerrainStyle_Rockies.Layers(Num).HeightMin = 85.0F
        TerrainStyle_Rockies.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Rockies.Layers(Num).AvoidLayers(TerrainStyle_Rockies.LayerCount - 1)
        TerrainStyle_Rockies.Layers(Num).WithinLayer = Num - 1

        Num += 1
        ReDim Preserve TerrainStyle_Rockies.Layers(Num)

        TerrainStyle_Rockies.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Rockies.Layers(Num).TileNum = 41 'brown & snow
        TerrainStyle_Rockies.Layers(Num).TerrainmapDensity = 1.0F
        TerrainStyle_Rockies.Layers(Num).TerrainmapScale = 0.0F
        TerrainStyle_Rockies.Layers(Num).HeightMin = 170.0F
        TerrainStyle_Rockies.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Rockies.Layers(Num).AvoidLayers(TerrainStyle_Rockies.LayerCount - 1)
        TerrainStyle_Rockies.Layers(Num).WithinLayer = -1

        Num += 1
        ReDim Preserve TerrainStyle_Rockies.Layers(Num)

        TerrainStyle_Rockies.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Rockies.Layers(Num).TileNum = 64 'snow
        TerrainStyle_Rockies.Layers(Num).TerrainmapDensity = 0.875F
        TerrainStyle_Rockies.Layers(Num).TerrainmapScale = 1.0F
        TerrainStyle_Rockies.Layers(Num).HeightMin = 170.0F
        TerrainStyle_Rockies.Layers(Num).HeightMax = 256.0F
        ReDim TerrainStyle_Rockies.Layers(Num).AvoidLayers(TerrainStyle_Rockies.LayerCount - 1)
        TerrainStyle_Rockies.Layers(Num).WithinLayer = Num - 1

        Num += 1
        ReDim Preserve TerrainStyle_Rockies.Layers(Num)

        TerrainStyle_Rockies.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
        TerrainStyle_Rockies.Layers(Num).TileNum = 30 'cliff
        TerrainStyle_Rockies.Layers(Num).TerrainmapDensity = 1.0F
        TerrainStyle_Rockies.Layers(Num).TerrainmapScale = 0.0F
        TerrainStyle_Rockies.Layers(Num).HeightMax = 256.0F
        TerrainStyle_Rockies.Layers(Num).IsCliff = True
        ReDim TerrainStyle_Rockies.Layers(Num).AvoidLayers(TerrainStyle_Rockies.LayerCount - 1)
        TerrainStyle_Rockies.Layers(Num).WithinLayer = -1

        TerrainStyle_Rockies.Tileset = Generator_TilesetRockies

        Generator_TilesetRockies.BorderTextureNum = 30
    End Sub
End Module
