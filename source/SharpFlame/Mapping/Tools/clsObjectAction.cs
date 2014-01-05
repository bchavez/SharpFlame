using System.Diagnostics;
using SharpFlame.Collections;
using SharpFlame.Mapping.Objects;

namespace SharpFlame.Mapping.Tools
{
    public abstract class clsObjectAction : SimpleListTool<clsUnit>
    {
        public clsMap Map;
        public clsUnit Unit;
        private SimpleClassList<clsUnit> _ResultUnits = new SimpleClassList<clsUnit>();
        public bool ActionPerformed;

        protected clsUnit ResultUnit;

        public SimpleClassList<clsUnit> ResultUnits
        {
            get { return _ResultUnits; }
        }

        protected virtual void ActionCondition()
        {
        }

        public void ActionPerform()
        {
            ResultUnit = null;
            ActionPerformed = false;
            if ( Unit == null )
            {
                Debugger.Break();
                return;
            }
            ActionPerformed = true;
            ActionCondition();
            if ( !ActionPerformed )
            {
                return;
            }
            ResultUnit = new clsUnit(Unit, Map);
            _ActionPerform();
            if ( ResultUnit == null )
            {
                ResultUnit = Unit;
            }
            else
            {
                _ResultUnits.Add(ResultUnit);
                Map.UnitSwap(Unit, ResultUnit);
            }
        }

        protected abstract void _ActionPerform();

        public void SetItem(clsUnit Item)
        {
            Unit = Item;
        }
    }
}