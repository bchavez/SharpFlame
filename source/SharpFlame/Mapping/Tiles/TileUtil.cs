using System;
using System.Diagnostics;
using SharpFlame.Maths;
using SharpFlame.Painters;

namespace SharpFlame.Mapping.Tiles
{
    public sealed class TileUtil
    {
        public static TileOrientation Clockwise = new TileOrientation(true, false, true);
        public static TileOrientation CounterClockwise = new TileOrientation(false, true, true);
        public static TileOrientation FlipX = new TileOrientation(true, false, false);
        public static TileOrientation FlipY = new TileOrientation(false, true, false);

        public static TileDirection TopLeft = new TileDirection(0, 0);
        public static TileDirection Top = new TileDirection(1, 0);
        public static TileDirection TopRight = new TileDirection(2, 0);
        public static TileDirection Right = new TileDirection(2, 1);
        public static TileDirection BottomRight = new TileDirection(2, 2);
        public static TileDirection Bottom = new TileDirection(1, 2);
        public static TileDirection BottomLeft = new TileDirection(0, 2);
        public static TileDirection Left = new TileDirection(0, 1);
        public static TileDirection None = new TileDirection(1, 1);

        public static clsTerrain.Tile.sTexture OrientateTile(ref TileOrientationChance tileChance, TileDirection newDirection)
        {
            clsTerrain.Tile.sTexture ReturnResult = new clsTerrain.Tile.sTexture();

            //use random for empty tiles
            if ( tileChance.TextureNum < 0 )
            {
                ReturnResult.Orientation.ResultXFlip = App.Random.Next() >= 0.5F;
                ReturnResult.Orientation.ResultYFlip = App.Random.Next() >= 0.5F;
                ReturnResult.Orientation.SwitchedAxes = App.Random.Next() >= 0.5F;
                ReturnResult.TextureNum = -1;
                return ReturnResult;
            }
            //stop invalid numbers
            if ( tileChance.Direction.X > 2 | tileChance.Direction.Y > 2 | newDirection.X > 2 | newDirection.Y > 2 )
            {
                Debugger.Break();
                return ReturnResult;
            }
            //stop different direction types
            if ( (newDirection.X == 1 ^ newDirection.Y == 1) ^ (tileChance.Direction.X == 1 ^ tileChance.Direction.Y == 1) )
            {
                Debugger.Break();
                return ReturnResult;
            }

            ReturnResult.TextureNum = tileChance.TextureNum;

            //if a direction is neutral then give a random orientation
            if ( (newDirection.X == 1 & newDirection.Y == 1) || (tileChance.Direction.X == 1 & tileChance.Direction.Y == 1) )
            {
                ReturnResult.Orientation.SwitchedAxes = App.Random.Next() >= 0.5F;
                ReturnResult.Orientation.ResultXFlip = App.Random.Next() >= 0.5F;
                ReturnResult.Orientation.ResultYFlip = App.Random.Next() >= 0.5F;
                return ReturnResult;
            }

            bool IsDiagonal = default(bool);

            IsDiagonal = newDirection.X != 1 & newDirection.Y != 1;
            if ( IsDiagonal )
            {
                ReturnResult.Orientation.SwitchedAxes = false;
                //use flips to match the directions
                if ( tileChance.Direction.X == 0 ^ newDirection.X == 0 )
                {
                    ReturnResult.Orientation.ResultXFlip = true;
                }
                else
                {
                    ReturnResult.Orientation.ResultXFlip = false;
                }
                if ( tileChance.Direction.Y == 0 ^ newDirection.Y == 0 )
                {
                    ReturnResult.Orientation.ResultYFlip = true;
                }
                else
                {
                    ReturnResult.Orientation.ResultYFlip = false;
                }
                //randomly switch to the alternate orientation
                if ( App.Random.Next() >= 0.5F )
                {
                    ReturnResult.Orientation.SwitchedAxes = !ReturnResult.Orientation.SwitchedAxes;
                    if ( (newDirection.X == 0 ^ newDirection.Y == 0) ^ (ReturnResult.Orientation.ResultXFlip ^ ReturnResult.Orientation.ResultYFlip) )
                    {
                        ReturnResult.Orientation.ResultXFlip = !ReturnResult.Orientation.ResultXFlip;
                        ReturnResult.Orientation.ResultYFlip = !ReturnResult.Orientation.ResultYFlip;
                    }
                }
            }
            else
            {
                //switch axes if the directions are on different axes
                ReturnResult.Orientation.SwitchedAxes = tileChance.Direction.X == 1 ^ newDirection.X == 1;
                //use a flip to match the directions
                if ( ReturnResult.Orientation.SwitchedAxes )
                {
                    if ( tileChance.Direction.Y != newDirection.X )
                    {
                        ReturnResult.Orientation.ResultXFlip = true;
                    }
                    else
                    {
                        ReturnResult.Orientation.ResultXFlip = false;
                    }
                    if ( tileChance.Direction.X != newDirection.Y )
                    {
                        ReturnResult.Orientation.ResultYFlip = true;
                    }
                    else
                    {
                        ReturnResult.Orientation.ResultYFlip = false;
                    }
                }
                else
                {
                    if ( tileChance.Direction.X != newDirection.X )
                    {
                        ReturnResult.Orientation.ResultXFlip = true;
                    }
                    else
                    {
                        ReturnResult.Orientation.ResultXFlip = false;
                    }
                    if ( tileChance.Direction.Y != newDirection.Y )
                    {
                        ReturnResult.Orientation.ResultYFlip = true;
                    }
                    else
                    {
                        ReturnResult.Orientation.ResultYFlip = false;
                    }
                }
                //randomly switch to the alternate orientation
                if ( App.Random.Next() >= 0.5F )
                {
                    if ( newDirection.X == 1 )
                    {
                        ReturnResult.Orientation.ResultXFlip = !ReturnResult.Orientation.ResultXFlip;
                    }
                    else
                    {
                        ReturnResult.Orientation.ResultYFlip = !ReturnResult.Orientation.ResultYFlip;
                    }
                }
            }

            return ReturnResult;
        }

