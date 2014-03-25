#region

using System;
using System.Diagnostics;
using System.Windows.Forms;
using NLog;
using SharpFlame.Core.Extensions;
using SharpFlame.Old.Collections.Specialized;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Old.Domain;
using SharpFlame.Old.Generators;
using SharpFlame.Old.Mapping;
using SharpFlame.Old.Mapping.Objects;
using SharpFlame.Old.Mapping.Tiles;
using SharpFlame.Old.Maths;
using SharpFlame.Old.Pathfinding;
using SharpFlame.Old.Util;

#endregion

namespace SharpFlame.Old
{
    public class clsGenerateMap
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public int BaseFlatArea;
        public int BaseLevel;
        public int BaseOilCount;
        public int BaseTruckCount;
        private int connectionCount;
        private clsConnection[] connections;
        public int ExtraOilClusterSizeMax;
        public int ExtraOilClusterSizeMin;
        public int ExtraOilCount;
        public float FeatureClusterChance;
        public int FeatureClusterMaxUnits;
        public int FeatureClusterMinUnits;
        public int FeatureScatterCount;
        public int FeatureScatterGap;
        public int FlatsChance;
        public GenerateTerrainTile[,] GenerateTerrainTiles = new GenerateTerrainTile[0, 0];
        public GenerateTerrainVertex[,] GenerateTerrainVertices;
        public clsGeneratorTileset GenerateTileset;
        public int JitterScale;
        public int LevelCount;
        public float LevelHeight;
        public Map Map;
        public float MaxDisconnectionDist;
        public int MaxLevelTransition;
        private int nearestCount;
        private clsNearest[] nearests;
        public float NodeScale;
        public int OilAtATime;
        public float OilDispersion;
        private int passageNodeCount;
        private float[,,,] passageNodeDists;
        private clsPassageNode[,] passageNodes;
        public int PassagesChance;
        public XYInt[] PlayerBasePos;

        private sPlayerBase[] playerBases;
        public double RampBase;
        public int SymmetryBlockCount;
        public XYInt SymmetryBlockCountXY;
        public sSymmetryBlock[] SymmetryBlocks;
        public bool SymmetryIsRotational;


        public PathfinderNetwork TilePathMap;
        public XYInt TileSize;
        public int TopLeftPlayerCount;
        private int totalPlayerCount;
        public int TotalWaterQuantity;
        public int VariationChance;
        public PathfinderNetwork VertexPathMap;
        public int WaterSpawnQuantity;

        public int GetTotalPlayerCount
        {
            get { return totalPlayerCount; }
        }

