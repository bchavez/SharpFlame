using System;
using System.Diagnostics;
using System.Windows.Forms;
using Matrix3D;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using SharpFlame.Collections.Specialized;
using SharpFlame.Domain;
using SharpFlame.Generators;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;
using SharpFlame.Pathfinding;
using SharpFlame.Util;

namespace SharpFlame
{
    public class clsGenerateMap
    {
        public clsMap Map;

        public sXY_int TileSize;
        public int LevelCount;
        public float LevelHeight;
        public bool SymmetryIsRotational;

        public struct sSymmetryBlock
        {
            public sXY_int XYNum;
            public TileOrientation Orientation;
            public int[] ReflectToNum;
        }

        public sSymmetryBlock[] SymmetryBlocks;
        public sXY_int SymmetryBlockCountXY;
        public int SymmetryBlockCount;
        public int JitterScale;
        public int MaxLevelTransition;
        public float NodeScale;
        public int BaseLevel;
        public int BaseFlatArea;
        public sXY_int[] PlayerBasePos;
        public int TopLeftPlayerCount;
        public int PassagesChance;
        public int VariationChance;
        public int FlatsChance;
        public float MaxDisconnectionDist;
        public double RampBase;
        public int BaseOilCount;
        public int ExtraOilCount;
        public int ExtraOilClusterSizeMin;
        public int ExtraOilClusterSizeMax;
        public float OilDispersion;
        public int OilAtATime;
        public int WaterSpawnQuantity;
        public int TotalWaterQuantity;
        public float FeatureClusterChance;
        public int FeatureClusterMinUnits;
        public int FeatureClusterMaxUnits;
        public int FeatureScatterCount;
        public int FeatureScatterGap;
        public int BaseTruckCount;

        private clsConnection[] Connections;
        private int ConnectionCount;
        private clsPassageNode[,] PassageNodes;
        private int PassageNodeCount;
        private float[,,,] PassageNodeDists;
        private clsNearest[] Nearests;
        private int NearestCount;

        private struct sPlayerBase
        {
            public clsPassageNode[] Nodes;
            public int NodeCount;
            public sXY_int Pos;

            public void CalcPos()
            {
                int A = 0;
                Position.XY_dbl Total = default(Position.XY_dbl);

                for ( A = 0; A <= NodeCount - 1; A++ )
                {
                    Total.X += Nodes[A].Pos.X;
                    Total.Y += Nodes[A].Pos.Y;
                }
                Pos.X = (int)(Total.X / NodeCount);
                Pos.Y = (int)(Total.Y / NodeCount);
            }
        }

        private sPlayerBase[] PlayerBases;
        private int TotalPlayerCount;

        public int GetTotalPlayerCount
        {
            get { return TotalPlayerCount; }
        }

        public class clsPassageNode
        {
            public int Num = -1;

            public int MirrorNum = -1;

            public int Level = -1;

            public sXY_int Pos;

            public bool IsOnBorder;
            public bool IsNearBorder;

            public int OilCount;

            public bool HasFeatureCluster;

            public bool IsWater;

            public int PlayerBaseNum = -1;

            public struct sConnection
            {
                public clsConnection Connection;
                public bool IsB;

                public clsPassageNode GetOther()
                {
                    if ( IsB )
                    {
                        return Connection.PassageNodeA;
                    }
                    else
                    {
                        return Connection.PassageNodeB;
                    }
                }
            }

            public sConnection[] Connections;
            public int ConnectionCount;

            public void Connection_Add(sConnection NewConnection)
            {
                if ( NewConnection.IsB )
                {
                    NewConnection.Connection.PassageNodeB_ConnectionNum = ConnectionCount;
                }
                else
                {
                    NewConnection.Connection.PassageNodeA_ConnectionNum = ConnectionCount;
                }

                Array.Resize(ref Connections, ConnectionCount + 1);
                Connections[ConnectionCount] = NewConnection;
                ConnectionCount++;
            }

            public void Connection_Remove(int Num)
            {
                if ( Connections[Num].IsB )
                {
                    Connections[Num].Connection.PassageNodeB_ConnectionNum = -1;
                }
                else
                {
                    Connections[Num].Connection.PassageNodeA_ConnectionNum = -1;
                }

                ConnectionCount--;
                if ( Num != ConnectionCount )
                {
                    Connections[Num] = Connections[ConnectionCount];
                    if ( Connections[Num].IsB )
                    {
                        Connections[Num].Connection.PassageNodeB_ConnectionNum = Num;
                    }
                    else
                    {
                        Connections[Num].Connection.PassageNodeA_ConnectionNum = Num;
                    }
                }
            }

            public clsConnection FindConnection(clsPassageNode PassageNode)
            {
                int A = 0;

                for ( A = 0; A <= ConnectionCount - 1; A++ )
                {
                    if ( Connections[A].GetOther() == PassageNode )
                    {
                        return Connections[A].Connection;
                    }
                }
                return null;
            }

            public void ReorderConnections()
            {
                int A = 0;
                int B = 0;
                int C = 0;
                sConnection[] NewOrder = new sConnection[ConnectionCount];
                double[] AwayAngles = new double[ConnectionCount];
                clsPassageNode OtherNode = default(clsPassageNode);
                sXY_int XY_int = new sXY_int();
                double AwayAngle = 0;

                for ( A = 0; A <= ConnectionCount - 1; A++ )
                {
                    OtherNode = Connections[A].GetOther();
                    XY_int.X = OtherNode.Pos.X - Pos.X;
                    XY_int.Y = OtherNode.Pos.Y - Pos.Y;
                    AwayAngle = XY_int.ToDoubles().GetAngle();
                    for ( B = 0; B <= A - 1; B++ )
                    {
                        if ( AwayAngle < AwayAngles[B] )
                        {
                            break;
                        }
                    }
                    for ( C = A - 1; C >= B; C-- )
                    {
                        NewOrder[C + 1] = NewOrder[C];
                        AwayAngles[C + 1] = AwayAngles[C];
                    }
                    NewOrder[B] = Connections[A];
                    AwayAngles[B] = AwayAngle;
                }
                for ( A = 0; A <= ConnectionCount - 1; A++ )
                {
                    Connections[A] = NewOrder[A];
                    if ( Connections[A].IsB )
                    {
                        Connections[A].Connection.PassageNodeB_ConnectionNum = A;
                    }
                    else
                    {
                        Connections[A].Connection.PassageNodeA_ConnectionNum = A;
                    }
                }
            }

            public void CalcIsNearBorder()
            {
                int A = 0;

                for ( A = 0; A <= ConnectionCount - 1; A++ )
                {
                    if ( Connections[A].GetOther().IsOnBorder )
                    {
                        IsNearBorder = true;
                        return;
                    }
                }
                IsNearBorder = false;
            }
        }

        public class clsConnection
        {
            public clsPassageNode PassageNodeA;
            public int PassageNodeA_ConnectionNum = -1;
            public clsPassageNode PassageNodeB;
            public int PassageNodeB_ConnectionNum = -1;
            public bool IsRamp;
            public clsConnection[] Reflections;
            public int ReflectionCount;

            public clsConnection(clsPassageNode NewPassageNodeA, clsPassageNode NewPassageNodeB)
            {
                clsPassageNode.sConnection NewConnection = new clsPassageNode.sConnection();

                PassageNodeA = NewPassageNodeA;
                NewConnection.Connection = this;
                NewConnection.IsB = false;
                PassageNodeA.Connection_Add(NewConnection);

                PassageNodeB = NewPassageNodeB;
                NewConnection.Connection = this;
                NewConnection.IsB = true;
                PassageNodeB.Connection_Add(NewConnection);
            }
        }


        public PathfinderNetwork TilePathMap;
        public PathfinderNetwork VertexPathMap;

        public clsGeneratorTileset GenerateTileset;

        public struct GenerateTerrainVertex
        {
            public PathfinderNode Node;
            public PathfinderConnection TopLink;
            public PathfinderConnection TopRightLink;
            public PathfinderConnection RightLink;
            public PathfinderConnection BottomRightLink;
            public PathfinderConnection BottomLink;
            public PathfinderConnection BottomLeftLink;
            public PathfinderConnection LeftLink;
            public PathfinderConnection TopLeftLink;
        }

        public GenerateTerrainVertex[,] GenerateTerrainVertices;

        public struct GenerateTerrainTile
        {
            public PathfinderNode Node;
            public PathfinderConnection TopLeftLink;
            public PathfinderConnection TopLink;
            public PathfinderConnection TopRightLink;
            public PathfinderConnection RightLink;
            public PathfinderConnection BottomRightLink;
            public PathfinderConnection BottomLink;
            public PathfinderConnection BottomLeftLink;
            public PathfinderConnection LeftLink;
        }

        public GenerateTerrainTile[,] GenerateTerrainTiles = new GenerateTerrainTile[0, 0];

        public clsResult GenerateLayout()
        {
            clsResult ReturnResult = new clsResult("Layout");

            int X = 0;
            int Y = 0;
            int A = 0;
            int B = 0;
            int C = 0;
            int D = 0;
            int E = 0;
            int F = 0;
            int G = 0;
            int H = 0;

            TotalPlayerCount = TopLeftPlayerCount * SymmetryBlockCount;

            sXY_int SymmetrySize = new sXY_int();

            SymmetrySize.X = (int)(TileSize.X * App.TerrainGridSpacing / SymmetryBlockCountXY.X);
            SymmetrySize.Y = (int)(TileSize.Y * App.TerrainGridSpacing / SymmetryBlockCountXY.Y);

            //create passage nodes

            int PassageRadius = (int)(128.0F * NodeScale);
            int MaxLikelyPassageNodeCount = 0;
            MaxLikelyPassageNodeCount =
                (int)(Math.Ceiling(Convert.ToDecimal(2.0D * TileSize.X * 128 * TileSize.Y * 128 / (Math.PI * PassageRadius * PassageRadius))));

            PassageNodes = new clsPassageNode[SymmetryBlockCount, MaxLikelyPassageNodeCount];
            int LoopCount = 0;
            int EdgeOffset = 0 * 128;
            bool PointIsValid;
            sXY_int EdgeSections = new sXY_int();
            Position.XY_dbl EdgeSectionSize = default(Position.XY_dbl);
            sXY_int NewPointPos = new sXY_int();

            if ( SymmetryBlockCountXY.X == 1 )
            {
                EdgeSections.X =
                    Convert.ToInt32(
                        Conversion.Int((TileSize.X * App.TerrainGridSpacing - EdgeOffset * 2.0D) / (NodeScale * App.TerrainGridSpacing * 2.0F)));
                EdgeSectionSize.X = (TileSize.X * App.TerrainGridSpacing - EdgeOffset * 2.0D) / EdgeSections.X;
                EdgeSections.X--;
            }
            else
            {
                EdgeSections.X =
                    (int)
                        (Conversion.Int((TileSize.X * App.TerrainGridSpacing / SymmetryBlockCountXY.X - EdgeOffset) /
                                        (NodeScale * App.TerrainGridSpacing * 2.0F) - 0.5D));
                EdgeSectionSize.X =
                    Convert.ToDouble((TileSize.X * App.TerrainGridSpacing / SymmetryBlockCountXY.X - EdgeOffset) /
                                            (Convert.ToDouble(
                                                Conversion.Int((TileSize.X * App.TerrainGridSpacing / SymmetryBlockCountXY.X - EdgeOffset) /
                                                               (NodeScale * App.TerrainGridSpacing * 2.0F) - 0.5D)) + 0.5D));
            }
            if ( SymmetryBlockCountXY.Y == 1 )
            {
                EdgeSections.Y =
                    Convert.ToInt32(
                        Conversion.Int((TileSize.Y * App.TerrainGridSpacing - EdgeOffset * 2.0D) / (NodeScale * App.TerrainGridSpacing * 2.0F)));
                EdgeSectionSize.Y = (TileSize.Y * App.TerrainGridSpacing - EdgeOffset * 2.0D) / EdgeSections.Y;
                EdgeSections.Y--;
            }
            else
            {
                EdgeSections.Y =
                    Convert.ToInt32(
                        Conversion.Int((TileSize.Y * App.TerrainGridSpacing / SymmetryBlockCountXY.Y - EdgeOffset) /
                                       (NodeScale * App.TerrainGridSpacing * 2.0F) - 0.5D));
                EdgeSectionSize.Y =
                    Convert.ToDouble((TileSize.Y * App.TerrainGridSpacing / SymmetryBlockCountXY.Y - EdgeOffset) /
                                            (Convert.ToDouble(
                                                Conversion.Int((TileSize.Y * App.TerrainGridSpacing / SymmetryBlockCountXY.Y - EdgeOffset) /
                                                               (NodeScale * App.TerrainGridSpacing * 2.0F) - 0.5D)) + 0.5D));
            }

            PassageNodeCount = 0;
            for ( Y = 0; Y <= EdgeSections.Y; Y++ )
            {
                if ( !MakePassageNodes(new sXY_int(EdgeOffset, EdgeOffset + (int)(Y * EdgeSectionSize.Y)), true) )
                {
                    ReturnResult.ProblemAdd("Error: Bad border node.");
                    return ReturnResult;
                }
                if ( SymmetryBlockCountXY.X == 1 )
                {
                    if (
                        !MakePassageNodes(new sXY_int(TileSize.X * App.TerrainGridSpacing - EdgeOffset, EdgeOffset + (int)(Y * EdgeSectionSize.Y)), true) )
                    {
                        ReturnResult.ProblemAdd("Error: Bad border node.");
                        return ReturnResult;
                    }
                }
            }
            for ( X = 1; X <= EdgeSections.X; X++ )
            {
                if ( !MakePassageNodes(new sXY_int(EdgeOffset + (int)(X * EdgeSectionSize.X), EdgeOffset), true) )
                {
                    ReturnResult.ProblemAdd("Error: Bad border node.");
                    return ReturnResult;
                }
                if ( SymmetryBlockCountXY.Y == 1 )
                {
                    if (
                        !MakePassageNodes(new sXY_int(EdgeOffset + (int)(X * EdgeSectionSize.X), TileSize.Y * App.TerrainGridSpacing - EdgeOffset), true) )
                    {
                        ReturnResult.ProblemAdd("Error: Bad border node.");
                        return ReturnResult;
                    }
                }
            }
            do
            {
                LoopCount = 0;
                do
                {
                    PointIsValid = true;
                    if ( SymmetryBlockCountXY.X == 1 )
                    {
                        NewPointPos.X = (int)(EdgeOffset + Conversion.Int(VBMath.Rnd() * (SymmetrySize.X - EdgeOffset * 2 + 1)));
                    }
                    else
                    {
                        NewPointPos.X = EdgeOffset + (int)(Conversion.Int(VBMath.Rnd() * (SymmetrySize.X - EdgeOffset + 1)));
                    }
                    if ( SymmetryBlockCountXY.Y == 1 )
                    {
                        NewPointPos.Y = EdgeOffset + (int)(Conversion.Int(VBMath.Rnd() * (SymmetrySize.Y - EdgeOffset * 2 + 1)));
                    }
                    else
                    {
                        NewPointPos.Y = EdgeOffset + Convert.ToInt32(Conversion.Int(VBMath.Rnd() * (SymmetrySize.Y - EdgeOffset + 1)));
                    }
                    for ( A = 0; A <= PassageNodeCount - 1; A++ )
                    {
                        for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                        {
                            if ( (PassageNodes[B, A].Pos - NewPointPos).ToDoubles().GetMagnitude() < PassageRadius * 2 )
                            {
                                goto PointTooClose;
                            }
                        }
                    }
                    PointTooClose:
                    if ( A == PassageNodeCount )
                    {
                        if ( MakePassageNodes(NewPointPos, false) )
                        {
                            break;
                        }
                    }
                    LoopCount++;
                    if ( LoopCount >= (int)(64.0F * TileSize.X * TileSize.Y / (NodeScale * NodeScale)) )
                    {
                        goto PointMakingFinished;
                    }
                } while ( true );
            } while ( true );
            PointMakingFinished:
            PassageNodes =
                (clsPassageNode[,])
                    Utils.CopyArray((Array)PassageNodes, new clsPassageNode[SymmetryBlockCount, PassageNodeCount]);

            //connect until all are connected without intersecting

            MathUtil.sIntersectPos IntersectPos = new MathUtil.sIntersectPos();
            int MaxConDist2 = PassageRadius * 2 * 4;
            MaxConDist2 *= MaxConDist2;
            clsNearest NearestA = default(clsNearest);
            Nearests = new clsNearest[PassageNodeCount * 64];
            clsPassageNode tmpPassageNodeA = default(clsPassageNode);
            clsPassageNode tmpPassageNodeB = default(clsPassageNode);
            clsTestNearestArgs NearestArgs = new clsTestNearestArgs();
            int MinConDist = (int)(NodeScale * 1.25F * 128.0F);

            NearestArgs.MaxConDist2 = MaxConDist2;
            NearestArgs.MinConDist = MinConDist;

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                NearestArgs.PassageNodeA = PassageNodes[0, A];
                for ( B = A; B <= PassageNodeCount - 1; B++ )
                {
                    for ( C = 0; C <= SymmetryBlockCount - 1; C++ )
                    {
                        NearestArgs.PassageNodeB = PassageNodes[C, B];
                        if ( NearestArgs.PassageNodeA != NearestArgs.PassageNodeB )
                        {
                            TestNearest(NearestArgs);
                        }
                    }
                }
            }

            clsNearest NearestB = default(clsNearest);
            bool Flag = default(bool);

