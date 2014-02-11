using System;
using System.Diagnostics;

namespace SharpFlame.Pathfinding
{
    public class PathfinderNode
    {
        public object Tag;

        public int Network_FindParentNum = -1;

        public PathfinderLayer Layer;

        public PathfinderLayer GetLayer
        {
            get { return Layer; }
        }

        public int Layer_NodeNum = -1;

        public int GetLayer_NodeNum
        {
            get { return Layer_NodeNum; }
        }

        public int Layer_ChangedNodeNum = -1;

        public PathfinderNode ParentNode;

        public PathfinderNode GetParentNode
        {
            get { return ParentNode; }
        }

        public int ParentNode_NodeNum = -1;

        public int GetParentNode_NodeNum
        {
            get { return ParentNode_NodeNum; }
        }

        public PathfinderNode[] Nodes = new PathfinderNode[4];

        public PathfinderNode get_GetChildNode(int Num)
        {
            return Nodes[Num];
        }

        public int NodeCount;

        public int GetChildNodeCount
        {
            get { return NodeCount; }
        }

        public PathfinderConnection[] Connections = new PathfinderConnection[2];

        public PathfinderConnection get_GetConnection(int Num)
        {
            return Connections[Num];
        }

        public int ConnectionCount;

        public int GetConnectionCount
        {
            get { return ConnectionCount; }
        }

        public float SiblingSpan;

        public float GetSiblingSpan
        {
            get { return SiblingSpan; }
        }

        public float ChildrenSpan;

        public float GetChildrenSpan
        {
            get { return ChildrenSpan; }
        }

        public int Clearance = int.MaxValue;

        public int GetClearance
        {
            get { return Clearance; }
        }

        public void Node_Add(PathfinderNode NodeToAdd)
        {
            if ( Layer == null )
            {
                Debugger.Break();
                return;
            }

            if ( NodeToAdd.Layer.Network_LayerNum != Layer.Network_LayerNum - 1 )
            {
                Debugger.Break();
                return;
            }

            if ( NodeToAdd.ParentNode != null )
            {
                Debugger.Break();
                return;
            }

            if ( Layer_ChangedNodeNum < 0 )
            {
                Layer.ChangedNode_Add(this);
            }

            NodeToAdd.ParentNode = this;
            NodeToAdd.ParentNode_NodeNum = NodeCount;

            Nodes[NodeCount] = NodeToAdd;
            NodeCount++;

            if ( NodeToAdd.ConnectionCount == 0 )
            {
                Debugger.Break();
            }

            if ( NodeToAdd.Clearance > Clearance )
            {
                ClearanceSet(Clearance);
            }
            else
            {
                ClearanceCalc();
            }
        }

        public void RaiseConnections()
        {
            int A = 0;

            for ( A = 0; A <= ConnectionCount - 1; A++ )
            {
                Connections[A].RaiseDependant();
            }
        }

        public PathfinderConnection FindConnection(PathfinderNode NodeToFind)
        {
            int A = 0;
            PathfinderConnection tmpConnection = default(PathfinderConnection);

            for ( A = 0; A <= ConnectionCount - 1; A++ )
            {
                tmpConnection = Connections[A];
                if ( tmpConnection.GetOtherNode(this) == NodeToFind )
                {
                    return tmpConnection;
                }
            }
            return null;
        }

        public void Node_Remove(int Num)
        {
            PathfinderNode tmpNodeA = Nodes[Num];
            PathfinderConnection tmpConnection = default(PathfinderConnection);
            int A = 0;

            if ( tmpNodeA.ParentNode_NodeNum != Num || tmpNodeA.ParentNode != this )
            {
                Debugger.Break();
                return;
            }

            if ( Layer_ChangedNodeNum < 0 )
            {
                Layer.ChangedNode_Add(this);
            }

            tmpNodeA.ParentNode = null;
            tmpNodeA.ParentNode_NodeNum = -1;

            NodeCount--;
            if ( Num < NodeCount )
            {
                Nodes[Num] = Nodes[NodeCount];
                Nodes[Num].ParentNode_NodeNum = Num;
            }
            Nodes[NodeCount] = null;

            for ( A = 0; A <= tmpNodeA.ConnectionCount - 1; A++ )
            {
                tmpConnection = tmpNodeA.Connections[A];
                tmpConnection.UnlinkParentDependants();
            }

            if ( NodeCount == 0 & ConnectionCount > 0 )
            {
                Debugger.Break();
            }

            ClearanceCalc();
        }

        public struct sVisited
        {
            public bool[] Visited;
        }

