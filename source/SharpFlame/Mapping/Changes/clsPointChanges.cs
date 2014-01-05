using SharpFlame.Collections;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Changes
{
    public class clsPointChanges
    {
        public bool[,] PointIsChanged;
        public SimpleList<clsXY_int> ChangedPoints = new SimpleList<clsXY_int>();

        public clsPointChanges(sXY_int PointSize)
        {
            PointIsChanged = new bool[PointSize.X, PointSize.Y];
            ChangedPoints.MinSize = PointSize.X * PointSize.Y;
            ChangedPoints.Clear();
        }

        public void Changed(sXY_int Num)
        {
            if ( !PointIsChanged[Num.X, Num.Y] )
            {
                PointIsChanged[Num.X, Num.Y] = true;
                ChangedPoints.Add(new clsXY_int(Num));
            }
        }

        public void SetAllChanged()
        {
            int X = 0;
            int Y = 0;
            sXY_int Num = new sXY_int();

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
            clsXY_int Point = default(clsXY_int);

            foreach ( clsXY_int tempLoopVar_Point in ChangedPoints )
            {
                Point = tempLoopVar_Point;
                PointIsChanged[Point.X, Point.Y] = false;
            }
            ChangedPoints.Clear();
        }

        public void PerformTool(clsMap.clsAction Tool)
        {
            clsXY_int Point = default(clsXY_int);

            foreach ( clsXY_int tempLoopVar_Point in ChangedPoints )
            {
                Point = tempLoopVar_Point;
                Tool.PosNum = Point.XY;
                Tool.ActionPerform();
            }
        }
    }
}