        public Result GenerateLayout()
        {
            var returnResult = new Result("Layout", false);
            logger.Info("Generating Layouts");

            var x = 0;
            var y = 0;
            var a = 0;
            var b = 0;
            var c = 0;
            var d = 0;
            var e = 0;
            var g = 0;
            var h = 0;

            totalPlayerCount = TopLeftPlayerCount * SymmetryBlockCount;

            var SymmetrySize = new XYInt();

            SymmetrySize.X = TileSize.X * Constants.TerrainGridSpacing / SymmetryBlockCountXY.X;
            SymmetrySize.Y = TileSize.Y * Constants.TerrainGridSpacing / SymmetryBlockCountXY.Y;

            //create passage nodes

            var passageRadius = (128.0F * NodeScale).ToInt();
            var maxLikelyPassageNodeCount = 0;
            maxLikelyPassageNodeCount = Math.Ceiling(Convert.ToDecimal(2.0D * TileSize.X * 128 * TileSize.Y * 128 / (Math.PI * passageRadius * passageRadius))).ToInt();

            passageNodes = new clsPassageNode[SymmetryBlockCount, maxLikelyPassageNodeCount];
            const int EdgeOffset = 0 * 128;
            var edgeSections = new XYInt();
            var edgeSectionSize = default(XYDouble);
            var newPointPos = new XYInt();

            if ( SymmetryBlockCountXY.X == 1 )
            {
                edgeSections.X = ((TileSize.X * Constants.TerrainGridSpacing - EdgeOffset * 2.0D) / (NodeScale * Constants.TerrainGridSpacing * 2.0F)).Floor().ToInt();
                edgeSectionSize.X = (TileSize.X * Constants.TerrainGridSpacing - EdgeOffset * 2.0D) / edgeSections.X;
                edgeSections.X--;
            }
            else
            {
                edgeSections.X = ((TileSize.X * Constants.TerrainGridSpacing / SymmetryBlockCountXY.X - EdgeOffset) / (NodeScale * Constants.TerrainGridSpacing * 2.0F) - 0.5D).Floor().ToInt();
                edgeSectionSize.X = (TileSize.X * Constants.TerrainGridSpacing / SymmetryBlockCountXY.X - EdgeOffset) /
                                    (((TileSize.X * Constants.TerrainGridSpacing / SymmetryBlockCountXY.X - EdgeOffset) /
                                      (NodeScale * Constants.TerrainGridSpacing * 2.0F) - 0.5D).Floor() + 0.5D);
            }

            if ( SymmetryBlockCountXY.Y == 1 )
            {
                edgeSections.Y = (
                    (TileSize.Y * Constants.TerrainGridSpacing - EdgeOffset * 2.0D) / (NodeScale * Constants.TerrainGridSpacing * 2.0F)
                    ).Floor().ToInt();
                edgeSectionSize.Y = (TileSize.Y * Constants.TerrainGridSpacing - EdgeOffset * 2.0D) / edgeSections.Y;
                edgeSections.Y--;
            }
            else
            {
                edgeSections.Y =(
                        (TileSize.Y * Constants.TerrainGridSpacing / SymmetryBlockCountXY.Y - EdgeOffset) /
                        (NodeScale * Constants.TerrainGridSpacing * 2.0F) - 0.5D
                        ).Floor().ToInt();
                edgeSectionSize.Y =
                    Convert.ToDouble((TileSize.Y * Constants.TerrainGridSpacing / SymmetryBlockCountXY.Y - EdgeOffset) /
                                     (Convert.ToDouble(
                                         ((TileSize.Y * Constants.TerrainGridSpacing / SymmetryBlockCountXY.Y - EdgeOffset) /
                                          (NodeScale * Constants.TerrainGridSpacing * 2.0F) - 0.5D)) + 0.5D));
            }

            passageNodeCount = 0;
            for ( y = 0; y <= edgeSections.Y; y++ )
            {
                if ( !MakePassageNodes(new XYInt(EdgeOffset, EdgeOffset + (y * edgeSectionSize.Y).ToInt()), true) )
                {
                    returnResult.ProblemAdd("Error: Bad border node.");
                    return returnResult;
                }
                if ( SymmetryBlockCountXY.X == 1 )
                {
                    if (
                        !MakePassageNodes(new XYInt(TileSize.X * Constants.TerrainGridSpacing - EdgeOffset, EdgeOffset + Convert.ToInt32(y * edgeSectionSize.Y)), true) )
                    {
                        returnResult.ProblemAdd("Error: Bad border node.");
                        return returnResult;
                    }
                }
            }
            for ( x = 1; x <= edgeSections.X; x++ )
            {
                if ( !MakePassageNodes(new XYInt(EdgeOffset + Convert.ToInt32(x * edgeSectionSize.X), EdgeOffset), true) )
                {
                    returnResult.ProblemAdd("Error: Bad border node.");
                    return returnResult;
                }
                if ( SymmetryBlockCountXY.Y == 1 )
                {
                    if (
                        !MakePassageNodes(new XYInt(EdgeOffset + Convert.ToInt32(x * edgeSectionSize.X), TileSize.Y * Constants.TerrainGridSpacing - EdgeOffset), true) )
                    {
                        returnResult.ProblemAdd("Error: Bad border node.");
                        return returnResult;
                    }
                }
            }
            do
            {
                var loopCount = 0;
                do
                {
                    if ( SymmetryBlockCountXY.X == 1 )
                    {
                        newPointPos.X = EdgeOffset + (App.Random.Next() * (SymmetrySize.X - EdgeOffset * 2 + 1));
                    }
                    else
                    {
                        newPointPos.X = EdgeOffset + App.Random.Next() * (SymmetrySize.X - EdgeOffset + 1);
                    }
                    if ( SymmetryBlockCountXY.Y == 1 )
                    {
                        newPointPos.Y = EdgeOffset + App.Random.Next() * (SymmetrySize.Y - EdgeOffset * 2 + 1);
                    }
                    else
                    {
                        newPointPos.Y = EdgeOffset + Convert.ToInt32((App.Random.Next() * (SymmetrySize.Y - EdgeOffset + 1)));
                    }
                    for ( a = 0; a <= passageNodeCount - 1; a++ )
                    {
                        for ( b = 0; b <= SymmetryBlockCount - 1; b++ )
                        {
                            if ( (passageNodes[b, a].Pos - newPointPos).ToDoubles().GetMagnitude() < passageRadius * 2 )
                            {
                                goto PointTooClose;
                            }
                        }
                    }
                    PointTooClose:
                    if ( a == passageNodeCount )
                    {
                        if ( MakePassageNodes(newPointPos, false) )
                        {
                            break;
                        }
                    }
                    loopCount++;
                    if ( loopCount >= (64.0F * TileSize.X * TileSize.Y / (NodeScale * NodeScale)).ToInt() )
                    {
                        goto PointMakingFinished;
                    }
                } while ( true );
            } while ( true );
            PointMakingFinished:
            var tmpPassgeNodes = new clsPassageNode[SymmetryBlockCount, passageNodeCount];
            Array.Copy(passageNodes, tmpPassgeNodes, passageNodeCount);
            passageNodes = tmpPassgeNodes;
            //connect until all are connected without intersecting

            var maxConDist2 = passageRadius * 2 * 4;
            maxConDist2 *= maxConDist2;
            var nearestA = default(clsNearest);
            nearests = new clsNearest[passageNodeCount * 64];
            var tmpPassageNodeA = default(clsPassageNode);
            var tmpPassageNodeB = default(clsPassageNode);
            var nearestArgs = new clsTestNearestArgs();
            var minConDist = (NodeScale * 1.25F * 128.0F).ToInt();

            nearestArgs.MaxConDist2 = maxConDist2;
            nearestArgs.MinConDist = minConDist;

            for ( a = 0; a <= passageNodeCount - 1; a++ )
            {
                nearestArgs.PassageNodeA = passageNodes[0, a];
                for ( b = a; b <= passageNodeCount - 1; b++ )
                {
                    for ( c = 0; c <= SymmetryBlockCount - 1; c++ )
                    {
                        nearestArgs.PassageNodeB = passageNodes[c, b];
                        if ( nearestArgs.PassageNodeA != nearestArgs.PassageNodeB )
                        {
                            TestNearest(nearestArgs);
                        }
                    }
                }
            }

            var flag = default(bool);

            for ( g = 0; g <= nearestCount - 1; g++ )
            {
                nearestA = nearests[g];
                for ( a = 0; a <= nearestA.NodeCount - 1; a++ )
                {
                    tmpPassageNodeA = nearestA.NodeA[a];
                    tmpPassageNodeB = nearestA.NodeB[a];
                    for ( h = 0; h <= nearestCount - 1; h++ )
                    {
                        var nearestB = nearests[h];
                        if ( nearestB != nearestA )
                        {
                            if ( nearestB.Dist2 < nearestA.Dist2 )
                            {
                                flag = true;
                            }
                            else if ( nearestB.Dist2 == nearestA.Dist2 )
                            {
                                flag = nearestA.Num > nearestB.Num;
                            }
                            else
                            {
                                flag = false;
                            }
                            if ( flag )
                            {
                                for ( b = 0; b <= nearestB.NodeCount - 1; b++ )
                                {
                                    if ( !(tmpPassageNodeA == nearestB.NodeA[b] || tmpPassageNodeA == nearestB.NodeB[b]
                                           || tmpPassageNodeB == nearestB.NodeA[b] || tmpPassageNodeB == nearestB.NodeB[b]) )
                                    {
                                        MathUtil.sIntersectPos intersectPos = MathUtil.GetLinesIntersectBetween(tmpPassageNodeA.Pos, tmpPassageNodeB.Pos, nearestB.NodeA[b].Pos,
                                            nearestB.NodeB[b].Pos);
                                        if ( intersectPos.Exists )
                                        {
                                            break;
                                        }
                                    }
                                }
                                if ( b < nearestB.NodeCount )
                                {
                                    nearestA.BlockedCount++;
                                    nearestB.BlockedNearests[nearestB.BlockedNearestCount] = nearestA;
                                    nearestB.BlockedNearestCount++;
                                }
                            }
                        }
                    }
                }
            }

            var ChangeCount = 0;
            connections = new clsConnection[passageNodeCount * 16];

            do
            {
                //create valid connections
                ChangeCount = 0;
                g = 0;
                while ( g < nearestCount )
                {
                    nearestA = nearests[g];
                    flag = true;
                    if ( nearestA.BlockedCount == 0 && flag )
                    {
                        var f = connectionCount;
                        for ( d = 0; d <= nearestA.NodeCount - 1; d++ )
                        {
                            connections[connectionCount] = new clsConnection(nearestA.NodeA[d], nearestA.NodeB[d]);
                            connectionCount++;
                        }
                        for ( d = 0; d <= nearestA.NodeCount - 1; d++ )
                        {
                            a = f + d;
                            connections[a].ReflectionCount = nearestA.NodeCount - 1;
                            connections[a].Reflections = new clsConnection[connections[a].ReflectionCount];
                            b = 0;
                            for ( e = 0; e <= nearestA.NodeCount - 1; e++ )
                            {
                                if ( e != d )
                                {
                                    connections[a].Reflections[b] = connections[f + e];
                                    b++;
                                }
                            }
                        }
                        for ( c = 0; c <= nearestA.BlockedNearestCount - 1; c++ )
                        {
                            nearestA.BlockedNearests[c].Invalid = true;
                        }
                        nearestCount--;
                        h = nearestA.Num;
                        nearestA.Num = -1;
                        if ( h != nearestCount )
                        {
                            nearests[h] = nearests[nearestCount];
                            nearests[h].Num = h;
                        }
                        ChangeCount++;
                    }
                    else
                    {
                        if ( !flag )
                        {
                            nearestA.Invalid = true;
                        }
                        g++;
                    }
                }
                //remove blocked ones and their blocking effect
                g = 0;
                while ( g < nearestCount )
                {
                    nearestA = nearests[g];
                    if ( nearestA.Invalid )
                    {
                        nearestA.Num = -1;
                        for ( d = 0; d <= nearestA.BlockedNearestCount - 1; d++ )
                        {
                            nearestA.BlockedNearests[d].BlockedCount--;
                        }
                        nearestCount--;
                        if ( g != nearestCount )
                        {
                            nearests[g] = nearests[nearestCount];
                            nearests[g].Num = g;
                        }
                    }
                    else
                    {
                        g++;
                    }
                }
            } while ( ChangeCount > 0 );

            //put connections in order of angle

            for ( a = 0; a <= passageNodeCount - 1; a++ )
            {
                for ( b = 0; b <= SymmetryBlockCount - 1; b++ )
                {
                    passageNodes[b, a].ReorderConnections();
                    passageNodes[b, a].CalcIsNearBorder();
                }
            }

            //get nodes in random order

            var PassageNodeListOrder = new clsPassageNode[passageNodeCount];
            var PassageNodeListOrderCount = 0;
            var PassageNodeOrder = new clsPassageNode[passageNodeCount];
            for ( a = 0; a <= passageNodeCount - 1; a++ )
            {
                PassageNodeListOrder[PassageNodeListOrderCount] = passageNodes[0, a];
                PassageNodeListOrderCount++;
            }
            b = 0;
            while ( PassageNodeListOrderCount > 0 )
            {
                a = App.Random.Next() * PassageNodeListOrderCount;
                PassageNodeOrder[b] = PassageNodeListOrder[a];
                b++;
                PassageNodeListOrderCount--;
                PassageNodeListOrder[a] = PassageNodeListOrder[PassageNodeListOrderCount];
            }

            //designate height levels

            LevelHeight = 255.0F / (LevelCount - 1);
            var BestNum = 0;
            double Dist = 0;
            var HeightsArgs = new clsPassageNodeHeightLevelArgs();
            HeightsArgs.PassageNodesMinLevel.Nodes = new int[passageNodeCount];
            HeightsArgs.PassageNodesMaxLevel.Nodes = new int[passageNodeCount];
            HeightsArgs.MapLevelCount = new int[LevelCount];
            var RotatedPos = new XYInt();

            for ( a = 0; a <= passageNodeCount - 1; a++ )
            {
                HeightsArgs.PassageNodesMinLevel.Nodes[a] = 0;
                HeightsArgs.PassageNodesMaxLevel.Nodes[a] = LevelCount - 1;
            }

            //create bases
            var BestDists = new double[BaseFlatArea];
            var BestNodes = new clsPassageNode[BaseFlatArea];
            var BestNodesReflectionNums = new int[BaseFlatArea];
            var BestDistCount = 0;
            playerBases = new sPlayerBase[totalPlayerCount];
            for ( b = 0; b <= TopLeftPlayerCount - 1; b++ )
            {
                BestDistCount = 0;
                for ( a = 0; a <= passageNodeCount - 1; a++ )
                {
                    for ( e = 0; e <= SymmetryBlockCount - 1; e++ )
                    {
                        tmpPassageNodeA = passageNodes[e, a];
                        if ( !tmpPassageNodeA.IsOnBorder )
                        {
                            Dist = (tmpPassageNodeA.Pos - PlayerBasePos[b]).ToDoubles().GetMagnitude();
                            for ( c = BestDistCount - 1; c >= 0; c-- )
                            {
                                if ( Dist > BestDists[c] )
                                {
                                    break;
                                }
                            }
                            c++;
                            for ( d = Math.Min(BestDistCount - 1, BaseFlatArea - 2); d >= c; d-- )
                            {
                                BestDists[d + 1] = BestDists[d];
                                BestNodes[d + 1] = BestNodes[d];
                            }
                            if ( c < BaseFlatArea )
                            {
                                BestDists[c] = Dist;
                                BestNodes[c] = tmpPassageNodeA;
                                BestDistCount = Math.Max(BestDistCount, c + 1);
                            }
                        }
                    }
                }

                if ( BaseLevel < 0 )
                {
                    d = Convert.ToInt32((App.Random.Next() * LevelCount));
                }
                else
                {
                    d = BaseLevel;
                }

                HeightsArgs.MapLevelCount[d] += BestDistCount;

                for ( a = 0; a <= BestDistCount - 1; a++ )
                {
                    if ( BestNodes[a].MirrorNum == 0 )
                    {
                        BestNodesReflectionNums[a] = -1;
                    }
                    else
                    {
                        for ( c = 0; c <= (SymmetryBlockCount / 2.0D).ToInt() - 1; c++ )
                        {
                            if ( SymmetryBlocks[0].ReflectToNum[c] == BestNodes[a].MirrorNum )
                            {
                                break;
                            }
                        }
                        BestNodesReflectionNums[a] = c;
                    }
                }

                for ( a = 0; a <= SymmetryBlockCount - 1; a++ )
                {
                    e = a * TopLeftPlayerCount + b;
                    playerBases[e].NodeCount = BestDistCount;
                    playerBases[e].Nodes = new clsPassageNode[playerBases[e].NodeCount];
                    for ( c = 0; c <= BestDistCount - 1; c++ )
                    {
                        if ( BestNodesReflectionNums[c] < 0 )
                        {
                            playerBases[e].Nodes[c] = passageNodes[a, BestNodes[c].Num];
                        }
                        else
                        {
                            playerBases[e].Nodes[c] = passageNodes[SymmetryBlocks[a].ReflectToNum[BestNodesReflectionNums[c]], BestNodes[c].Num];
                        }
                        playerBases[e].Nodes[c].PlayerBaseNum = e;
                        playerBases[e].Nodes[c].Level = d;
                        PassageNodesMinLevelSet(playerBases[e].Nodes[c], HeightsArgs.PassageNodesMinLevel, d, MaxLevelTransition);
                        PassageNodesMaxLevelSet(playerBases[e].Nodes[c], HeightsArgs.PassageNodesMaxLevel, d, MaxLevelTransition);
                    }
                    //PlayerBases(E).CalcPos()
                    RotatedPos = TileUtil.GetRotatedPos(SymmetryBlocks[a].Orientation, PlayerBasePos[b],
                        new XYInt(SymmetrySize.X - 1, SymmetrySize.Y - 1));
                    playerBases[e].Pos.X = SymmetryBlocks[a].XYNum.X * SymmetrySize.X + RotatedPos.X;
                    playerBases[e].Pos.Y = SymmetryBlocks[a].XYNum.Y * SymmetrySize.Y + RotatedPos.Y;
                }
            }

            var WaterCount = 0;
            var CanDoFlatsAroundWater = default(bool);
            var TotalWater = 0;
            var WaterSpawns = 0;

            for ( a = 0; a <= passageNodeCount - 1; a++ )
            {
                tmpPassageNodeA = PassageNodeOrder[a];
                if ( tmpPassageNodeA.Level < 0 && !tmpPassageNodeA.IsOnBorder )
                {
                    WaterCount = 0;
                    for ( b = 0; b <= tmpPassageNodeA.ConnectionCount - 1; b++ )
                    {
                        tmpPassageNodeB = tmpPassageNodeA.Connections[b].GetOther();
                        if ( tmpPassageNodeB.IsWater )
                        {
                            WaterCount++;
                        }
                    }
                    CanDoFlatsAroundWater = true;
                    for ( b = 0; b <= tmpPassageNodeA.ConnectionCount - 1; b++ )
                    {
                        if ( HeightsArgs.PassageNodesMinLevel.Nodes[tmpPassageNodeA.Connections[b].GetOther().Num] > 0 )
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
                        c = tmpPassageNodeA.Num;
                        for ( d = 0; d <= SymmetryBlockCount - 1; d++ )
                        {
                            passageNodes[d, c].IsWater = true;
                            passageNodes[d, c].Level = 0;
                        }
                        PassageNodesMinLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMinLevel, 0, MaxLevelTransition);
                        PassageNodesMaxLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMaxLevel, 0, MaxLevelTransition);
                        HeightsArgs.MapLevelCount[0]++;
                        for ( b = 0; b <= tmpPassageNodeA.ConnectionCount - 1; b++ )
                        {
                            tmpPassageNodeB = tmpPassageNodeA.Connections[b].GetOther();
                            PassageNodesMinLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMinLevel, 0, MaxLevelTransition);
                            PassageNodesMaxLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMaxLevel, 0, MaxLevelTransition);
                        }
                    }
                }
            }

            var tmpPassageNodeC = default(clsPassageNode);
            var Result = new SimpleResult();

            HeightsArgs.FlatsCutoff = 1;
            HeightsArgs.PassagesCutoff = 1;
            HeightsArgs.VariationCutoff = 1;
            HeightsArgs.ActionTotal = 1;

            for ( a = 0; a <= passageNodeCount - 1; a++ )
            {
                tmpPassageNodeA = PassageNodeOrder[a];
                if ( tmpPassageNodeA.Level < 0 && !tmpPassageNodeA.IsOnBorder && tmpPassageNodeA.IsNearBorder )
                {
                    HeightsArgs.PassageNode = tmpPassageNodeA;
                    Result = PassageNodeHeightLevel(HeightsArgs);
                    if ( !Result.Success )
                    {
                        returnResult.ProblemAdd(Result.Problem);
                        return returnResult;
                    }
                }
            }

            HeightsArgs.FlatsCutoff = FlatsChance;
            HeightsArgs.PassagesCutoff = HeightsArgs.FlatsCutoff + PassagesChance;
            HeightsArgs.VariationCutoff = HeightsArgs.PassagesCutoff + VariationChance;
            HeightsArgs.ActionTotal = HeightsArgs.VariationCutoff;
            if ( HeightsArgs.ActionTotal <= 0 )
            {
                returnResult.ProblemAdd("All height level behaviors are zero");
                return returnResult;
            }

            for ( a = 0; a <= passageNodeCount - 1; a++ )
            {
                tmpPassageNodeA = PassageNodeOrder[a];
                if ( tmpPassageNodeA.Level < 0 && !tmpPassageNodeA.IsOnBorder )
                {
                    HeightsArgs.PassageNode = tmpPassageNodeA;
                    Result = PassageNodeHeightLevel(HeightsArgs);
                    if ( !Result.Success )
                    {
                        returnResult.ProblemAdd(Result.Problem);
                        return returnResult;
                    }
                }
            }

            //set edge points to the level of their neighbour
            for ( a = 0; a <= passageNodeCount - 1; a++ )
            {
                tmpPassageNodeA = passageNodes[0, a];
                if ( tmpPassageNodeA.IsOnBorder )
                {
                    if ( tmpPassageNodeA.Level >= 0 )
                    {
                        returnResult.ProblemAdd("Error: Border has had its height set.");
                        return returnResult;
                    }
                    //If tmpPassageNodeA.ConnectionCount <> 1 Then
                    //    ReturnResult.Problem = "Error: Border has incorrect connections."
                    //    Exit Function
                    //End If
                    tmpPassageNodeC = null;
                    CanDoFlatsAroundWater = true;
                    for ( b = 0; b <= tmpPassageNodeA.ConnectionCount - 1; b++ )
                    {
                        tmpPassageNodeB = tmpPassageNodeA.Connections[b].GetOther();
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
                        for ( d = 0; d <= SymmetryBlockCount - 1; d++ )
                        {
                            passageNodes[d, a].IsWater = tmpPassageNodeC.IsWater && CanDoFlatsAroundWater;
                            passageNodes[d, a].Level = BestNum;
                        }
                        if ( tmpPassageNodeA.IsWater )
                        {
                            for ( b = 0; b <= tmpPassageNodeA.ConnectionCount - 1; b++ )
                            {
                                tmpPassageNodeB = tmpPassageNodeA.Connections[b].GetOther();
                                PassageNodesMinLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMinLevel, tmpPassageNodeA.Level, MaxLevelTransition);
                                PassageNodesMaxLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMaxLevel, tmpPassageNodeA.Level, MaxLevelTransition);
                            }
                        }
                    }
                }
                else if ( tmpPassageNodeA.Level < 0 )
                {
                    returnResult.ProblemAdd("Error: Node height not set");
                    return returnResult;
                }
            }
            //set level of edge points only connected to another border point
            for ( a = 0; a <= passageNodeCount - 1; a++ )
            {
                tmpPassageNodeA = passageNodes[0, a];
                if ( tmpPassageNodeA.IsOnBorder && tmpPassageNodeA.Level < 0 )
                {
                    tmpPassageNodeC = null;
                    CanDoFlatsAroundWater = true;
                    for ( b = 0; b <= tmpPassageNodeA.ConnectionCount - 1; b++ )
                    {
                        tmpPassageNodeB = tmpPassageNodeA.Connections[b].GetOther();
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
                        returnResult.ProblemAdd("Error: No connection for border node");
                        return returnResult;
                    }
                    BestNum = tmpPassageNodeC.Level;
                    PassageNodesMinLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMinLevel, BestNum, MaxLevelTransition);
                    PassageNodesMaxLevelSet(tmpPassageNodeA, HeightsArgs.PassageNodesMaxLevel, BestNum, MaxLevelTransition);
                    for ( d = 0; d <= SymmetryBlockCount - 1; d++ )
                    {
                        passageNodes[d, a].IsWater = tmpPassageNodeC.IsWater && CanDoFlatsAroundWater;
                        passageNodes[d, a].Level = BestNum;
                    }
                    if ( tmpPassageNodeA.IsWater )
                    {
                        for ( b = 0; b <= tmpPassageNodeA.ConnectionCount - 1; b++ )
                        {
                            tmpPassageNodeB = tmpPassageNodeA.Connections[b].GetOther();
                            PassageNodesMinLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMinLevel, tmpPassageNodeA.Level, MaxLevelTransition);
                            PassageNodesMaxLevelSet(tmpPassageNodeB, HeightsArgs.PassageNodesMaxLevel, tmpPassageNodeA.Level, MaxLevelTransition);
                        }
                    }
                }
            }

            RampBase = 1.0D;
            MaxDisconnectionDist = 99999.0F;

            var RampResult = GenerateRamps();
            returnResult.Add(RampResult);

            return returnResult;
        }

        private bool TestNearest(clsTestNearestArgs args)
        {
            var xyInt = new XYInt();
            var nearestA = default(clsNearest);
            var dist2 = 0;
            var index0 = 0;
            var index1 = 0;

            if ( args.PassageNodeA.MirrorNum != 0 )
            {
                Debugger.Break();
                return false;
            }

            xyInt.X = args.PassageNodeB.Pos.X - args.PassageNodeA.Pos.X;
            xyInt.Y = args.PassageNodeB.Pos.Y - args.PassageNodeA.Pos.Y;
            dist2 = xyInt.X * xyInt.X + xyInt.Y * xyInt.Y;
            if ( dist2 > args.MaxConDist2 )
            {
                return false;
            }
            for ( index0 = 0; index0 <= passageNodeCount - 1; index0++ )
            {
                for ( index1 = 0; index1 <= SymmetryBlockCount - 1; index1++ )
                {
                    if ( passageNodes[index1, index0] != args.PassageNodeA && passageNodes[index1, index0] != args.PassageNodeB )
                    {
                        xyInt = MathUtil.PointGetClosestPosOnLine(args.PassageNodeA.Pos, args.PassageNodeB.Pos, passageNodes[index1, index0].Pos);
                        if ( (xyInt - passageNodes[index1, index0].Pos).ToDoubles().GetMagnitude() < args.MinConDist )
                        {
                            return false;
                        }
                    }
                }
            }

            nearestA = new clsNearest();
            nearestA.Num = nearestCount;
            nearestA.Dist2 = dist2;
            if ( args.PassageNodeA.MirrorNum == args.PassageNodeB.MirrorNum )
            {
                nearestA.NodeA = new clsPassageNode[SymmetryBlockCount];
                nearestA.NodeB = new clsPassageNode[SymmetryBlockCount];
                for ( index0 = 0; index0 <= SymmetryBlockCount - 1; index0++ )
                {
                    nearestA.NodeA[index0] = passageNodes[index0, args.PassageNodeA.Num];
                    nearestA.NodeB[index0] = passageNodes[index0, args.PassageNodeB.Num];
                }
                nearestA.NodeCount = SymmetryBlockCount;
            }
            else
            {
                if ( SymmetryIsRotational )
                {
                    nearestA.NodeA = new clsPassageNode[SymmetryBlockCount];
                    nearestA.NodeB = new clsPassageNode[SymmetryBlockCount];
                    var reflectionCount = (SymmetryBlockCount / 2.0D).ToInt();
                    var reflectionNum = 0;                  
                    for ( reflectionNum = 0; reflectionNum <= reflectionCount - 1; reflectionNum++ )
                    {
                        if ( SymmetryBlocks[0].ReflectToNum[reflectionNum] == args.PassageNodeB.MirrorNum )
                        {
                            break;
                        }
                    }
                    if ( reflectionNum == reflectionCount )
                    {
                        return false;
                    }
                    for ( index0 = 0; index0 <= SymmetryBlockCount - 1; index0++ )
                    {
                        nearestA.NodeA[index0] = passageNodes[index0, args.PassageNodeA.Num];
                        nearestA.NodeB[index0] = passageNodes[SymmetryBlocks[index0].ReflectToNum[reflectionNum], args.PassageNodeB.Num];
                    }
                    nearestA.NodeCount = SymmetryBlockCount;
                }
                else
                {
                    if ( args.PassageNodeA.Num != args.PassageNodeB.Num )
                    {
                        return false;
                    }
                    if ( SymmetryBlockCount == 4 )
                    {
                        nearestA.NodeA = new clsPassageNode[2];
                        nearestA.NodeB = new clsPassageNode[2];
                        var reflectionCount = (SymmetryBlockCount / 2.0D).ToInt();
                        var reflectionNum = 0;
                        for ( reflectionNum = 0; reflectionNum <= reflectionCount - 1; reflectionNum++ )
                        {
                            if ( SymmetryBlocks[0].ReflectToNum[reflectionNum] == args.PassageNodeB.MirrorNum )
                            {
                                break;
                            }
                        }
                        if ( reflectionNum == reflectionCount )
                        {
                            return false;
                        }
                        nearestA.NodeA[0] = args.PassageNodeA;
                        nearestA.NodeB[0] = args.PassageNodeB;
                        index1 = Convert.ToInt32(SymmetryBlocks[0].ReflectToNum[1 - reflectionNum]);
                        nearestA.NodeA[1] = passageNodes[index1, args.PassageNodeA.Num];
                        nearestA.NodeB[1] = passageNodes[SymmetryBlocks[index1].ReflectToNum[reflectionNum], args.PassageNodeB.Num];
                        nearestA.NodeCount = 2;
                    }
                    else
                    {
                        nearestA.NodeA = new clsPassageNode[1];
                        nearestA.NodeB = new clsPassageNode[1];
                        nearestA.NodeA[0] = args.PassageNodeA;
                        nearestA.NodeB[0] = args.PassageNodeB;
                        nearestA.NodeCount = 1;
                    }
                }
            }

            nearestA.BlockedNearests = new clsNearest[512];
            nearests[nearestCount] = nearestA;
            nearestCount++;

            return true;
        }

        public float GetNodePosDist(PathfinderNode NodeA, PathfinderNode NodeB)
        {
            var TagA = (clsNodeTag)NodeA.Tag;
            var TagB = (clsNodeTag)NodeB.Tag;

            return Convert.ToSingle((TagA.Pos - TagB.Pos).ToDoubles().GetMagnitude());
        }

        public void CalcNodePos(PathfinderNode Node, ref XYDouble Pos, ref int SampleCount)
        {
            if ( Node.GetLayer.GetNetwork_LayerNum == 0 )
            {
                var NodeTag = default(clsNodeTag);
                NodeTag = (clsNodeTag)Node.Tag;
                Pos.X += NodeTag.Pos.X;
                Pos.Y += NodeTag.Pos.Y;
            }
            else
            {
                var A = 0;
                for ( A = 0; A <= Node.GetChildNodeCount - 1; A++ )
                {
                    CalcNodePos(Node.get_GetChildNode(A), ref Pos, ref SampleCount);
                }
                SampleCount += Node.GetChildNodeCount;
            }
        }

        public Result GenerateLayoutTerrain()
        {
            var ReturnResult = new Result("Terrain heights", false);
            logger.Info("Generating Terrain heights");

            var NodeTag = default(clsNodeTag);
            var tmpNodeA = default(PathfinderNode);
            var tmpNodeB = default(PathfinderNode);
            var A = 0;
            var B = 0;
            var C = 0;
            var D = 0;
            var X = 0;
            var Y = 0;
            var XY_int = new XYInt();
            double Dist = 0;
            double BestDist = 0;
            var Flag = default(bool);

            Map = new Map(TileSize);
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
                    NodeTag.Pos = new XYInt(X * 128, Y * 128);
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

            var BaseLayer = VertexPathMap.get_GetNodeLayer(0);
            var JitterLayer = VertexPathMap.get_GetNodeLayer(JitterScale);
            A = JitterLayer.GetNodeCount - 1;
            var NodeLevel = new int[A + 1];
            var BaseNodeLevel = new clsBaseNodeLevels();
            BaseNodeLevel.NodeLevels = new float[BaseLayer.GetNodeCount];

            //set position of jitter layer nodes

            var XY_dbl = default(XYDouble);

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
                    NodeTag.Pos.X = (XY_dbl.X / C).ToInt();
                    NodeTag.Pos.Y = (XY_dbl.Y / C).ToInt();
                    tmpNodeA.Tag = NodeTag;
                }
            }

            //set node heights

            var BestConnection = default(clsConnection);
            var BestNode = default(clsPassageNode);

            for ( A = 0; A <= JitterLayer.GetNodeCount - 1; A++ )
            {
                NodeTag = (clsNodeTag)(JitterLayer.get_GetNode(A).Tag);
                NodeLevel[A] = -1;
                BestDist = float.MaxValue;
                BestConnection = null;
                BestNode = null;
                for ( B = 0; B <= connectionCount - 1; B++ )
                {
                    //If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) Then
                    if ( connections[B].PassageNodeA.Level == connections[B].PassageNodeB.Level )
                    {
                        //only do this if the levels are the same
                        //this is to make sure nodes that are connected are actually connected on the terrain
                        XY_int = MathUtil.PointGetClosestPosOnLine(connections[B].PassageNodeA.Pos, connections[B].PassageNodeB.Pos, NodeTag.Pos);
                        Dist = Convert.ToSingle((XY_int - NodeTag.Pos).ToDoubles().GetMagnitude());
                        if ( Dist < BestDist )
                        {
                            BestDist = Dist;
                            if ( (NodeTag.Pos - connections[B].PassageNodeA.Pos).ToDoubles().GetMagnitude() <=
                                 (NodeTag.Pos - connections[B].PassageNodeB.Pos).ToDoubles().GetMagnitude() )
                            {
                                BestNode = connections[B].PassageNodeA;
                            }
                            else
                            {
                                BestNode = connections[B].PassageNodeB;
                            }
                            Flag = true;
                        }
                    }
                }
                for ( C = 0; C <= passageNodeCount - 1; C++ )
                {
                    //If Not PassageNodesA(C).IsOnBorder Then
                    for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                    {
                        Dist = Convert.ToSingle((NodeTag.Pos - passageNodes[D, C].Pos).ToDoubles().GetMagnitude());
                        if ( Dist < BestDist )
                        {
                            BestDist = Dist;
                            BestNode = passageNodes[D, C];
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

            var MinRampLength = (LevelHeight * Map.HeightMultiplier * 2.0D).ToInt() + 128;
            var RampArgs = new clsSetBaseLevelRampArgs();
            RampArgs.BaseLevel = BaseNodeLevel;
            RampArgs.RampRadius = 320.0F;
            for ( B = 0; B <= connectionCount - 1; B++ )
            {
                RampArgs.Connection = connections[B];
                RampArgs.RampLength =
                    Math.Max(Convert.ToInt32((connections[B].PassageNodeA.Pos - connections[B].PassageNodeB.Pos).ToDoubles().GetMagnitude() * 0.75D),
                        MinRampLength * Math.Abs(connections[B].PassageNodeA.Level - connections[B].PassageNodeB.Level));
                for ( A = 0; A <= JitterLayer.GetNodeCount - 1; A++ )
                {
                    if ( connections[B].IsRamp )
                    {
                        NodeTag = (clsNodeTag)(JitterLayer.get_GetNode(A).Tag);
                        XY_int = MathUtil.PointGetClosestPosOnLine(connections[B].PassageNodeA.Pos, connections[B].PassageNodeB.Pos, NodeTag.Pos);
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
                Map.Terrain.Vertices[(NodeTag.Pos.X / 128.0F).ToInt(), (NodeTag.Pos.Y / 128.0F).ToInt()].Height = (byte)(BaseNodeLevel.NodeLevels[A] * LevelHeight);
            }

            return ReturnResult;
        }

        public void GenerateTilePathMap()
        {
            var NodeTag = default(clsNodeTag);
            var tmpNodeA = default(PathfinderNode);
            var tmpNodeB = default(PathfinderNode);
            var X = 0;
            var Y = 0;

            TilePathMap = new PathfinderNetwork();

            for ( Y = 0; Y <= Map.Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X - 1; X++ )
                {
                    GenerateTerrainTiles[X, Y] = new GenerateTerrainTile();
                    GenerateTerrainTiles[X, Y].Node = new PathfinderNode(TilePathMap);
                    NodeTag = new clsNodeTag();
                    NodeTag.Pos = new XYInt(((X + 0.5D) * 128.0D).ToInt(), ((Y + 0.5D) * 128.0D).ToInt());
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

        private void PassageNodesMinLevelSet(clsPassageNode PassageNode, clsPassageNodeLevels PassageNodesMinLevel, int Level, int LevelChange)
        {
            var A = 0;
            var tmpPassageNode = default(clsPassageNode);

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
            var A = 0;
            var tmpPassageNode = default(clsPassageNode);

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

        private void UpdateNodeConnectedness(clsUpdateNodeConnectednessArgs Args, clsPassageNode PassageNode)
        {
            var A = 0;
            var tmpConnection = default(clsConnection);
            var tmpOtherNode = default(clsPassageNode);
            var PassableCount = 0;

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
            var StartNodes = new PathfinderNode[1];
            StartNodes[0] = Args.Args.PassageNodePathNodes[0, Args.OriginalNode.Num];
            Paths = Args.Args.PassageNodePathMap.GetPath(StartNodes, Args.Args.PassageNodePathNodes[PassageNode.MirrorNum, PassageNode.Num], -1, 0);
            Args.Args.NodeConnectedness[Args.OriginalNode.Num] += (float)(PassableCount * Math.Pow(0.999D, Paths[0].Paths[0].Value));
        }

        private void UpdateNetworkConnectedness(clsUpdateNetworkConnectednessArgs Args, clsPassageNode PassageNode)
        {
            var A = 0;
            var tmpConnection = default(clsConnection);
            var tmpOtherNode = default(clsPassageNode);
            var NodeConnectednessArgs = new clsUpdateNodeConnectednessArgs();
            var B = 0;
            var C = 0;

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
                        for ( B = 0; B <= passageNodeCount - 1; B++ )
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

        private void OilBalanceLoop(clsOilBalanceLoopArgs Args, int LoopNum)
        {
            var A = 0;
            var C = 0;
            var NextLoopNum = LoopNum + 1;
            var tmpPassageNodeA = default(clsPassageNode);

            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                tmpPassageNodeA = passageNodes[0, A];
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
            var NewPossibility = new clsOilPossibilities.clsPossibility();
            var BaseOilScore = new double[TopLeftPlayerCount];

            NewPossibility.PlayerOilScoreAddition = new double[TopLeftPlayerCount];

            var NewOilNum = 0;
            var OtherOilNum = 0;
            var NewOilNodeNum = 0;
            var OtherOilNodeNum = 0;
            var SymmetryBlockNum = 0;
            var MapNodeNum = 0;
            var PlayerNum = 0;
            //Dim NewOilCount As Integer
            double OilMassMultiplier = 0;
            double OilDistValue = 0;
            var NearestOilValue = double.MaxValue;

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
                    OilDistValue = 4.0D * passageNodeDists[0, NewOilNodeNum, 0, OtherOilNodeNum];
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
                        OilDistValue = 4.0D * passageNodeDists[0, NewOilNodeNum, SymmetryBlockNum, OtherOilNodeNum];
                        // + GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, OtherOilNodeNum).Pos))
                        if ( OilDistValue < NearestOilValue )
                        {
                            NearestOilValue = OilDistValue;
                        }
                    }
                }
                //oil on the map
                for ( MapNodeNum = 0; MapNodeNum <= passageNodeCount - 1; MapNodeNum++ )
                {
                    for ( SymmetryBlockNum = 0; SymmetryBlockNum <= SymmetryBlockCount - 1; SymmetryBlockNum++ )
                    {
                        if ( passageNodes[SymmetryBlockNum, MapNodeNum].OilCount > 0 )
                        {
                            //OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * PassageNodes(SymmetryBlockNum, MapNodeNum).OilCount
                            //OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, SymmetryBlockNum, MapNodeNum)
                            //OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, MapNodeNum).Pos)
                            OilDistValue = 4.0D * OilMassMultiplier / passageNodeDists[0, NewOilNodeNum, SymmetryBlockNum, MapNodeNum];
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
                            Convert.ToDouble(passageNodeDists[0, playerBases[PlayerNum].Nodes[0].Num, SymmetryBlockNum, NewOilNodeNum] * 2.0D +
                                             (playerBases[PlayerNum].Nodes[0].Pos - passageNodes[SymmetryBlockNum, NewOilNodeNum].Pos).ToDoubles().GetMagnitude());
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

        private void SetBaseLevel(PathfinderNode Node, int NewLevel, clsBaseNodeLevels BaseLevel)
        {
            if ( Node.GetChildNodeCount == 0 )
            {
                var A = 0;
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
                var A = 0;
                for ( A = 0; A <= Node.GetChildNodeCount - 1; A++ )
                {
                    SetBaseLevel(Node.get_GetChildNode(A), NewLevel, BaseLevel);
                }
            }
        }

        private void SetBaseLevelRamp(clsSetBaseLevelRampArgs Args, PathfinderNode Node)
        {
            if ( Node.GetChildNodeCount == 0 )
            {
                var NodeTag = (clsNodeTag)Node.Tag;
                var XY_int = MathUtil.PointGetClosestPosOnLine(Args.Connection.PassageNodeA.Pos, Args.Connection.PassageNodeB.Pos, NodeTag.Pos);
                var ConnectionLength = Convert.ToSingle((Args.Connection.PassageNodeA.Pos - Args.Connection.PassageNodeB.Pos).ToDoubles().GetMagnitude());
                var Extra = ConnectionLength - Args.RampLength;
                var ConnectionPos = Convert.ToSingle((XY_int - Args.Connection.PassageNodeA.Pos).ToDoubles().GetMagnitude());
                var RampPos = MathUtil.ClampSng((ConnectionPos - Extra / 2.0F) / Args.RampLength, 0.0F, 1.0F);
                var Layer_NodeNum = Node.GetLayer_NodeNum;
                RampPos = (float)(1.0D - (Math.Cos(RampPos * Math.PI) + 1.0D) / 2.0D);
                if ( RampPos > 0.0F & RampPos < 1.0F )
                {
                    var Dist2 = Convert.ToSingle((NodeTag.Pos - XY_int).ToDoubles().GetMagnitude());
                    if ( Dist2 < Args.RampRadius )
                    {
                        var Dist2Factor = 1.0F; //Math.Min(3.0F - 3.0F * Dist2 / 384.0F, 1.0F) 'distance fading
                        if ( Args.BaseLevel.NodeLevels[Layer_NodeNum] == (Args.BaseLevel.NodeLevels[Layer_NodeNum]) )
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
                var A = 0;
                for ( A = 0; A <= Node.GetChildNodeCount - 1; A++ )
                {
                    SetBaseLevelRamp(Args, Node.get_GetChildNode(A));
                }
            }
        }

        public void TerrainBlockPaths()
        {
            var X = 0;
            var Y = 0;

            for ( Y = 0; Y <= Map.Terrain.TileSize.Y - 1; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X - 1; X++ )
                {
                    if ( Map.Terrain.Tiles[X, Y].Texture.TextureNum >= 0 )
                    {
                        if ( GenerateTileset.Tileset.Tiles[Map.Terrain.Tiles[X, Y].Texture.TextureNum].DefaultType == Constants.TileTypeNumCliff ||
                             GenerateTileset.Tileset.Tiles[Map.Terrain.Tiles[X, Y].Texture.TextureNum].DefaultType == Constants.TileTypeNumWater )
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
            var ReturnResult = new BooleanMap();
            float BestDist = 0;
            var BestIsWater = default(bool);
            var Pos = new XYInt();
            float Dist = 0;
            var B = 0;
            var C = 0;
            var XY_int = new XYInt();
            var X = 0;
            var Y = 0;

            ReturnResult.Blank(Map.Terrain.TileSize.X + 1, Map.Terrain.TileSize.Y + 1);
            for ( Y = 0; Y <= Map.Terrain.TileSize.Y; Y++ )
            {
                for ( X = 0; X <= Map.Terrain.TileSize.X; X++ )
                {
                    BestDist = float.MaxValue;
                    Pos = new XYInt(X * Constants.TerrainGridSpacing, Y * Constants.TerrainGridSpacing);
                    for ( B = 0; B <= connectionCount - 1; B++ )
                    {
                        //If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) Then
                        if ( connections[B].PassageNodeA.IsWater == connections[B].PassageNodeB.IsWater )
                        {
                            //only do this if the waters are the same
                            //this is to make sure nodes that are connected are actually connected as water
                            XY_int = MathUtil.PointGetClosestPosOnLine(connections[B].PassageNodeA.Pos, connections[B].PassageNodeB.Pos, Pos);
                            Dist = Convert.ToSingle((XY_int - Pos).ToDoubles().GetMagnitude());
                            if ( Dist < BestDist )
                            {
                                BestDist = Dist;
                                if ( (Pos - connections[B].PassageNodeA.Pos).ToDoubles().GetMagnitude() <=
                                     (Pos - connections[B].PassageNodeB.Pos).ToDoubles().GetMagnitude() )
                                {
                                    BestIsWater = connections[B].PassageNodeA.IsWater;
                                }
                                else
                                {
                                    BestIsWater = connections[B].PassageNodeB.IsWater;
                                }
                            }
                        }
                    }
                    for ( C = 0; C <= passageNodeCount - 1; C++ )
                    {
                        for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                        {
                            Dist = Convert.ToSingle((Pos - passageNodes[B, C].Pos).ToDoubles().GetMagnitude());
                            if ( Dist < BestDist )
                            {
                                BestDist = Dist;
                                BestIsWater = passageNodes[B, C].IsWater;
                            }
                        }
                    }
                    ReturnResult.ValueData.Value[Y, X] = BestIsWater;
                }
            }
            return ReturnResult;
        }

        public PathfinderNode GetNearestNode(PathfinderNetwork Network, XYInt Pos, int MinClearance)
        {
            var A = 0;
            double Dist = 0;
            var tmpNode = default(PathfinderNode);
            var BestNode = default(PathfinderNode);
            double BestDist = 0;
            var tmpNodeTag = default(clsNodeTag);

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

        private PathfinderNode GetNearestNodeConnection(PathfinderNetwork Network, XYInt Pos, int MinClearance, float MaxDistance)
        {
            var A = 0;
            var TravelNodes = new PathfinderNode[Network.get_GetNodeLayer(0).GetNodeCount * 10];
            var TravelNodeCount = 0;
            var NodeTravelDists = new float[Network.get_GetNodeLayer(0).GetNodeCount];
            var TravelNodeNum = 0;
            var CurrentNode = default(PathfinderNode);
            var OtherNode = default(PathfinderNode);
            var tmpConnection = default(PathfinderConnection);
            PathfinderNode BestNode = null;
            float TravelDist = 0;
            var Flag = default(bool);

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

        public Unit PlaceUnitNear(UnitTypeBase TypeBase, XYInt Pos, clsUnitGroup UnitGroup, int Clearance, int Rotation, int MaxDistFromPos)
        {
            var PosNode = default(PathfinderNode);
            var NodeTag = default(clsNodeTag);
            var FinalTilePos = new XYInt();
            var TilePosA = new XYInt();
            var TilePosB = new XYInt();
            var X2 = 0;
            var Y2 = 0;
            var Remainder = 0;
            var Footprint = new XYInt();

            PosNode = GetNearestNodeConnection(TilePathMap, Pos, Clearance, MaxDistFromPos);
            if ( PosNode != null )
            {
                NodeTag = (clsNodeTag)PosNode.Tag;
                if ( (Pos - NodeTag.Pos).ToDoubles().GetMagnitude() <= MaxDistFromPos )
                {
                    var NewUnitAdd = new clsUnitAdd();
                    NewUnitAdd.Map = Map;
                    NewUnitAdd.StoreChange = true;
                    var NewUnit = new Unit();
                    NewUnitAdd.NewUnit = NewUnit;
                    NewUnit.TypeBase = TypeBase;
                    NewUnit.UnitGroup = UnitGroup;

                    FinalTilePos.X = NodeTag.Pos.X / Constants.TerrainGridSpacing;
                    FinalTilePos.Y = (NodeTag.Pos.Y / Constants.TerrainGridSpacing);
                    Footprint = TypeBase.GetGetFootprintSelected(Rotation);
                    Remainder = Footprint.X % 2;
                    if ( Remainder > 0 )
                    {
                        NewUnit.Pos.Horizontal.X = NodeTag.Pos.X;
                    }
                    else
                    {
                        if ( App.Random.Next() >= 0.5F )
                        {
                            NewUnit.Pos.Horizontal.X = (NodeTag.Pos.X - Constants.TerrainGridSpacing / 2.0D).ToInt();
                        }
                        else
                        {
                            NewUnit.Pos.Horizontal.X = (NodeTag.Pos.X + Constants.TerrainGridSpacing / 2.0D).ToInt();
                        }
                    }
                    Remainder = Footprint.Y % 2;
                    if ( Remainder > 0 )
                    {
                        NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y;
                    }
                    else
                    {
                        if ( App.Random.Next() >= 0.5F )
                        {
                            NewUnit.Pos.Horizontal.Y = (NodeTag.Pos.Y - Constants.TerrainGridSpacing / 2.0D).ToInt();
                        }
                        else
                        {
                            NewUnit.Pos.Horizontal.Y = (NodeTag.Pos.Y + Constants.TerrainGridSpacing / 2.0D).ToInt();
                        }
                    }
                    TilePosA.X = ((double)NewUnit.Pos.Horizontal.X / Constants.TerrainGridSpacing - Footprint.X / 2.0D + 0.5D).Floor().ToInt();
                    TilePosA.Y = ((double)NewUnit.Pos.Horizontal.Y / Constants.TerrainGridSpacing - Footprint.Y / 2.0D + 0.5D).Floor().ToInt();
                    TilePosB.X = (((double)NewUnit.Pos.Horizontal.X / Constants.TerrainGridSpacing + Footprint.X / 2.0D - 0.5D)).Floor().ToInt();
                    TilePosB.Y = (((double)NewUnit.Pos.Horizontal.Y / Constants.TerrainGridSpacing + Footprint.Y / 2.0D - 0.5D)).Floor().ToInt();
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
                return null;
            }
            return null;
        }

        public Unit PlaceUnit(UnitTypeBase TypeBase, WorldPos Pos, clsUnitGroup UnitGroup, int Rotation)
        {
            var TilePosA = new XYInt();
            var TilePosB = new XYInt();
            var X2 = 0;
            var Y2 = 0;
            var FinalTilePos = new XYInt();
            var Footprint = new XYInt();

            var NewUnitAdd = new clsUnitAdd();
            NewUnitAdd.Map = Map;
            NewUnitAdd.StoreChange = true;
            var NewUnit = new Unit();
            NewUnitAdd.NewUnit = NewUnit;
            NewUnit.TypeBase = TypeBase;
            NewUnit.UnitGroup = UnitGroup;

            FinalTilePos.X = Pos.Horizontal.X / Constants.TerrainGridSpacing;
            FinalTilePos.Y = Pos.Horizontal.Y / Constants.TerrainGridSpacing;

            Footprint = TypeBase.GetGetFootprintSelected(Rotation);

            NewUnit.Pos = Pos;
            TilePosA.X = ((double)NewUnit.Pos.Horizontal.X / Constants.TerrainGridSpacing - Footprint.X / 2.0D + 0.5D).Floor().ToInt();
            TilePosA.Y = ((double)NewUnit.Pos.Horizontal.Y / Constants.TerrainGridSpacing - Footprint.Y / 2.0D + 0.5D).Floor().ToInt();
            TilePosB.X = ((double)NewUnit.Pos.Horizontal.X / Constants.TerrainGridSpacing + Footprint.X / 2.0D - 0.5D).Floor().ToInt();
            TilePosB.Y = ((double)NewUnit.Pos.Horizontal.Y / Constants.TerrainGridSpacing + Footprint.Y / 2.0D - 0.5D).Floor().ToInt();
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
            var X2 = 0;
            var Y2 = 0;

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
            var X = 0;
            var Y = 0;
            var Terrain = Map.Terrain;

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

        public Result GenerateUnits()
        {
            var ReturnResult = new Result("Objects", false);
            logger.Info("Generating Objects");

            var A = 0;
            var B = 0;
            var C = 0;
            var D = 0;
            var tmpUnit = default(Unit);
            var Count = 0;
            var FeaturePlaceRange = 6 * 128;
            var BasePlaceRange = 16 * 128;
            var TilePos = new XYInt();
            byte AverageHeight = 0;
            var PlayerNum = 0;
            var Terrain = Map.Terrain;

            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    passageNodes[B, A].HasFeatureCluster = false;
                }
            }

            for ( A = 0; A <= totalPlayerCount - 1; A++ )
            {
                PlayerNum = A;
                tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseCommandCentre, playerBases[A].Pos, Map.UnitGroups[PlayerNum], 3, 0, BasePlaceRange);
                if ( tmpUnit == null )
                {
                    ReturnResult.ProblemAdd("No room for base structures");
                    return ReturnResult;
                }
                tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBasePowerGenerator, playerBases[A].Pos, Map.UnitGroups[PlayerNum], 3, 0, BasePlaceRange);
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
                    tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseResearchFacility, playerBases[A].Pos, Map.UnitGroups[PlayerNum], 3, 0, BasePlaceRange);
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
                    tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseFactory, playerBases[A].Pos, Map.UnitGroups[PlayerNum], 4, 0, BasePlaceRange);
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
                tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseCyborgFactory, playerBases[A].Pos, Map.UnitGroups[PlayerNum], 3, 0, BasePlaceRange);
                if ( tmpUnit == null )
                {
                    ReturnResult.ProblemAdd("No room for base structures");
                    return ReturnResult;
                }
                for ( B = 1; B <= BaseTruckCount; B++ )
                {
                    tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseTruck, playerBases[A].Pos, Map.UnitGroups[PlayerNum], 2, 0, BasePlaceRange);
                    if ( tmpUnit == null )
                    {
                        ReturnResult.ProblemAdd("No room for trucks");
                        return ReturnResult;
                    }
                }
            }
            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                {
                    for ( B = 0; B <= passageNodes[D, A].OilCount - 1; B++ )
                    {
                        if ( passageNodes[D, A].PlayerBaseNum >= 0 )
                        {
                            tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseOilResource, passageNodes[D, A].Pos, Map.ScavengerUnitGroup, 2, 0, BasePlaceRange);
                        }
                        else
                        {
                            tmpUnit = PlaceUnitNear(DefaultGenerator.UnitTypeBaseOilResource, passageNodes[D, A].Pos, Map.ScavengerUnitGroup, 2, 0, FeaturePlaceRange);
                        }
                        if ( tmpUnit == null )
                        {
                            ReturnResult.ProblemAdd("No room for oil.");
                            return ReturnResult;
                        }
                        //flatten ground underneath
                        TilePos.X = (tmpUnit.Pos.Horizontal.X / Constants.TerrainGridSpacing);
                        TilePos.Y = tmpUnit.Pos.Horizontal.Y / Constants.TerrainGridSpacing;
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
                        if ( passageNodes[D, A].PlayerBaseNum >= 0 )
                        {
                            //place base derrick
                            tmpUnit = PlaceUnit(DefaultGenerator.UnitTypeBaseDerrick, tmpUnit.Pos, Map.UnitGroups[passageNodes[D, A].PlayerBaseNum], 0);
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
            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                {
                    if ( passageNodes[D, A].PlayerBaseNum < 0 && !passageNodes[D, A].IsOnBorder )
                    {
                        passageNodes[D, A].HasFeatureCluster = App.Random.Next() < FeatureClusterChance;
                    }
                }
            }

            UInt32 RandNum = 0;
            UInt32 uintTemp = 0;
            var tmpNode = default(PathfinderNode);
            var E = 0;
            var Footprint = new XYInt();
            var MissingUnitCount = 0;
            var Rotation = 0;

            if ( GenerateTileset.ClusteredUnitChanceTotal > 0 )
            {
                for ( A = 0; A <= passageNodeCount - 1; A++ )
                {
                    for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                    {
                        if ( passageNodes[D, A].HasFeatureCluster )
                        {
                            Count = FeatureClusterMinUnits +
                                    Convert.ToInt32((App.Random.Next() * (FeatureClusterMaxUnits - FeatureClusterMinUnits + 1)));
                            for ( B = 1; B <= Count; B++ )
                            {
                                RandNum = (uint)((App.Random.Next() * GenerateTileset.ClusteredUnitChanceTotal));
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
                                Footprint = GenerateTileset.ClusteredUnits[C].TypeBase.GetGetFootprintSelected(Rotation);
                                E = Math.Ceiling(Math.Max(Footprint.X, Footprint.Y) / 2.0F).ToInt() + 1;
                                tmpUnit = PlaceUnitNear(GenerateTileset.ClusteredUnits[C].TypeBase, passageNodes[D, A].Pos, Map.ScavengerUnitGroup, E, Rotation,
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
                    RandNum = (uint)((App.Random.Next() * GenerateTileset.ScatteredUnitChanceTotal));
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
                    Footprint = GenerateTileset.ScatteredUnits[C].TypeBase.GetGetFootprintSelected(Rotation);
                    B = FeatureScatterGap + Math.Ceiling(Math.Max(Footprint.X, Footprint.Y) / 2.0F).ToInt();
                    tmpNode = GetRandomChildNode(TilePathMap.get_GetNodeLayer(TilePathMap.GetNodeLayerCount - 1).get_GetNode(0), B);
                    if ( tmpNode == null )
                    {
                        break;
                    }
                    var NodeTag = (clsNodeTag)tmpNode.Tag;
                    if ( PlaceUnitNear(GenerateTileset.ScatteredUnits[C].TypeBase, NodeTag.Pos, Map.ScavengerUnitGroup, B, Rotation, FeaturePlaceRange) == null )
                    {
                        break;
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
            var A = 0;
            do
            {
                A = Convert.ToInt32((App.Random.Next() * InputNode.GetChildNodeCount));
            } while ( InputNode.get_GetChildNode(A).GetClearance < MinClearance );

            var ReturnResult = GetRandomChildNode(InputNode.get_GetChildNode(A), MinClearance);
            return ReturnResult;
        }

        public SimpleResult GenerateGateways()
        {
            var ReturnResult = new SimpleResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            //must be done before units otherwise the units will be treated as gateway obstacles

            var Terrain = Map.Terrain;

            var X = 0;
            var SpaceCount = 0;
            var Y = 0;
            var PossibleGateways = new sPossibleGateway[Terrain.TileSize.X * Terrain.TileSize.Y * 2];
            var PossibleGatewayCount = 0;

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

            var A = 0;
            float Value = 0;
            float BestValue = 0;
            var BestNum = 0;
            var TileIsGateway = new bool[Terrain.TileSize.X, Terrain.TileSize.Y];
            var Valid = default(bool);
            var InvalidPos = new XYInt();
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
                        new XYInt(PossibleGateways[BestNum].StartPos.X, PossibleGateways[BestNum].StartPos.Y + PossibleGateways[BestNum].Length - 1));
                    for ( Y = PossibleGateways[BestNum].StartPos.Y; Y <= PossibleGateways[BestNum].StartPos.Y + PossibleGateways[BestNum].Length - 1; Y++ )
                    {
                        TileIsGateway[PossibleGateways[BestNum].StartPos.X, Y] = true;
                    }
                }
                else
                {
                    Map.GatewayCreateStoreChange(PossibleGateways[BestNum].StartPos,
                        new XYInt(PossibleGateways[BestNum].StartPos.X + PossibleGateways[BestNum].Length - 1, PossibleGateways[BestNum].StartPos.Y));
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
            var A = 0;
            var B = 0;

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

            for ( A = 0; A <= connectionCount - 1; A++ )
            {
                connections[A].PassageNodeA = null;
                connections[A].PassageNodeB = null;
                connections[A].Reflections = null;
            }
            connectionCount = 0;

            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    passageNodes[B, A].Connections = null;
                }
            }
            passageNodeCount = 0;

            nearestCount = 0;
        }

        private bool MakePassageNodes(XYInt Pos, bool IsOnBorder)
        {
            var A = 0;
            var B = 0;
            var tmpNode = default(clsPassageNode);
            var RotatedPos = new XYInt();
            var SymmetrySize = new XYInt();
            var Positions = new XYInt[4];
            var Limits = new XYInt();

            SymmetrySize.X = TileSize.X * Constants.TerrainGridSpacing / SymmetryBlockCountXY.X;
            SymmetrySize.Y = TileSize.Y * Constants.TerrainGridSpacing / SymmetryBlockCountXY.Y;

            Limits.X = SymmetrySize.X - 1;
            Limits.Y = SymmetrySize.Y - 1;

            for ( A = 0; A <= SymmetryBlockCount - 1; A++ )
            {
                RotatedPos = TileUtil.GetRotatedPos(SymmetryBlocks[A].Orientation, Pos, Limits);
                Positions[A].X = SymmetryBlocks[A].XYNum.X * SymmetrySize.X + RotatedPos.X;
                Positions[A].Y = SymmetryBlocks[A].XYNum.Y * SymmetrySize.Y + RotatedPos.Y;
                for ( B = 0; B <= A - 1; B++ )
                {
                    if ( (Positions[A] - Positions[B]).ToDoubles().GetMagnitude() < NodeScale * Constants.TerrainGridSpacing * 2.0D )
                    {
                        return false;
                    }
                }
            }

            for ( A = 0; A <= SymmetryBlockCount - 1; A++ )
            {
                tmpNode = new clsPassageNode();
                passageNodes[A, passageNodeCount] = tmpNode;
                tmpNode.Num = passageNodeCount;
                tmpNode.MirrorNum = A;
                tmpNode.Pos = Positions[A];
                tmpNode.IsOnBorder = IsOnBorder;
            }
            passageNodeCount++;

            return true;
        }

        private bool CheckRampAngles(clsConnection NewRampConnection, double MinSpacingAngle, double MinSpacingAngle2, double MinPassageSpacingAngle)
        {
            var XY_int = new XYInt();
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
            var ConnectionNum = 0;
            var tmpConnection = default(clsConnection);
            var OtherNode = default(clsPassageNode);
            var XY_int = new XYInt();
            double SpacingAngle = 0;
            var RampDifference = 0;

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
            var ConnectionNum = 0;

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
            var ConnectionNum = 0;
            var OtherPassageNode = default(clsPassageNode);
            var OtherNum = 0;
            var NarrowConnection = default(bool);
            var XY_int = new XYInt();

            for ( ConnectionNum = 0; ConnectionNum <= RampPassageNode.ConnectionCount - 1; ConnectionNum++ )
            {
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

        private SimpleResult PassageNodeHeightLevel(clsPassageNodeHeightLevelArgs Args)
        {
            var ReturnResult = new SimpleResult();
            ReturnResult.Problem = "";
            ReturnResult.Success = false;

            var LevelCounts = new int[LevelCount];
            var WaterCount = 0;
            var ConnectedToLevel = default(bool);
            var tmpPassageNodeB = default(clsPassageNode);
            var tmpPassageNodeC = default(clsPassageNode);
            var EligableCount = 0;
            var Eligables = new int[LevelCount];
            var NewHeightLevel = 0;
            var RandomAction = 0;
            var A = 0;
            var B = 0;

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
                NewHeightLevel = Eligables[App.Random.Next() * EligableCount];
            }
            else
            {
                RandomAction = Convert.ToInt32((App.Random.Next() * Args.ActionTotal));
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
                NewHeightLevel = Eligables[App.Random.Next() * EligableCount];
            }
            for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
            {
                passageNodes[B, Args.PassageNode.Num].Level = NewHeightLevel;
            }
            PassageNodesMinLevelSet(Args.PassageNode, Args.PassageNodesMinLevel, NewHeightLevel, MaxLevelTransition);
            PassageNodesMaxLevelSet(Args.PassageNode, Args.PassageNodesMaxLevel, NewHeightLevel, MaxLevelTransition);
            Args.MapLevelCount[NewHeightLevel]++;

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public Result GenerateRamps()
        {
            var ReturnResult = new Result("Ramps", false);
            logger.Info("Generating Ramps");

            var A = 0;
            var B = 0;
            var C = 0;
            var E = 0;
            double BestDist = 0;
            var BestNum = 0;
            var XY_int = new XYInt();
            double Dist = 0;

            //make ramps

            for ( A = 0; A <= connectionCount - 1; A++ )
            {
                connections[A].IsRamp = false;
            }

            PathfinderNode[,] PassageNodePathNodes = null;

            var PassageNodeNetwork = MakePassageNodeNetwork();
            PassageNodePathNodes = PassageNodeNetwork.PassageNodePathNodes;

            var PossibleRamps = new clsConnection[connectionCount];
            var PossibleRampCount = 0;
            var GetPathStartNodes = new PathfinderNode[1];
            PathfinderNetwork.PathList[] ResultPaths = null;

            //ramp connections whose points are too far apart

            var ConnectionsCanRamp = new bool[connectionCount];

            for ( B = 0; B <= connectionCount - 1; B++ )
            {
                C = Math.Abs(connections[B].PassageNodeA.Level - connections[B].PassageNodeB.Level);
                if ( C == 1 )
                {
                    if ( !(connections[B].PassageNodeA.IsOnBorder || connections[B].PassageNodeB.IsOnBorder)
                         && connections[B].PassageNodeA.MirrorNum == 0
                         && connections[B].PassageNodeA.Num != connections[B].PassageNodeB.Num )
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

            var Connectedness = new clsNodeConnectedness();
            Connectedness.NodeConnectedness = new float[passageNodeCount];
            Connectedness.PassageNodeVisited = new bool[SymmetryBlockCount, passageNodeCount];
            Connectedness.PassageNodePathNodes = PassageNodePathNodes;
            Connectedness.PassageNodePathMap = PassageNodeNetwork.Network;

            double Value = 0;
            double BestDistB = 0;
            double BaseDist = 0;
            double RampDist = 0;
            var UpdateNodeConnectednessArgs = new clsUpdateNodeConnectednessArgs();
            var UpdateNetworkConnectednessArgs = new clsUpdateNetworkConnectednessArgs();

            UpdateNodeConnectednessArgs.Args = Connectedness;
            UpdateNetworkConnectednessArgs.Args = Connectedness;
            UpdateNetworkConnectednessArgs.PassageNodeUpdated = new bool[passageNodeCount];
            UpdateNetworkConnectednessArgs.SymmetryBlockCount = SymmetryBlockCount;

            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                Connectedness.NodeConnectedness[A] = 0.0F;
                for ( B = 0; B <= passageNodeCount - 1; B++ )
                {
                    for ( C = 0; C <= SymmetryBlockCount - 1; C++ )
                    {
                        Connectedness.PassageNodeVisited[C, B] = false;
                    }
                }
                UpdateNodeConnectednessArgs.OriginalNode = passageNodes[0, A];
                UpdateNodeConnectedness(UpdateNodeConnectednessArgs, passageNodes[0, A]);
            }

            do
            {
                BestNum = -1;
                BestDist = 1.0F; //for connections that can already reach the other side
                BestDistB = 0.0F; //for connections that cant
                PossibleRampCount = 0;
                for ( B = 0; B <= connectionCount - 1; B++ )
                {
                    if ( ConnectionsCanRamp[B] && !connections[B].IsRamp )
                    {
                        if ( CheckRampAngles(connections[B], Convert.ToDouble(80.0D * MathUtil.RadOf1Deg), Convert.ToDouble(120.0D * MathUtil.RadOf1Deg),
                            0.0D * MathUtil.RadOf1Deg) )
                        {
                            GetPathStartNodes[0] = PassageNodePathNodes[connections[B].PassageNodeA.MirrorNum, connections[B].PassageNodeA.Num];
                            ResultPaths = PassageNodeNetwork.Network.GetPath(GetPathStartNodes,
                                PassageNodePathNodes[connections[B].PassageNodeB.MirrorNum, connections[B].PassageNodeB.Num], -1, 0);
                            BaseDist = double.MaxValue;
                            XY_int.X = ((connections[B].PassageNodeA.Pos.X + connections[B].PassageNodeB.Pos.X) / 2.0D).ToInt();
                            XY_int.Y = ((connections[B].PassageNodeA.Pos.Y + connections[B].PassageNodeB.Pos.Y) / 2.0D).ToInt();
                            for ( E = 0; E <= totalPlayerCount - 1; E++ )
                            {
                                Dist = Convert.ToDouble((playerBases[E].Pos - XY_int).ToDoubles().GetMagnitude());
                                if ( Dist < BaseDist )
                                {
                                    BaseDist = Dist;
                                }
                            }
                            RampDist = Math.Max(MaxDisconnectionDist * Math.Pow(RampBase, (BaseDist / 1024.0D)), 1.0F);
                            if ( ResultPaths == null )
                            {
                                Value = Connectedness.NodeConnectedness[connections[B].PassageNodeA.Num] +
                                        Connectedness.NodeConnectedness[connections[B].PassageNodeB.Num];
                                if ( double.MaxValue > BestDist )
                                {
                                    BestDist = double.MaxValue;
                                    BestDistB = Value;
                                    PossibleRamps[0] = connections[B];
                                    PossibleRampCount = 1;
                                }
                                else
                                {
                                    if ( Value < BestDistB )
                                    {
                                        BestDistB = Value;
                                        PossibleRamps[0] = connections[B];
                                        PossibleRampCount = 1;
                                    }
                                    else if ( Value == BestDistB )
                                    {
                                        PossibleRamps[PossibleRampCount] = connections[B];
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
                                PossibleRamps[0] = connections[B];
                                PossibleRampCount = 1;
                            }
                            else if ( ResultPaths[0].Paths[0].Value / RampDist == BestDist )
                            {
                                PossibleRamps[PossibleRampCount] = connections[B];
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
                    BestNum = App.Random.Next() * PossibleRampCount;
                    PossibleRamps[BestNum].IsRamp = true;
                    for ( C = 0; C <= PossibleRamps[BestNum].ReflectionCount - 1; C++ )
                    {
                        PossibleRamps[BestNum].Reflections[C].IsRamp = true;
                    }
                    PassageNodeNetwork.Network.FindCalc();
                    for ( E = 0; E <= passageNodeCount - 1; E++ )
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

            var FloodArgs = new PathfinderNetwork.sFloodProximityArgs();
            FloodArgs.StartNode = PassageNodeNetwork.PassageNodePathNodes[0, 0];
            FloodArgs.NodeValues = PassageNodeNetwork.Network.NetworkLargeArrays.Nodes_ValuesA;
            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    FloodArgs.NodeValues[PassageNodeNetwork.PassageNodePathNodes[B, A].Layer_NodeNum] = float.MaxValue;
                }
            }
            PassageNodeNetwork.Network.FloodProximity(ref FloodArgs);
            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    if ( !passageNodes[B, A].IsWater )
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

        private clsPassageNodeNework MakePassageNodeNetwork()
        {
            var ReturnResult = new clsPassageNodeNework();
            var NodeTag = default(clsNodeTag);
            var A = 0;
            var B = 0;

            ReturnResult.Network = new PathfinderNetwork();
            ReturnResult.PassageNodePathNodes = new PathfinderNode[SymmetryBlockCount, passageNodeCount];
            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    ReturnResult.PassageNodePathNodes[B, A] = new PathfinderNode(ReturnResult.Network);
                    NodeTag = new clsNodeTag();
                    NodeTag.Pos = passageNodes[B, A].Pos;
                    ReturnResult.PassageNodePathNodes[B, A].Tag = NodeTag;
                }
            }
            ReturnResult.Network.LargeArraysResize();
            ReturnResult.Network.FindCalc();

            return ReturnResult;
        }

        public Result GenerateOil()
        {
            var ReturnResult = new Result("Oil", false);
            logger.Info("Generating Oil");

            var A = 0;
            var B = 0;
            var C = 0;
            var D = 0;

            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                for ( B = 0; B <= SymmetryBlockCount - 1; B++ )
                {
                    passageNodes[B, A].OilCount = 0;
                }
            }

            //store passage node route distances
            var PassageNodePathMap = MakePassageNodeNetwork();
            var GetPathStartNodes = new PathfinderNode[1];
            PathfinderNetwork.PathList[] ResultPaths = null;

            passageNodeDists = new float[SymmetryBlockCount, passageNodeCount, SymmetryBlockCount, passageNodeCount];
            for ( A = 0; A <= passageNodeCount - 1; A++ )
            {
                for ( D = 0; D <= SymmetryBlockCount - 1; D++ )
                {
                    passageNodeDists[D, A, D, A] = 0.0F;
                    for ( B = 0; B <= passageNodeCount - 1; B++ )
                    {
                        for ( C = 0; C <= SymmetryBlockCount - 1; C++ )
                        {
                            if ( passageNodes[0, A].IsWater || passageNodes[C, B].IsWater || (C != 0 & D != 0) )
                            {
                                passageNodeDists[D, A, C, B] = float.MaxValue;
                                passageNodeDists[C, B, D, A] = float.MaxValue;
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
                                if ( ResultPaths[0].PathCount != 1 )
                                {
                                    Debugger.Break();
                                }
                                passageNodeDists[D, A, C, B] = ResultPaths[0].Paths[0].Value;
                                passageNodeDists[C, B, D, A] = ResultPaths[0].Paths[0].Value;
                            }
                        }
                    }
                }
            }

            PassageNodePathMap.Network.Deallocate();

            //place oil
            var PlacedExtraOilCount = 0;
            var MaxBestNodeCount = 0;
            MaxBestNodeCount = 1;
            for ( A = 0; A <= OilAtATime - 1; A++ )
            {
                MaxBestNodeCount *= passageNodeCount;
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
                        Math.Min(ExtraOilClusterSizeMin + App.Random.Next() * (ExtraOilClusterSizeMax - ExtraOilClusterSizeMin + 1),
                            Math.Max(Math.Ceiling(Convert.ToDecimal((ExtraOilCount - PlacedExtraOilCount) / SymmetryBlockCount)).ToInt(), 1));
                }
                oilArgs.OilPossibilities = new clsOilPossibilities();
                OilBalanceLoop(oilArgs, 0);

                var bestPossibility = oilArgs.OilPossibilities.BestPossibility;

                if ( bestPossibility != null )
                {
                    for ( B = 0; B <= OilAtATime - 1; B++ )
                    {
                        for ( A = 0; A <= SymmetryBlockCount - 1; A++ )
                        {
                            passageNodes[A, bestPossibility.Nodes[B].Num].OilCount += oilArgs.OilClusterSizes[B];
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
                    passageNodes[B, playerBases[A].Nodes[0].Num].OilCount += BaseOilCount;
                }
            }

            return ReturnResult;
        }

        public struct GenerateTerrainTile
        {
            public PathfinderConnection BottomLeftLink;
            public PathfinderConnection BottomLink;
            public PathfinderConnection BottomRightLink;
            public PathfinderConnection LeftLink;
            public PathfinderNode Node;
            public PathfinderConnection RightLink;
            public PathfinderConnection TopLeftLink;
            public PathfinderConnection TopLink;
            public PathfinderConnection TopRightLink;
        }

        public struct GenerateTerrainVertex
        {
            public PathfinderConnection BottomLeftLink;
            public PathfinderConnection BottomLink;
            public PathfinderConnection BottomRightLink;
            public PathfinderConnection LeftLink;
            public PathfinderNode Node;
            public PathfinderConnection RightLink;
            public PathfinderConnection TopLeftLink;
            public PathfinderConnection TopLink;
            public PathfinderConnection TopRightLink;
        }

        private class clsBaseNodeLevels
        {
            public float[] NodeLevels;
        }

        public class clsConnection
        {
            public bool IsRamp;
            public clsPassageNode PassageNodeA;
            public int PassageNodeA_ConnectionNum = -1;
            public clsPassageNode PassageNodeB;
            public int PassageNodeB_ConnectionNum = -1;
            public int ReflectionCount;
            public clsConnection[] Reflections;

            public clsConnection(clsPassageNode NewPassageNodeA, clsPassageNode NewPassageNodeB)
            {
                var NewConnection = new clsPassageNode.sConnection();

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

        private class clsNearest
        {
            public int BlockedCount;
            public int BlockedNearestCount;
            public clsNearest[] BlockedNearests;
            public float Dist2;
            public bool Invalid;
            public clsPassageNode[] NodeA;
            public clsPassageNode[] NodeB;
            public int NodeCount;
            public int Num = -1;
        }

        private class clsNodeConnectedness
        {
            public float[] NodeConnectedness;
            public PathfinderNetwork PassageNodePathMap;
            public PathfinderNode[,] PassageNodePathNodes;
            public bool[,] PassageNodeVisited;
        }

        public class clsNodeTag
        {
            public XYInt Pos;
        }

        private class clsOilBalanceLoopArgs
        {
            public int[] OilClusterSizes;
            public clsPassageNode[] OilNodes;
            public clsOilPossibilities OilPossibilities;
            public double[] PlayerOilScore;
        }

        public class clsOilPossibilities
        {
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

            public class clsPossibility
            {
                public clsPassageNode[] Nodes;
                public double[] PlayerOilScoreAddition;
                public double Score;
            }
        }

        public class clsPassageNode
        {
            public int ConnectionCount;
            public sConnection[] Connections;
            public bool HasFeatureCluster;
            public bool IsNearBorder;
            public bool IsOnBorder;

            public bool IsWater;
            public int Level = -1;
            public int MirrorNum = -1;
            public int Num = -1;
            public int OilCount;

            public int PlayerBaseNum = -1;
            public XYInt Pos;

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
                var A = 0;

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
                var A = 0;
                var B = 0;
                var C = 0;
                var NewOrder = new sConnection[ConnectionCount];
                var AwayAngles = new double[ConnectionCount];
                var OtherNode = default(clsPassageNode);
                var XY_int = new XYInt(0, 0);
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
                var A = 0;

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
                    return Connection.PassageNodeB;
                }
            }
        }

        private class clsPassageNodeHeightLevelArgs
        {
            public readonly clsPassageNodeLevels PassageNodesMaxLevel = new clsPassageNodeLevels();
            public readonly clsPassageNodeLevels PassageNodesMinLevel = new clsPassageNodeLevels();
            public int ActionTotal;
            public int FlatsCutoff;
            public int[] MapLevelCount;
            public clsPassageNode PassageNode;
            public int PassagesCutoff;
            public int VariationCutoff;
        }

        private class clsPassageNodeLevels
        {
            public int[] Nodes;
        }

        private class clsPassageNodeNework
        {
            public PathfinderNetwork Network;
            public PathfinderNode[,] PassageNodePathNodes;
        }

        private class clsSetBaseLevelRampArgs
        {
            public clsBaseNodeLevels BaseLevel = new clsBaseNodeLevels();
            public clsConnection Connection;
            public int RampLength;
            public float RampRadius;
        }

        private class clsTestNearestArgs
        {
            public int MaxConDist2;
            public int MinConDist;
            public clsPassageNode PassageNodeA;
            public clsPassageNode PassageNodeB;
        }

        private class clsUpdateNetworkConnectednessArgs
        {
            public clsNodeConnectedness Args;
            public bool[] PassageNodeUpdated;
            public int SymmetryBlockCount;
        }

        private class clsUpdateNodeConnectednessArgs
        {
            public clsNodeConnectedness Args;
            public clsPassageNode OriginalNode;
        }

        private struct sPlayerBase
        {
            public int NodeCount;
            public clsPassageNode[] Nodes;
            public XYInt Pos;

            public void CalcPos()
            {
                var A = 0;
                var Total = default(XYDouble);

                for ( A = 0; A <= NodeCount - 1; A++ )
                {
                    Total.X += Nodes[A].Pos.X;
                    Total.Y += Nodes[A].Pos.Y;
                }
                Pos.X = (Total.X / NodeCount).ToInt();
                Pos.Y = (Total.Y / NodeCount).ToInt();
            }
        }

        private struct sPossibleGateway
        {
            public bool IsVertical;
            public int Length;
            public XYInt MiddlePos;
            public XYInt StartPos;
        }

        public struct sSymmetryBlock
        {
            public TileOrientation Orientation;
            public int[] ReflectToNum;
            public XYInt XYNum;
        }
    }
}