        public void FloodCheckInternal(PathfinderNode CurrentNode, ref sVisited Visited)
        {
            int A = 0;
            PathfinderNode tmpNode = default(PathfinderNode);
            PathfinderConnection tmpConnection = default(PathfinderConnection);

            Visited.Visited[CurrentNode.ParentNode_NodeNum] = true;

            for ( A = 0; A <= CurrentNode.ConnectionCount - 1; A++ )
            {
                tmpConnection = CurrentNode.Connections[A];
                tmpNode = tmpConnection.GetOtherNode(CurrentNode);
                if ( tmpNode.ParentNode == this )
                {
                    if ( !Visited.Visited[tmpNode.ParentNode_NodeNum] )
                    {
                        FloodCheckInternal(tmpNode, ref Visited);
                    }
                }
            }
        }

        public void Deallocate()
        {
            if ( Network_FindParentNum >= 0 )
            {
                Layer.Network.FindParentNode_Remove(Network_FindParentNum);
            }
            if ( Layer_ChangedNodeNum >= 0 )
            {
                Layer.ChangedNode_Remove(Layer_ChangedNodeNum);
            }
            if ( ConnectionCount > 0 )
            {
                Debugger.Break();
            }
            if ( NodeCount > 0 )
            {
                Debugger.Break();
            }
            if ( ParentNode != null )
            {
                Debugger.Break();
            }
            Layer.Node_Remove(Layer_NodeNum);
        }

        public void ForceDeallocate()
        {
            int A = 0;

            for ( A = 0; A <= ConnectionCount - 1; A++ )
            {
                Connections[A].ForceDeallocate();
            }
            Connections = null;
            Nodes = null;
            ParentNode = null;
            Layer = null;
        }

        public void FindParent()
        {
            PathfinderNode tmpNodeA = default(PathfinderNode);
            float BestScore = 0;
            PathfinderNode BestNode = null;
            float Score = 0;
            int A = 0;
            bool MakeNew = default(bool);
            int B = 0;
            int Count = 0;
            int C = 0;
            bool Allow = default(bool);
            PathfinderConnection tmpConnection = default(PathfinderConnection);
            PathfinderNode DestNode = default(PathfinderNode);

            if ( NodeCount == 0 & Layer.Network_LayerNum > 0 )
            {
                Debugger.Break();
                return;
            }

            if ( ParentNode != null )
            {
                Debugger.Break();
                return;
            }

            BestScore = float.MaxValue;
            for ( A = 0; A <= ConnectionCount - 1; A++ )
            {
                tmpConnection = Connections[A];
                DestNode = tmpConnection.GetOtherNode(this);
                tmpNodeA = DestNode.ParentNode;
                if ( tmpNodeA == null )
                {
                    tmpNodeA = tmpConnection.GetOtherNode(this);
                    Score = tmpConnection.Value * (0.98F + App.Random.Next() * 0.04F);
                    if ( Score < BestScore )
                    {
                        BestScore = Score;
                        BestNode = tmpNodeA;
                        MakeNew = true;
                    }
                }
                else
                {
                    //dont allow this to join to another when the other has 3 nodes and they only have one connection
                    if ( tmpNodeA.NodeCount == 3 )
                    {
                        Count = 0;
                        Allow = false;
                        for ( B = 0; B <= tmpNodeA.NodeCount - 1; B++ )
                        {
                            for ( C = 0; C <= tmpNodeA.Nodes[B].ConnectionCount - 1; C++ )
                            {
                                if ( tmpNodeA.Nodes[B].Connections[C].GetOtherNode(tmpNodeA.Nodes[B]) == this )
                                {
                                    Count++;
                                    if ( Count >= 2 )
                                    {
                                        Allow = true;
                                        goto CountFinished;
                                    }
                                    break;
                                }
                            }
                        }
                        CountFinished:
                        1.GetHashCode(); //TODO: cleanup this loop
                    }
                    else
                    {
                        Allow = true;
                    }
                    if ( Allow )
                    {
                        Score = (DestNode.SiblingSpan + tmpConnection.Value) * (0.98F + App.Random.Next() * 0.04F);
                        if ( Score < BestScore )
                        {
                            BestScore = Score;
                            BestNode = tmpNodeA;
                            MakeNew = false;
                        }
                    }
                }
            }
            if ( BestNode != null )
            {
                if ( MakeNew )
                {
                    PathfinderLayer tmpLayer = default(PathfinderLayer);
                    if ( Layer.ParentLayer == null )
                    {
                        tmpLayer = new PathfinderLayer(Layer.Network);
                    }
                    else
                    {
                        tmpLayer = Layer.ParentLayer;
                    }
                    PathfinderNode NewNode = new PathfinderNode(tmpLayer);
                    NewNode.Node_Add(this);
                    NewNode.Node_Add(BestNode);
                    NewNode.SpanCalc();
                    RaiseConnections();
                    BestNode.RaiseConnections();
                    NewNode.Layer.Network.FindParentNode_Add(NewNode);
                }
                else
                {
                    if ( BestNode != null )
                    {
                        BestNode.Node_Add(this);
                        if ( BestNode.NodeCount >= 4 )
                        {
                            BestNode.Split();
                        }
                        else
                        {
                            BestNode.SpanCalc();
                            RaiseConnections();
                            if ( BestNode.ParentNode == null )
                            {
                                BestNode.Layer.Network.FindParentNode_Add(BestNode);
                            }
                        }
                    }
                }
            }
            else if ( ConnectionCount > 0 )
            {
                //it is part of a network but there is no suitable parent to join, so make a new isolated parent
                PathfinderLayer tmpLayer = default(PathfinderLayer);
                if ( Layer.ParentLayer == null )
                {
                    tmpLayer = new PathfinderLayer(Layer.Network);
                }
                else
                {
                    tmpLayer = Layer.ParentLayer;
                }
                PathfinderNode NewNode = new PathfinderNode(tmpLayer);
                NewNode.Node_Add(this);
                NewNode.SpanCalc();
                RaiseConnections();
                NewNode.Layer.Network.FindParentNode_Add(NewNode);
            }
        }

