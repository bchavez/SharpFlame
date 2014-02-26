#region

using System.Diagnostics;
using SharpFlame.Old.Collections;
using SharpFlame.Old.Mapping.Objects;
using SharpFlame.Core.Collections;
using SharpFlame.Old.Mapping.Objects;

#endregion

namespace SharpFlame.Old.Mapping.Tools
{
    public abstract class clsObjectAction : ISimpleListTool<Unit>
    {
        private readonly SimpleClassList<Unit> _ResultUnits = new SimpleClassList<Unit>();
        public bool ActionPerformed;
        public Map Map;

        protected Unit ResultUnit;
        public Unit Unit;

        public SimpleClassList<Unit> ResultUnits
        {
            get { return _ResultUnits; }
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
            ResultUnit = new Unit(Unit, Map);
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

        public void SetItem(Unit item)
        {
            Unit = item;
        }

        protected virtual void ActionCondition()
        {
        }

        protected abstract void _ActionPerform();
    }
}