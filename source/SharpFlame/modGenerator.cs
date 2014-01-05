using System;

namespace SharpFlame
{
    public sealed class modGenerator
    {
        public static clsGeneratorTileset Generator_TilesetArizona;
        public static clsGeneratorTileset Generator_TilesetUrban;
        public static clsGeneratorTileset Generator_TilesetRockies;

        public static clsUnitType UnitType_OilResource;
        public static clsUnitType UnitType_CommandCentre;
        public static clsUnitType UnitType_Truck;
        public static clsUnitType UnitType_Factory;
        public static clsUnitType UnitType_FactoryModule;
        public static clsUnitType UnitType_CyborgFactory;
        public static clsUnitType UnitType_ResearchFacility;
        public static clsUnitType UnitType_ResearchModule;
        public static clsUnitType UnitType_PowerGenerator;
        public static clsUnitType UnitType_PowerModule;
        public static clsUnitType UnitType_Derrick;

        public static clsUnitType UnitType_Boulder1;
        public static clsUnitType UnitType_Boulder2;
        public static clsUnitType UnitType_Boulder3;
        public static clsUnitType UnitType_Ruin1;
        public static clsUnitType UnitType_Ruin3;
        public static clsUnitType UnitType_Ruin4;
        public static clsUnitType UnitType_Ruin5;

        public static clsUnitType UnitType_5Trees;
        public static clsUnitType UnitType_4Trees;
        public static clsUnitType UnitType_1Tree;
        public static clsUnitType UnitType_5TreesSnowy;
        public static clsUnitType UnitType_4TreesSnowy;
        public static clsUnitType UnitType_1TreeSnowy;

        public static clsUnitType UnitType_Highrise1;
        public static clsUnitType UnitType_Highrise2;
        public static clsUnitType UnitType_Highrise3;
        public static clsUnitType UnitType_HalfHighrise;
        public static clsUnitType UnitType_HighriseStump1;
        public static clsUnitType UnitType_HighriseStump2;
        public static clsUnitType UnitType_BuildingStump;
        public static clsUnitType UnitType_SmallBuilding1;
        public static clsUnitType UnitType_SmallBuilding2;
        public static clsUnitType UnitType_SmallBuilding3;

        public static clsUnitType UnitType_LogCabin1;
        public static clsUnitType UnitType_LogCabin2;
        public static clsUnitType UnitType_LogCabin3;
        public static clsUnitType UnitType_LogCabin4;
        public static clsUnitType UnitType_LogCabin5;

        public static clsUnitType UnitType_Crane;

        public static clsUnitType UnitType_OilDrum;

        public static clsMap.sGenerateMasterTerrainArgs TerrainStyle_Arizona;
        public static clsMap.sGenerateMasterTerrainArgs TerrainStyle_Urban;
        public static clsMap.sGenerateMasterTerrainArgs TerrainStyle_Rockies;

        public static clsUnitType GetUnitTypeFromCode(string Code)
        {
            clsUnitType UnitType = default(clsUnitType);

            foreach ( clsUnitType tempLoopVar_UnitType in modProgram.ObjectData.UnitTypes )
            {
                UnitType = tempLoopVar_UnitType;
                string UnitCode = null;
                if ( UnitType.GetCode(ref UnitCode) )
                {
                    if ( UnitCode == Code )
                    {
                        return UnitType;
                    }
                }
            }
            return null;
        }