        public static void RotateDirection(TileDirection initialDirection, TileOrientation orientation, ref TileDirection ResultDirection)
        {
            ResultDirection = initialDirection;
            if ( orientation.SwitchedAxes )
            {
                ResultDirection.SwitchAxes();
            }
            if ( orientation.ResultXFlip )
            {
                ResultDirection.FlipX();
            }
            if ( orientation.ResultYFlip )
            {
                ResultDirection.FlipY();
            }
        }

        public static sXY_int GetTileRotatedOffset(TileOrientation tileOrientation, sXY_int pos)
        {
            sXY_int Result = new sXY_int();

            if ( tileOrientation.SwitchedAxes )
            {
                if ( tileOrientation.ResultXFlip )
                {
                    Result.X = App.TerrainGridSpacing - pos.Y;
                }
                else
                {
                    Result.X = pos.Y;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    Result.Y = App.TerrainGridSpacing - pos.X;
                }
                else
                {
                    Result.Y = pos.X;
                }
            }
            else
            {
                if ( tileOrientation.ResultXFlip )
                {
                    Result.X = App.TerrainGridSpacing - pos.X;
                }
                else
                {
                    Result.X = pos.X;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    Result.Y = App.TerrainGridSpacing - pos.Y;
                }
                else
                {
                    Result.Y = pos.Y;
                }
            }

            return Result;
        }

        public static Position.XY_dbl GetTileRotatedPos_sng(TileOrientation tileOrientation, Position.XY_dbl pos)
        {
            Position.XY_dbl ReturnResult = new Position.XY_dbl();

            if ( tileOrientation.SwitchedAxes )
            {
                if ( tileOrientation.ResultXFlip )
                {
                    ReturnResult.X = 1.0F - pos.Y;
                }
                else
                {
                    ReturnResult.X = pos.Y;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    ReturnResult.Y = 1.0F - pos.X;
                }
                else
                {
                    ReturnResult.Y = pos.X;
                }
            }
            else
            {
                if ( tileOrientation.ResultXFlip )
                {
                    ReturnResult.X = 1.0F - pos.X;
                }
                else
                {
                    ReturnResult.X = pos.X;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    ReturnResult.Y = 1.0F - pos.Y;
                }
                else
                {
                    ReturnResult.Y = pos.Y;
                }
            }

            return ReturnResult;
        }