            for ( G = 0; G <= NearestCount - 1; G++ )
            {
                NearestA = Nearests[G];
                for ( A = 0; A <= NearestA.NodeCount - 1; A++ )
                {
                    tmpPassageNodeA = NearestA.NodeA[A];
                    tmpPassageNodeB = NearestA.NodeB[A];
                    for ( H = 0; H <= NearestCount - 1; H++ )
                    {
                        NearestB = Nearests[H];
                        if ( NearestB != NearestA )
                        {
                            if ( NearestB.Dist2 < NearestA.Dist2 )
                            {
                                Flag = true;
                            }
                            else if ( NearestB.Dist2 == NearestA.Dist2 )
                            {
                                Flag = NearestA.Num > NearestB.Num;
                            }
                            else
                            {
                                Flag = false;
                            }
                            if ( Flag )
                            {
                                for ( B = 0; B <= NearestB.NodeCount - 1; B++ )
                                {
                                    if ( !(tmpPassageNodeA == NearestB.NodeA[B] || tmpPassageNodeA == NearestB.NodeB[B]
                                           || tmpPassageNodeB == NearestB.NodeA[B] || tmpPassageNodeB == NearestB.NodeB[B]) )
                                    {
                                        IntersectPos = MathUtil.GetLinesIntersectBetween(tmpPassageNodeA.Pos, tmpPassageNodeB.Pos, NearestB.NodeA[B].Pos,
                                            NearestB.NodeB[B].Pos);
                                        if ( IntersectPos.Exists )
                                        {
                                            break;
                                        }
                                    }
                                }
                                if ( B < NearestB.NodeCount )
                                {
                                    NearestA.BlockedCount++;
                                    NearestB.BlockedNearests[NearestB.BlockedNearestCount] = NearestA;
                                    NearestB.BlockedNearestCount++;
                                }
                            }
                        }
                    }
                }
            }

            int ChangeCount = 0;
            Connections = new clsConnection[PassageNodeCount * 16];

            do
            {
                //create valid connections
                ChangeCount = 0;
                G = 0;
                while ( G < NearestCount )
                {
                    NearestA = Nearests[G];
                    Flag = true;
                    if ( NearestA.BlockedCount == 0 && Flag )
                    {
                        F = ConnectionCount;
                        for ( D = 0; D <= NearestA.NodeCount - 1; D++ )
                        {
                            Connections[ConnectionCount] = new clsConnection(NearestA.NodeA[D], NearestA.NodeB[D]);
                            ConnectionCount++;
                        }
                        for ( D = 0; D <= NearestA.NodeCount - 1; D++ )
                        {
                            A = F + D;
                            Connections[A].ReflectionCount = NearestA.NodeCount - 1;
                            Connections[A].Reflections = new clsConnection[Connections[A].ReflectionCount];
                            B = 0;
                            for ( E = 0; E <= NearestA.NodeCount - 1; E++ )
                            {
                                if ( E != D )
                                {
                                    Connections[A].Reflections[B] = Connections[F + E];
                                    B++;
                                }
                            }
                        }
                        for ( C = 0; C <= NearestA.BlockedNearestCount - 1; C++ )
                        {
                            NearestA.BlockedNearests[C].Invalid = true;
                        }
                        NearestCount--;
                        H = NearestA.Num;
                        NearestA.Num = -1;
                        if ( H != NearestCount )
                        {
                            Nearests[H] = Nearests[NearestCount];
                            Nearests[H].Num = H;
                        }
                        ChangeCount++;
                    }
                    else
                    {
                        if ( !Flag )
                        {
                            NearestA.Invalid = true;
                        }
                        G++;
                    }
                }
                //remove blocked ones and their blocking effect
                G = 0;
                while ( G < NearestCount )
                {
                    NearestA = Nearests[G];
                    if ( NearestA.Invalid )
                    {
                        NearestA.Num = -1;
                        for ( D = 0; D <= NearestA.BlockedNearestCount - 1; D++ )
                        {
                            NearestA.BlockedNearests[D].BlockedCount--;
                        }
                        NearestCount--;
                        if ( G != NearestCount )
                        {
                            Nearests[G] = Nearests[NearestCount];
                            Nearests[G].Num = G;
                        }
                    }
                    else
                    {
                        G++;
                    }
                }
            } while ( ChangeCount > 0 );

