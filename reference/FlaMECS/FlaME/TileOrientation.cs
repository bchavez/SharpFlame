namespace FlaME
{
    using Matrix3D;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [StandardModule]
    public sealed class TileOrientation
    {
        public static sTileOrientation Orientation_Clockwise = new sTileOrientation(true, false, true);
        public static sTileOrientation Orientation_CounterClockwise = new sTileOrientation(false, true, true);
        public static sTileOrientation Orientation_FlipX = new sTileOrientation(true, false, false);
        public static sTileOrientation Orientation_FlipY = new sTileOrientation(false, true, false);
        public static sTileDirection TileDirection_Bottom = new sTileDirection(1, 2);
        public static sTileDirection TileDirection_BottomLeft = new sTileDirection(0, 2);
        public static sTileDirection TileDirection_BottomRight = new sTileDirection(2, 2);
        public static sTileDirection TileDirection_Left = new sTileDirection(0, 1);
        public static sTileDirection TileDirection_None = new sTileDirection(1, 1);
        public static sTileDirection TileDirection_Right = new sTileDirection(2, 1);
        public static sTileDirection TileDirection_Top = new sTileDirection(1, 0);
        public static sTileDirection TileDirection_TopLeft = new sTileDirection(0, 0);
        public static sTileDirection TileDirection_TopRight = new sTileDirection(2, 0);

        public static bool DirectionsAreInLine(sTileDirection DirectionA, sTileDirection DirectionB)
        {
            return ((DirectionA.X == DirectionB.X) || (DirectionA.Y == DirectionB.Y));
        }

        public static bool DirectionsOnSameSide(sTileDirection DirectionA, sTileDirection DirectionB)
        {
            return (((DirectionA.X == 0) && (DirectionB.X == 0)) || (((DirectionA.X == 2) && (DirectionB.X == 2)) || (((DirectionA.Y == 0) && (DirectionB.Y == 0)) || ((DirectionA.Y == 2) && (DirectionB.Y == 2)))));
        }

        public static double GetRotatedAngle(sTileOrientation Orientation, double Angle)
        {
            Position.XY_dbl pos = new Position.XY_dbl((Math.Cos(Angle) + 1.0) / 2.0, (Math.Sin(Angle) + 1.0) / 2.0);
            Position.XY_dbl _dbl = GetTileRotatedPos_dbl(Orientation, pos);
            _dbl.X = (_dbl.X * 2.0) - 1.0;
            _dbl.Y = (_dbl.Y * 2.0) - 1.0;
            return Math.Atan2(_dbl.Y, _dbl.X);
        }

        public static modMath.sXY_int GetRotatedPos(sTileOrientation Orientation, modMath.sXY_int Pos, modMath.sXY_int Limits)
        {
            modMath.sXY_int _int2;
            if (Orientation.SwitchedAxes)
            {
                if (Orientation.ResultXFlip)
                {
                    _int2.X = Limits.Y - Pos.Y;
                }
                else
                {
                    _int2.X = Pos.Y;
                }
                if (Orientation.ResultYFlip)
                {
                    _int2.Y = Limits.X - Pos.X;
                    return _int2;
                }
                _int2.Y = Pos.X;
                return _int2;
            }
            if (Orientation.ResultXFlip)
            {
                _int2.X = Limits.X - Pos.X;
            }
            else
            {
                _int2.X = Pos.X;
            }
            if (Orientation.ResultYFlip)
            {
                _int2.Y = Limits.Y - Pos.Y;
                return _int2;
            }
            _int2.Y = Pos.Y;
            return _int2;
        }

        public static modMath.sXY_int GetTileRotatedOffset(sTileOrientation TileOrientation, modMath.sXY_int Pos)
        {
            modMath.sXY_int _int2;
            if (TileOrientation.SwitchedAxes)
            {
                if (TileOrientation.ResultXFlip)
                {
                    _int2.X = 0x80 - Pos.Y;
                }
                else
                {
                    _int2.X = Pos.Y;
                }
                if (TileOrientation.ResultYFlip)
                {
                    _int2.Y = 0x80 - Pos.X;
                    return _int2;
                }
                _int2.Y = Pos.X;
                return _int2;
            }
            if (TileOrientation.ResultXFlip)
            {
                _int2.X = 0x80 - Pos.X;
            }
            else
            {
                _int2.X = Pos.X;
            }
            if (TileOrientation.ResultYFlip)
            {
                _int2.Y = 0x80 - Pos.Y;
                return _int2;
            }
            _int2.Y = Pos.Y;
            return _int2;
        }

        public static Position.XY_dbl GetTileRotatedPos_dbl(sTileOrientation TileOrientation, Position.XY_dbl Pos)
        {
            Position.XY_dbl _dbl2;
            if (TileOrientation.SwitchedAxes)
            {
                if (TileOrientation.ResultXFlip)
                {
                    _dbl2.X = 1.0 - Pos.Y;
                }
                else
                {
                    _dbl2.X = Pos.Y;
                }
                if (TileOrientation.ResultYFlip)
                {
                    _dbl2.Y = 1.0 - Pos.X;
                    return _dbl2;
                }
                _dbl2.Y = Pos.X;
                return _dbl2;
            }
            if (TileOrientation.ResultXFlip)
            {
                _dbl2.X = 1.0 - Pos.X;
            }
            else
            {
                _dbl2.X = Pos.X;
            }
            if (TileOrientation.ResultYFlip)
            {
                _dbl2.Y = 1.0 - Pos.Y;
                return _dbl2;
            }
            _dbl2.Y = Pos.Y;
            return _dbl2;
        }

        public static modMath.sXY_sng GetTileRotatedPos_sng(sTileOrientation TileOrientation, modMath.sXY_sng Pos)
        {
            modMath.sXY_sng _sng2;
            if (TileOrientation.SwitchedAxes)
            {
                if (TileOrientation.ResultXFlip)
                {
                    _sng2.X = 1f - Pos.Y;
                }
                else
                {
                    _sng2.X = Pos.Y;
                }
                if (TileOrientation.ResultYFlip)
                {
                    _sng2.Y = 1f - Pos.X;
                    return _sng2;
                }
                _sng2.Y = Pos.X;
                return _sng2;
            }
            if (TileOrientation.ResultXFlip)
            {
                _sng2.X = 1f - Pos.X;
            }
            else
            {
                _sng2.X = Pos.X;
            }
            if (TileOrientation.ResultYFlip)
            {
                _sng2.Y = 1f - Pos.Y;
                return _sng2;
            }
            _sng2.Y = Pos.Y;
            return _sng2;
        }

        public static void GetTileRotatedTexCoords(sTileOrientation TileOrientation, ref modMath.sXY_sng CoordA, ref modMath.sXY_sng CoordB, ref modMath.sXY_sng CoordC, ref modMath.sXY_sng CoordD)
        {
            sTileOrientation orientation = TileOrientation;
            orientation.Reverse();
            if (orientation.SwitchedAxes)
            {
                if (orientation.ResultXFlip)
                {
                    CoordA.X = 1f;
                    CoordB.X = 1f;
                    CoordC.X = 0f;
                    CoordD.X = 0f;
                }
                else
                {
                    CoordA.X = 0f;
                    CoordB.X = 0f;
                    CoordC.X = 1f;
                    CoordD.X = 1f;
                }
                if (orientation.ResultYFlip)
                {
                    CoordA.Y = 1f;
                    CoordB.Y = 0f;
                    CoordC.Y = 1f;
                    CoordD.Y = 0f;
                }
                else
                {
                    CoordA.Y = 0f;
                    CoordB.Y = 1f;
                    CoordC.Y = 0f;
                    CoordD.Y = 1f;
                }
            }
            else
            {
                if (orientation.ResultXFlip)
                {
                    CoordA.X = 1f;
                    CoordB.X = 0f;
                    CoordC.X = 1f;
                    CoordD.X = 0f;
                }
                else
                {
                    CoordA.X = 0f;
                    CoordB.X = 1f;
                    CoordC.X = 0f;
                    CoordD.X = 1f;
                }
                if (orientation.ResultYFlip)
                {
                    CoordA.Y = 1f;
                    CoordB.Y = 1f;
                    CoordC.Y = 0f;
                    CoordD.Y = 0f;
                }
                else
                {
                    CoordA.Y = 0f;
                    CoordB.Y = 0f;
                    CoordC.Y = 1f;
                    CoordD.Y = 1f;
                }
            }
        }

        public static bool IdenticalTileDirections(sTileDirection TileOrientationA, sTileDirection TileOrientationB)
        {
            return ((TileOrientationA.X == TileOrientationB.X) & (TileOrientationA.Y == TileOrientationB.Y));
        }

        public static void OldOrientation_To_TileOrientation(byte OldRotation, bool OldFlipX, bool OldFlipZ, ref sTileOrientation Result)
        {
            if (OldRotation == 0)
            {
                Result.SwitchedAxes = false;
                Result.ResultXFlip = false;
                Result.ResultYFlip = false;
            }
            else if (OldRotation == 1)
            {
                Result.SwitchedAxes = true;
                Result.ResultXFlip = true;
                Result.ResultYFlip = false;
            }
            else if (OldRotation == 2)
            {
                Result.SwitchedAxes = false;
                Result.ResultXFlip = true;
                Result.ResultYFlip = true;
            }
            else if (OldRotation == 3)
            {
                Result.SwitchedAxes = true;
                Result.ResultXFlip = false;
                Result.ResultYFlip = true;
            }
            if (OldFlipX)
            {
                if (Result.SwitchedAxes)
                {
                    Result.ResultYFlip = !Result.ResultYFlip;
                }
                else
                {
                    Result.ResultXFlip = !Result.ResultXFlip;
                }
            }
            if (OldFlipZ)
            {
                if (Result.SwitchedAxes)
                {
                    Result.ResultXFlip = !Result.ResultXFlip;
                }
                else
                {
                    Result.ResultYFlip = !Result.ResultYFlip;
                }
            }
        }

        public static clsMap.clsTerrain.Tile.sTexture OrientateTile(ref clsPainter.clsTileList.sTileOrientationChance TileChance, sTileDirection NewDirection)
        {
            clsMap.clsTerrain.Tile.sTexture texture2;
            if (TileChance.TextureNum < 0)
            {
                texture2.Orientation.ResultXFlip = VBMath.Rnd() >= 0.5f;
                texture2.Orientation.ResultYFlip = VBMath.Rnd() >= 0.5f;
                texture2.Orientation.SwitchedAxes = VBMath.Rnd() >= 0.5f;
                texture2.TextureNum = -1;
                return texture2;
            }
            if ((((TileChance.Direction.X > 2) | (TileChance.Direction.Y > 2)) | (NewDirection.X > 2)) | (NewDirection.Y > 2))
            {
                Debugger.Break();
                return texture2;
            }
            if (((NewDirection.X == 1) ^ (NewDirection.Y == 1)) ^ ((TileChance.Direction.X == 1) ^ (TileChance.Direction.Y == 1)))
            {
                Debugger.Break();
                return texture2;
            }
            texture2.TextureNum = TileChance.TextureNum;
            if (((NewDirection.X == 1) & (NewDirection.Y == 1)) | ((TileChance.Direction.X == 1) & (TileChance.Direction.Y == 1)))
            {
                texture2.Orientation.SwitchedAxes = VBMath.Rnd() >= 0.5f;
                texture2.Orientation.ResultXFlip = VBMath.Rnd() >= 0.5f;
                texture2.Orientation.ResultYFlip = VBMath.Rnd() >= 0.5f;
                return texture2;
            }
            if ((NewDirection.X != 1) & (NewDirection.Y != 1))
            {
                texture2.Orientation.SwitchedAxes = false;
                if ((TileChance.Direction.X == 0) ^ (NewDirection.X == 0))
                {
                    texture2.Orientation.ResultXFlip = true;
                }
                else
                {
                    texture2.Orientation.ResultXFlip = false;
                }
                if ((TileChance.Direction.Y == 0) ^ (NewDirection.Y == 0))
                {
                    texture2.Orientation.ResultYFlip = true;
                }
                else
                {
                    texture2.Orientation.ResultYFlip = false;
                }
                if (VBMath.Rnd() >= 0.5f)
                {
                    texture2.Orientation.SwitchedAxes = !texture2.Orientation.SwitchedAxes;
                    if (((NewDirection.X == 0) ^ (NewDirection.Y == 0)) ^ (texture2.Orientation.ResultXFlip ^ texture2.Orientation.ResultYFlip))
                    {
                        texture2.Orientation.ResultXFlip = !texture2.Orientation.ResultXFlip;
                        texture2.Orientation.ResultYFlip = !texture2.Orientation.ResultYFlip;
                    }
                }
                return texture2;
            }
            texture2.Orientation.SwitchedAxes = (TileChance.Direction.X == 1) ^ (NewDirection.X == 1);
            if (texture2.Orientation.SwitchedAxes)
            {
                if (TileChance.Direction.Y != NewDirection.X)
                {
                    texture2.Orientation.ResultXFlip = true;
                }
                else
                {
                    texture2.Orientation.ResultXFlip = false;
                }
                if (TileChance.Direction.X != NewDirection.Y)
                {
                    texture2.Orientation.ResultYFlip = true;
                }
                else
                {
                    texture2.Orientation.ResultYFlip = false;
                }
            }
            else
            {
                if (TileChance.Direction.X != NewDirection.X)
                {
                    texture2.Orientation.ResultXFlip = true;
                }
                else
                {
                    texture2.Orientation.ResultXFlip = false;
                }
                if (TileChance.Direction.Y != NewDirection.Y)
                {
                    texture2.Orientation.ResultYFlip = true;
                }
                else
                {
                    texture2.Orientation.ResultYFlip = false;
                }
            }
            if (VBMath.Rnd() >= 0.5f)
            {
                if (NewDirection.X == 1)
                {
                    texture2.Orientation.ResultXFlip = !texture2.Orientation.ResultXFlip;
                    return texture2;
                }
                texture2.Orientation.ResultYFlip = !texture2.Orientation.ResultYFlip;
            }
            return texture2;
        }

        public static void RotateDirection(sTileDirection InitialDirection, sTileOrientation Orientation, ref sTileDirection ResultDirection)
        {
            ResultDirection = InitialDirection;
            if (Orientation.SwitchedAxes)
            {
                ResultDirection.SwitchAxes();
            }
            if (Orientation.ResultXFlip)
            {
                ResultDirection.FlipX();
            }
            if (Orientation.ResultYFlip)
            {
                ResultDirection.FlipY();
            }
        }

        public static void TileOrientation_To_OldOrientation(sTileOrientation TileOrientation, ref byte OutputRotation, ref bool OutputFlipX)
        {
            if (TileOrientation.SwitchedAxes)
            {
                if (TileOrientation.ResultXFlip)
                {
                    OutputRotation = 1;
                }
                else
                {
                    OutputRotation = 3;
                }
                OutputFlipX = !(TileOrientation.ResultXFlip ^ TileOrientation.ResultYFlip);
            }
            else
            {
                if (TileOrientation.ResultYFlip)
                {
                    OutputRotation = 2;
                }
                else
                {
                    OutputRotation = 0;
                }
                OutputFlipX = TileOrientation.ResultXFlip ^ TileOrientation.ResultYFlip;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sTileDirection
        {
            public byte X;
            public byte Y;
            public sTileDirection(byte NewX, byte NewY)
            {
                this = new TileOrientation.sTileDirection();
                this.X = NewX;
                this.Y = NewY;
            }

            public TileOrientation.sTileDirection GetRotated(TileOrientation.sTileOrientation Orientation)
            {
                TileOrientation.sTileDirection direction2;
                if (Orientation.SwitchedAxes)
                {
                    if (Orientation.ResultXFlip)
                    {
                        direction2.X = (byte) (2 - this.Y);
                    }
                    else
                    {
                        direction2.X = this.Y;
                    }
                    if (Orientation.ResultYFlip)
                    {
                        direction2.Y = (byte) (2 - this.X);
                        return direction2;
                    }
                    direction2.Y = this.X;
                    return direction2;
                }
                if (Orientation.ResultXFlip)
                {
                    direction2.X = (byte) (2 - this.X);
                }
                else
                {
                    direction2.X = this.X;
                }
                if (Orientation.ResultYFlip)
                {
                    direction2.Y = (byte) (2 - this.Y);
                    return direction2;
                }
                direction2.Y = this.Y;
                return direction2;
            }

            public void FlipX()
            {
                this.X = (byte) (2 - this.X);
            }

            public void FlipY()
            {
                this.Y = (byte) (2 - this.Y);
            }

            public void RotateClockwise()
            {
                byte x = this.X;
                this.X = (byte) (2 - this.Y);
                this.Y = x;
            }

            public void RotateAnticlockwise()
            {
                byte x = this.X;
                this.X = this.Y;
                this.Y = (byte) (2 - x);
            }

            public void SwitchAxes()
            {
                byte x = this.X;
                this.X = this.Y;
                this.Y = x;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sTileOrientation
        {
            public bool ResultXFlip;
            public bool ResultYFlip;
            public bool SwitchedAxes;
            public sTileOrientation(bool ResultXFlip, bool ResultZFlip, bool SwitchedAxes)
            {
                this = new TileOrientation.sTileOrientation();
                this.ResultXFlip = ResultXFlip;
                this.ResultYFlip = ResultZFlip;
                this.SwitchedAxes = SwitchedAxes;
            }

            public TileOrientation.sTileOrientation GetRotated(TileOrientation.sTileOrientation Orientation)
            {
                TileOrientation.sTileOrientation orientation2;
                orientation2.SwitchedAxes = this.SwitchedAxes ^ Orientation.SwitchedAxes;
                if (Orientation.SwitchedAxes)
                {
                    if (Orientation.ResultXFlip)
                    {
                        orientation2.ResultXFlip = !this.ResultYFlip;
                    }
                    else
                    {
                        orientation2.ResultXFlip = this.ResultYFlip;
                    }
                    if (Orientation.ResultYFlip)
                    {
                        orientation2.ResultYFlip = !this.ResultXFlip;
                        return orientation2;
                    }
                    orientation2.ResultYFlip = this.ResultXFlip;
                    return orientation2;
                }
                if (Orientation.ResultXFlip)
                {
                    orientation2.ResultXFlip = !this.ResultXFlip;
                }
                else
                {
                    orientation2.ResultXFlip = this.ResultXFlip;
                }
                if (Orientation.ResultYFlip)
                {
                    orientation2.ResultYFlip = !this.ResultYFlip;
                    return orientation2;
                }
                orientation2.ResultYFlip = this.ResultYFlip;
                return orientation2;
            }

            public void Reverse()
            {
                if (this.SwitchedAxes && (this.ResultXFlip ^ this.ResultYFlip))
                {
                    this.ResultXFlip = !this.ResultXFlip;
                    this.ResultYFlip = !this.ResultYFlip;
                }
            }

            public void RotateClockwise()
            {
                this.SwitchedAxes = !this.SwitchedAxes;
                if (this.ResultXFlip ^ this.ResultYFlip)
                {
                    this.ResultYFlip = !this.ResultYFlip;
                }
                else
                {
                    this.ResultXFlip = !this.ResultXFlip;
                }
            }

            public void RotateAnticlockwise()
            {
                this.SwitchedAxes = !this.SwitchedAxes;
                if (this.ResultXFlip ^ this.ResultYFlip)
                {
                    this.ResultXFlip = !this.ResultXFlip;
                }
                else
                {
                    this.ResultYFlip = !this.ResultYFlip;
                }
            }
        }
    }
}

