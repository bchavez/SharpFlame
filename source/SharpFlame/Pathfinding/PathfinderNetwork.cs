

using System;
using System.Diagnostics;


namespace SharpFlame.Pathfinding
{
    public class PathfinderNetwork
    {
        public int FindParentNodeCount;
        public PathfinderNode[] FindParentNodes = new PathfinderNode[0];

        public LargeArrays NetworkLargeArrays = new LargeArrays();
        public int NodeLayerCount;
        public PathfinderLayer[] NodeLayers = new PathfinderLayer[0];

        public int GetNodeLayerCount
        {
            get { return NodeLayerCount; }
        }

        public PathfinderLayer get_GetNodeLayer(int Num)
        {
            return NodeLayers[Num];
        }

        public void NodeLayer_Add(PathfinderLayer NewNodeLayer)
        {
            if ( NodeLayerCount > 0 )
            {
                NodeLayers[NodeLayerCount - 1].ParentLayer = NewNodeLayer;
            }

            Array.Resize(ref NodeLayers, NodeLayerCount + 1);
            NodeLayers[NodeLayerCount] = NewNodeLayer;
            NodeLayers[NodeLayerCount].Network_LayerNum = NodeLayerCount;
            NodeLayerCount++;
        }

        public void FindParentNode_Add(PathfinderNode NewFindParentNode)
        {
            if ( NewFindParentNode.Network_FindParentNum >= 0 )
            {
                return;
            }

            if ( FindParentNodes.GetUpperBound(0) < FindParentNodeCount )
            {
                Array.Resize(ref FindParentNodes, (FindParentNodeCount + 1) * 2);
            }
            FindParentNodes[FindParentNodeCount] = NewFindParentNode;
            FindParentNodes[FindParentNodeCount].Network_FindParentNum = FindParentNodeCount;
            FindParentNodeCount++;
        }

        public void FindParentNode_Remove(int Num)
        {
            FindParentNodes[Num].Network_FindParentNum = -1;
            FindParentNodes[Num] = null;

            FindParentNodeCount--;
            if ( Num < FindParentNodeCount )
            {
                FindParentNodes[Num] = FindParentNodes[FindParentNodeCount];
                FindParentNodes[Num].Network_FindParentNum = Num;
            }
            if ( FindParentNodeCount * 3 < FindParentNodes.GetUpperBound(0) )
            {
                Array.Resize(ref FindParentNodes, FindParentNodeCount * 2);
            }
        }

        public void Deallocate()
        {
            var A = 0;

            for ( A = 0; A <= NodeLayerCount - 1; A++ )
            {
                NodeLayers[A].ForceDeallocate();
            }

            NodeLayers = null;
            FindParentNodes = null;
        }

