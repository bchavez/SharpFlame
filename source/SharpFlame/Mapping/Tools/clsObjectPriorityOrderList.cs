#region

using SharpFlame.Collections;
using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public class clsObjectPriorityOrderList : ISimpleListTool<clsUnit>
    {
        private readonly SimpleClassList<clsUnit> result = new SimpleClassList<clsUnit>();

        private clsUnit Unit;

        public clsObjectPriorityOrderList()
        {
            result.MaintainOrder = true;
        }

        public SimpleClassList<clsUnit> Result
        {
            get { return result; }
        }

        public void ActionPerform()
        {
            var A = 0;

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