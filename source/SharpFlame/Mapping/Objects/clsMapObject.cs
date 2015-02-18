using System;
using System.Diagnostics;
using SharpFlame.FileIO;
using SharpFlame.Core;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Mapping.Objects;


namespace SharpFlame.Mapping
{
    public partial class Map
    {
        public clsUnitGroup ScavengerUnitGroup;
        public ConnectedList<clsUnitGroup, Map> UnitGroups;
        public ConnectedList<Unit, Map> Units;

        private clsUnitGroupContainer _SelectedUnitGroup;

        public clsUnitGroupContainer SelectedUnitGroup
        {
            get { return _SelectedUnitGroup; }
        }

        public UInt32 GetAvailableID()
        {
            var Unit = default(Unit);
            UInt32 ID = 0;

            ID = 1U;
            foreach ( var tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.ID >= ID )
                {
                    ID = Unit.ID + 1U;
                }
            }

            return ID;
        }

        public void UnitRemoveStoreChange(int num)
        {
            var unitChange = new UnitChange
	            {
		            Type = UnitChangeType.Deleted,
					Unit = Units[num]
	            };
	        UnitChanges.Add(unitChange);

            UnitRemove(num);
        }

        public void UnitRemove(int num)
        {
	        var unit = Units[num];

            if ( SectorGraphicsChanges != null )
            {
                UnitSectorsGraphicsChanged(unit);
            }

            if ( this.ViewInfo != null )
            {
                var mouseOverTerrain = this.ViewInfo.GetMouseOverTerrain();
                if ( mouseOverTerrain != null )
                {
                    var pos = mouseOverTerrain.Units.FindFirstItemPosition(unit);
                    if ( pos >= 0 )
                    {
                        mouseOverTerrain.Units.RemoveAt(pos);
                    }
                }
            }

            unit.DisconnectFromMap();
        }

        public void UnitSwap(Unit oldUnit, Unit newUnit)
        {
            if ( oldUnit.MapLink.Source != this )
            {
                Debugger.Break();
                return;
            }

            UnitRemoveStoreChange(oldUnit.MapLink.ArrayPosition);
            var unitAdd = new clsUnitAdd
	            {
		            Map = this, 
					StoreChange = true,
					ID = oldUnit.ID,
					NewUnit = newUnit,
					Label = oldUnit.Label
	            };
	        unitAdd.Perform();
            App.ErrorIDChange(oldUnit.ID, newUnit, "UnitSwap");
        }

        public void MakeDefaultUnitGroups()
        {
            var A = 0;
            var NewGroup = default(clsUnitGroup);

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

        public SRgb GetUnitGroupColour(clsUnitGroup ColourUnitGroup)
        {
            if ( ColourUnitGroup.WZ_StartPos < 0 )
            {
                return new SRgb(1.0F, 1.0F, 1.0F);
            }
            return App.PlayerColour[ColourUnitGroup.WZ_StartPos].Colour;
        }

        public SRgb GetUnitGroupMinimapColour(clsUnitGroup ColourUnitGroup)
        {
            if ( ColourUnitGroup.WZ_StartPos < 0 )
            {
                return new SRgb(1.0F, 1.0F, 1.0F);
            }
            return App.PlayerColour[ColourUnitGroup.WZ_StartPos].MinimapColour;
        }

        public Unit IDUsage(UInt32 ID)
        {
            foreach ( var Unit in Units )
            {
                if ( Unit.ID == ID )
                {
                    return Unit;
                    //							break;
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
                var Rotation = 0;
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