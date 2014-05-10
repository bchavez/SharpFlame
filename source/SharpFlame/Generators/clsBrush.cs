

using System;
using System.Diagnostics;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;



namespace SharpFlame
{
    public class clsBrush
    {
        public sBrushTiles Tiles;

        private bool alignment;
        private double radius;
        private ShapeType shape = ShapeType.Circle;

        public clsBrush(double initialRadius, ShapeType initialShape)
        {
            radius = initialRadius;
            shape = initialShape;
            CreateTiles();
        }

        public bool Alignment
        {
            get { return alignment; }
        }

        public double Radius
        {
            get { return radius; }
            set
            {
                if ( radius == value )
                {
                    return;
                }
                radius = value;
                CreateTiles();
            }
        }

        public ShapeType Shape
        {
            get { return shape; }
            set
            {
                if ( shape == value )
                {
                    return;
                }
                shape = value;
                CreateTiles();
            }
        }

        private void CreateTiles()
        {
            var alignmentOffset = radius - Math.Floor(radius);
            double radiusB = Math.Floor(radius + 0.25D);

            alignment = alignmentOffset >= 0.25D & alignmentOffset < 0.75D;
            switch ( shape )
            {
                case ShapeType.Circle:
                    Tiles.CreateCircle(radiusB, 1.0D, alignment);
                    break;
                case ShapeType.Square:
                    Tiles.CreateSquare(radiusB, 1.0D, alignment);
                    break;
            }
        }

        public void PerformActionMapTiles(clsAction tool, sPosNum centre)
        {
            PerformAction(tool, centre, new XYInt(tool.Map.Terrain.TileSize.X - 1, tool.Map.Terrain.TileSize.Y - 1));
        }

        public void PerformActionMapVertices(clsAction tool, sPosNum centre)
        {
            PerformAction(tool, centre, tool.Map.Terrain.TileSize);
        }

        public void PerformActionMapSectors(clsAction tool, sPosNum centre)
        {
            PerformAction(tool, centre, new XYInt(tool.Map.SectorCount.X - 1, tool.Map.SectorCount.Y - 1));
        }

        public XYInt GetPosNum(sPosNum posNum)
        {
            if ( alignment )
            {
                return posNum.Alignment;
            }
            return posNum.Normal;
        }

        private void PerformAction(clsAction action, sPosNum posNum, XYInt lastValidNum)
        {
            var y = 0;

            if ( action.Map == null )
            {
                Debugger.Break();
                return;
            }

            var centre = GetPosNum(posNum);

            action.Effect = 1.0D;
            for ( y = MathUtil.ClampInt(Tiles.YMin + centre.Y, 0, lastValidNum.Y) - centre.Y;
                y <= MathUtil.ClampInt(Tiles.YMax + centre.Y, 0, lastValidNum.Y) - centre.Y;
                y++ )
            {
                action.PosNum.Y = centre.Y + y;
                var xNum = y - Tiles.YMin;
                var x = 0;
                for ( x = MathUtil.ClampInt(Tiles.XMin[xNum] + centre.X, 0, lastValidNum.X) - centre.X;
                    x <= MathUtil.ClampInt(Convert.ToInt32(Tiles.XMax[xNum] + centre.X), 0, lastValidNum.X) - centre.X;
                    x++ )
                {
                    action.PosNum.X = centre.X + x;
                    if ( action.UseEffect )
                    {
                        if ( Tiles.ResultRadius > 0.0D )
                        {
                            switch ( shape )
                            {
                                case ShapeType.Circle:
                                    if ( alignment )
                                    {
                                        action.Effect =
                                            Convert.ToDouble(1.0D -
                                                             (new XYDouble(action.PosNum.X, action.PosNum.Y) -
                                                              new XYDouble(centre.X - 0.5D, centre.Y - 0.5D)).GetMagnitude() /
                                                             (Tiles.ResultRadius + 0.5D));
                                    }
                                    else
                                    {
                                        action.Effect = Convert.ToDouble(1.0D - (centre - action.PosNum).ToDoubles().GetMagnitude() / (Tiles.ResultRadius + 0.5D));
                                    }
                                    break;
                                case ShapeType.Square:
                                    if ( alignment )
                                    {
                                        action.Effect = 1.0D -
                                                        Math.Max(Math.Abs(action.PosNum.X - (centre.X - 0.5D)), Math.Abs(action.PosNum.Y - (centre.Y - 0.5D))) /
                                                        (Tiles.ResultRadius + 0.5D);
                                    }
                                    else
                                    {
                                        action.Effect = 1.0D -
                                                        Math.Max(Math.Abs(action.PosNum.X - centre.X), Math.Abs(action.PosNum.Y - centre.Y)) /
                                                        (Tiles.ResultRadius + 0.5D);
                                    }
                                    break;
                            }
                        }
                    }
                    action.ActionPerform();
                }
            }
        }

