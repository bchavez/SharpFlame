using Microsoft.VisualBasic;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Mapping.Tools;
using SharpFlame.Painters;

namespace SharpFlame.Mapping
{
    public class clsUpdateAutotexture : clsAction
    {
        public bool MakeInvalidTiles;

        private Painters.Terrain Terrain_Inner;
        private Painters.Terrain Terrain_Outer;
        private Road Road;
        private bool RoadTop;
        private bool RoadLeft;
        private bool RoadRight;
        private bool RoadBottom;
        private Painter Painter;
        private clsTerrain Terrain;
        private TileList ResultTiles;
        private TileDirection ResultDirection;
        private TileOrientationChance ResultTexture;

        public override void ActionPerform()
        {
            Terrain = Map.Terrain;

            Painter = Map.Painter;

            ResultTiles = null;
            ResultDirection = TileUtil.None;

            //apply centre brushes
            if ( !Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff )
            {
                for ( int BrushNum = 0; BrushNum <= Painter.TerrainCount - 1; BrushNum++ )
                {
                    Terrain_Inner = Painter.Terrains[BrushNum];
                    if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                    {
                        if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                        {
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                            {
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    //i i i i
                                    ResultTiles = Terrain_Inner.Tiles;
                                    ResultDirection = TileUtil.None;
                                }
                            }
                        }
                    }
                }
            }

