namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class PathfinderNetwork
    {
        public int FindParentNodeCount;
        public PathfinderNode[] FindParentNodes = new PathfinderNode[0];
        public LargeArrays NetworkLargeArrays = new LargeArrays();
        public int NodeLayerCount;
        public PathfinderLayer[] NodeLayers = new PathfinderLayer[0];

        public void ClearChangedNodes()
        {
            int num2 = this.NodeLayerCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.NodeLayers[i].ClearChangedNodes();
            }
        }

        public void Deallocate()
        {
            int num2 = this.NodeLayerCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.NodeLayers[i].ForceDeallocate();
            }
            this.NodeLayers = null;
            this.FindParentNodes = null;
        }

        public void FindCalc()
        {
            while (this.FindParentNodeCount > 0)
            {
                int num3;
                int[] numArray = new int[(this.FindParentNodeCount - 1) + 1];
                int findParentNodeCount = this.FindParentNodeCount;
                PathfinderNode[] nodeArray = new PathfinderNode[(findParentNodeCount - 1) + 1];
                int num6 = this.FindParentNodeCount - 1;
                int num = 0;
                while (num <= num6)
                {
                    numArray[num3] = num3;
                    num3++;
                    num++;
                }
                int num7 = this.FindParentNodeCount - 1;
                num = 0;
                while (num <= num7)
                {
                    int num4 = (int) Math.Round((double) ((float) (App.Random.Next() * num3)));
                    nodeArray[numArray[num4]] = this.FindParentNodes[num];
                    num3--;
                    if (num4 < num3)
                    {
                        numArray[num4] = numArray[num3];
                    }
                    num++;
                }
                int num8 = findParentNodeCount - 1;
                for (num = 0; num <= num8; num++)
                {
                    if (nodeArray[num].Network_FindParentNum >= 0)
                    {
                        if (nodeArray[num].ParentNode == null)
                        {
                            nodeArray[num].FindParent();
                        }
                        this.FindParentNode_Remove(nodeArray[num].Network_FindParentNum);
                    }
                }
            }
            int index = this.NodeLayerCount - 1;
            while (true)
            {
                if (this.NodeLayers[index].NodeCount > 0)
                {
                    break;
                }
                this.NodeLayers[index].Network_LayerNum = -1;
                if (index == 0)
                {
                    break;
                }
                this.NodeLayers[index - 1].ParentLayer = null;
                index--;
            }
            if (index < (this.NodeLayerCount - 1))
            {
                this.NodeLayers = (PathfinderLayer[]) Utils.CopyArray((Array) this.NodeLayers, new PathfinderLayer[index + 1]);
                this.NodeLayerCount = index + 1;
            }
        }

        public void FindParentNode_Add(PathfinderNode NewFindParentNode)
        {
            if (NewFindParentNode.Network_FindParentNum < 0)
            {
                if (this.FindParentNodes.GetUpperBound(0) < this.FindParentNodeCount)
                {
                    this.FindParentNodes = (PathfinderNode[]) Utils.CopyArray((Array) this.FindParentNodes, new PathfinderNode[(((this.FindParentNodeCount + 1) * 2) - 1) + 1]);
                }
                this.FindParentNodes[this.FindParentNodeCount] = NewFindParentNode;
                this.FindParentNodes[this.FindParentNodeCount].Network_FindParentNum = this.FindParentNodeCount;
                this.FindParentNodeCount++;
            }
        }

        public void FindParentNode_Remove(int Num)
        {
            this.FindParentNodes[Num].Network_FindParentNum = -1;
            this.FindParentNodes[Num] = null;
            this.FindParentNodeCount--;
            if (Num < this.FindParentNodeCount)
            {
                this.FindParentNodes[Num] = this.FindParentNodes[this.FindParentNodeCount];
                this.FindParentNodes[Num].Network_FindParentNum = Num;
            }
            if ((this.FindParentNodeCount * 3) < this.FindParentNodes.GetUpperBound(0))
            {
                this.FindParentNodes = (PathfinderNode[]) Utils.CopyArray((Array) this.FindParentNodes, new PathfinderNode[((this.FindParentNodeCount * 2) - 1) + 1]);
            }
        }

        public void FloodForValues(ref sFloodForValuesArgs Args)
        {
            int num;
            PathfinderNode otherNode;
            PathfinderNode node3;
            PathfinderNode node4 = Args.CurrentPath.Nodes[0];
            Args.SourceNodes[0] = node4;
            int index = 1;
            Args.NodeValues[node4.Layer_NodeNum] = 0f;
            for (int i = 0; i < index; i++)
            {
                node3 = Args.SourceNodes[i];
                int num9 = node3.ConnectionCount - 1;
                num = 0;
                while (num <= num9)
                {
                    PathfinderConnection connection = node3.Connections[num];
                    otherNode = connection.GetOtherNode(node3);
                    if ((otherNode.ParentNode == Args.SourceParentNodeA) | (otherNode.ParentNode == Args.SourceParentNodeB))
                    {
                        float num6 = Args.NodeValues[node3.Layer_NodeNum] + connection.Value;
                        if (num6 < Args.NodeValues[otherNode.Layer_NodeNum])
                        {
                            Args.NodeValues[otherNode.Layer_NodeNum] = num6;
                            Args.SourceNodes[index] = otherNode;
                            index++;
                        }
                    }
                    num++;
                }
            }
            int num10 = Args.FinishNodeCount - 1;
            for (num = 0; num <= num10; num++)
            {
                float maxValue;
                PathfinderNode node;
                float num5;
                if (Args.FinishIsParent)
                {
                    node = null;
                    maxValue = float.MaxValue;
                    int num11 = Args.FinishNodes[num].NodeCount - 1;
                    for (int j = 0; j <= num11; j++)
                    {
                        node3 = Args.FinishNodes[num].Nodes[j];
                        num5 = Args.NodeValues[node3.Layer_NodeNum];
                        if (num5 < maxValue)
                        {
                            maxValue = num5;
                            node = node3;
                        }
                    }
                    node3 = node;
                }
                else
                {
                    node3 = Args.FinishNodes[num];
                }
                if (node3 != null)
                {
                    index = 0;
                    maxValue = Args.NodeValues[node3.Layer_NodeNum];
                    if (maxValue != float.MaxValue)
                    {
                        int num2;
                        while (true)
                        {
                            Args.SourceNodes[index] = node3;
                            index++;
                            if (node3 == node4)
                            {
                                break;
                            }
                            node = null;
                            int num12 = node3.ConnectionCount - 1;
                            num2 = 0;
                            while (num2 <= num12)
                            {
                                otherNode = node3.Connections[num2].GetOtherNode(node3);
                                if ((otherNode.ParentNode == Args.SourceParentNodeA) | (otherNode.ParentNode == Args.SourceParentNodeB))
                                {
                                    num5 = Args.NodeValues[otherNode.Layer_NodeNum];
                                    if (num5 < maxValue)
                                    {
                                        maxValue = num5;
                                        node = otherNode;
                                    }
                                }
                                num2++;
                            }
                            if (node == null)
                            {
                                Args.BestPaths[num] = null;
                                continue;
                            }
                            node3 = node;
                        }
                        Path path = new Path();
                        Args.BestPaths[num] = path;
                        path.Value = Args.NodeValues[Args.FinishNodes[num].Layer_NodeNum];
                        path.NodeCount = index;
                        path.Nodes = new PathfinderNode[(path.NodeCount - 1) + 1];
                        int num13 = path.NodeCount - 1;
                        for (num2 = 0; num2 <= num13; num2++)
                        {
                            path.Nodes[num2] = Args.SourceNodes[(index - num2) - 1];
                        }
                    }
                }
            }
        }

        public void FloodProximity(ref sFloodProximityArgs Args)
        {
            PathfinderNode startNode = Args.StartNode;
            this.NetworkLargeArrays.Nodes_Nodes[0] = startNode;
            int index = 1;
            Args.NodeValues[startNode.Layer_NodeNum] = 0f;
            for (int i = 0; i < index; i++)
            {
                PathfinderNode self = this.NetworkLargeArrays.Nodes_Nodes[i];
                int num5 = self.ConnectionCount - 1;
                for (int j = 0; j <= num5; j++)
                {
                    PathfinderConnection connection = self.Connections[j];
                    PathfinderNode otherNode = connection.GetOtherNode(self);
                    float num2 = Args.NodeValues[self.Layer_NodeNum] + connection.Value;
                    if (num2 < Args.NodeValues[otherNode.Layer_NodeNum])
                    {
                        Args.NodeValues[otherNode.Layer_NodeNum] = num2;
                        this.NetworkLargeArrays.Nodes_Nodes[index] = otherNode;
                        index++;
                    }
                }
            }
        }

        public void FloodRoute(ref sFloodRouteArgs Args)
        {
            int num;
            PathfinderNode otherNode;
            PathfinderNode node3;
            PathfinderNode node4 = Args.CurrentPath.Nodes[0];
            Args.SourceNodes[0] = node4;
            int index = 1;
            Args.NodeValues[node4.Layer_NodeNum] = 0f;
            for (int i = 0; i < index; i++)
            {
                node3 = Args.SourceNodes[i];
                int num9 = node3.ConnectionCount - 1;
                num = 0;
                while (num <= num9)
                {
                    PathfinderConnection connection = node3.Connections[num];
                    otherNode = connection.GetOtherNode(node3);
                    if (((otherNode.ParentNode != null) && Args.Visit[otherNode.ParentNode.Layer_NodeNum]) && (otherNode.Clearance >= Args.MinClearance))
                    {
                        float num6 = Args.NodeValues[node3.Layer_NodeNum] + connection.Value;
                        if (num6 < Args.NodeValues[otherNode.Layer_NodeNum])
                        {
                            Args.NodeValues[otherNode.Layer_NodeNum] = num6;
                            Args.SourceNodes[index] = otherNode;
                            index++;
                        }
                    }
                    num++;
                }
            }
            int num10 = Args.FinishNodeCount - 1;
            for (num = 0; num <= num10; num++)
            {
                float maxValue;
                PathfinderNode node;
                float num5;
                if (Args.FinishIsParent)
                {
                    node = null;
                    maxValue = float.MaxValue;
                    int num11 = Args.FinishNodes[num].NodeCount - 1;
                    for (int j = 0; j <= num11; j++)
                    {
                        node3 = Args.FinishNodes[num].Nodes[j];
                        num5 = Args.NodeValues[node3.Layer_NodeNum];
                        if (num5 < maxValue)
                        {
                            maxValue = num5;
                            node = node3;
                        }
                    }
                    node3 = node;
                }
                else
                {
                    node3 = Args.FinishNodes[num];
                }
                if (node3 != null)
                {
                    index = 0;
                    maxValue = Args.NodeValues[node3.Layer_NodeNum];
                    if (maxValue != float.MaxValue)
                    {
                        int num2;
                        while (true)
                        {
                            Args.SourceNodes[index] = node3;
                            index++;
                            if (node3 == node4)
                            {
                                break;
                            }
                            node = null;
                            int num12 = node3.ConnectionCount - 1;
                            num2 = 0;
                            while (num2 <= num12)
                            {
                                otherNode = node3.Connections[num2].GetOtherNode(node3);
                                if ((otherNode.ParentNode != null) && Args.Visit[otherNode.ParentNode.Layer_NodeNum])
                                {
                                    num5 = Args.NodeValues[otherNode.Layer_NodeNum];
                                    if (num5 < maxValue)
                                    {
                                        maxValue = num5;
                                        node = otherNode;
                                    }
                                }
                                num2++;
                            }
                            if (node == null)
                            {
                                Args.BestPaths[num] = null;
                                continue;
                            }
                            node3 = node;
                        }
                        Path path = new Path();
                        Args.BestPaths[num] = path;
                        path.Value = Args.NodeValues[Args.FinishNodes[num].Layer_NodeNum];
                        path.NodeCount = index;
                        path.Nodes = new PathfinderNode[(path.NodeCount - 1) + 1];
                        int num13 = path.NodeCount - 1;
                        for (num2 = 0; num2 <= num13; num2++)
                        {
                            path.Nodes[num2] = Args.SourceNodes[(index - num2) - 1];
                        }
                    }
                }
            }
        }

        public void FloodRouteAll(ref sFloodRouteAllArgs Args)
        {
            PathfinderNode otherNode;
            PathfinderNode node2;
            float num3;
            PathfinderConnection connection;
            int startNodeCount = Args.StartNodeCount;
            int num6 = startNodeCount - 1;
            int index = 0;
            while (index <= num6)
            {
                Args.SourceNodes[index] = Args.StartNodes[index];
                Args.NodeValuesA[Args.StartNodes[index].Layer_NodeNum] = 0f;
                index++;
            }
            int num5 = 0;
            while (num5 < startNodeCount)
            {
                node2 = Args.SourceNodes[num5];
                int num7 = node2.ConnectionCount - 1;
                index = 0;
                while (index <= num7)
                {
                    connection = node2.Connections[index];
                    otherNode = connection.GetOtherNode(node2);
                    if (Args.Visit[otherNode.ParentNode.Layer_NodeNum] && (otherNode.Clearance >= Args.MinClearance))
                    {
                        num3 = Args.NodeValuesA[node2.Layer_NodeNum] + connection.Value;
                        if (num3 < Args.NodeValuesA[otherNode.Layer_NodeNum])
                        {
                            Args.NodeValuesA[otherNode.Layer_NodeNum] = num3;
                            Args.SourceNodes[startNodeCount] = otherNode;
                            startNodeCount++;
                        }
                    }
                    index++;
                }
                num5++;
            }
            startNodeCount = 0;
            float num2 = Args.NodeValuesA[Args.FinishNode.Layer_NodeNum];
            if (num2 == float.MaxValue)
            {
                Args.BestNodeCount = 0;
            }
            else
            {
                num2 *= Args.BestTolerance;
                Args.SourceNodes[0] = Args.FinishNode;
                startNodeCount = 1;
                Args.NodeValuesB[Args.FinishNode.Layer_NodeNum] = 0f;
                for (num5 = 0; num5 < startNodeCount; num5++)
                {
                    node2 = Args.SourceNodes[num5];
                    int num8 = node2.ConnectionCount - 1;
                    for (index = 0; index <= num8; index++)
                    {
                        connection = node2.Connections[index];
                        otherNode = connection.GetOtherNode(node2);
                        if (Args.Visit[otherNode.ParentNode.Layer_NodeNum])
                        {
                            num3 = Args.NodeValuesB[node2.Layer_NodeNum] + connection.Value;
                            if (num3 < Args.NodeValuesB[otherNode.Layer_NodeNum])
                            {
                                Args.NodeValuesB[otherNode.Layer_NodeNum] = num3;
                                if ((Args.NodeValuesA[otherNode.Layer_NodeNum] + num3) <= (num2 + 500f))
                                {
                                    Args.SourceNodes[startNodeCount] = otherNode;
                                    startNodeCount++;
                                }
                            }
                        }
                    }
                }
                Args.BestNodeCount = startNodeCount;
            }
        }

        public void FloodSpan(ref sFloodSpanArgs Args)
        {
            int num;
            PathfinderNode otherNode;
            PathfinderNode node3;
            PathfinderNode node4 = Args.CurrentPath.Nodes[0];
            Args.SourceNodes[0] = node4;
            int index = 1;
            Args.NodeValues[node4.Layer_NodeNum] = 0f;
            for (int i = 0; i < index; i++)
            {
                node3 = Args.SourceNodes[i];
                int num9 = node3.ConnectionCount - 1;
                num = 0;
                while (num <= num9)
                {
                    PathfinderConnection connection = node3.Connections[num];
                    otherNode = connection.GetOtherNode(node3);
                    if (otherNode.ParentNode == Args.SourceParentNode)
                    {
                        float num6 = Args.NodeValues[node3.Layer_NodeNum] + connection.Value;
                        if (num6 < Args.NodeValues[otherNode.Layer_NodeNum])
                        {
                            Args.NodeValues[otherNode.Layer_NodeNum] = num6;
                            Args.SourceNodes[index] = otherNode;
                            index++;
                        }
                    }
                    num++;
                }
            }
            int num10 = Args.FinishNodeCount - 1;
            for (num = 0; num <= num10; num++)
            {
                float maxValue;
                PathfinderNode node;
                float num5;
                if (Args.FinishIsParent)
                {
                    node = null;
                    maxValue = float.MaxValue;
                    int num11 = Args.FinishNodes[num].NodeCount - 1;
                    for (int j = 0; j <= num11; j++)
                    {
                        node3 = Args.FinishNodes[num].Nodes[j];
                        num5 = Args.NodeValues[node3.Layer_NodeNum];
                        if (num5 < maxValue)
                        {
                            maxValue = num5;
                            node = node3;
                        }
                    }
                    node3 = node;
                }
                else
                {
                    node3 = Args.FinishNodes[num];
                }
                if (node3 != null)
                {
                    index = 0;
                    maxValue = Args.NodeValues[node3.Layer_NodeNum];
                    if (maxValue != float.MaxValue)
                    {
                        int num2;
                        while (true)
                        {
                            Args.SourceNodes[index] = node3;
                            index++;
                            if (node3 == node4)
                            {
                                break;
                            }
                            node = null;
                            int num12 = node3.ConnectionCount - 1;
                            num2 = 0;
                            while (num2 <= num12)
                            {
                                otherNode = node3.Connections[num2].GetOtherNode(node3);
                                if (otherNode.ParentNode == Args.SourceParentNode)
                                {
                                    num5 = Args.NodeValues[otherNode.Layer_NodeNum];
                                    if (num5 < maxValue)
                                    {
                                        maxValue = num5;
                                        node = otherNode;
                                    }
                                }
                                num2++;
                            }
                            if (node == null)
                            {
                                Args.BestPaths[num] = null;
                                continue;
                            }
                            node3 = node;
                        }
                        Path path = new Path();
                        Args.BestPaths[num] = path;
                        path.Value = Args.NodeValues[Args.FinishNodes[num].Layer_NodeNum];
                        path.NodeCount = index;
                        path.Nodes = new PathfinderNode[(path.NodeCount - 1) + 1];
                        int num13 = path.NodeCount - 1;
                        for (num2 = 0; num2 <= num13; num2++)
                        {
                            path.Nodes[num2] = Args.SourceNodes[(index - num2) - 1];
                        }
                    }
                }
            }
        }

        public Path[] GetAllPaths(PathfinderNode[] StartNodes, PathfinderNode FinishNode, int MinClearance)
        {
            int num4;
            int num7;
            sFloodRouteAllArgs args;
            int num10 = StartNodes.GetUpperBound(0) + 1;
            PathfinderNode[,] nodeArray3 = new PathfinderNode[0x20, (num10 - 1) + 1];
            PathfinderNode[] nodeArray2 = new PathfinderNode[0x20];
            PathfinderNode[] nodeArray = new PathfinderNode[0x18];
            bool[] flagArray = new bool[(num10 - 1) + 1];
            Path[] pathArray2 = new Path[0x20];
            PathfinderNode[] nodeArray4 = this.NetworkLargeArrays.Nodes_Nodes;
            bool[] flagArray2 = this.NetworkLargeArrays.Nodes_Booleans;
            float[] numArray = this.NetworkLargeArrays.Nodes_ValuesA;
            float[] numArray2 = this.NetworkLargeArrays.Nodes_ValuesB;
            int index = StartNodes[0].Layer.Network_LayerNum;
            nodeArray2[index] = FinishNode;
            int num2 = index;
            while (true)
            {
                if (nodeArray2[num2].ParentNode == null)
                {
                    num7 = num2;
                    break;
                }
                nodeArray2[num2 + 1] = nodeArray2[num2].ParentNode;
                num2++;
            }
            int num11 = num10 - 1;
            int num = 0;
            while (num <= num11)
            {
                nodeArray3[index, num] = StartNodes[num];
                num2 = index;
                while (true)
                {
                    if (nodeArray3[num2, num].ParentNode == null)
                    {
                        if (nodeArray3[num2, num] == nodeArray2[num2])
                        {
                            flagArray[num] = true;
                            num4++;
                        }
                        break;
                    }
                    nodeArray3[num2 + 1, num] = nodeArray3[num2, num].ParentNode;
                    num2++;
                }
                num++;
            }
            if (num4 == 0)
            {
                return null;
            }
            int num9 = num7;
            pathArray2[num9] = new Path();
            pathArray2[num9].Nodes = new PathfinderNode[] { nodeArray2[num9] };
            pathArray2[num9].NodeCount = 1;
            do
            {
                int num3;
                PathfinderNode node;
                int num8 = num9;
                num9--;
                if (num9 < index)
                {
                    return pathArray2;
                }
                int num12 = pathArray2[num8].NodeCount - 1;
                num = 0;
                while (num <= num12)
                {
                    node = pathArray2[num8].Nodes[num];
                    int num13 = node.ConnectionCount - 1;
                    for (num2 = 0; num2 <= num13; num2++)
                    {
                        num3 = node.Connections[num2].GetOtherNode(node).Layer_NodeNum;
                        flagArray2[num3] = false;
                    }
                    num++;
                }
                int num14 = pathArray2[num8].NodeCount - 1;
                num = 0;
                while (num <= num14)
                {
                    node = pathArray2[num8].Nodes[num];
                    num3 = node.Layer_NodeNum;
                    flagArray2[num3] = true;
                    int num15 = node.NodeCount - 1;
                    for (int i = 0; i <= num15; i++)
                    {
                        num3 = node.Nodes[i].Layer_NodeNum;
                        numArray[num3] = float.MaxValue;
                        numArray2[num3] = float.MaxValue;
                    }
                    num++;
                }
                args = new sFloodRouteAllArgs {
                    FinishNode = nodeArray2[num9],
                    Visit = flagArray2,
                    NodeValuesA = numArray,
                    SourceNodes = nodeArray4,
                    NodeValuesB = numArray2,
                    MinClearance = MinClearance,
                    BestTolerance = (float) Math.Pow(1.1, (double) num9),
                    StartNodes = new PathfinderNode[(num10 - 1) + 1]
                };
                int num16 = num10 - 1;
                num = 0;
                while (num <= num16)
                {
                    if (flagArray[num])
                    {
                        args.StartNodes[args.StartNodeCount] = nodeArray3[num9, num];
                        args.StartNodeCount++;
                    }
                    num++;
                }
                args.StartNodes = (PathfinderNode[]) Utils.CopyArray((Array) args.StartNodes, new PathfinderNode[(args.StartNodeCount - 1) + 1]);
                this.FloodRouteAll(ref args);
                pathArray2[num9] = new Path();
                pathArray2[num9].Nodes = new PathfinderNode[(args.BestNodeCount - 1) + 1];
                int num17 = args.BestNodeCount - 1;
                for (num = 0; num <= num17; num++)
                {
                    pathArray2[num9].Nodes[num] = args.SourceNodes[num];
                }
                pathArray2[num9].NodeCount = args.BestNodeCount;
            }
            while (args.BestNodeCount != 0);
            Debugger.Break();
            return pathArray2;
        }

        public PathList[] GetPath(PathfinderNode[] StartNodes, PathfinderNode FinishNode, int Accuracy, int MinClearance)
        {
            int num;
            int num4;
            int num9;
            int num13 = StartNodes.GetUpperBound(0) + 1;
            PathList[] listArray2 = new PathList[(this.NodeLayerCount - 1) + 1];
            PathfinderNode[,] nodeArray3 = new PathfinderNode[(this.NodeLayerCount - 1) + 1, (num13 - 1) + 1];
            PathfinderNode[] nodeArray2 = new PathfinderNode[(this.NodeLayerCount - 1) + 1];
            PathfinderNode[] nodeArray = new PathfinderNode[0x18];
            int[] numArray2 = new int[0x18];
            bool[] flagArray = new bool[(num13 - 1) + 1];
            Path[] pathArray = new Path[0x18];
            float[] numArray = new float[0x18];
            bool[] flagArray2 = this.NetworkLargeArrays.Nodes_Booleans;
            float[] numArray3 = this.NetworkLargeArrays.Nodes_ValuesA;
            PathfinderNode[] nodeArray4 = this.NetworkLargeArrays.Nodes_Nodes;
            Path path = this.NetworkLargeArrays.Nodes_Path;
            int index = StartNodes[0].Layer.Network_LayerNum;
            nodeArray2[index] = FinishNode;
            int num2 = index;
            while (true)
            {
                if (nodeArray2[num2].ParentNode == null)
                {
                    num9 = num2;
                    break;
                }
                nodeArray2[num2 + 1] = nodeArray2[num2].ParentNode;
                num2++;
            }
            int num14 = num13 - 1;
            for (num = 0; num <= num14; num++)
            {
                nodeArray3[index, num] = StartNodes[num];
                num2 = index;
                while (true)
                {
                    if (nodeArray3[num2, num].ParentNode == null)
                    {
                        if (nodeArray3[num2, num] == nodeArray2[num2])
                        {
                            flagArray[num] = true;
                            num4++;
                        }
                        break;
                    }
                    nodeArray3[num2 + 1, num] = nodeArray3[num2, num].ParentNode;
                    num2++;
                }
            }
            if (num4 != 0)
            {
                int num11 = num9;
                listArray2[num11].Paths = new Path[] { new Path() };
                listArray2[num11].PathCount = 1;
                listArray2[num11].Paths[0].Nodes = new PathfinderNode[] { nodeArray2[num11] };
                listArray2[num11].Paths[0].NodeCount = 1;
                do
                {
                    int num3;
                    int num5;
                    int num6;
                    int num7;
                    bool flag;
                    int num12;
                    bool flag3;
                    PathfinderNode node;
                    PathfinderNode otherNode;
                    int num10 = num11;
                    num11--;
                    if (num11 < index)
                    {
                        return listArray2;
                    }
                    if (flag3)
                    {
                        if (Accuracy < 0)
                        {
                            Debugger.Break();
                        }
                        int num15 = listArray2[num10].PathCount - 1;
                        for (num12 = 0; num12 <= num15; num12++)
                        {
                            numArray2[num12] = Math.Min(Accuracy, listArray2[num10].Paths[num12].NodeCount - 1);
                        }
                        nodeArray[0] = listArray2[num10].Paths[0].Nodes[numArray2[0]];
                        num6 = 1;
                        flag = true;
                    }
                    else
                    {
                        if (Accuracy >= 0)
                        {
                            int num16 = listArray2[num10].PathCount - 1;
                            for (num12 = 0; num12 <= num16; num12++)
                            {
                                if (listArray2[num10].Paths[num12].NodeCount > Accuracy)
                                {
                                    flag3 = true;
                                    break;
                                }
                            }
                        }
                        nodeArray[0] = nodeArray2[num11];
                        if (num11 == index)
                        {
                            num6 = 1;
                        }
                        else
                        {
                            int num17 = nodeArray[0].ConnectionCount - 1;
                            num = 0;
                            while (num <= num17)
                            {
                                nodeArray[1 + num] = nodeArray[0].Connections[num].GetOtherNode(nodeArray[0]);
                                num++;
                            }
                            num6 = 1 + nodeArray[0].ConnectionCount;
                        }
                        int num18 = listArray2[num10].PathCount - 1;
                        for (num12 = 0; num12 <= num18; num12++)
                        {
                            numArray2[num12] = listArray2[num10].Paths[num12].NodeCount - 1;
                        }
                        flag = false;
                    }
                    int num19 = listArray2[num10].PathCount - 1;
                    for (num12 = 0; num12 <= num19; num12++)
                    {
                        int num20 = numArray2[num12];
                        num = 0;
                        while (num <= num20)
                        {
                            node = listArray2[num10].Paths[num12].Nodes[num];
                            int num21 = node.ConnectionCount - 1;
                            num5 = 0;
                            while (num5 <= num21)
                            {
                                otherNode = node.Connections[num5].GetOtherNode(node);
                                int num22 = otherNode.ConnectionCount - 1;
                                num7 = 0;
                                while (num7 <= num22)
                                {
                                    num3 = otherNode.Connections[num7].GetOtherNode(otherNode).Layer_NodeNum;
                                    flagArray2[num3] = false;
                                    num7++;
                                }
                                num5++;
                            }
                            num++;
                        }
                    }
                    int num23 = listArray2[num10].PathCount - 1;
                    num12 = 0;
                    while (num12 <= num23)
                    {
                        int num24 = numArray2[num12];
                        num = 0;
                        while (num <= num24)
                        {
                            node = listArray2[num10].Paths[num12].Nodes[num];
                            num3 = node.Layer_NodeNum;
                            flagArray2[num3] = true;
                            int num25 = node.NodeCount - 1;
                            num7 = 0;
                            while (num7 <= num25)
                            {
                                num3 = node.Nodes[num7].Layer_NodeNum;
                                numArray3[num3] = float.MaxValue;
                                num7++;
                            }
                            int num26 = node.ConnectionCount - 1;
                            for (num5 = 0; num5 <= num26; num5++)
                            {
                                otherNode = node.Connections[num5].GetOtherNode(node);
                                num3 = otherNode.Layer_NodeNum;
                                flagArray2[num3] = true;
                                int num27 = otherNode.NodeCount - 1;
                                for (num7 = 0; num7 <= num27; num7++)
                                {
                                    num3 = otherNode.Nodes[num7].Layer_NodeNum;
                                    numArray3[num3] = float.MaxValue;
                                }
                            }
                            num++;
                        }
                        num12++;
                    }
                    sFloodRouteArgs args = new sFloodRouteArgs {
                        CurrentPath = path,
                        FinishNodes = nodeArray,
                        FinishNodeCount = num6,
                        FinishIsParent = flag,
                        Visit = flagArray2,
                        NodeValues = numArray3,
                        SourceNodes = nodeArray4,
                        MinClearance = MinClearance
                    };
                    int num28 = num6 - 1;
                    for (num = 0; num <= num28; num++)
                    {
                        pathArray[num] = null;
                        numArray[num] = float.MaxValue;
                    }
                    int num29 = num13 - 1;
                    for (num = 0; num <= num29; num++)
                    {
                        if (flagArray[num])
                        {
                            path.NodeCount = 1;
                            path.Nodes[0] = nodeArray3[num11, num];
                            path.Value = 0f;
                            args.BestPaths = new Path[(num6 - 1) + 1];
                            this.FloodRoute(ref args);
                            int num30 = num6 - 1;
                            num12 = 0;
                            while (num12 <= num30)
                            {
                                if ((args.BestPaths[num12] != null) && (args.BestPaths[num12].Value < numArray[num12]))
                                {
                                    numArray[num12] = args.BestPaths[num12].Value;
                                    pathArray[num12] = args.BestPaths[num12];
                                }
                                num12++;
                            }
                        }
                    }
                    listArray2[num11].Paths = new Path[(num6 - 1) + 1];
                    listArray2[num11].PathCount = 0;
                    int num31 = num6 - 1;
                    for (num12 = 0; num12 <= num31; num12++)
                    {
                        if (pathArray[num12] != null)
                        {
                            listArray2[num11].Paths[listArray2[num11].PathCount] = pathArray[num12];
                            PathList[] listArray3 = listArray2;
                            int num32 = num11;
                            listArray3[num32].PathCount++;
                        }
                    }
                    listArray2[num11].Paths = (Path[]) Utils.CopyArray((Array) listArray2[num11].Paths, new Path[(listArray2[num11].PathCount - 1) + 1]);
                }
                while (listArray2[num11].PathCount != 0);
            }
            return null;
        }

        public void LargeArraysResize()
        {
            this.NetworkLargeArrays.Resize(this);
        }

        public bool NodeCanReachNode(PathfinderNode StartNode, PathfinderNode FinishNode)
        {
            PathfinderNode parentNode = StartNode;
            PathfinderNode node = FinishNode;
            do
            {
                if (parentNode == node)
                {
                    return true;
                }
                parentNode = parentNode.ParentNode;
                if (parentNode == null)
                {
                    return false;
                }
                node = node.ParentNode;
            }
            while (node != null);
            return false;
        }

        public void NodeLayer_Add(PathfinderLayer NewNodeLayer)
        {
            if (this.NodeLayerCount > 0)
            {
                this.NodeLayers[this.NodeLayerCount - 1].ParentLayer = NewNodeLayer;
            }
            this.NodeLayers = (PathfinderLayer[]) Utils.CopyArray((Array) this.NodeLayers, new PathfinderLayer[this.NodeLayerCount + 1]);
            this.NodeLayers[this.NodeLayerCount] = NewNodeLayer;
            this.NodeLayers[this.NodeLayerCount].Network_LayerNum = this.NodeLayerCount;
            this.NodeLayerCount++;
        }

        public PathfinderLayer this[int Num]
        {
            get
            {
                return this.NodeLayers[Num];
            }
        }

        public int GetNodeLayerCount
        {
            get
            {
                return this.NodeLayerCount;
            }
        }

        public class LargeArrays
        {
            public bool[] Nodes_Booleans;
            public PathfinderNode[] Nodes_Nodes;
            public PathfinderNetwork.Path Nodes_Path = new PathfinderNetwork.Path();
            public float[] Nodes_ValuesA;
            public float[] Nodes_ValuesB;
            public int Size;
            public float SizeEnlargementRatio = 2f;
            public float SizeReductionRatio = 3f;

            public void Resize(PathfinderNetwork NetworkForSize)
            {
                int nodeCount;
                if (NetworkForSize.NodeLayerCount > 0)
                {
                    nodeCount = NetworkForSize.NodeLayers[0].NodeCount;
                }
                else
                {
                    nodeCount = 0;
                }
                if (this.Size < nodeCount)
                {
                    this.Size = (int) Math.Round((double) (nodeCount * this.SizeEnlargementRatio));
                    int num2 = this.Size - 1;
                    this.Nodes_Booleans = new bool[num2 + 1];
                    this.Nodes_ValuesA = new float[num2 + 1];
                    this.Nodes_ValuesB = new float[num2 + 1];
                    this.Nodes_Booleans = new bool[num2 + 1];
                    this.Nodes_Path.Nodes = new PathfinderNode[num2 + 1];
                    this.Nodes_Nodes = new PathfinderNode[num2 + 1];
                }
                else if (this.Size > (nodeCount * this.SizeReductionRatio))
                {
                    this.Size = (int) Math.Round((double) (nodeCount * this.SizeEnlargementRatio));
                    int num3 = this.Size - 1;
                    this.Nodes_Booleans = new bool[num3 + 1];
                    this.Nodes_ValuesA = new float[num3 + 1];
                    this.Nodes_ValuesB = new float[num3 + 1];
                    this.Nodes_Booleans = new bool[num3 + 1];
                    this.Nodes_Path.Nodes = new PathfinderNode[num3 + 1];
                    this.Nodes_Nodes = new PathfinderNode[num3 + 1];
                }
            }
        }

        public class Path
        {
            public int NodeCount;
            public PathfinderNode[] Nodes = new PathfinderNode[0];
            public float Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PathList
        {
            public PathfinderNetwork.Path[] Paths;
            public int PathCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sFloodForValuesArgs
        {
            public PathfinderNetwork.Path CurrentPath;
            public PathfinderNode SourceParentNodeA;
            public PathfinderNode SourceParentNodeB;
            public PathfinderNode[] FinishNodes;
            public int FinishNodeCount;
            public bool FinishIsParent;
            public float[] NodeValues;
            public PathfinderNode[] SourceNodes;
            public PathfinderNetwork.Path[] BestPaths;
            public int MinClearance;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sFloodProximityArgs
        {
            public PathfinderNode StartNode;
            public float[] NodeValues;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sFloodRouteAllArgs
        {
            public PathfinderNode[] StartNodes;
            public int StartNodeCount;
            public PathfinderNode FinishNode;
            public bool[] Visit;
            public float[] NodeValuesA;
            public float[] NodeValuesB;
            public PathfinderNode[] SourceNodes;
            public float BestTolerance;
            public int BestNodeCount;
            public int MinClearance;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sFloodRouteArgs
        {
            public PathfinderNetwork.Path CurrentPath;
            public PathfinderNode[] FinishNodes;
            public int FinishNodeCount;
            public bool FinishIsParent;
            public bool[] Visit;
            public float[] NodeValues;
            public PathfinderNode[] SourceNodes;
            public PathfinderNetwork.Path[] BestPaths;
            public int MinClearance;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sFloodSpanArgs
        {
            public PathfinderNetwork.Path CurrentPath;
            public PathfinderNode SourceParentNode;
            public PathfinderNode[] FinishNodes;
            public int FinishNodeCount;
            public bool FinishIsParent;
            public float[] NodeValues;
            public PathfinderNode[] SourceNodes;
            public PathfinderNetwork.Path[] BestPaths;
            public int MinClearance;
        }
    }
}

