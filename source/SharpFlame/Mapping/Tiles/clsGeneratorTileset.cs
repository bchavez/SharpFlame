

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;
using SharpFlame.Util;


namespace SharpFlame.Mapping.Tiles
{
    public class clsGeneratorTileset
    {
        public int BorderTextureNum = -1;
        public int ClusteredUnitChanceTotal;
        public int ClusteredUnitCount;
        public sUnitChance[] ClusteredUnits = new sUnitChance[0];

        public sLayerList OldTextureLayers;
        public int ScatteredUnitChanceTotal;
        public int ScatteredUnitCount;
        public sUnitChance[] ScatteredUnits = new sUnitChance[0];
        public Tileset Tileset;

        public void ScatteredUnit_Add(sUnitChance NewUnit)
        {
            if ( NewUnit.TypeBase == null )
            {
                return;
            }

            ScatteredUnitChanceTotal += Convert.ToInt32(NewUnit.Chance);

            Array.Resize(ref ScatteredUnits, ScatteredUnitCount + 1);
            ScatteredUnits[ScatteredUnitCount] = NewUnit;
            ScatteredUnitCount++;
        }

        public void ClusteredUnit_Add(sUnitChance NewUnit)
        {
            if ( NewUnit.TypeBase == null )
            {
                return;
            }

            ClusteredUnitChanceTotal += Convert.ToInt32(NewUnit.Chance);

            Array.Resize(ref ClusteredUnits, ClusteredUnitCount + 1);
            ClusteredUnits[ClusteredUnitCount] = NewUnit;
            ClusteredUnitCount++;
        }

        public struct sUnitChance
        {
            public UInt32 Chance;
            public UnitTypeBase TypeBase;

            public sUnitChance(UnitTypeBase TypeBase, UInt32 Chance)
            {
                this.TypeBase = TypeBase;
                this.Chance = Chance;
            }
        }
    }
}