#region

using System;
using System.Diagnostics;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO;
using SharpFlame.Core;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Mapping.Objects;

#endregion

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

        public void UnitRemoveStoreChange(int Num)
        {
            var UnitChange = new UnitChange();
            UnitChange.Type = UnitChangeType.Deleted;
            UnitChange.Unit = Units[Num];
            UnitChanges.Add(UnitChange);

            UnitRemove(Num);
        }

        public void UnitRemove(int Num)
        {
            var Unit = default(Unit);

            Unit = Units[Num];

            if ( SectorGraphicsChanges != null )
            {
                UnitSectorsGraphicsChanged(Unit);
            }

            if ( viewInfo != null )
            {
                var MouseOverTerrain = viewInfo.GetMouseOverTerrain();
                if ( MouseOverTerrain != null )
                {
                    var Pos = MouseOverTerrain.Units.FindFirstItemPosition(Unit);
                    if ( Pos >= 0 )
                    {
                        MouseOverTerrain.Units.RemoveAt(Pos);
                    }
                }
            }

            Unit.DisconnectFromMap();
        }

        public void UnitSwap(Unit OldUnit, Unit NewUnit)
        {
            if ( OldUnit.MapLink.Source != this )
            {
                Debugger.Break();
                return;
            }

            UnitRemoveStoreChange(OldUnit.MapLink.ArrayPosition);
            var UnitAdd = new clsUnitAdd();
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