            //put connections in order of angle

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    PassageNodes[B, A].ReorderConnections();
                    PassageNodes[B, A].CalcIsNearBorder();
                }
            }

            //get nodes in random order

            clsPassageNode[] PassageNodeListOrder = new clsPassageNode[PassageNodeCount];
            int PassageNodeListOrderCount = 0;
            clsPassageNode[] PassageNodeOrder = new clsPassageNode[PassageNodeCount];
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                PassageNodeListOrder[PassageNodeListOrderCount] = PassageNodes[0, A];
                PassageNodeListOrderCount++;
            }
            B = 0;
            while ( PassageNodeListOrderCount > 0 )
            {
                A = (int)(Conversion.Int(VBMath.Rnd() * PassageNodeListOrderCount));
                PassageNodeOrder[B] = PassageNodeListOrder[A];
                B++;
                PassageNodeListOrderCount--;
                PassageNodeListOrder[A] = PassageNodeListOrder[PassageNodeListOrderCount];
            }

            //designate height levels

            LevelHeight = 255.0F / (LevelCount - 1);
            int BestNum = 0;
            double Dist = 0;
            clsPassageNodeHeightLevelArgs HeightsArgs = new clsPassageNodeHeightLevelArgs();
            HeightsArgs.PassageNodesMinLevel.Nodes = new int[PassageNodeCount];
            HeightsArgs.PassageNodesMaxLevel.Nodes = new int[PassageNodeCount];
            HeightsArgs.MapLevelCount = new int[LevelCount];
            sXY_int RotatedPos = new sXY_int();

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                HeightsArgs.PassageNodesMinLevel.Nodes[A] = 0;
                HeightsArgs.PassageNodesMaxLevel.Nodes[A] = LevelCount - 1;
            }

            //create bases
            double[] BestDists = new double[BaseFlatArea];
            clsPassageNode[] BestNodes = new clsPassageNode[BaseFlatArea];
            int[] BestNodesReflectionNums = new int[BaseFlatArea];
            int BestDistCount = 0;
            PlayerBases = new sPlayerBase[TotalPlayerCount];
            for ( B = 0; B <= TopLeftPlayerCount - 1; B++ )
            {
                BestDistCount = 0;
                for ( A = 0; A <= PassageNodeCount - 1; A++ )
                {
                    for ( E = 0; E <= SymmetryBlockCount - 1; E++ )
                    {
                        tmpPassageNodeA = PassageNodes[E, A];
                        if ( !tmpPassageNodeA.IsOnBorder )
                        {
                            Dist = (tmpPassageNodeA.Pos - PlayerBasePos[B]).ToDoubles().GetMagnitude();
                            for ( C = BestDistCount - 1; C >= 0; C-- )
                            {
                                if ( Dist > BestDists[C] )
                                {
                                    break;
                                }
                            }
                            C++;
                            for ( D = Math.Min(BestDistCount - 1, BaseFlatArea - 2); D >= C; D-- )
                            {
                                BestDists[D + 1] = BestDists[D];
                                BestNodes[D + 1] = BestNodes[D];
                            }
                            if ( C < BaseFlatArea )
                            {
                                BestDists[C] = Dist;
                                BestNodes[C] = tmpPassageNodeA;
                                BestDistCount = Math.Max(BestDistCount, C + 1);
                            }
                        }
                    }
                }

                if ( BaseLevel < 0 )
                {
                    D = Convert.ToInt32(Conversion.Int(VBMath.Rnd() * LevelCount));
                }
                else
                {
                    D = BaseLevel;
                }

                HeightsArgs.MapLevelCount[D] += BestDistCount;

                for ( A = 0; A <= BestDistCount - 1; A++ )
                {
                    if ( BestNodes[A].MirrorNum == 0 )
                    {
                        BestNodesReflectionNums[A] = -1;
                    }
                    else
                    {
                        for ( C = 0; C <= ((int)(SymmetryBlockCount / 2.0D)) - 1; C++ )
                        {
                            if ( SymmetryBlocks[0].ReflectToNum[C] == BestNodes[A].MirrorNum )
                            {
                                break;
                            }
                        }
                        BestNodesReflectionNums[A] = C;
                    }
                }

                for ( A = 0; A <= SymmetryBlockCount - 1; A++ )
                {
                    E = A * TopLeftPlayerCount + B;
                    PlayerBases[E].NodeCount = BestDistCount;
                    PlayerBases[E].Nodes = new clsPassageNode[PlayerBases[E].NodeCount];
                    for ( C = 0; C <= BestDistCount - 1; C++ )
                    {
                        if ( BestNodesReflectionNums[C] < 0 )
                        {
                            PlayerBases[E].Nodes[C] = PassageNodes[A, BestNodes[C].Num];
                        }
                        else
                        {
                            PlayerBases[E].Nodes[C] = PassageNodes[SymmetryBlocks[A].ReflectToNum[BestNodesReflectionNums[C]], BestNodes[C].Num];
                        }
                        PlayerBases[E].Nodes[C].PlayerBaseNum = E;
                        PlayerBases[E].Nodes[C].Level = D;
                        PassageNodesMinLevelSet(PlayerBases[E].Nodes[C], HeightsArgs.PassageNodesMinLevel, D, MaxLevelTransition);
                        PassageNodesMaxLevelSet(PlayerBases[E].Nodes[C], HeightsArgs.PassageNodesMaxLevel, D, MaxLevelTransition);
                    }
                    //PlayerBases(E).CalcPos()
                    RotatedPos = TileUtil.GetRotatedPos(SymmetryBlocks[A].Orientation, PlayerBasePos[B],
                        new sXY_int(SymmetrySize.X - 1, SymmetrySize.Y - 1));
                    PlayerBases[E].Pos.X = SymmetryBlocks[A].XYNum.X * SymmetrySize.X + RotatedPos.X;
                    PlayerBases[E].Pos.Y = SymmetryBlocks[A].XYNum.Y * SymmetrySize.Y + RotatedPos.Y;
                }
            }

            int WaterCount = 0;
            bool CanDoFlatsAroundWater = default(bool);
            int TotalWater = 0;
            int WaterSpawns = 0;

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                tmpPassageNodeA = PassageNodeOrder[A];
                if ( tmpPassageNodeA.Level < 0 && !tmpPassageNodeA.IsOnBorder )
                {
                    WaterCount = 0;
                    for ( B = 0; B <= tmpPassageNodeA.ConnectionCount - 1; B++ )
                    {
                        tmpPassageNodeB = tmpPassageNodeA.Connections[B].GetOther();
                        if ( tmpPassageNodeB.IsWater )
                        {
                            WaterCount++;
                        }
                    }
                    CanDoFlatsAroundWater = true;
                    for ( B = 0; B <= tmpPassageNodeA.ConnectionCount - 1; B++ )
                    {
                        if ( HeightsArgs.PassageNodesMinLevel.Nodes[tmpPassageNodeA.Connections[B].GetOther().Num] > 0 )
                        {
                            CanDoFlatsAroundWater = false;
                        }
                    }
                    if ( CanDoFlatsAroundWater &&
                         ((WaterCount == 0 & WaterSpawns < WaterSpawnQuantity) || (WaterCount == 1 & TotalWaterQuantity - TotalWater > WaterSpawnQuantity - WaterSpawns)) &&
                         HeightsArgs.PassageNodesMinLevel.Nodes[tmpPassageNodeA.Num] == 0 & TotalWater < TotalWaterQuantity )
                    {
                        if ( WaterCount == 0 )
                        {
                            WaterSpawns++;
                        }
                        TotalWater++;
                        C = tmpPassageNodeA.Num;
                        for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                        {
                            PassageNodes[D, C].IsWater = true;
                            PassageNodes[D, C].Level = 0;
                        }
                        PassageNodesMinLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMinLevel, 0, MaxLevelTransition);
                        PassageNodesMaxLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMaxLevel, 0, MaxLevelTransition);
                        HeightsArgs.MapLevelCount[0]++;
                        for ( B = 0; B <= tmpPassageNodeA.ConnectionCount - 1; B++ )
                        {
                            tmpPassageNodeB = tmpPassageNodeA.Connections[B].GetOther();
                            PassageNodesMinLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMinLevel, 0, MaxLevelTransition);
                            PassageNodesMaxLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMaxLevel, 0, MaxLevelTransition);
                        }
                    }
                }
            }

            clsPassageNode tmpPassageNodeC = default(clsPassageNode);
            sResult Result = new sResult();

            HeightsArgs.FlatsCutoff = 1;
            HeightsArgs.PassagesCutoff = 1;
            HeightsArgs.VariationCutoff = 1;
            HeightsArgs.ActionTotal = 1;

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                tmpPassageNodeA = PassageNodeOrder[A];
                if ( tmpPassageNodeA.Level < 0 && !tmpPassageNodeA.IsOnBorder && tmpPassageNodeA.IsNearBorder )
                {
                    HeightsArgs.PassageNode = tmpPassageNodeA;
                    Result = PassageNodeHeightLevel(HeightsArgs);
                    if ( !Result.Success )
                    {
                        ReturnResult.ProblemAdd(Result.Problem);
                        return ReturnResult;
                    }
                }
            }

            HeightsArgs.FlatsCutoff = FlatsChance;
            HeightsArgs.PassagesCutoff = HeightsArgs.FlatsCutoff + PassagesChance;
            HeightsArgs.VariationCutoff = HeightsArgs.PassagesCutoff + VariationChance;
            HeightsArgs.ActionTotal = HeightsArgs.VariationCutoff;
            if ( HeightsArgs.ActionTotal <= 0 )
            {
                ReturnResult.ProblemAdd("All height level behaviors are zero");
                return ReturnResult;
            }

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                tmpPassageNodeA = PassageNodeOrder[A];
                if ( tmpPassageNodeA.Level < 0 && !tmpPassageNodeA.IsOnBorder )
                {
                    HeightsArgs.PassageNode = tmpPassageNodeA;
                    Result = PassageNodeHeightLevel(HeightsArgs);
                    if ( !Result.Success )
                    {
                        ReturnResult.ProblemAdd(Result.Problem);
                        return ReturnResult;
                    }
                }
            }

            //set edge points to the level of their neighbour
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                tmpPassageNodeA = PassageNodes[0, A];
                if ( tmpPassageNodeA.IsOnBorder )
                {
                    if ( tmpPassageNodeA.Level >= 0 )
                    {
                        ReturnResult.ProblemAdd("Error: Border has had its height set.");
                        return ReturnResult;
                    }
                    //If tmpPassageNodeA.ConnectionCount <> 1 Then
                    //    ReturnResult.Problem = "Error: Border has incorrect connections."
                    //    Exit Function
                    //End If
                    tmpPassageNodeC = null;
                    CanDoFlatsAroundWater = true;
                    for ( B = 0; B <= tmpPassageNodeA.ConnectionCount - 1; B++ )
                    {
                        tmpPassageNodeB = tmpPassageNodeA.Connections[B].GetOther();
                        if ( tmpPassageNodeB.Level >= 0 && !tmpPassageNodeB.IsOnBorder )
                        {
                            if ( HeightsArgs.PassageNodesMinLevel.Nodes[tmpPassageNodeA.Num] <= tmpPassageNodeB.Level &&
                                 HeightsArgs.PassageNodesMaxLevel.Nodes[tmpPassageNodeA.Num] >= tmpPassageNodeB.Level )
                            {
                                if ( tmpPassageNodeC == null )
                                {
                                    tmpPassageNodeC = tmpPassageNodeB;
                                }
                            }
                        }
                        if ( HeightsArgs.PassageNodesMinLevel.Nodes[tmpPassageNodeB.Num] > 0 )
                        {
                            CanDoFlatsAroundWater = false;
                        }
                    }
                    //If tmpPassageNodeC Is Nothing Then
                    //    ReturnResult.Problem_Add("Error: No connection for border node")
                    //    Return ReturnResult
                    //End If
                    if ( tmpPassageNodeC != null )
                    {
                        BestNum = tmpPassageNodeC.Level;
                        PassageNodesMinLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMinLevel, BestNum, MaxLevelTransition);
                        PassageNodesMaxLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMaxLevel, BestNum, MaxLevelTransition);
                        for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                        {
                            PassageNodes[D, A].IsWater = tmpPassageNodeC.IsWater && CanDoFlatsAroundWater;
                            PassageNodes[D, A].Level = BestNum;
                        }
                        if ( tmpPassageNodeA.IsWater )
                        {
                            for ( B = 0; B <= tmpPassageNodeA.ConnectionCount - 1; B++ )
                            {
                                tmpPassageNodeB = tmpPassageNodeA.Connections[B].GetOther();
                                PassageNodesMinLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMinLevel, tmpPassageNodeA.Level, MaxLevelTransition);
                                PassageNodesMaxLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMaxLevel, tmpPassageNodeA.Level, MaxLevelTransition);
                            }
                        }
                    }
                }
                else if ( tmpPassageNodeA.Level < 0 )
                {
                    ReturnResult.ProblemAdd("Error: Node height not set");
                    return ReturnResult;
                }
            }
            //set level of edge points only connected to another border point
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                tmpPassageNodeA = PassageNodes[0, A];
                if ( tmpPassageNodeA.IsOnBorder && tmpPassageNodeA.Level < 0 )
                {
                    tmpPassageNodeC = null;
                    CanDoFlatsAroundWater = true;
                    for ( B = 0; B <= tmpPassageNodeA.ConnectionCount - 1; B++ )
                    {
                        tmpPassageNodeB = tmpPassageNodeA.Connections[B].GetOther();
                        if ( tmpPassageNodeB.Level >= 0 )
                        {
                            if ( HeightsArgs.PassageNodesMinLevel.Nodes[tmpPassageNodeA.Num] <= tmpPassageNodeB.Level &&
                                 HeightsArgs.PassageNodesMaxLevel.Nodes[tmpPassageNodeA.Num] >= tmpPassageNodeB.Level )
                            {
                                if ( tmpPassageNodeC == null )
                                {
                                    tmpPassageNodeC = tmpPassageNodeB;
                                }
                            }
                        }
                        if ( HeightsArgs.PassageNodesMinLevel.Nodes[tmpPassageNodeB.Num] > 0 )
                        {
                            CanDoFlatsAroundWater = false;
                        }
                    }
                    if ( tmpPassageNodeC == null )
                    {
                        ReturnResult.ProblemAdd("Error: No connection for border node");
                        return ReturnResult;
                    }
                    BestNum = tmpPassageNodeC.Level;
                    PassageNodesMinLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMinLevel, BestNum, MaxLevelTransition);
                    PassageNodesMaxLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMaxLevel, BestNum, MaxLevelTransition);
                    for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                    {
                        PassageNodes[D, A].IsWater = tmpPassageNodeC.IsWater && CanDoFlatsAroundWater;
                        PassageNodes[D, A].Level = BestNum;
                    }
                    if ( tmpPassageNodeA.IsWater )
                    {
                        for ( B = 0; B <= tmpPassageNodeA.ConnectionCount - 1; B++ )
                        {
                            tmpPassageNodeB = tmpPassageNodeA.Connections[B].GetOther();
                            PassageNodesMinLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMinLevel, tmpPassageNodeA.Level, MaxLevelTransition);
                            PassageNodesMaxLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMaxLevel, tmpPassageNodeA.Level, MaxLevelTransition);
                        }
                    }
                }
            }

            RampBase = 1.0D;
            MaxDisconnectionDist = 99999.0F;

            clsResult RampResult = GenerateRamps();
            ReturnResult.Add(RampResult);

            return ReturnResult;
        }

        private class clsNearest
        {
            public int Num = -1;
            public clsPassageNode[] NodeA;
            public clsPassageNode[] NodeB;
            public int NodeCount;
            public float Dist2;
            public int BlockedCount;
            public clsNearest[] BlockedNearests;
            public int BlockedNearestCount;
            public bool Invalid;
        }

        private class clsTestNearestArgs
        {
            public int MaxConDist2;
            public int MinConDist;
            public clsPassageNode PassageNodeA;
            public clsPassageNode PassageNodeB;
        }

        private bool TestNearest(clsTestNearestArgs Args)
        {
            sXY_int XY_int = new sXY_int();
            clsNearest NearestA = default(clsNearest);
            int Dist2 = 0;
            int A = 0;
            int B = 0;
            int ReflectionNum = 0;
            int ReflectionCount = 0;

            if ( Args.PassageNodeA.MirrorNum != 0 )
            {
                Debugger.Break();
                return false;
            }

            XY_int.X = Args.PassageNodeB.Pos.X - Args.PassageNodeA.Pos.X;
            XY_int.Y = Args.PassageNodeB.Pos.Y - Args.PassageNodeA.Pos.Y;
            Dist2 = XY_int.X * XY_int.X + XY_int.Y * XY_int.Y;
            if ( Dist2 > Args.MaxConDist2 )
            {
                return false;
            }
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    if ( PassageNodes[B, A] != Args.PassageNodeA && PassageNodes[B, A] != Args.PassageNodeB )
                    {
                        XY_int = MathUtil.PointGetClosestPosOnLine(Args.PassageNodeA.Pos, Args.PassageNodeB.Pos, PassageNodes[B, A].Pos);
                        if ( (XY_int - PassageNodes[B, A].Pos).ToDoubles().GetMagnitude() < Args.MinConDist )
                        {
                            return false;
                        }
                    }
                }
            }

            NearestA = new clsNearest();
            NearestA.Num = NearestCount;
            NearestA.Dist2 = Dist2;
            if ( Args.PassageNodeA.MirrorNum == Args.PassageNodeB.MirrorNum )
            {
                NearestA.NodeA = new clsPassageNode[SymmetryBlockCount];
                NearestA.NodeB = new clsPassageNode[SymmetryBlockCount];
                for ( A = 0; A <= SymmetryBlockCount - 1; A++ )
                {
                    NearestA.NodeA[A] = PassageNodes[A, Args.PassageNodeA.Num];
                    NearestA.NodeB[A] = PassageNodes[A, Args.PassageNodeB.Num];
                }
                NearestA.NodeCount = SymmetryBlockCount;
            }
            else
            {
                if ( SymmetryIsRotational )
                {
                    NearestA.NodeA = new clsPassageNode[SymmetryBlockCount];
                    NearestA.NodeB = new clsPassageNode[SymmetryBlockCount];
                    ReflectionCount = (int)(SymmetryBlockCount / 2.0D);
                    for ( ReflectionNum = 0; ReflectionNum <= ReflectionCount - 1; ReflectionNum++ )
                    {
                        if ( SymmetryBlocks[0].ReflectToNum[ReflectionNum] == Args.PassageNodeB.MirrorNum )
                        {
                            break;
                        }
                    }
                    if ( ReflectionNum == ReflectionCount )
                    {
                        return false;
                    }
                    for ( A = 0; A <= SymmetryBlockCount - 1; A++ )
                    {
                        NearestA.NodeA[A] = PassageNodes[A, Args.PassageNodeA.Num];
                        NearestA.NodeB[A] = PassageNodes[SymmetryBlocks[A].ReflectToNum[ReflectionNum], Args.PassageNodeB.Num];
                    }
                    NearestA.NodeCount = SymmetryBlockCount;
                }
                else
                {
                    if ( Args.PassageNodeA.Num != Args.PassageNodeB.Num )
                    {
                        return false;
                    }
                    if ( SymmetryBlockCount == 4 )
                    {
                        NearestA.NodeA = new clsPassageNode[2];
                        NearestA.NodeB = new clsPassageNode[2];
                        ReflectionCount = (int)(SymmetryBlockCount / 2.0D);
                        for ( ReflectionNum = 0; ReflectionNum <= ReflectionCount - 1; ReflectionNum++ )
                        {
                            if ( SymmetryBlocks[0].ReflectToNum[ReflectionNum] == Args.PassageNodeB.MirrorNum )
                            {
                                break;
                            }
                        }
                        if ( ReflectionNum == ReflectionCount )
                        {
                            return false;
                        }
                        NearestA.NodeA[0] = Args.PassageNodeA;
                        NearestA.NodeB[0] = Args.PassageNodeB;
                        B = Convert.ToInt32(SymmetryBlocks[0].ReflectToNum[1 - ReflectionNum]);
                        NearestA.NodeA[1] = PassageNodes[B, Args.PassageNodeA.Num];
                        NearestA.NodeB[1] = PassageNodes[SymmetryBlocks[B].ReflectToNum[ReflectionNum], Args.PassageNodeB.Num];
                        NearestA.NodeCount = 2;
                    }
                    else
                    {
                        NearestA.NodeA = new clsPassageNode[1];
                        NearestA.NodeB = new clsPassageNode[1];
                        NearestA.NodeA[0] = Args.PassageNodeA;
                        NearestA.NodeB[0] = Args.PassageNodeB;
                        NearestA.NodeCount = 1;
                    }
                }
            }

            NearestA.BlockedNearests = new clsNearest[512];
            Nearests[NearestCount] = NearestA;
            NearestCount++;

            return true;
        }

        public class clsNodeTag
        {
            public sXY_int Pos;
        }

        public float GetNodePosDist(PathfinderNode NodeA, PathfinderNode NodeB)
        {
            clsNodeTag TagA = (clsNodeTag)NodeA.Tag;
            clsNodeTag TagB = (clsNodeTag)NodeB.Tag;

            return Convert.ToSingle((TagA.Pos - TagB.Pos).ToDoubles().GetMagnitude());
        }

        public void CalcNodePos(PathfinderNode Node, ref Position.XY_dbl Pos, ref int SampleCount)
        {
            if ( Node.GetLayer.GetNetwork_LayerNum == 0 )
            {
                clsNodeTag NodeTag = default(clsNodeTag);
                NodeTag = (clsNodeTag)Node.Tag;
                Pos.X += NodeTag.Pos.X;
                Pos.Y += NodeTag.Pos.Y;
            }
            else
            {
                int A = 0;
                for ( A = 0; A <= Node.GetChildNodeCount - 1; A++ )
                {
                    CalcNodePos(Node.get_GetChildNode(A), ref Pos, ref SampleCount);
                }
                SampleCount += Node.GetChildNodeCount;
            }
        }

        public clsResult GenerateLayoutTerrain()
        {
            clsResult ReturnResult = new clsResult("Terrain heights");

            clsNodeTag NodeTag = default(clsNodeTag);
            PathfinderNode tmpNodeA = default(PathfinderNode);
            PathfinderNode tmpNodeB = default(PathfinderNode);
            int A = 0;
            int B = 0;
            int C = 0;
            int D = 0;
            int X = 0;
            int Y = 0;
            sXY_int XY_int = new sXY_int();
            double Dist = 0;
            double BestDist = 0;
            bool Flag = default(bool);

            Map = new clsMap(TileSize);
            GenerateTerrainTiles = new GenerateTerrainTile[Map.Terrain.TileSize.X, Map.Terrain.TileSize.Y];
            GenerateTerrainVertices = new GenerateTerrainVertex[Map.Terrain.TileSize.X + 1, Map.Terrain.TileSize.Y + 1];

            //set terrain heights

            VertexPathMap = new PathfinderNetwork();

            for ( Y = 0; Y <= Map.Terrain.TileSize.Y; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X; X++ )
                {
                    GenerateTerrainVertices[X, Y] = new GenerateTerrainVertex();
                    GenerateTerrainVertices[X, Y].Node = new PathfinderNode(VertexPathMap);
                    NodeTag = new clsNodeTag();
                    NodeTag.Pos = new sXY_int(X * 128, Y * 128);
                    GenerateTerrainVertices[X, Y].Node.Tag = NodeTag;
                }
            }
            for ( Y = 0; Y <= Map.Terrain.TileSize.Y; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X; X++ )
                {
                    tmpNodeA = GenerateTerrainVertices[X, Y].Node;
                    if ( X > 0 )
                    {
                        tmpNodeB = GenerateTerrainVertices[X - 1, Y].Node;
                        GenerateTerrainVertices[X, Y].LeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                    }
                    if ( Y > 0 )
                    {
                        if ( X > 0 )
                        {
                            tmpNodeB = GenerateTerrainVertices[X - 1, Y - 1].Node;
                            GenerateTerrainVertices[X, Y].TopLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        }
                        tmpNodeB = GenerateTerrainVertices[X, Y - 1].Node;
                        GenerateTerrainVertices[X, Y].TopLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        if ( X < Map.Terrain.TileSize.X )
                        {
                            tmpNodeB = GenerateTerrainVertices[X + 1, Y - 1].Node;
                            GenerateTerrainVertices[X, Y].TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        }
                    }
                    if ( X < Map.Terrain.TileSize.X )
                    {
                        tmpNodeB = GenerateTerrainVertices[X + 1, Y].Node;
                        GenerateTerrainVertices[X, Y].RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                    }
                    if ( Y < Map.Terrain.TileSize.Y )
                    {
                        if ( X > 0 )
                        {
                            tmpNodeB = GenerateTerrainVertices[X - 1, Y + 1].Node;
                            GenerateTerrainVertices[X, Y].BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        }
                        tmpNodeB = GenerateTerrainVertices[X, Y + 1].Node;
                        GenerateTerrainVertices[X, Y].BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        if ( X < Map.Terrain.TileSize.X )
                        {
                            tmpNodeB = GenerateTerrainVertices[X + 1, Y + 1].Node;
                            GenerateTerrainVertices[X, Y].BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        }
                    }
                }
            }

            VertexPathMap.LargeArraysResize();
            VertexPathMap.FindCalc();

            PathfinderLayer BaseLayer = VertexPathMap.get_GetNodeLayer(0);
            PathfinderLayer JitterLayer = VertexPathMap.get_GetNodeLayer(JitterScale);
            A = JitterLayer.GetNodeCount - 1;
            int[] NodeLevel = new int[A + 1];
            clsBaseNodeLevels BaseNodeLevel = new clsBaseNodeLevels();
            BaseNodeLevel.NodeLevels = new float[BaseLayer.GetNodeCount];

            //set position of jitter layer nodes

            Position.XY_dbl XY_dbl = default(Position.XY_dbl);

            if ( A > 0 )
            {
                for ( B = 0; B <= A; B++ )
                {
                    tmpNodeA = JitterLayer.get_GetNode(B);
                    C = 0;
                    XY_dbl.X = 0.0D;
                    XY_dbl.Y = 0.0D;
                    CalcNodePos(tmpNodeA, ref XY_dbl, ref C);
                    NodeTag = new clsNodeTag();
                    NodeTag.Pos.X = (int)(XY_dbl.X / C);
                    NodeTag.Pos.Y = (int)(XY_dbl.Y / C);
                    tmpNodeA.Tag = NodeTag;
                }
            }

            //set node heights

            clsConnection BestConnection = default(clsConnection);
            clsPassageNode BestNode = default(clsPassageNode);

            for ( A = 0; A <= JitterLayer.GetNodeCount - 1; A++ )
            {
                NodeTag = (clsNodeTag)(JitterLayer.get_GetNode(A).Tag);
                NodeLevel[A] = -1;
                BestDist = float.MaxValue;
                BestConnection = null;
                BestNode = null;
                for ( B = 0; B <= ConnectionCount - 1; B++ )
                {
                    //If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) Then
                    if ( Connections[B].PassageNodeA.Level == Connections[B].PassageNodeB.Level )
                    {
                        //only do this if the levels are the same
                        //this is to make sure nodes that are connected are actually connected on the terrain
                        XY_int = MathUtil.PointGetClosestPosOnLine(Connections[B].PassageNodeA.Pos, Connections[B].PassageNodeB.Pos, NodeTag.Pos);
                        Dist = Convert.ToSingle((XY_int - NodeTag.Pos).ToDoubles().GetMagnitude());
                        if ( Dist < BestDist )
                        {
                            BestDist = Dist;
                            if ( (NodeTag.Pos - Connections[B].PassageNodeA.Pos).ToDoubles().GetMagnitude() <=
                                 (NodeTag.Pos - Connections[B].PassageNodeB.Pos).ToDoubles().GetMagnitude() )
                            {
                                BestNode = Connections[B].PassageNodeA;
                            }
                            else
                            {
                                BestNode = Connections[B].PassageNodeB;
                            }
                            Flag = true;
                        }
                    }
                }
                for ( C = 0; C <= PassageNodeCount - 1; C++ )
                {
                    //If Not PassageNodesA(C).IsOnBorder Then
                    for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                    {
                        Dist = Convert.ToSingle((NodeTag.Pos - PassageNodes[D, C].Pos).ToDoubles().GetMagnitude());
                        if ( Dist < BestDist )
                        {
                            BestDist = Dist;
                            BestNode = PassageNodes[D, C];
                            Flag = true;
                        }
                    }
                    //End If
                }
                if ( Flag )
                {
                    NodeLevel[A] = BestNode.Level;
                }
                else
                {
                    NodeLevel[A] = BestConnection.PassageNodeA.Level;
                }
                if ( NodeLevel[A] < 0 )
                {
                    ReturnResult.ProblemAdd("Error: Node height is not set.");
                    return ReturnResult;
                }
            }

            for ( A = 0; A <= LevelCount - 1; A++ )
            {
                for ( B = 0; B <= JitterLayer.GetNodeCount - 1; B++ )
                {
                    if ( NodeLevel[B] >= A )
                    {
                        SetBaseLevel(JitterLayer.get_GetNode(B), A, BaseNodeLevel);
                    }
                }
            }

            //make ramp slopes

            int MinRampLength = ((int)(LevelHeight * Map.HeightMultiplier * 2.0D)) + 128;
            clsSetBaseLevelRampArgs RampArgs = new clsSetBaseLevelRampArgs();
            RampArgs.BaseLevel = BaseNodeLevel;
            RampArgs.RampRadius = 320.0F;
            for ( B = 0; B <= ConnectionCount - 1; B++ )
            {
                RampArgs.Connection = Connections[B];
                RampArgs.RampLength =
                    Math.Max(Convert.ToInt32((Connections[B].PassageNodeA.Pos - Connections[B].PassageNodeB.Pos).ToDoubles().GetMagnitude() * 0.75D),
                        MinRampLength * Math.Abs(Connections[B].PassageNodeA.Level - Connections[B].PassageNodeB.Level));
                for ( A = 0; A <= JitterLayer.GetNodeCount - 1; A++ )
                {
                    if ( Connections[B].IsRamp )
                    {
                        NodeTag = (clsNodeTag)(JitterLayer.get_GetNode(A).Tag);
                        XY_int = MathUtil.PointGetClosestPosOnLine(Connections[B].PassageNodeA.Pos, Connections[B].PassageNodeB.Pos, NodeTag.Pos);
                        Dist = Convert.ToSingle((XY_int - NodeTag.Pos).ToDoubles().GetMagnitude());
                        if ( Dist < RampArgs.RampLength * 2.0F )
                        {
                            SetBaseLevelRamp(RampArgs, JitterLayer.get_GetNode(A));
                        }
                    }
                }
            }

            for ( A = 0; A <= BaseLayer.GetNodeCount - 1; A++ )
            {
                NodeTag = (clsNodeTag)(BaseLayer.get_GetNode(A).Tag);
                Map.Terrain.Vertices[(int)(NodeTag.Pos.X / 128.0F), (int)(NodeTag.Pos.Y / 128.0F)].Height = (byte)(BaseNodeLevel.NodeLevels[A] * LevelHeight);
            }

            return ReturnResult;
        }

        public void GenerateTilePathMap()
        {
            clsNodeTag NodeTag = default(clsNodeTag);
            PathfinderNode tmpNodeA = default(PathfinderNode);
            PathfinderNode tmpNodeB = default(PathfinderNode);
            int X = 0;
            int Y = 0;

            TilePathMap = new PathfinderNetwork();

            for ( Y = 0; Y <= Map.Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X - 1; X++ )
                {
                    GenerateTerrainTiles[X, Y] = new GenerateTerrainTile();
                    GenerateTerrainTiles[X, Y].Node = new PathfinderNode(TilePathMap);
                    NodeTag = new clsNodeTag();
                    NodeTag.Pos = new sXY_int((int)((X + 0.5D) * 128.0D), (int)((Y + 0.5D) * 128.0D));
                    GenerateTerrainTiles[X, Y].Node.Tag = NodeTag;
                }
            }
            for ( Y = 0; Y <= Map.Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X - 1; X++ )
                {
                    tmpNodeA = GenerateTerrainTiles[X, Y].Node;
                    if ( X > 0 )
                    {
                        tmpNodeB = GenerateTerrainTiles[X - 1, Y].Node;
                        GenerateTerrainTiles[X, Y].LeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                    }
                    if ( Y > 0 )
                    {
                        if ( X > 0 )
                        {
                            tmpNodeB = GenerateTerrainTiles[X - 1, Y - 1].Node;
                            GenerateTerrainTiles[X, Y].TopLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        }
                        tmpNodeB = GenerateTerrainTiles[X, Y - 1].Node;
                        GenerateTerrainTiles[X, Y].TopLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        if ( X < Map.Terrain.TileSize.X - 1 )
                        {
                            tmpNodeB = GenerateTerrainTiles[X + 1, Y - 1].Node;
                            GenerateTerrainTiles[X, Y].TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        }
                    }
                    if ( X < Map.Terrain.TileSize.X - 1 )
                    {
                        tmpNodeB = GenerateTerrainTiles[X + 1, Y].Node;
                        GenerateTerrainTiles[X, Y].RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                    }
                    if ( Y < Map.Terrain.TileSize.Y - 1 )
                    {
                        if ( X > 0 )
                        {
                            tmpNodeB = GenerateTerrainTiles[X - 1, Y + 1].Node;
                            GenerateTerrainTiles[X, Y].BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        }
                        tmpNodeB = GenerateTerrainTiles[X, Y + 1].Node;
                        GenerateTerrainTiles[X, Y].BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        if ( X < Map.Terrain.TileSize.X - 1 )
                        {
                            tmpNodeB = GenerateTerrainTiles[X + 1, Y + 1].Node;
                            GenerateTerrainTiles[X, Y].BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                        }
                    }
                }
            }

            TilePathMap.LargeArraysResize();
            TilePathMap.FindCalc();
        }

        private class clsPassageNodeLevels
        {
            public int[] Nodes;
        }

        private void PassageNodesMinLevelSet(clsPassageNode PassageNode, clsPassageNodeLevels PassageNodesMinLevel, int Level, int LevelChange)
        {
            int A = 0;
            clsPassageNode tmpPassageNode = default(clsPassageNode);

            if ( Level > PassageNodesMinLevel.Nodes[PassageNode.Num] )
            {
                PassageNodesMinLevel.Nodes[PassageNode.Num] = Level;
                for ( A = 0; A <= PassageNode.ConnectionCount - 1; A++ )
                {
                    tmpPassageNode = PassageNode.Connections[A].GetOther();
                    if ( PassageNode.IsNearBorder || tmpPassageNode.IsNearBorder )
                    {
                        PassageNodesMinLevelSet(tmpPassageNode, PassageNodesMinLevel, Level - 1, LevelChange);
                    }
                    else
                    {
                        PassageNodesMinLevelSet(tmpPassageNode, PassageNodesMinLevel, Level - LevelChange, LevelChange);
                    }
                }
            }
        }

        private void PassageNodesMaxLevelSet(clsPassageNode PassageNode, clsPassageNodeLevels PassageNodesMaxLevel, int Level, int LevelChange)
        {
            int A = 0;
            clsPassageNode tmpPassageNode = default(clsPassageNode);

            if ( Level < PassageNodesMaxLevel.Nodes[PassageNode.Num] )
            {
                PassageNodesMaxLevel.Nodes[PassageNode.Num] = Level;
                for ( A = 0; A <= PassageNode.ConnectionCount - 1; A++ )
                {
                    tmpPassageNode = PassageNode.Connections[A].GetOther();
                    if ( PassageNode.IsNearBorder || tmpPassageNode.IsNearBorder )
                    {
                        PassageNodesMaxLevelSet(tmpPassageNode, PassageNodesMaxLevel, Level + 1, LevelChange);
                    }
                    else
                    {
                        PassageNodesMaxLevelSet(tmpPassageNode, PassageNodesMaxLevel, Level + LevelChange, LevelChange);
                    }
                }
            }
        }

        private class clsNodeConnectedness
        {
            public float[] NodeConnectedness;
            public bool[,] PassageNodeVisited;
            public PathfinderNetwork PassageNodePathMap;
            public PathfinderNode[,] PassageNodePathNodes;
        }

        private class clsUpdateNodeConnectednessArgs
        {
            public clsPassageNode OriginalNode;
            public clsNodeConnectedness Args;
        }

        private void UpdateNodeConnectedness(clsUpdateNodeConnectednessArgs Args, clsPassageNode PassageNode)
        {
            int A = 0;
            clsConnection tmpConnection = default(clsConnection);
            clsPassageNode tmpOtherNode = default(clsPassageNode);
            int PassableCount = 0;

            Args.Args.PassageNodeVisited[PassageNode.MirrorNum, PassageNode.Num] = true;

            for ( A = 0; A <= PassageNode.ConnectionCount - 1; A++ )
            {
                tmpConnection = PassageNode.Connections[A].Connection;
                if (
                    !(tmpConnection.PassageNodeA.IsOnBorder || tmpConnection.PassageNodeB.IsOnBorder || tmpConnection.PassageNodeA.IsWater ||
                      tmpConnection.PassageNodeB.IsWater) && (tmpConnection.IsRamp || tmpConnection.PassageNodeA.Level == tmpConnection.PassageNodeB.Level) )
                {
                    tmpOtherNode = PassageNode.Connections[A].GetOther();
                    if ( !Args.Args.PassageNodeVisited[tmpOtherNode.MirrorNum, tmpOtherNode.Num] )
                    {
                        UpdateNodeConnectedness(Args, tmpOtherNode);
                    }
                    PassableCount++;
                }
            }

            PathfinderNetwork.PathList[] Paths = null;
            PathfinderNode[] StartNodes = new PathfinderNode[1];
            StartNodes[0] = Args.Args.PassageNodePathNodes[0, Args.OriginalNode.Num];
            Paths = Args.Args.PassageNodePathMap.GetPath(StartNodes, Args.Args.PassageNodePathNodes[PassageNode.MirrorNum, PassageNode.Num], -1, 0);
            Args.Args.NodeConnectedness[Args.OriginalNode.Num] += (float)(PassableCount * Math.Pow(0.999D, Paths[0].Paths[0].Value));
        }

        private class clsUpdateNetworkConnectednessArgs
        {
            public bool[] PassageNodeUpdated;
            public int SymmetryBlockCount;
            public clsNodeConnectedness Args;
        }

        private void UpdateNetworkConnectedness(clsUpdateNetworkConnectednessArgs Args, clsPassageNode PassageNode)
        {
            int A = 0;
            clsConnection tmpConnection = default(clsConnection);
            clsPassageNode tmpOtherNode = default(clsPassageNode);
            clsUpdateNodeConnectednessArgs NodeConnectednessArgs = new clsUpdateNodeConnectednessArgs();
            int B = 0;
            int C = 0;

            Args.PassageNodeUpdated[PassageNode.Num] = true;

            for ( A = 0; A <= PassageNode.ConnectionCount - 1; A++ )
            {
                tmpConnection = PassageNode.Connections[A].Connection;
                if (
                    !(tmpConnection.PassageNodeA.IsOnBorder || tmpConnection.PassageNodeB.IsOnBorder || tmpConnection.PassageNodeA.IsWater ||
                      tmpConnection.PassageNodeB.IsWater) && (tmpConnection.IsRamp || tmpConnection.PassageNodeA.Level == tmpConnection.PassageNodeB.Level) )
                {
                    tmpOtherNode = PassageNode.Connections[A].GetOther();
                    if ( !Args.PassageNodeUpdated[tmpOtherNode.Num] && tmpOtherNode.MirrorNum == 0 )
                    {
                        for ( B = 0; B <= PassageNodeCount - 1; B++ )
                        {
                            for ( C = 0; C <= Args.SymmetryBlockCount - 1; C++ )
                            {
                                Args.Args.PassageNodeVisited[C, B] = false;
                            }
                        }
                        NodeConnectednessArgs.OriginalNode = PassageNode;
                        NodeConnectednessArgs.Args = Args.Args;
                        UpdateNodeConnectedness(NodeConnectednessArgs, PassageNode);
                    }
                }
            }
        }

        private class clsOilBalanceLoopArgs
        {
            public clsPassageNode[] OilNodes;
            public int[] OilClusterSizes;
            public clsOilPossibilities OilPossibilities;
            public double[] PlayerOilScore;
        }

        private void OilBalanceLoop(clsOilBalanceLoopArgs Args, int LoopNum)
        {
            int A = 0;
            int C = 0;
            int NextLoopNum = LoopNum + 1;
            clsPassageNode tmpPassageNodeA = default(clsPassageNode);

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                tmpPassageNodeA = PassageNodes[0, A];
                if ( tmpPassageNodeA.PlayerBaseNum < 0 && !tmpPassageNodeA.IsOnBorder && tmpPassageNodeA.OilCount == 0 && !tmpPassageNodeA.IsWater )
                {
                    for ( C = 0; C <= LoopNum - 1; C++ )
                    {
                        if ( Args.OilNodes[C] == tmpPassageNodeA )
                        {
                            break;
                        }
                    }
                    if ( C == LoopNum )
                    {
                        Args.OilNodes[LoopNum] = tmpPassageNodeA;
                        if ( NextLoopNum < OilAtATime )
                        {
                            OilBalanceLoop(Args, NextLoopNum);
                        }
                        else
                        {
                            OilValueCalc(Args);
                        }
                    }
                }
            }
        }

        public class clsOilPossibilities
        {
            public class clsPossibility
            {
                public clsPassageNode[] Nodes;
                public double Score;
                public double[] PlayerOilScoreAddition;
            }

            public clsPossibility BestPossibility;

            public void NewPossibility(clsPossibility Possibility)
            {
                if ( BestPossibility == null )
                {
                    BestPossibility = Possibility;
                }
                else if ( Possibility.Score < BestPossibility.Score )
                {
                    BestPossibility = Possibility;
                }
            }
        }

        private void OilValueCalc(clsOilBalanceLoopArgs Args)
        {
            //Dim OilDistScore As Double
            //Dim OilStraightDistScore As Double
            double LowestScore = 0;
            double HighestScore = 0;
            //Dim TotalOilScore As Double
            double UnbalancedScore = 0;
            double dblTemp = 0;
            double Value = 0;
            clsOilPossibilities.clsPossibility NewPossibility = new clsOilPossibilities.clsPossibility();
            double[] BaseOilScore = new double[TopLeftPlayerCount];

            NewPossibility.PlayerOilScoreAddition = new double[TopLeftPlayerCount];

            int NewOilNum = 0;
            int OtherOilNum = 0;
            int NewOilNodeNum = 0;
            int OtherOilNodeNum = 0;
            int SymmetryBlockNum = 0;
            int MapNodeNum = 0;
            int PlayerNum = 0;
            //Dim NewOilCount As Integer
            double OilMassMultiplier = 0;
            double OilDistValue = 0;
            double NearestOilValue = double.MaxValue;

            //OilDistScore = 0.0#
            //OilStraightDistScore = 0.0#
            for ( PlayerNum = 0; PlayerNum <= TopLeftPlayerCount - 1; PlayerNum++ )
            {
                NewPossibility.PlayerOilScoreAddition[PlayerNum] = 0.0D;
            }
            for ( NewOilNum = 0; NewOilNum <= OilAtATime - 1; NewOilNum++ )
            {
                NewOilNodeNum = Args.OilNodes[NewOilNum].Num;
                //other oil to be placed in the first area
                for ( OtherOilNum = NewOilNum + 1; OtherOilNum <= OilAtATime - 1; OtherOilNum++ )
                {
                    OtherOilNodeNum = Args.OilNodes[OtherOilNum].Num;
                    //OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * Args.OilClusterSizes(OtherOilNum)
                    //OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, 0, OtherOilNodeNum)
                    //OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(0, OtherOilNodeNum).Pos)
                    OilDistValue = 4.0D * PassageNodeDists[0, NewOilNodeNum, 0, OtherOilNodeNum];
                    //+ GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(0, OtherOilNodeNum).Pos))
                    if ( OilDistValue < NearestOilValue )
                    {
                        NearestOilValue = OilDistValue;
                    }
                }
                //other oil to be placed in symmetrical areas
                for ( OtherOilNum = 0; OtherOilNum <= OilAtATime - 1; OtherOilNum++ )
                {
                    OtherOilNodeNum = Args.OilNodes[OtherOilNum].Num;
                    //OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * Args.OilClusterSizes(OtherOilNum)
                    for ( SymmetryBlockNum = 1; SymmetryBlockNum <= SymmetryBlockCount - 1; SymmetryBlockNum++ )
                    {
                        //OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, SymmetryBlockNum, OtherOilNodeNum)
                        //OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, OtherOilNodeNum).Pos)
                        OilDistValue = 4.0D * PassageNodeDists[0, NewOilNodeNum, SymmetryBlockNum, OtherOilNodeNum];
                        // + GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, OtherOilNodeNum).Pos))
                        if ( OilDistValue < NearestOilValue )
                        {
                            NearestOilValue = OilDistValue;
                        }
                    }
                }
                //oil on the map
                for ( MapNodeNum = 0; MapNodeNum <= PassageNodeCount - 1; MapNodeNum++ )
                {
                    for ( SymmetryBlockNum = 0; SymmetryBlockNum <= SymmetryBlockCount - 1; SymmetryBlockNum++ )
                    {
                        if ( PassageNodes[SymmetryBlockNum, MapNodeNum].OilCount > 0 )
                        {
                            //OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * PassageNodes(SymmetryBlockNum, MapNodeNum).OilCount
                            //OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, SymmetryBlockNum, MapNodeNum)
                            //OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, MapNodeNum).Pos)
                            OilDistValue = 4.0D * OilMassMultiplier / PassageNodeDists[0, NewOilNodeNum, SymmetryBlockNum, MapNodeNum];
                            // + GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, MapNodeNum).Pos))
                            if ( OilDistValue < NearestOilValue )
                            {
                                NearestOilValue = OilDistValue;
                            }
                        }
                    }
                }
                //extra oil score for players
                for ( PlayerNum = 0; PlayerNum <= TopLeftPlayerCount - 1; PlayerNum++ )
                {
                    BaseOilScore[PlayerNum] = 0.0D;
                    for ( SymmetryBlockNum = 0; SymmetryBlockNum <= SymmetryBlockCount - 1; SymmetryBlockNum++ )
                    {
                        dblTemp =
                            Convert.ToDouble(PassageNodeDists[0, PlayerBases[PlayerNum].Nodes[0].Num, SymmetryBlockNum, NewOilNodeNum] * 2.0D +
                                                    (PlayerBases[PlayerNum].Nodes[0].Pos - PassageNodes[SymmetryBlockNum, NewOilNodeNum].Pos).ToDoubles().GetMagnitude());
                        BaseOilScore[PlayerNum] += 100.0D / dblTemp;
                    }
                }
                //TotalOilScore = 0.0#
                //For PlayerNum = 0 To PlayerCount - 1
                //    TotalOilScore += BaseOilScore(PlayerNum)
                //Next
                for ( PlayerNum = 0; PlayerNum <= TopLeftPlayerCount - 1; PlayerNum++ )
                {
                    NewPossibility.PlayerOilScoreAddition[PlayerNum] += Args.OilClusterSizes[NewOilNum] * BaseOilScore[PlayerNum];
                }
            }

            LowestScore = double.MaxValue;
            HighestScore = double.MinValue;
            for ( PlayerNum = 0; PlayerNum <= TopLeftPlayerCount - 1; PlayerNum++ )
            {
                dblTemp = Args.PlayerOilScore[PlayerNum] + NewPossibility.PlayerOilScoreAddition[PlayerNum];
                if ( dblTemp < LowestScore )
                {
                    LowestScore = dblTemp;
                }
                if ( dblTemp > HighestScore )
                {
                    HighestScore = dblTemp;
                }
            }
            UnbalancedScore = HighestScore - LowestScore;

            //NewOilCount = 0
            //For NewOilNum = 0 To OilAtATime - 1
            //    NewOilCount += Args.OilClusterSizes(NewOilNum)
            //Next
            //divide all dists by the number of oil resources placed. does not include other symmetries, since they were never added in, and are exactly the same.
            if ( NearestOilValue == double.MaxValue )
            {
                NearestOilValue = 0.0D;
            }
            else
            {
                NearestOilValue = 10.0D / NearestOilValue;
            }
            //Value = OilDispersion * (OilDistScore * 4.0# + OilStraightDistScore) + UnbalancedScore
            Value = OilDispersion * NearestOilValue + UnbalancedScore;
            NewPossibility.Score = Value;
            NewPossibility.Nodes = new clsPassageNode[OilAtATime];
            for ( NewOilNum = 0; NewOilNum <= OilAtATime - 1; NewOilNum++ )
            {
                NewPossibility.Nodes[NewOilNum] = Args.OilNodes[NewOilNum];
            }
            Args.OilPossibilities.NewPossibility(NewPossibility);
        }

        private class clsBaseNodeLevels
        {
            public float[] NodeLevels;
        }

        private void SetBaseLevel(PathfinderNode Node, int NewLevel, clsBaseNodeLevels BaseLevel)
        {
            if ( Node.GetChildNodeCount == 0 )
            {
                int A = 0;
                float Height = 0;
                float Lowest = NewLevel;
                for ( A = 0; A <= Node.GetConnectionCount - 1; A++ )
                {
                    Height = BaseLevel.NodeLevels[Node.get_GetConnection(A).GetOtherNode(Node).GetLayer_NodeNum];
                    if ( Height < Lowest )
                    {
                        Lowest = Height;
                    }
                }
                if ( NewLevel - Lowest > 1.0F )
                {
                    BaseLevel.NodeLevels[Node.GetLayer_NodeNum] = Lowest + 1.0F;
                }
                else
                {
                    BaseLevel.NodeLevels[Node.GetLayer_NodeNum] = NewLevel;
                }
            }
            else
            {
                int A = 0;
                for ( A = 0; A <= Node.GetChildNodeCount - 1; A++ )
                {
                    SetBaseLevel(Node.get_GetChildNode(A), NewLevel, BaseLevel);
                }
            }
        }

        private class clsSetBaseLevelRampArgs
        {
            public clsConnection Connection;
            public clsBaseNodeLevels BaseLevel = new clsBaseNodeLevels();
            public int RampLength;
            public float RampRadius;
        }

        private void SetBaseLevelRamp(clsSetBaseLevelRampArgs Args, PathfinderNode Node)
        {
            if ( Node.GetChildNodeCount == 0 )
            {
                clsNodeTag NodeTag = (clsNodeTag)Node.Tag;
                sXY_int XY_int = MathUtil.PointGetClosestPosOnLine(Args.Connection.PassageNodeA.Pos, Args.Connection.PassageNodeB.Pos, NodeTag.Pos);
                float ConnectionLength = Convert.ToSingle((Args.Connection.PassageNodeA.Pos - Args.Connection.PassageNodeB.Pos).ToDoubles().GetMagnitude());
                float Extra = ConnectionLength - Args.RampLength;
                float ConnectionPos = Convert.ToSingle((XY_int - Args.Connection.PassageNodeA.Pos).ToDoubles().GetMagnitude());
                float RampPos = MathUtil.Clamp_sng((float)((ConnectionPos - Extra / 2.0F) / Args.RampLength), 0.0F, 1.0F);
                int Layer_NodeNum = Node.GetLayer_NodeNum;
                RampPos = (float)(1.0D - (Math.Cos(RampPos * Math.PI) + 1.0D) / 2.0D);
                if ( RampPos > 0.0F & RampPos < 1.0F )
                {
                    float Dist2 = Convert.ToSingle((NodeTag.Pos - XY_int).ToDoubles().GetMagnitude());
                    if ( Dist2 < Args.RampRadius )
                    {
                        float Dist2Factor = 1.0F; //Math.Min(3.0F - 3.0F * Dist2 / 384.0F, 1.0F) 'distance fading
                        if ( Args.BaseLevel.NodeLevels[Layer_NodeNum] == Conversion.Int(Args.BaseLevel.NodeLevels[Layer_NodeNum]) )
                        {
                            Args.BaseLevel.NodeLevels[Layer_NodeNum] = Args.BaseLevel.NodeLevels[Layer_NodeNum] * (1.0F - Dist2Factor) +
                                                                       (Args.Connection.PassageNodeA.Level * (1.0F - RampPos) +
                                                                        Args.Connection.PassageNodeB.Level * RampPos) * Dist2Factor;
                        }
                        else
                        {
                            Args.BaseLevel.NodeLevels[Layer_NodeNum] = (Args.BaseLevel.NodeLevels[Layer_NodeNum] * (2.0F - Dist2Factor) +
                                                                        (Args.Connection.PassageNodeA.Level * (1.0F - RampPos) +
                                                                         Args.Connection.PassageNodeB.Level * RampPos) * Dist2Factor) / 2.0F;
                        }
                    }
                }
            }
            else
            {
                int A = 0;
                for ( A = 0; A <= Node.GetChildNodeCount - 1; A++ )
                {
                    SetBaseLevelRamp(Args, Node.get_GetChildNode(A));
                }
            }
        }

        public void TerrainBlockPaths()
        {
            int X = 0;
            int Y = 0;

            for ( Y = 0; Y <= Map.Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X - 1; X++ )
                {
                    if ( Map.Terrain.Tiles[X, Y].Texture.TextureNum >= 0 )
                    {
                        if ( GenerateTileset.Tileset.Tiles[Map.Terrain.Tiles[X, Y].Texture.TextureNum].DefaultType == App.TileTypeNum_Cliff ||
                             GenerateTileset.Tileset.Tiles[Map.Terrain.Tiles[X, Y].Texture.TextureNum].DefaultType == App.TileTypeNum_Water )
                        {
                            TileNodeBlock(X, Y);
                        }
                    }
                }
            }
            TilePathMap.FindCalc();
        }

        public BooleanMap GetWaterMap()
        {
            BooleanMap ReturnResult = new BooleanMap();
            float BestDist = 0;
            bool BestIsWater = default(bool);
            sXY_int Pos = new sXY_int();
            float Dist = 0;
            int B = 0;
            int C = 0;
            sXY_int XY_int = new sXY_int();
            int X = 0;
            int Y = 0;

            ReturnResult.Blank(Map.Terrain.TileSize.X + 1, Map.Terrain.TileSize.Y + 1);
            for ( Y = 0; Y <= Map.Terrain.TileSize.Y; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X; X++ )
                {
                    BestDist = float.MaxValue;
                    Pos = new sXY_int(X * App.TerrainGridSpacing, Y * App.TerrainGridSpacing);
                    for ( B = 0; B <= ConnectionCount - 1; B++ )
                    {
                        //If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) Then
                        if ( Connections[B].PassageNodeA.IsWater == Connections[B].PassageNodeB.IsWater )
                        {
                            //only do this if the waters are the same
                            //this is to make sure nodes that are connected are actually connected as water
                            XY_int = MathUtil.PointGetClosestPosOnLine(Connections[B].PassageNodeA.Pos, Connections[B].PassageNodeB.Pos, Pos);
                            Dist = Convert.ToSingle((XY_int - Pos).ToDoubles().GetMagnitude());
                            if ( Dist < BestDist )
                            {
                                BestDist = Dist;
                                if ( (Pos - Connections[B].PassageNodeA.Pos).ToDoubles().GetMagnitude() <=
                                     (Pos - Connections[B].PassageNodeB.Pos).ToDoubles().GetMagnitude() )
                                {
                                    BestIsWater = Connections[B].PassageNodeA.IsWater;
                                }
                                else
                                {
                                    BestIsWater = Connections[B].PassageNodeB.IsWater;
                                }
                            }
                        }
                    }
                    for ( C = 0; C <= PassageNodeCount - 1; C++ )
                    {
                        for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                        {
                            Dist = Convert.ToSingle((Pos - PassageNodes[B, C].Pos).ToDoubles().GetMagnitude());
                            if ( Dist < BestDist )
                            {
                                BestDist = Dist;
                                BestIsWater = PassageNodes[B, C].IsWater;
                            }
                        }
                    }
                    ReturnResult.ValueData.Value[Y, X] = BestIsWater;
                }
            }
            return ReturnResult;
        }

        public PathfinderNode GetNearestNode(PathfinderNetwork Network, sXY_int Pos, int MinClearance)
        {
            int A = 0;
            double Dist = 0;
            PathfinderNode tmpNode = default(PathfinderNode);
            PathfinderNode BestNode = default(PathfinderNode);
            double BestDist = 0;
            clsNodeTag tmpNodeTag = default(clsNodeTag);

            BestDist = double.MaxValue;
            BestNode = null;
            for ( A = 0; A <= Network.get_GetNodeLayer(0).GetNodeCount - 1; A++ )
            {
                tmpNode = Network.get_GetNodeLayer(0).get_GetNode(A);
                if ( tmpNode.GetClearance >= MinClearance )
                {
                    tmpNodeTag = (clsNodeTag)tmpNode.Tag;
                    Dist = (tmpNodeTag.Pos - Pos).ToDoubles().GetMagnitude();
                    if ( Dist < BestDist )
                    {
                        BestDist = Dist;
                        BestNode = tmpNode;
                    }
                }
            }
            return BestNode;
        }

        private PathfinderNode GetNearestNodeConnection(PathfinderNetwork Network, sXY_int Pos, int MinClearance, float MaxDistance)
        {
            int A = 0;
            PathfinderNode[] TravelNodes = new PathfinderNode[Network.get_GetNodeLayer(0).GetNodeCount * 10];
            int TravelNodeCount = 0;
            float[] NodeTravelDists = new float[Network.get_GetNodeLayer(0).GetNodeCount];
            int TravelNodeNum = 0;
            PathfinderNode CurrentNode = default(PathfinderNode);
            PathfinderNode OtherNode = default(PathfinderNode);
            PathfinderConnection tmpConnection = default(PathfinderConnection);
            PathfinderNode BestNode = null;
            float TravelDist = 0;
            bool Flag = default(bool);

            for ( A = 0; A <= Network.get_GetNodeLayer(0).GetNodeCount - 1; A++ )
            {
                NodeTravelDists[A] = float.MaxValue;
            }
            TravelNodes[0] = GetNearestNode(Network, Pos, 1);
            if ( TravelNodes[0] == null )
            {
                return null;
            }
            TravelNodeCount = 1;
            NodeTravelDists[TravelNodes[0].Layer_NodeNum] = 0.0F;
            while ( TravelNodeNum < TravelNodeCount )
            {
                CurrentNode = TravelNodes[TravelNodeNum];
                if ( CurrentNode.Clearance >= MinClearance )
                {
                    if ( BestNode == null )
                    {
                        BestNode = CurrentNode;
                    }
                    else if ( NodeTravelDists[CurrentNode.Layer_NodeNum] < NodeTravelDists[BestNode.Layer_NodeNum] )
                    {
                        BestNode = CurrentNode;
                    }
                }
                for ( A = 0; A <= CurrentNode.GetConnectionCount - 1; A++ )
                {
                    tmpConnection = CurrentNode.get_GetConnection(A);
                    OtherNode = tmpConnection.GetOtherNode(CurrentNode);
                    TravelDist = NodeTravelDists[CurrentNode.Layer_NodeNum] + tmpConnection.GetValue;
                    if ( BestNode == null )
                    {
                        Flag = true;
                    }
                    else if ( TravelDist < NodeTravelDists[BestNode.Layer_NodeNum] )
                    {
                        Flag = true;
                    }
                    else
                    {
                        Flag = false;
                    }
                    if ( Flag && TravelDist < NodeTravelDists[OtherNode.Layer_NodeNum] )
                    {
                        NodeTravelDists[OtherNode.Layer_NodeNum] = TravelDist;
                        TravelNodes[TravelNodeCount] = OtherNode;
                        TravelNodeCount++;
                    }
                }
                TravelNodeNum++;
            }
            return BestNode;
        }

        public clsUnit PlaceUnitNear(UnitTypeBase TypeBase, sXY_int Pos, clsUnitGroup UnitGroup, int Clearance, int Rotation, int MaxDistFromPos)
        {
            PathfinderNode PosNode = default(PathfinderNode);
            clsNodeTag NodeTag = default(clsNodeTag);
            sXY_int FinalTilePos = new sXY_int();
            sXY_int TilePosA = new sXY_int();
            sXY_int TilePosB = new sXY_int();
            int X2 = 0;
            int Y2 = 0;
            int Remainder = 0;
            sXY_int Footprint = new sXY_int();

            PosNode = GetNearestNodeConnection(TilePathMap, Pos, Clearance, MaxDistFromPos);
            if ( PosNode != null )
            {
                NodeTag = (clsNodeTag)PosNode.Tag;
                if ( (Pos - NodeTag.Pos).ToDoubles().GetMagnitude() <= MaxDistFromPos )
                {
                    clsUnitAdd NewUnitAdd = new clsUnitAdd();
                    NewUnitAdd.Map = Map;
                    NewUnitAdd.StoreChange = true;
                    clsUnit NewUnit = new clsUnit();
                    NewUnitAdd.NewUnit = NewUnit;
                    NewUnit.TypeBase = TypeBase;
                    NewUnit.UnitGroup = UnitGroup;

                    FinalTilePos.X = (int)(Conversion.Int(NodeTag.Pos.X / App.TerrainGridSpacing));
                    FinalTilePos.Y = Conversion.Int(NodeTag.Pos.Y / App.TerrainGridSpacing);
                    Footprint = TypeBase.get_GetFootprintSelected(Rotation);
                    Remainder = Footprint.X % 2;
                    if ( Remainder > 0 )
                    {
                        NewUnit.Pos.Horizontal.X = NodeTag.Pos.X;
                    }
                    else
                    {
                        if ( VBMath.Rnd() >= 0.5F )
                        {
                            NewUnit.Pos.Horizontal.X = (int)(NodeTag.Pos.X - App.TerrainGridSpacing / 2.0D);
                        }
                        else
                        {
                            NewUnit.Pos.Horizontal.X = (int)(NodeTag.Pos.X + App.TerrainGridSpacing / 2.0D);
                        }
                    }
                    Remainder = Footprint.Y % 2;
                    if ( Remainder > 0 )
                    {
                        NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y;
                    }
                    else
                    {
                        if ( VBMath.Rnd() >= 0.5F )
                        {
                            NewUnit.Pos.Horizontal.Y = (int)(NodeTag.Pos.Y - App.TerrainGridSpacing / 2.0D);
                        }
                        else
                        {
                            NewUnit.Pos.Horizontal.Y = (int)(NodeTag.Pos.Y + App.TerrainGridSpacing / 2.0D);
                        }
                    }
                    TilePosA.X = (int)Conversion.Int((double)NewUnit.Pos.Horizontal.X / App.TerrainGridSpacing - Footprint.X / 2.0D + 0.5D);
                    TilePosA.Y = (int)Conversion.Int((double)NewUnit.Pos.Horizontal.Y / App.TerrainGridSpacing - Footprint.Y / 2.0D + 0.5D);
                    TilePosB.X = (int)(Conversion.Int((double)NewUnit.Pos.Horizontal.X / App.TerrainGridSpacing + Footprint.X / 2.0D - 0.5D));
                    TilePosB.Y = (int)(Conversion.Int((double)NewUnit.Pos.Horizontal.Y / App.TerrainGridSpacing + Footprint.Y / 2.0D - 0.5D));
                    NewUnit.Rotation = Rotation;

                    NewUnitAdd.Perform();

                    for ( Y2 = Math.Max(TilePosA.Y, 0); Y2 <= Math.Min(TilePosB.Y, Map.Terrain.TileSize.Y - 1); Y2++ )
                    {
                        for ( X2 = Math.Max(TilePosA.X, 0); X2 <= Math.Min(TilePosB.X, Map.Terrain.TileSize.X - 1); X2++ )
                        {
                            TileNodeBlock(X2, Y2);
                        }
                    }

                    TilePathMap.FindCalc();

                    return NewUnit;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public clsUnit PlaceUnit(UnitTypeBase TypeBase, sWorldPos Pos, clsUnitGroup UnitGroup, int Rotation)
        {
            sXY_int TilePosA = new sXY_int();
            sXY_int TilePosB = new sXY_int();
            int X2 = 0;
            int Y2 = 0;
            sXY_int FinalTilePos = new sXY_int();
            sXY_int Footprint = new sXY_int();

            clsUnitAdd NewUnitAdd = new clsUnitAdd();
            NewUnitAdd.Map = Map;
            NewUnitAdd.StoreChange = true;
            clsUnit NewUnit = new clsUnit();
            NewUnitAdd.NewUnit = NewUnit;
            NewUnit.TypeBase = TypeBase;
            NewUnit.UnitGroup = UnitGroup;

            FinalTilePos.X = (int)(Conversion.Int(Pos.Horizontal.X / App.TerrainGridSpacing));
            FinalTilePos.Y = (int)(Conversion.Int(Pos.Horizontal.Y / App.TerrainGridSpacing));

            Footprint = TypeBase.get_GetFootprintSelected(Rotation);

            NewUnit.Pos = Pos;
            TilePosA.X = (int)(Conversion.Int((double)NewUnit.Pos.Horizontal.X / App.TerrainGridSpacing - Footprint.X / 2.0D + 0.5D));
            TilePosA.Y = (int)(Conversion.Int((double)NewUnit.Pos.Horizontal.Y / App.TerrainGridSpacing - Footprint.Y / 2.0D + 0.5D));
            TilePosB.X = (int)Conversion.Int((double)NewUnit.Pos.Horizontal.X / App.TerrainGridSpacing + Footprint.X / 2.0D - 0.5D);
            TilePosB.Y = (int)Conversion.Int((double)NewUnit.Pos.Horizontal.Y / App.TerrainGridSpacing + Footprint.Y / 2.0D - 0.5D);
            NewUnit.Rotation = Rotation;

            NewUnitAdd.Perform();

            for ( Y2 = Math.Max(TilePosA.Y, 0); Y2 <= Math.Min(TilePosB.Y, Map.Terrain.TileSize.Y - 1); Y2++ )
            {
                for ( X2 = Math.Max(TilePosA.X, 0); X2 <= Math.Min(TilePosB.X, Map.Terrain.TileSize.X - 1); X2++ )
                {
                    TileNodeBlock(X2, Y2);
                }
            }

            TilePathMap.FindCalc();

            return NewUnit;
        }

        public void TileNodeBlock(int X, int Y)
        {
            int X2 = 0;
            int Y2 = 0;

            for ( Y2 = Math.Max(Y - 6, 0); Y2 <= Math.Min(Y + 6, Map.Terrain.TileSize.Y - 1); Y2++ )
            {
                for ( X2 = Math.Max(X - 6, 0); X2 <= Math.Min(X + 6, Map.Terrain.TileSize.X - 1); X2++ )
                {
                    GenerateTerrainTiles[X2, Y2].Node.ClearanceSet(Math.Min(GenerateTerrainTiles[X2, Y2].Node.GetClearance,
                        Math.Max(Math.Abs(Y2 - Y), Math.Abs(X2 - X))));
                }
            }

            if ( GenerateTerrainTiles[X, Y].TopLeftLink != null )
            {
                GenerateTerrainTiles[X, Y].TopLeftLink.Destroy();
                GenerateTerrainTiles[X, Y].TopLeftLink = null;
            }
            if ( GenerateTerrainTiles[X, Y].TopLink != null )
            {
                GenerateTerrainTiles[X, Y].TopLink.Destroy();
                GenerateTerrainTiles[X, Y].TopLink = null;
            }
            if ( GenerateTerrainTiles[X, Y].TopRightLink != null )
            {
                GenerateTerrainTiles[X, Y].TopRightLink.Destroy();
                GenerateTerrainTiles[X, Y].TopRightLink = null;
            }
            if ( GenerateTerrainTiles[X, Y].RightLink != null )
            {
                GenerateTerrainTiles[X, Y].RightLink.Destroy();
                GenerateTerrainTiles[X, Y].RightLink = null;
            }
            if ( GenerateTerrainTiles[X, Y].BottomRightLink != null )
            {
                GenerateTerrainTiles[X, Y].BottomRightLink.Destroy();
                GenerateTerrainTiles[X, Y].BottomRightLink = null;
            }
            if ( GenerateTerrainTiles[X, Y].BottomLink != null )
            {
                GenerateTerrainTiles[X, Y].BottomLink.Destroy();
                GenerateTerrainTiles[X, Y].BottomLink = null;
            }
            if ( GenerateTerrainTiles[X, Y].BottomLeftLink != null )
            {
                GenerateTerrainTiles[X, Y].BottomLeftLink.Destroy();
                GenerateTerrainTiles[X, Y].BottomLeftLink = null;
            }
            if ( GenerateTerrainTiles[X, Y].LeftLink != null )
            {
                GenerateTerrainTiles[X, Y].LeftLink.Destroy();
                GenerateTerrainTiles[X, Y].LeftLink = null;
            }
        }

        public void BlockEdgeTiles()
        {
            int X = 0;
            int Y = 0;
            clsTerrain Terrain = Map.Terrain;

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= 2; X++ )
                {
                    TileNodeBlock(X, Y);
                }
                for ( X = Terrain.TileSize.X - 4; X <= Terrain.TileSize.X - 1; X++ )
                {
                    TileNodeBlock(X, Y);
                }
            }
            for ( X = 3; X <= Terrain.TileSize.X - 5; X++ )
            {
                for ( Y = 0; Y <= 2; Y++ )
                {
                    TileNodeBlock(X, Y);
                }
                for ( Y = Terrain.TileSize.Y - 4; Y <= Terrain.TileSize.Y - 1; Y++ )
                {
                    TileNodeBlock(X, Y);
                }
            }
            TilePathMap.FindCalc();
        }

        public clsResult GenerateUnits()
        {
            clsResult ReturnResult = new clsResult("Objects");

            int A = 0;
            int B = 0;
            int C = 0;
            int D = 0;
            clsUnit tmpUnit = default(clsUnit);
            int Count = 0;
            int FeaturePlaceRange = 6 * 128;
            int BasePlaceRange = 16 * 128;
            sXY_int TilePos = new sXY_int();
            byte AverageHeight = 0;
            int PlayerNum = 0;
            clsTerrain Terrain = Map.Terrain;

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    PassageNodes[B, A].HasFeatureCluster = false;
                }
            }

            for ( A = 0; A <= TotalPlayerCount - 1; A++ )
            {
                PlayerNum = A;
                tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseCommandCentre, PlayerBases[A].Pos, Map.UnitGroups[PlayerNum], 3, 0, BasePlaceRange);
                if ( tmpUnit == null )
                {
                    ReturnResult.ProblemAdd("No room for base structures");
                    return ReturnResult;
                }
                tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBasePowerGenerator, PlayerBases[A].Pos, Map.UnitGroups[PlayerNum], 3, 0, BasePlaceRange);
                if ( tmpUnit == null )
                {
                    ReturnResult.ProblemAdd("No room for base structures.");
                    return ReturnResult;
                }
                tmpUnit = PlaceUnit(DefaultGenerator.UnitTypeBasePowerModule, tmpUnit.Pos, Map.UnitGroups[PlayerNum], 0);
                if ( tmpUnit == null )
                {
                    ReturnResult.ProblemAdd("No room for module.");
                    return ReturnResult;
                }
                for ( B = 1; B <= 2; B++ )
                {
                    tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseResearchFacility, PlayerBases[A].Pos, Map.UnitGroups[PlayerNum], 3, 0, BasePlaceRange);
                    if ( tmpUnit == null )
                    {
                        ReturnResult.ProblemAdd("No room for base structures");
                        return ReturnResult;
                    }
                    tmpUnit = PlaceUnit(DefaultGenerator.UnitTypeBaseResearchModule, tmpUnit.Pos, Map.UnitGroups[PlayerNum], 0);
                    if ( tmpUnit == null )
                    {
                        ReturnResult.ProblemAdd("No room for module.");
                        return ReturnResult;
                    }
                }
                for ( B = 1; B <= 2; B++ )
                {
                    tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseFactory, PlayerBases[A].Pos, Map.UnitGroups[PlayerNum], 4, 0, BasePlaceRange);
                    if ( tmpUnit == null )
                    {
                        ReturnResult.ProblemAdd("No room for base structures");
                        return ReturnResult;
                    }
                    tmpUnit = PlaceUnit(DefaultGenerator.UnitTypeBaseFactoryModule, tmpUnit.Pos, Map.UnitGroups[PlayerNum], 0);
                    if ( tmpUnit == null )
                    {
                        ReturnResult.ProblemAdd("No room for module.");
                        return ReturnResult;
                    }
                }
                tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseCyborgFactory, PlayerBases[A].Pos, Map.UnitGroups[PlayerNum], 3, 0, BasePlaceRange);
                if ( tmpUnit == null )
                {
                    ReturnResult.ProblemAdd("No room for base structures");
                    return ReturnResult;
                }
                for ( B = 1; B <= BaseTruckCount; B++ )
                {
                    tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseTruck, PlayerBases[A].Pos, Map.UnitGroups[PlayerNum], 2, 0, BasePlaceRange);
                    if ( tmpUnit == null )
                    {
                        ReturnResult.ProblemAdd("No room for trucks");
                        return ReturnResult;
                    }
                }
            }
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                {
                    for ( B = 0; B <= PassageNodes[D, A].OilCount - 1; B++ )
                    {
                        if ( PassageNodes[D, A].PlayerBaseNum >= 0 )
                        {
                            tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseOilResource, PassageNodes[D, A].Pos, Map.ScavengerUnitGroup, 2, 0, BasePlaceRange);
                        }
                        else
                        {
                            tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseOilResource, PassageNodes[D, A].Pos, Map.ScavengerUnitGroup, 2, 0, FeaturePlaceRange);
                        }
                        if ( tmpUnit == null )
                        {
                            ReturnResult.ProblemAdd("No room for oil.");
                            return ReturnResult;
                        }
                        //flatten ground underneath
                        TilePos.X = Conversion.Int(tmpUnit.Pos.Horizontal.X / App.TerrainGridSpacing);
                        TilePos.Y = (int)(Conversion.Int(tmpUnit.Pos.Horizontal.Y / App.TerrainGridSpacing));
                        AverageHeight =
                            (byte)
                                (((Terrain.Vertices[TilePos.X, TilePos.Y].Height) + (Terrain.Vertices[TilePos.X + 1, TilePos.Y].Height) +
                                  (Terrain.Vertices[TilePos.X, TilePos.Y + 1].Height) + Terrain.Vertices[TilePos.X + 1, TilePos.Y + 1].Height) / 4.0D);
                        Terrain.Vertices[TilePos.X, TilePos.Y].Height = AverageHeight;
                        Terrain.Vertices[TilePos.X + 1, TilePos.Y].Height = AverageHeight;
                        Terrain.Vertices[TilePos.X, TilePos.Y + 1].Height = AverageHeight;
                        Terrain.Vertices[TilePos.X + 1, TilePos.Y + 1].Height = AverageHeight;
                        Map.SectorGraphicsChanges.TileChanged(TilePos);
                        Map.SectorUnitHeightsChanges.TileChanged(TilePos);
                        Map.SectorTerrainUndoChanges.TileChanged(TilePos);
                        tmpUnit.Pos.Altitude = AverageHeight * Map.HeightMultiplier;
                        if ( PassageNodes[D, A].PlayerBaseNum >= 0 )
                        {
                            //place base derrick
                            tmpUnit = PlaceUnit(DefaultGenerator.UnitTypeBaseDerrick, tmpUnit.Pos, Map.UnitGroups[PassageNodes[D, A].PlayerBaseNum], 0);
                            if ( tmpUnit == null )
                            {
                                ReturnResult.ProblemAdd("No room for derrick.");
                                return ReturnResult;
                            }
                        }
                    }
                }
            }

            //feature clusters
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                {
                    if ( PassageNodes[D, A].PlayerBaseNum < 0 && !PassageNodes[D, A].IsOnBorder )
                    {
                        PassageNodes[D, A].HasFeatureCluster = VBMath.Rnd() < FeatureClusterChance;
                    }
                }
            }

            UInt32 RandNum = 0;
            UInt32 uintTemp = 0;
            PathfinderNode tmpNode = default(PathfinderNode);
            int E = 0;
            sXY_int Footprint = new sXY_int();
            int MissingUnitCount = 0;
            int Rotation = 0;

            if ( GenerateTileset.ClusteredUnitChanceTotal > 0 )
            {
                for ( A = 0; A <= PassageNodeCount - 1; A++ )
                {
                    for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                    {
                        if ( PassageNodes[D, A].HasFeatureCluster )
                        {
                            Count = FeatureClusterMinUnits +
                                    Convert.ToInt32(Conversion.Int(VBMath.Rnd() * (FeatureClusterMaxUnits - FeatureClusterMinUnits + 1)));
                            for ( B = 1; B <= Count; B++ )
                            {
                                RandNum = (uint)(Conversion.Int(VBMath.Rnd() * GenerateTileset.ClusteredUnitChanceTotal));
                                uintTemp = 0;
                                for ( C = 0; C <= GenerateTileset.ClusteredUnitCount - 1; C++ )
                                {
                                    uintTemp += GenerateTileset.ClusteredUnits[C].Chance;
                                    if ( RandNum < uintTemp )
                                    {
                                        break;
                                    }
                                }
                                Rotation = 0;
                                Footprint = GenerateTileset.ClusteredUnits[C].TypeBase.get_GetFootprintSelected(Rotation);
                                E = ((int)(Math.Ceiling((decimal)(Math.Max(Footprint.X, Footprint.Y) / 2.0F)))) + 1;
                                tmpUnit = PlaceUnitNear(GenerateTileset.ClusteredUnits[C].TypeBase, PassageNodes[D, A].Pos, Map.ScavengerUnitGroup, E, Rotation,
                                    FeaturePlaceRange);
                                if ( tmpUnit == null )
                                {
                                    MissingUnitCount += Count - B + 1;
                                    break;
                                }
                            }
                        }
                    }
                }
                if ( MissingUnitCount > 0 )
                {
                    ReturnResult.WarningAdd("Not enough space for " + Convert.ToString(MissingUnitCount) + " clustered objects.");
                }
            }

            if ( TilePathMap.get_GetNodeLayer(TilePathMap.GetNodeLayerCount - 1).GetNodeCount != 1 )
            {
                ReturnResult.ProblemAdd("Error: bad node count on top layer!");
                return ReturnResult;
            }

            if ( GenerateTileset.ScatteredUnitChanceTotal > 0 )
            {
                for ( A = 1; A <= FeatureScatterCount; A++ )
                {
                    RandNum = (uint)(Conversion.Int(VBMath.Rnd() * GenerateTileset.ScatteredUnitChanceTotal));
                    uintTemp = 0;
                    for ( C = 0; C <= GenerateTileset.ScatteredUnitCount - 1; C++ )
                    {
                        uintTemp += GenerateTileset.ScatteredUnits[C].Chance;
                        if ( RandNum < uintTemp )
                        {
                            break;
                        }
                    }
                    Rotation = 0;
                    Footprint = GenerateTileset.ScatteredUnits[C].TypeBase.get_GetFootprintSelected(Rotation);
                    B = FeatureScatterGap + (int)(Math.Ceiling((decimal)(Math.Max(Footprint.X, Footprint.Y) / 2.0F)));
                    tmpNode = GetRandomChildNode(TilePathMap.get_GetNodeLayer(TilePathMap.GetNodeLayerCount - 1).get_GetNode(0), B);
                    if ( tmpNode == null )
                    {
                        break;
                    }
                    else
                    {
                        clsNodeTag NodeTag = (clsNodeTag)tmpNode.Tag;
                        if ( PlaceUnitNear(GenerateTileset.ScatteredUnits[C].TypeBase, NodeTag.Pos, Map.ScavengerUnitGroup, B, Rotation, FeaturePlaceRange) == null )
                        {
                            break;
                        }
                    }
                }
                if ( A < FeatureScatterCount + 1 )
                {
                    ReturnResult.WarningAdd("Only enough space for " + Convert.ToString(A - 1) + " scattered objects.");
                }
            }

            return ReturnResult;
        }

        public PathfinderNode GetRandomChildNode(PathfinderNode InputNode, int MinClearance)
        {
            if ( InputNode.GetClearance < MinClearance )
            {
                return null;
            }

            if ( InputNode.GetChildNodeCount == 0 )
            {
                return InputNode;
            }
            else
            {
                int A = 0;
                do
                {
                    A = Convert.ToInt32(Conversion.Int(VBMath.Rnd() * InputNode.GetChildNodeCount));
                } while ( InputNode.get_GetChildNode(A).GetClearance < MinClearance );

                PathfinderNode ReturnResult = GetRandomChildNode(InputNode.get_GetChildNode(A), MinClearance);
                return ReturnResult;
            }
        }

        private struct sPossibleGateway
        {
            public sXY_int StartPos;
            public sXY_int MiddlePos;
            public bool IsVertical;
            public int Length;
        }

        public sResult GenerateGateways()
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            //must be done before units otherwise the units will be treated as gateway obstacles

            clsTerrain Terrain = Map.Terrain;

            int X = 0;
            int SpaceCount = 0;
            int Y = 0;
            sPossibleGateway[] PossibleGateways = new sPossibleGateway[Terrain.TileSize.X * Terrain.TileSize.Y * 2];
            int PossibleGatewayCount = 0;

            for ( Y = 0; Y <= Terrain.TileSize.Y - 1; Y++ )
            {
                SpaceCount = 0;
                for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
                {
                    if ( GenerateTerrainTiles[X, Y].Node.GetClearance < 1 )
                    {
                    }
                    else if ( GenerateTerrainTiles[X, Y].Node.GetClearance == 1 )
                    {
                        if ( SpaceCount > 3 & SpaceCount <= 9 )
                        {
                            PossibleGateways[PossibleGatewayCount].StartPos.X = X - SpaceCount;
                            PossibleGateways[PossibleGatewayCount].StartPos.Y = Y;
                            PossibleGateways[PossibleGatewayCount].Length = SpaceCount + 1;
                            PossibleGateways[PossibleGatewayCount].IsVertical = false;
                            PossibleGateways[PossibleGatewayCount].MiddlePos.X = PossibleGateways[PossibleGatewayCount].StartPos.X * 128 +
                                                                                 PossibleGateways[PossibleGatewayCount].Length * 64;
                            PossibleGateways[PossibleGatewayCount].MiddlePos.Y = PossibleGateways[PossibleGatewayCount].StartPos.Y * 128;
                            PossibleGatewayCount++;
                        }
                        SpaceCount = 1;
                    }
                    else
                    {
                        SpaceCount++;
                    }
                }
            }
            for ( X = 0; X <= Terrain.TileSize.X - 1; X++ )
            {
                SpaceCount = 0;
                Y = 0;
                while ( Y < Terrain.TileSize.Y )
                {
                    if ( GenerateTerrainTiles[X, Y].Node.GetClearance < 1 )
                    {
                    }
                    else if ( GenerateTerrainTiles[X, Y].Node.GetClearance == 1 )
                    {
                        if ( SpaceCount >= 3 & SpaceCount <= 9 )
                        {
                            PossibleGateways[PossibleGatewayCount].StartPos.X = X;
                            PossibleGateways[PossibleGatewayCount].StartPos.Y = Y - SpaceCount;
                            PossibleGateways[PossibleGatewayCount].Length = SpaceCount + 1;
                            PossibleGateways[PossibleGatewayCount].IsVertical = true;
                            PossibleGateways[PossibleGatewayCount].MiddlePos.X = PossibleGateways[PossibleGatewayCount].StartPos.X * 128;
                            PossibleGateways[PossibleGatewayCount].MiddlePos.Y = PossibleGateways[PossibleGatewayCount].StartPos.Y * 128 +
                                                                                 PossibleGateways[PossibleGatewayCount].Length * 64;
                            PossibleGatewayCount++;
                        }
                        SpaceCount = 1;
                    }
                    else
                    {
                        SpaceCount++;
                    }
                    Y++;
                }
            }

            //add the best gateways

            int A = 0;
            float Value = 0;
            float BestValue = 0;
            int BestNum = 0;
            bool[,] TileIsGateway = new bool[Terrain.TileSize.X, Terrain.TileSize.Y];
            bool Valid = default(bool);
            sXY_int InvalidPos = new sXY_int();
            double InvalidDist = 0;

            while ( PossibleGatewayCount > 0 )
            {
                BestNum = -1;
                BestValue = float.MaxValue;
                for ( A = 0; A <= PossibleGatewayCount - 1; A++ )
                {
                    //Value = 0.0F
                    //For B = 0 To PossibleGatewayCount - 1
                    //    Value += GetDist(PossibleGateways(A).MiddlePos, PossibleGateways(B).MiddlePos)
                    //Next
                    Value = PossibleGateways[A].Length;
                    if ( Value < BestValue )
                    {
                        BestValue = Value;
                        BestNum = A;
                    }
                }
                if ( PossibleGateways[BestNum].IsVertical )
                {
                    Map.GatewayCreateStoreChange(PossibleGateways[BestNum].StartPos,
                        new sXY_int(PossibleGateways[BestNum].StartPos.X, PossibleGateways[BestNum].StartPos.Y + PossibleGateways[BestNum].Length - 1));
                    for ( Y = PossibleGateways[BestNum].StartPos.Y; Y <= PossibleGateways[BestNum].StartPos.Y + PossibleGateways[BestNum].Length - 1; Y++ )
                    {
                        TileIsGateway[PossibleGateways[BestNum].StartPos.X, Y] = true;
                    }
                }
                else
                {
                    Map.GatewayCreateStoreChange(PossibleGateways[BestNum].StartPos,
                        new sXY_int(PossibleGateways[BestNum].StartPos.X + PossibleGateways[BestNum].Length - 1, PossibleGateways[BestNum].StartPos.Y));
                    for ( X = PossibleGateways[BestNum].StartPos.X; X <= PossibleGateways[BestNum].StartPos.X + PossibleGateways[BestNum].Length - 1; X++ )
                    {
                        TileIsGateway[X, PossibleGateways[BestNum].StartPos.Y] = true;
                    }
                }
                InvalidPos = PossibleGateways[BestNum].MiddlePos;
                InvalidDist = PossibleGateways[BestNum].Length * 128;
                A = 0;
                while ( A < PossibleGatewayCount )
                {
                    Valid = true;
                    if ( PossibleGateways[A].IsVertical )
                    {
                        for ( Y = PossibleGateways[A].StartPos.Y; Y <= PossibleGateways[A].StartPos.Y + PossibleGateways[A].Length - 1; Y++ )
                        {
                            if ( TileIsGateway[PossibleGateways[A].StartPos.X, Y] )
                            {
                                Valid = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for ( X = PossibleGateways[A].StartPos.X; X <= PossibleGateways[A].StartPos.X + PossibleGateways[A].Length - 1; X++ )
                        {
                            if ( TileIsGateway[X, PossibleGateways[A].StartPos.Y] )
                            {
                                Valid = false;
                                break;
                            }
                        }
                    }
                    if ( Valid )
                    {
                        if ( (InvalidPos - PossibleGateways[A].MiddlePos).ToDoubles().GetMagnitude() < InvalidDist )
                        {
                            Valid = false;
                        }
                    }
                    if ( !Valid )
                    {
                        PossibleGatewayCount--;
                        if ( A != PossibleGatewayCount )
                        {
                            PossibleGateways[A] = PossibleGateways[PossibleGatewayCount];
                        }
                    }
                    else
                    {
                        A++;
                    }
                }
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        //Public Structure sGenerateSide
        //    Public Node As PathfinderNode
        //    Public TopLink As PathfinderNode
        //    Public TopRightLink As PathfinderNode
        //    Public RightLink As PathfinderNode
        //    Public BottomRightLink As PathfinderNode
        //    Public BottomLink As PathfinderNode
        //    Public BottomLeftLink As PathfinderNode
        //    Public LeftLink As PathfinderNode
        //    Public TopLeftLink As PathfinderNode
        //End Structure

        //Public Structure sGenerateRoadsArgs
        //    Public RoadType As sPainter.clsRoad
        //    Public MaxAlt As Byte
        //    Public Terrain As sPainter.clsTerrain
        //    Public MinLength As Integer
        //    Public MaxLength As Integer
        //    Public MinTurnRatio As Single
        //    Public MaxTurnRatio As Single
        //    Public Quantity As Integer
        //End Structure

        //Public Sub GenerateRoads( Args As sGenerateRoadsArgs)
        //    Dim RoadPathMap As New PathfinderNetwork
        //    Dim tmpNode As PathfinderNode
        //    Dim NodeTag As clsNodeTag


        //    For Y = 0 To Terrain.Size.Y
        //        For X = 0 To Terrain.Size.X
        //            GenerateTerrainVertex(X, Y).Node = New PathfinderNode(RoadPathMap)
        //            NodeTag = New clsNodeTag
        //            NodeTag.Pos = New sXY_int(X * 128, Y * 128)
        //            GenerateTerrainVertex(X, Y).Node.Tag = NodeTag
        //        Next
        //    Next
        //    For Y = 0 To Terrain.Size.Y
        //        For X = 0 To Terrain.Size.X
        //            tmpNodeA = GenerateTerrainVertex(X, Y).Node
        //            If X > 0 Then
        //                tmpNodeB = GenerateTerrainVertex(X - 1, Y).Node
        //                GenerateTerrainVertex(X, Y).LeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
        //            End If
        //            If Y > 0 Then
        //                If X > 0 Then
        //                    tmpNodeB = GenerateTerrainVertex(X - 1, Y - 1).Node
        //                    GenerateTerrainVertex(X, Y).TopLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
        //                End If
        //                tmpNodeB = GenerateTerrainVertex(X, Y - 1).Node
        //                GenerateTerrainVertex(X, Y).TopLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
        //                If X < Terrain.Size.X Then
        //                    tmpNodeB = GenerateTerrainVertex(X + 1, Y - 1).Node
        //                    GenerateTerrainVertex(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
        //                End If
        //            End If
        //            If X < Terrain.Size.X Then
        //                tmpNodeB = GenerateTerrainVertex(X + 1, Y).Node
        //                GenerateTerrainVertex(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
        //            End If
        //            If Y < Terrain.Size.Y Then
        //                If X > 0 Then
        //                    tmpNodeB = GenerateTerrainVertex(X - 1, Y + 1).Node
        //                    GenerateTerrainVertex(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
        //                End If
        //                tmpNodeB = GenerateTerrainVertex(X, Y + 1).Node
        //                GenerateTerrainVertex(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
        //                If X < Terrain.Size.X Then
        //                    tmpNodeB = GenerateTerrainVertex(X + 1, Y + 1).Node
        //                    GenerateTerrainVertex(X, Y).BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
        //                End If
        //            End If
        //        Next
        //    Next

        //    RoadPathMap.LargeArraysResize()
        //    RoadPathMap.FindCalc()

        //    RoadPathMap.Deallocate()
        //End Sub

        public void ClearLayout()
        {
            int A = 0;
            int B = 0;

            if ( TilePathMap != null )
            {
                TilePathMap.Deallocate();
                TilePathMap = null;
            }
            if ( VertexPathMap != null )
            {
                VertexPathMap.Deallocate();
                VertexPathMap = null;
            }

            for ( A = 0; A <= ConnectionCount - 1; A++ )
            {
                Connections[A].PassageNodeA = null;
                Connections[A].PassageNodeB = null;
                Connections[A].Reflections = null;
            }
            ConnectionCount = 0;

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    PassageNodes[B, A].Connections = null;
                }
            }
            PassageNodeCount = 0;

            NearestCount = 0;
        }

        private bool MakePassageNodes(sXY_int Pos, bool IsOnBorder)
        {
            int A = 0;
            int B = 0;
            clsPassageNode tmpNode = default(clsPassageNode);
            sXY_int RotatedPos = new sXY_int();
            sXY_int SymmetrySize = new sXY_int();
            sXY_int[] Positions = new sXY_int[4];
            sXY_int Limits = new sXY_int();

            SymmetrySize.X = (int)(TileSize.X * App.TerrainGridSpacing / SymmetryBlockCountXY.X);
            SymmetrySize.Y = (int)(TileSize.Y * App.TerrainGridSpacing / SymmetryBlockCountXY.Y);

            Limits.X = SymmetrySize.X - 1;
            Limits.Y = SymmetrySize.Y - 1;

            for ( A = 0; A <= SymmetryBlockCount - 1; A++ )
            {
                RotatedPos = TileUtil.GetRotatedPos(SymmetryBlocks[A].Orientation, Pos, Limits);
                Positions[A].X = SymmetryBlocks[A].XYNum.X * SymmetrySize.X + RotatedPos.X;
                Positions[A].Y = SymmetryBlocks[A].XYNum.Y * SymmetrySize.Y + RotatedPos.Y;
                for ( B = 0; B <= A - 1; B++ )
                {
                    if ( (Positions[A] - Positions[B]).ToDoubles().GetMagnitude() < NodeScale * App.TerrainGridSpacing * 2.0D )
                    {
                        return false;
                    }
                }
            }

            for ( A = 0; A <= SymmetryBlockCount - 1; A++ )
            {
                tmpNode = new clsPassageNode();
                PassageNodes[A, PassageNodeCount] = tmpNode;
                tmpNode.Num = PassageNodeCount;
                tmpNode.MirrorNum = A;
                tmpNode.Pos = Positions[A];
                tmpNode.IsOnBorder = IsOnBorder;
            }
            PassageNodeCount++;

            return true;
        }

        private bool CheckRampAngles(clsConnection NewRampConnection, double MinSpacingAngle, double MinSpacingAngle2, double MinPassageSpacingAngle)
        {
            sXY_int XY_int = new sXY_int();
            double NodeAAwayAngle = 0;
            double NodeBAwayAngle = 0;

            XY_int.X = NewRampConnection.PassageNodeB.Pos.X - NewRampConnection.PassageNodeA.Pos.X;
            XY_int.Y = NewRampConnection.PassageNodeB.Pos.Y - NewRampConnection.PassageNodeA.Pos.Y;
            if ( NewRampConnection.PassageNodeA.Connections[NewRampConnection.PassageNodeA_ConnectionNum].IsB )
            {
                NodeBAwayAngle = XY_int.ToDoubles().GetAngle();
                NodeAAwayAngle = MathUtil.AngleClamp(NodeBAwayAngle - Math.PI);
            }
            else
            {
                NodeAAwayAngle = XY_int.ToDoubles().GetAngle();
                NodeBAwayAngle = MathUtil.AngleClamp(NodeBAwayAngle - Math.PI);
            }
            if ( !CheckRampNodeRampAngles(NewRampConnection.PassageNodeA, NewRampConnection.PassageNodeB, NodeAAwayAngle, MinSpacingAngle, MinSpacingAngle2) )
            {
                return false;
            }
            if ( !CheckRampNodeRampAngles(NewRampConnection.PassageNodeB, NewRampConnection.PassageNodeA, NodeBAwayAngle, MinSpacingAngle, MinSpacingAngle2) )
            {
                return false;
            }
            if ( !CheckRampNodeLevelAngles(NewRampConnection.PassageNodeA, NodeAAwayAngle, MinPassageSpacingAngle) )
            {
                return false;
            }
            if ( !CheckRampNodeLevelAngles(NewRampConnection.PassageNodeB, NodeBAwayAngle, MinPassageSpacingAngle) )
            {
                return false;
            }
            return true;
        }

        private bool CheckRampNodeRampAngles(clsPassageNode RampPassageNode, clsPassageNode OtherRampPassageNode, double RampAwayAngle, double MinSpacingAngle,
            double MinSpacingAngle2)
        {
            int ConnectionNum = 0;
            clsConnection tmpConnection = default(clsConnection);
            clsPassageNode OtherNode = default(clsPassageNode);
            sXY_int XY_int = new sXY_int();
            double SpacingAngle = 0;
            int RampDifference = 0;

            for ( ConnectionNum = 0; ConnectionNum <= RampPassageNode.ConnectionCount - 1; ConnectionNum++ )
            {
                tmpConnection = RampPassageNode.Connections[ConnectionNum].Connection;
                if ( tmpConnection.IsRamp )
                {
                    OtherNode = RampPassageNode.Connections[ConnectionNum].GetOther();
                    XY_int.X = OtherNode.Pos.X - RampPassageNode.Pos.X;
                    XY_int.Y = OtherNode.Pos.Y - RampPassageNode.Pos.Y;
                    SpacingAngle = Math.Abs(MathUtil.AngleClamp(RampAwayAngle - XY_int.ToDoubles().GetAngle()));
                    RampDifference = Math.Abs(OtherNode.Level - OtherRampPassageNode.Level);
                    if ( RampDifference >= 2 )
                    {
                        if ( SpacingAngle < MinSpacingAngle2 )
                        {
                            return false;
                        }
                    }
                    else if ( RampDifference == 0 )
                    {
                        if ( SpacingAngle < MinSpacingAngle )
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Debugger.Break();
                        return false;
                    }
                }
            }
            return true;
        }

        private bool PassageNodeHasRamp(clsPassageNode PassageNode)
        {
            int ConnectionNum = 0;

            for ( ConnectionNum = 0; ConnectionNum <= PassageNode.ConnectionCount - 1; ConnectionNum++ )
            {
                if ( PassageNode.Connections[ConnectionNum].Connection.IsRamp )
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckRampNodeLevelAngles(clsPassageNode RampPassageNode, double RampAwayAngle, double MinSpacingAngle)
        {
            int ConnectionNum = 0;
            clsConnection tmpConnection;
            clsPassageNode OtherPassageNode = default(clsPassageNode);
            int OtherNum = 0;
            bool NarrowConnection = default(bool);
            sXY_int XY_int = new sXY_int();
            bool HasRamp = PassageNodeHasRamp(RampPassageNode);

            for ( ConnectionNum = 0; ConnectionNum <= RampPassageNode.ConnectionCount - 1; ConnectionNum++ )
            {
                tmpConnection = RampPassageNode.Connections[ConnectionNum].Connection;
                OtherPassageNode = RampPassageNode.Connections[ConnectionNum].GetOther();
                if ( OtherPassageNode.Level == RampPassageNode.Level )
                {
                    NarrowConnection = true;
                    if ( ConnectionNum == 0 )
                    {
                        OtherNum = RampPassageNode.ConnectionCount - 1;
                    }
                    else
                    {
                        OtherNum = ConnectionNum - 1;
                    }
                    if ( OtherNum != ConnectionNum )
                    {
                        if ( RampPassageNode.Connections[OtherNum].GetOther().Level == OtherPassageNode.Level )
                        {
                            NarrowConnection = false;
                        }
                    }
                    else
                    {
                        NarrowConnection = false;
                    }
                    if ( ConnectionNum == RampPassageNode.ConnectionCount - 1 )
                    {
                        OtherNum = 0;
                    }
                    else
                    {
                        OtherNum = ConnectionNum + 1;
                    }
                    if ( OtherNum != ConnectionNum )
                    {
                        if ( RampPassageNode.Connections[OtherNum].GetOther().Level == OtherPassageNode.Level )
                        {
                            NarrowConnection = false;
                        }
                    }
                    else
                    {
                        NarrowConnection = false;
                    }
                    if ( NarrowConnection )
                    {
                        XY_int.X = OtherPassageNode.Pos.X - RampPassageNode.Pos.X;
                        XY_int.Y = OtherPassageNode.Pos.Y - RampPassageNode.Pos.Y;
                        //If HasRamp Then
                        //    Return False
                        //End If
                        if ( Math.Abs(MathUtil.AngleClamp(XY_int.ToDoubles().GetAngle() - RampAwayAngle)) < MinSpacingAngle )
                        {
                            return false;
                        }
                        //If PassageNodeHasRamp(OtherPassageNode) Then
                        //    Return False
                        //End If
                    }
                }
            }
            return true;
        }

        private class clsPassageNodeHeightLevelArgs
        {
            public clsPassageNode PassageNode;
            public int[] MapLevelCount;
            public clsPassageNodeLevels PassageNodesMinLevel = new clsPassageNodeLevels();
            public clsPassageNodeLevels PassageNodesMaxLevel = new clsPassageNodeLevels();
            public int FlatsCutoff;
            public int PassagesCutoff;
            public int VariationCutoff;
            public int ActionTotal;
        }

        private sResult PassageNodeHeightLevel(clsPassageNodeHeightLevelArgs Args)
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Problem = "";
            ReturnResult.Success = false;

            int[] LevelCounts = new int[LevelCount];
            int WaterCount = 0;
            bool ConnectedToLevel = default(bool);
            clsPassageNode tmpPassageNodeB = default(clsPassageNode);
            clsPassageNode tmpPassageNodeC = default(clsPassageNode);
            int EligableCount = 0;
            int[] Eligables = new int[LevelCount];
            int NewHeightLevel = 0;
            int RandomAction = 0;
            int A = 0;
            int B = 0;

            for ( B = 0; B <= Args.PassageNode.ConnectionCount - 1; B++ )
            {
                tmpPassageNodeB = Args.PassageNode.Connections[B].GetOther();
                if ( tmpPassageNodeB.Level >= 0 )
                {
                    LevelCounts[tmpPassageNodeB.Level]++;
                    ConnectedToLevel = true;
                }
                if ( tmpPassageNodeB.IsWater )
                {
                    WaterCount++;
                }
            }
            if ( WaterCount > 0 )
            {
                NewHeightLevel = 0;
            }
            else if ( Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num] > Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num] )
            {
                ReturnResult.Problem = "Error: Min height more than max.";
                return ReturnResult;
            }
            else if ( !ConnectedToLevel )
            {
                //switch to the most uncommon level on the map
                A = int.MaxValue;
                EligableCount = 0;
                for ( B = Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num]; B <= Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num]; B++ )
                {
                    if ( Args.MapLevelCount[B] < A )
                    {
                        A = Args.MapLevelCount[B];
                        Eligables[0] = B;
                        EligableCount = 1;
                    }
                    else if ( Args.MapLevelCount[B] == A )
                    {
                        Eligables[EligableCount] = B;
                        EligableCount++;
                    }
                }
                NewHeightLevel = Eligables[(int)(Conversion.Int(VBMath.Rnd() * EligableCount))];
            }
            else
            {
                RandomAction = Convert.ToInt32(Conversion.Int(VBMath.Rnd() * Args.ActionTotal));
                if ( RandomAction < Args.FlatsCutoff )
                {
                    //extend the level that surrounds this most
                    A = 0;
                    EligableCount = 0;
                    for ( B = Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num]; B <= Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num]; B++ )
                    {
                        if ( LevelCounts[B] > A )
                        {
                            A = LevelCounts[B];
                            Eligables[0] = B;
                            EligableCount = 1;
                        }
                        else if ( LevelCounts[B] == A )
                        {
                            Eligables[EligableCount] = B;
                            EligableCount++;
                        }
                    }
                }
                else if ( RandomAction < Args.PassagesCutoff )
                {
                    //extend any level that surrounds only once, or twice by nodes that aren't already connected
                    EligableCount = 0;
                    for ( B = Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num]; B <= Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num]; B++ )
                    {
                        if ( LevelCounts[B] == 1 )
                        {
                            Eligables[EligableCount] = B;
                            EligableCount++;
                        }
                        else if ( LevelCounts[B] == 2 )
                        {
                            EligableCount = 0;
                            tmpPassageNodeC = null;
                            for ( A = 0; A <= Args.PassageNode.ConnectionCount - 1; A++ )
                            {
                                tmpPassageNodeB = Args.PassageNode.Connections[A].GetOther();
                                if ( tmpPassageNodeB.Level == B )
                                {
                                    if ( tmpPassageNodeC == null )
                                    {
                                        tmpPassageNodeC = tmpPassageNodeB;
                                    }
                                    else
                                    {
                                        if ( tmpPassageNodeC.FindConnection(tmpPassageNodeB) == null )
                                        {
                                            Eligables[EligableCount] = B;
                                            EligableCount++;
                                        }
                                        break;
                                    }
                                }
                            }
                            if ( A == Args.PassageNode.ConnectionCount )
                            {
                                MessageBox.Show("Error: two nodes not found");
                            }
                        }
                    }
                }
                else if ( RandomAction < Args.VariationCutoff )
                {
                    EligableCount = 0;
                }
                else
                {
                    ReturnResult.Problem = "Error: Random number out of range.";
                    return ReturnResult;
                }
                if ( EligableCount == 0 )
                {
                    //extend the most uncommon surrounding
                    A = int.MaxValue;
                    EligableCount = 0;
                    for ( B = Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num]; B <= Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num]; B++ )
                    {
                        if ( LevelCounts[B] < A )
                        {
                            A = LevelCounts[B];
                            Eligables[0] = B;
                            EligableCount = 1;
                        }
                        else if ( LevelCounts[B] == A )
                        {
                            Eligables[EligableCount] = B;
                            EligableCount++;
                        }
                    }
                }
                NewHeightLevel = Eligables[(int)(Conversion.Int(VBMath.Rnd() * EligableCount))];
            }
            for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
            {
                PassageNodes[B, Args.PassageNode.Num].Level = NewHeightLevel;
            }
            PassageNodesMinLevelSet(Args.PassageNode, Args.PassageNodesMinLevel, NewHeightLevel, MaxLevelTransition);
            PassageNodesMaxLevelSet(Args.PassageNode, Args.PassageNodesMaxLevel, NewHeightLevel, MaxLevelTransition);
            Args.MapLevelCount[NewHeightLevel]++;

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public clsResult GenerateRamps()
        {
            clsResult ReturnResult = new clsResult("Ramps");

            int A = 0;
            int B = 0;
            int C = 0;
            int E = 0;
            double BestDist = 0;
            int BestNum = 0;
            sXY_int XY_int = new sXY_int();
            clsPassageNode tmpPassageNodeA = default(clsPassageNode);
            clsPassageNode tmpPassageNodeB = default(clsPassageNode);
            double Dist = 0;

            //make ramps

            for ( A = 0; A <= ConnectionCount - 1; A++ )
            {
                Connections[A].IsRamp = false;
            }

            PathfinderNode tmpNodeA = default(PathfinderNode);
            PathfinderNode tmpNodeB = default(PathfinderNode);
            PathfinderNode[,] PassageNodePathNodes = null;
            PathfinderConnection NewConnection;

            clsPassageNodeNework PassageNodeNetwork = MakePassageNodeNetwork();
            PassageNodePathNodes = PassageNodeNetwork.PassageNodePathNodes;

            clsConnection[] PossibleRamps = new clsConnection[ConnectionCount];
            int PossibleRampCount = 0;
            PathfinderNode[] GetPathStartNodes = new PathfinderNode[1];
            PathfinderNetwork.PathList[] ResultPaths = null;

            //ramp connections whose points are too far apart

            bool[] ConnectionsCanRamp = new bool[ConnectionCount];

            for ( B = 0; B <= ConnectionCount - 1; B++ )
            {
                C = Math.Abs(Connections[B].PassageNodeA.Level - Connections[B].PassageNodeB.Level);
                if ( C == 1 )
                {
                    if ( !(Connections[B].PassageNodeA.IsOnBorder || Connections[B].PassageNodeB.IsOnBorder)
                         && Connections[B].PassageNodeA.MirrorNum == 0
                         && Connections[B].PassageNodeA.Num != Connections[B].PassageNodeB.Num )
                    {
                        ConnectionsCanRamp[B] = true;
                    }
                    else
                    {
                        ConnectionsCanRamp[B] = false;
                    }
                }
                else
                {
                    ConnectionsCanRamp[B] = false;
                }
            }

            clsNodeConnectedness Connectedness = new clsNodeConnectedness();
            Connectedness.NodeConnectedness = new float[PassageNodeCount];
            Connectedness.PassageNodeVisited = new bool[SymmetryBlockCount, PassageNodeCount];
            Connectedness.PassageNodePathNodes = PassageNodePathNodes;
            Connectedness.PassageNodePathMap = PassageNodeNetwork.Network;

            PathfinderConnection[] tmpPathConnection = new PathfinderConnection[4];
            double Value = 0;
            double BestDistB = 0;
            double BaseDist = 0;
            double RampDist = 0;
            clsUpdateNodeConnectednessArgs UpdateNodeConnectednessArgs = new clsUpdateNodeConnectednessArgs();
            clsUpdateNetworkConnectednessArgs UpdateNetworkConnectednessArgs = new clsUpdateNetworkConnectednessArgs();

            UpdateNodeConnectednessArgs.Args = Connectedness;
            UpdateNetworkConnectednessArgs.Args = Connectedness;
            UpdateNetworkConnectednessArgs.PassageNodeUpdated = new bool[PassageNodeCount];
            UpdateNetworkConnectednessArgs.SymmetryBlockCount = SymmetryBlockCount;

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                Connectedness.NodeConnectedness[A] = 0.0F;
                for ( B = 0; B <= PassageNodeCount - 1; B++ )
                {
                    for ( C = 0; C <= SymmetryBlockCount - 1; C++ )
                    {
                        Connectedness.PassageNodeVisited[C, B] = false;
                    }
                }
                UpdateNodeConnectednessArgs.OriginalNode = PassageNodes[0, A];
                UpdateNodeConnectedness(UpdateNodeConnectednessArgs, PassageNodes[0, A]);
            }

            do
            {
                BestNum = -1;
                BestDist = 1.0F; //for connections that can already reach the other side
                BestDistB = 0.0F; //for connections that cant
                PossibleRampCount = 0;
                for ( B = 0; B <= ConnectionCount - 1; B++ )
                {
                    if ( ConnectionsCanRamp[B] && !Connections[B].IsRamp )
                    {
                        if ( CheckRampAngles(Connections[B], Convert.ToDouble(80.0D * MathUtil.RadOf1Deg), Convert.ToDouble(120.0D * MathUtil.RadOf1Deg),
                            0.0D * MathUtil.RadOf1Deg) )
                        {
                            GetPathStartNodes[0] = PassageNodePathNodes[Connections[B].PassageNodeA.MirrorNum, Connections[B].PassageNodeA.Num];
                            ResultPaths = PassageNodeNetwork.Network.GetPath(GetPathStartNodes,
                                PassageNodePathNodes[Connections[B].PassageNodeB.MirrorNum, Connections[B].PassageNodeB.Num], -1, 0);
                            BaseDist = double.MaxValue;
                            XY_int.X = (int)((Connections[B].PassageNodeA.Pos.X + Connections[B].PassageNodeB.Pos.X) / 2.0D);
                            XY_int.Y = (int)((Connections[B].PassageNodeA.Pos.Y + Connections[B].PassageNodeB.Pos.Y) / 2.0D);
                            for ( E = 0; E <= TotalPlayerCount - 1; E++ )
                            {
                                Dist = Convert.ToDouble((PlayerBases[E].Pos - XY_int).ToDoubles().GetMagnitude());
                                if ( Dist < BaseDist )
                                {
                                    BaseDist = Dist;
                                }
                            }
                            RampDist = Math.Max(MaxDisconnectionDist * Math.Pow(RampBase, (BaseDist / 1024.0D)), 1.0F);
                            if ( ResultPaths == null )
                            {
                                Value = Connectedness.NodeConnectedness[Connections[B].PassageNodeA.Num] +
                                        Connectedness.NodeConnectedness[Connections[B].PassageNodeB.Num];
                                if ( double.MaxValue > BestDist )
                                {
                                    BestDist = double.MaxValue;
                                    BestDistB = Value;
                                    PossibleRamps[0] = Connections[B];
                                    PossibleRampCount = 1;
                                }
                                else
                                {
                                    if ( Value < BestDistB )
                                    {
                                        BestDistB = Value;
                                        PossibleRamps[0] = Connections[B];
                                        PossibleRampCount = 1;
                                    }
                                    else if ( Value == BestDistB )
                                    {
                                        PossibleRamps[PossibleRampCount] = Connections[B];
                                        PossibleRampCount++;
                                    }
                                }
                            }
                            else if ( ResultPaths[0].PathCount != 1 )
                            {
                                ReturnResult.ProblemAdd("Error: Invalid number of routes returned.");
                                goto Finish;
                            }
                            else if ( ResultPaths[0].Paths[0].Value / RampDist > BestDist )
                            {
                                BestDist = ResultPaths[0].Paths[0].Value / RampDist;
                                PossibleRamps[0] = Connections[B];
                                PossibleRampCount = 1;
                            }
                            else if ( ResultPaths[0].Paths[0].Value / RampDist == BestDist )
                            {
                                PossibleRamps[PossibleRampCount] = Connections[B];
                                PossibleRampCount++;
                            }
                            else if ( ResultPaths[0].Paths[0].Value <= RampDist )
                            {
                                ConnectionsCanRamp[B] = false;
                            }
                        }
                        else
                        {
                            ConnectionsCanRamp[B] = false;
                        }
                    }
                    else
                    {
                        ConnectionsCanRamp[B] = false;
                    }
                }
                if ( PossibleRampCount > 0 )
                {
                    BestNum = (int)(Conversion.Int(VBMath.Rnd() * PossibleRampCount));
                    PossibleRamps[BestNum].IsRamp = true;
                    tmpPassageNodeA = PossibleRamps[BestNum].PassageNodeA;
                    tmpPassageNodeB = PossibleRamps[BestNum].PassageNodeB;
                    tmpNodeA = PassageNodePathNodes[tmpPassageNodeA.MirrorNum, tmpPassageNodeA.Num];
                    tmpNodeB = PassageNodePathNodes[tmpPassageNodeB.MirrorNum, tmpPassageNodeB.Num];
                    NewConnection = tmpNodeA.CreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                    for ( C = 0; C <= PossibleRamps[BestNum].ReflectionCount - 1; C++ )
                    {
                        PossibleRamps[BestNum].Reflections[C].IsRamp = true;
                        tmpPassageNodeA = PossibleRamps[BestNum].Reflections[C].PassageNodeA;
                        tmpPassageNodeB = PossibleRamps[BestNum].Reflections[C].PassageNodeB;
                        tmpNodeA = PassageNodePathNodes[tmpPassageNodeA.MirrorNum, tmpPassageNodeA.Num];
                        tmpNodeB = PassageNodePathNodes[tmpPassageNodeB.MirrorNum, tmpPassageNodeB.Num];
                        NewConnection = tmpNodeA.CreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                    }
                    PassageNodeNetwork.Network.FindCalc();
                    for ( E = 0; E <= PassageNodeCount - 1; E++ )
                    {
                        UpdateNetworkConnectednessArgs.PassageNodeUpdated[E] = false;
                    }
                    if ( PossibleRamps[BestNum].PassageNodeA.MirrorNum == 0 )
                    {
                        UpdateNetworkConnectedness(UpdateNetworkConnectednessArgs, PossibleRamps[BestNum].PassageNodeA);
                    }
                    else if ( PossibleRamps[BestNum].PassageNodeB.MirrorNum == 0 )
                    {
                        UpdateNetworkConnectedness(UpdateNetworkConnectednessArgs, PossibleRamps[BestNum].PassageNodeB);
                    }
                    else
                    {
                        ReturnResult.ProblemAdd("Error: Initial ramp not in area 0.");
                        goto Finish;
                    }
                }
                else
                {
                    break;
                }
            } while ( true );

            PathfinderNetwork.sFloodProximityArgs FloodArgs = new PathfinderNetwork.sFloodProximityArgs();
            FloodArgs.StartNode = PassageNodeNetwork.PassageNodePathNodes[0, 0];
            FloodArgs.NodeValues = PassageNodeNetwork.Network.NetworkLargeArrays.Nodes_ValuesA;
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    FloodArgs.NodeValues[PassageNodeNetwork.PassageNodePathNodes[B, A].Layer_NodeNum] = float.MaxValue;
                }
            }
            PassageNodeNetwork.Network.FloodProximity(ref FloodArgs);
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    if ( !PassageNodes[B, A].IsWater )
                    {
                        if ( FloodArgs.NodeValues[PassageNodeNetwork.PassageNodePathNodes[B, A].Layer_NodeNum] == float.MaxValue )
                        {
                            ReturnResult.ProblemAdd("Land is unreachable. Reduce variation or retry.");
                            goto Finish;
                        }
                    }
                }
            }

            Finish:
            PassageNodeNetwork.Network.Deallocate();

            return ReturnResult;
        }

        private class clsPassageNodeNework
        {
            public PathfinderNetwork Network;
            public PathfinderNode[,] PassageNodePathNodes;
        }

        private clsPassageNodeNework MakePassageNodeNetwork()
        {
            clsPassageNodeNework ReturnResult = new clsPassageNodeNework();
            PathfinderConnection NewConnection;
            clsNodeTag NodeTag = default(clsNodeTag);
            PathfinderNode tmpNodeA = default(PathfinderNode);
            PathfinderNode tmpNodeB = default(PathfinderNode);
            int A = 0;
            int B = 0;

            ReturnResult.Network = new PathfinderNetwork();
            ReturnResult.PassageNodePathNodes = new PathfinderNode[SymmetryBlockCount, PassageNodeCount];
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    ReturnResult.PassageNodePathNodes[B, A] = new PathfinderNode(ReturnResult.Network);
                    NodeTag = new clsNodeTag();
                    NodeTag.Pos = PassageNodes[B, A].Pos;
                    ReturnResult.PassageNodePathNodes[B, A].Tag = NodeTag;
                }
            }
            for ( A = 0; A <= ConnectionCount - 1; A++ )
            {
                if ( Connections[A].PassageNodeA.Level == Connections[A].PassageNodeB.Level || Connections[A].IsRamp )
                {
                    if ( !(Connections[A].PassageNodeA.IsWater || Connections[A].PassageNodeB.IsWater) )
                    {
                        tmpNodeA = ReturnResult.PassageNodePathNodes[Connections[A].PassageNodeA.MirrorNum, Connections[A].PassageNodeA.Num];
                        tmpNodeB = ReturnResult.PassageNodePathNodes[Connections[A].PassageNodeB.MirrorNum, Connections[A].PassageNodeB.Num];
                        NewConnection = tmpNodeA.CreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB));
                    }
                }
            }
            ReturnResult.Network.LargeArraysResize();
            ReturnResult.Network.FindCalc();

            return ReturnResult;
        }

        public clsResult GenerateOil()
        {
            clsResult ReturnResult = new clsResult("Oil");

            int A = 0;
            int B = 0;
            int C = 0;
            int D = 0;

            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    PassageNodes[B, A].OilCount = 0;
                }
            }

            //store passage node route distances
            clsPassageNodeNework PassageNodePathMap = MakePassageNodeNetwork();
            PathfinderNode[] GetPathStartNodes = new PathfinderNode[1];
            PathfinderNetwork.PathList[] ResultPaths = null;

            PassageNodeDists = new float[SymmetryBlockCount, PassageNodeCount, SymmetryBlockCount, PassageNodeCount];
            for ( A = 0; A <= PassageNodeCount - 1; A++ )
            {
                for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                {
                    PassageNodeDists[D, A, D, A] = 0.0F;
                    for ( B = 0; B <= PassageNodeCount - 1; B++ )
                    {
                        for ( C = 0; C <= SymmetryBlockCount - 1; C++ )
                        {
                            if ( PassageNodes[0, A].IsWater || PassageNodes[C, B].IsWater || (C != 0 & D != 0) )
                            {
                                PassageNodeDists[D, A, C, B] = float.MaxValue;
                                PassageNodeDists[C, B, D, A] = float.MaxValue;
                            }
                            else
                            {
                                GetPathStartNodes[0] = PassageNodePathMap.PassageNodePathNodes[D, A];
                                ResultPaths = PassageNodePathMap.Network.GetPath(GetPathStartNodes, PassageNodePathMap.PassageNodePathNodes[C, B], -1, 0);
                                if ( ResultPaths == null )
                                {
                                    ReturnResult.ProblemAdd("Map is not all connected.");
                                    PassageNodePathMap.Network.Deallocate();
                                    return ReturnResult;
                                }
                                else
                                {
                                    if ( ResultPaths[0].PathCount != 1 )
                                    {
                                        Debugger.Break();
                                    }
                                    PassageNodeDists[D, A, C, B] = ResultPaths[0].Paths[0].Value;
                                    PassageNodeDists[C, B, D, A] = ResultPaths[0].Paths[0].Value;
                                }
                            }
                        }
                    }
                }
            }

            PassageNodePathMap.Network.Deallocate();

            //place oil
            int PlacedExtraOilCount = 0;
            int MaxBestNodeCount = 0;
            MaxBestNodeCount = 1;
            for ( A = 0; A <= OilAtATime - 1; A++ )
            {
                MaxBestNodeCount *= PassageNodeCount;
            }
            var oilArgs = new clsOilBalanceLoopArgs
                {
                    OilClusterSizes = new int[OilAtATime],
                    PlayerOilScore = new double[TopLeftPlayerCount],
                    OilNodes = new clsPassageNode[OilAtATime]
                };

            //balanced oil
            while ( PlacedExtraOilCount < ExtraOilCount )
            {
                //place oil farthest away from other oil and where it best balances the player oil score
                for ( A = 0; A <= OilAtATime - 1; A++ )
                {
                    oilArgs.OilClusterSizes[A] =
                        Math.Min(ExtraOilClusterSizeMin + (int)(Conversion.Int(VBMath.Rnd() * (ExtraOilClusterSizeMax - ExtraOilClusterSizeMin + 1))),
                            Math.Max((int)(Math.Ceiling(Convert.ToDecimal((ExtraOilCount - PlacedExtraOilCount) / SymmetryBlockCount))), 1));
                }
                oilArgs.OilPossibilities = new clsOilPossibilities();
                OilBalanceLoop(oilArgs, 0);

                clsOilPossibilities.clsPossibility bestPossibility = oilArgs.OilPossibilities.BestPossibility;

                if ( bestPossibility != null )
                {
                    for ( B = 0; B <= OilAtATime - 1; B++ )
                    {
                        for ( A = 0; A <= SymmetryBlockCount - 1; A++ )
                        {
                            PassageNodes[A, bestPossibility.Nodes[B].Num].OilCount += oilArgs.OilClusterSizes[B];
                        }
                        PlacedExtraOilCount += oilArgs.OilClusterSizes[B] * SymmetryBlockCount;
                    }
                    for ( A = 0; A <= TopLeftPlayerCount - 1; A++ )
                    {
                        oilArgs.PlayerOilScore[A] += bestPossibility.PlayerOilScoreAddition[A];
                    }
                }
                else
                {
                    ReturnResult.WarningAdd("Could not place all of the oil. " + Convert.ToString(PlacedExtraOilCount) + " oil was placed.");
                    break;
                }
            }

            //base oil
            for ( A = 0; A <= TopLeftPlayerCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    PassageNodes[B, PlayerBases[A].Nodes[0].Num].OilCount += BaseOilCount;
                }
            }

            return ReturnResult;
        }
    }
}