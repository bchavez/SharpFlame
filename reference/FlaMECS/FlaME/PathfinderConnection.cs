namespace FlaME
{
    using System;
    using System.Diagnostics;

    public class PathfinderConnection
    {
        public int CalcValueNum;
        private PathfinderConnection DependantConnection;
        public bool Destroyed;
        public int Layer_ConnectionNum;
        private int LinkCount;
        public PathfinderNode NodeA;
        public int NodeA_ConnectionNum;
        public PathfinderNode NodeB;
        public int NodeB_ConnectionNum;
        public float Value;

        public PathfinderConnection(PathfinderConnection SourceConnection)
        {
            this.Layer_ConnectionNum = -1;
            this.NodeA_ConnectionNum = -1;
            this.NodeB_ConnectionNum = -1;
            this.Value = 1f;
            this.CalcValueNum = -1;
            this.NodeA = SourceConnection.NodeA.ParentNode;
            this.NodeB = SourceConnection.NodeB.ParentNode;
            this.NodeA.Connection_Add(this, ref this.NodeA_ConnectionNum);
            this.NodeB.Connection_Add(this, ref this.NodeB_ConnectionNum);
            this.NodeA.Layer.Connection_Add(this);
            this.ValueCalc();
        }

        public PathfinderConnection(PathfinderNode NewNodeA, PathfinderNode NewNodeB, float NewValue)
        {
            this.Layer_ConnectionNum = -1;
            this.NodeA_ConnectionNum = -1;
            this.NodeB_ConnectionNum = -1;
            this.Value = 1f;
            this.CalcValueNum = -1;
            if (((NewNodeA.Layer.Network_LayerNum > 0) | (NewNodeB.Layer.Network_LayerNum > 0)) | (NewValue <= 0f))
            {
                Debugger.Break();
            }
            else
            {
                this.Value = NewValue;
                this.LinkCount = 1;
                this.NodeA = NewNodeA;
                this.NodeB = NewNodeB;
                this.NodeA.Connection_Add(this, ref this.NodeA_ConnectionNum);
                this.NodeB.Connection_Add(this, ref this.NodeB_ConnectionNum);
                this.NodeA.Layer.Connection_Add(this);
                this.RaiseDependant();
            }
        }

        public void Destroy()
        {
            if (!this.Destroyed)
            {
                this.Destroyed = true;
                PathfinderLayer layer = this.NodeA.Layer;
                PathfinderNode parentNode = this.NodeA.ParentNode;
                PathfinderNode node2 = this.NodeB.ParentNode;
                this.RemoveFromNodes();
                if (parentNode != null)
                {
                    parentNode.CheckIntegrity();
                }
                if ((node2 != null) & (node2 != parentNode))
                {
                    node2.CheckIntegrity();
                }
                this.UnlinkParentDependants();
                layer.Connection_Remove(this.Layer_ConnectionNum);
            }
        }

        public void ForceDeallocate()
        {
            this.DependantConnection = null;
            this.NodeA = null;
            this.NodeB = null;
        }

        public PathfinderNode GetOtherNode(PathfinderNode Self)
        {
            if (this.NodeA == Self)
            {
                return this.NodeB;
            }
            return this.NodeA;
        }

        private void LinkDecrease()
        {
            this.LinkCount--;
            if (this.LinkCount == 0)
            {
                this.Destroy();
            }
            else if (this.LinkCount < 0)
            {
                Debugger.Break();
            }
        }

        private void LinkIncrease()
        {
            this.LinkCount++;
        }

        public void RaiseDependant()
        {
            if ((this.DependantConnection == null) && ((this.NodeA.ParentNode != this.NodeB.ParentNode) && ((this.NodeA.ParentNode != null) & (this.NodeB.ParentNode != null))))
            {
                PathfinderConnection connection = this.NodeA.ParentNode.FindConnection(this.NodeB.ParentNode);
                if (connection == null)
                {
                    this.DependantConnection = new PathfinderConnection(this);
                    this.DependantConnection.LinkIncrease();
                    this.DependantConnection.RaiseDependant();
                }
                else
                {
                    this.DependantConnection = connection;
                    this.DependantConnection.LinkIncrease();
                }
            }
        }

        private void RemoveFromNodes()
        {
            this.NodeA.Connection_Remove(this.NodeA_ConnectionNum);
            this.NodeA = null;
            this.NodeA_ConnectionNum = -1;
            this.NodeB.Connection_Remove(this.NodeB_ConnectionNum);
            this.NodeB = null;
            this.NodeB_ConnectionNum = -1;
        }

        public void UnlinkParentDependants()
        {
            if (this.DependantConnection != null)
            {
                PathfinderConnection dependantConnection = this.DependantConnection;
                this.DependantConnection = null;
                dependantConnection.LinkDecrease();
            }
        }

        public void ValueCalc()
        {
            float num3;
            if (this.NodeA.Layer.Network_LayerNum == 0)
            {
                Debugger.Break();
            }
            PathfinderNetwork.sFloodForValuesArgs args = new PathfinderNetwork.sFloodForValuesArgs();
            PathfinderNetwork network = this.NodeA.Layer.Network;
            args.NodeValues = this.NodeA.Layer.Network.NetworkLargeArrays.Nodes_ValuesA;
            args.FinishIsParent = false;
            args.SourceNodes = this.NodeA.Layer.Network.NetworkLargeArrays.Nodes_Nodes;
            args.SourceParentNodeA = this.NodeA;
            args.SourceParentNodeB = this.NodeB;
            args.CurrentPath = this.NodeA.Layer.Network.NetworkLargeArrays.Nodes_Path;
            args.FinishNodeCount = this.NodeB.NodeCount;
            args.FinishNodes = new PathfinderNode[(this.NodeB.NodeCount - 1) + 1];
            int num4 = this.NodeB.NodeCount - 1;
            int index = 0;
            while (index <= num4)
            {
                args.FinishNodes[index] = this.NodeB.Nodes[index];
                index++;
            }
            int num5 = this.NodeA.NodeCount - 1;
            for (int i = 0; i <= num5; i++)
            {
                args.CurrentPath.Nodes[0] = this.NodeA.Nodes[i];
                args.CurrentPath.NodeCount = 1;
                int num6 = this.NodeA.NodeCount - 1;
                index = 0;
                while (index <= num6)
                {
                    args.NodeValues[this.NodeA.Nodes[index].Layer_NodeNum] = float.MaxValue;
                    index++;
                }
                int num7 = this.NodeB.NodeCount - 1;
                index = 0;
                while (index <= num7)
                {
                    args.NodeValues[this.NodeB.Nodes[index].Layer_NodeNum] = float.MaxValue;
                    index++;
                }
                args.BestPaths = new PathfinderNetwork.Path[(args.FinishNodeCount - 1) + 1];
                this.NodeA.Layer.Network.FloodForValues(ref args);
                int num8 = this.NodeB.NodeCount - 1;
                for (index = 0; index <= num8; index++)
                {
                    if (args.BestPaths[index] == null)
                    {
                        Debugger.Break();
                        return;
                    }
                    num3 += args.BestPaths[index].Value;
                }
            }
            this.Value = num3 / ((float) (this.NodeA.NodeCount * this.NodeB.NodeCount));
            if (this.Value == 0f)
            {
                Debugger.Break();
            }
            this.CalcValueNum = -1;
        }

        public PathfinderNode GetNodeA
        {
            get
            {
                return this.NodeA;
            }
        }

        public int GetNodeA_ConnectionNum
        {
            get
            {
                return this.NodeA_ConnectionNum;
            }
        }

        public PathfinderNode GetNodeB
        {
            get
            {
                return this.NodeB;
            }
        }

        public int GetNodeB_ConnectionNum
        {
            get
            {
                return this.NodeB_ConnectionNum;
            }
        }

        public float GetValue
        {
            get
            {
                return this.Value;
            }
        }
    }
}

