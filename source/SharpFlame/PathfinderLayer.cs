using System;

namespace SharpFlame
{
    public class PathfinderLayer
    {
        public PathfinderNetwork Network;

        public PathfinderNetwork GetNetwork
        {
            get { return Network; }
        }

        public int Network_LayerNum = -1;

        public int GetNetwork_LayerNum
        {
            get { return Network_LayerNum; }
        }

        public PathfinderLayer ParentLayer;

        public PathfinderLayer GetParentLayer
        {
            get { return ParentLayer; }
        }

        public PathfinderNode[] Nodes = new PathfinderNode[0];

        public PathfinderNode get_GetNode(int Num)
        {
            return Nodes[Num];
        }

        public int NodeCount;

        public int GetNodeCount
        {
            get { return NodeCount; }
        }

        public PathfinderConnection[] Connections = new PathfinderConnection[0];

        public PathfinderConnection get_GetConnection(int Num)
        {
            return Connections[Num];
        }

        public int ConnectionCount;

        public int GetConnectionCount
        {
            get { return ConnectionCount; }
        }

        public void Node_Add(PathfinderNode NewNode)
        {
            if ( Nodes.GetUpperBound(0) < NodeCount )
            {
                Array.Resize(ref Nodes, (NodeCount + 1) * 2);
            }
            Nodes[NodeCount] = NewNode;
            Nodes[NodeCount].Layer_NodeNum = NodeCount;
            NodeCount++;
        }

        public void Node_Remove(int Num)
        {
            Nodes[Num].Layer = null;
            Nodes[Num].Layer_NodeNum = -1;

            NodeCount--;
            if ( Num < NodeCount )
            {
                Nodes[Num] = Nodes[NodeCount];
                Nodes[Num].Layer_NodeNum = Num;
            }
            if ( NodeCount * 3 < Nodes.GetUpperBound(0) )
            {
                Array.Resize(ref Nodes, NodeCount * 2);
            }
        }

        public void Connection_Add(PathfinderConnection NewConnection)
        {
            if ( Connections.GetUpperBound(0) < ConnectionCount )
            {
                Array.Resize(ref Connections, (ConnectionCount + 1) * 2);
            }
            Connections[ConnectionCount] = NewConnection;
            Connections[ConnectionCount].Layer_ConnectionNum = ConnectionCount;
            ConnectionCount++;
        }

        public void Connection_Remove(int Num)
        {
            Connections[Num].Layer_ConnectionNum = -1;

            ConnectionCount--;
            if ( Num < ConnectionCount )
            {
                Connections[Num] = Connections[ConnectionCount];
                Connections[Num].Layer_ConnectionNum = Num;
            }
            if ( ConnectionCount * 3 < Connections.GetUpperBound(0) )
            {
                Array.Resize(ref Connections, ConnectionCount * 2);
            }
        }

        public PathfinderLayer(PathfinderNetwork NewParentNetwork)
        {
            Network = NewParentNetwork;
            Network.NodeLayer_Add(this);
        }

        public void ForceDeallocate()
        {
            int A = 0;

            for ( A = 0; A <= NodeCount - 1; A++ )
            {
                Nodes[A].ForceDeallocate();
            }

            Nodes = null;
            Connections = null;
            Network = null;
            ParentLayer = null;
        }

        public PathfinderNode[] ChangedNodes = new PathfinderNode[4];
        public int ChangedNodeCount;

        public PathfinderNode get_GetChangedNode(int Num)
        {
            return ChangedNodes[Num];
        }

        public int GetChangedNodeCount
        {
            get { return ChangedNodeCount; }
        }

        public void ClearChangedNodes()
        {
            while ( ChangedNodeCount > 0 )
            {
                ChangedNode_Remove(0);
            }
        }

        public void ChangedNode_Add(PathfinderNode NewChangedNode)
        {
            if ( NewChangedNode.Layer_ChangedNodeNum >= 0 )
            {
                return;
            }

            if ( ChangedNodes.GetUpperBound(0) < ChangedNodeCount )
            {
                Array.Resize(ref ChangedNodes, ChangedNodeCount * 2 + 1 + 1);
            }
            ChangedNodes[ChangedNodeCount] = NewChangedNode;
            ChangedNodes[ChangedNodeCount].Layer_ChangedNodeNum = ChangedNodeCount;
            ChangedNodeCount++;
        }

        public void ChangedNode_Remove(int Num)
        {
            ChangedNodes[Num].Layer_ChangedNodeNum = -1;
            ChangedNodes[Num] = null;

            ChangedNodeCount--;
            if ( Num < ChangedNodeCount )
            {
                ChangedNodes[Num] = ChangedNodes[ChangedNodeCount];
                ChangedNodes[Num].Layer_ChangedNodeNum = Num;
            }
            if ( ChangedNodeCount * 3 < ChangedNodes.GetUpperBound(0) )
            {
                Array.Resize(ref ChangedNodes, ChangedNodeCount * 2);
            }
        }
    }
}