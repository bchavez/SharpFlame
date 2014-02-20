#region

using System;
using SharpFlame.Domain;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;

#endregion

namespace SharpFlame.Generators
{
    public sealed class DefaultGenerator
    {
        public static clsGeneratorTileset Generator_TilesetArizona;
        public static clsGeneratorTileset Generator_TilesetUrban;
        public static clsGeneratorTileset Generator_TilesetRockies;

        public static UnitTypeBase UnitTypeBaseOilResource;
        public static UnitTypeBase UnitTypeBaseCommandCentre;
        public static UnitTypeBase UnitTypeBaseTruck;
        public static UnitTypeBase UnitTypeBaseFactory;
        public static UnitTypeBase UnitTypeBaseFactoryModule;
        public static UnitTypeBase UnitTypeBaseCyborgFactory;
        public static UnitTypeBase UnitTypeBaseResearchFacility;
        public static UnitTypeBase UnitTypeBaseResearchModule;
        public static UnitTypeBase UnitTypeBasePowerGenerator;
        public static UnitTypeBase UnitTypeBasePowerModule;
        public static UnitTypeBase UnitTypeBaseDerrick;

        public static UnitTypeBase UnitTypeBaseBoulder1;
        public static UnitTypeBase UnitTypeBaseBoulder2;
        public static UnitTypeBase UnitTypeBaseBoulder3;
        public static UnitTypeBase UnitTypeBaseRuin1;
        public static UnitTypeBase UnitTypeBaseRuin3;
        public static UnitTypeBase UnitTypeBaseRuin4;
        public static UnitTypeBase UnitTypeBaseRuin5;

        public static UnitTypeBase UnitTypeBase5Trees;
        public static UnitTypeBase UnitTypeBase4Trees;
        public static UnitTypeBase UnitTypeBase1Tree;
        public static UnitTypeBase UnitTypeBase5TreesSnowy;
        public static UnitTypeBase UnitTypeBase4TreesSnowy;
        public static UnitTypeBase UnitTypeBase1TreeSnowy;

        public static UnitTypeBase UnitTypeBaseHighrise1;
        public static UnitTypeBase UnitTypeBaseHighrise2;
        public static UnitTypeBase UnitTypeBaseHighrise3;
        public static UnitTypeBase UnitTypeBaseHalfHighrise;
        public static UnitTypeBase UnitTypeBaseHighriseStump1;
        public static UnitTypeBase UnitTypeBaseHighriseStump2;
        public static UnitTypeBase UnitTypeBaseBuildingStump;
        public static UnitTypeBase UnitTypeBaseSmallBuilding1;
        public static UnitTypeBase UnitTypeBaseSmallBuilding2;
        public static UnitTypeBase UnitTypeBaseSmallBuilding3;

        public static UnitTypeBase UnitTypeBaseLogCabin1;
        public static UnitTypeBase UnitTypeBaseLogCabin2;
        public static UnitTypeBase UnitTypeBaseLogCabin3;
        public static UnitTypeBase UnitTypeBaseLogCabin4;
        public static UnitTypeBase UnitTypeBaseLogCabin5;

        public static UnitTypeBase UnitTypeBaseCrane;

        public static UnitTypeBase UnitTypeBaseOilDrum;

        public static sGenerateMasterTerrainArgs TerrainStyle_Arizona;
        public static sGenerateMasterTerrainArgs TerrainStyle_Urban;
        public static sGenerateMasterTerrainArgs TerrainStyle_Rockies;

        public static UnitTypeBase GetUnitTypeFromCode(string Code)
        {
            var unitTypeBase = default(UnitTypeBase);

            foreach ( var tempLoopVar_UnitType in App.ObjectData.UnitTypes )
            {
                unitTypeBase = tempLoopVar_UnitType;
                string UnitCode = null;
                if ( unitTypeBase.GetCode(ref UnitCode) )
                {
                    if ( UnitCode == Code )
                    {
                        return unitTypeBase;
                    }
                }
            }
            return null;
        }