        public void Split()
        {
            if ( NodeCount != 4 )
            {
                Debugger.Break();
            }

            float Value = 0;
            float BestValue = 0;
            PathfinderNode BestNodeA = null;
            PathfinderNode BestNodeB = null;
            PathfinderNode BestNodeC = null;
            PathfinderNode BestNodeD = null;
            int A = 0;
            int B = 0;
            PathfinderNode tmpNodeA = default(PathfinderNode);
            PathfinderNode tmpNodeB = default(PathfinderNode);
            PathfinderNode tmpNodeC = default(PathfinderNode);
            PathfinderNode tmpNodeD = default(PathfinderNode);
            PathfinderConnection tmpConnectionA = null;
            PathfinderConnection tmpConnectionB = null;
            int C = 0;
            int D = 0;

            PathfinderNode[] Children = new PathfinderNode[NodeCount];
            for ( A = 0; A <= NodeCount - 1; A++ )
            {
                Children[A] = Nodes[A];
            }
            int ChildCount = NodeCount;
            PathfinderLayer ThisLayer = Layer;

            Disband();

            BestValue = float.MaxValue;
            for ( A = 0; A <= ChildCount - 1; A++ )
            {
                tmpNodeA = Children[A];
                for ( B = A + 1; B <= ChildCount - 1; B++ )
                {
                    tmpNodeB = Children[B];
                    for ( C = 0; C <= ChildCount - 1; C++ )
                    {
                        if ( Children[C] != tmpNodeA && Children[C] != tmpNodeB )
                        {
                            break;
                        }
                    }
                    tmpNodeC = Children[C];
                    for ( D = C + 1; D <= ChildCount - 1; D++ )
                    {
                        if ( Children[D] != tmpNodeA && Children[D] != tmpNodeB )
                        {
                            break;
                        }
                    }
                    tmpNodeD = Children[D];
                    for ( C = 0; C <= tmpNodeA.ConnectionCount - 1; C++ )
                    {
                        tmpConnectionA = tmpNodeA.Connections[C];
                        if ( tmpConnectionA.GetOtherNode(tmpNodeA) == tmpNodeB )
                        {
                            break;
                        }
                    }
                    for ( D = 0; D <= tmpNodeC.ConnectionCount - 1; D++ )
                    {
                        tmpConnectionB = tmpNodeC.Connections[D];
                        if ( tmpConnectionB.GetOtherNode(tmpNodeC) == tmpNodeD )
                        {
                            break;
                        }
                    }
                    if ( C < tmpNodeA.ConnectionCount & D < tmpNodeC.ConnectionCount )
                    {
                        Value = tmpConnectionA.Value + tmpConnectionB.Value;
                        if ( Value < BestValue )
                        {
                            BestValue = Value;
                            BestNodeA = tmpNodeA;
                            BestNodeB = tmpNodeB;
                            BestNodeC = tmpNodeC;
                            BestNodeD = tmpNodeD;
                        }
                    }
                }
            }

            if ( BestNodeA != null )
            {
                if ( ParentNode != null )
                {
                    tmpNodeA = ParentNode;
                    tmpNodeA.Node_Remove(ParentNode_NodeNum);
                }
                else
                {
                    tmpNodeA = null;
                }
                if ( tmpNodeA != null )
                {
                    tmpNodeA.CheckIntegrity();
                }

                PathfinderNode NewNodeA = new PathfinderNode(ThisLayer);
                PathfinderNode NewNodeB = new PathfinderNode(ThisLayer);

                NewNodeA.Node_Add(BestNodeA);
                NewNodeA.Node_Add(BestNodeB);

                NewNodeA.SpanCalc();
                BestNodeA.RaiseConnections();
                BestNodeB.RaiseConnections();

                NewNodeA.Layer.Network.FindParentNode_Add(NewNodeA);

                NewNodeB.Node_Add(BestNodeC);
                NewNodeB.Node_Add(BestNodeD);

                NewNodeB.SpanCalc();
                BestNodeC.RaiseConnections();
                BestNodeD.RaiseConnections();

                NewNodeB.Layer.Network.FindParentNode_Add(NewNodeB);
            }
            else
            {
                Debugger.Break();
            }
        }