        public PathList[] GetPath(PathfinderNode[] StartNodes, PathfinderNode FinishNode, int Accuracy, int MinClearance)
        {
            var StartNodeCount = StartNodes.GetUpperBound(0) + 1;
            var Paths = new PathList[NodeLayerCount];
            var LayerStartNodes = new PathfinderNode[NodeLayerCount, StartNodeCount];
            var LayerFinishNodes = new PathfinderNode[NodeLayerCount];
            var LayerNum = 0;
            var Destinations = new PathfinderNode[24];
            var DestinationCount = 0;
            var FinishIsParent = default(bool);
            var CalcNodeCount = new int[24];
            var FloodRouteArgs = new sFloodRouteArgs();
            var FinalLayer = 0;
            var StartCanReach = new bool[StartNodeCount];
            var tmpNodeA = default(PathfinderNode);
            var tmpNodeB = default(PathfinderNode);
            var CanReachCount = 0;
            var FirstLayer = 0;
            var BestPaths = new Path[24];
            var BestValues = new float[24];
            var PathNum = 0;
            var StopMultiPathing = default(bool);
            var Visit = NetworkLargeArrays.Nodes_Booleans;
            var NodeValues = NetworkLargeArrays.Nodes_ValuesA;
            var Nodes_Nodes = NetworkLargeArrays.Nodes_Nodes;
            var StartPath = NetworkLargeArrays.Nodes_Path;
            var A = 0;
            var B = 0;
            var C = 0;
            var D = 0;
            var E = 0;

            FinalLayer = StartNodes[0].Layer.Network_LayerNum;
            LayerFinishNodes[FinalLayer] = FinishNode;
            B = FinalLayer;
            do
            {
                if ( LayerFinishNodes[B].ParentNode == null )
                {
                    FirstLayer = B;
                    break;
                }
                LayerFinishNodes[B + 1] = LayerFinishNodes[B].ParentNode;
                B++;
            } while ( true );
            for ( A = 0; A <= StartNodeCount - 1; A++ )
            {
                LayerStartNodes[FinalLayer, A] = StartNodes[A];
                B = FinalLayer;
                do
                {
                    if ( LayerStartNodes[B, A].ParentNode == null )
                    {
                        if ( LayerStartNodes[B, A] == LayerFinishNodes[B] )
                        {
                            StartCanReach[A] = true;
                            CanReachCount++;
                        }
                        break;
                    }
                    LayerStartNodes[B + 1, A] = LayerStartNodes[B, A].ParentNode;
                    B++;
                } while ( true );
            }
            if ( CanReachCount == 0 )
            {
                return null;
            }
            LayerNum = FirstLayer;
            Paths[LayerNum].Paths = new Path[0];
            Paths[LayerNum].Paths[0] = new Path();
            Paths[LayerNum].PathCount = 1;
            Paths[LayerNum].Paths[0].Nodes = new PathfinderNode[1];
            Paths[LayerNum].Paths[0].Nodes[0] = LayerFinishNodes[LayerNum];
            Paths[LayerNum].Paths[0].NodeCount = 1;
            var LastLayer = 0;
            do
            {
                LastLayer = LayerNum;
                LayerNum--;
                if ( LayerNum < FinalLayer )
                {
                    break;
                }
                if ( StopMultiPathing )
                {
                    if ( Accuracy < 0 )
                    {
                        Debugger.Break();
                    }
                    for ( PathNum = 0; PathNum <= Paths[LastLayer].PathCount - 1; PathNum++ )
                    {
                        CalcNodeCount[PathNum] = Math.Min(Accuracy, Convert.ToInt32(Paths[LastLayer].Paths[PathNum].NodeCount - 1));
                    }
                    Destinations[0] = Paths[LastLayer].Paths[0].Nodes[CalcNodeCount[0]];
                    DestinationCount = 1;
                    FinishIsParent = true;
                }
                else
                {
                    if ( Accuracy >= 0 )
                    {
                        for ( PathNum = 0; PathNum <= Paths[LastLayer].PathCount - 1; PathNum++ )
                        {
                            if ( Paths[LastLayer].Paths[PathNum].NodeCount > Accuracy )
                            {
                                StopMultiPathing = true;
                                break;
                            }
                        }
                    }
                    Destinations[0] = LayerFinishNodes[LayerNum];
                    if ( LayerNum == FinalLayer )
                    {
                        DestinationCount = 1;
                    }
                    else
                    {
                        for ( A = 0; A <= Destinations[0].ConnectionCount - 1; A++ )
                        {
                            Destinations[1 + A] = Destinations[0].Connections[A].GetOtherNode(Destinations[0]);
                        }
                        DestinationCount = 1 + Destinations[0].ConnectionCount;
                    }
                    for ( PathNum = 0; PathNum <= Paths[LastLayer].PathCount - 1; PathNum++ )
                    {
                        CalcNodeCount[PathNum] = Paths[LastLayer].Paths[PathNum].NodeCount - 1;
                    }
                    FinishIsParent = false;
                }
                for ( PathNum = 0; PathNum <= Paths[LastLayer].PathCount - 1; PathNum++ )
                {
                    for ( A = 0; A <= CalcNodeCount[PathNum]; A++ )
                    {
                        tmpNodeA = Paths[LastLayer].Paths[PathNum].Nodes[A];
                        for ( D = 0; D <= tmpNodeA.ConnectionCount - 1; D++ )
                        {
                            tmpNodeB = tmpNodeA.Connections[D].GetOtherNode(tmpNodeA);
                            for ( E = 0; E <= tmpNodeB.ConnectionCount - 1; E++ )
                            {
                                C = tmpNodeB.Connections[E].GetOtherNode(tmpNodeB).Layer_NodeNum;
                                Visit[C] = false;
                            }
                        }
                    }
                }
                for ( PathNum = 0; PathNum <= Paths[LastLayer].PathCount - 1; PathNum++ )
                {
                    for ( A = 0; A <= CalcNodeCount[PathNum]; A++ )
                    {
                        tmpNodeA = Paths[LastLayer].Paths[PathNum].Nodes[A];
                        C = tmpNodeA.Layer_NodeNum;
                        Visit[C] = true;
                        for ( E = 0; E <= tmpNodeA.NodeCount - 1; E++ )
                        {
                            C = tmpNodeA.Nodes[E].Layer_NodeNum;
                            NodeValues[C] = float.MaxValue;
                        }
                        for ( D = 0; D <= tmpNodeA.ConnectionCount - 1; D++ )
                        {
                            tmpNodeB = tmpNodeA.Connections[D].GetOtherNode(tmpNodeA);
                            C = tmpNodeB.Layer_NodeNum;
                            Visit[C] = true;
                            for ( E = 0; E <= tmpNodeB.NodeCount - 1; E++ )
                            {
                                C = tmpNodeB.Nodes[E].Layer_NodeNum;
                                NodeValues[C] = float.MaxValue;
                            }
                        }
                    }
                }
                FloodRouteArgs = new sFloodRouteArgs();
                FloodRouteArgs.CurrentPath = StartPath;
                FloodRouteArgs.FinishNodes = Destinations;
                FloodRouteArgs.FinishNodeCount = DestinationCount;
                FloodRouteArgs.FinishIsParent = FinishIsParent;
                FloodRouteArgs.Visit = Visit;
                FloodRouteArgs.NodeValues = NodeValues;
                FloodRouteArgs.SourceNodes = Nodes_Nodes;
                FloodRouteArgs.MinClearance = MinClearance;
                for ( A = 0; A <= DestinationCount - 1; A++ )
                {
                    BestPaths[A] = null;
                    BestValues[A] = float.MaxValue;
                }
                for ( A = 0; A <= StartNodeCount - 1; A++ )
                {
                    if ( StartCanReach[A] )
                    {
                        StartPath.NodeCount = 1;
                        StartPath.Nodes[0] = LayerStartNodes[LayerNum, A];
                        StartPath.Value = 0.0F;
                        FloodRouteArgs.BestPaths = new Path[DestinationCount];
                        FloodRoute(ref FloodRouteArgs);
                        for ( PathNum = 0; PathNum <= DestinationCount - 1; PathNum++ )
                        {
                            if ( FloodRouteArgs.BestPaths[PathNum] != null )
                            {
                                if ( FloodRouteArgs.BestPaths[PathNum].Value < BestValues[PathNum] )
                                {
                                    BestValues[PathNum] = FloodRouteArgs.BestPaths[PathNum].Value;
                                    BestPaths[PathNum] = FloodRouteArgs.BestPaths[PathNum];
                                }
                            }
                        }
                    }
                }
                Paths[LayerNum].Paths = new Path[DestinationCount];
                Paths[LayerNum].PathCount = 0;
                for ( PathNum = 0; PathNum <= DestinationCount - 1; PathNum++ )
                {
                    if ( BestPaths[PathNum] != null )
                    {
                        Paths[LayerNum].Paths[Paths[LayerNum].PathCount] = BestPaths[PathNum];
                        Paths[LayerNum].PathCount++;
                    }
                }
                Array.Resize(ref Paths[LayerNum].Paths, Paths[LayerNum].PathCount);
                if ( Paths[LayerNum].PathCount == 0 )
                {
                    return null;
                }
            } while ( true );
            return Paths;
        }