        public static void CreateGeneratorTilesets()
        {
            Generator_TilesetArizona = new clsGeneratorTileset();
            Generator_TilesetArizona.Tileset = modProgram.Tileset_Arizona;
            Generator_TilesetUrban = new clsGeneratorTileset();
            Generator_TilesetUrban.Tileset = modProgram.Tileset_Urban;
            Generator_TilesetRockies = new clsGeneratorTileset();
            Generator_TilesetRockies.Tileset = modProgram.Tileset_Rockies;

            UnitType_OilResource = GetUnitTypeFromCode("OilResource");
            UnitType_CommandCentre = GetUnitTypeFromCode("A0CommandCentre");
            UnitType_Truck = GetUnitTypeFromCode("ConstructionDroid");
            UnitType_Factory = GetUnitTypeFromCode("A0LightFactory");
            UnitType_FactoryModule = GetUnitTypeFromCode("A0FacMod1");
            UnitType_CyborgFactory = GetUnitTypeFromCode("A0CyborgFactory");
            UnitType_ResearchFacility = GetUnitTypeFromCode("A0ResearchFacility");
            UnitType_ResearchModule = GetUnitTypeFromCode("A0ResearchModule1");
            UnitType_PowerGenerator = GetUnitTypeFromCode("A0PowerGenerator");
            UnitType_PowerModule = GetUnitTypeFromCode("A0PowMod1");
            UnitType_Derrick = GetUnitTypeFromCode("A0ResourceExtractor");
            UnitType_Boulder1 = GetUnitTypeFromCode("Boulder1");
            UnitType_Boulder2 = GetUnitTypeFromCode("Boulder2");
            UnitType_Boulder3 = GetUnitTypeFromCode("Boulder3");
            UnitType_Ruin1 = GetUnitTypeFromCode("Ruin1");
            UnitType_Ruin3 = GetUnitTypeFromCode("Ruin3");
            UnitType_Ruin4 = GetUnitTypeFromCode("Ruin4");
            UnitType_Ruin5 = GetUnitTypeFromCode("Ruin5");
            UnitType_5Trees = GetUnitTypeFromCode("Tree1");
            UnitType_4Trees = GetUnitTypeFromCode("Tree2");
            UnitType_1Tree = GetUnitTypeFromCode("Tree3");
            UnitType_5TreesSnowy = GetUnitTypeFromCode("TreeSnow1");
            UnitType_4TreesSnowy = GetUnitTypeFromCode("TreeSnow2");
            UnitType_1TreeSnowy = GetUnitTypeFromCode("TreeSnow3");
            UnitType_Highrise1 = GetUnitTypeFromCode("building1");
            UnitType_Highrise2 = GetUnitTypeFromCode("building2");
            UnitType_Highrise3 = GetUnitTypeFromCode("building3");
            UnitType_HalfHighrise = GetUnitTypeFromCode("building11");
            UnitType_HighriseStump1 = GetUnitTypeFromCode("building7");
            UnitType_HighriseStump2 = GetUnitTypeFromCode("building8");
            UnitType_BuildingStump = GetUnitTypeFromCode("WreckedBuilding9");
            UnitType_SmallBuilding1 = GetUnitTypeFromCode("building10");
            UnitType_SmallBuilding2 = GetUnitTypeFromCode("building12");
            UnitType_SmallBuilding3 = GetUnitTypeFromCode("WreckedBuilding17");
            UnitType_LogCabin1 = GetUnitTypeFromCode("LogCabin1");
            UnitType_LogCabin2 = GetUnitTypeFromCode("LogCabin2");
            UnitType_LogCabin3 = GetUnitTypeFromCode("LogCabin3");
            UnitType_LogCabin4 = GetUnitTypeFromCode("LogCabin4");
            UnitType_LogCabin5 = GetUnitTypeFromCode("LogCabin5");
            UnitType_Crane = GetUnitTypeFromCode("Crane");
            UnitType_OilDrum = GetUnitTypeFromCode("OilDrum");

            Generator_TilesetArizona.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Boulder1, 1));
            Generator_TilesetArizona.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Boulder2, 1));
            Generator_TilesetArizona.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Boulder3, 1));

            Generator_TilesetArizona.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Ruin1, 1));
            Generator_TilesetArizona.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Ruin3, 1));
            Generator_TilesetArizona.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Ruin4, 1));
            Generator_TilesetArizona.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Ruin5, 1));

            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Highrise1, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Highrise2, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Highrise3, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_HalfHighrise, 1));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_SmallBuilding1, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_SmallBuilding2, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_SmallBuilding3, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_HighriseStump1, 1));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_HighriseStump2, 1));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_BuildingStump, 1));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_Crane, 2));

            Generator_TilesetRockies.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_5Trees, 1));
            Generator_TilesetRockies.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_4Trees, 1));
            Generator_TilesetRockies.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_1Tree, 2));
            //Tileset_Rockies.ScatteredUnit_Add(New clsWZTileset.sUnitChance(UnitType_5TreesSnowy, 1))
            //Tileset_Rockies.ScatteredUnit_Add(New clsWZTileset.sUnitChance(UnitType_4TreesSnowy, 1))
            //Tileset_Rockies.ScatteredUnit_Add(New clsWZTileset.sUnitChance(UnitType_1TreeSnowy, 2))

            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_LogCabin1, 3));
            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_LogCabin2, 1));
            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_LogCabin3, 1));
            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_LogCabin4, 1));
            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitType_LogCabin5, 3));

            int Num = 0;

            //terrain arizona

            TerrainStyle_Arizona.LayerCount = 5;

            Num = 0;

            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[Num].TileNum = 48; //red
            TerrainStyle_Arizona.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Arizona.Layers[Num].TerrainmapScale = 0.0F;
            TerrainStyle_Arizona.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Arizona.Layers[Num].AvoidLayers = new bool[TerrainStyle_Arizona.LayerCount];
            TerrainStyle_Arizona.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[Num].TileNum = 11; //yellow
            TerrainStyle_Arizona.Layers[Num].TerrainmapDensity = 0.5F;
            TerrainStyle_Arizona.Layers[Num].TerrainmapScale = 2.0F;
            TerrainStyle_Arizona.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Arizona.Layers[Num].AvoidLayers = new bool[TerrainStyle_Arizona.LayerCount];
            TerrainStyle_Arizona.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[Num].TileNum = 5; //brown
            TerrainStyle_Arizona.Layers[Num].TerrainmapDensity = 0.4F;
            TerrainStyle_Arizona.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Arizona.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Arizona.Layers[Num].AvoidLayers = new bool[TerrainStyle_Arizona.LayerCount];
            TerrainStyle_Arizona.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[Num].TileNum = 23; //green
            TerrainStyle_Arizona.Layers[Num].TerrainmapDensity = 0.75F;
            TerrainStyle_Arizona.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Arizona.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Arizona.Layers[Num].AvoidLayers = new bool[TerrainStyle_Arizona.LayerCount];
            TerrainStyle_Arizona.Layers[Num].WithinLayer = Num - 1;

            Num++;
            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[Num].TileNum = 18; //cliff
            TerrainStyle_Arizona.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Arizona.Layers[Num].TerrainmapScale = 0.0F;
            TerrainStyle_Arizona.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Arizona.Layers[Num].IsCliff = true;
            TerrainStyle_Arizona.Layers[Num].AvoidLayers = new bool[TerrainStyle_Arizona.LayerCount];
            TerrainStyle_Arizona.Layers[Num].WithinLayer = -1;

            TerrainStyle_Arizona.Tileset = Generator_TilesetArizona;

            Generator_TilesetArizona.BorderTextureNum = 18;

            //terrain urban

            TerrainStyle_Urban.LayerCount = 6;

            Num = 0;

            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 7;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 0;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 0.5F;
            TerrainStyle_Urban.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 22;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 0.333F;
            TerrainStyle_Urban.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 50;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 0.333F;
            TerrainStyle_Urban.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            //Num += 1
            //ReDim Preserve TerrainStyle_Urban.Layers(Num)

            //TerrainStyle_Urban.Layers(Num) = New clsMap.sGenerateMasterTerrainArgs.clsLayer
            //TerrainStyle_Urban.Layers(Num).TileNum = 19
            //TerrainStyle_Urban.Layers(Num).TerrainmapDensity = 0.25F
            //TerrainStyle_Urban.Layers(Num).TerrainmapScale = 1.5F
            //TerrainStyle_Urban.Layers(Num).HeightMax = 256.0F
            //ReDim TerrainStyle_Urban.Layers(Num).AvoidLayers(TerrainStyle_Urban.LayerCount - 1)
            //TerrainStyle_Urban.Layers(Num).WithinLayer = -1

            Num++;
            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 51;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 0.4F;
            TerrainStyle_Urban.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 70;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Urban.Layers[Num].TerrainmapScale = 0.0F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].IsCliff = true;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            TerrainStyle_Urban.Tileset = Generator_TilesetUrban;

            Generator_TilesetUrban.BorderTextureNum = 70;

            //terrain rockies

            TerrainStyle_Rockies.LayerCount = 7;

            Num = 0;

            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 0; //green
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 0.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 53; //brown
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 0.4F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 23; //green & snow
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 0.333F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Rockies.Layers[Num].HeightMin = 85.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 64; //snow
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 0.5F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 1.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMin = 85.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = Num - 1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 41; //brown & snow
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 0.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMin = 170.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 64; //snow
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 0.875F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 1.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMin = 170.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = Num - 1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 30; //cliff
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 0.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].IsCliff = true;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = -1;

            TerrainStyle_Rockies.Tileset = Generator_TilesetRockies;

            Generator_TilesetRockies.BorderTextureNum = 30;
        }
    }
}