        public static void CreateGeneratorTilesets()
        {
            Generator_TilesetArizona = new clsGeneratorTileset();
            Generator_TilesetArizona.Tileset = App.Tileset_Arizona;
            Generator_TilesetUrban = new clsGeneratorTileset();
            Generator_TilesetUrban.Tileset = App.Tileset_Urban;
            Generator_TilesetRockies = new clsGeneratorTileset();
            Generator_TilesetRockies.Tileset = App.Tileset_Rockies;

            UnitTypeBaseOilResource = GetUnitTypeFromCode("OilResource");
            UnitTypeBaseCommandCentre = GetUnitTypeFromCode("A0CommandCentre");
            UnitTypeBaseTruck = GetUnitTypeFromCode("ConstructionDroid");
            UnitTypeBaseFactory = GetUnitTypeFromCode("A0LightFactory");
            UnitTypeBaseFactoryModule = GetUnitTypeFromCode("A0FacMod1");
            UnitTypeBaseCyborgFactory = GetUnitTypeFromCode("A0CyborgFactory");
            UnitTypeBaseResearchFacility = GetUnitTypeFromCode("A0ResearchFacility");
            UnitTypeBaseResearchModule = GetUnitTypeFromCode("A0ResearchModule1");
            UnitTypeBasePowerGenerator = GetUnitTypeFromCode("A0PowerGenerator");
            UnitTypeBasePowerModule = GetUnitTypeFromCode("A0PowMod1");
            UnitTypeBaseDerrick = GetUnitTypeFromCode("A0ResourceExtractor");
            UnitTypeBaseBoulder1 = GetUnitTypeFromCode("Boulder1");
            UnitTypeBaseBoulder2 = GetUnitTypeFromCode("Boulder2");
            UnitTypeBaseBoulder3 = GetUnitTypeFromCode("Boulder3");
            UnitTypeBaseRuin1 = GetUnitTypeFromCode("Ruin1");
            UnitTypeBaseRuin3 = GetUnitTypeFromCode("Ruin3");
            UnitTypeBaseRuin4 = GetUnitTypeFromCode("Ruin4");
            UnitTypeBaseRuin5 = GetUnitTypeFromCode("Ruin5");
            UnitTypeBase5Trees = GetUnitTypeFromCode("Tree1");
            UnitTypeBase4Trees = GetUnitTypeFromCode("Tree2");
            UnitTypeBase1Tree = GetUnitTypeFromCode("Tree3");
            UnitTypeBase5TreesSnowy = GetUnitTypeFromCode("TreeSnow1");
            UnitTypeBase4TreesSnowy = GetUnitTypeFromCode("TreeSnow2");
            UnitTypeBase1TreeSnowy = GetUnitTypeFromCode("TreeSnow3");
            UnitTypeBaseHighrise1 = GetUnitTypeFromCode("building1");
            UnitTypeBaseHighrise2 = GetUnitTypeFromCode("building2");
            UnitTypeBaseHighrise3 = GetUnitTypeFromCode("building3");
            UnitTypeBaseHalfHighrise = GetUnitTypeFromCode("building11");
            UnitTypeBaseHighriseStump1 = GetUnitTypeFromCode("building7");
            UnitTypeBaseHighriseStump2 = GetUnitTypeFromCode("building8");
            UnitTypeBaseBuildingStump = GetUnitTypeFromCode("WreckedBuilding9");
            UnitTypeBaseSmallBuilding1 = GetUnitTypeFromCode("building10");
            UnitTypeBaseSmallBuilding2 = GetUnitTypeFromCode("building12");
            UnitTypeBaseSmallBuilding3 = GetUnitTypeFromCode("WreckedBuilding17");
            UnitTypeBaseLogCabin1 = GetUnitTypeFromCode("LogCabin1");
            UnitTypeBaseLogCabin2 = GetUnitTypeFromCode("LogCabin2");
            UnitTypeBaseLogCabin3 = GetUnitTypeFromCode("LogCabin3");
            UnitTypeBaseLogCabin4 = GetUnitTypeFromCode("LogCabin4");
            UnitTypeBaseLogCabin5 = GetUnitTypeFromCode("LogCabin5");
            UnitTypeBaseCrane = GetUnitTypeFromCode("Crane");
            UnitTypeBaseOilDrum = GetUnitTypeFromCode("OilDrum");

            Generator_TilesetArizona.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseBoulder1, 1));
            Generator_TilesetArizona.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseBoulder2, 1));
            Generator_TilesetArizona.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseBoulder3, 1));

            Generator_TilesetArizona.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseRuin1, 1));
            Generator_TilesetArizona.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseRuin3, 1));
            Generator_TilesetArizona.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseRuin4, 1));
            Generator_TilesetArizona.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseRuin5, 1));

            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseHighrise1, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseHighrise2, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseHighrise3, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseHalfHighrise, 1));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseSmallBuilding1, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseSmallBuilding2, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseSmallBuilding3, 3));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseHighriseStump1, 1));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseHighriseStump2, 1));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseBuildingStump, 1));
            Generator_TilesetUrban.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseCrane, 2));

            Generator_TilesetRockies.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBase5Trees, 1));
            Generator_TilesetRockies.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBase4Trees, 1));
            Generator_TilesetRockies.ScatteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBase1Tree, 2));
            //Tileset_Rockies.ScatteredUnit_Add(New clsWZTileset.sUnitChance(UnitType_5TreesSnowy, 1))
            //Tileset_Rockies.ScatteredUnit_Add(New clsWZTileset.sUnitChance(UnitType_4TreesSnowy, 1))
            //Tileset_Rockies.ScatteredUnit_Add(New clsWZTileset.sUnitChance(UnitType_1TreeSnowy, 2))

            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseLogCabin1, 3));
            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseLogCabin2, 1));
            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseLogCabin3, 1));
            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseLogCabin4, 1));
            Generator_TilesetRockies.ClusteredUnit_Add(new clsGeneratorTileset.sUnitChance(UnitTypeBaseLogCabin5, 3));

            var Num = 0;

            //terrain arizona

            TerrainStyle_Arizona.LayerCount = 5;

            Num = 0;

            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[Num].TileNum = 48; //red
            TerrainStyle_Arizona.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Arizona.Layers[Num].TerrainmapScale = 0.0F;
            TerrainStyle_Arizona.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Arizona.Layers[Num].AvoidLayers = new bool[TerrainStyle_Arizona.LayerCount];
            TerrainStyle_Arizona.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[Num].TileNum = 11; //yellow
            TerrainStyle_Arizona.Layers[Num].TerrainmapDensity = 0.5F;
            TerrainStyle_Arizona.Layers[Num].TerrainmapScale = 2.0F;
            TerrainStyle_Arizona.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Arizona.Layers[Num].AvoidLayers = new bool[TerrainStyle_Arizona.LayerCount];
            TerrainStyle_Arizona.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[Num].TileNum = 5; //brown
            TerrainStyle_Arizona.Layers[Num].TerrainmapDensity = 0.4F;
            TerrainStyle_Arizona.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Arizona.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Arizona.Layers[Num].AvoidLayers = new bool[TerrainStyle_Arizona.LayerCount];
            TerrainStyle_Arizona.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[Num].TileNum = 23; //green
            TerrainStyle_Arizona.Layers[Num].TerrainmapDensity = 0.75F;
            TerrainStyle_Arizona.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Arizona.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Arizona.Layers[Num].AvoidLayers = new bool[TerrainStyle_Arizona.LayerCount];
            TerrainStyle_Arizona.Layers[Num].WithinLayer = Num - 1;

            Num++;
            Array.Resize(ref TerrainStyle_Arizona.Layers, Num + 1);

            TerrainStyle_Arizona.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
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

            TerrainStyle_Urban.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 7;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 0;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 0.5F;
            TerrainStyle_Urban.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 22;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 0.333F;
            TerrainStyle_Urban.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
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

            TerrainStyle_Urban.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[Num].TileNum = 51;
            TerrainStyle_Urban.Layers[Num].TerrainmapDensity = 0.4F;
            TerrainStyle_Urban.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Urban.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Urban.Layers[Num].AvoidLayers = new bool[TerrainStyle_Urban.LayerCount];
            TerrainStyle_Urban.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Urban.Layers, Num + 1);

            TerrainStyle_Urban.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
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

            TerrainStyle_Rockies.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 0; //green
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 0.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 53; //brown
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 0.4F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 23; //green & snow
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 0.333F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 1.5F;
            TerrainStyle_Rockies.Layers[Num].HeightMin = 85.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 64; //snow
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 0.5F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 1.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMin = 85.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = Num - 1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 41; //brown & snow
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 1.0F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 0.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMin = 170.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = -1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[Num].TileNum = 64; //snow
            TerrainStyle_Rockies.Layers[Num].TerrainmapDensity = 0.875F;
            TerrainStyle_Rockies.Layers[Num].TerrainmapScale = 1.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMin = 170.0F;
            TerrainStyle_Rockies.Layers[Num].HeightMax = 256.0F;
            TerrainStyle_Rockies.Layers[Num].AvoidLayers = new bool[TerrainStyle_Rockies.LayerCount];
            TerrainStyle_Rockies.Layers[Num].WithinLayer = Num - 1;

            Num++;
            Array.Resize(ref TerrainStyle_Rockies.Layers, Num + 1);

            TerrainStyle_Rockies.Layers[Num] = new sGenerateMasterTerrainArgs.clsLayer();
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