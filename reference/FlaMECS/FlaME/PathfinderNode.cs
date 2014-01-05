namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class PathfinderNode
    {
        public float ChildrenSpan;
        public int Clearance;
        public int ConnectionCount;
        public PathfinderConnection[] Connections;
        public PathfinderLayer Layer;
        public int Layer_ChangedNodeNum;
        public int Layer_NodeNum;
        public int Network_FindParentNum;
        public int NodeCount;
        public PathfinderNode[] Nodes;
        public PathfinderNode ParentNode;
        public int ParentNode_NodeNum;
        public float SiblingSpan;
        public object Tag;

        public PathfinderNode(PathfinderLayer NewParentLayer)
        {
            this.Network_FindParentNum = -1;
            this.Layer_NodeNum = -1;
            this.Layer_ChangedNodeNum = -1;
            this.ParentNode_NodeNum = -1;
            this.Nodes = new PathfinderNode[4];
            this.Connections = new PathfinderConnection[2];
            this.Clearance = 0x7fffffff;
            this.Layer = NewParentLayer;
            this.Layer.Node_Add(this);
        }

        public PathfinderNode(PathfinderNetwork ParentNetwork)
        {
            PathfinderLayer layer;
            this.Network_FindParentNum = -1;
            this.Layer_NodeNum = -1;
            this.Layer_ChangedNodeNum = -1;
            this.ParentNode_NodeNum = -1;
            this.Nodes = new PathfinderNode[4];
            this.Connections = new PathfinderConnection[2];
            this.Clearance = 0x7fffffff;
            if (ParentNetwork.NodeLayerCount == 0)
            {
                layer = new PathfinderLayer(ParentNetwork);
            }
            else
            {
                layer = ParentNetwork.NodeLayers[0];
            }
            this.Layer = layer;
            layer.Node_Add(this);
        }

        public void CheckIntegrity()
        {
            int num;
            PathfinderNode[] nodeArray;
            if (this.NodeCount >= 2)
            {
                sVisited visited;
                visited.Visited = new bool[(this.NodeCount - 1) + 1];
                this.FloodCheckInternal(this.Nodes[0], ref visited);
                int num4 = this.NodeCount - 1;
                for (int i = 0; i <= num4; i++)
                {
                    if (!visited.Visited[i])
                    {
                        goto Label_00BF;
                    }
                }
            }
            if (!((this.NodeCount == 1) & (this.ConnectionCount == 0)))
            {
                if (this.NodeCount > 1)
                {
                    this.SpanCalc();
                    return;
                }
                if (this.NodeCount == 0)
                {
                    if (this.ParentNode != null)
                    {
                        PathfinderNode parentNode = this.ParentNode;
                        parentNode.Node_Remove(this.ParentNode_NodeNum);
                        parentNode.CheckIntegrity();
                    }
                    if (this.Layer.Network_LayerNum > 0)
                    {
                        this.Deallocate();
                    }
                    return;
                }
                return;
            }
        Label_00BF:
            nodeArray = new PathfinderNode[(this.NodeCount - 1) + 1];
            int num5 = this.NodeCount - 1;
            for (num = 0; num <= num5; num++)
            {
                nodeArray[num] = this.Nodes[num];
            }
            int nodeCount = this.NodeCount;
            this.Disband();
            int num6 = nodeCount - 1;
            for (num = 0; num <= num6; num++)
            {
                nodeArray[num].Layer.Network.FindParentNode_Add(nodeArray[num]);
            }
        }

        public void ClearanceCalc()
        {
            if (this.Layer.Network_LayerNum == 0)
            {
                Debugger.Break();
            }
            this.Clearance = 0;
            int num2 = this.NodeCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                if (this.Nodes[i].Clearance > this.Clearance)
                {
                    this.Clearance = this.Nodes[i].Clearance;
                }
            }
            if (this.ParentNode != null)
            {
                if (this.Clearance > this.ParentNode.Clearance)
                {
                    this.ParentNode.ClearanceSet(this.Clearance);
                }
                else if (this.Clearance < this.ParentNode.Clearance)
                {
                    this.ParentNode.ClearanceCalc();
                }
            }
        }

        public void ClearanceSet(int Value)
        {
            if (Value != this.Clearance)
            {
                this.Clearance = Value;
                if (this.ParentNode != null)
                {
                    if (this.Clearance > this.ParentNode.Clearance)
                    {
                        this.ParentNode.ClearanceSet(this.Clearance);
                    }
                    else if (this.Clearance < this.ParentNode.Clearance)
                    {
                        this.ParentNode.ClearanceCalc();
                    }
                }
            }
        }

        public void Connection_Add(PathfinderConnection Connection, ref int OutputNum)
        {
            OutputNum = this.ConnectionCount;
            if (this.Connections.GetUpperBound(0) < this.ConnectionCount)
            {
                this.Connections = (PathfinderConnection[]) Utils.CopyArray((Array) this.Connections, new PathfinderConnection[((this.ConnectionCount * 2) + 1) + 1]);
            }
            this.Connections[this.ConnectionCount] = Connection;
            this.ConnectionCount++;
            if (this.ParentNode == null)
            {
                this.Layer.Network.FindParentNode_Add(this);
            }
        }

        public void Connection_Remove(int Num)
        {
            this.ConnectionCount--;
            if (Num < this.ConnectionCount)
            {
                this.Connections[Num] = this.Connections[this.ConnectionCount];
                if (this.Connections[Num].NodeA == this)
                {
                    this.Connections[Num].NodeA_ConnectionNum = Num;
                }
                else if (this.Connections[Num].NodeB == this)
                {
                    this.Connections[Num].NodeB_ConnectionNum = Num;
                }
                else
                {
                    Debugger.Break();
                }
            }
            if ((this.Connections.GetUpperBound(0) + 1) > (this.ConnectionCount * 3))
            {
                this.Connections = (PathfinderConnection[]) Utils.CopyArray((Array) this.Connections, new PathfinderConnection[((this.ConnectionCount * 2) + 1) + 1]);
            }
        }

        public PathfinderConnection CreateConnection(PathfinderNode OtherNode, float Value)
        {
            if ((OtherNode.Layer == this.Layer) && (this.FindConnection(OtherNode) == null))
            {
                return new PathfinderConnection(this, OtherNode, Value);
            }
            return null;
        }

        public void Deallocate()
        {
            if (this.Network_FindParentNum >= 0)
            {
                this.Layer.Network.FindParentNode_Remove(this.Network_FindParentNum);
            }
            if (this.Layer_ChangedNodeNum >= 0)
            {
                this.Layer.ChangedNode_Remove(this.Layer_ChangedNodeNum);
            }
            if (this.ConnectionCount > 0)
            {
                Debugger.Break();
            }
            if (this.NodeCount > 0)
            {
                Debugger.Break();
            }
            if (this.ParentNode != null)
            {
                Debugger.Break();
            }
            this.Layer.Node_Remove(this.Layer_NodeNum);
        }

        public void Disband()
        {
            PathfinderNode parentNode = this.ParentNode;
            if (parentNode != null)
            {
                parentNode.Node_Remove(this.ParentNode_NodeNum);
                parentNode.CheckIntegrity();
            }
            while (this.NodeCount > 0)
            {
                this.Node_Remove(0);
            }
            this.Deallocate();
        }

        public PathfinderConnection FindConnection(PathfinderNode NodeToFind)
        {
            int num2 = this.ConnectionCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                PathfinderConnection connection2 = this.Connections[i];
                if (connection2.GetOtherNode(this) == NodeToFind)
                {
                    return connection2;
                }
            }
            return null;
        }

        public void FindParent()
        {
            PathfinderNode nodeToAdd = null;
            if ((this.NodeCount == 0) & (this.Layer.Network_LayerNum > 0))
            {
                Debugger.Break();
            }
            else if (this.ParentNode != null)
            {
                Debugger.Break();
            }
            else
            {
                bool flag2;
                float maxValue = float.MaxValue;
                int num7 = this.ConnectionCount - 1;
                for (int i = 0; i <= num7; i++)
                {
                    bool flag;
                    float num6;
                    PathfinderConnection connection = this.Connections[i];
                    PathfinderNode otherNode = connection.GetOtherNode(this);
                    PathfinderNode parentNode = otherNode.ParentNode;
                    if (parentNode == null)
                    {
                        parentNode = connection.GetOtherNode(this);
                        num6 = connection.Value * (0.98f + (VBMath.Rnd() * 0.04f));
                        if (num6 < maxValue)
                        {
                            maxValue = num6;
                            nodeToAdd = parentNode;
                            flag2 = true;
                        }
                        continue;
                    }
                    if (parentNode.NodeCount == 3)
                    {
                        int num5 = 0;
                        flag = false;
                        int num8 = parentNode.NodeCount - 1;
                        for (int j = 0; j <= num8; j++)
                        {
                            int num9 = parentNode.Nodes[j].ConnectionCount - 1;
                            for (int k = 0; k <= num9; k++)
                            {
                                if (parentNode.Nodes[j].Connections[k].GetOtherNode(parentNode.Nodes[j]) == this)
                                {
                                    num5++;
                                    if (num5 >= 2)
                                    {
                                        flag = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        num6 = (otherNode.SiblingSpan + connection.Value) * (0.98f + (VBMath.Rnd() * 0.04f));
                        if (num6 < maxValue)
                        {
                            maxValue = num6;
                            nodeToAdd = parentNode;
                            flag2 = false;
                        }
                    }
                }
                if (nodeToAdd != null)
                {
                    if (flag2)
                    {
                        PathfinderLayer parentLayer;
                        if (this.Layer.ParentLayer == null)
                        {
                            parentLayer = new PathfinderLayer(this.Layer.Network);
                        }
                        else
                        {
                            parentLayer = this.Layer.ParentLayer;
                        }
                        PathfinderNode newFindParentNode = new PathfinderNode(parentLayer);
                        newFindParentNode.Node_Add(this);
                        newFindParentNode.Node_Add(nodeToAdd);
                        newFindParentNode.SpanCalc();
                        this.RaiseConnections();
                        nodeToAdd.RaiseConnections();
                        newFindParentNode.Layer.Network.FindParentNode_Add(newFindParentNode);
                    }
                    else if (nodeToAdd != null)
                    {
                        nodeToAdd.Node_Add(this);
                        if (nodeToAdd.NodeCount >= 4)
                        {
                            nodeToAdd.Split();
                        }
                        else
                        {
                            nodeToAdd.SpanCalc();
                            this.RaiseConnections();
                            if (nodeToAdd.ParentNode == null)
                            {
                                nodeToAdd.Layer.Network.FindParentNode_Add(nodeToAdd);
                            }
                        }
                    }
                }
                else if (this.ConnectionCount > 0)
                {
                    PathfinderLayer layer2;
                    if (this.Layer.ParentLayer == null)
                    {
                        layer2 = new PathfinderLayer(this.Layer.Network);
                    }
                    else
                    {
                        layer2 = this.Layer.ParentLayer;
                    }
                    PathfinderNode node5 = new PathfinderNode(layer2);
                    node5.Node_Add(this);
                    node5.SpanCalc();
                    this.RaiseConnections();
                    node5.Layer.Network.FindParentNode_Add(node5);
                }
            }
        }

        public void FloodCheckInternal(PathfinderNode CurrentNode, ref sVisited Visited)
        {
            Visited.Visited[CurrentNode.ParentNode_NodeNum] = true;
            int num2 = CurrentNode.ConnectionCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                PathfinderNode otherNode = CurrentNode.Connections[i].GetOtherNode(CurrentNode);
                if ((otherNode.ParentNode == this) && !Visited.Visited[otherNode.ParentNode_NodeNum])
                {
                    this.FloodCheckInternal(otherNode, ref Visited);
                }
            }
        }

        public void ForceDeallocate()
        {
            int num2 = this.ConnectionCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.Connections[i].ForceDeallocate();
            }
            this.Connections = null;
            this.Nodes = null;
            this.ParentNode = null;
            this.Layer = null;
        }

        public PathfinderConnection GetOrCreateConnection(PathfinderNode OtherNode, float Value)
        {
            if (OtherNode.Layer != this.Layer)
            {
                return null;
            }
            PathfinderConnection connection2 = this.FindConnection(OtherNode);
            if (connection2 == null)
            {
                return new PathfinderConnection(this, OtherNode, Value);
            }
            return connection2;
        }

        public void Node_Add(PathfinderNode NodeToAdd)
        {
            if (this.Layer == null)
            {
                Debugger.Break();
            }
            else if (NodeToAdd.Layer.Network_LayerNum != (this.Layer.Network_LayerNum - 1))
            {
                Debugger.Break();
            }
            else if (NodeToAdd.ParentNode != null)
            {
                Debugger.Break();
            }
            else
            {
                if (this.Layer_ChangedNodeNum < 0)
                {
                    this.Layer.ChangedNode_Add(this);
                }
                NodeToAdd.ParentNode = this;
                NodeToAdd.ParentNode_NodeNum = this.NodeCount;
                this.Nodes[this.NodeCount] = NodeToAdd;
                this.NodeCount++;
                if (NodeToAdd.ConnectionCount == 0)
                {
                    Debugger.Break();
                }
                if (NodeToAdd.Clearance > this.Clearance)
                {
                    this.ClearanceSet(this.Clearance);
                }
                else
                {
                    this.ClearanceCalc();
                }
            }
        }

        public void Node_Remove(int Num)
        {
            PathfinderNode node = this.Nodes[Num];
            if ((node.ParentNode_NodeNum != Num) | (node.ParentNode != this))
            {
                Debugger.Break();
            }
            else
            {
                if (this.Layer_ChangedNodeNum < 0)
                {
                    this.Layer.ChangedNode_Add(this);
                }
                node.ParentNode = null;
                node.ParentNode_NodeNum = -1;
                this.NodeCount--;
                if (Num < this.NodeCount)
                {
                    this.Nodes[Num] = this.Nodes[this.NodeCount];
                    this.Nodes[Num].ParentNode_NodeNum = Num;
                }
                this.Nodes[this.NodeCount] = null;
                int num2 = node.ConnectionCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    node.Connections[i].UnlinkParentDependants();
                }
                if ((this.NodeCount == 0) & (this.ConnectionCount > 0))
                {
                    Debugger.Break();
                }
                this.ClearanceCalc();
            }
        }

        public void RaiseConnections()
        {
            int num2 = this.ConnectionCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.Connections[i].RaiseDependant();
            }
        }

        public void SpanCalc()
        {
            int num;
            PathfinderNetwork.sFloodSpanArgs args = new PathfinderNetwork.sFloodSpanArgs {
                NodeValues = this.Layer.Network.NetworkLargeArrays.Nodes_ValuesA,
                FinishIsParent = false,
                SourceNodes = this.Layer.Network.NetworkLargeArrays.Nodes_Nodes,
                SourceParentNode = this,
                CurrentPath = this.Layer.Network.NetworkLargeArrays.Nodes_Path,
                FinishNodeCount = this.NodeCount,
                FinishNodes = new PathfinderNode[(this.NodeCount - 1) + 1]
            };
            int num3 = this.NodeCount - 1;
            for (num = 0; num <= num3; num++)
            {
                args.FinishNodes[num] = this.Nodes[num];
            }
            this.ChildrenSpan = 0f;
            int num4 = this.NodeCount - 1;
            num = 0;
            while (num <= num4)
            {
                this.Nodes[num].SiblingSpan = 0f;
                num++;
            }
            int num5 = this.NodeCount - 1;
            for (int i = 0; i <= num5; i++)
            {
                args.CurrentPath.Nodes[0] = this.Nodes[i];
                args.CurrentPath.NodeCount = 1;
                int num6 = this.NodeCount - 1;
                num = 0;
                while (num <= num6)
                {
                    args.NodeValues[this.Nodes[num].Layer_NodeNum] = float.MaxValue;
                    num++;
                }
                args.BestPaths = new PathfinderNetwork.Path[(args.FinishNodeCount - 1) + 1];
                this.Layer.Network.FloodSpan(ref args);
                int num7 = this.NodeCount - 1;
                for (num = i + 1; num <= num7; num++)
                {
                    if (args.BestPaths[num] == null)
                    {
                        Debugger.Break();
                        return;
                    }
                    if (args.BestPaths[num].Value > this.ChildrenSpan)
                    {
                        this.ChildrenSpan = args.BestPaths[num].Value;
                    }
                    if (args.BestPaths[num].Value > this.Nodes[i].SiblingSpan)
                    {
                        this.Nodes[i].SiblingSpan = args.BestPaths[num].Value;
                    }
                    if (args.BestPaths[num].Value > this.Nodes[num].SiblingSpan)
                    {
                        this.Nodes[num].SiblingSpan = args.BestPaths[num].Value;
                    }
                }
            }
        }

        public void Split()
        {
            int num;
            PathfinderNode parentNode;
            if (this.NodeCount != 4)
            {
                Debugger.Break();
            }
            PathfinderNode nodeToAdd = null;
            PathfinderNode node2 = null;
            PathfinderNode node3 = null;
            PathfinderNode node4 = null;
            PathfinderConnection connection = null;
            PathfinderConnection connection2 = null;
            PathfinderNode[] nodeArray = new PathfinderNode[(this.NodeCount - 1) + 1];
            int num8 = this.NodeCount - 1;
            for (num = 0; num <= num8; num++)
            {
                nodeArray[num] = this.Nodes[num];
            }
            int nodeCount = this.NodeCount;
            PathfinderLayer newParentLayer = this.Layer;
            this.Disband();
            float maxValue = float.MaxValue;
            int num9 = nodeCount - 1;
            for (num = 0; num <= num9; num++)
            {
                parentNode = nodeArray[num];
                int num10 = nodeCount - 1;
                for (int i = num + 1; i <= num10; i++)
                {
                    PathfinderNode node6 = nodeArray[i];
                    int num11 = nodeCount - 1;
                    int index = 0;
                    while (index <= num11)
                    {
                        if ((nodeArray[index] != parentNode) & (nodeArray[index] != node6))
                        {
                            break;
                        }
                        index++;
                    }
                    PathfinderNode self = nodeArray[index];
                    int num12 = nodeCount - 1;
                    int num6 = index + 1;
                    while (num6 <= num12)
                    {
                        if ((nodeArray[num6] != parentNode) & (nodeArray[num6] != node6))
                        {
                            break;
                        }
                        num6++;
                    }
                    PathfinderNode node8 = nodeArray[num6];
                    int num13 = parentNode.ConnectionCount - 1;
                    index = 0;
                    while (index <= num13)
                    {
                        connection = parentNode.Connections[index];
                        if (connection.GetOtherNode(parentNode) == node6)
                        {
                            break;
                        }
                        index++;
                    }
                    int num14 = self.ConnectionCount - 1;
                    num6 = 0;
                    while (num6 <= num14)
                    {
                        connection2 = self.Connections[num6];
                        if (connection2.GetOtherNode(self) == node8)
                        {
                            break;
                        }
                        num6++;
                    }
                    if ((index < parentNode.ConnectionCount) & (num6 < self.ConnectionCount))
                    {
                        float num7 = connection.Value + connection2.Value;
                        if (num7 < maxValue)
                        {
                            maxValue = num7;
                            nodeToAdd = parentNode;
                            node2 = node6;
                            node3 = self;
                            node4 = node8;
                        }
                    }
                }
            }
            if (nodeToAdd != null)
            {
                if (this.ParentNode != null)
                {
                    parentNode = this.ParentNode;
                    parentNode.Node_Remove(this.ParentNode_NodeNum);
                }
                else
                {
                    parentNode = null;
                }
                if (parentNode != null)
                {
                    parentNode.CheckIntegrity();
                }
                PathfinderNode newFindParentNode = new PathfinderNode(newParentLayer);
                PathfinderNode node10 = new PathfinderNode(newParentLayer);
                newFindParentNode.Node_Add(nodeToAdd);
                newFindParentNode.Node_Add(node2);
                newFindParentNode.SpanCalc();
                nodeToAdd.RaiseConnections();
                node2.RaiseConnections();
                newFindParentNode.Layer.Network.FindParentNode_Add(newFindParentNode);
                node10.Node_Add(node3);
                node10.Node_Add(node4);
                node10.SpanCalc();
                node3.RaiseConnections();
                node4.RaiseConnections();
                node10.Layer.Network.FindParentNode_Add(node10);
            }
            else
            {
                Debugger.Break();
            }
        }

        public PathfinderNode this[int Num]
        {
            get
            {
                return this.Nodes[Num];
            }
        }

        public int GetChildNodeCount
        {
            get
            {
                return this.NodeCount;
            }
        }

        public float GetChildrenSpan
        {
            get
            {
                return this.ChildrenSpan;
            }
        }

        public int GetClearance
        {
            get
            {
                return this.Clearance;
            }
        }

        public PathfinderConnection this[int Num]
        {
            get
            {
                return this.Connections[Num];
            }
        }

        public int GetConnectionCount
        {
            get
            {
                return this.ConnectionCount;
            }
        }

        public PathfinderLayer GetLayer
        {
            get
            {
                return this.Layer;
            }
        }

        public int GetLayer_NodeNum
        {
            get
            {
                return this.Layer_NodeNum;
            }
        }

        public PathfinderNode GetParentNode
        {
            get
            {
                return this.ParentNode;
            }
        }

        public int GetParentNode_NodeNum
        {
            get
            {
                return this.ParentNode_NodeNum;
            }
        }

        public float GetSiblingSpan
        {
            get
            {
                return this.SiblingSpan;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sVisited
        {
            public bool[] Visited;
        }
    }
}