        public static Position.XY_dbl GetTileRotatedPos_dbl(TileOrientation tileOrientation, Position.XY_dbl pos)
        {
            Position.XY_dbl ReturnResult = default(Position.XY_dbl);

            if ( tileOrientation.SwitchedAxes )
            {
                if ( tileOrientation.ResultXFlip )
                {
                    ReturnResult.X = 1.0D - pos.Y;
                }
                else
                {
                    ReturnResult.X = pos.Y;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    ReturnResult.Y = 1.0D - pos.X;
                }
                else
                {
                    ReturnResult.Y = pos.X;
                }
            }
            else
            {
                if ( tileOrientation.ResultXFlip )
                {
                    ReturnResult.X = 1.0D - pos.X;
                }
                else
                {
                    ReturnResult.X = pos.X;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    ReturnResult.Y = 1.0D - pos.Y;
                }
                else
                {
                    ReturnResult.Y = pos.Y;
                }
            }

            return ReturnResult;
        }

        public static sXY_int GetRotatedPos(TileOrientation orientation, sXY_int pos, sXY_int limits)
        {
            sXY_int Result = new sXY_int();

            if ( orientation.SwitchedAxes )
            {
                if ( orientation.ResultXFlip )
                {
                    Result.X = limits.Y - pos.Y;
                }
                else
                {
                    Result.X = pos.Y;
                }
                if ( orientation.ResultYFlip )
                {
                    Result.Y = limits.X - pos.X;
                }
                else
                {
                    Result.Y = pos.X;
                }
            }
            else
            {
                if ( orientation.ResultXFlip )
                {
                    Result.X = limits.X - pos.X;
                }
                else
                {
                    Result.X = pos.X;
                }
                if ( orientation.ResultYFlip )
                {
                    Result.Y = limits.Y - pos.Y;
                }
                else
                {
                    Result.Y = pos.Y;
                }
            }

            return Result;
        }

        public static double GetRotatedAngle(TileOrientation orientation, double angle)
        {
            Position.XY_dbl XY_dbl = default(Position.XY_dbl);

            XY_dbl = GetTileRotatedPos_dbl(orientation, new Position.XY_dbl((Math.Cos(angle) + 1.0D) / 2.0D, (Math.Sin(angle) + 1.0D) / 2.0D));
            XY_dbl.X = XY_dbl.X * 2.0D - 1.0D;
            XY_dbl.Y = XY_dbl.Y * 2.0D - 1.0D;
            return Math.Atan2(XY_dbl.Y, XY_dbl.X);
        }

        public static void GetTileRotatedTexCoords(TileOrientation tileOrientation, ref Position.XY_dbl coordA, ref Position.XY_dbl coordB, 
            ref Position.XY_dbl coordC, ref Position.XY_dbl coordD)
        {
            TileOrientation reverseOrientation = new TileOrientation();

            reverseOrientation = tileOrientation;
            reverseOrientation.Reverse();

            if ( reverseOrientation.SwitchedAxes )
            {
                if ( reverseOrientation.ResultXFlip )
                {
                    coordA.X = 1f;
                    coordB.X = 1f;
                    coordC.X = 0f;
                    coordD.X = 0f;
                }
                else
                {
                    coordA.X = 0f;
                    coordB.X = 0f;
                    coordC.X = 1f;
                    coordD.X = 1f;
                }
                if ( reverseOrientation.ResultYFlip )
                {
                    coordA.Y = 1f;
                    coordB.Y = 0f;
                    coordC.Y = 1f;
                    coordD.Y = 0f;
                }
                else
                {
                    coordA.Y = 0f;
                    coordB.Y = 1f;
                    coordC.Y = 0f;
                    coordD.Y = 1f;
                }
            }
            else
            {
                if ( reverseOrientation.ResultXFlip )
                {
                    coordA.X = 1f;
                    coordB.X = 0f;
                    coordC.X = 1f;
                    coordD.X = 0f;
                }
                else
                {
                    coordA.X = 0f;
                    coordB.X = 1F;
                    coordC.X = 0f;
                    coordD.X = 1f;
                }
                if ( reverseOrientation.ResultYFlip )
                {
                    coordA.Y = 1f;
                    coordB.Y = 1f;
                    coordC.Y = 0f;
                    coordD.Y = 0f;
                }
                else
                {
                    coordA.Y = 0f;
                    coordB.Y = 0f;
                    coordC.Y = 1f;
                    coordD.Y = 1f;
                }
            }
        }