            //apply transition brushes
            if ( !Terrain.Tiles[PosNum.X, PosNum.Y].Terrain_IsCliff )
            {
                for ( int BrushNum = 0; BrushNum <= Painter.TransitionBrushCount - 1; BrushNum++ )
                {
                    Terrain_Inner = Painter.TransitionBrushes[BrushNum].Terrain_Inner;
                    Terrain_Outer = Painter.TransitionBrushes[BrushNum].Terrain_Outer;
                    if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                    {
                        if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                        {
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                            {
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    //i i i i
                                    //nothing to do here
                                    break;
                                }
                                else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    //i i i o
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_In;
                                    ResultDirection = TileUtil.BottomRight;
                                    break;
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    //i i o i
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_In;
                                    ResultDirection = TileUtil.BottomLeft;
                                    break;
                                }
                                else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    //i i o o
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileUtil.Bottom;
                                    break;
                                }
                            }
                        }
                        else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                        {
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                            {
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    //i o i i
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_In;
                                    ResultDirection = TileUtil.TopRight;
                                    break;
                                }
                                else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    //i o i o
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileUtil.Right;
                                    break;
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    //i o o i
                                    ResultTiles = null;
                                    ResultDirection = TileUtil.None;
                                    break;
                                }
                                else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    //i o o o
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_Out;
                                    ResultDirection = TileUtil.BottomRight;
                                    break;
                                }
                            }
                        }
                    }
                    else if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                    {
                        if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                        {
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                            {
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    //o i i i
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_In;
                                    ResultDirection = TileUtil.TopLeft;
                                    break;
                                }
                                else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    //o i i o
                                    ResultTiles = null;
                                    ResultDirection = TileUtil.None;
                                    break;
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    //o i o i
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileUtil.Left;
                                    break;
                                }
                                else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    //o i o o
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_Out;
                                    ResultDirection = TileUtil.BottomLeft;
                                    break;
                                }
                            }
                        }
                        else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                        {
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                            {
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    //o o i i
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = TileUtil.Top;
                                    break;
                                }
                                else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    //o o i o
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_Out;
                                    ResultDirection = TileUtil.TopRight;
                                    break;
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    //o o o i
                                    ResultTiles = Painter.TransitionBrushes[BrushNum].Tiles_Corner_Out;
                                    ResultDirection = TileUtil.TopLeft;
                                    break;
                                }
                                else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
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
            if ( Terrain.Tiles[PosNum.X, PosNum.Y].Tri )
            {
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopLeftIsCliff )
                {
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                    {
                        int BrushNum = 0;
                        for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                        {
                            Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                            Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                            if ( Terrain_Inner == Terrain_Outer )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( A >= 3 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = Terrain.Tiles[PosNum.X, PosNum.Y].DownSide;
                                    break;
                                }
                            }
                            if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner && Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner) &&
                                  (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer ||
                                   Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) ||
                                 ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner || Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner) &&
                                  (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer &&
                                   Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                ResultDirection = TileUtil.Bottom;
                                break;
                            }
                            else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer &&
                                        Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer) &&
                                       (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner ||
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) ||
                                      ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer ||
                                        Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer) &&
                                       (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner &&
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                ResultDirection = TileUtil.Left;
                                break;
                            }
                            else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer &&
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer) &&
                                       (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner ||
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) ||
                                      ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer ||
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer) &&
                                       (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner &&
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                ResultDirection = TileUtil.Top;
                                break;
                            }
                            else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner &&
                                        Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner) &&
                                       (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer ||
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) ||
                                      ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner ||
                                        Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner) &&
                                       (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer &&
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                ResultDirection = TileUtil.Right;
                                break;
                            }
                        }
                        if ( BrushNum == Painter.CliffBrushCount )
                        {
                            ResultTiles = null;
                            ResultDirection = TileUtil.None;
                        }
                    }
                    else
                    {
                        int BrushNum = 0;
                        for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                        {
                            Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                            Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( A >= 2 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_In;
                                    ResultDirection = TileUtil.TopLeft;
                                    break;
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( A >= 2 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_Out;
                                    ResultDirection = TileUtil.BottomRight;
                                    break;
                                }
                            }
                        }
                        if ( BrushNum == Painter.CliffBrushCount )
                        {
                            ResultTiles = null;
                            ResultDirection = TileUtil.None;
                        }
                    }
                }
                else if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomRightIsCliff )
                {
                    int BrushNum = 0;
                    for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                    {
                        Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                        Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                        if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                        {
                            int A = 0;
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                            {
                                A++;
                            }
                            if ( A >= 2 )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_In;
                                ResultDirection = TileUtil.BottomRight;
                                break;
                            }
                        }
                        else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                        {
                            int A = 0;
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( A >= 2 )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_Out;
                                ResultDirection = TileUtil.TopLeft;
                                break;
                            }
                        }
                    }
                    if ( BrushNum == Painter.CliffBrushCount )
                    {
                        ResultTiles = null;
                        ResultDirection = TileUtil.None;
                    }
                }
                else
                {
                    //no cliff
                }
            }
            else
            {
                //default tri orientation
                if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriTopRightIsCliff )
                {
                    if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                    {
                        int BrushNum = 0;
                        for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                        {
                            Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                            Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                            if ( Terrain_Inner == Terrain_Outer )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( A >= 3 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                    ResultDirection = Terrain.Tiles[PosNum.X, PosNum.Y].DownSide;
                                    break;
                                }
                            }
                            if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner && Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner) &&
                                  (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer ||
                                   Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) ||
                                 ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner || Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner) &&
                                  (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer &&
                                   Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                ResultDirection = TileUtil.Bottom;
                                break;
                            }
                            else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer &&
                                        Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer) &&
                                       (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner ||
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) ||
                                      ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer ||
                                        Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer) &&
                                       (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner &&
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                ResultDirection = TileUtil.Left;
                                break;
                            }
                            else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer &&
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer) &&
                                       (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner ||
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) ||
                                      ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer ||
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer) &&
                                       (Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner &&
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner)) )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                ResultDirection = TileUtil.Top;
                                break;
                            }
                            else if ( ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner &&
                                        Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner) &&
                                       (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer ||
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) ||
                                      ((Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner ||
                                        Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner) &&
                                       (Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer &&
                                        Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer)) )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Straight;
                                ResultDirection = TileUtil.Right;
                                break;
                            }
                        }
                        if ( BrushNum == Painter.CliffBrushCount )
                        {
                            ResultTiles = null;
                            ResultDirection = TileUtil.None;
                        }
                    }
                    else
                    {
                        int BrushNum = 0;
                        for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                        {
                            Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                            Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                                {
                                    A++;
                                }
                                if ( A >= 2 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_In;
                                    ResultDirection = TileUtil.TopRight;
                                    break;
                                }
                            }
                            else if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                            {
                                int A = 0;
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                                {
                                    A++;
                                }
                                if ( A >= 2 )
                                {
                                    ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_Out;
                                    ResultDirection = TileUtil.BottomLeft;
                                    break;
                                }
                            }
                        }
                        if ( BrushNum == Painter.CliffBrushCount )
                        {
                            ResultTiles = null;
                            ResultDirection = TileUtil.None;
                        }
                    }
                }
                else if ( Terrain.Tiles[PosNum.X, PosNum.Y].TriBottomLeftIsCliff )
                {
                    int BrushNum = 0;
                    for ( BrushNum = 0; BrushNum <= Painter.CliffBrushCount - 1; BrushNum++ )
                    {
                        Terrain_Inner = Painter.CliffBrushes[BrushNum].Terrain_Inner;
                        Terrain_Outer = Painter.CliffBrushes[BrushNum].Terrain_Outer;
                        if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                        {
                            int A = 0;
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Inner )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Inner )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Inner )
                            {
                                A++;
                            }
                            if ( A >= 2 )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_In;
                                ResultDirection = TileUtil.BottomLeft;
                                break;
                            }
                        }
                        else if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Inner )
                        {
                            int A = 0;
                            if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                            {
                                A++;
                            }
                            if ( A >= 2 )
                            {
                                ResultTiles = Painter.CliffBrushes[BrushNum].Tiles_Corner_Out;
                                ResultDirection = TileUtil.TopRight;
                                break;
                            }
                        }
                    }
                    if ( BrushNum == Painter.CliffBrushCount )
                    {
                        ResultTiles = null;
                        ResultDirection = TileUtil.None;
                    }
                }
                else
                {
                    //no cliff
                }
            }

            //apply roads
            Road = null;
            if ( Terrain.SideH[PosNum.X, PosNum.Y].Road != null )
            {
                Road = Terrain.SideH[PosNum.X, PosNum.Y].Road;
            }
            else if ( Terrain.SideH[PosNum.X, PosNum.Y + 1].Road != null )
            {
                Road = Terrain.SideH[PosNum.X, PosNum.Y + 1].Road;
            }
            else if ( Terrain.SideV[PosNum.X + 1, PosNum.Y].Road != null )
            {
                Road = Terrain.SideV[PosNum.X + 1, PosNum.Y].Road;
            }
            else if ( Terrain.SideV[PosNum.X, PosNum.Y].Road != null )
            {
                Road = Terrain.SideV[PosNum.X, PosNum.Y].Road;
            }
            if ( Road != null )
            {
                int BrushNum = 0;
                for ( BrushNum = 0; BrushNum <= Painter.RoadBrushCount - 1; BrushNum++ )
                {
                    if ( Painter.RoadBrushes[BrushNum].Road == Road )
                    {
                        Terrain_Outer = Painter.RoadBrushes[BrushNum].Terrain;
                        int A = 0;
                        if ( Terrain.Vertices[PosNum.X, PosNum.Y].Terrain == Terrain_Outer )
                        {
                            A++;
                        }
                        if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y].Terrain == Terrain_Outer )
                        {
                            A++;
                        }
                        if ( Terrain.Vertices[PosNum.X, PosNum.Y + 1].Terrain == Terrain_Outer )
                        {
                            A++;
                        }
                        if ( Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Terrain == Terrain_Outer )
                        {
                            A++;
                        }
                        if ( A >= 2 )
                        {
                            break;
                        }
                    }
                }

                ResultTiles = null;
                ResultDirection = TileUtil.None;

                if ( BrushNum < Painter.RoadBrushCount )
                {
                    RoadTop = Terrain.SideH[PosNum.X, PosNum.Y].Road == Road;
                    RoadLeft = Terrain.SideV[PosNum.X, PosNum.Y].Road == Road;
                    RoadRight = Terrain.SideV[PosNum.X + 1, PosNum.Y].Road == Road;
                    RoadBottom = Terrain.SideH[PosNum.X, PosNum.Y + 1].Road == Road;
                    //do cross intersection
                    if ( RoadTop && RoadLeft && RoadRight && RoadBottom )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_CrossIntersection;
                        ResultDirection = TileUtil.None;
                        //do T intersection
                    }
                    else if ( RoadTop && RoadLeft && RoadRight )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_TIntersection;
                        ResultDirection = TileUtil.Top;
                    }
                    else if ( RoadTop && RoadLeft && RoadBottom )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_TIntersection;
                        ResultDirection = TileUtil.Left;
                    }
                    else if ( RoadTop && RoadRight && RoadBottom )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_TIntersection;
                        ResultDirection = TileUtil.Right;
                    }
                    else if ( RoadLeft && RoadRight && RoadBottom )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_TIntersection;
                        ResultDirection = TileUtil.Bottom;
                        //do straight
                    }
                    else if ( RoadTop && RoadBottom )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Straight;
                        if ( App.Random.Next() >= 0.5F )
                        {
                            ResultDirection = TileUtil.Top;
                        }
                        else
                        {
                            ResultDirection = TileUtil.Bottom;
                        }
                    }
                    else if ( RoadLeft && RoadRight )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Straight;
                        if ( App.Random.Next() >= 0.5F )
                        {
                            ResultDirection = TileUtil.Left;
                        }
                        else
                        {
                            ResultDirection = TileUtil.Right;
                        }
                        //do corner
                    }
                    else if ( RoadTop && RoadLeft )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Corner_In;
                        ResultDirection = TileUtil.TopLeft;
                    }
                    else if ( RoadTop && RoadRight )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Corner_In;
                        ResultDirection = TileUtil.TopRight;
                    }
                    else if ( RoadLeft && RoadBottom )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Corner_In;
                        ResultDirection = TileUtil.BottomLeft;
                    }
                    else if ( RoadRight && RoadBottom )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_Corner_In;
                        ResultDirection = TileUtil.BottomRight;
                        //do end
                    }
                    else if ( RoadTop )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_End;
                        ResultDirection = TileUtil.Top;
                    }
                    else if ( RoadLeft )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_End;
                        ResultDirection = TileUtil.Left;
                    }
                    else if ( RoadRight )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_End;
                        ResultDirection = TileUtil.Right;
                    }
                    else if ( RoadBottom )
                    {
                        ResultTiles = Painter.RoadBrushes[BrushNum].Tile_End;
                        ResultDirection = TileUtil.Bottom;
                    }
                }
            }

            if ( ResultTiles == null )
            {
                ResultTexture.TextureNum = -1;
                ResultTexture.Direction = TileUtil.None;
            }
            else
            {
                ResultTexture = ResultTiles.GetRandom();
            }
            if ( ResultTexture.TextureNum < 0 )
            {
                if ( MakeInvalidTiles )
                {
                    Terrain.Tiles[PosNum.X, PosNum.Y].Texture = TileUtil.OrientateTile(ref ResultTexture, ResultDirection);
                }
            }
            else
            {
                Terrain.Tiles[PosNum.X, PosNum.Y].Texture = TileUtil.OrientateTile(ref ResultTexture, ResultDirection);
            }

            Map.SectorGraphicsChanges.TileChanged(PosNum);
            Map.SectorTerrainUndoChanges.TileChanged(PosNum);
        }
    }
}