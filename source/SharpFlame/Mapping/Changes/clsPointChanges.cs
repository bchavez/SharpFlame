

using System.Collections.Generic;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Tools;


namespace SharpFlame.Mapping.Changes
{
    public class clsPointChanges
    {
        public List<XYInt> ChangedPoints = new List<XYInt>();

        private XYInt pointSize;

        public clsPointChanges(XYInt ps)
        {
            pointSize = ps;
            ChangedPoints.Clear();
        }

        public void Changed(XYInt Num)
        {
            if(!ChangedPoints.Contains(Num))
            {
                ChangedPoints.Add(Num);
            }
        }

        public void SetAllChanged()
        {
            ChangedPoints.Clear();
            var num = new XYInt(0, 0);
            for ( var y = 0; y < pointSize.Y; y++ )
            {
                num.Y = y;
                for ( var x = 0; x < pointSize.X; x++ )
                {
                    num.X = x;
                    ChangedPoints.Add(num);
                }
            }
        }

        public void Clear()
        {
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