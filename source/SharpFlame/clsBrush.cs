using System;
using System.Diagnostics;
using Matrix3D;
using Microsoft.VisualBasic;
using SharpFlame.MathExtra;

namespace SharpFlame
{
    public class clsBrush
    {
        public struct sPosNum
        {
            public sXY_int Normal;
            public sXY_int Alignment;
        }

        private double _Radius;

        public enum enumShape
        {
            Circle,
            Square
        }

        private enumShape _Shape = enumShape.Circle;

        public sBrushTiles Tiles;

        public bool Alignment
        {
            get { return _Alignment; }
        }

        private bool _Alignment;

        public double Radius
        {
            get { return _Radius; }
            set
            {
                if ( _Radius == value )
                {
                    return;
                }
                _Radius = value;
                CreateTiles();
            }
        }

        public enumShape Shape
        {
            get { return _Shape; }
            set
            {
                if ( _Shape == value )
                {
                    return;
                }
                _Shape = value;
                CreateTiles();
            }
        }

        private void CreateTiles()
        {
            double AlignmentOffset = _Radius - Conversion.Int(_Radius);
            double RadiusB = Conversion.Int(_Radius + 0.25D);

            _Alignment = AlignmentOffset >= 0.25D & AlignmentOffset < 0.75D;
            switch ( _Shape )
            {
                case enumShape.Circle:
                    Tiles.CreateCircle(RadiusB, 1.0D, _Alignment);
                    break;
                case enumShape.Square:
                    Tiles.CreateSquare(RadiusB, 1.0D, _Alignment);
                    break;
            }
        }

        public clsBrush(double InitialRadius, enumShape InitialShape)
        {
            _Radius = InitialRadius;
            _Shape = InitialShape;
            CreateTiles();
        }

        public void PerformActionMapTiles(clsMap.clsAction Tool, sPosNum Centre)
        {
            PerformAction(Tool, Centre, new sXY_int(Tool.Map.Terrain.TileSize.X - 1, Tool.Map.Terrain.TileSize.Y - 1));
        }

        public void PerformActionMapVertices(clsMap.clsAction Tool, sPosNum Centre)
        {
            PerformAction(Tool, Centre, Tool.Map.Terrain.TileSize);
        }

        public void PerformActionMapSectors(clsMap.clsAction Tool, sPosNum Centre)
        {
            PerformAction(Tool, Centre, new sXY_int(Tool.Map.SectorCount.X - 1, Tool.Map.SectorCount.Y - 1));
        }

        public sXY_int GetPosNum(sPosNum PosNum)
        {
            if ( _Alignment )
            {
                return PosNum.Alignment;
            }
            else
            {
                return PosNum.Normal;
            }
        }

        private void PerformAction(clsMap.clsAction Action, sPosNum PosNum, sXY_int LastValidNum)
        {
            int XNum = 0;
            int X = 0;
            int Y = 0;
            sXY_int Centre = new sXY_int();

            if ( Action.Map == null )
            {
                Debugger.Break();
                return;
            }

            Centre = GetPosNum(PosNum);

            Action.Effect = 1.0D;
            for ( Y = MathUtil.Clamp_int(Tiles.YMin + Centre.Y, 0, LastValidNum.Y) - Centre.Y;
                Y <= MathUtil.Clamp_int(Tiles.YMax + Centre.Y, 0, LastValidNum.Y) - Centre.Y;
                Y++ )
            {
                Action.PosNum.Y = Centre.Y + Y;
                XNum = Y - Tiles.YMin;
                for ( X = MathUtil.Clamp_int(Tiles.XMin[XNum] + Centre.X, 0, LastValidNum.X) - Centre.X;
                    X <= MathUtil.Clamp_int(Convert.ToInt32(Tiles.XMax[XNum] + Centre.X), 0, LastValidNum.X) - Centre.X;
                    X++ )
                {
                    Action.PosNum.X = Centre.X + X;
                    if ( Action.UseEffect )
                    {
                        if ( Tiles.ResultRadius > 0.0D )
                        {
                            switch ( _Shape )
                            {
                                case enumShape.Circle:
                                    if ( _Alignment )
                                    {
                                        Action.Effect =
                                            Convert.ToDouble(1.0D -
                                                                    (new Position.XY_dbl(Action.PosNum.X, Action.PosNum.Y) -
                                                                     new Position.XY_dbl(Centre.X - 0.5D, Centre.Y - 0.5D)).GetMagnitude() /
                                                                    (Tiles.ResultRadius + 0.5D));
                                    }
                                    else
                                    {
                                        Action.Effect = Convert.ToDouble(1.0D - (Centre - Action.PosNum).ToDoubles().GetMagnitude() / (Tiles.ResultRadius + 0.5D));
                                    }
                                    break;
                                case enumShape.Square:
                                    if ( _Alignment )
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
    }

    public struct sBrushTiles
    {
        public int[] XMin;
        public int[] XMax;
        public int YMin;
        public int YMax;
        public double ResultRadius;

        public void CreateCircle(double Radius, double TileSize, bool Alignment)
        {
            int X = 0;
            int Y = 0;
            double dblX = 0;
            double dblY = 0;
            double RadiusB = 0;
            double RadiusC = 0;
            int A = 0;
            int B = 0;

            RadiusB = Radius / TileSize;
            if ( Alignment )
            {
                RadiusB += 1.0D;
                Y = (int)(Conversion.Int(RadiusB));
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
                    X = (int)(Conversion.Int(dblX));
                    XMin[A] = Convert.ToInt32(- X);
                    XMax[A] = X - 1;
                }
            }
            else
            {
                RadiusB += 0.125D;
                Y = (int)(Conversion.Int(RadiusB));
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
                    X = (int)(Conversion.Int(dblX));
                    XMin[A] = Convert.ToInt32(- X);
                    XMax[A] = X;
                }
            }

            ResultRadius = B / 2.0D;
        }

        public void CreateSquare(double Radius, double TileSize, bool Alignment)
        {
            int Y = 0;
            int A = 0;
            int B = 0;
            double RadiusB = 0;

            RadiusB = Radius / TileSize + 0.5D;
            if ( Alignment )
            {
                RadiusB += 0.5D;
                A = (int)(Conversion.Int(RadiusB));
                YMin = Convert.ToInt32(- A);
                YMax = A - 1;
            }
            else
            {
                A = (int)(Conversion.Int(RadiusB));
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