        public Path[] GetAllPaths(PathfinderNode[] StartNodes, PathfinderNode FinishNode, int MinClearance)
        {
            var StartNodeCount = StartNodes.GetUpperBound(0) + 1;
            var LayerStartNodes = new PathfinderNode[32, StartNodeCount];
            var LayerFinishNodes = new PathfinderNode[32];
            var LayerNum = 0;
            var FloodRouteDArgs = new sFloodRouteAllArgs();
            var FinalLayer = 0;
            var StartCanReach = new bool[StartNodeCount];
            var tmpNodeA = default(PathfinderNode);
            var CanReachCount = 0;
            var FirstLayer = 0;
            var SubPaths = new Path[32];
            var Nodes_Nodes = NetworkLargeArrays.Nodes_Nodes;
            var Visit = NetworkLargeArrays.Nodes_Booleans;
            var NodeValues = NetworkLargeArrays.Nodes_ValuesA;
            var NodeValuesB = NetworkLargeArrays.Nodes_ValuesB;
            var A = 0;
            var B = 0;
            var C = 0;
            var D = 0;

            FinalLayer = StartNodes[0].Layer.Network_LayerNum;
            LayerFinishNodes[FinalLayer] = FinishNode;
            B = FinalLayer;
            do
            {
                if ( LayerFinishNodes[B].ParentNode == null )
                {
                    FirstLayer = B;
                    break;
                }
                LayerFinishNodes[B + 1] = LayerFinishNodes[B].ParentNode;
                B++;
            } while ( true );
            for ( A = 0; A <= StartNodeCount - 1; A++ )
            {
                LayerStartNodes[FinalLayer, A] = StartNodes[A];
                B = FinalLayer;
                do
                {
                    if ( LayerStartNodes[B, A].ParentNode == null )
                    {
                        if ( LayerStartNodes[B, A] == LayerFinishNodes[B] )
                        {
                            StartCanReach[A] = true;
                            CanReachCount++;
                        }
                        break;
                    }
                    LayerStartNodes[B + 1, A] = LayerStartNodes[B, A].ParentNode;
                    B++;
                } while ( true );
            }
            if ( CanReachCount == 0 )
            {
                return null;
            }
            LayerNum = FirstLayer;
            SubPaths[LayerNum] = new Path();
            SubPaths[LayerNum].Nodes = new[] {LayerFinishNodes[LayerNum]};
            SubPaths[LayerNum].NodeCount = 1;
            var LastLayer = 0;
            do
            {
                LastLayer = LayerNum;
                LayerNum--;
                if ( LayerNum < FinalLayer )
                {
                    break;
                }
                for ( A = 0; A <= SubPaths[LastLayer].NodeCount - 1; A++ )
                {
                    tmpNodeA = SubPaths[LastLayer].Nodes[A];
                    for ( B = 0; B <= tmpNodeA.ConnectionCount - 1; B++ )
                    {
                        C = tmpNodeA.Connections[B].GetOtherNode(tmpNodeA).Layer_NodeNum;
                        Visit[C] = false;
                    }
                }
                for ( A = 0; A <= SubPaths[LastLayer].NodeCount - 1; A++ )
                {
                    tmpNodeA = SubPaths[LastLayer].Nodes[A];
                    C = tmpNodeA.Layer_NodeNum;
                    Visit[C] = true;
                    for ( D = 0; D <= tmpNodeA.NodeCount - 1; D++ )
                    {
                        C = tmpNodeA.Nodes[D].Layer_NodeNum;
                        NodeValues[C] = float.MaxValue;
                        NodeValuesB[C] = float.MaxValue;
                    }
                }
                FloodRouteDArgs = new sFloodRouteAllArgs();
                FloodRouteDArgs.FinishNode = LayerFinishNodes[LayerNum];
                FloodRouteDArgs.Visit = Visit;
                FloodRouteDArgs.NodeValuesA = NodeValues;
                FloodRouteDArgs.SourceNodes = Nodes_Nodes;
                FloodRouteDArgs.NodeValuesB = NodeValuesB;
                FloodRouteDArgs.MinClearance = MinClearance;
                FloodRouteDArgs.BestTolerance = (float)(Math.Pow(1.1D, LayerNum));
                FloodRouteDArgs.StartNodes = new PathfinderNode[StartNodeCount];
                for ( A = 0; A <= StartNodeCount - 1; A++ )
                {
                    if ( StartCanReach[A] )
                    {
                        FloodRouteDArgs.StartNodes[FloodRouteDArgs.StartNodeCount] = LayerStartNodes[LayerNum, A];
                        FloodRouteDArgs.StartNodeCount++;
                    }
                }
                Array.Resize(ref FloodRouteDArgs.StartNodes, FloodRouteDArgs.StartNodeCount);
                FloodRouteAll(ref FloodRouteDArgs);
                SubPaths[LayerNum] = new Path();
                SubPaths[LayerNum].Nodes = new PathfinderNode[FloodRouteDArgs.BestNodeCount];
                for ( A = 0; A <= FloodRouteDArgs.BestNodeCount - 1; A++ )
                {
                    SubPaths[LayerNum].Nodes[A] = FloodRouteDArgs.SourceNodes[A];
                }
                SubPaths[LayerNum].NodeCount = FloodRouteDArgs.BestNodeCount;
                if ( FloodRouteDArgs.BestNodeCount == 0 )
                {
                    Debugger.Break();
                    return SubPaths;
                }
            } while ( true );
            return SubPaths;
        }

