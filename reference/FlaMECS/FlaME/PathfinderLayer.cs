namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;

    public class PathfinderLayer
    {
        public int ChangedNodeCount;
        public PathfinderNode[] ChangedNodes = new PathfinderNode[4];
        public int ConnectionCount;
        public PathfinderConnection[] Connections = new PathfinderConnection[0];
        public PathfinderNetwork Network;
        public int Network_LayerNum = -1;
        public int NodeCount;
        public PathfinderNode[] Nodes = new PathfinderNode[0];
        public PathfinderLayer ParentLayer;

        public PathfinderLayer(PathfinderNetwork NewParentNetwork)
        {
            this.Network = NewParentNetwork;
            this.Network.NodeLayer_Add(this);
        }

        public void ChangedNode_Add(PathfinderNode NewChangedNode)
        {
            if (NewChangedNode.Layer_ChangedNodeNum < 0)
            {
                if (this.ChangedNodes.GetUpperBound(0) < this.ChangedNodeCount)
                {
                    this.ChangedNodes = (PathfinderNode[]) Utils.CopyArray((Array) this.ChangedNodes, new PathfinderNode[((this.ChangedNodeCount * 2) + 1) + 1]);
                }
                this.ChangedNodes[this.ChangedNodeCount] = NewChangedNode;
                this.ChangedNodes[this.ChangedNodeCount].Layer_ChangedNodeNum = this.ChangedNodeCount;
                this.ChangedNodeCount++;
            }
        }

        public void ChangedNode_Remove(int Num)
        {
            this.ChangedNodes[Num].Layer_ChangedNodeNum = -1;
            this.ChangedNodes[Num] = null;
            this.ChangedNodeCount--;
            if (Num < this.ChangedNodeCount)
            {
                this.ChangedNodes[Num] = this.ChangedNodes[this.ChangedNodeCount];
                this.ChangedNodes[Num].Layer_ChangedNodeNum = Num;
            }
            if ((this.ChangedNodeCount * 3) < this.ChangedNodes.GetUpperBound(0))
            {
                this.ChangedNodes = (PathfinderNode[]) Utils.CopyArray((Array) this.ChangedNodes, new PathfinderNode[((this.ChangedNodeCount * 2) - 1) + 1]);
            }
        }

        public void ClearChangedNodes()
        {
            while (this.ChangedNodeCount > 0)
            {
                this.ChangedNode_Remove(0);
            }
        }

        public void Connection_Add(PathfinderConnection NewConnection)
        {
            if (this.Connections.GetUpperBound(0) < this.ConnectionCount)
            {
                this.Connections = (PathfinderConnection[]) Utils.CopyArray((Array) this.Connections, new PathfinderConnection[(((this.ConnectionCount + 1) * 2) - 1) + 1]);
            }
            this.Connections[this.ConnectionCount] = NewConnection;
            this.Connections[this.ConnectionCount].Layer_ConnectionNum = this.ConnectionCount;
            this.ConnectionCount++;
        }

        public void Connection_Remove(int Num)
        {
            this.Connections[Num].Layer_ConnectionNum = -1;
            this.ConnectionCount--;
            if (Num < this.ConnectionCount)
            {
                this.Connections[Num] = this.Connections[this.ConnectionCount];
                this.Connections[Num].Layer_ConnectionNum = Num;
            }
            if ((this.ConnectionCount * 3) < this.Connections.GetUpperBound(0))
            {
                this.Connections = (PathfinderConnection[]) Utils.CopyArray((Array) this.Connections, new PathfinderConnection[((this.ConnectionCount * 2) - 1) + 1]);
            }
        }

        public void ForceDeallocate()
        {
            int num2 = this.NodeCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.Nodes[i].ForceDeallocate();
            }
            this.Nodes = null;
            this.Connections = null;
            this.Network = null;
            this.ParentLayer = null;
        }

        public void Node_Add(PathfinderNode NewNode)
        {
            if (this.Nodes.GetUpperBound(0) < this.NodeCount)
            {
                this.Nodes = (PathfinderNode[]) Utils.CopyArray((Array) this.Nodes, new PathfinderNode[(((this.NodeCount + 1) * 2) - 1) + 1]);
            }
            this.Nodes[this.NodeCount] = NewNode;
            this.Nodes[this.NodeCount].Layer_NodeNum = this.NodeCount;
            this.NodeCount++;
        }

        public void Node_Remove(int Num)
        {
            this.Nodes[Num].Layer = null;
            this.Nodes[Num].Layer_NodeNum = -1;
            this.NodeCount--;
            if (Num < this.NodeCount)
            {
                this.Nodes[Num] = this.Nodes[this.NodeCount];
                this.Nodes[Num].Layer_NodeNum = Num;
            }
            if ((this.NodeCount * 3) < this.Nodes.GetUpperBound(0))
            {
                this.Nodes = (PathfinderNode[]) Utils.CopyArray((Array) this.Nodes, new PathfinderNode[((this.NodeCount * 2) - 1) + 1]);
            }
        }

        public PathfinderNode this[int Num]
        {
            get
            {
                return this.ChangedNodes[Num];
            }
        }

        public int GetChangedNodeCount
        {
            get
            {
                return this.ChangedNodeCount;
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

        public PathfinderNetwork GetNetwork
        {
            get
            {
                return this.Network;
            }
        }

        public int GetNetwork_LayerNum
        {
            get
            {
                return this.Network_LayerNum;
            }
        }

        public PathfinderNode this[int Num]
        {
            get
            {
                return this.Nodes[Num];
            }
        }

        public int GetNodeCount
        {
            get
            {
                return this.NodeCount;
            }
        }

        public PathfinderLayer GetParentLayer
        {
            get
            {
                return this.ParentLayer;
            }
        }
    }
}

