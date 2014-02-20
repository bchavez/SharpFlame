#region

using SharpFlame.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Tools;

#endregion

namespace SharpFlame.Mapping.Changes
{
    public class clsPointChanges
    {
        public SimpleList<XYInt> ChangedPoints = new SimpleList<XYInt>();
        public bool[,] PointIsChanged;

        public clsPointChanges(XYInt PointSize)
        {
            PointIsChanged = new bool[PointSize.X, PointSize.Y];
            ChangedPoints.MinSize = PointSize.X * PointSize.Y;
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
            var X = 0;
            var Y = 0;
            var Num = new XYInt(0, 0);

            for ( Y = 0; Y <= PointIsChanged.GetUpperBound(1); Y++ )
            {
                Num.Y = Y;
                for ( X = 0; X <= PointIsChanged.GetUpperBound(0); X++ )
                {
                    Num.X = X;
                    Changed(Num);
                }
            }
        }

        public void Clear()
        {
            var Point = default(XYInt);

            foreach ( var tempLoopVar_Point in ChangedPoints )
            {
                Point = tempLoopVar_Point;
                PointIsChanged[Point.X, Point.Y] = false;
            }
            ChangedPoints.Clear();
        }

        public void PerformTool(clsAction Tool)
        {
            foreach ( var tempLoopVar_Point in ChangedPoints )
            {
                Tool.PosNum = tempLoopVar_Point;
                Tool.ActionPerform();
            }
        }
    }
}