        public void FloodRoute(ref sFloodRouteArgs Args)
        {
            var CurrentNode = default(PathfinderNode);
            var ConnectedNode = default(PathfinderNode);
            var A = 0;
            var SourceNodeCount = 0;
            var SourceNodeNum = 0;
            var tmpConnection = default(PathfinderConnection);
            float ResultValue = 0;
            var BestPath = default(Path);
            var StartNode = default(PathfinderNode);
            var B = 0;
            float BestDist = 0;
            float Dist = 0;
            var BestNode = default(PathfinderNode);
            var C = 0;

            StartNode = Args.CurrentPath.Nodes[0];

            Args.SourceNodes[0] = StartNode;
            SourceNodeCount = 1;
            Args.NodeValues[StartNode.Layer_NodeNum] = 0.0F;

            SourceNodeNum = 0;
            while ( SourceNodeNum < SourceNodeCount )
            {
                CurrentNode = Args.SourceNodes[SourceNodeNum];
                for ( A = 0; A <= CurrentNode.ConnectionCount - 1; A++ )
                {
                    tmpConnection = CurrentNode.Connections[A];
                    ConnectedNode = tmpConnection.GetOtherNode(CurrentNode);
                    if ( ConnectedNode.ParentNode != null )
                    {
                        if ( Args.Visit[ConnectedNode.ParentNode.Layer_NodeNum] )
                        {
                            if ( ConnectedNode.Clearance >= Args.MinClearance )
                            {
                                ResultValue = Args.NodeValues[CurrentNode.Layer_NodeNum] + tmpConnection.Value;
                                if ( ResultValue < Args.NodeValues[ConnectedNode.Layer_NodeNum] )
                                {
                                    Args.NodeValues[ConnectedNode.Layer_NodeNum] = ResultValue;
                                    Args.SourceNodes[SourceNodeCount] = ConnectedNode;
                                    SourceNodeCount++;
                                }
                            }
                        }
                    }
                }
                SourceNodeNum++;
            }

            for ( A = 0; A <= Args.FinishNodeCount - 1; A++ )
            {
                if ( Args.FinishIsParent )
                {
                    BestNode = null;
                    BestDist = float.MaxValue;
                    for ( C = 0; C <= Args.FinishNodes[A].NodeCount - 1; C++ )
                    {
                        CurrentNode = Args.FinishNodes[A].Nodes[C];
                        Dist = Args.NodeValues[CurrentNode.Layer_NodeNum];
                        if ( Dist < BestDist )
                        {
                            BestDist = Dist;
                            BestNode = CurrentNode;
                        }
                    }
                    CurrentNode = BestNode;
                }
                else
                {
                    CurrentNode = Args.FinishNodes[A];
                }
                if ( CurrentNode == null )
                {
                    //no path
                    return;
                }
                SourceNodeCount = 0;
                BestDist = Args.NodeValues[CurrentNode.Layer_NodeNum];
                if ( BestDist == float.MaxValue )
                {
                    //no path
                    return;
                }
                do
                {
                    Args.SourceNodes[SourceNodeCount] = CurrentNode;
                    SourceNodeCount++;
                    if ( CurrentNode == StartNode )
                    {
                        break;
                    }
                    BestNode = null;
                    for ( B = 0; B <= CurrentNode.ConnectionCount - 1; B++ )
                    {
                        tmpConnection = CurrentNode.Connections[B];
                        ConnectedNode = tmpConnection.GetOtherNode(CurrentNode);
                        if ( ConnectedNode.ParentNode != null )
                        {
                            if ( Args.Visit[ConnectedNode.ParentNode.Layer_NodeNum] )
                            {
                                Dist = Args.NodeValues[ConnectedNode.Layer_NodeNum];
                                if ( Dist < BestDist )
                                {
                                    BestDist = Dist;
                                    BestNode = ConnectedNode;
                                }
                            }
                        }
                    }
                    if ( BestNode == null )
                    {
                        Args.BestPaths[A] = null;
                        return;
                    }
                    CurrentNode = BestNode;
                } while ( true );

                BestPath = new Path();
                Args.BestPaths[A] = BestPath;
                BestPath.Value = Args.NodeValues[Args.FinishNodes[A].Layer_NodeNum];
                BestPath.NodeCount = SourceNodeCount;
                BestPath.Nodes = new PathfinderNode[BestPath.NodeCount];
                for ( B = 0; B <= BestPath.NodeCount - 1; B++ )
                {
                    BestPath.Nodes[B] = Args.SourceNodes[SourceNodeCount - B - 1];
                }
            }
        }

