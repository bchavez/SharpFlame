

using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Painters;


namespace SharpFlame.Mapping
{
    public class clsUpdateAutotexture : clsAction
    {
        public bool MakeInvalidTiles;
        private Painter painter;
        private TileDirection resultDirection;
        private TileOrientationChance resultTexture;
        private TileList resultTiles;

        private Road road;
        private bool roadBottom;
        private bool roadLeft;
        private bool roadRight;
        private bool roadTop;
        private clsTerrain terrain;
        private Terrain terrainInner;
        private Terrain terrainOuter;

        public override void ActionPerform()
        {
            terrain = Map.Terrain;

            painter = Map.Painter;

            resultTiles = null;
            resultDirection = TileUtil.None;

            //apply centre brushes
            if ( !terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff )
            {
                for ( var brushNum = 0; brushNum <= painter.TerrainCount - 1; brushNum++ )
                {
                    terrainInner = painter.Terrains[brushNum];
                    if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner )
                    {
                        if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner )
                        {
                            if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                            {
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    //i i i i
                                    resultTiles = terrainInner.Tiles;
                                    resultDirection = TileUtil.None;
                                }
                            }
                        }
                    }
                }
            }

            //apply transition brushes
            if ( !terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff )
            {
                for ( var brushNum = 0; brushNum <= painter.TransitionBrushCount - 1; brushNum++ )
                {
                    terrainInner = painter.TransitionBrushes[brushNum].TerrainInner;
                    terrainOuter = painter.TransitionBrushes[brushNum].TerrainOuter;
                    if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner )
                    {
                        if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner )
                        {
                            if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                            {
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    //i i i i
                                    //nothing to do here
                                    break;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    //i i i o
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesCornerIn;
                                    resultDirection = TileUtil.BottomRight;
                                    break;
                                }
                            }
                            else if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter )
                            {
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    //i i o i
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesCornerIn;
                                    resultDirection = TileUtil.BottomLeft;
                                    break;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    //i i o o
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesStraight;
                                    resultDirection = TileUtil.Bottom;
                                    break;
                                }
                            }
                        }
                        else if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter )
                        {
                            if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                            {
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    //i o i i
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesCornerIn;
                                    resultDirection = TileUtil.TopRight;
                                    break;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    //i o i o
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesStraight;
                                    resultDirection = TileUtil.Right;
                                    break;
                                }
                            }
                            else if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter )
                            {
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    //i o o i
                                    resultTiles = null;
                                    resultDirection = TileUtil.None;
                                    break;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    //i o o o
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesCornerOut;
                                    resultDirection = TileUtil.BottomRight;
                                    break;
                                }
                            }
                        }
                    }
                    else if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter )
                    {
                        if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner )
                        {
                            if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                            {
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    //o i i i
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesCornerIn;
                                    resultDirection = TileUtil.TopLeft;
                                    break;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    //o i i o
                                    resultTiles = null;
                                    resultDirection = TileUtil.None;
                                    break;
                                }
                            }
                            else if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter )
                            {
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    //o i o i
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesStraight;
                                    resultDirection = TileUtil.Left;
                                    break;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    //o i o o
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesCornerOut;
                                    resultDirection = TileUtil.BottomLeft;
                                    break;
                                }
                            }
                        }
                        else if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter )
                        {
                            if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                            {
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    //o o i i
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesStraight;
                                    resultDirection = TileUtil.Top;
                                    break;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    //o o i o
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesCornerOut;
                                    resultDirection = TileUtil.TopRight;
                                    break;
                                }
                            }
                            else if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter )
                            {
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    //o o o i
                                    resultTiles = painter.TransitionBrushes[brushNum].TilesCornerOut;
                                    resultDirection = TileUtil.TopLeft;
                                    break;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    //o o o o
                                    //nothing to do here
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //set cliff tiles
            if ( terrain.Tiles[PosNum.X, PosNum.Y].Tri )
            {
                if ( terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff )
                {
                    if ( terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                    {
                        var brushNum = 0;
                        for ( brushNum = 0; brushNum <= painter.CliffBrushCount - 1; brushNum++ )
                        {
                            terrainInner = painter.CliffBrushes[brushNum].Terrain_Inner;
                            terrainOuter = painter.CliffBrushes[brushNum].Terrain_Outer;
                            if ( terrainInner == terrainOuter )
                            {
                                var a = 0;
                                if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( a >= 3 )
                                {
                                    resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                    resultDirection = terrain.Tiles[PosNum.X, PosNum.Y].DownSide;
                                    break;
                                }
                            }
                            if ( ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner && terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner) &&
                                  (terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter)) ||
                                 ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner || terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner) &&
                                  (terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter)) )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                resultDirection = TileUtil.Bottom;
                                break;
                            }
                            if ( ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter &&
                                   terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter) &&
                                  (terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner)) ||
                                 ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter ||
                                   terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter) &&
                                  (terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner)) )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                resultDirection = TileUtil.Left;
                                break;
                            }
                            if ( ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter) &&
                                  (terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner)) ||
                                 ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter) &&
                                  (terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner)) )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                resultDirection = TileUtil.Top;
                                break;
                            }
                            if ( ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner &&
                                   terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner) &&
                                  (terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter)) ||
                                 ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner ||
                                   terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner) &&
                                  (terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter)) )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                resultDirection = TileUtil.Right;
                                break;
                            }
                        }
                        if ( brushNum == painter.CliffBrushCount )
                        {
                            resultTiles = null;
                            resultDirection = TileUtil.None;
                        }
                    }
                    else
                    {
                        var brushNum = 0;
                        for ( brushNum = 0; brushNum <= painter.CliffBrushCount - 1; brushNum++ )
                        {
                            terrainInner = painter.CliffBrushes[brushNum].Terrain_Inner;
                            terrainOuter = painter.CliffBrushes[brushNum].Terrain_Outer;
                            if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter )
                            {
                                var a = 0;
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( a >= 2 )
                                {
                                    resultTiles = painter.CliffBrushes[brushNum].Tiles_Corner_In;
                                    resultDirection = TileUtil.TopLeft;
                                    break;
                                }
                            }
                            else if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner )
                            {
                                var A = 0;
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter )
                                {
                                    A++;
                                }
                                if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    A++;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    A++;
                                }
                                if ( A >= 2 )
                                {
                                    resultTiles = painter.CliffBrushes[brushNum].Tiles_Corner_Out;
                                    resultDirection = TileUtil.BottomRight;
                                    break;
                                }
                            }
                        }
                        if ( brushNum == painter.CliffBrushCount )
                        {
                            resultTiles = null;
                            resultDirection = TileUtil.None;
                        }
                    }
                }
                else if ( terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                {
                    var brushNum = 0;
                    for ( brushNum = 0; brushNum <= painter.CliffBrushCount - 1; brushNum++ )
                    {
                        terrainInner = painter.CliffBrushes[brushNum].Terrain_Inner;
                        terrainOuter = painter.CliffBrushes[brushNum].Terrain_Outer;
                        if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                        {
                            var a = 0;
                            if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner )
                            {
                                a++;
                            }
                            if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner )
                            {
                                a++;
                            }
                            if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                            {
                                a++;
                            }
                            if ( a >= 2 )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Corner_In;
                                resultDirection = TileUtil.BottomRight;
                                break;
                            }
                        }
                        else if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                        {
                            var a = 0;
                            if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter )
                            {
                                a++;
                            }
                            if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter )
                            {
                                a++;
                            }
                            if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter )
                            {
                                a++;
                            }
                            if ( a >= 2 )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Corner_Out;
                                resultDirection = TileUtil.TopLeft;
                                break;
                            }
                        }
                    }
                    if ( brushNum == painter.CliffBrushCount )
                    {
                        resultTiles = null;
                        resultDirection = TileUtil.None;
                    }
                }
            }
            else
            {
                //default tri orientation
                if ( terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                {
                    if ( terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                    {
                        var brushNum = 0;
                        for ( brushNum = 0; brushNum <= painter.CliffBrushCount - 1; brushNum++ )
                        {
                            terrainInner = painter.CliffBrushes[brushNum].Terrain_Inner;
                            terrainOuter = painter.CliffBrushes[brushNum].Terrain_Outer;
                            if ( terrainInner == terrainOuter )
                            {
                                var a = 0;
                                if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( a >= 3 )
                                {
                                    resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                    resultDirection = terrain.Tiles[PosNum.X, PosNum.Y].DownSide;
                                    break;
                                }
                            }
                            if ( ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner && terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner) &&
                                  (terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter)) ||
                                 ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner || terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner) &&
                                  (terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter)) )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                resultDirection = TileUtil.Bottom;
                                break;
                            }
                            if ( ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter &&
                                   terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter) &&
                                  (terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner)) ||
                                 ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter ||
                                   terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter) &&
                                  (terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner)) )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                resultDirection = TileUtil.Left;
                                break;
                            }
                            if ( ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter) &&
                                  (terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner)) ||
                                 ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter) &&
                                  (terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner)) )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                resultDirection = TileUtil.Top;
                                break;
                            }
                            if ( ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner &&
                                   terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner) &&
                                  (terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter ||
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter)) ||
                                 ((terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner ||
                                   terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner) &&
                                  (terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter &&
                                   terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter)) )
                            {
                                resultTiles = painter.CliffBrushes[brushNum].Tiles_Straight;
                                resultDirection = TileUtil.Right;
                                break;
                            }
                        }
                        if ( brushNum == painter.CliffBrushCount )
                        {
                            resultTiles = null;
                            resultDirection = TileUtil.None;
                        }
                    }
                    else
                    {
                        var brushNum = 0;
                        for ( brushNum = 0; brushNum <= painter.CliffBrushCount - 1; brushNum++ )
                        {
                            terrainInner = painter.CliffBrushes[brushNum].Terrain_Inner;
                            terrainOuter = painter.CliffBrushes[brushNum].Terrain_Outer;
                            if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter )
                            {
                                var a = 0;
                                if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                                {
                                    a++;
                                }
                                if ( a >= 2 )
                                {
                                    resultTiles = painter.CliffBrushes[brushNum].Tiles_Corner_In;
                                    resultDirection = TileUtil.TopRight;
                                    break;
                                }
                            }
                            else if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner )
                            {
                                var a = 0;
                                if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    a++;
                                }
                                if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                                {
                                    a++;
                                }
                                if ( a >= 2 )
                                {
                                    resultTiles = painter.CliffBrushes[brushNum].Tiles_Corner_Out;
                                    resultDirection = TileUtil.BottomLeft;
                                    break;
                                }
                            }
                        }
                        if ( brushNum == painter.CliffBrushCount )
                        {
                            resultTiles = null;
                            resultDirection = TileUtil.None;
                        }
                    }
                }
                else if ( terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                {
                    var BrushNum = 0;
                    for ( BrushNum = 0; BrushNum <= painter.CliffBrushCount - 1; BrushNum++ )
                    {
                        terrainInner = painter.CliffBrushes[BrushNum].Terrain_Inner;
                        terrainOuter = painter.CliffBrushes[BrushNum].Terrain_Outer;
                        if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter )
                        {
                            var A = 0;
                            if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainInner )
                            {
                                A++;
                            }
                            if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainInner )
                            {
                                A++;
                            }
                            if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainInner )
                            {
                                A++;
                            }
                            if ( A >= 2 )
                            {
                                resultTiles = painter.CliffBrushes[BrushNum].Tiles_Corner_In;
                                resultDirection = TileUtil.BottomLeft;
                                break;
                            }
                        }
                        else if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainInner )
                        {
                            var A = 0;
                            if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter )
                            {
                                A++;
                            }
                            if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter )
                            {
                                A++;
                            }
                            if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                            {
                                A++;
                            }
                            if ( A >= 2 )
                            {
                                resultTiles = painter.CliffBrushes[BrushNum].Tiles_Corner_Out;
                                resultDirection = TileUtil.TopRight;
                                break;
                            }
                        }
                    }
                    if ( BrushNum == painter.CliffBrushCount )
                    {
                        resultTiles = null;
                        resultDirection = TileUtil.None;
                    }
                }
            }

            //apply roads
            road = null;
            if ( terrain.SideH[PosNum.X, PosNum.Y].Road != null )
            {
                road = terrain.SideH[PosNum.X, PosNum.Y].Road;
            }
            else if ( terrain.SideH[PosNum.X, PosNum.Y + 1].Road != null )
            {
                road = terrain.SideH[PosNum.X, PosNum.Y + 1].Road;
            }
            else if ( terrain.SideV[PosNum.X + 1, PosNum.Y].Road != null )
            {
                road = terrain.SideV[PosNum.X + 1, PosNum.Y].Road;
            }
            else if ( terrain.SideV[PosNum.X, PosNum.Y].Road != null )
            {
                road = terrain.SideV[PosNum.X, PosNum.Y].Road;
            }
            if ( road != null )
            {
                var BrushNum = 0;
                for ( BrushNum = 0; BrushNum <= painter.RoadBrushCount - 1; BrushNum++ )
                {
                    if ( painter.RoadBrushes[BrushNum].Road == road )
                    {
                        terrainOuter = painter.RoadBrushes[BrushNum].Terrain;
                        var A = 0;
                        if ( terrain.Vertices[PosNum.X, PosNum.Y].Terrain == terrainOuter )
                        {
                            A++;
                        }
                        if ( terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == terrainOuter )
                        {
                            A++;
                        }
                        if ( terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == terrainOuter )
                        {
                            A++;
                        }
                        if ( terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == terrainOuter )
                        {
                            A++;
                        }
                        if ( A >= 2 )
                        {
                            break;
                        }
                    }
                }

                resultTiles = null;
                resultDirection = TileUtil.None;

                if ( BrushNum < painter.RoadBrushCount )
                {
                    roadTop = terrain.SideH[PosNum.X, PosNum.Y].Road == road;
                    roadLeft = terrain.SideV[PosNum.X, PosNum.Y].Road == road;
                    roadRight = terrain.SideV[PosNum.X + 1, PosNum.Y].Road == road;
                    roadBottom = terrain.SideH[PosNum.X, PosNum.Y + 1].Road == road;
                    //do cross intersection
                    if ( roadTop && roadLeft && roadRight && roadBottom )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_CrossIntersection;
                        resultDirection = TileUtil.None;
                        //do T intersection
                    }
                    else if ( roadTop && roadLeft && roadRight )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_TIntersection;
                        resultDirection = TileUtil.Top;
                    }
                    else if ( roadTop && roadLeft && roadBottom )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_TIntersection;
                        resultDirection = TileUtil.Left;
                    }
                    else if ( roadTop && roadRight && roadBottom )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_TIntersection;
                        resultDirection = TileUtil.Right;
                    }
                    else if ( roadLeft && roadRight && roadBottom )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_TIntersection;
                        resultDirection = TileUtil.Bottom;
                        //do straight
                    }
                    else if ( roadTop && roadBottom )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_Straight;
                        if ( App.Random.Next() >= 0.5F )
                        {
                            resultDirection = TileUtil.Top;
                        }
                        else
                        {
                            resultDirection = TileUtil.Bottom;
                        }
                    }
                    else if ( roadLeft && roadRight )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_Straight;
                        if ( App.Random.Next() >= 0.5F )
                        {
                            resultDirection = TileUtil.Left;
                        }
                        else
                        {
                            resultDirection = TileUtil.Right;
                        }
                        //do corner
                    }
                    else if ( roadTop && roadLeft )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_Corner_In;
                        resultDirection = TileUtil.TopLeft;
                    }
                    else if ( roadTop && roadRight )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_Corner_In;
                        resultDirection = TileUtil.TopRight;
                    }
                    else if ( roadLeft && roadBottom )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_Corner_In;
                        resultDirection = TileUtil.BottomLeft;
                    }
                    else if ( roadRight && roadBottom )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_Corner_In;
                        resultDirection = TileUtil.BottomRight;
                        //do end
                    }
                    else if ( roadTop )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_End;
                        resultDirection = TileUtil.Top;
                    }
                    else if ( roadLeft )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_End;
                        resultDirection = TileUtil.Left;
                    }
                    else if ( roadRight )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_End;
                        resultDirection = TileUtil.Right;
                    }
                    else if ( roadBottom )
                    {
                        resultTiles = painter.RoadBrushes[BrushNum].Tile_End;
                        resultDirection = TileUtil.Bottom;
                    }
                }
            }

            if ( resultTiles == null )
            {
                resultTexture.TextureNum = -1;
                resultTexture.Direction = TileUtil.None;
            }
            else
            {
                resultTexture = resultTiles.GetRandom();
            }
            if ( resultTexture.TextureNum < 0 )
            {
                if ( MakeInvalidTiles )
                {
                    terrain.Tiles[PosNum.X, PosNum.Y].Texture = TileUtil.OrientateTile(ref resultTexture, resultDirection);
                }
            }
            else
            {
                terrain.Tiles[PosNum.X, PosNum.Y].Texture = TileUtil.OrientateTile(ref resultTexture, resultDirection);
            }

            Map.SectorGraphicsChanges.TileChanged(PosNum);
            Map.SectorTerrainUndoChanges.TileChanged(PosNum);
        }
    }
}