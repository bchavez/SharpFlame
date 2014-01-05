namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Runtime.InteropServices;

    public class clsGeneratorTileset
    {
        public int BorderTextureNum = -1;
        public int ClusteredUnitChanceTotal;
        public int ClusteredUnitCount;
        public sUnitChance[] ClusteredUnits = new sUnitChance[0];
        public modProgram.sLayerList OldTextureLayers;
        public int ScatteredUnitChanceTotal;
        public int ScatteredUnitCount;
        public sUnitChance[] ScatteredUnits = new sUnitChance[0];
        public clsTileset Tileset;

        public void ClusteredUnit_Add(sUnitChance NewUnit)
        {
            if (NewUnit.Type != null)
            {
                this.ClusteredUnitChanceTotal += (int) NewUnit.Chance;
                this.ClusteredUnits = (sUnitChance[]) Utils.CopyArray((Array) this.ClusteredUnits, new sUnitChance[this.ClusteredUnitCount + 1]);
                this.ClusteredUnits[this.ClusteredUnitCount] = NewUnit;
                this.ClusteredUnitCount++;
            }
        }

        public void ScatteredUnit_Add(sUnitChance NewUnit)
        {
            if (NewUnit.Type != null)
            {
                this.ScatteredUnitChanceTotal += (int) NewUnit.Chance;
                this.ScatteredUnits = (sUnitChance[]) Utils.CopyArray((Array) this.ScatteredUnits, new sUnitChance[this.ScatteredUnitCount + 1]);
                this.ScatteredUnits[this.ScatteredUnitCount] = NewUnit;
                this.ScatteredUnitCount++;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sUnitChance
        {
            public clsUnitType Type;
            public uint Chance;
            public sUnitChance(clsUnitType Type, uint Chance)
            {
                this = new clsGeneratorTileset.sUnitChance();
                this.Type = Type;
                this.Chance = Chance;
            }
        }
    }
}

