namespace FlaME
{
    using Matrix3D;
    using Microsoft.VisualBasic;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class clsBrush
    {
        private bool _Alignment;
        private double _Radius;
        private enumShape _Shape = enumShape.Circle;
        public sBrushTiles Tiles;

        public clsBrush(double InitialRadius, enumShape InitialShape)
        {
            this._Radius = InitialRadius;
            this._Shape = InitialShape;
            this.CreateTiles();
        }

        private void CreateTiles()
        {
            double num = this._Radius - Conversion.Int(this._Radius);
            double radius = Conversion.Int((double) (this._Radius + 0.25));
            this._Alignment = (num >= 0.25) & (num < 0.75);
            switch (this._Shape)
            {
                case enumShape.Circle:
                    this.Tiles.CreateCircle(radius, 1.0, this._Alignment);
                    break;

                case enumShape.Square:
                    this.Tiles.CreateSquare(radius, 1.0, this._Alignment);
                    break;
            }
        }

        public modMath.sXY_int GetPosNum(sPosNum PosNum)
        {
            if (this._Alignment)
            {
                return PosNum.Alignment;
            }
            return PosNum.Normal;
        }

        private void PerformAction(clsMap.clsAction Action, sPosNum PosNum, modMath.sXY_int LastValidNum)
        {
            if (Action.Map == null)
            {
                Debugger.Break();
            }
            else
            {
                modMath.sXY_int posNum = this.GetPosNum(PosNum);
                Action.Effect = 1.0;
                int introduced11 = modMath.Clamp_int(this.Tiles.YMin + posNum.Y, 0, LastValidNum.Y);
                int introduced12 = modMath.Clamp_int(this.Tiles.YMax + posNum.Y, 0, LastValidNum.Y);
                int num4 = introduced12 - posNum.Y;
                for (int i = introduced11 - posNum.Y; i <= num4; i++)
                {
                    Action.PosNum.Y = posNum.Y + i;
                    int index = i - this.Tiles.YMin;
                    int introduced13 = modMath.Clamp_int(this.Tiles.XMin[index] + posNum.X, 0, LastValidNum.X);
                    int introduced14 = modMath.Clamp_int(this.Tiles.XMax[index] + posNum.X, 0, LastValidNum.X);
                    int num5 = introduced14 - posNum.X;
                    for (int j = introduced13 - posNum.X; j <= num5; j++)
                    {
                        modMath.sXY_int _int2;
                        Action.PosNum.X = posNum.X + j;
                        if (Action.UseEffect && (this.Tiles.ResultRadius > 0.0))
                        {
                            switch (this._Shape)
                            {
                                case enumShape.Circle:
                                {
                                    if (!this._Alignment)
                                    {
                                        goto Label_01DD;
                                    }
                                    Position.XY_dbl _dbl = new Position.XY_dbl((double) Action.PosNum.X, (double) Action.PosNum.Y);
                                    Position.XY_dbl _dbl2 = new Position.XY_dbl(posNum.X - 0.5, posNum.Y - 0.5);
                                    Position.XY_dbl _dbl3 = _dbl - _dbl2;
                                    Action.Effect = 1.0 - (_dbl3.GetMagnitude() / (this.Tiles.ResultRadius + 0.5));
                                    break;
                                }
                                case enumShape.Square:
                                {
                                    if (!this._Alignment)
                                    {
                                        goto Label_02A3;
                                    }
                                    double introduced15 = Math.Abs((double) (Action.PosNum.X - (posNum.X - 0.5)));
                                    Action.Effect = 1.0 - (Math.Max(introduced15, Math.Abs((double) (Action.PosNum.Y - (posNum.Y - 0.5)))) / (this.Tiles.ResultRadius + 0.5));
                                    break;
                                }
                            }
                        }
                        goto Label_02FF;
                    Label_01DD:
                        _int2 = posNum - Action.PosNum;
                        Action.Effect = 1.0 - (_int2.ToDoubles().GetMagnitude() / (this.Tiles.ResultRadius + 0.5));
                        goto Label_02FF;
                    Label_02A3:
                        int introduced16 = Math.Abs((int) (Action.PosNum.X - posNum.X));
                        Action.Effect = 1.0 - (((double) Math.Max(introduced16, Math.Abs((int) (Action.PosNum.Y - posNum.Y)))) / (this.Tiles.ResultRadius + 0.5));
                    Label_02FF:
                        Action.ActionPerform();
                    }
                }
            }
        }

        public void PerformActionMapSectors(clsMap.clsAction Tool, sPosNum Centre)
        {
            modMath.sXY_int lastValidNum = new modMath.sXY_int(Tool.Map.SectorCount.X - 1, Tool.Map.SectorCount.Y - 1);
            this.PerformAction(Tool, Centre, lastValidNum);
        }

        public void PerformActionMapTiles(clsMap.clsAction Tool, sPosNum Centre)
        {
            modMath.sXY_int lastValidNum = new modMath.sXY_int(Tool.Map.Terrain.TileSize.X - 1, Tool.Map.Terrain.TileSize.Y - 1);
            this.PerformAction(Tool, Centre, lastValidNum);
        }

        public void PerformActionMapVertices(clsMap.clsAction Tool, sPosNum Centre)
        {
            this.PerformAction(Tool, Centre, Tool.Map.Terrain.TileSize);
        }

        public bool Alignment
        {
            get
            {
                return this._Alignment;
            }
        }

        public double Radius
        {
            get
            {
                return this._Radius;
            }
            set
            {
                if (this._Radius != value)
                {
                    this._Radius = value;
                    this.CreateTiles();
                }
            }
        }

        public enumShape Shape
        {
            get
            {
                return this._Shape;
            }
            set
            {
                if (this._Shape != value)
                {
                    this._Shape = value;
                    this.CreateTiles();
                }
            }
        }

        public enum enumShape : byte
        {
            Circle = 0,
            Square = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sPosNum
        {
            public modMath.sXY_int Normal;
            public modMath.sXY_int Alignment;
        }
    }
}

