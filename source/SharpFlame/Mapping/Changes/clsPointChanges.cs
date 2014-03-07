#region

using System.Collections.Generic;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Tools;

#endregion

namespace SharpFlame.Mapping.Changes
{
    public class clsPointChanges
    {
        public List<XYInt> ChangedPoints = new List<XYInt>();
        public bool[,] PointIsChanged;

        public clsPointChanges(XYInt PointSize)
        {
            PointIsChanged = new bool[PointSize.X, PointSize.Y];
            ChangedPoints.Clear();
        }

        public void Changed(XYInt Num)
        {
            if ( !PointIsChanged[Num.X, Num.Y] )
            {
                PointIsChanged[Num.X, Num.Y] = true;
                ChangedPoints.Add(Num);
            }
        }

        public void SetAllChanged()
        {
            var num = new XYInt(0, 0);
            for ( var y = 0; y <= PointIsChanged.GetUpperBound(1); y++ )
            {
                num.Y = y;
                for ( var x = 0; x <= PointIsChanged.GetUpperBound(0); x++ )
                {
                    num.X = x;
                    Changed(num);
                }
            }
        }

        public void Clear()
        {
            foreach ( var point in ChangedPoints )
            {
                PointIsChanged[point.X, point.Y] = false;
            }
            ChangedPoints.Clear();
        }

        public void PerformTool(clsAction Tool)
        {
            foreach ( var point in ChangedPoints )
            {
                Tool.PosNum = point;
                Tool.ActionPerform();
            }
        }
    }
}