        public void FloodSpan(ref sFloodSpanArgs Args)
        {
            var CurrentNode = default(PathfinderNode);
            var ConnectedNode = default(PathfinderNode);
            var A = 0;
            var SourceNodeCount = 0;
            var SourceNodeNum = 0;
            var tmpConnection = default(PathfinderConnection);
            float ResultValue = 0;
            var BestPath = default(Path);
            var StartNode = default(PathfinderNode);
            var B = 0;
            float BestDist = 0;
            float Dist = 0;
            var BestNode = default(PathfinderNode);
            var C = 0;

            StartNode = Args.CurrentPath.Nodes[0];

            Args.SourceNodes[0] = StartNode;
            SourceNodeCount = 1;
            Args.NodeValues[StartNode.Layer_NodeNum] = 0.0F;

            SourceNodeNum = 0;
            while ( SourceNodeNum < SourceNodeCount )
            {
                CurrentNode = Args.SourceNodes[SourceNodeNum];
                for ( A = 0; A <= CurrentNode.ConnectionCount - 1; A++ )
                {
                    tmpConnection = CurrentNode.Connections[A];
                    ConnectedNode = tmpConnection.GetOtherNode(CurrentNode);
                    if ( ConnectedNode.ParentNode == Args.SourceParentNode )
                    {
                        ResultValue = Args.NodeValues[CurrentNode.Layer_NodeNum] + tmpConnection.Value;
                        if ( ResultValue < Args.NodeValues[ConnectedNode.Layer_NodeNum] )
                        {
                            Args.NodeValues[ConnectedNode.Layer_NodeNum] = ResultValue;
                            Args.SourceNodes[SourceNodeCount] = ConnectedNode;
                            SourceNodeCount++;
                        }
                    }
                }
                SourceNodeNum++;
            }

            for ( A = 0; A <= Args.FinishNodeCount - 1; A++ )
            {
                if ( Args.FinishIsParent )
                {
                    BestNode = null;
                    BestDist = float.MaxValue;
                    for ( C = 0; C <= Args.FinishNodes[A].NodeCount - 1; C++ )
                    {
                        CurrentNode = Args.FinishNodes[A].Nodes[C];
                        Dist = Args.NodeValues[CurrentNode.Layer_NodeNum];
                        if ( Dist < BestDist )
                        {
                            BestDist = Dist;
                            BestNode = CurrentNode;
                        }
                    }
                    CurrentNode = BestNode;
                }
                else
                {
                    CurrentNode = Args.FinishNodes[A];
                }
                if ( CurrentNode == null )
                {
                    //no path
                    return;
                }
                SourceNodeCount = 0;
                BestDist = Args.NodeValues[CurrentNode.Layer_NodeNum];
                if ( BestDist == float.MaxValue )
                {
                    //no path
                    return;
                }
                do
                {
                    Args.SourceNodes[SourceNodeCount] = CurrentNode;
                    SourceNodeCount++;
                    if ( CurrentNode == StartNode )
                    {
                        break;
                    }
                    BestNode = null;
                    for ( B = 0; B <= CurrentNode.ConnectionCount - 1; B++ )
                    {
                        tmpConnection = CurrentNode.Connections[B];
                        ConnectedNode = tmpConnection.GetOtherNode(CurrentNode);
                        if ( ConnectedNode.ParentNode == Args.SourceParentNode )
                        {
                            Dist = Args.NodeValues[ConnectedNode.Layer_NodeNum];
                            if ( Dist < BestDist )
                            {
                                BestDist = Dist;
                                BestNode = ConnectedNode;
                            }
                        }
                    }
                    if ( BestNode == null )
                    {
                        Args.BestPaths[A] = null;
                        //no path
                        return;
                    }
                    CurrentNode = BestNode;
                } while ( true );

                BestPath = new Path();
                Args.BestPaths[A] = BestPath;
                BestPath.Value = Args.NodeValues[Args.FinishNodes[A].Layer_NodeNum];
                BestPath.NodeCount = SourceNodeCount;
                BestPath.Nodes = new PathfinderNode[BestPath.NodeCount];
                for ( B = 0; B <= BestPath.NodeCount - 1; B++ )
                {
                    BestPath.Nodes[B] = Args.SourceNodes[SourceNodeCount - B - 1];
                }
            }
        }

