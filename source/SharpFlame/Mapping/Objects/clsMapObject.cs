using System;
using System.Diagnostics;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Objects;

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public ConnectedList<clsUnit, clsMap> Units;

        private clsUnitGroupContainer _SelectedUnitGroup;

        public clsUnitGroupContainer SelectedUnitGroup
        {
            get { return _SelectedUnitGroup; }
        }

        public ConnectedList<clsUnitGroup, clsMap> UnitGroups;
        public clsUnitGroup ScavengerUnitGroup;

        public UInt32 GetAvailableID()
        {
            clsUnit Unit = default(clsUnit);
            UInt32 ID = 0;

            ID = 1U;
            foreach ( clsUnit tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.ID >= ID )
                {
                    ID = Unit.ID + 1U;
                }
            }

            return ID;
        }

        public void UnitRemoveStoreChange(int Num)
        {
            clsUnitChange UnitChange = new clsUnitChange();
            UnitChange.Type = clsUnitChange.enumType.Deleted;
            UnitChange.Unit = Units[Num];
            UnitChanges.Add(UnitChange);

            UnitRemove(Num);
        }

        public void UnitRemove(int Num)
        {
            clsUnit Unit = default(clsUnit);

            Unit = Units[Num];

            if ( SectorGraphicsChanges != null )
            {
                UnitSectorsGraphicsChanged(Unit);
            }

            if ( ViewInfo != null )
            {
                clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = ViewInfo.GetMouseOverTerrain();
                if ( MouseOverTerrain != null )
                {
                    int Pos = MouseOverTerrain.Units.FindFirstItemPosition(Unit);
                    if ( Pos >= 0 )
                    {
                        MouseOverTerrain.Units.Remove(Pos);
                    }
                }
            }

            Unit.DisconnectFromMap();
        }

        public void UnitSwap(clsUnit OldUnit, clsUnit NewUnit)
        {
            if ( OldUnit.MapLink.Source != this )
            {
                Debugger.Break();
                return;
            }

            UnitRemoveStoreChange(OldUnit.MapLink.ArrayPosition);
            clsUnitAdd UnitAdd = new clsUnitAdd();
            UnitAdd.Map = this;
            UnitAdd.StoreChange = true;
            UnitAdd.ID = OldUnit.ID;
            UnitAdd.NewUnit = NewUnit;
            UnitAdd.Label = OldUnit.Label;
            UnitAdd.Perform();
            App.ErrorIDChange(OldUnit.ID, NewUnit, "UnitSwap");
        }

        public void MakeDefaultUnitGroups()
        {
            int A = 0;
            clsUnitGroup NewGroup = default(clsUnitGroup);

            UnitGroups.Clear();
            for ( A = 0; A <= Constants.PlayerCountMax - 1; A++ )
            {
                NewGroup = new clsUnitGroup();
                NewGroup.WZ_StartPos = A;
                NewGroup.MapLink.Connect(UnitGroups);
            }
            ScavengerUnitGroup = new clsUnitGroup();
            ScavengerUnitGroup.MapLink.Connect(UnitGroups);
            ScavengerUnitGroup.WZ_StartPos = -1;
        }

        public sRGB_sng GetUnitGroupColour(clsUnitGroup ColourUnitGroup)
        {
            if ( ColourUnitGroup.WZ_StartPos < 0 )
            {
                return new sRGB_sng(1.0F, 1.0F, 1.0F);
            }
            else
            {
                return App.PlayerColour[ColourUnitGroup.WZ_StartPos].Colour;
            }
        }

        public sRGB_sng GetUnitGroupMinimapColour(clsUnitGroup ColourUnitGroup)
        {
            if ( ColourUnitGroup.WZ_StartPos < 0 )
            {
                return new sRGB_sng(1.0F, 1.0F, 1.0F);
            }
            else
            {
                return App.PlayerColour[ColourUnitGroup.WZ_StartPos].MinimapColour;
            }
        }

        public clsUnit IDUsage(UInt32 ID)
        {
            foreach ( clsUnit Unit in Units )
            {
                if ( Unit.ID == ID )
                {
                    return Unit;
                    //                          break;
                }
            }

            return null;
        }

        public void SetObjectCreatorDefaults(clsUnitCreate objectCreator)
        {
            objectCreator.Map = this;

            objectCreator.ObjectTypeBase = Program.frmMainInstance.SingleSelectedObjectTypeBase;
            objectCreator.AutoWalls = Program.frmMainInstance.cbxAutoWalls.Checked;
            objectCreator.UnitGroup = SelectedUnitGroup.Item;
            try
            {
                int Rotation = 0;
                IOUtil.InvariantParse(Program.frmMainInstance.txtNewObjectRotation.Text, ref Rotation);
                if ( Rotation < 0 | Rotation > 359 )
                {
                    objectCreator.Rotation = 0;
                }
                else
                {
                    objectCreator.Rotation = Rotation;
                }
            }
            catch
            {
                objectCreator.Rotation = 0;
            }
            objectCreator.RandomizeRotation = Program.frmMainInstance.cbxObjectRandomRotation.Checked;
        }
    }
}