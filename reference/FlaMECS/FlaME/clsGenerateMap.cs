namespace FlaME
{
    using Matrix3D;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class clsGenerateMap
    {
        public int BaseFlatArea;
        public int BaseLevel;
        public int BaseOilCount;
        public int BaseTruckCount;
        private int ConnectionCount;
        private clsConnection[] Connections;
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
        public clsMap Map;
        public float MaxDisconnectionDist;
        public int MaxLevelTransition;
        private int NearestCount;
        private clsNearest[] Nearests;
        public float NodeScale;
        public int OilAtATime;
        public float OilDispersion;
        private int PassageNodeCount;
        private float[,,,] PassageNodeDists;
        private clsPassageNode[,] PassageNodes;
        public int PassagesChance;
        public modMath.sXY_int[] PlayerBasePos;
        private sPlayerBase[] PlayerBases;
        public double RampBase;
        public int SymmetryBlockCount;
        public modMath.sXY_int SymmetryBlockCountXY;
        public sSymmetryBlock[] SymmetryBlocks;
        public bool SymmetryIsRotational;
        public PathfinderNetwork TilePathMap;
        public modMath.sXY_int TileSize;
        public int TopLeftPlayerCount;
        private int TotalPlayerCount;
        public int TotalWaterQuantity;
        public int VariationChance;
        public PathfinderNetwork VertexPathMap;
        public int WaterSpawnQuantity;

        public void BlockEdgeTiles()
        {
            int num;
            clsMap.clsTerrain terrain = this.Map.Terrain;
            int num3 = terrain.TileSize.Y - 1;
            int y = 0;
            while (y <= num3)
            {
                num = 0;
                do
                {
                    this.TileNodeBlock(num, y);
                    num++;
                }
                while (num <= 2);
                int num4 = terrain.TileSize.X - 1;
                num = terrain.TileSize.X - 4;
                while (num <= num4)
                {
                    this.TileNodeBlock(num, y);
                    num++;
                }
                y++;
            }
            int num5 = terrain.TileSize.X - 5;
            for (num = 3; num <= num5; num++)
            {
                y = 0;
                do
                {
                    this.TileNodeBlock(num, y);
                    y++;
                }
                while (y <= 2);
                int num6 = terrain.TileSize.Y - 1;
                for (y = terrain.TileSize.Y - 4; y <= num6; y++)
                {
                    this.TileNodeBlock(num, y);
                }
            }
            this.TilePathMap.FindCalc();
        }

        public void CalcNodePos(PathfinderNode Node, ref Position.XY_dbl Pos, ref int SampleCount)
        {
            if (Node.GetLayer.GetNetwork_LayerNum == 0)
            {
                clsNodeTag tag = (clsNodeTag) Node.Tag;
                Pos.X += tag.Pos.X;
                Pos.Y += tag.Pos.Y;
            }
            else
            {
                int num2 = Node.GetChildNodeCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    this.CalcNodePos(Node.get_GetChildNode(i), ref Pos, ref SampleCount);
                }
                SampleCount += Node.GetChildNodeCount;
            }
        }

        private bool CheckRampAngles(clsConnection NewRampConnection, double MinSpacingAngle, double MinSpacingAngle2, double MinPassageSpacingAngle)
        {
            double num;
            double angle;
            modMath.sXY_int _int;
            _int.X = NewRampConnection.PassageNodeB.Pos.X - NewRampConnection.PassageNodeA.Pos.X;
            _int.Y = NewRampConnection.PassageNodeB.Pos.Y - NewRampConnection.PassageNodeA.Pos.Y;
            if (NewRampConnection.PassageNodeA.Connections[NewRampConnection.PassageNodeA_ConnectionNum].IsB)
            {
                angle = _int.ToDoubles().GetAngle();
                num = modMath.AngleClamp(angle - 3.1415926535897931);
            }
            else
            {
                num = _int.ToDoubles().GetAngle();
                angle = modMath.AngleClamp(angle - 3.1415926535897931);
            }
            if (!this.CheckRampNodeRampAngles(NewRampConnection.PassageNodeA, NewRampConnection.PassageNodeB, num, MinSpacingAngle, MinSpacingAngle2))
            {
                return false;
            }
            if (!this.CheckRampNodeRampAngles(NewRampConnection.PassageNodeB, NewRampConnection.PassageNodeA, angle, MinSpacingAngle, MinSpacingAngle2))
            {
                return false;
            }
            if (!this.CheckRampNodeLevelAngles(NewRampConnection.PassageNodeA, num, MinPassageSpacingAngle))
            {
                return false;
            }
            if (!this.CheckRampNodeLevelAngles(NewRampConnection.PassageNodeB, angle, MinPassageSpacingAngle))
            {
                return false;
            }
            return true;
        }

        private bool CheckRampNodeLevelAngles(clsPassageNode RampPassageNode, double RampAwayAngle, double MinSpacingAngle)
        {
            bool flag2 = this.PassageNodeHasRamp(RampPassageNode);
            int num3 = RampPassageNode.ConnectionCount - 1;
            for (int i = 0; i <= num3; i++)
            {
                clsConnection connection = RampPassageNode.Connections[i].Connection;
                clsPassageNode other = RampPassageNode.Connections[i].GetOther();
                if (other.Level == RampPassageNode.Level)
                {
                    int num2;
                    bool flag3 = true;
                    if (i == 0)
                    {
                        num2 = RampPassageNode.ConnectionCount - 1;
                    }
                    else
                    {
                        num2 = i - 1;
                    }
                    if (num2 != i)
                    {
                        if (RampPassageNode.Connections[num2].GetOther().Level == other.Level)
                        {
                            flag3 = false;
                        }
                    }
                    else
                    {
                        flag3 = false;
                    }
                    if (i == (RampPassageNode.ConnectionCount - 1))
                    {
                        num2 = 0;
                    }
                    else
                    {
                        num2 = i + 1;
                    }
                    if (num2 != i)
                    {
                        if (RampPassageNode.Connections[num2].GetOther().Level == other.Level)
                        {
                            flag3 = false;
                        }
                    }
                    else
                    {
                        flag3 = false;
                    }
                    if (flag3)
                    {
                        modMath.sXY_int _int;
                        _int.X = other.Pos.X - RampPassageNode.Pos.X;
                        _int.Y = other.Pos.Y - RampPassageNode.Pos.Y;
                        if (Math.Abs(modMath.AngleClamp(_int.ToDoubles().GetAngle() - RampAwayAngle)) < MinSpacingAngle)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CheckRampNodeRampAngles(clsPassageNode RampPassageNode, clsPassageNode OtherRampPassageNode, double RampAwayAngle, double MinSpacingAngle, double MinSpacingAngle2)
        {
            int num4 = RampPassageNode.ConnectionCount - 1;
            for (int i = 0; i <= num4; i++)
            {
                if (RampPassageNode.Connections[i].Connection.IsRamp)
                {
                    modMath.sXY_int _int;
                    clsPassageNode other = RampPassageNode.Connections[i].GetOther();
                    _int.X = other.Pos.X - RampPassageNode.Pos.X;
                    _int.Y = other.Pos.Y - RampPassageNode.Pos.Y;
                    double num3 = Math.Abs(modMath.AngleClamp(RampAwayAngle - _int.ToDoubles().GetAngle()));
                    int num2 = Math.Abs((int) (other.Level - OtherRampPassageNode.Level));
                    if (num2 < 2)
                    {
                        if (num2 != 0)
                        {
                            Debugger.Break();
                            return false;
                        }
                        if (num3 < MinSpacingAngle)
                        {
                            return false;
                        }
                    }
                    else if (num3 < MinSpacingAngle2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void ClearLayout()
        {
            int num;
            if (this.TilePathMap != null)
            {
                this.TilePathMap.Deallocate();
                this.TilePathMap = null;
            }
            if (this.VertexPathMap != null)
            {
                this.VertexPathMap.Deallocate();
                this.VertexPathMap = null;
            }
            int num3 = this.ConnectionCount - 1;
            for (num = 0; num <= num3; num++)
            {
                this.Connections[num].PassageNodeA = null;
                this.Connections[num].PassageNodeB = null;
                this.Connections[num].Reflections = null;
            }
            this.ConnectionCount = 0;
            int num4 = this.PassageNodeCount - 1;
            for (num = 0; num <= num4; num++)
            {
                int num5 = this.SymmetryBlockCount - 1;
                for (int i = 0; i <= num5; i++)
                {
                    this.PassageNodes[i, num].Connections = null;
                }
            }
            this.PassageNodeCount = 0;
            this.NearestCount = 0;
        }

        public modProgram.sResult GenerateGateways()
        {
            int num5;
            modProgram.sResult result2;
            int num6;
            int x;
            result2.Success = false;
            result2.Problem = "";
            clsMap.clsTerrain terrain = this.Map.Terrain;
            sPossibleGateway[] gatewayArray = new sPossibleGateway[(((terrain.TileSize.X * terrain.TileSize.Y) * 2) - 1) + 1];
            int num10 = terrain.TileSize.Y - 1;
            int y = 0;
            while (y <= num10)
            {
                num6 = 0;
                int num11 = terrain.TileSize.X - 1;
                for (x = 0; x <= num11; x++)
                {
                    if (this.GenerateTerrainTiles[x, y].Node.GetClearance >= 1)
                    {
                        if (this.GenerateTerrainTiles[x, y].Node.GetClearance == 1)
                        {
                            if ((num6 > 3) & (num6 <= 9))
                            {
                                gatewayArray[num5].StartPos.X = x - num6;
                                gatewayArray[num5].StartPos.Y = y;
                                gatewayArray[num5].Length = num6 + 1;
                                gatewayArray[num5].IsVertical = false;
                                gatewayArray[num5].MiddlePos.X = (gatewayArray[num5].StartPos.X * 0x80) + (gatewayArray[num5].Length * 0x40);
                                gatewayArray[num5].MiddlePos.Y = gatewayArray[num5].StartPos.Y * 0x80;
                                num5++;
                            }
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                    }
                }
                y++;
            }
            int num12 = terrain.TileSize.X - 1;
            x = 0;
            while (x <= num12)
            {
                num6 = 0;
                y = 0;
                while (y < terrain.TileSize.Y)
                {
                    if (this.GenerateTerrainTiles[x, y].Node.GetClearance >= 1)
                    {
                        if (this.GenerateTerrainTiles[x, y].Node.GetClearance == 1)
                        {
                            if ((num6 >= 3) & (num6 <= 9))
                            {
                                gatewayArray[num5].StartPos.X = x;
                                gatewayArray[num5].StartPos.Y = y - num6;
                                gatewayArray[num5].Length = num6 + 1;
                                gatewayArray[num5].IsVertical = true;
                                gatewayArray[num5].MiddlePos.X = gatewayArray[num5].StartPos.X * 0x80;
                                gatewayArray[num5].MiddlePos.Y = (gatewayArray[num5].StartPos.Y * 0x80) + (gatewayArray[num5].Length * 0x40);
                                num5++;
                            }
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                    }
                    y++;
                }
                x++;
            }
            bool[,] flagArray = new bool[(terrain.TileSize.X - 1) + 1, (terrain.TileSize.Y - 1) + 1];
            while (num5 > 0)
            {
                modMath.sXY_int _int2;
                int index = -1;
                float maxValue = float.MaxValue;
                int num13 = num5 - 1;
                int num = 0;
                while (num <= num13)
                {
                    float length = gatewayArray[num].Length;
                    if (length < maxValue)
                    {
                        maxValue = length;
                        index = num;
                    }
                    num++;
                }
                if (gatewayArray[index].IsVertical)
                {
                    _int2 = new modMath.sXY_int(gatewayArray[index].StartPos.X, (gatewayArray[index].StartPos.Y + gatewayArray[index].Length) - 1);
                    this.Map.GatewayCreateStoreChange(gatewayArray[index].StartPos, _int2);
                    int num14 = (gatewayArray[index].StartPos.Y + gatewayArray[index].Length) - 1;
                    y = gatewayArray[index].StartPos.Y;
                    while (y <= num14)
                    {
                        flagArray[gatewayArray[index].StartPos.X, y] = true;
                        y++;
                    }
                }
                else
                {
                    _int2 = new modMath.sXY_int((gatewayArray[index].StartPos.X + gatewayArray[index].Length) - 1, gatewayArray[index].StartPos.Y);
                    this.Map.GatewayCreateStoreChange(gatewayArray[index].StartPos, _int2);
                    int num15 = (gatewayArray[index].StartPos.X + gatewayArray[index].Length) - 1;
                    x = gatewayArray[index].StartPos.X;
                    while (x <= num15)
                    {
                        flagArray[x, gatewayArray[index].StartPos.Y] = true;
                        x++;
                    }
                }
                modMath.sXY_int middlePos = gatewayArray[index].MiddlePos;
                double num4 = gatewayArray[index].Length * 0x80;
                num = 0;
                while (num < num5)
                {
                    bool flag = true;
                    if (gatewayArray[num].IsVertical)
                    {
                        int num16 = (gatewayArray[num].StartPos.Y + gatewayArray[num].Length) - 1;
                        for (y = gatewayArray[num].StartPos.Y; y <= num16; y++)
                        {
                            if (flagArray[gatewayArray[num].StartPos.X, y])
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        int num17 = (gatewayArray[num].StartPos.X + gatewayArray[num].Length) - 1;
                        for (x = gatewayArray[num].StartPos.X; x <= num17; x++)
                        {
                            if (flagArray[x, gatewayArray[num].StartPos.Y])
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        _int2 = middlePos - gatewayArray[num].MiddlePos;
                        if (_int2.ToDoubles().GetMagnitude() < num4)
                        {
                            flag = false;
                        }
                    }
                    if (!flag)
                    {
                        num5--;
                        if (num != num5)
                        {
                            gatewayArray[num] = gatewayArray[num5];
                        }
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            result2.Success = true;
            return result2;
        }

        public clsResult GenerateLayout()
        {
            int num;
            int num2;
            int level;
            int num5;
            bool flag;
            int num6;
            int baseLevel;
            int num9;
            modMath.sXY_int _int;
            Position.XY_dbl _dbl;
            bool flag2;
            int num12;
            int num13;
            int num14;
            clsNearest nearest;
            modProgram.sResult result3;
            modMath.sXY_int _int4;
            clsPassageNode node;
            clsPassageNode other;
            clsPassageNode node3;
            modMath.sXY_int _int5;
            int num41;
            int[] mapLevelCount;
            clsResult result4 = new clsResult("Layout");
            this.TotalPlayerCount = this.TopLeftPlayerCount * this.SymmetryBlockCount;
            _int4.X = (int) Math.Round((double) (((double) (this.TileSize.X * 0x80)) / ((double) this.SymmetryBlockCountXY.X)));
            _int4.Y = (int) Math.Round((double) (((double) (this.TileSize.Y * 0x80)) / ((double) this.SymmetryBlockCountXY.Y)));
            int num19 = (int) Math.Round((double) (128f * this.NodeScale));
            int num16 = (int) Math.Round(Math.Ceiling((double) (((((2.0 * this.TileSize.X) * 128.0) * this.TileSize.Y) * 128.0) / ((3.1415926535897931 * num19) * num19))));
            this.PassageNodes = new clsPassageNode[(this.SymmetryBlockCount - 1) + 1, (num16 - 1) + 1];
            int x = 0;
            if (this.SymmetryBlockCountXY.X == 1)
            {
                _int.X = (int) Math.Round(((double) (((this.TileSize.X * 0x80) - (x * 2.0)) / ((double) ((this.NodeScale * 128f) * 2f)))));
                _dbl.X = ((this.TileSize.X * 0x80) - (x * 2.0)) / ((double) _int.X);
                _int.X--;
            }
            else
            {
                _int.X = (int) Math.Round(((double) ((((((double) (this.TileSize.X * 0x80)) / ((double) this.SymmetryBlockCountXY.X)) - x) / ((double) ((this.NodeScale * 128f) * 2f))) - 0.5)));
                _dbl.X = ((((double) (this.TileSize.X * 0x80)) / ((double) this.SymmetryBlockCountXY.X)) - x) / (((double) ((((((double) (this.TileSize.X * 0x80)) / ((double) this.SymmetryBlockCountXY.X)) - x) / ((double) ((this.NodeScale * 128f) * 2f))) - 0.5)) + 0.5);
            }
            if (this.SymmetryBlockCountXY.Y == 1)
            {
                _int.Y = (int) Math.Round(((double) (((this.TileSize.Y * 0x80) - (x * 2.0)) / ((double) ((this.NodeScale * 128f) * 2f)))));
                _dbl.Y = ((this.TileSize.Y * 0x80) - (x * 2.0)) / ((double) _int.Y);
                _int.Y--;
            }
            else
            {
                _int.Y = (int) Math.Round(((double) ((((((double) (this.TileSize.Y * 0x80)) / ((double) this.SymmetryBlockCountXY.Y)) - x) / ((double) ((this.NodeScale * 128f) * 2f))) - 0.5)));
                _dbl.Y = ((((double) (this.TileSize.Y * 0x80)) / ((double) this.SymmetryBlockCountXY.Y)) - x) / (((double) ((((((double) (this.TileSize.Y * 0x80)) / ((double) this.SymmetryBlockCountXY.Y)) - x) / ((double) ((this.NodeScale * 128f) * 2f))) - 0.5)) + 0.5);
            }
            this.PassageNodeCount = 0;
            int y = _int.Y;
            for (int i = 0; i <= y; i++)
            {
                _int5 = new modMath.sXY_int(x, x + ((int) Math.Round((double) (i * _dbl.Y))));
                if (!this.MakePassageNodes(_int5, true))
                {
                    result4.ProblemAdd("Error: Bad border node.");
                    return result4;
                }
                if (this.SymmetryBlockCountXY.X == 1)
                {
                    _int5 = new modMath.sXY_int((this.TileSize.X * 0x80) - x, x + ((int) Math.Round((double) (i * _dbl.Y))));
                    if (!this.MakePassageNodes(_int5, true))
                    {
                        result4.ProblemAdd("Error: Bad border node.");
                        return result4;
                    }
                }
            }
            int num26 = _int.X;
            for (int j = 1; j <= num26; j++)
            {
                _int5 = new modMath.sXY_int(x + ((int) Math.Round((double) (j * _dbl.X))), x);
                if (!this.MakePassageNodes(_int5, true))
                {
                    result4.ProblemAdd("Error: Bad border node.");
                    return result4;
                }
                if (this.SymmetryBlockCountXY.Y == 1)
                {
                    _int5 = new modMath.sXY_int(x + ((int) Math.Round((double) (j * _dbl.X))), (this.TileSize.Y * 0x80) - x);
                    if (!this.MakePassageNodes(_int5, true))
                    {
                        result4.ProblemAdd("Error: Bad border node.");
                        return result4;
                    }
                }
            }
        Label_0538:
            num14 = 0;
            do
            {
                modMath.sXY_int _int2;
                if (this.SymmetryBlockCountXY.X == 1)
                {
                    _int2.X = x + ((int) Math.Round((double) ((float) (App.Random.Next() * ((_int4.X - (x * 2)) + 1)))));
                }
                else
                {
                    _int2.X = x + ((int) Math.Round((double) ((float) (App.Random.Next() * ((_int4.X - x) + 1)))));
                }
                if (this.SymmetryBlockCountXY.Y == 1)
                {
                    _int2.Y = x + ((int) Math.Round((double) ((float) (App.Random.Next() * ((_int4.Y - (x * 2)) + 1)))));
                }
                else
                {
                    _int2.Y = x + ((int) Math.Round((double) ((float) (App.Random.Next() * ((_int4.Y - x) + 1)))));
                }
                int num27 = this.PassageNodeCount - 1;
                num = 0;
                while (num <= num27)
                {
                    int num28 = this.SymmetryBlockCount - 1;
                    num2 = 0;
                    while (num2 <= num28)
                    {
                        _int5 = this.PassageNodes[num2, num].Pos - _int2;
                        if (_int5.ToDoubles().GetMagnitude() < (num19 * 2))
                        {
                            break;
                        }
                        num2++;
                    }
                    num++;
                }
                if ((num == this.PassageNodeCount) && this.MakePassageNodes(_int2, false))
                {
                    goto Label_0538;
                }
                num14++;
            }
            while (num14 < ((int) Math.Round((double) (((64f * this.TileSize.X) * this.TileSize.Y) / (this.NodeScale * this.NodeScale)))));
            this.PassageNodes = (clsPassageNode[,]) Utils.CopyArray((Array) this.PassageNodes, new clsPassageNode[(this.SymmetryBlockCount - 1) + 1, (this.PassageNodeCount - 1) + 1]);
            int num15 = (num19 * 2) * 4;
            num15 *= num15;
            this.Nearests = new clsNearest[((this.PassageNodeCount * 0x40) - 1) + 1];
            clsTestNearestArgs args2 = new clsTestNearestArgs();
            int num17 = (int) Math.Round((double) ((this.NodeScale * 1.25f) * 128f));
            args2.MaxConDist2 = num15;
            args2.MinConDist = num17;
            int num29 = this.PassageNodeCount - 1;
            num = 0;
            while (num <= num29)
            {
                args2.PassageNodeA = this.PassageNodes[0, num];
                int num30 = this.PassageNodeCount - 1;
                num2 = num;
                while (num2 <= num30)
                {
                    int num31 = this.SymmetryBlockCount - 1;
                    num5 = 0;
                    while (num5 <= num31)
                    {
                        args2.PassageNodeB = this.PassageNodes[num5, num2];
                        if (args2.PassageNodeA != args2.PassageNodeB)
                        {
                            this.TestNearest(args2);
                        }
                        num5++;
                    }
                    num2++;
                }
                num++;
            }
            int num32 = this.NearestCount - 1;
            for (num12 = 0; num12 <= num32; num12++)
            {
                nearest = this.Nearests[num12];
                int num33 = nearest.NodeCount - 1;
                num = 0;
                while (num <= num33)
                {
                    node = nearest.NodeA[num];
                    other = nearest.NodeB[num];
                    int num34 = this.NearestCount - 1;
                    num13 = 0;
                    while (num13 <= num34)
                    {
                        clsNearest nearest2 = this.Nearests[num13];
                        if (nearest2 != nearest)
                        {
                            if (nearest2.Dist2 < nearest.Dist2)
                            {
                                flag2 = true;
                            }
                            else if (nearest2.Dist2 == nearest.Dist2)
                            {
                                flag2 = nearest.Num > nearest2.Num;
                            }
                            else
                            {
                                flag2 = false;
                            }
                            if (flag2)
                            {
                                int num35 = nearest2.NodeCount - 1;
                                num2 = 0;
                                while (num2 <= num35)
                                {
                                    if (!((((node == nearest2.NodeA[num2]) | (node == nearest2.NodeB[num2])) | (other == nearest2.NodeA[num2])) | (other == nearest2.NodeB[num2])) && modMath.GetLinesIntersectBetween(node.Pos, other.Pos, nearest2.NodeA[num2].Pos, nearest2.NodeB[num2].Pos).Exists)
                                    {
                                        break;
                                    }
                                    num2++;
                                }
                                if (num2 < nearest2.NodeCount)
                                {
                                    clsNearest nearest3 = nearest;
                                    nearest3.BlockedCount++;
                                    clsNearest nearest4 = nearest2;
                                    nearest4.BlockedNearests[nearest4.BlockedNearestCount] = nearest;
                                    nearest3 = nearest4;
                                    nearest3.BlockedNearestCount++;
                                    nearest4 = null;
                                }
                            }
                        }
                        num13++;
                    }
                    num++;
                }
            }
            this.Connections = new clsConnection[((this.PassageNodeCount * 0x10) - 1) + 1];
            do
            {
                num6 = 0;
                num12 = 0;
                while (num12 < this.NearestCount)
                {
                    nearest = this.Nearests[num12];
                    flag2 = true;
                    if ((nearest.BlockedCount == 0) & flag2)
                    {
                        int connectionCount = this.ConnectionCount;
                        int num36 = nearest.NodeCount - 1;
                        baseLevel = 0;
                        while (baseLevel <= num36)
                        {
                            this.Connections[this.ConnectionCount] = new clsConnection(nearest.NodeA[baseLevel], nearest.NodeB[baseLevel]);
                            this.ConnectionCount++;
                            baseLevel++;
                        }
                        int num37 = nearest.NodeCount - 1;
                        baseLevel = 0;
                        while (baseLevel <= num37)
                        {
                            num = connectionCount + baseLevel;
                            this.Connections[num].ReflectionCount = nearest.NodeCount - 1;
                            this.Connections[num].Reflections = new clsConnection[(this.Connections[num].ReflectionCount - 1) + 1];
                            num2 = 0;
                            int num38 = nearest.NodeCount - 1;
                            num9 = 0;
                            while (num9 <= num38)
                            {
                                if (num9 != baseLevel)
                                {
                                    this.Connections[num].Reflections[num2] = this.Connections[connectionCount + num9];
                                    num2++;
                                }
                                num9++;
                            }
                            baseLevel++;
                        }
                        int num39 = nearest.BlockedNearestCount - 1;
                        num5 = 0;
                        while (num5 <= num39)
                        {
                            nearest.BlockedNearests[num5].Invalid = true;
                            num5++;
                        }
                        this.NearestCount--;
                        num13 = nearest.Num;
                        nearest.Num = -1;
                        if (num13 != this.NearestCount)
                        {
                            this.Nearests[num13] = this.Nearests[this.NearestCount];
                            this.Nearests[num13].Num = num13;
                        }
                        num6++;
                    }
                    else
                    {
                        if (!flag2)
                        {
                            nearest.Invalid = true;
                        }
                        num12++;
                    }
                }
                num12 = 0;
                while (num12 < this.NearestCount)
                {
                    nearest = this.Nearests[num12];
                    if (nearest.Invalid)
                    {
                        nearest.Num = -1;
                        int num40 = nearest.BlockedNearestCount - 1;
                        baseLevel = 0;
                        while (baseLevel <= num40)
                        {
                            clsNearest[] blockedNearests = nearest.BlockedNearests;
                            num41 = baseLevel;
                            blockedNearests[num41].BlockedCount--;
                            baseLevel++;
                        }
                        this.NearestCount--;
                        if (num12 != this.NearestCount)
                        {
                            this.Nearests[num12] = this.Nearests[this.NearestCount];
                            this.Nearests[num12].Num = num12;
                        }
                    }
                    else
                    {
                        num12++;
                    }
                }
            }
            while (num6 > 0);
            int num42 = this.PassageNodeCount - 1;
            for (num = 0; num <= num42; num++)
            {
                int num43 = this.SymmetryBlockCount - 1;
                for (num2 = 0; num2 <= num43; num2++)
                {
                    this.PassageNodes[num2, num].ReorderConnections();
                    this.PassageNodes[num2, num].CalcIsNearBorder();
                }
            }
            clsPassageNode[] nodeArray2 = new clsPassageNode[(this.PassageNodeCount - 1) + 1];
            int index = 0;
            clsPassageNode[] nodeArray3 = new clsPassageNode[(this.PassageNodeCount - 1) + 1];
            int num44 = this.PassageNodeCount - 1;
            num = 0;
            while (num <= num44)
            {
                nodeArray2[index] = this.PassageNodes[0, num];
                index++;
                num++;
            }
            num2 = 0;
            while (index > 0)
            {
                num = (int) Math.Round((double) ((float) (App.Random.Next() * index)));
                nodeArray3[num2] = nodeArray2[num];
                num2++;
                index--;
                nodeArray2[num] = nodeArray2[index];
            }
            this.LevelHeight = 255f / ((float) (this.LevelCount - 1));
            clsPassageNodeHeightLevelArgs args = new clsPassageNodeHeightLevelArgs {
                PassageNodesMinLevel = { Nodes = new int[(this.PassageNodeCount - 1) + 1] },
                PassageNodesMaxLevel = { Nodes = new int[(this.PassageNodeCount - 1) + 1] },
                MapLevelCount = new int[(this.LevelCount - 1) + 1]
            };
            int num45 = this.PassageNodeCount - 1;
            num = 0;
            while (num <= num45)
            {
                args.PassageNodesMinLevel.Nodes[num] = 0;
                args.PassageNodesMaxLevel.Nodes[num] = this.LevelCount - 1;
                num++;
            }
            double[] numArray = new double[(this.BaseFlatArea - 1) + 1];
            clsPassageNode[] nodeArray = new clsPassageNode[(this.BaseFlatArea - 1) + 1];
            int[] numArray2 = new int[(this.BaseFlatArea - 1) + 1];
            this.PlayerBases = new sPlayerBase[(this.TotalPlayerCount - 1) + 1];
            int num46 = this.TopLeftPlayerCount - 1;
            num2 = 0;
            while (num2 <= num46)
            {
                int num3 = 0;
                int num47 = this.PassageNodeCount - 1;
                num = 0;
                while (num <= num47)
                {
                    int num48 = this.SymmetryBlockCount - 1;
                    num9 = 0;
                    while (num9 <= num48)
                    {
                        node = this.PassageNodes[num9, num];
                        if (!node.IsOnBorder)
                        {
                            _int5 = node.Pos - this.PlayerBasePos[num2];
                            double magnitude = _int5.ToDoubles().GetMagnitude();
                            num5 = num3 - 1;
                            while (num5 >= 0)
                            {
                                if (magnitude > numArray[num5])
                                {
                                    break;
                                }
                                num5 += -1;
                            }
                            num5++;
                            int num49 = num5;
                            baseLevel = Math.Min((int) (num3 - 1), (int) (this.BaseFlatArea - 2));
                            while (baseLevel >= num49)
                            {
                                numArray[baseLevel + 1] = numArray[baseLevel];
                                nodeArray[baseLevel + 1] = nodeArray[baseLevel];
                                baseLevel += -1;
                            }
                            if (num5 < this.BaseFlatArea)
                            {
                                numArray[num5] = magnitude;
                                nodeArray[num5] = node;
                                num3 = Math.Max(num3, num5 + 1);
                            }
                        }
                        num9++;
                    }
                    num++;
                }
                if (this.BaseLevel < 0)
                {
                    baseLevel = (int) Math.Round((double) ((float) (App.Random.Next() * this.LevelCount)));
                }
                else
                {
                    baseLevel = this.BaseLevel;
                }
                mapLevelCount = args.MapLevelCount;
                num41 = baseLevel;
                mapLevelCount[num41] += num3;
                int num50 = num3 - 1;
                num = 0;
                while (num <= num50)
                {
                    if (nodeArray[num].MirrorNum == 0)
                    {
                        numArray2[num] = -1;
                    }
                    else
                    {
                        int num51 = ((int) Math.Round((double) (((double) this.SymmetryBlockCount) / 2.0))) - 1;
                        num5 = 0;
                        while (num5 <= num51)
                        {
                            if (this.SymmetryBlocks[0].ReflectToNum[num5] == nodeArray[num].MirrorNum)
                            {
                                break;
                            }
                            num5++;
                        }
                        numArray2[num] = num5;
                    }
                    num++;
                }
                int num52 = this.SymmetryBlockCount - 1;
                num = 0;
                while (num <= num52)
                {
                    num9 = (num * this.TopLeftPlayerCount) + num2;
                    this.PlayerBases[num9].NodeCount = num3;
                    this.PlayerBases[num9].Nodes = new clsPassageNode[(this.PlayerBases[num9].NodeCount - 1) + 1];
                    int num53 = num3 - 1;
                    num5 = 0;
                    while (num5 <= num53)
                    {
                        if (numArray2[num5] < 0)
                        {
                            this.PlayerBases[num9].Nodes[num5] = this.PassageNodes[num, nodeArray[num5].Num];
                        }
                        else
                        {
                            this.PlayerBases[num9].Nodes[num5] = this.PassageNodes[this.SymmetryBlocks[num].ReflectToNum[numArray2[num5]], nodeArray[num5].Num];
                        }
                        this.PlayerBases[num9].Nodes[num5].PlayerBaseNum = num9;
                        this.PlayerBases[num9].Nodes[num5].Level = baseLevel;
                        this.PassageNodesMinLevelSet(this.PlayerBases[num9].Nodes[num5], args.PassageNodesMinLevel, baseLevel, this.MaxLevelTransition);
                        this.PassageNodesMaxLevelSet(this.PlayerBases[num9].Nodes[num5], args.PassageNodesMaxLevel, baseLevel, this.MaxLevelTransition);
                        num5++;
                    }
                    _int5 = new modMath.sXY_int(_int4.X - 1, _int4.Y - 1);
                    modMath.sXY_int _int3 = TileOrientation.GetRotatedPos(this.SymmetryBlocks[num].Orientation, this.PlayerBasePos[num2], _int5);
                    this.PlayerBases[num9].Pos.X = (this.SymmetryBlocks[num].XYNum.X * _int4.X) + _int3.X;
                    this.PlayerBases[num9].Pos.Y = (this.SymmetryBlocks[num].XYNum.Y * _int4.Y) + _int3.Y;
                    num++;
                }
                num2++;
            }
            int num54 = this.PassageNodeCount - 1;
            for (num = 0; num <= num54; num++)
            {
                node = nodeArray3[num];
                if ((node.Level < 0) & !node.IsOnBorder)
                {
                    int num20;
                    int num22;
                    int num21 = 0;
                    int num55 = node.ConnectionCount - 1;
                    num2 = 0;
                    while (num2 <= num55)
                    {
                        if (node.Connections[num2].GetOther().IsWater)
                        {
                            num21++;
                        }
                        num2++;
                    }
                    flag = true;
                    int num56 = node.ConnectionCount - 1;
                    num2 = 0;
                    while (num2 <= num56)
                    {
                        if (args.PassageNodesMinLevel.Nodes[node.Connections[num2].GetOther().Num] > 0)
                        {
                            flag = false;
                        }
                        num2++;
                    }
                    if (((flag & (((num21 == 0) & (num22 < this.WaterSpawnQuantity)) | ((num21 == 1) & ((this.TotalWaterQuantity - num20) > (this.WaterSpawnQuantity - num22))))) & (args.PassageNodesMinLevel.Nodes[node.Num] == 0)) & (num20 < this.TotalWaterQuantity))
                    {
                        if (num21 == 0)
                        {
                            num22++;
                        }
                        num20++;
                        num5 = node.Num;
                        int num57 = this.SymmetryBlockCount - 1;
                        baseLevel = 0;
                        while (baseLevel <= num57)
                        {
                            this.PassageNodes[baseLevel, num5].IsWater = true;
                            this.PassageNodes[baseLevel, num5].Level = 0;
                            baseLevel++;
                        }
                        this.PassageNodesMinLevelSet(node, args.PassageNodesMinLevel, 0, this.MaxLevelTransition);
                        this.PassageNodesMaxLevelSet(node, args.PassageNodesMaxLevel, 0, this.MaxLevelTransition);
                        mapLevelCount = args.MapLevelCount;
                        num41 = 0;
                        mapLevelCount[num41]++;
                        int num58 = node.ConnectionCount - 1;
                        num2 = 0;
                        while (num2 <= num58)
                        {
                            other = node.Connections[num2].GetOther();
                            this.PassageNodesMinLevelSet(other, args.PassageNodesMinLevel, 0, this.MaxLevelTransition);
                            this.PassageNodesMaxLevelSet(other, args.PassageNodesMaxLevel, 0, this.MaxLevelTransition);
                            num2++;
                        }
                    }
                }
            }
            args.FlatsCutoff = 1;
            args.PassagesCutoff = 1;
            args.VariationCutoff = 1;
            args.ActionTotal = 1;
            int num59 = this.PassageNodeCount - 1;
            for (num = 0; num <= num59; num++)
            {
                node = nodeArray3[num];
                if (((node.Level < 0) & !node.IsOnBorder) & node.IsNearBorder)
                {
                    args.PassageNode = node;
                    result3 = this.PassageNodeHeightLevel(args);
                    if (!result3.Success)
                    {
                        result4.ProblemAdd(result3.Problem);
                        return result4;
                    }
                }
            }
            args.FlatsCutoff = this.FlatsChance;
            args.PassagesCutoff = args.FlatsCutoff + this.PassagesChance;
            args.VariationCutoff = args.PassagesCutoff + this.VariationChance;
            args.ActionTotal = args.VariationCutoff;
            if (args.ActionTotal <= 0)
            {
                result4.ProblemAdd("All height level behaviors are zero");
                return result4;
            }
            int num60 = this.PassageNodeCount - 1;
            for (num = 0; num <= num60; num++)
            {
                node = nodeArray3[num];
                if ((node.Level < 0) & !node.IsOnBorder)
                {
                    args.PassageNode = node;
                    result3 = this.PassageNodeHeightLevel(args);
                    if (!result3.Success)
                    {
                        result4.ProblemAdd(result3.Problem);
                        return result4;
                    }
                }
            }
            int num61 = this.PassageNodeCount - 1;
            for (num = 0; num <= num61; num++)
            {
                node = this.PassageNodes[0, num];
                if (node.IsOnBorder)
                {
                    if (node.Level >= 0)
                    {
                        result4.ProblemAdd("Error: Border has had its height set.");
                        return result4;
                    }
                    node3 = null;
                    flag = true;
                    int num62 = node.ConnectionCount - 1;
                    num2 = 0;
                    while (num2 <= num62)
                    {
                        other = node.Connections[num2].GetOther();
                        if ((((other.Level >= 0) & !other.IsOnBorder) && ((args.PassageNodesMinLevel.Nodes[node.Num] <= other.Level) & (args.PassageNodesMaxLevel.Nodes[node.Num] >= other.Level))) && (node3 == null))
                        {
                            node3 = other;
                        }
                        if (args.PassageNodesMinLevel.Nodes[other.Num] > 0)
                        {
                            flag = false;
                        }
                        num2++;
                    }
                    if (node3 != null)
                    {
                        level = node3.Level;
                        this.PassageNodesMinLevelSet(node, args.PassageNodesMinLevel, level, this.MaxLevelTransition);
                        this.PassageNodesMaxLevelSet(node, args.PassageNodesMaxLevel, level, this.MaxLevelTransition);
                        int num63 = this.SymmetryBlockCount - 1;
                        baseLevel = 0;
                        while (baseLevel <= num63)
                        {
                            this.PassageNodes[baseLevel, num].IsWater = node3.IsWater & flag;
                            this.PassageNodes[baseLevel, num].Level = level;
                            baseLevel++;
                        }
                        if (node.IsWater)
                        {
                            int num64 = node.ConnectionCount - 1;
                            num2 = 0;
                            while (num2 <= num64)
                            {
                                other = node.Connections[num2].GetOther();
                                this.PassageNodesMinLevelSet(other, args.PassageNodesMinLevel, node.Level, this.MaxLevelTransition);
                                this.PassageNodesMaxLevelSet(other, args.PassageNodesMaxLevel, node.Level, this.MaxLevelTransition);
                                num2++;
                            }
                        }
                    }
                }
                else if (node.Level < 0)
                {
                    result4.ProblemAdd("Error: Node height not set");
                    return result4;
                }
            }
            int num65 = this.PassageNodeCount - 1;
            for (num = 0; num <= num65; num++)
            {
                node = this.PassageNodes[0, num];
                if (node.IsOnBorder & (node.Level < 0))
                {
                    node3 = null;
                    flag = true;
                    int num66 = node.ConnectionCount - 1;
                    num2 = 0;
                    while (num2 <= num66)
                    {
                        other = node.Connections[num2].GetOther();
                        if (((other.Level >= 0) && ((args.PassageNodesMinLevel.Nodes[node.Num] <= other.Level) & (args.PassageNodesMaxLevel.Nodes[node.Num] >= other.Level))) && (node3 == null))
                        {
                            node3 = other;
                        }
                        if (args.PassageNodesMinLevel.Nodes[other.Num] > 0)
                        {
                            flag = false;
                        }
                        num2++;
                    }
                    if (node3 == null)
                    {
                        result4.ProblemAdd("Error: No connection for border node");
                        return result4;
                    }
                    level = node3.Level;
                    this.PassageNodesMinLevelSet(node, args.PassageNodesMinLevel, level, this.MaxLevelTransition);
                    this.PassageNodesMaxLevelSet(node, args.PassageNodesMaxLevel, level, this.MaxLevelTransition);
                    int num67 = this.SymmetryBlockCount - 1;
                    for (baseLevel = 0; baseLevel <= num67; baseLevel++)
                    {
                        this.PassageNodes[baseLevel, num].IsWater = node3.IsWater & flag;
                        this.PassageNodes[baseLevel, num].Level = level;
                    }
                    if (node.IsWater)
                    {
                        int num68 = node.ConnectionCount - 1;
                        for (num2 = 0; num2 <= num68; num2++)
                        {
                            other = node.Connections[num2].GetOther();
                            this.PassageNodesMinLevelSet(other, args.PassageNodesMinLevel, node.Level, this.MaxLevelTransition);
                            this.PassageNodesMaxLevelSet(other, args.PassageNodesMaxLevel, node.Level, this.MaxLevelTransition);
                        }
                    }
                }
            }
            this.RampBase = 1.0;
            this.MaxDisconnectionDist = 99999f;
            clsResult resultToAdd = this.GenerateRamps();
            result4.Add(resultToAdd);
            return result4;
        }

        public clsResult GenerateLayoutTerrain()
        {
            int num2;
            int num4;
            double magnitude;
            clsNodeTag tag;
            PathfinderNode node2;
            int num8;
            int num9;
            modMath.sXY_int _int3;
            clsResult result2 = new clsResult("Terrain heights");
            this.Map = new clsMap(this.TileSize);
            this.GenerateTerrainTiles = new GenerateTerrainTile[(this.Map.Terrain.TileSize.X - 1) + 1, (this.Map.Terrain.TileSize.Y - 1) + 1];
            this.GenerateTerrainVertices = new GenerateTerrainVertex[this.Map.Terrain.TileSize.X + 1, this.Map.Terrain.TileSize.Y + 1];
            this.VertexPathMap = new PathfinderNetwork();
            int y = this.Map.Terrain.TileSize.Y;
            for (num9 = 0; num9 <= y; num9++)
            {
                int x = this.Map.Terrain.TileSize.X;
                num8 = 0;
                while (num8 <= x)
                {
                    this.GenerateTerrainVertices[num8, num9] = new GenerateTerrainVertex();
                    this.GenerateTerrainVertices[num8, num9].Node = new PathfinderNode(this.VertexPathMap);
                    tag = new clsNodeTag {
                        Pos = new modMath.sXY_int(num8 * 0x80, num9 * 0x80)
                    };
                    this.GenerateTerrainVertices[num8, num9].Node.Tag = tag;
                    num8++;
                }
            }
            int num12 = this.Map.Terrain.TileSize.Y;
            for (num9 = 0; num9 <= num12; num9++)
            {
                int num13 = this.Map.Terrain.TileSize.X;
                for (num8 = 0; num8 <= num13; num8++)
                {
                    PathfinderNode node3;
                    node2 = this.GenerateTerrainVertices[num8, num9].Node;
                    if (num8 > 0)
                    {
                        node3 = this.GenerateTerrainVertices[num8 - 1, num9].Node;
                        this.GenerateTerrainVertices[num8, num9].LeftLink = node2.GetOrCreateConnection(node3, this.GetNodePosDist(node2, node3));
                    }
                    if (num9 > 0)
                    {
                        if (num8 > 0)
                        {
                            node3 = this.GenerateTerrainVertices[num8 - 1, num9 - 1].Node;
                            this.GenerateTerrainVertices[num8, num9].TopLeftLink = node2.GetOrCreateConnection(node3, this.GetNodePosDist(node2, node3));
                        }
                        node3 = this.GenerateTerrainVertices[num8, num9 - 1].Node;
                        this.GenerateTerrainVertices[num8, num9].TopLink = node2.GetOrCreateConnection(node3, this.GetNodePosDist(node2, node3));
                        if (num8 < this.Map.Terrain.TileSize.X)
                        {
                            node3 = this.GenerateTerrainVertices[num8 + 1, num9 - 1].Node;
                            this.GenerateTerrainVertices[num8, num9].TopRightLink = node2.GetOrCreateConnection(node3, this.GetNodePosDist(node2, node3));
                        }
                    }
                    if (num8 < this.Map.Terrain.TileSize.X)
                    {
                        node3 = this.GenerateTerrainVertices[num8 + 1, num9].Node;
                        this.GenerateTerrainVertices[num8, num9].RightLink = node2.GetOrCreateConnection(node3, this.GetNodePosDist(node2, node3));
                    }
                    if (num9 < this.Map.Terrain.TileSize.Y)
                    {
                        if (num8 > 0)
                        {
                            node3 = this.GenerateTerrainVertices[num8 - 1, num9 + 1].Node;
                            this.GenerateTerrainVertices[num8, num9].BottomLeftLink = node2.GetOrCreateConnection(node3, this.GetNodePosDist(node2, node3));
                        }
                        node3 = this.GenerateTerrainVertices[num8, num9 + 1].Node;
                        this.GenerateTerrainVertices[num8, num9].BottomLink = node2.GetOrCreateConnection(node3, this.GetNodePosDist(node2, node3));
                        if (num8 < this.Map.Terrain.TileSize.X)
                        {
                            node3 = this.GenerateTerrainVertices[num8 + 1, num9 + 1].Node;
                            this.GenerateTerrainVertices[num8, num9].BottomRightLink = node2.GetOrCreateConnection(node3, this.GetNodePosDist(node2, node3));
                        }
                    }
                }
            }
            this.VertexPathMap.LargeArraysResize();
            this.VertexPathMap.FindCalc();
            PathfinderLayer layer = this.VertexPathMap.get_GetNodeLayer(0);
            PathfinderLayer layer2 = this.VertexPathMap.get_GetNodeLayer(this.JitterScale);
            int num = layer2.GetNodeCount - 1;
            int[] numArray = new int[num + 1];
            clsBaseNodeLevels baseLevel = new clsBaseNodeLevels {
                NodeLevels = new float[(layer.GetNodeCount - 1) + 1]
            };
            if (num > 0)
            {
                int num14 = num;
                num2 = 0;
                while (num2 <= num14)
                {
                    Position.XY_dbl _dbl;
                    node2 = layer2.get_GetNode(num2);
                    num4 = 0;
                    _dbl.X = 0.0;
                    _dbl.Y = 0.0;
                    this.CalcNodePos(node2, ref _dbl, ref num4);
                    tag = new clsNodeTag();
                    tag.Pos.X = (int) Math.Round((double) (_dbl.X / ((double) num4)));
                    tag.Pos.Y = (int) Math.Round((double) (_dbl.Y / ((double) num4)));
                    node2.Tag = tag;
                    num2++;
                }
            }
            int num15 = layer2.GetNodeCount - 1;
            for (num = 0; num <= num15; num++)
            {
                bool flag;
                tag = (clsNodeTag) layer2.get_GetNode(num).Tag;
                numArray[num] = -1;
                double num3 = 3.4028234663852886E+38;
                clsConnection connection = null;
                clsPassageNode passageNodeA = null;
                int num16 = this.ConnectionCount - 1;
                num2 = 0;
                while (num2 <= num16)
                {
                    if (this.Connections[num2].PassageNodeA.Level == this.Connections[num2].PassageNodeB.Level)
                    {
                        modMath.sXY_int _int2 = modMath.PointGetClosestPosOnLine(this.Connections[num2].PassageNodeA.Pos, this.Connections[num2].PassageNodeB.Pos, tag.Pos) - tag.Pos;
                        magnitude = (float) _int2.ToDoubles().GetMagnitude();
                        if (magnitude < num3)
                        {
                            num3 = magnitude;
                            _int2 = tag.Pos - this.Connections[num2].PassageNodeA.Pos;
                            _int3 = tag.Pos - this.Connections[num2].PassageNodeB.Pos;
                            if (_int2.ToDoubles().GetMagnitude() <= _int3.ToDoubles().GetMagnitude())
                            {
                                passageNodeA = this.Connections[num2].PassageNodeA;
                            }
                            else
                            {
                                passageNodeA = this.Connections[num2].PassageNodeB;
                            }
                            flag = true;
                        }
                    }
                    num2++;
                }
                int num17 = this.PassageNodeCount - 1;
                for (num4 = 0; num4 <= num17; num4++)
                {
                    int num18 = this.SymmetryBlockCount - 1;
                    for (int i = 0; i <= num18; i++)
                    {
                        _int3 = tag.Pos - this.PassageNodes[i, num4].Pos;
                        magnitude = (float) _int3.ToDoubles().GetMagnitude();
                        if (magnitude < num3)
                        {
                            num3 = magnitude;
                            passageNodeA = this.PassageNodes[i, num4];
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    numArray[num] = passageNodeA.Level;
                }
                else
                {
                    numArray[num] = connection.PassageNodeA.Level;
                }
                if (numArray[num] < 0)
                {
                    result2.ProblemAdd("Error: Node height is not set.");
                    return result2;
                }
            }
            int num19 = this.LevelCount - 1;
            num = 0;
            while (num <= num19)
            {
                int num20 = layer2.GetNodeCount - 1;
                num2 = 0;
                while (num2 <= num20)
                {
                    if (numArray[num2] >= num)
                    {
                        this.SetBaseLevel(layer2.get_GetNode(num2), num, baseLevel);
                    }
                    num2++;
                }
                num++;
            }
            int num7 = ((int) Math.Round((double) ((this.LevelHeight * this.Map.HeightMultiplier) * 2.0))) + 0x80;
            clsSetBaseLevelRampArgs args = new clsSetBaseLevelRampArgs {
                BaseLevel = baseLevel,
                RampRadius = 320f
            };
            int num21 = this.ConnectionCount - 1;
            for (num2 = 0; num2 <= num21; num2++)
            {
                args.Connection = this.Connections[num2];
                _int3 = this.Connections[num2].PassageNodeA.Pos - this.Connections[num2].PassageNodeB.Pos;
                args.RampLength = Math.Max((int) Math.Round((double) (_int3.ToDoubles().GetMagnitude() * 0.75)), num7 * Math.Abs((int) (this.Connections[num2].PassageNodeA.Level - this.Connections[num2].PassageNodeB.Level)));
                int num22 = layer2.GetNodeCount - 1;
                num = 0;
                while (num <= num22)
                {
                    if (this.Connections[num2].IsRamp)
                    {
                        tag = (clsNodeTag) layer2.get_GetNode(num).Tag;
                        _int3 = modMath.PointGetClosestPosOnLine(this.Connections[num2].PassageNodeA.Pos, this.Connections[num2].PassageNodeB.Pos, tag.Pos) - tag.Pos;
                        magnitude = (float) _int3.ToDoubles().GetMagnitude();
                        if (magnitude < (args.RampLength * 2f))
                        {
                            this.SetBaseLevelRamp(args, layer2.get_GetNode(num));
                        }
                    }
                    num++;
                }
            }
            int num23 = layer.GetNodeCount - 1;
            for (num = 0; num <= num23; num++)
            {
                tag = (clsNodeTag) layer.get_GetNode(num).Tag;
                this.Map.Terrain.Vertices[(int) Math.Round((double) (((float) tag.Pos.X) / 128f)), (int) Math.Round((double) (((float) tag.Pos.Y) / 128f))].Height = (byte) Math.Round((double) (baseLevel.NodeLevels[num] * this.LevelHeight));
            }
            return result2;
        }

        public clsResult GenerateOil()
        {
            int num;
            int num2;
            int num6;
            clsPassageNode[,] passageNodes;
            int num17;
            int num18;
            clsResult result2 = new clsResult("Oil");
            int num7 = this.PassageNodeCount - 1;
            for (num = 0; num <= num7; num++)
            {
                int num8 = this.SymmetryBlockCount - 1;
                num2 = 0;
                while (num2 <= num8)
                {
                    this.PassageNodes[num2, num].OilCount = 0;
                    num2++;
                }
            }
            clsPassageNodeNework nework = this.MakePassageNodeNetwork();
            PathfinderNode[] startNodes = new PathfinderNode[1];
            this.PassageNodeDists = new float[(this.SymmetryBlockCount - 1) + 1, (this.PassageNodeCount - 1) + 1, (this.SymmetryBlockCount - 1) + 1, (this.PassageNodeCount - 1) + 1];
            int num9 = this.PassageNodeCount - 1;
            for (num = 0; num <= num9; num++)
            {
                int num10 = this.SymmetryBlockCount - 1;
                for (int i = 0; i <= num10; i++)
                {
                    this.PassageNodeDists[i, num, i, num] = 0f;
                    int num11 = this.PassageNodeCount - 1;
                    num2 = 0;
                    while (num2 <= num11)
                    {
                        int num12 = this.SymmetryBlockCount - 1;
                        for (int j = 0; j <= num12; j++)
                        {
                            if ((this.PassageNodes[0, num].IsWater | this.PassageNodes[j, num2].IsWater) | ((j != 0) & (i != 0)))
                            {
                                this.PassageNodeDists[i, num, j, num2] = float.MaxValue;
                                this.PassageNodeDists[j, num2, i, num] = float.MaxValue;
                            }
                            else
                            {
                                startNodes[0] = nework.PassageNodePathNodes[i, num];
                                PathfinderNetwork.PathList[] listArray = nework.Network.GetPath(startNodes, nework.PassageNodePathNodes[j, num2], -1, 0);
                                if (listArray == null)
                                {
                                    result2.ProblemAdd("Map is not all connected.");
                                    nework.Network.Deallocate();
                                    return result2;
                                }
                                if (listArray[0].PathCount != 1)
                                {
                                    Debugger.Break();
                                }
                                this.PassageNodeDists[i, num, j, num2] = listArray[0].Paths[0].Value;
                                this.PassageNodeDists[j, num2, i, num] = listArray[0].Paths[0].Value;
                            }
                        }
                        num2++;
                    }
                }
            }
            nework.Network.Deallocate();
            int num5 = 1;
            int num13 = this.OilAtATime - 1;
            num = 0;
            while (num <= num13)
            {
                num5 *= this.PassageNodeCount;
                num++;
            }
            clsOilBalanceLoopArgs args = new clsOilBalanceLoopArgs {
                OilClusterSizes = new int[(this.OilAtATime - 1) + 1],
                PlayerOilScore = new double[(this.TopLeftPlayerCount - 1) + 1],
                OilNodes = new clsPassageNode[(this.OilAtATime - 1) + 1]
            };
            while (num6 < this.ExtraOilCount)
            {
                int num14 = this.OilAtATime - 1;
                num = 0;
                while (num <= num14)
                {
                    args.OilClusterSizes[num] = Math.Min(this.ExtraOilClusterSizeMin + ((int) Math.Round((double) ((float) (App.Random.Next() * ((this.ExtraOilClusterSizeMax - this.ExtraOilClusterSizeMin) + 1))))), Math.Max((int) Math.Round(Math.Ceiling((double) (((double) (this.ExtraOilCount - num6)) / ((double) this.SymmetryBlockCount)))), 1));
                    num++;
                }
                args.OilPossibilities = new clsOilPossibilities();
                this.OilBalanceLoop(args, 0);
                clsOilPossibilities.clsPossibility bestPossibility = args.OilPossibilities.BestPossibility;
                if (bestPossibility != null)
                {
                    int num15 = this.OilAtATime - 1;
                    num2 = 0;
                    while (num2 <= num15)
                    {
                        int num16 = this.SymmetryBlockCount - 1;
                        num = 0;
                        while (num <= num16)
                        {
                            passageNodes = this.PassageNodes;
                            num17 = num;
                            num18 = bestPossibility.Nodes[num2].Num;
                            passageNodes[num17, num18].OilCount += args.OilClusterSizes[num2];
                            num++;
                        }
                        num6 += args.OilClusterSizes[num2] * this.SymmetryBlockCount;
                        num2++;
                    }
                    int num19 = this.TopLeftPlayerCount - 1;
                    num = 0;
                    while (num <= num19)
                    {
                        double[] playerOilScore = args.PlayerOilScore;
                        num18 = num;
                        playerOilScore[num18] += bestPossibility.PlayerOilScoreAddition[num];
                        num++;
                    }
                }
                else
                {
                    result2.WarningAdd("Could not place all of the oil. " + Conversions.ToString(num6) + " oil was placed.");
                    break;
                }
            }
            int num20 = this.TopLeftPlayerCount - 1;
            for (num = 0; num <= num20; num++)
            {
                int num21 = this.SymmetryBlockCount - 1;
                for (num2 = 0; num2 <= num21; num2++)
                {
                    passageNodes = this.PassageNodes;
                    num18 = num2;
                    num17 = this.PlayerBases[num].Nodes[0].Num;
                    passageNodes[num18, num17].OilCount += this.BaseOilCount;
                }
            }
            return result2;
        }

        public clsResult GenerateRamps()
        {
            int num;
            int num6;
            int num7;
            int num9;
            clsResult result2 = new clsResult("Ramps");
            int num13 = this.ConnectionCount - 1;
            for (num = 0; num <= num13; num++)
            {
                this.Connections[num].IsRamp = false;
            }
            clsPassageNodeNework nework = this.MakePassageNodeNetwork();
            PathfinderNode[,] passageNodePathNodes = nework.PassageNodePathNodes;
            clsConnection[] connectionArray = new clsConnection[(this.ConnectionCount - 1) + 1];
            PathfinderNode[] startNodes = new PathfinderNode[1];
            bool[] flagArray = new bool[(this.ConnectionCount - 1) + 1];
            int num14 = this.ConnectionCount - 1;
            int index = 0;
            while (index <= num14)
            {
                if (Math.Abs((int) (this.Connections[index].PassageNodeA.Level - this.Connections[index].PassageNodeB.Level)) == 1)
                {
                    if ((!(this.Connections[index].PassageNodeA.IsOnBorder | this.Connections[index].PassageNodeB.IsOnBorder) & (this.Connections[index].PassageNodeA.MirrorNum == 0)) & (this.Connections[index].PassageNodeA.Num != this.Connections[index].PassageNodeB.Num))
                    {
                        flagArray[index] = true;
                    }
                    else
                    {
                        flagArray[index] = false;
                    }
                }
                else
                {
                    flagArray[index] = false;
                }
                index++;
            }
            clsNodeConnectedness connectedness = new clsNodeConnectedness {
                NodeConnectedness = new float[(this.PassageNodeCount - 1) + 1],
                PassageNodeVisited = new bool[(this.SymmetryBlockCount - 1) + 1, (this.PassageNodeCount - 1) + 1],
                PassageNodePathNodes = passageNodePathNodes,
                PassageNodePathMap = nework.Network
            };
            PathfinderConnection[] connectionArray2 = new PathfinderConnection[4];
            clsUpdateNodeConnectednessArgs args3 = new clsUpdateNodeConnectednessArgs();
            clsUpdateNetworkConnectednessArgs args2 = new clsUpdateNetworkConnectednessArgs();
            args3.Args = connectedness;
            args2.Args = connectedness;
            args2.PassageNodeUpdated = new bool[(this.PassageNodeCount - 1) + 1];
            args2.SymmetryBlockCount = this.SymmetryBlockCount;
            int num15 = this.PassageNodeCount - 1;
            for (num = 0; num <= num15; num++)
            {
                connectedness.NodeConnectedness[num] = 0f;
                int num16 = this.PassageNodeCount - 1;
                for (index = 0; index <= num16; index++)
                {
                    int num17 = this.SymmetryBlockCount - 1;
                    num7 = 0;
                    while (num7 <= num17)
                    {
                        connectedness.PassageNodeVisited[num7, index] = false;
                        num7++;
                    }
                }
                args3.OriginalNode = this.PassageNodes[0, num];
                this.UpdateNodeConnectedness(args3, this.PassageNodes[0, num]);
            }
        Label_0269:
            num6 = -1;
            double num4 = 1.0;
            double num5 = 0.0;
            int num10 = 0;
            int num18 = this.ConnectionCount - 1;
            index = 0;
            while (index <= num18)
            {
                if (flagArray[index] & !this.Connections[index].IsRamp)
                {
                    if (this.CheckRampAngles(this.Connections[index], 1.3962634015954636, 2.0943951023931953, 0.0))
                    {
                        modMath.sXY_int _int;
                        startNodes[0] = passageNodePathNodes[this.Connections[index].PassageNodeA.MirrorNum, this.Connections[index].PassageNodeA.Num];
                        PathfinderNetwork.PathList[] listArray = nework.Network.GetPath(startNodes, passageNodePathNodes[this.Connections[index].PassageNodeB.MirrorNum, this.Connections[index].PassageNodeB.Num], -1, 0);
                        double maxValue = double.MaxValue;
                        _int.X = (int) Math.Round((double) (((double) (this.Connections[index].PassageNodeA.Pos.X + this.Connections[index].PassageNodeB.Pos.X)) / 2.0));
                        _int.Y = (int) Math.Round((double) (((double) (this.Connections[index].PassageNodeA.Pos.Y + this.Connections[index].PassageNodeB.Pos.Y)) / 2.0));
                        int num19 = this.TotalPlayerCount - 1;
                        num9 = 0;
                        while (num9 <= num19)
                        {
                            modMath.sXY_int _int2 = this.PlayerBases[num9].Pos - _int;
                            double magnitude = _int2.ToDoubles().GetMagnitude();
                            if (magnitude < maxValue)
                            {
                                maxValue = magnitude;
                            }
                            num9++;
                        }
                        double num11 = Math.Max((double) (this.MaxDisconnectionDist * Math.Pow(this.RampBase, maxValue / 1024.0)), (double) 1.0);
                        if (listArray == null)
                        {
                            double num12 = connectedness.NodeConnectedness[this.Connections[index].PassageNodeA.Num] + connectedness.NodeConnectedness[this.Connections[index].PassageNodeB.Num];
                            if (double.MaxValue > num4)
                            {
                                num4 = double.MaxValue;
                                num5 = num12;
                                connectionArray[0] = this.Connections[index];
                                num10 = 1;
                            }
                            else if (num12 < num5)
                            {
                                num5 = num12;
                                connectionArray[0] = this.Connections[index];
                                num10 = 1;
                            }
                            else if (num12 == num5)
                            {
                                connectionArray[num10] = this.Connections[index];
                                num10++;
                            }
                        }
                        else
                        {
                            if (listArray[0].PathCount != 1)
                            {
                                result2.ProblemAdd("Error: Invalid number of routes returned.");
                                goto Label_088A;
                            }
                            if ((((double) listArray[0].Paths[0].Value) / num11) > num4)
                            {
                                num4 = ((double) listArray[0].Paths[0].Value) / num11;
                                connectionArray[0] = this.Connections[index];
                                num10 = 1;
                            }
                            else if ((((double) listArray[0].Paths[0].Value) / num11) == num4)
                            {
                                connectionArray[num10] = this.Connections[index];
                                num10++;
                            }
                            else if (listArray[0].Paths[0].Value <= num11)
                            {
                                flagArray[index] = false;
                            }
                        }
                    }
                    else
                    {
                        flagArray[index] = false;
                    }
                }
                else
                {
                    flagArray[index] = false;
                }
                index++;
            }
            if (num10 > 0)
            {
                num6 = (int) Math.Round((double) ((float) (App.Random.Next() * num10)));
                connectionArray[num6].IsRamp = true;
                clsPassageNode passageNodeA = connectionArray[num6].PassageNodeA;
                clsPassageNode passageNodeB = connectionArray[num6].PassageNodeB;
                PathfinderNode nodeA = passageNodePathNodes[passageNodeA.MirrorNum, passageNodeA.Num];
                PathfinderNode otherNode = passageNodePathNodes[passageNodeB.MirrorNum, passageNodeB.Num];
                PathfinderConnection connection = nodeA.CreateConnection(otherNode, this.GetNodePosDist(nodeA, otherNode));
                int num20 = connectionArray[num6].ReflectionCount - 1;
                for (num7 = 0; num7 <= num20; num7++)
                {
                    connectionArray[num6].Reflections[num7].IsRamp = true;
                    passageNodeA = connectionArray[num6].Reflections[num7].PassageNodeA;
                    passageNodeB = connectionArray[num6].Reflections[num7].PassageNodeB;
                    nodeA = passageNodePathNodes[passageNodeA.MirrorNum, passageNodeA.Num];
                    otherNode = passageNodePathNodes[passageNodeB.MirrorNum, passageNodeB.Num];
                    connection = nodeA.CreateConnection(otherNode, this.GetNodePosDist(nodeA, otherNode));
                }
                nework.Network.FindCalc();
                int num21 = this.PassageNodeCount - 1;
                for (num9 = 0; num9 <= num21; num9++)
                {
                    args2.PassageNodeUpdated[num9] = false;
                }
                if (connectionArray[num6].PassageNodeA.MirrorNum == 0)
                {
                    this.UpdateNetworkConnectedness(args2, connectionArray[num6].PassageNodeA);
                    goto Label_0269;
                }
                if (connectionArray[num6].PassageNodeB.MirrorNum == 0)
                {
                    this.UpdateNetworkConnectedness(args2, connectionArray[num6].PassageNodeB);
                    goto Label_0269;
                }
                result2.ProblemAdd("Error: Initial ramp not in area 0.");
            }
            else
            {
                PathfinderNetwork.sFloodProximityArgs args;
                args.StartNode = nework.PassageNodePathNodes[0, 0];
                args.NodeValues = nework.Network.NetworkLargeArrays.Nodes_ValuesA;
                int num22 = this.PassageNodeCount - 1;
                for (num = 0; num <= num22; num++)
                {
                    int num23 = this.SymmetryBlockCount - 1;
                    index = 0;
                    while (index <= num23)
                    {
                        args.NodeValues[nework.PassageNodePathNodes[index, num].Layer_NodeNum] = float.MaxValue;
                        index++;
                    }
                }
                nework.Network.FloodProximity(ref args);
                int num24 = this.PassageNodeCount - 1;
                for (num = 0; num <= num24; num++)
                {
                    int num25 = this.SymmetryBlockCount - 1;
                    for (index = 0; index <= num25; index++)
                    {
                        if (!this.PassageNodes[index, num].IsWater && (args.NodeValues[nework.PassageNodePathNodes[index, num].Layer_NodeNum] == float.MaxValue))
                        {
                            result2.ProblemAdd("Land is unreachable. Reduce variation or retry.");
                            break;
                        }
                    }
                }
            }
        Label_088A:
            nework.Network.Deallocate();
            return result2;
        }

        public void GenerateTilePathMap()
        {
            int num;
            int num2;
            this.TilePathMap = new PathfinderNetwork();
            int num3 = this.Map.Terrain.TileSize.Y - 1;
            for (num2 = 0; num2 <= num3; num2++)
            {
                int num4 = this.Map.Terrain.TileSize.X - 1;
                num = 0;
                while (num <= num4)
                {
                    this.GenerateTerrainTiles[num, num2] = new GenerateTerrainTile();
                    this.GenerateTerrainTiles[num, num2].Node = new PathfinderNode(this.TilePathMap);
                    clsNodeTag tag = new clsNodeTag {
                        Pos = new modMath.sXY_int((int) Math.Round((double) ((num + 0.5) * 128.0)), (int) Math.Round((double) ((num2 + 0.5) * 128.0)))
                    };
                    this.GenerateTerrainTiles[num, num2].Node.Tag = tag;
                    num++;
                }
            }
            int num5 = this.Map.Terrain.TileSize.Y - 1;
            for (num2 = 0; num2 <= num5; num2++)
            {
                int num6 = this.Map.Terrain.TileSize.X - 1;
                for (num = 0; num <= num6; num++)
                {
                    PathfinderNode node;
                    PathfinderNode nodeA = this.GenerateTerrainTiles[num, num2].Node;
                    if (num > 0)
                    {
                        node = this.GenerateTerrainTiles[num - 1, num2].Node;
                        this.GenerateTerrainTiles[num, num2].LeftLink = nodeA.GetOrCreateConnection(node, this.GetNodePosDist(nodeA, node));
                    }
                    if (num2 > 0)
                    {
                        if (num > 0)
                        {
                            node = this.GenerateTerrainTiles[num - 1, num2 - 1].Node;
                            this.GenerateTerrainTiles[num, num2].TopLeftLink = nodeA.GetOrCreateConnection(node, this.GetNodePosDist(nodeA, node));
                        }
                        node = this.GenerateTerrainTiles[num, num2 - 1].Node;
                        this.GenerateTerrainTiles[num, num2].TopLink = nodeA.GetOrCreateConnection(node, this.GetNodePosDist(nodeA, node));
                        if (num < (this.Map.Terrain.TileSize.X - 1))
                        {
                            node = this.GenerateTerrainTiles[num + 1, num2 - 1].Node;
                            this.GenerateTerrainTiles[num, num2].TopRightLink = nodeA.GetOrCreateConnection(node, this.GetNodePosDist(nodeA, node));
                        }
                    }
                    if (num < (this.Map.Terrain.TileSize.X - 1))
                    {
                        node = this.GenerateTerrainTiles[num + 1, num2].Node;
                        this.GenerateTerrainTiles[num, num2].RightLink = nodeA.GetOrCreateConnection(node, this.GetNodePosDist(nodeA, node));
                    }
                    if (num2 < (this.Map.Terrain.TileSize.Y - 1))
                    {
                        if (num > 0)
                        {
                            node = this.GenerateTerrainTiles[num - 1, num2 + 1].Node;
                            this.GenerateTerrainTiles[num, num2].BottomLeftLink = nodeA.GetOrCreateConnection(node, this.GetNodePosDist(nodeA, node));
                        }
                        node = this.GenerateTerrainTiles[num, num2 + 1].Node;
                        this.GenerateTerrainTiles[num, num2].BottomLink = nodeA.GetOrCreateConnection(node, this.GetNodePosDist(nodeA, node));
                        if (num < (this.Map.Terrain.TileSize.X - 1))
                        {
                            node = this.GenerateTerrainTiles[num + 1, num2 + 1].Node;
                            this.GenerateTerrainTiles[num, num2].BottomRightLink = nodeA.GetOrCreateConnection(node, this.GetNodePosDist(nodeA, node));
                        }
                    }
                }
            }
            this.TilePathMap.LargeArraysResize();
            this.TilePathMap.FindCalc();
        }

        public clsResult GenerateUnits()
        {
            int num;
            int num3;
            int num5;
            int num7;
            modMath.sXY_int _int;
            uint num12;
            int num13;
            clsMap.clsUnit unit;
            uint num14;
            clsResult result2 = new clsResult("Objects");
            int maxDistFromPos = 0x300;
            int num4 = 0x800;
            clsMap.clsTerrain terrain = this.Map.Terrain;
            int num15 = this.PassageNodeCount - 1;
            for (num = 0; num <= num15; num++)
            {
                int num16 = this.SymmetryBlockCount - 1;
                num3 = 0;
                while (num3 <= num16)
                {
                    this.PassageNodes[num3, num].HasFeatureCluster = false;
                    num3++;
                }
            }
            int num17 = this.TotalPlayerCount - 1;
            for (num = 0; num <= num17; num++)
            {
                int num11 = num;
                if (this.PlaceUnitNear(modGenerator.UnitType_CommandCentre, this.PlayerBases[num].Pos, this.Map.UnitGroups[num11], 3, 0, num4) == null)
                {
                    result2.ProblemAdd("No room for base structures");
                    return result2;
                }
                unit = this.PlaceUnitNear(modGenerator.UnitType_PowerGenerator, this.PlayerBases[num].Pos, this.Map.UnitGroups[num11], 3, 0, num4);
                if (unit == null)
                {
                    result2.ProblemAdd("No room for base structures.");
                    return result2;
                }
                if (this.PlaceUnit(modGenerator.UnitType_PowerModule, unit.Pos, this.Map.UnitGroups[num11], 0) == null)
                {
                    result2.ProblemAdd("No room for module.");
                    return result2;
                }
                num3 = 1;
                do
                {
                    unit = this.PlaceUnitNear(modGenerator.UnitType_ResearchFacility, this.PlayerBases[num].Pos, this.Map.UnitGroups[num11], 3, 0, num4);
                    if (unit == null)
                    {
                        result2.ProblemAdd("No room for base structures");
                        return result2;
                    }
                    if (this.PlaceUnit(modGenerator.UnitType_ResearchModule, unit.Pos, this.Map.UnitGroups[num11], 0) == null)
                    {
                        result2.ProblemAdd("No room for module.");
                        return result2;
                    }
                    num3++;
                }
                while (num3 <= 2);
                num3 = 1;
                do
                {
                    unit = this.PlaceUnitNear(modGenerator.UnitType_Factory, this.PlayerBases[num].Pos, this.Map.UnitGroups[num11], 4, 0, num4);
                    if (unit == null)
                    {
                        result2.ProblemAdd("No room for base structures");
                        return result2;
                    }
                    if (this.PlaceUnit(modGenerator.UnitType_FactoryModule, unit.Pos, this.Map.UnitGroups[num11], 0) == null)
                    {
                        result2.ProblemAdd("No room for module.");
                        return result2;
                    }
                    num3++;
                }
                while (num3 <= 2);
                if (this.PlaceUnitNear(modGenerator.UnitType_CyborgFactory, this.PlayerBases[num].Pos, this.Map.UnitGroups[num11], 3, 0, num4) == null)
                {
                    result2.ProblemAdd("No room for base structures");
                    return result2;
                }
                int baseTruckCount = this.BaseTruckCount;
                num3 = 1;
                while (num3 <= baseTruckCount)
                {
                    if (this.PlaceUnitNear(modGenerator.UnitType_Truck, this.PlayerBases[num].Pos, this.Map.UnitGroups[num11], 2, 0, num4) == null)
                    {
                        result2.ProblemAdd("No room for trucks");
                        return result2;
                    }
                    num3++;
                }
            }
            int num19 = this.PassageNodeCount - 1;
            for (num = 0; num <= num19; num++)
            {
                int num20 = this.SymmetryBlockCount - 1;
                num7 = 0;
                while (num7 <= num20)
                {
                    int num21 = this.PassageNodes[num7, num].OilCount - 1;
                    num3 = 0;
                    while (num3 <= num21)
                    {
                        modMath.sXY_int _int2;
                        if (this.PassageNodes[num7, num].PlayerBaseNum >= 0)
                        {
                            unit = this.PlaceUnitNear(modGenerator.UnitType_OilResource, this.PassageNodes[num7, num].Pos, this.Map.ScavengerUnitGroup, 2, 0, num4);
                        }
                        else
                        {
                            unit = this.PlaceUnitNear(modGenerator.UnitType_OilResource, this.PassageNodes[num7, num].Pos, this.Map.ScavengerUnitGroup, 2, 0, maxDistFromPos);
                        }
                        if (unit == null)
                        {
                            result2.ProblemAdd("No room for oil.");
                            return result2;
                        }
                        _int2.X = (int) Math.Round(((double) (((double) unit.Pos.Horizontal.X) / 128.0)));
                        _int2.Y = (int) Math.Round(((double) (((double) unit.Pos.Horizontal.Y) / 128.0)));
                        byte num2 = (byte) Math.Round((double) (((double) (((terrain.Vertices[_int2.X, _int2.Y].Height + terrain.Vertices[_int2.X + 1, _int2.Y].Height) + terrain.Vertices[_int2.X, _int2.Y + 1].Height) + terrain.Vertices[_int2.X + 1, _int2.Y + 1].Height)) / 4.0));
                        terrain.Vertices[_int2.X, _int2.Y].Height = num2;
                        terrain.Vertices[_int2.X + 1, _int2.Y].Height = num2;
                        terrain.Vertices[_int2.X, _int2.Y + 1].Height = num2;
                        terrain.Vertices[_int2.X + 1, _int2.Y + 1].Height = num2;
                        this.Map.SectorGraphicsChanges.TileChanged(_int2);
                        this.Map.SectorUnitHeightsChanges.TileChanged(_int2);
                        this.Map.SectorTerrainUndoChanges.TileChanged(_int2);
                        unit.Pos.Altitude = num2 * this.Map.HeightMultiplier;
                        if ((this.PassageNodes[num7, num].PlayerBaseNum >= 0) && (this.PlaceUnit(modGenerator.UnitType_Derrick, unit.Pos, this.Map.UnitGroups[this.PassageNodes[num7, num].PlayerBaseNum], 0) == null))
                        {
                            result2.ProblemAdd("No room for derrick.");
                            return result2;
                        }
                        num3++;
                    }
                    num7++;
                }
            }
            int num22 = this.PassageNodeCount - 1;
            for (num = 0; num <= num22; num++)
            {
                int num23 = this.SymmetryBlockCount - 1;
                num7 = 0;
                while (num7 <= num23)
                {
                    if ((this.PassageNodes[num7, num].PlayerBaseNum < 0) & !this.PassageNodes[num7, num].IsOnBorder)
                    {
                        this.PassageNodes[num7, num].HasFeatureCluster = App.Random.Next() < this.FeatureClusterChance;
                    }
                    num7++;
                }
            }
            int num10 = 0;
            if (this.GenerateTileset.ClusteredUnitChanceTotal > 0)
            {
                int num24 = this.PassageNodeCount - 1;
                for (num = 0; num <= num24; num++)
                {
                    int num25 = this.SymmetryBlockCount - 1;
                    for (num7 = 0; num7 <= num25; num7++)
                    {
                        if (this.PassageNodes[num7, num].HasFeatureCluster)
                        {
                            int num6 = this.FeatureClusterMinUnits + ((int) Math.Round((double) ((float) (App.Random.Next() * ((this.FeatureClusterMaxUnits - this.FeatureClusterMinUnits) + 1)))));
                            int num26 = num6;
                            num3 = 1;
                            while (num3 <= num26)
                            {
                                num12 = (uint) Math.Round(((double) (App.Random.Next() * this.GenerateTileset.ClusteredUnitChanceTotal)));
                                num14 = 0;
                                int num27 = this.GenerateTileset.ClusteredUnitCount - 1;
                                num5 = 0;
                                while (num5 <= num27)
                                {
                                    num14 += this.GenerateTileset.ClusteredUnits[num5].Chance;
                                    if (num12 < num14)
                                    {
                                        break;
                                    }
                                    num5++;
                                }
                                num13 = 0;
                                _int = this.GenerateTileset.ClusteredUnits[num5].Type.get_GetFootprintSelected(num13);
                                int clearance = ((int) Math.Round(Math.Ceiling((double) (((float) Math.Max(_int.X, _int.Y)) / 2f)))) + 1;
                                if (this.PlaceUnitNear(this.GenerateTileset.ClusteredUnits[num5].Type, this.PassageNodes[num7, num].Pos, this.Map.ScavengerUnitGroup, clearance, num13, maxDistFromPos) == null)
                                {
                                    num10 += (num6 - num3) + 1;
                                    break;
                                }
                                num3++;
                            }
                        }
                    }
                }
                if (num10 > 0)
                {
                    result2.WarningAdd("Not enough space for " + Conversions.ToString(num10) + " clustered objects.");
                }
            }
            if (this.TilePathMap.get_GetNodeLayer(this.TilePathMap.GetNodeLayerCount - 1).GetNodeCount != 1)
            {
                result2.ProblemAdd("Error: bad node count on top layer!");
                return result2;
            }
            if (this.GenerateTileset.ScatteredUnitChanceTotal > 0)
            {
                int featureScatterCount = this.FeatureScatterCount;
                num = 1;
                while (num <= featureScatterCount)
                {
                    num12 = (uint) Math.Round(((double) (App.Random.Next() * this.GenerateTileset.ScatteredUnitChanceTotal)));
                    num14 = 0;
                    int num29 = this.GenerateTileset.ScatteredUnitCount - 1;
                    num5 = 0;
                    while (num5 <= num29)
                    {
                        num14 += this.GenerateTileset.ScatteredUnits[num5].Chance;
                        if (num12 < num14)
                        {
                            break;
                        }
                        num5++;
                    }
                    num13 = 0;
                    _int = this.GenerateTileset.ScatteredUnits[num5].Type.get_GetFootprintSelected(num13);
                    num3 = this.FeatureScatterGap + ((int) Math.Round(Math.Ceiling((double) (((float) Math.Max(_int.X, _int.Y)) / 2f))));
                    PathfinderNode randomChildNode = this.GetRandomChildNode(this.TilePathMap.get_GetNodeLayer(this.TilePathMap.GetNodeLayerCount - 1).get_GetNode(0), num3);
                    if (randomChildNode == null)
                    {
                        break;
                    }
                    clsNodeTag tag = (clsNodeTag) randomChildNode.Tag;
                    if (this.PlaceUnitNear(this.GenerateTileset.ScatteredUnits[num5].Type, tag.Pos, this.Map.ScavengerUnitGroup, num3, num13, maxDistFromPos) == null)
                    {
                        break;
                    }
                    num++;
                }
                if (num < (this.FeatureScatterCount + 1))
                {
                    result2.WarningAdd("Only enough space for " + Conversions.ToString((int) (num - 1)) + " scattered objects.");
                }
            }
            return result2;
        }

        public PathfinderNode GetNearestNode(PathfinderNetwork Network, modMath.sXY_int Pos, int MinClearance)
        {
            double maxValue = double.MaxValue;
            PathfinderNode node = null;
            int num4 = Network.get_GetNodeLayer(0).GetNodeCount - 1;
            for (int i = 0; i <= num4; i++)
            {
                PathfinderNode node3 = Network.get_GetNodeLayer(0).get_GetNode(i);
                if (node3.GetClearance >= MinClearance)
                {
                    clsNodeTag tag = (clsNodeTag) node3.Tag;
                    modMath.sXY_int _int = tag.Pos - Pos;
                    double magnitude = _int.ToDoubles().GetMagnitude();
                    if (magnitude < maxValue)
                    {
                        maxValue = magnitude;
                        node = node3;
                    }
                }
            }
            return node;
        }

        private PathfinderNode GetNearestNodeConnection(PathfinderNetwork Network, modMath.sXY_int Pos, int MinClearance, float MaxDistance)
        {
            PathfinderNode[] nodeArray = new PathfinderNode[((Network.get_GetNodeLayer(0).GetNodeCount * 10) - 1) + 1];
            float[] numArray = new float[(Network.get_GetNodeLayer(0).GetNodeCount - 1) + 1];
            int index = 0;
            PathfinderNode node = null;
            int num5 = Network.get_GetNodeLayer(0).GetNodeCount - 1;
            int num = 0;
            while (num <= num5)
            {
                numArray[num] = float.MaxValue;
                num++;
            }
            nodeArray[0] = this.GetNearestNode(Network, Pos, 1);
            if (nodeArray[0] == null)
            {
                return null;
            }
            int num3 = 1;
            numArray[nodeArray[0].Layer_NodeNum] = 0f;
            while (index < num3)
            {
                PathfinderNode self = nodeArray[index];
                if (self.Clearance >= MinClearance)
                {
                    if (node == null)
                    {
                        node = self;
                    }
                    else if (numArray[self.Layer_NodeNum] < numArray[node.Layer_NodeNum])
                    {
                        node = self;
                    }
                }
                int num6 = self.GetConnectionCount - 1;
                for (num = 0; num <= num6; num++)
                {
                    bool flag;
                    PathfinderConnection connection = self.get_GetConnection(num);
                    PathfinderNode otherNode = connection.GetOtherNode(self);
                    float num2 = numArray[self.Layer_NodeNum] + connection.GetValue;
                    if (node == null)
                    {
                        flag = true;
                    }
                    else if (num2 < numArray[node.Layer_NodeNum])
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                    if (flag & (num2 < numArray[otherNode.Layer_NodeNum]))
                    {
                        numArray[otherNode.Layer_NodeNum] = num2;
                        nodeArray[num3] = otherNode;
                        num3++;
                    }
                }
                index++;
            }
            return node;
        }

        public float GetNodePosDist(PathfinderNode NodeA, PathfinderNode NodeB)
        {
            clsNodeTag tag = (clsNodeTag) NodeA.Tag;
            clsNodeTag tag2 = (clsNodeTag) NodeB.Tag;
            modMath.sXY_int _int = tag.Pos - tag2.Pos;
            return (float) _int.ToDoubles().GetMagnitude();
        }

        public PathfinderNode GetRandomChildNode(PathfinderNode InputNode, int MinClearance)
        {
            int num;
            if (InputNode.GetClearance < MinClearance)
            {
                return null;
            }
            if (InputNode.GetChildNodeCount == 0)
            {
                return InputNode;
            }
            do
            {
                num = (int) Math.Round((double) ((float) (App.Random.Next() * InputNode.GetChildNodeCount)));
            }
            while (InputNode.get_GetChildNode(num).GetClearance < MinClearance);
            return this.GetRandomChildNode(InputNode.get_GetChildNode(num), MinClearance);
        }

        public clsBooleanMap GetWaterMap()
        {
            clsBooleanMap map2 = new clsBooleanMap();
            map2.Blank(this.Map.Terrain.TileSize.X + 1, this.Map.Terrain.TileSize.Y + 1);
            int y = this.Map.Terrain.TileSize.Y;
            for (int i = 0; i <= y; i++)
            {
                int x = this.Map.Terrain.TileSize.X;
                for (int j = 0; j <= x; j++)
                {
                    bool isWater;
                    float magnitude;
                    modMath.sXY_int _int4;
                    float maxValue = float.MaxValue;
                    modMath.sXY_int point = new modMath.sXY_int(j * 0x80, i * 0x80);
                    int num9 = this.ConnectionCount - 1;
                    int index = 0;
                    while (index <= num9)
                    {
                        if (this.Connections[index].PassageNodeA.IsWater == this.Connections[index].PassageNodeB.IsWater)
                        {
                            modMath.sXY_int _int3 = modMath.PointGetClosestPosOnLine(this.Connections[index].PassageNodeA.Pos, this.Connections[index].PassageNodeB.Pos, point) - point;
                            magnitude = (float) _int3.ToDoubles().GetMagnitude();
                            if (magnitude < maxValue)
                            {
                                maxValue = magnitude;
                                _int3 = point - this.Connections[index].PassageNodeA.Pos;
                                _int4 = point - this.Connections[index].PassageNodeB.Pos;
                                if (_int3.ToDoubles().GetMagnitude() <= _int4.ToDoubles().GetMagnitude())
                                {
                                    isWater = this.Connections[index].PassageNodeA.IsWater;
                                }
                                else
                                {
                                    isWater = this.Connections[index].PassageNodeB.IsWater;
                                }
                            }
                        }
                        index++;
                    }
                    int num10 = this.PassageNodeCount - 1;
                    for (int k = 0; k <= num10; k++)
                    {
                        int num11 = this.SymmetryBlockCount - 1;
                        for (index = 0; index <= num11; index++)
                        {
                            _int4 = point - this.PassageNodes[index, k].Pos;
                            magnitude = (float) _int4.ToDoubles().GetMagnitude();
                            if (magnitude < maxValue)
                            {
                                maxValue = magnitude;
                                isWater = this.PassageNodes[index, k].IsWater;
                            }
                        }
                    }
                    map2.ValueData.Value[i, j] = isWater;
                }
            }
            return map2;
        }

        private clsPassageNodeNework MakePassageNodeNetwork()
        {
            int num;
            clsPassageNodeNework nework2 = new clsPassageNodeNework {
                Network = new PathfinderNetwork(),
                PassageNodePathNodes = new PathfinderNode[(this.SymmetryBlockCount - 1) + 1, (this.PassageNodeCount - 1) + 1]
            };
            int num3 = this.PassageNodeCount - 1;
            for (num = 0; num <= num3; num++)
            {
                int num4 = this.SymmetryBlockCount - 1;
                for (int i = 0; i <= num4; i++)
                {
                    nework2.PassageNodePathNodes[i, num] = new PathfinderNode(nework2.Network);
                    clsNodeTag tag = new clsNodeTag {
                        Pos = this.PassageNodes[i, num].Pos
                    };
                    nework2.PassageNodePathNodes[i, num].Tag = tag;
                }
            }
            int num5 = this.ConnectionCount - 1;
            for (num = 0; num <= num5; num++)
            {
                if (((this.Connections[num].PassageNodeA.Level == this.Connections[num].PassageNodeB.Level) | this.Connections[num].IsRamp) && !(this.Connections[num].PassageNodeA.IsWater | this.Connections[num].PassageNodeB.IsWater))
                {
                    PathfinderNode nodeA = nework2.PassageNodePathNodes[this.Connections[num].PassageNodeA.MirrorNum, this.Connections[num].PassageNodeA.Num];
                    PathfinderNode otherNode = nework2.PassageNodePathNodes[this.Connections[num].PassageNodeB.MirrorNum, this.Connections[num].PassageNodeB.Num];
                    PathfinderConnection connection = nodeA.CreateConnection(otherNode, this.GetNodePosDist(nodeA, otherNode));
                }
            }
            nework2.Network.LargeArraysResize();
            nework2.Network.FindCalc();
            return nework2;
        }

        private bool MakePassageNodes(modMath.sXY_int Pos, bool IsOnBorder)
        {
            int num;
            modMath.sXY_int _int;
            modMath.sXY_int _int3;
            modMath.sXY_int[] _intArray = new modMath.sXY_int[4];
            _int3.X = (int) Math.Round((double) (((double) (this.TileSize.X * 0x80)) / ((double) this.SymmetryBlockCountXY.X)));
            _int3.Y = (int) Math.Round((double) (((double) (this.TileSize.Y * 0x80)) / ((double) this.SymmetryBlockCountXY.Y)));
            _int.X = _int3.X - 1;
            _int.Y = _int3.Y - 1;
            int num3 = this.SymmetryBlockCount - 1;
            for (num = 0; num <= num3; num++)
            {
                modMath.sXY_int _int2 = TileOrientation.GetRotatedPos(this.SymmetryBlocks[num].Orientation, Pos, _int);
                _intArray[num].X = (this.SymmetryBlocks[num].XYNum.X * _int3.X) + _int2.X;
                _intArray[num].Y = (this.SymmetryBlocks[num].XYNum.Y * _int3.Y) + _int2.Y;
                int num4 = num - 1;
                for (int i = 0; i <= num4; i++)
                {
                    modMath.sXY_int _int4 = _intArray[num] - _intArray[i];
                    if (_int4.ToDoubles().GetMagnitude() < ((this.NodeScale * 128f) * 2.0))
                    {
                        return false;
                    }
                }
            }
            int num5 = this.SymmetryBlockCount - 1;
            for (num = 0; num <= num5; num++)
            {
                clsPassageNode node = new clsPassageNode();
                this.PassageNodes[num, this.PassageNodeCount] = node;
                node.Num = this.PassageNodeCount;
                node.MirrorNum = num;
                node.Pos = _intArray[num];
                node.IsOnBorder = IsOnBorder;
            }
            this.PassageNodeCount++;
            return true;
        }

        private void OilBalanceLoop(clsOilBalanceLoopArgs Args, int LoopNum)
        {
            int loopNum = LoopNum + 1;
            int num4 = this.PassageNodeCount - 1;
            for (int i = 0; i <= num4; i++)
            {
                clsPassageNode node = this.PassageNodes[0, i];
                if ((((node.PlayerBaseNum < 0) & !node.IsOnBorder) & (node.OilCount == 0)) & !node.IsWater)
                {
                    int num5 = LoopNum - 1;
                    int index = 0;
                    while (index <= num5)
                    {
                        if (Args.OilNodes[index] == node)
                        {
                            break;
                        }
                        index++;
                    }
                    if (index == LoopNum)
                    {
                        Args.OilNodes[LoopNum] = node;
                        if (loopNum < this.OilAtATime)
                        {
                            this.OilBalanceLoop(Args, loopNum);
                        }
                        else
                        {
                            this.OilValueCalc(Args);
                        }
                    }
                }
            }
        }

        private void OilValueCalc(clsOilBalanceLoopArgs Args)
        {
            double num;
            int num7;
            clsOilPossibilities.clsPossibility possibility = new clsOilPossibilities.clsPossibility();
            double[] numArray = new double[(this.TopLeftPlayerCount - 1) + 1];
            possibility.PlayerOilScoreAddition = new double[(this.TopLeftPlayerCount - 1) + 1];
            double maxValue = double.MaxValue;
            int num16 = this.TopLeftPlayerCount - 1;
            int index = 0;
            while (index <= num16)
            {
                possibility.PlayerOilScoreAddition[index] = 0.0;
                index++;
            }
            int num17 = this.OilAtATime - 1;
            for (num7 = 0; num7 <= num17; num7++)
            {
                double num8;
                int num10;
                int num13;
                double[] playerOilScoreAddition;
                int num25;
                int num6 = Args.OilNodes[num7].Num;
                int num18 = this.OilAtATime - 1;
                int num11 = num7 + 1;
                while (num11 <= num18)
                {
                    num10 = Args.OilNodes[num11].Num;
                    num8 = 4.0 * this.PassageNodeDists[0, num6, 0, num10];
                    if (num8 < maxValue)
                    {
                        maxValue = num8;
                    }
                    num11++;
                }
                int num19 = this.OilAtATime - 1;
                for (num11 = 0; num11 <= num19; num11++)
                {
                    num10 = Args.OilNodes[num11].Num;
                    int num20 = this.SymmetryBlockCount - 1;
                    num13 = 1;
                    while (num13 <= num20)
                    {
                        num8 = 4.0 * this.PassageNodeDists[0, num6, num13, num10];
                        if (num8 < maxValue)
                        {
                            maxValue = num8;
                        }
                        num13++;
                    }
                }
                int num21 = this.PassageNodeCount - 1;
                for (int i = 0; i <= num21; i++)
                {
                    int num22 = this.SymmetryBlockCount - 1;
                    num13 = 0;
                    while (num13 <= num22)
                    {
                        if (this.PassageNodes[num13, i].OilCount > 0)
                        {
                            double num9;
                            num8 = (4.0 * num9) / ((double) this.PassageNodeDists[0, num6, num13, i]);
                            if (num8 < maxValue)
                            {
                                maxValue = num8;
                            }
                        }
                        num13++;
                    }
                }
                int num23 = this.TopLeftPlayerCount - 1;
                index = 0;
                while (index <= num23)
                {
                    numArray[index] = 0.0;
                    int num24 = this.SymmetryBlockCount - 1;
                    for (num13 = 0; num13 <= num24; num13++)
                    {
                        modMath.sXY_int _int = this.PlayerBases[index].Nodes[0].Pos - this.PassageNodes[num13, num6].Pos;
                        num = (this.PassageNodeDists[0, this.PlayerBases[index].Nodes[0].Num, num13, num6] * 2.0) + _int.ToDoubles().GetMagnitude();
                        playerOilScoreAddition = numArray;
                        num25 = index;
                        playerOilScoreAddition[num25] += 100.0 / num;
                    }
                    index++;
                }
                int num26 = this.TopLeftPlayerCount - 1;
                index = 0;
                while (index <= num26)
                {
                    playerOilScoreAddition = possibility.PlayerOilScoreAddition;
                    num25 = index;
                    playerOilScoreAddition[num25] += Args.OilClusterSizes[num7] * numArray[index];
                    index++;
                }
            }
            double num3 = double.MaxValue;
            double minValue = double.MinValue;
            int num27 = this.TopLeftPlayerCount - 1;
            for (index = 0; index <= num27; index++)
            {
                num = Args.PlayerOilScore[index] + possibility.PlayerOilScoreAddition[index];
                if (num < num3)
                {
                    num3 = num;
                }
                if (num > minValue)
                {
                    minValue = num;
                }
            }
            double num14 = minValue - num3;
            if (maxValue == double.MaxValue)
            {
                maxValue = 0.0;
            }
            else
            {
                maxValue = 10.0 / maxValue;
            }
            double num15 = (this.OilDispersion * maxValue) + num14;
            possibility.Score = num15;
            possibility.Nodes = new clsPassageNode[(this.OilAtATime - 1) + 1];
            int num28 = this.OilAtATime - 1;
            for (num7 = 0; num7 <= num28; num7++)
            {
                possibility.Nodes[num7] = Args.OilNodes[num7];
            }
            Args.OilPossibilities.NewPossibility(possibility);
        }

        private bool PassageNodeHasRamp(clsPassageNode PassageNode)
        {
            int num2 = PassageNode.ConnectionCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                if (PassageNode.Connections[i].Connection.IsRamp)
                {
                    return true;
                }
            }
            return false;
        }

        private modProgram.sResult PassageNodeHeightLevel(clsPassageNodeHeightLevelArgs Args)
        {
            int num2;
            bool flag;
            int num4;
            modProgram.sResult result2;
            clsPassageNode other;
            int num6;
            int[] mapLevelCount;
            int level;
            result2.Problem = "";
            result2.Success = false;
            int[] numArray2 = new int[(this.LevelCount - 1) + 1];
            int[] numArray = new int[(this.LevelCount - 1) + 1];
            int num7 = Args.PassageNode.ConnectionCount - 1;
            for (num2 = 0; num2 <= num7; num2++)
            {
                other = Args.PassageNode.Connections[num2].GetOther();
                if (other.Level >= 0)
                {
                    mapLevelCount = numArray2;
                    level = other.Level;
                    mapLevelCount[level]++;
                    flag = true;
                }
                if (other.IsWater)
                {
                    num6++;
                }
            }
            if (num6 > 0)
            {
                num4 = 0;
            }
            else
            {
                int num;
                int num3;
                if (Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num] > Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num])
                {
                    result2.Problem = "Error: Min height more than max.";
                    return result2;
                }
                if (!flag)
                {
                    num = 0x7fffffff;
                    num3 = 0;
                    int num9 = Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num];
                    for (num2 = Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num]; num2 <= num9; num2++)
                    {
                        if (Args.MapLevelCount[num2] < num)
                        {
                            num = Args.MapLevelCount[num2];
                            numArray[0] = num2;
                            num3 = 1;
                        }
                        else if (Args.MapLevelCount[num2] == num)
                        {
                            numArray[num3] = num2;
                            num3++;
                        }
                    }
                    num4 = numArray[(int) Math.Round((double) ((float) (App.Random.Next() * num3)))];
                }
                else
                {
                    int num5 = (int) Math.Round((double) ((float) (App.Random.Next() * Args.ActionTotal)));
                    if (num5 < Args.FlatsCutoff)
                    {
                        num = 0;
                        num3 = 0;
                        int num10 = Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num];
                        for (num2 = Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num]; num2 <= num10; num2++)
                        {
                            if (numArray2[num2] > num)
                            {
                                num = numArray2[num2];
                                numArray[0] = num2;
                                num3 = 1;
                            }
                            else if (numArray2[num2] == num)
                            {
                                numArray[num3] = num2;
                                num3++;
                            }
                        }
                    }
                    else if (num5 < Args.PassagesCutoff)
                    {
                        num3 = 0;
                        int num11 = Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num];
                        for (num2 = Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num]; num2 <= num11; num2++)
                        {
                            if (numArray2[num2] == 1)
                            {
                                numArray[num3] = num2;
                                num3++;
                                continue;
                            }
                            if (numArray2[num2] == 2)
                            {
                                num3 = 0;
                                clsPassageNode node2 = null;
                                int num12 = Args.PassageNode.ConnectionCount - 1;
                                num = 0;
                                while (num <= num12)
                                {
                                    other = Args.PassageNode.Connections[num].GetOther();
                                    if (other.Level == num2)
                                    {
                                        if (node2 == null)
                                        {
                                            node2 = other;
                                        }
                                        else
                                        {
                                            if (node2.FindConnection(other) == null)
                                            {
                                                numArray[num3] = num2;
                                                num3++;
                                            }
                                            break;
                                        }
                                    }
                                    num++;
                                }
                                if (num == Args.PassageNode.ConnectionCount)
                                {
                                    Interaction.MsgBox("Error: two nodes not found", MsgBoxStyle.ApplicationModal, null);
                                }
                            }
                        }
                    }
                    else if (num5 < Args.VariationCutoff)
                    {
                        num3 = 0;
                    }
                    else
                    {
                        result2.Problem = "Error: Random number out of range.";
                        return result2;
                    }
                    if (num3 == 0)
                    {
                        num = 0x7fffffff;
                        num3 = 0;
                        int num13 = Args.PassageNodesMaxLevel.Nodes[Args.PassageNode.Num];
                        for (num2 = Args.PassageNodesMinLevel.Nodes[Args.PassageNode.Num]; num2 <= num13; num2++)
                        {
                            if (numArray2[num2] < num)
                            {
                                num = numArray2[num2];
                                numArray[0] = num2;
                                num3 = 1;
                            }
                            else if (numArray2[num2] == num)
                            {
                                numArray[num3] = num2;
                                num3++;
                            }
                        }
                    }
                    num4 = numArray[(int) Math.Round((double) ((float) (App.Random.Next() * num3)))];
                }
            }
            int num14 = this.SymmetryBlockCount - 1;
            for (num2 = 0; num2 <= num14; num2++)
            {
                this.PassageNodes[num2, Args.PassageNode.Num].Level = num4;
            }
            this.PassageNodesMinLevelSet(Args.PassageNode, Args.PassageNodesMinLevel, num4, this.MaxLevelTransition);
            this.PassageNodesMaxLevelSet(Args.PassageNode, Args.PassageNodesMaxLevel, num4, this.MaxLevelTransition);
            mapLevelCount = Args.MapLevelCount;
            level = num4;
            mapLevelCount[level]++;
            result2.Success = true;
            return result2;
        }

        private void PassageNodesMaxLevelSet(clsPassageNode PassageNode, clsPassageNodeLevels PassageNodesMaxLevel, int Level, int LevelChange)
        {
            if (Level < PassageNodesMaxLevel.Nodes[PassageNode.Num])
            {
                PassageNodesMaxLevel.Nodes[PassageNode.Num] = Level;
                int num2 = PassageNode.ConnectionCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    clsPassageNode other = PassageNode.Connections[i].GetOther();
                    if (PassageNode.IsNearBorder | other.IsNearBorder)
                    {
                        this.PassageNodesMaxLevelSet(other, PassageNodesMaxLevel, Level + 1, LevelChange);
                    }
                    else
                    {
                        this.PassageNodesMaxLevelSet(other, PassageNodesMaxLevel, Level + LevelChange, LevelChange);
                    }
                }
            }
        }

        private void PassageNodesMinLevelSet(clsPassageNode PassageNode, clsPassageNodeLevels PassageNodesMinLevel, int Level, int LevelChange)
        {
            if (Level > PassageNodesMinLevel.Nodes[PassageNode.Num])
            {
                PassageNodesMinLevel.Nodes[PassageNode.Num] = Level;
                int num2 = PassageNode.ConnectionCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    clsPassageNode other = PassageNode.Connections[i].GetOther();
                    if (PassageNode.IsNearBorder | other.IsNearBorder)
                    {
                        this.PassageNodesMinLevelSet(other, PassageNodesMinLevel, Level - 1, LevelChange);
                    }
                    else
                    {
                        this.PassageNodesMinLevelSet(other, PassageNodesMinLevel, Level - LevelChange, LevelChange);
                    }
                }
            }
        }

        public clsMap.clsUnit PlaceUnit(clsUnitType Type, modProgram.sWorldPos Pos, clsMap.clsUnitGroup UnitGroup, int Rotation)
        {
            modMath.sXY_int _int;
            modMath.sXY_int _int3;
            modMath.sXY_int _int4;
            clsMap.clsUnitAdd add = new clsMap.clsUnitAdd {
                Map = this.Map,
                StoreChange = true
            };
            clsMap.clsUnit unit = new clsMap.clsUnit();
            add.NewUnit = unit;
            unit.Type = Type;
            unit.UnitGroup = UnitGroup;
            _int.X = (int) Math.Round(((double) (((double) Pos.Horizontal.X) / 128.0)));
            _int.Y = (int) Math.Round(((double) (((double) Pos.Horizontal.Y) / 128.0)));
            modMath.sXY_int _int2 = Type.get_GetFootprintSelected(Rotation);
            unit.Pos = Pos;
            _int3.X = (int) Math.Round(((double) (((((double) unit.Pos.Horizontal.X) / 128.0) - (((double) _int2.X) / 2.0)) + 0.5)));
            _int3.Y = (int) Math.Round(((double) (((((double) unit.Pos.Horizontal.Y) / 128.0) - (((double) _int2.Y) / 2.0)) + 0.5)));
            _int4.X = (int) Math.Round(((double) (((((double) unit.Pos.Horizontal.X) / 128.0) + (((double) _int2.X) / 2.0)) - 0.5)));
            _int4.Y = (int) Math.Round(((double) (((((double) unit.Pos.Horizontal.Y) / 128.0) + (((double) _int2.Y) / 2.0)) - 0.5)));
            unit.Rotation = Rotation;
            add.Perform();
            int num3 = Math.Min(_int4.Y, this.Map.Terrain.TileSize.Y - 1);
            for (int i = Math.Max(_int3.Y, 0); i <= num3; i++)
            {
                int num4 = Math.Min(_int4.X, this.Map.Terrain.TileSize.X - 1);
                for (int j = Math.Max(_int3.X, 0); j <= num4; j++)
                {
                    this.TileNodeBlock(j, i);
                }
            }
            this.TilePathMap.FindCalc();
            return unit;
        }

        public clsMap.clsUnit PlaceUnitNear(clsUnitType Type, modMath.sXY_int Pos, clsMap.clsUnitGroup UnitGroup, int Clearance, int Rotation, int MaxDistFromPos)
        {
            modMath.sXY_int _int;
            modMath.sXY_int _int3;
            modMath.sXY_int _int4;
            PathfinderNode node = this.GetNearestNodeConnection(this.TilePathMap, Pos, Clearance, (float) MaxDistFromPos);
            if (node == null)
            {
                return null;
            }
            clsNodeTag tag = (clsNodeTag) node.Tag;
            modMath.sXY_int _int5 = Pos - tag.Pos;
            if (_int5.ToDoubles().GetMagnitude() > MaxDistFromPos)
            {
                return null;
            }
            clsMap.clsUnitAdd add = new clsMap.clsUnitAdd {
                Map = this.Map,
                StoreChange = true
            };
            clsMap.clsUnit unit2 = new clsMap.clsUnit();
            add.NewUnit = unit2;
            unit2.Type = Type;
            unit2.UnitGroup = UnitGroup;
            _int.X = (int) Math.Round(((double) (((double) tag.Pos.X) / 128.0)));
            _int.Y = (int) Math.Round(((double) (((double) tag.Pos.Y) / 128.0)));
            modMath.sXY_int _int2 = Type.get_GetFootprintSelected(Rotation);
            int num = _int2.X % 2;
            if (num > 0)
            {
                unit2.Pos.Horizontal.X = tag.Pos.X;
            }
            else if (App.Random.Next() >= 0.5f)
            {
                unit2.Pos.Horizontal.X = tag.Pos.X - 0x40;
            }
            else
            {
                unit2.Pos.Horizontal.X = tag.Pos.X + 0x40;
            }
            num = _int2.Y % 2;
            if (num > 0)
            {
                unit2.Pos.Horizontal.Y = tag.Pos.Y;
            }
            else if (App.Random.Next() >= 0.5f)
            {
                unit2.Pos.Horizontal.Y = tag.Pos.Y - 0x40;
            }
            else
            {
                unit2.Pos.Horizontal.Y = tag.Pos.Y + 0x40;
            }
            _int3.X = (int) Math.Round(((double) (((((double) unit2.Pos.Horizontal.X) / 128.0) - (((double) _int2.X) / 2.0)) + 0.5)));
            _int3.Y = (int) Math.Round(((double) (((((double) unit2.Pos.Horizontal.Y) / 128.0) - (((double) _int2.Y) / 2.0)) + 0.5)));
            _int4.X = (int) Math.Round(((double) (((((double) unit2.Pos.Horizontal.X) / 128.0) + (((double) _int2.X) / 2.0)) - 0.5)));
            _int4.Y = (int) Math.Round(((double) (((((double) unit2.Pos.Horizontal.Y) / 128.0) + (((double) _int2.Y) / 2.0)) - 0.5)));
            unit2.Rotation = Rotation;
            add.Perform();
            int num4 = Math.Min(_int4.Y, this.Map.Terrain.TileSize.Y - 1);
            for (int i = Math.Max(_int3.Y, 0); i <= num4; i++)
            {
                int num5 = Math.Min(_int4.X, this.Map.Terrain.TileSize.X - 1);
                for (int j = Math.Max(_int3.X, 0); j <= num5; j++)
                {
                    this.TileNodeBlock(j, i);
                }
            }
            this.TilePathMap.FindCalc();
            return unit2;
        }

        private void SetBaseLevel(PathfinderNode Node, int NewLevel, clsBaseNodeLevels BaseLevel)
        {
            if (Node.GetChildNodeCount == 0)
            {
                float num3 = NewLevel;
                int num5 = Node.GetConnectionCount - 1;
                for (int i = 0; i <= num5; i++)
                {
                    float num2 = BaseLevel.NodeLevels[Node.get_GetConnection(i).GetOtherNode(Node).GetLayer_NodeNum];
                    if (num2 < num3)
                    {
                        num3 = num2;
                    }
                }
                if ((NewLevel - num3) > 1f)
                {
                    BaseLevel.NodeLevels[Node.GetLayer_NodeNum] = num3 + 1f;
                }
                else
                {
                    BaseLevel.NodeLevels[Node.GetLayer_NodeNum] = NewLevel;
                }
            }
            else
            {
                int num6 = Node.GetChildNodeCount - 1;
                for (int j = 0; j <= num6; j++)
                {
                    this.SetBaseLevel(Node.get_GetChildNode(j), NewLevel, BaseLevel);
                }
            }
        }

        private void SetBaseLevelRamp(clsSetBaseLevelRampArgs Args, PathfinderNode Node)
        {
            if (Node.GetChildNodeCount == 0)
            {
                clsNodeTag tag = (clsNodeTag) Node.Tag;
                modMath.sXY_int _int = modMath.PointGetClosestPosOnLine(Args.Connection.PassageNodeA.Pos, Args.Connection.PassageNodeB.Pos, tag.Pos);
                modMath.sXY_int _int2 = Args.Connection.PassageNodeA.Pos - Args.Connection.PassageNodeB.Pos;
                float magnitude = (float) _int2.ToDoubles().GetMagnitude();
                float num3 = magnitude - Args.RampLength;
                _int2 = _int - Args.Connection.PassageNodeA.Pos;
                float num2 = (float) _int2.ToDoubles().GetMagnitude();
                float num5 = modMath.Clamp_sng((num2 - (num3 / 2f)) / ((float) Args.RampLength), 0f, 1f);
                int index = Node.GetLayer_NodeNum;
                num5 = (float) (1.0 - ((Math.Cos(num5 * 3.1415926535897931) + 1.0) / 2.0));
                if ((num5 > 0f) & (num5 < 1f))
                {
                    _int2 = tag.Pos - _int;
                    float num6 = (float) _int2.ToDoubles().GetMagnitude();
                    if (num6 < Args.RampRadius)
                    {
                        float num7 = 1f;
                        if (Args.BaseLevel.NodeLevels[index] == (Args.BaseLevel.NodeLevels[index]))
                        {
                            Args.BaseLevel.NodeLevels[index] = (Args.BaseLevel.NodeLevels[index] * (1f - num7)) + (((Args.Connection.PassageNodeA.Level * (1f - num5)) + (Args.Connection.PassageNodeB.Level * num5)) * num7);
                        }
                        else
                        {
                            Args.BaseLevel.NodeLevels[index] = ((Args.BaseLevel.NodeLevels[index] * (2f - num7)) + (((Args.Connection.PassageNodeA.Level * (1f - num5)) + (Args.Connection.PassageNodeB.Level * num5)) * num7)) / 2f;
                        }
                    }
                }
            }
            else
            {
                int num9 = Node.GetChildNodeCount - 1;
                for (int i = 0; i <= num9; i++)
                {
                    this.SetBaseLevelRamp(Args, Node.get_GetChildNode(i));
                }
            }
        }

        public void TerrainBlockPaths()
        {
            int num3 = this.Map.Terrain.TileSize.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = this.Map.Terrain.TileSize.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    if ((this.Map.Terrain.Tiles[j, i].Texture.TextureNum >= 0) && ((this.GenerateTileset.Tileset.Tiles[this.Map.Terrain.Tiles[j, i].Texture.TextureNum].Default_Type == 8) | (this.GenerateTileset.Tileset.Tiles[this.Map.Terrain.Tiles[j, i].Texture.TextureNum].Default_Type == 7)))
                    {
                        this.TileNodeBlock(j, i);
                    }
                }
            }
            this.TilePathMap.FindCalc();
        }

        private bool TestNearest(clsTestNearestArgs Args)
        {
            int num;
            int num2;
            modMath.sXY_int _int;
            if (Args.PassageNodeA.MirrorNum != 0)
            {
                Debugger.Break();
                return false;
            }
            _int.X = Args.PassageNodeB.Pos.X - Args.PassageNodeA.Pos.X;
            _int.Y = Args.PassageNodeB.Pos.Y - Args.PassageNodeA.Pos.Y;
            int num3 = (_int.X * _int.X) + (_int.Y * _int.Y);
            if (num3 > Args.MaxConDist2)
            {
                return false;
            }
            int num6 = this.PassageNodeCount - 1;
            for (num = 0; num <= num6; num++)
            {
                int num7 = this.SymmetryBlockCount - 1;
                for (num2 = 0; num2 <= num7; num2++)
                {
                    if ((this.PassageNodes[num2, num] != Args.PassageNodeA) & (this.PassageNodes[num2, num] != Args.PassageNodeB))
                    {
                        modMath.sXY_int _int2 = modMath.PointGetClosestPosOnLine(Args.PassageNodeA.Pos, Args.PassageNodeB.Pos, this.PassageNodes[num2, num].Pos) - this.PassageNodes[num2, num].Pos;
                        if (_int2.ToDoubles().GetMagnitude() < Args.MinConDist)
                        {
                            return false;
                        }
                    }
                }
            }
            clsNearest nearest = new clsNearest();
            clsNearest nearest2 = nearest;
            nearest2.Num = this.NearestCount;
            nearest2.Dist2 = num3;
            if (Args.PassageNodeA.MirrorNum == Args.PassageNodeB.MirrorNum)
            {
                nearest2.NodeA = new clsPassageNode[(this.SymmetryBlockCount - 1) + 1];
                nearest2.NodeB = new clsPassageNode[(this.SymmetryBlockCount - 1) + 1];
                int num8 = this.SymmetryBlockCount - 1;
                for (num = 0; num <= num8; num++)
                {
                    nearest2.NodeA[num] = this.PassageNodes[num, Args.PassageNodeA.Num];
                    nearest2.NodeB[num] = this.PassageNodes[num, Args.PassageNodeB.Num];
                }
                nearest2.NodeCount = this.SymmetryBlockCount;
            }
            else
            {
                int num4;
                int num5;
                if (!this.SymmetryIsRotational)
                {
                    if (Args.PassageNodeA.Num != Args.PassageNodeB.Num)
                    {
                        return false;
                    }
                    if (this.SymmetryBlockCount != 4)
                    {
                        nearest2.NodeA = new clsPassageNode[1];
                        nearest2.NodeB = new clsPassageNode[1];
                        nearest2.NodeA[0] = Args.PassageNodeA;
                        nearest2.NodeB[0] = Args.PassageNodeB;
                        nearest2.NodeCount = 1;
                        goto Label_04A8;
                    }
                    nearest2.NodeA = new clsPassageNode[2];
                    nearest2.NodeB = new clsPassageNode[2];
                    num4 = (int) Math.Round((double) (((double) this.SymmetryBlockCount) / 2.0));
                    int num11 = num4 - 1;
                    for (num5 = 0; num5 <= num11; num5++)
                    {
                        if (this.SymmetryBlocks[0].ReflectToNum[num5] == Args.PassageNodeB.MirrorNum)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    nearest2.NodeA = new clsPassageNode[(this.SymmetryBlockCount - 1) + 1];
                    nearest2.NodeB = new clsPassageNode[(this.SymmetryBlockCount - 1) + 1];
                    num4 = (int) Math.Round((double) (((double) this.SymmetryBlockCount) / 2.0));
                    int num9 = num4 - 1;
                    num5 = 0;
                    while (num5 <= num9)
                    {
                        if (this.SymmetryBlocks[0].ReflectToNum[num5] == Args.PassageNodeB.MirrorNum)
                        {
                            break;
                        }
                        num5++;
                    }
                    if (num5 == num4)
                    {
                        return false;
                    }
                    int num10 = this.SymmetryBlockCount - 1;
                    for (num = 0; num <= num10; num++)
                    {
                        nearest2.NodeA[num] = this.PassageNodes[num, Args.PassageNodeA.Num];
                        nearest2.NodeB[num] = this.PassageNodes[this.SymmetryBlocks[num].ReflectToNum[num5], Args.PassageNodeB.Num];
                    }
                    nearest2.NodeCount = this.SymmetryBlockCount;
                    goto Label_04A8;
                }
                if (num5 == num4)
                {
                    return false;
                }
                nearest2.NodeA[0] = Args.PassageNodeA;
                nearest2.NodeB[0] = Args.PassageNodeB;
                num2 = this.SymmetryBlocks[0].ReflectToNum[1 - num5];
                nearest2.NodeA[1] = this.PassageNodes[num2, Args.PassageNodeA.Num];
                nearest2.NodeB[1] = this.PassageNodes[this.SymmetryBlocks[num2].ReflectToNum[num5], Args.PassageNodeB.Num];
                nearest2.NodeCount = 2;
            }
        Label_04A8:
            nearest2.BlockedNearests = new clsNearest[0x200];
            nearest2 = null;
            this.Nearests[this.NearestCount] = nearest;
            this.NearestCount++;
            return true;
        }

        public void TileNodeBlock(int X, int Y)
        {
            int num3 = Math.Min((int) (Y + 6), (int) (this.Map.Terrain.TileSize.Y - 1));
            for (int i = Math.Max(Y - 6, 0); i <= num3; i++)
            {
                int num4 = Math.Min((int) (X + 6), (int) (this.Map.Terrain.TileSize.X - 1));
                for (int j = Math.Max(X - 6, 0); j <= num4; j++)
                {
                    this.GenerateTerrainTiles[j, i].Node.ClearanceSet(Math.Min(this.GenerateTerrainTiles[j, i].Node.GetClearance, Math.Max(Math.Abs((int) (i - Y)), Math.Abs((int) (j - X)))));
                }
            }
            if (this.GenerateTerrainTiles[X, Y].TopLeftLink != null)
            {
                this.GenerateTerrainTiles[X, Y].TopLeftLink.Destroy();
                this.GenerateTerrainTiles[X, Y].TopLeftLink = null;
            }
            if (this.GenerateTerrainTiles[X, Y].TopLink != null)
            {
                this.GenerateTerrainTiles[X, Y].TopLink.Destroy();
                this.GenerateTerrainTiles[X, Y].TopLink = null;
            }
            if (this.GenerateTerrainTiles[X, Y].TopRightLink != null)
            {
                this.GenerateTerrainTiles[X, Y].TopRightLink.Destroy();
                this.GenerateTerrainTiles[X, Y].TopRightLink = null;
            }
            if (this.GenerateTerrainTiles[X, Y].RightLink != null)
            {
                this.GenerateTerrainTiles[X, Y].RightLink.Destroy();
                this.GenerateTerrainTiles[X, Y].RightLink = null;
            }
            if (this.GenerateTerrainTiles[X, Y].BottomRightLink != null)
            {
                this.GenerateTerrainTiles[X, Y].BottomRightLink.Destroy();
                this.GenerateTerrainTiles[X, Y].BottomRightLink = null;
            }
            if (this.GenerateTerrainTiles[X, Y].BottomLink != null)
            {
                this.GenerateTerrainTiles[X, Y].BottomLink.Destroy();
                this.GenerateTerrainTiles[X, Y].BottomLink = null;
            }
            if (this.GenerateTerrainTiles[X, Y].BottomLeftLink != null)
            {
                this.GenerateTerrainTiles[X, Y].BottomLeftLink.Destroy();
                this.GenerateTerrainTiles[X, Y].BottomLeftLink = null;
            }
            if (this.GenerateTerrainTiles[X, Y].LeftLink != null)
            {
                this.GenerateTerrainTiles[X, Y].LeftLink.Destroy();
                this.GenerateTerrainTiles[X, Y].LeftLink = null;
            }
        }

        private void UpdateNetworkConnectedness(clsUpdateNetworkConnectednessArgs Args, clsPassageNode PassageNode)
        {
            clsUpdateNodeConnectednessArgs args = new clsUpdateNodeConnectednessArgs();
            Args.PassageNodeUpdated[PassageNode.Num] = true;
            int num4 = PassageNode.ConnectionCount - 1;
            for (int i = 0; i <= num4; i++)
            {
                clsConnection connection = PassageNode.Connections[i].Connection;
                if (!(((connection.PassageNodeA.IsOnBorder | connection.PassageNodeB.IsOnBorder) | connection.PassageNodeA.IsWater) | connection.PassageNodeB.IsWater) & (connection.IsRamp | (connection.PassageNodeA.Level == connection.PassageNodeB.Level)))
                {
                    clsPassageNode other = PassageNode.Connections[i].GetOther();
                    if (!Args.PassageNodeUpdated[other.Num] & (other.MirrorNum == 0))
                    {
                        int num5 = this.PassageNodeCount - 1;
                        for (int j = 0; j <= num5; j++)
                        {
                            int num6 = Args.SymmetryBlockCount - 1;
                            for (int k = 0; k <= num6; k++)
                            {
                                Args.Args.PassageNodeVisited[k, j] = false;
                            }
                        }
                        args.OriginalNode = PassageNode;
                        args.Args = Args.Args;
                        this.UpdateNodeConnectedness(args, PassageNode);
                    }
                }
            }
        }

        private void UpdateNodeConnectedness(clsUpdateNodeConnectednessArgs Args, clsPassageNode PassageNode)
        {
            int num2;
            Args.Args.PassageNodeVisited[PassageNode.MirrorNum, PassageNode.Num] = true;
            int num3 = PassageNode.ConnectionCount - 1;
            for (int i = 0; i <= num3; i++)
            {
                clsConnection connection = PassageNode.Connections[i].Connection;
                if (!(((connection.PassageNodeA.IsOnBorder | connection.PassageNodeB.IsOnBorder) | connection.PassageNodeA.IsWater) | connection.PassageNodeB.IsWater) & (connection.IsRamp | (connection.PassageNodeA.Level == connection.PassageNodeB.Level)))
                {
                    clsPassageNode other = PassageNode.Connections[i].GetOther();
                    if (!Args.Args.PassageNodeVisited[other.MirrorNum, other.Num])
                    {
                        this.UpdateNodeConnectedness(Args, other);
                    }
                    num2++;
                }
            }
            PathfinderNode[] startNodes = new PathfinderNode[] { Args.Args.PassageNodePathNodes[0, Args.OriginalNode.Num] };
            PathfinderNetwork.PathList[] listArray = Args.Args.PassageNodePathMap.GetPath(startNodes, Args.Args.PassageNodePathNodes[PassageNode.MirrorNum, PassageNode.Num], -1, 0);
            float[] nodeConnectedness = Args.Args.NodeConnectedness;
            int num = Args.OriginalNode.Num;
            nodeConnectedness[num] += (float) (num2 * Math.Pow(0.999, (double) listArray[0].Paths[0].Value));
        }

        public int GetTotalPlayerCount
        {
            get
            {
                return this.TotalPlayerCount;
            }
        }

        private class clsBaseNodeLevels
        {
            public float[] NodeLevels;
        }

        public class clsConnection
        {
            public bool IsRamp;
            public clsGenerateMap.clsPassageNode PassageNodeA;
            public int PassageNodeA_ConnectionNum;
            public clsGenerateMap.clsPassageNode PassageNodeB;
            public int PassageNodeB_ConnectionNum;
            public int ReflectionCount;
            public clsGenerateMap.clsConnection[] Reflections;

            public clsConnection(clsGenerateMap.clsPassageNode NewPassageNodeA, clsGenerateMap.clsPassageNode NewPassageNodeB)
            {
                clsGenerateMap.clsPassageNode.sConnection connection;
                this.PassageNodeA_ConnectionNum = -1;
                this.PassageNodeB_ConnectionNum = -1;
                this.PassageNodeA = NewPassageNodeA;
                connection.Connection = this;
                connection.IsB = false;
                this.PassageNodeA.Connection_Add(connection);
                this.PassageNodeB = NewPassageNodeB;
                connection.Connection = this;
                connection.IsB = true;
                this.PassageNodeB.Connection_Add(connection);
            }
        }

        private class clsNearest
        {
            public int BlockedCount;
            public int BlockedNearestCount;
            public clsGenerateMap.clsNearest[] BlockedNearests;
            public float Dist2;
            public bool Invalid;
            public clsGenerateMap.clsPassageNode[] NodeA;
            public clsGenerateMap.clsPassageNode[] NodeB;
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
            public modMath.sXY_int Pos;
        }

        private class clsOilBalanceLoopArgs
        {
            public int[] OilClusterSizes;
            public clsGenerateMap.clsPassageNode[] OilNodes;
            public clsGenerateMap.clsOilPossibilities OilPossibilities;
            public double[] PlayerOilScore;
        }

        public class clsOilPossibilities
        {
            public clsPossibility BestPossibility;

            public void NewPossibility(clsPossibility Possibility)
            {
                if (this.BestPossibility == null)
                {
                    this.BestPossibility = Possibility;
                }
                else if (Possibility.Score < this.BestPossibility.Score)
                {
                    this.BestPossibility = Possibility;
                }
            }

            public class clsPossibility
            {
                public clsGenerateMap.clsPassageNode[] Nodes;
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
            public modMath.sXY_int Pos;

            public void CalcIsNearBorder()
            {
                int num2 = this.ConnectionCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    if (this.Connections[i].GetOther().IsOnBorder)
                    {
                        this.IsNearBorder = true;
                        return;
                    }
                }
                this.IsNearBorder = false;
            }

            public void Connection_Add(sConnection NewConnection)
            {
                if (NewConnection.IsB)
                {
                    NewConnection.Connection.PassageNodeB_ConnectionNum = this.ConnectionCount;
                }
                else
                {
                    NewConnection.Connection.PassageNodeA_ConnectionNum = this.ConnectionCount;
                }
                this.Connections = (sConnection[]) Utils.CopyArray((Array) this.Connections, new sConnection[this.ConnectionCount + 1]);
                this.Connections[this.ConnectionCount] = NewConnection;
                this.ConnectionCount++;
            }

            public void Connection_Remove(int Num)
            {
                if (this.Connections[Num].IsB)
                {
                    this.Connections[Num].Connection.PassageNodeB_ConnectionNum = -1;
                }
                else
                {
                    this.Connections[Num].Connection.PassageNodeA_ConnectionNum = -1;
                }
                this.ConnectionCount--;
                if (Num != this.ConnectionCount)
                {
                    this.Connections[Num] = this.Connections[this.ConnectionCount];
                    if (this.Connections[Num].IsB)
                    {
                        this.Connections[Num].Connection.PassageNodeB_ConnectionNum = Num;
                    }
                    else
                    {
                        this.Connections[Num].Connection.PassageNodeA_ConnectionNum = Num;
                    }
                }
            }

            public clsGenerateMap.clsConnection FindConnection(clsGenerateMap.clsPassageNode PassageNode)
            {
                int num2 = this.ConnectionCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    if (this.Connections[i].GetOther() == PassageNode)
                    {
                        return this.Connections[i].Connection;
                    }
                }
                return null;
            }

            public void ReorderConnections()
            {
                int num;
                sConnection[] connectionArray = new sConnection[(this.ConnectionCount - 1) + 1];
                double[] numArray = new double[(this.ConnectionCount - 1) + 1];
                int num5 = this.ConnectionCount - 1;
                for (num = 0; num <= num5; num++)
                {
                    modMath.sXY_int _int;
                    clsGenerateMap.clsPassageNode other = this.Connections[num].GetOther();
                    _int.X = other.Pos.X - this.Pos.X;
                    _int.Y = other.Pos.Y - this.Pos.Y;
                    double angle = _int.ToDoubles().GetAngle();
                    int num6 = num - 1;
                    int index = 0;
                    while (index <= num6)
                    {
                        if (angle < numArray[index])
                        {
                            break;
                        }
                        index++;
                    }
                    int num7 = index;
                    for (int i = num - 1; i >= num7; i += -1)
                    {
                        connectionArray[i + 1] = connectionArray[i];
                        numArray[i + 1] = numArray[i];
                    }
                    connectionArray[index] = this.Connections[num];
                    numArray[index] = angle;
                }
                int num8 = this.ConnectionCount - 1;
                for (num = 0; num <= num8; num++)
                {
                    this.Connections[num] = connectionArray[num];
                    if (this.Connections[num].IsB)
                    {
                        this.Connections[num].Connection.PassageNodeB_ConnectionNum = num;
                    }
                    else
                    {
                        this.Connections[num].Connection.PassageNodeA_ConnectionNum = num;
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct sConnection
            {
                public clsGenerateMap.clsConnection Connection;
                public bool IsB;
                public clsGenerateMap.clsPassageNode GetOther()
                {
                    if (this.IsB)
                    {
                        return this.Connection.PassageNodeA;
                    }
                    return this.Connection.PassageNodeB;
                }
            }
        }

        private class clsPassageNodeHeightLevelArgs
        {
            public int ActionTotal;
            public int FlatsCutoff;
            public int[] MapLevelCount;
            public clsGenerateMap.clsPassageNode PassageNode;
            public clsGenerateMap.clsPassageNodeLevels PassageNodesMaxLevel = new clsGenerateMap.clsPassageNodeLevels();
            public clsGenerateMap.clsPassageNodeLevels PassageNodesMinLevel = new clsGenerateMap.clsPassageNodeLevels();
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
            public clsGenerateMap.clsBaseNodeLevels BaseLevel = new clsGenerateMap.clsBaseNodeLevels();
            public clsGenerateMap.clsConnection Connection;
            public int RampLength;
            public float RampRadius;
        }

        private class clsTestNearestArgs
        {
            public int MaxConDist2;
            public int MinConDist;
            public clsGenerateMap.clsPassageNode PassageNodeA;
            public clsGenerateMap.clsPassageNode PassageNodeB;
        }

        private class clsUpdateNetworkConnectednessArgs
        {
            public clsGenerateMap.clsNodeConnectedness Args;
            public bool[] PassageNodeUpdated;
            public int SymmetryBlockCount;
        }

        private class clsUpdateNodeConnectednessArgs
        {
            public clsGenerateMap.clsNodeConnectedness Args;
            public clsGenerateMap.clsPassageNode OriginalNode;
        }

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        private struct sPlayerBase
        {
            public clsGenerateMap.clsPassageNode[] Nodes;
            public int NodeCount;
            public modMath.sXY_int Pos;
            public void CalcPos()
            {
                Position.XY_dbl _dbl;
                int num2 = this.NodeCount - 1;
                for (int i = 0; i <= num2; i++)
                {
                    _dbl.X += this.Nodes[i].Pos.X;
                    _dbl.Y += this.Nodes[i].Pos.Y;
                }
                this.Pos.X = (int) Math.Round((double) (_dbl.X / ((double) this.NodeCount)));
                this.Pos.Y = (int) Math.Round((double) (_dbl.Y / ((double) this.NodeCount)));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct sPossibleGateway
        {
            public modMath.sXY_int StartPos;
            public modMath.sXY_int MiddlePos;
            public bool IsVertical;
            public int Length;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sSymmetryBlock
        {
            public modMath.sXY_int XYNum;
            public TileOrientation.sTileOrientation Orientation;
            public int[] ReflectToNum;
        }
    }
}