        public void CheckIntegrity()
        {
            //make sure im still a good parent

            if ( NodeCount >= 2 )
            {
                sVisited Visited = new sVisited();
                Visited.Visited = new bool[NodeCount];
                int A = 0;
                FloodCheckInternal(Nodes[0], ref Visited);
                for ( A = 0; A <= NodeCount - 1; A++ )
                {
                    if ( !Visited.Visited[A] )
                    {
                        goto DisbandAndFind;
                    }
                }
            }

            if ( NodeCount == 1 & ConnectionCount == 0 )
            {
                goto DisbandAndFind;
            }
            else if ( NodeCount > 1 )
            {
                SpanCalc();
            }
            else if ( NodeCount == 0 )
            {
                if ( ParentNode != null )
                {
                    PathfinderNode tmpNode = ParentNode;
                    tmpNode.Node_Remove(ParentNode_NodeNum);
                    tmpNode.CheckIntegrity();
                }
                if ( Layer.Network_LayerNum > 0 )
                {
                    Deallocate();
                }
            }

            return;
            DisbandAndFind:
            int B = 0;
            PathfinderNode[] Children = new PathfinderNode[NodeCount];
            for ( B = 0; B <= NodeCount - 1; B++ )
            {
                Children[B] = Nodes[B];
            }
            int ChildCount = NodeCount;

            Disband();

            for ( B = 0; B <= ChildCount - 1; B++ )
            {
                Children[B].Layer.Network.FindParentNode_Add(Children[B]);
            }
        }

        public PathfinderNode(PathfinderNetwork ParentNetwork)
        {
            PathfinderLayer tmpLayer = default(PathfinderLayer);

            if ( ParentNetwork.NodeLayerCount == 0 )
            {
                tmpLayer = new PathfinderLayer(ParentNetwork);
            }
            else
            {
                tmpLayer = ParentNetwork.NodeLayers[0];
            }

            Layer = tmpLayer;
            tmpLayer.Node_Add(this);
        }

        public PathfinderNode(PathfinderLayer NewParentLayer)
        {
            Layer = NewParentLayer;
            Layer.Node_Add(this);
        }

        public void Disband()
        {
            PathfinderNode tmpNode = default(PathfinderNode);

            tmpNode = ParentNode;
            if ( tmpNode != null )
            {
                tmpNode.Node_Remove(ParentNode_NodeNum);
                tmpNode.CheckIntegrity();
            }

            while ( NodeCount > 0 )
            {
                Node_Remove(0);
            }

            Deallocate();
        }

        public PathfinderConnection CreateConnection(PathfinderNode OtherNode, float Value)
        {
            PathfinderConnection tmpConnection = default(PathfinderConnection);

            if ( OtherNode.Layer != Layer )
            {
                return null;
            }

            tmpConnection = FindConnection(OtherNode);
            if ( tmpConnection == null )
            {
                return new PathfinderConnection(this, OtherNode, Value);
            }
            return null;
        }

        public PathfinderConnection GetOrCreateConnection(PathfinderNode OtherNode, float Value)
        {
            PathfinderConnection tmpConnection = default(PathfinderConnection);

            if ( OtherNode.Layer != Layer )
            {
                return null;
            }

            tmpConnection = FindConnection(OtherNode);
            if ( tmpConnection == null )
            {
                return new PathfinderConnection(this, OtherNode, Value);
            }
            return tmpConnection;
        }

