#region

using System;
using System.Diagnostics;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;

#endregion

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
            var AlignmentOffset = radius - (int)radius;
            double RadiusB = (int)(radius + 0.25D);

            alignment = AlignmentOffset >= 0.25D & AlignmentOffset < 0.75D;
            switch ( shape )
            {
                case ShapeType.Circle:
                    Tiles.CreateCircle(RadiusB, 1.0D, alignment);
                    break;
                case ShapeType.Square:
                    Tiles.CreateSquare(RadiusB, 1.0D, alignment);
                    break;
            }
        }

        public void PerformActionMapTiles(clsAction Tool, sPosNum Centre)
        {
            PerformAction(Tool, Centre, new XYInt(Tool.Map.Terrain.TileSize.X - 1, Tool.Map.Terrain.TileSize.Y - 1));
        }

        public void PerformActionMapVertices(clsAction Tool, sPosNum Centre)
        {
            PerformAction(Tool, Centre, Tool.Map.Terrain.TileSize);
        }

        public void PerformActionMapSectors(clsAction Tool, sPosNum Centre)
        {
            PerformAction(Tool, Centre, new XYInt(Tool.Map.SectorCount.X - 1, Tool.Map.SectorCount.Y - 1));
        }

        public XYInt GetPosNum(sPosNum PosNum)
        {
            if ( alignment )
            {
                return PosNum.Alignment;
            }
            return PosNum.Normal;
        }

        private void PerformAction(clsAction Action, sPosNum PosNum, XYInt LastValidNum)
        {
            var XNum = 0;
            var X = 0;
            var Y = 0;
            var Centre = new XYInt(0, 0);

            if ( Action.Map == null )
            {
                Debugger.Break();
                return;
            }

            Centre = GetPosNum(PosNum);

            Action.Effect = 1.0D;
            for ( Y = MathUtil.ClampInt(Tiles.YMin + Centre.Y, 0, LastValidNum.Y) - Centre.Y;
                Y <= MathUtil.ClampInt(Tiles.YMax + Centre.Y, 0, LastValidNum.Y) - Centre.Y;
                Y++ )
            {
                Action.PosNum.Y = Centre.Y + Y;
                XNum = Y - Tiles.YMin;
                for ( X = MathUtil.ClampInt(Tiles.XMin[XNum] + Centre.X, 0, LastValidNum.X) - Centre.X;
                    X <= MathUtil.ClampInt(Convert.ToInt32(Tiles.XMax[XNum] + Centre.X), 0, LastValidNum.X) - Centre.X;
                    X++ )
                {
                    Action.PosNum.X = Centre.X + X;
                    if ( Action.UseEffect )
                    {
                        if ( Tiles.ResultRadius > 0.0D )
                        {
                            switch ( shape )
                            {
                                case ShapeType.Circle:
                                    if ( alignment )
                                    {
                                        Action.Effect =
                                            Convert.ToDouble(1.0D -
                                                             (new XYDouble(Action.PosNum.X, Action.PosNum.Y) -
                                                              new XYDouble(Centre.X - 0.5D, Centre.Y - 0.5D)).GetMagnitude() /
                                                             (Tiles.ResultRadius + 0.5D));
                                    }
                                    else
                                    {
                                        Action.Effect = Convert.ToDouble(1.0D - (Centre - Action.PosNum).ToDoubles().GetMagnitude() / (Tiles.ResultRadius + 0.5D));
                                    }
                                    break;
                                case ShapeType.Square:
                                    if ( alignment )
                                    {
                                        Action.Effect = 1.0D -
                                                        Math.Max(Math.Abs(Action.PosNum.X - (Centre.X - 0.5D)), Math.Abs(Action.PosNum.Y - (Centre.Y - 0.5D))) /
                                                        (Tiles.ResultRadius + 0.5D);
                                    }
                                    else
                                    {
                                        Action.Effect = 1.0D -
                                                        Math.Max(Math.Abs(Action.PosNum.X - Centre.X), Math.Abs(Action.PosNum.Y - Centre.Y)) /
                                                        (Tiles.ResultRadius + 0.5D);
                                    }
                                    break;
                            }
                        }
                    }
                    Action.ActionPerform();
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
            double RadiusB = 0;
            double RadiusC = 0;
            var A = 0;
            var B = 0;

            RadiusB = Radius / TileSize;
            if ( Alignment )
            {
                RadiusB += 1.0D;
                Y = (int)RadiusB;
                YMin = Convert.ToInt32(- Y);
                YMax = Y - 1;
                B = YMax - YMin;
                XMin = new int[B + 1];
                XMax = new int[B + 1];
                RadiusC = RadiusB * RadiusB;
                for ( Y = YMin; Y <= YMax; Y++ )
                {
                    dblY = Y + 0.5D;
                    dblX = Math.Sqrt(RadiusC - dblY * dblY) + 0.5D;
                    A = Y - YMin;
                    X = (int)dblX;
                    XMin[A] = Convert.ToInt32(- X);
                    XMax[A] = X - 1;
                }
            }
            else
            {
                RadiusB += 0.125D;
                Y = (int)RadiusB;
                YMin = Convert.ToInt32(- Y);
                YMax = Y;
                B = YMax - YMin;
                XMin = new int[B + 1];
                XMax = new int[B + 1];
                RadiusC = RadiusB * RadiusB;
                for ( Y = YMin; Y <= YMax; Y++ )
                {
                    dblY = Y;
                    dblX = Math.Sqrt(RadiusC - dblY * dblY);
                    A = Y - YMin;
                    X = (int)dblX;
                    XMin[A] = Convert.ToInt32(- X);
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
                A = (int)RadiusB;
                YMin = Convert.ToInt32(- A);
                YMax = A - 1;
            }
            else
            {
                A = (int)RadiusB;
                YMin = Convert.ToInt32(- A);
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