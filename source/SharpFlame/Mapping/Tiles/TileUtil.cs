#region

using System;
using System.Diagnostics;
using SharpFlame.Core.Domain;
using SharpFlame.Painters;

#endregion

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
            var returnResult = new clsTerrain.Tile.sTexture();

            //use random for empty tiles
            if ( tileChance.TextureNum < 0 )
            {
                returnResult.Orientation.ResultXFlip = App.Random.Next() >= 0.5F;
                returnResult.Orientation.ResultYFlip = App.Random.Next() >= 0.5F;
                returnResult.Orientation.SwitchedAxes = App.Random.Next() >= 0.5F;
                returnResult.TextureNum = -1;
                return returnResult;
            }
            //stop invalid numbers
            if ( tileChance.Direction.X > 2 | tileChance.Direction.Y > 2 | newDirection.X > 2 | newDirection.Y > 2 )
            {
                Debugger.Break();
                return returnResult;
            }
            //stop different direction types
            if ( (newDirection.X == 1 ^ newDirection.Y == 1) ^ (tileChance.Direction.X == 1 ^ tileChance.Direction.Y == 1) )
            {
                Debugger.Break();
                return returnResult;
            }

            returnResult.TextureNum = tileChance.TextureNum;

            //if a direction is neutral then give a random orientation
            if ( (newDirection.X == 1 & newDirection.Y == 1) || (tileChance.Direction.X == 1 & tileChance.Direction.Y == 1) )
            {
                returnResult.Orientation.SwitchedAxes = App.Random.Next() >= 0.5F;
                returnResult.Orientation.ResultXFlip = App.Random.Next() >= 0.5F;
                returnResult.Orientation.ResultYFlip = App.Random.Next() >= 0.5F;
                return returnResult;
            }

            bool isDiagonal = newDirection.X != 1 & newDirection.Y != 1;

            if ( isDiagonal )
            {
                returnResult.Orientation.SwitchedAxes = false;
                //use flips to match the directions
                if ( tileChance.Direction.X == 0 ^ newDirection.X == 0 )
                {
                    returnResult.Orientation.ResultXFlip = true;
                }
                else
                {
                    returnResult.Orientation.ResultXFlip = false;
                }
                if ( tileChance.Direction.Y == 0 ^ newDirection.Y == 0 )
                {
                    returnResult.Orientation.ResultYFlip = true;
                }
                else
                {
                    returnResult.Orientation.ResultYFlip = false;
                }
                //randomly switch to the alternate orientation
                if ( App.Random.Next() >= 0.5F )
                {
                    returnResult.Orientation.SwitchedAxes = !returnResult.Orientation.SwitchedAxes;
                    if ( (newDirection.X == 0 ^ newDirection.Y == 0) ^ (returnResult.Orientation.ResultXFlip ^ returnResult.Orientation.ResultYFlip) )
                    {
                        returnResult.Orientation.ResultXFlip = !returnResult.Orientation.ResultXFlip;
                        returnResult.Orientation.ResultYFlip = !returnResult.Orientation.ResultYFlip;
                    }
                }
            }
            else
            {
                //switch axes if the directions are on different axes
                returnResult.Orientation.SwitchedAxes = tileChance.Direction.X == 1 ^ newDirection.X == 1;
                //use a flip to match the directions
                if ( returnResult.Orientation.SwitchedAxes )
                {
                    if ( tileChance.Direction.Y != newDirection.X )
                    {
                        returnResult.Orientation.ResultXFlip = true;
                    }
                    else
                    {
                        returnResult.Orientation.ResultXFlip = false;
                    }
                    if ( tileChance.Direction.X != newDirection.Y )
                    {
                        returnResult.Orientation.ResultYFlip = true;
                    }
                    else
                    {
                        returnResult.Orientation.ResultYFlip = false;
                    }
                }
                else
                {
                    if ( tileChance.Direction.X != newDirection.X )
                    {
                        returnResult.Orientation.ResultXFlip = true;
                    }
                    else
                    {
                        returnResult.Orientation.ResultXFlip = false;
                    }
                    if ( tileChance.Direction.Y != newDirection.Y )
                    {
                        returnResult.Orientation.ResultYFlip = true;
                    }
                    else
                    {
                        returnResult.Orientation.ResultYFlip = false;
                    }
                }
                //randomly switch to the alternate orientation
                if ( App.Random.Next() >= 0.5F )
                {
                    if ( newDirection.X == 1 )
                    {
                        returnResult.Orientation.ResultXFlip = !returnResult.Orientation.ResultXFlip;
                    }
                    else
                    {
                        returnResult.Orientation.ResultYFlip = !returnResult.Orientation.ResultYFlip;
                    }
                }
            }

            return returnResult;
        }

        public static void RotateDirection(TileDirection initialDirection, TileOrientation orientation, ref TileDirection resultDirection)
        {
            resultDirection = initialDirection;
            if ( orientation.SwitchedAxes )
            {
                resultDirection.SwitchAxes();
            }
            if ( orientation.ResultXFlip )
            {
                resultDirection.FlipX();
            }
            if ( orientation.ResultYFlip )
            {
                resultDirection.FlipY();
            }
        }

        public static XYInt GetTileRotatedOffset(TileOrientation tileOrientation, XYInt pos)
        {
            var result = new XYInt();

            if ( tileOrientation.SwitchedAxes )
            {
                if ( tileOrientation.ResultXFlip )
                {
                    result.X = Constants.TerrainGridSpacing - pos.Y;
                }
                else
                {
                    result.X = pos.Y;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    result.Y = Constants.TerrainGridSpacing - pos.X;
                }
                else
                {
                    result.Y = pos.X;
                }
            }
            else
            {
                if ( tileOrientation.ResultXFlip )
                {
                    result.X = Constants.TerrainGridSpacing - pos.X;
                }
                else
                {
                    result.X = pos.X;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    result.Y = Constants.TerrainGridSpacing - pos.Y;
                }
                else
                {
                    result.Y = pos.Y;
                }
            }

            return result;
        }

        public static XYDouble GetTileRotatedPos_sng(TileOrientation tileOrientation, XYDouble pos)
        {
            var returnResult = new XYDouble();

            if ( tileOrientation.SwitchedAxes )
            {
                if ( tileOrientation.ResultXFlip )
                {
                    returnResult.X = 1.0F - pos.Y;
                }
                else
                {
                    returnResult.X = pos.Y;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    returnResult.Y = 1.0F - pos.X;
                }
                else
                {
                    returnResult.Y = pos.X;
                }
            }
            else
            {
                if ( tileOrientation.ResultXFlip )
                {
                    returnResult.X = 1.0F - pos.X;
                }
                else
                {
                    returnResult.X = pos.X;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    returnResult.Y = 1.0F - pos.Y;
                }
                else
                {
                    returnResult.Y = pos.Y;
                }
            }

            return returnResult;
        }

        public static XYDouble GetTileRotatedPos_dbl(TileOrientation tileOrientation, XYDouble pos)
        {
            var returnResult = default(XYDouble);

            if ( tileOrientation.SwitchedAxes )
            {
                if ( tileOrientation.ResultXFlip )
                {
                    returnResult.X = 1.0D - pos.Y;
                }
                else
                {
                    returnResult.X = pos.Y;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    returnResult.Y = 1.0D - pos.X;
                }
                else
                {
                    returnResult.Y = pos.X;
                }
            }
            else
            {
                if ( tileOrientation.ResultXFlip )
                {
                    returnResult.X = 1.0D - pos.X;
                }
                else
                {
                    returnResult.X = pos.X;
                }
                if ( tileOrientation.ResultYFlip )
                {
                    returnResult.Y = 1.0D - pos.Y;
                }
                else
                {
                    returnResult.Y = pos.Y;
                }
            }

            return returnResult;
        }

        public static XYInt GetRotatedPos(TileOrientation orientation, XYInt pos, XYInt limits)
        {
            var result = new XYInt();

            if ( orientation.SwitchedAxes )
            {
                if ( orientation.ResultXFlip )
                {
                    result.X = limits.Y - pos.Y;
                }
                else
                {
                    result.X = pos.Y;
                }
                if ( orientation.ResultYFlip )
                {
                    result.Y = limits.X - pos.X;
                }
                else
                {
                    result.Y = pos.X;
                }
            }
            else
            {
                if ( orientation.ResultXFlip )
                {
                    result.X = limits.X - pos.X;
                }
                else
                {
                    result.X = pos.X;
                }
                if ( orientation.ResultYFlip )
                {
                    result.Y = limits.Y - pos.Y;
                }
                else
                {
                    result.Y = pos.Y;
                }
            }

            return result;
        }

        public static double GetRotatedAngle(TileOrientation orientation, double angle)
        {
            var xyDbl = GetTileRotatedPos_dbl(orientation, new XYDouble((Math.Cos(angle) + 1.0D) / 2.0D, (Math.Sin(angle) + 1.0D) / 2.0D));
            xyDbl.X = xyDbl.X * 2.0D - 1.0D;
            xyDbl.Y = xyDbl.Y * 2.0D - 1.0D;
            return Math.Atan2(xyDbl.Y, xyDbl.X);
        }

        public static void GetTileRotatedTexCoords(TileOrientation tileOrientation, ref XYDouble coordA, ref XYDouble coordB,
            ref XYDouble coordC, ref XYDouble coordD)
        {
            var reverseOrientation = new TileOrientation();

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
                    OutputRotation = 1;
                }
                else
                {
                    OutputRotation = 3;
                }
                OutputFlipX = !(tileOrientation.ResultXFlip ^ tileOrientation.ResultYFlip);
            }
            else
            {
                if ( tileOrientation.ResultYFlip )
                {
                    OutputRotation = 2;
                }
                else
                {
                    OutputRotation = 0;
                }
                OutputFlipX = tileOrientation.ResultXFlip ^ tileOrientation.ResultYFlip;
            }
        }

        public static void OldOrientation_To_TileOrientation(byte oldRotation, bool oldFlipX, bool oldFlipZ, ref TileOrientation result)
        {
            if ( oldRotation == 0 )
            {
                result.SwitchedAxes = false;
                result.ResultXFlip = false;
                result.ResultYFlip = false;
            }
            else if ( oldRotation == 1 )
            {
                result.SwitchedAxes = true;
                result.ResultXFlip = true;
                result.ResultYFlip = false;
            }
            else if ( oldRotation == 2 )
            {
                result.SwitchedAxes = false;
                result.ResultXFlip = true;
                result.ResultYFlip = true;
            }
            else if ( oldRotation == 3 )
            {
                result.SwitchedAxes = true;
                result.ResultXFlip = false;
                result.ResultYFlip = true;
            }
            if ( oldFlipX )
            {
                if ( result.SwitchedAxes )
                {
                    result.ResultYFlip = !result.ResultYFlip;
                }
                else
                {
                    result.ResultXFlip = !result.ResultXFlip;
                }
            }
            if ( oldFlipZ )
            {
                if ( result.SwitchedAxes )
                {
                    result.ResultXFlip = !result.ResultXFlip;
                }
                else
                {
                    result.ResultYFlip = !result.ResultYFlip;
                }
            }
        }

        public static bool IdenticalTileDirections(TileDirection tileOrientationA, TileDirection tileOrientationB)
        {
            return tileOrientationA.X == tileOrientationB.X & tileOrientationA.Y == tileOrientationB.Y;
        }

        public static bool DirectionsOnSameSide(TileDirection directionA, TileDirection directionB)
        {
            if ( directionA.X == 0 )
            {
                if ( directionB.X == 0 )
                {
                    return true;
                }
            }
            if ( directionA.X == 2 )
            {
                if ( directionB.X == 2 )
                {
                    return true;
                }
            }
            if ( directionA.Y == 0 )
            {
                if ( directionB.Y == 0 )
                {
                    return true;
                }
            }
            if ( directionA.Y == 2 )
            {
                if ( directionB.Y == 2 )
                {
                    return true;
                }
            }
            return false;
        }

        public static bool DirectionsAreInLine(TileDirection directionA, TileDirection directionB)
        {
            if ( directionA.X == directionB.X )
            {
                return true;
            }
            if ( directionA.Y == directionB.Y )
            {
                return true;
            }
            return false;
        }
    }
}