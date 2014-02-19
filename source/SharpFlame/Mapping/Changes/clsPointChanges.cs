using SharpFlame.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Changes
{
    public class clsPointChanges
    {
        public bool[,] PointIsChanged;
        public SimpleList<XYInt> ChangedPoints = new SimpleList<XYInt>();

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
            int X = 0;
            int Y = 0;
            XYInt Num = new XYInt(0, 0);

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
            XYInt Point = default(XYInt);

            foreach ( XYInt tempLoopVar_Point in ChangedPoints )
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