        public struct sPosNum
        {
            public XYInt Alignment;
            public XYInt Normal;
        }
    }

    public enum ShapeType
    {
        Circle,
        Square
    }

    public struct sBrushTiles
    {
        public double ResultRadius;
        public int[] XMax;
        public int[] XMin;
        public int YMax;
        public int YMin;

        public void CreateCircle(double Radius, double TileSize, bool Alignment)
        {
            var X = 0;
            var Y = 0;
            double dblX = 0;
            double dblY = 0;
            double radiusB = 0;
            double radiusC = 0;
            var A = 0;
            var B = 0;

            radiusB = Radius / TileSize;
            if ( Alignment )
            {
                radiusB += 1.0D;
                Y = (radiusB).Floor().ToInt();
                YMin = Convert.ToInt32(- Y);
                YMax = Y - 1;
                B = YMax - YMin;
                XMin = new int[B + 1];
                XMax = new int[B + 1];
                radiusC = radiusB * radiusB;
                for ( Y = YMin; Y <= YMax; Y++ )
                {
                    dblY = Y + 0.5D;
                    dblX = Math.Sqrt(radiusC - dblY * dblY) + 0.5D;
                    A = Y - YMin;
                    X = (dblX).Floor().ToInt();
                    XMin[A] = Convert.ToInt32(- X);
                    XMax[A] = X - 1;
                }
            }
            else
            {
                radiusB += 0.125D;
                Y = Math.Floor(radiusB).ToInt();
                YMin = Convert.ToInt32(- Y);
                YMax = Y;
                B = YMax - YMin;
                XMin = new int[B + 1];
                XMax = new int[B + 1];
                radiusC = radiusB * radiusB;
                for ( Y = YMin; Y <= YMax; Y++ )
                {
                    dblY = Y;
                    dblX = Math.Sqrt(radiusC - dblY * dblY);
                    A = Y - YMin;
                    X = dblX.Floor().ToInt();
                    XMin[A] = -X;
                    XMax[A] = X;
                }
            }

            ResultRadius = B / 2.0D;
        }

        public void CreateSquare(double Radius, double TileSize, bool Alignment)
        {
            var Y = 0;
            var A = 0;
            var B = 0;
            double RadiusB = 0;

            RadiusB = Radius / TileSize + 0.5D;
            if ( Alignment )
            {
                RadiusB += 0.5D;
                A = RadiusB.Floor().ToInt();
                YMin = -A;
                YMax = A - 1;
            }
            else
            {
                A = RadiusB.Floor().ToInt();
                YMin = -A;
                YMax = A;
            }
            B = YMax - YMin;
            XMin = new int[B + 1];
            XMax = new int[B + 1];
            for ( Y = 0; Y <= B; Y++ )
            {
                XMin[Y] = YMin;
                XMax[Y] = YMax;
            }

            ResultRadius = B / 2.0D;
        }
    }
}