        public void FloodForValues(ref sFloodForValuesArgs Args)
        {
            var CurrentNode = default(PathfinderNode);
            var ConnectedNode = default(PathfinderNode);
            var A = 0;
            var SourceNodeCount = 0;
            var SourceNodeNum = 0;
            var tmpConnection = default(PathfinderConnection);
            float ResultValue = 0;
            var BestPath = default(Path);
            var StartNode = default(PathfinderNode);
            var B = 0;
            float BestDist = 0;
            float Dist = 0;
            var BestNode = default(PathfinderNode);
            var C = 0;

            StartNode = Args.CurrentPath.Nodes[0];

            Args.SourceNodes[0] = StartNode;
            SourceNodeCount = 1;
            Args.NodeValues[StartNode.Layer_NodeNum] = 0.0F;

            SourceNodeNum = 0;
            while ( SourceNodeNum < SourceNodeCount )
            {
                CurrentNode = Args.SourceNodes[SourceNodeNum];
                for ( A = 0; A <= CurrentNode.ConnectionCount - 1; A++ )
                {
                    tmpConnection = CurrentNode.Connections[A];
                    ConnectedNode = tmpConnection.GetOtherNode(CurrentNode);
                    if ( ConnectedNode.ParentNode == Args.SourceParentNodeA || ConnectedNode.ParentNode == Args.SourceParentNodeB )
                    {
                        ResultValue = Args.NodeValues[CurrentNode.Layer_NodeNum] + tmpConnection.Value;
                        if ( ResultValue < Args.NodeValues[ConnectedNode.Layer_NodeNum] )
                        {
                            Args.NodeValues[ConnectedNode.Layer_NodeNum] = ResultValue;
                            Args.SourceNodes[SourceNodeCount] = ConnectedNode;
                            SourceNodeCount++;
                        }
                    }
                }
                SourceNodeNum++;
            }

            for ( A = 0; A <= Args.FinishNodeCount - 1; A++ )
            {
                if ( Args.FinishIsParent )
                {
                    BestNode = null;
                    BestDist = float.MaxValue;
                    for ( C = 0; C <= Args.FinishNodes[A].NodeCount - 1; C++ )
                    {
                        CurrentNode = Args.FinishNodes[A].Nodes[C];
                        Dist = Args.NodeValues[CurrentNode.Layer_NodeNum];
                        if ( Dist < BestDist )
                        {
                            BestDist = Dist;
                            BestNode = CurrentNode;
                        }
                    }
                    CurrentNode = BestNode;
                }
                else
                {
                    CurrentNode = Args.FinishNodes[A];
                }
                if ( CurrentNode == null )
                {
                    //no path
                    return;
                }
                SourceNodeCount = 0;
                BestDist = Args.NodeValues[CurrentNode.Layer_NodeNum];
                if ( BestDist == float.MaxValue )
                {
                    //no path
                    return;
                }
                do
                {
                    Args.SourceNodes[SourceNodeCount] = CurrentNode;
                    SourceNodeCount++;
                    if ( CurrentNode == StartNode )
                    {
                        break;
                    }
                    BestNode = null;
                    for ( B = 0; B <= CurrentNode.ConnectionCount - 1; B++ )
                    {
                        tmpConnection = CurrentNode.Connections[B];
                        ConnectedNode = tmpConnection.GetOtherNode(CurrentNode);
                        if ( ConnectedNode.ParentNode == Args.SourceParentNodeA || ConnectedNode.ParentNode == Args.SourceParentNodeB )
                        {
                            Dist = Args.NodeValues[ConnectedNode.Layer_NodeNum];
                            if ( Dist < BestDist )
                            {
                                BestDist = Dist;
                                BestNode = ConnectedNode;
                            }
                        }
                    }
                    if ( BestNode == null )
                    {
                        Args.BestPaths[A] = null;
                        //no path
                        return;
                    }
                    CurrentNode = BestNode;
                } while ( true );

                BestPath = new Path();
                Args.BestPaths[A] = BestPath;
                BestPath.Value = Args.NodeValues[Args.FinishNodes[A].Layer_NodeNum];
                BestPath.NodeCount = SourceNodeCount;
                BestPath.Nodes = new PathfinderNode[BestPath.NodeCount];
                for ( B = 0; B <= BestPath.NodeCount - 1; B++ )
                {
                    BestPath.Nodes[B] = Args.SourceNodes[SourceNodeCount - B - 1];
                }
            }
        }