        public static void TileOrientation_To_OldOrientation(TileOrientation tileOrientation, ref byte OutputRotation, ref bool OutputFlipX)
        {
            if ( tileOrientation.SwitchedAxes )
            {
                if ( tileOrientation.ResultXFlip )
                {
                    OutputRotation = (byte)1;
                }
                else
                {
                    OutputRotation = (byte)3;
                }
                OutputFlipX = !(tileOrientation.ResultXFlip ^ tileOrientation.ResultYFlip);
            }
            else
            {
                if ( tileOrientation.ResultYFlip )
                {
                    OutputRotation = (byte)2;
                }
                else
                {
                    OutputRotation = (byte)0;
                }
                OutputFlipX = tileOrientation.ResultXFlip ^ tileOrientation.ResultYFlip;
            }
        }

        public static void OldOrientation_To_TileOrientation(byte OldRotation, bool OldFlipX, bool OldFlipZ, ref TileOrientation Result)
        {
            if ( OldRotation == 0 )
            {
                Result.SwitchedAxes = false;
                Result.ResultXFlip = false;
                Result.ResultYFlip = false;
            }
            else if ( OldRotation == 1 )
            {
                Result.SwitchedAxes = true;
                Result.ResultXFlip = true;
                Result.ResultYFlip = false;
            }
            else if ( OldRotation == 2 )
            {
                Result.SwitchedAxes = false;
                Result.ResultXFlip = true;
                Result.ResultYFlip = true;
            }
            else if ( OldRotation == 3 )
            {
                Result.SwitchedAxes = true;
                Result.ResultXFlip = false;
                Result.ResultYFlip = true;
            }
            if ( OldFlipX )
            {
                if ( Result.SwitchedAxes )
                {
                    Result.ResultYFlip = !Result.ResultYFlip;
                }
                else
                {
                    Result.ResultXFlip = !Result.ResultXFlip;
                }
            }
            if ( OldFlipZ )
            {
                if ( Result.SwitchedAxes )
                {
                    Result.ResultXFlip = !Result.ResultXFlip;
                }
                else
                {
                    Result.ResultYFlip = !Result.ResultYFlip;
                }
            }
        }

        public static bool IdenticalTileDirections(TileDirection TileOrientationA, TileDirection TileOrientationB)
        {
            return TileOrientationA.X == TileOrientationB.X & TileOrientationA.Y == TileOrientationB.Y;
        }

        public static bool DirectionsOnSameSide(TileDirection DirectionA, TileDirection DirectionB)
        {
            if ( DirectionA.X == 0 )
            {
                if ( DirectionB.X == 0 )
                {
                    return true;
                }
            }
            if ( DirectionA.X == 2 )
            {
                if ( DirectionB.X == 2 )
                {
                    return true;
                }
            }
            if ( DirectionA.Y == 0 )
            {
                if ( DirectionB.Y == 0 )
                {
                    return true;
                }
            }
            if ( DirectionA.Y == 2 )
            {
                if ( DirectionB.Y == 2 )
                {
                    return true;
                }
            }
            return false;
        }

        public static bool DirectionsAreInLine(TileDirection DirectionA, TileDirection DirectionB)
        {
            if ( DirectionA.X == DirectionB.X )
            {
                return true;
            }
            if ( DirectionA.Y == DirectionB.Y )
            {
                return true;
            }
            return false;
        }
    }
}