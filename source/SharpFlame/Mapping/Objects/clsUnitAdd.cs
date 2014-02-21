#region

using System;
using System.Diagnostics;
using System.Windows.Forms;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Mapping.Objects
{
    public class clsUnitAdd
    {
        public UInt32 ID = 0U;
        public string Label = null;
        public clsMap Map;
        public clsUnit NewUnit;
        public bool StoreChange = false;

        public bool Perform()
        {
            if ( Map == null )
            {
                Debugger.Break();
                return false;
            }
            if ( NewUnit == null )
            {
                Debugger.Break();
                return false;
            }

            if ( NewUnit.MapLink.IsConnected )
            {
                MessageBox.Show("Error: Added object already has a map assigned.");
                return false;
            }
            if ( NewUnit.UnitGroup == null )
            {
                MessageBox.Show("Error: Added object has no group.");
                NewUnit.UnitGroup = Map.ScavengerUnitGroup;
                return false;
            }
            if ( NewUnit.UnitGroup.MapLink.Source != Map )
            {
                MessageBox.Show("Error: Something terrible happened.");
                return false;
            }

            if ( StoreChange )
            {
                var UnitChange = new clsUnitChange();
                UnitChange.Type = clsUnitChange.enumType.Added;
                UnitChange.Unit = NewUnit;
                Map.UnitChanges.Add(UnitChange);
            }

            if ( ID <= 0U )
            {
                ID = Map.GetAvailableID();
            }
            else if ( Map.IDUsage(ID) != null )
            {
                ID = Map.GetAvailableID();
            }

            NewUnit.ID = ID;

            NewUnit.MapLink.Connect(Map.Units);

            NewUnit.Pos.Horizontal.X = MathUtil.ClampInt(NewUnit.Pos.Horizontal.X, 0, Map.Terrain.TileSize.X * Constants.TerrainGridSpacing - 1);
            NewUnit.Pos.Horizontal.Y = MathUtil.ClampInt(NewUnit.Pos.Horizontal.Y, 0, Map.Terrain.TileSize.Y * Constants.TerrainGridSpacing - 1);
            NewUnit.Pos.Altitude = (int)(Math.Ceiling(Map.GetTerrainHeight(NewUnit.Pos.Horizontal)));

            if ( Label != null )
            {
                NewUnit.SetLabel(Label);
            }

            Map.UnitSectorsCalc(NewUnit);

            if ( Map.SectorGraphicsChanges != null )
            {
                Map.UnitSectorsGraphicsChanged(NewUnit);
            }

            return true;
        }
    }
}