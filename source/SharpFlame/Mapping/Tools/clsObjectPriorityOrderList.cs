

using System.Collections.ObjectModel;
using SharpFlame.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.Mapping.Objects;


namespace SharpFlame.Mapping.Tools
{
    public class clsObjectPriorityOrderList : ISimpleListTool<Unit>
    {
        private readonly ObservableCollection<Unit> result = new ObservableCollection<Unit>();

        private Unit Unit;


        public ObservableCollection<Unit> Result
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
            result.Insert(A, Unit);
        }

        public void SetItem(Unit item)
        {
            Unit = item;
        }
    }
}