        public void FloodRouteAll(ref sFloodRouteAllArgs Args)
        {
            var CurrentNode = default(PathfinderNode);
            var ConnectedNode = default(PathfinderNode);
            var SourceNodeCount = 0;
            var SourceNodeNum = 0;
            var tmpConnection = default(PathfinderConnection);
            float ResultValue = 0;
            float BestValue = 0;
            var A = 0;

            SourceNodeCount = Args.StartNodeCount;
            for ( A = 0; A <= SourceNodeCount - 1; A++ )
            {
                Args.SourceNodes[A] = Args.StartNodes[A];
                Args.NodeValuesA[Args.StartNodes[A].Layer_NodeNum] = 0.0F;
            }

            SourceNodeNum = 0;
            while ( SourceNodeNum < SourceNodeCount )
            {
                CurrentNode = Args.SourceNodes[SourceNodeNum];
                for ( A = 0; A <= CurrentNode.ConnectionCount - 1; A++ )
                {
                    tmpConnection = CurrentNode.Connections[A];
                    ConnectedNode = tmpConnection.GetOtherNode(CurrentNode);
                    if ( Args.Visit[ConnectedNode.ParentNode.Layer_NodeNum] )
                    {
                        if ( ConnectedNode.Clearance >= Args.MinClearance )
                        {
                            ResultValue = Args.NodeValuesA[CurrentNode.Layer_NodeNum] + tmpConnection.Value;
                            if ( ResultValue < Args.NodeValuesA[ConnectedNode.Layer_NodeNum] )
                            {
                                Args.NodeValuesA[ConnectedNode.Layer_NodeNum] = ResultValue;
                                Args.SourceNodes[SourceNodeCount] = ConnectedNode;
                                SourceNodeCount++;
                            }
                        }
                    }
                }
                SourceNodeNum++;
            }

            SourceNodeCount = 0;
            BestValue = Args.NodeValuesA[Args.FinishNode.Layer_NodeNum];
            if ( BestValue == float.MaxValue )
            {
                Args.BestNodeCount = 0;
                return;
            }

            BestValue *= Args.BestTolerance;

            Args.SourceNodes[0] = Args.FinishNode;
            SourceNodeCount = 1;
            Args.NodeValuesB[Args.FinishNode.Layer_NodeNum] = 0.0F;

            SourceNodeNum = 0;
            while ( SourceNodeNum < SourceNodeCount )
            {
                CurrentNode = Args.SourceNodes[SourceNodeNum];
                for ( A = 0; A <= CurrentNode.ConnectionCount - 1; A++ )
                {
                    tmpConnection = CurrentNode.Connections[A];
                    ConnectedNode = tmpConnection.GetOtherNode(CurrentNode);
                    if ( Args.Visit[ConnectedNode.ParentNode.Layer_NodeNum] )
                    {
                        ResultValue = Args.NodeValuesB[CurrentNode.Layer_NodeNum] + tmpConnection.Value;
                        if ( ResultValue < Args.NodeValuesB[ConnectedNode.Layer_NodeNum] )
                        {
                            Args.NodeValuesB[ConnectedNode.Layer_NodeNum] = ResultValue;
                            if ( Args.NodeValuesA[ConnectedNode.Layer_NodeNum] + ResultValue <= BestValue + 500.0F )
                            {
                                Args.SourceNodes[SourceNodeCount] = ConnectedNode;
                                SourceNodeCount++;
                            }
                        }
                    }
                }
                SourceNodeNum++;
            }

            Args.BestNodeCount = SourceNodeCount;
        }

        public void FindCalc()
        {
            PathfinderNode[] ShuffledNodes = null;
            var ShuffledNodeCount = 0;
            int[] Positions = null;
            var PositionCount = 0;
            var RandNum = 0;
            var A = 0;

            while ( FindParentNodeCount > 0 )
            {
                Positions = new int[FindParentNodeCount];
                ShuffledNodeCount = FindParentNodeCount;
                ShuffledNodes = new PathfinderNode[ShuffledNodeCount];

                for ( A = 0; A <= FindParentNodeCount - 1; A++ )
                {
                    Positions[PositionCount] = PositionCount;
                    PositionCount++;
                }
                for ( A = 0; A <= FindParentNodeCount - 1; A++ )
                {
                    RandNum = App.Random.Next() * PositionCount;
                    ShuffledNodes[Positions[RandNum]] = FindParentNodes[A];
                    PositionCount--;
                    if ( RandNum < PositionCount )
                    {
                        Positions[RandNum] = Positions[PositionCount];
                    }
                }

                for ( A = 0; A <= ShuffledNodeCount - 1; A++ )
                {
                    if ( ShuffledNodes[A].Network_FindParentNum >= 0 )
                    {
                        if ( ShuffledNodes[A].ParentNode == null )
                        {
                            ShuffledNodes[A].FindParent();
                        }
                        FindParentNode_Remove(ShuffledNodes[A].Network_FindParentNum);
                    }
                }
            }

            //remove empty layers
            var LayerNum = NodeLayerCount - 1;
            do
            {
                if ( NodeLayers[LayerNum].NodeCount > 0 )
                {
                    break;
                }
                NodeLayers[LayerNum].Network_LayerNum = -1;
                if ( LayerNum == 0 )
                {
                    break;
                }
                NodeLayers[LayerNum - 1].ParentLayer = null;
                LayerNum--;
            } while ( true );
            if ( LayerNum < NodeLayerCount - 1 )
            {
                Array.Resize(ref NodeLayers, LayerNum + 1);
                NodeLayerCount = LayerNum + 1;
            }
        }