        public void Connection_Add(PathfinderConnection Connection, ref int OutputNum)
        {
            OutputNum = ConnectionCount;

            if ( Connections.GetUpperBound(0) < ConnectionCount )
            {
                Array.Resize(ref Connections, ConnectionCount * 2 + 1 + 1);
            }
            Connections[ConnectionCount] = Connection;
            ConnectionCount++;

            if ( ParentNode == null )
            {
                Layer.Network.FindParentNode_Add(this);
            }
        }

        public void Connection_Remove(int Num)
        {
            ConnectionCount--;
            if ( Num < ConnectionCount )
            {
                Connections[Num] = Connections[ConnectionCount];
                if ( Connections[Num].NodeA == this )
                {
                    Connections[Num].NodeA_ConnectionNum = Num;
                }
                else if ( Connections[Num].NodeB == this )
                {
                    Connections[Num].NodeB_ConnectionNum = Num;
                }
                else
                {
                    Debugger.Break();
                }
            }
            if ( Connections.GetUpperBound(0) + 1 > ConnectionCount * 3 )
            {
                Array.Resize(ref Connections, ConnectionCount * 2 + 1 + 1);
            }
        }

        public void ClearanceSet(int Value)
        {
            if ( Value == Clearance )
            {
                return;
            }

            Clearance = Value;

            if ( ParentNode != null )
            {
                if ( Clearance > ParentNode.Clearance )
                {
                    ParentNode.ClearanceSet(Clearance);
                }
                else if ( Clearance < ParentNode.Clearance )
                {
                    ParentNode.ClearanceCalc();
                }
            }
        }

        public void ClearanceCalc()
        {
            int A = 0;

            if ( Layer.Network_LayerNum == 0 )
            {
                Debugger.Break();
            }

            Clearance = 0;
            for ( A = 0; A <= NodeCount - 1; A++ )
            {
                if ( Nodes[A].Clearance > Clearance )
                {
                    Clearance = Nodes[A].Clearance;
                }
            }

            if ( ParentNode != null )
            {
                if ( Clearance > ParentNode.Clearance )
                {
                    ParentNode.ClearanceSet(Clearance);
                }
                else if ( Clearance < ParentNode.Clearance )
                {
                    ParentNode.ClearanceCalc();
                }
            }
        }

        public void SpanCalc()
        {
            PathfinderNetwork.sFloodSpanArgs Args = new PathfinderNetwork.sFloodSpanArgs();
            int A = 0;
            int NumA = 0;

            Args.NodeValues = Layer.Network.NetworkLargeArrays.Nodes_ValuesA;
            Args.FinishIsParent = false;
            Args.SourceNodes = Layer.Network.NetworkLargeArrays.Nodes_Nodes;
            Args.SourceParentNode = this;
            Args.CurrentPath = Layer.Network.NetworkLargeArrays.Nodes_Path;
            Args.FinishNodeCount = NodeCount;
            Args.FinishNodes = new PathfinderNode[NodeCount];
            for ( A = 0; A <= NodeCount - 1; A++ )
            {
                Args.FinishNodes[A] = Nodes[A];
            }
            ChildrenSpan = 0.0F;
            for ( A = 0; A <= NodeCount - 1; A++ )
            {
                Nodes[A].SiblingSpan = 0.0F;
            }
            for ( NumA = 0; NumA <= NodeCount - 1; NumA++ )
            {
                Args.CurrentPath.Nodes[0] = Nodes[NumA];
                Args.CurrentPath.NodeCount = 1;
                for ( A = 0; A <= NodeCount - 1; A++ )
                {
                    Args.NodeValues[Nodes[A].Layer_NodeNum] = float.MaxValue;
                }
                Args.BestPaths = new Path[Args.FinishNodeCount];
                Layer.Network.FloodSpan(ref Args);
                for ( A = NumA + 1; A <= NodeCount - 1; A++ )
                {
                    if ( Args.BestPaths[A] == null )
                    {
                        Debugger.Break();
                        return;
                    }
                    if ( Args.BestPaths[A].Value > ChildrenSpan )
                    {
                        ChildrenSpan = Args.BestPaths[A].Value;
                    }
                    if ( Args.BestPaths[A].Value > Nodes[NumA].SiblingSpan )
                    {
                        Nodes[NumA].SiblingSpan = Args.BestPaths[A].Value;
                    }
                    if ( Args.BestPaths[A].Value > Nodes[A].SiblingSpan )
                    {
                        Nodes[A].SiblingSpan = Args.BestPaths[A].Value;
                    }
                }
            }
        }
    }
}