using SharpFlame.Collections;
using SharpFlame.Mapping.Objects;

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectPriorityOrderList : SimpleListTool<clsUnit>
    {
        private SimpleClassList<clsUnit> result = new SimpleClassList<clsUnit>();

        public SimpleClassList<clsUnit> Result
        {
            get { return result; }
        }

        private clsUnit Unit;

        public clsObjectPriorityOrderList()
        {
            result.MaintainOrder = true;
        }

        public void ActionPerform()
        {
            int A = 0;

            for ( A = 0; A <= result.Count - 1; A++ )
            {
                if ( Unit.SavePriority > result[A].SavePriority )
                {
                    break;
                }
            }
            result.Insert(Unit, A);
        }

        public void SetItem(clsUnit Item)
        {
            Unit = Item;
        }
    }
}