        public void LargeArraysResize()
        {
            NetworkLargeArrays.Resize(this);
        }

        //maps lowest values from the start node to all other nodes
        public void FloodProximity(ref sFloodProximityArgs Args)
        {
            var CurrentNode = default(PathfinderNode);
            var ConnectedNode = default(PathfinderNode);
            var A = 0;
            var SourceNodeCount = 0;
            var SourceNodeNum = 0;
            var tmpConnection = default(PathfinderConnection);
            float ResultValue = 0;
            var StartNode = default(PathfinderNode);

            StartNode = Args.StartNode;

            NetworkLargeArrays.Nodes_Nodes[0] = StartNode;
            SourceNodeCount = 1;
            Args.NodeValues[StartNode.Layer_NodeNum] = 0.0F;

            SourceNodeNum = 0;
            while ( SourceNodeNum < SourceNodeCount )
            {
                CurrentNode = NetworkLargeArrays.Nodes_Nodes[SourceNodeNum];
                for ( A = 0; A <= CurrentNode.ConnectionCount - 1; A++ )
                {
                    tmpConnection = CurrentNode.Connections[A];
                    ConnectedNode = tmpConnection.GetOtherNode(CurrentNode);
                    ResultValue = Args.NodeValues[CurrentNode.Layer_NodeNum] + tmpConnection.Value;
                    if ( ResultValue < Args.NodeValues[ConnectedNode.Layer_NodeNum] )
                    {
                        Args.NodeValues[ConnectedNode.Layer_NodeNum] = ResultValue;
                        NetworkLargeArrays.Nodes_Nodes[SourceNodeCount] = ConnectedNode;
                        SourceNodeCount++;
                    }
                }
                SourceNodeNum++;
            }
        }

        public void ClearChangedNodes()
        {
            var A = 0;

            for ( A = 0; A <= NodeLayerCount - 1; A++ )
            {
                NodeLayers[A].ClearChangedNodes();
            }
        }

        public bool NodeCanReachNode(PathfinderNode StartNode, PathfinderNode FinishNode)
        {
            var StartParent = StartNode;
            var FinishParent = FinishNode;

            do
            {
                if ( StartParent == FinishParent )
                {
                    return true;
                }
                StartParent = StartParent.ParentNode;
                if ( StartParent == null )
                {
                    return false;
                }
                FinishParent = FinishParent.ParentNode;
                if ( FinishParent == null )
                {
                    return false;
                }
            } while ( true );
        }

        public struct PathList
        {
            public int PathCount;
            public Path[] Paths;
        }

        public struct sFloodForValuesArgs
        {
            public Path[] BestPaths;
            public Path CurrentPath;
            public bool FinishIsParent;
            public int FinishNodeCount;
            public PathfinderNode[] FinishNodes;
            public int MinClearance;
            public float[] NodeValues;
            public PathfinderNode[] SourceNodes;
            public PathfinderNode SourceParentNodeA;
            public PathfinderNode SourceParentNodeB;
        }

        public struct sFloodProximityArgs
        {
            public float[] NodeValues;
            public PathfinderNode StartNode;
        }

        public struct sFloodRouteAllArgs
        {
            public int BestNodeCount;
            public float BestTolerance;
            public PathfinderNode FinishNode;
            public int MinClearance;
            public float[] NodeValuesA;
            public float[] NodeValuesB;
            public PathfinderNode[] SourceNodes;
            public int StartNodeCount;
            public PathfinderNode[] StartNodes;
            public bool[] Visit;
        }

        public struct sFloodRouteArgs
        {
            public Path[] BestPaths;
            public Path CurrentPath;
            public bool FinishIsParent;
            public int FinishNodeCount;
            public PathfinderNode[] FinishNodes;
            public int MinClearance;
            public float[] NodeValues;
            public PathfinderNode[] SourceNodes;
            public bool[] Visit;
        }

        public struct sFloodSpanArgs
        {
            public Path[] BestPaths;
            public Path CurrentPath;
            public bool FinishIsParent;
            public int FinishNodeCount;
            public PathfinderNode[] FinishNodes;
            public int MinClearance;
            public float[] NodeValues;
            public PathfinderNode[] SourceNodes;
            public PathfinderNode SourceParentNode;
        }
    }
}