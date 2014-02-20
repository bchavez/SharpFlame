#region

using System;
using System.Diagnostics;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tools;

#endregion

namespace SharpFlame.Mapping
{
    public class clsUpdateSectorUnitHeights : clsAction
    {
        private UInt32 ID;
        private int NewAltitude;
        private clsUnit NewUnit;
        private int OldUnitCount;
        private clsUnit[] OldUnits;
        private bool Started;

        public void Start()
        {
            OldUnits = new clsUnit[Map.Units.Count];

            Started = true;
        }

        public void Finish()
        {
            if ( !Started )
            {
                Debugger.Break();
                return;
            }

            var A = 0;
            var UnitAdd = new clsUnitAdd();
            var Unit = default(clsUnit);

            UnitAdd.Map = Map;
            UnitAdd.StoreChange = true;

            for ( A = 0; A <= OldUnitCount - 1; A++ )
            {
                Unit = OldUnits[A];
                NewAltitude = (int)(Map.GetTerrainHeight(Unit.Pos.Horizontal));
                if ( NewAltitude != Unit.Pos.Altitude )
                {
                    NewUnit = new clsUnit(Unit, Map);
                    ID = Unit.ID;
                    //NewUnit.Pos.Altitude = NewAltitude
                    //these create changed sectors and must be done before drawing the new sectors
                    Map.UnitRemoveStoreChange(Unit.MapLink.ArrayPosition);
                    UnitAdd.NewUnit = NewUnit;
                    UnitAdd.ID = ID;
                    UnitAdd.Perform();
                    App.ErrorIDChange(ID, NewUnit, "UpdateSectorUnitHeights");
                }
            }

            Started = false;
        }

        public override void ActionPerform()
        {
            if ( !Started )
            {
                Debugger.Break();
                return;
            }

            var Connection = default(clsUnitSectorConnection);
            var Unit = default(clsUnit);
            var Sector = default(clsSector);
            var A = 0;

            Sector = Map.Sectors[PosNum.X, PosNum.Y];
            foreach ( var tempLoopVar_Connection in Sector.Units )
            {
                Connection = tempLoopVar_Connection;
                Unit = Connection.Unit;
                //units can be in multiple sectors, so dont include multiple times
                for ( A = 0; A <= OldUnitCount - 1; A++ )
                {
                    if ( OldUnits[A] == Unit )
                    {
                        break;
                    }
                }
                if ( A == OldUnitCount )
                {
                    OldUnits[OldUnitCount] = Unit;
                    OldUnitCount++;
                }
            }
        }
    }
}