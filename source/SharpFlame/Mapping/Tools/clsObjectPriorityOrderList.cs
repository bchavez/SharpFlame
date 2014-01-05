using SharpFlame.Collections;
using SharpFlame.Mapping.Objects;

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectPriorityOrderList : SimpleListTool<clsUnit>
    {
        private SimpleClassList<clsUnit> _Result = new SimpleClassList<clsUnit>();

        public SimpleClassList<clsUnit> Result
        {
            get { return _Result; }
        }

        private clsUnit Unit;

        public clsObjectPriorityOrderList()
        {
            _Result.MaintainOrder = true;
        }

        public void ActionPerform()
        {
            int A = 0;

            for ( A = 0; A <= _Result.Count - 1; A++ )
            {
                if ( Unit.SavePriority > _Result[A].SavePriority )
                {
                    break;
                }
            }
            _Result.Insert(Unit, A);
        }

        public void SetItem(clsUnit Item)
        {
            Unit = Item;
        }
    }
}