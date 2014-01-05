using System;

namespace SharpFlame
{
    public class clsGeneratorTileset
    {
        public clsTileset Tileset;

        public struct sUnitChance
        {
            public clsUnitType Type;
            public UInt32 Chance;

            public sUnitChance(clsUnitType Type, UInt32 Chance)
            {
                this.Type = Type;
                this.Chance = Chance;
            }
        }

        public sUnitChance[] ScatteredUnits = new sUnitChance[0];
        public int ScatteredUnitCount;
        public int ScatteredUnitChanceTotal;
        public sUnitChance[] ClusteredUnits = new sUnitChance[0];
        public int ClusteredUnitCount;
        public int ClusteredUnitChanceTotal;

        public int BorderTextureNum = -1;

        public modProgram.sLayerList OldTextureLayers;

        public void ScatteredUnit_Add(sUnitChance NewUnit)
        {
            if ( NewUnit.Type == null )
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
            if ( NewUnit.Type == null )
            {
                return;
            }

            ClusteredUnitChanceTotal += Convert.ToInt32(NewUnit.Chance);

            Array.Resize(ref ClusteredUnits, ClusteredUnitCount + 1);
            ClusteredUnits[ClusteredUnitCount] = NewUnit;
            ClusteredUnitCount++;
        }
    }
}