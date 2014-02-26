#region

using SharpFlame.Old.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.Old.Mapping.Objects;

#endregion

namespace SharpFlame.Old.Mapping.Tools
{
    public class clsObjectPriorityOrderList : ISimpleListTool<Unit>
    {
        private readonly SimpleClassList<Unit> result = new SimpleClassList<Unit>();

        private Unit Unit;

        public clsObjectPriorityOrderList()
        {
            result.MaintainOrder = true;
        }

        public SimpleClassList<Unit> Result
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

        public void SetItem(Unit item)
        {
            Unit = item;
        }
    }
}