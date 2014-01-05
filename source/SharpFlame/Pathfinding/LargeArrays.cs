namespace SharpFlame.Pathfinding
{
    public class LargeArrays
    {
        public bool[] Nodes_Booleans;
        public float[] Nodes_ValuesA;
        public float[] Nodes_ValuesB;
        public Path Nodes_Path = new Path();
        public PathfinderNode[] Nodes_Nodes;
        public int Size;

        public float SizeEnlargementRatio = 2.0F;
        public float SizeReductionRatio = 3.0F;

        public void Resize(PathfinderNetwork NetworkForSize)
        {
            int NewSize = 0;

            if ( NetworkForSize.NodeLayerCount > 0 )
            {
                NewSize = NetworkForSize.NodeLayers[0].NodeCount;
            }
            else
            {
                NewSize = 0;
            }
            if ( Size < NewSize )
            {
                int Num = 0;
                Size = (int)(NewSize * SizeEnlargementRatio);
                Num = Size - 1;
                Nodes_Booleans = new bool[Num + 1];
                Nodes_ValuesA = new float[Num + 1];
                Nodes_ValuesB = new float[Num + 1];
                Nodes_Booleans = new bool[Num + 1];
                Nodes_Path.Nodes = new PathfinderNode[Num + 1];
                Nodes_Nodes = new PathfinderNode[Num + 1];
            }
            else
            {
                if ( Size > NewSize * SizeReductionRatio )
                {
                    int Num = 0;
                    Size = (int)(NewSize * SizeEnlargementRatio);
                    Num = Size - 1;
                    Nodes_Booleans = new bool[Num + 1];
                    Nodes_ValuesA = new float[Num + 1];
                    Nodes_ValuesB = new float[Num + 1];
                    Nodes_Booleans = new bool[Num + 1];
                    Nodes_Path.Nodes = new PathfinderNode[Num + 1];
                    Nodes_Nodes = new PathfinderNode[Num + 1];
                }
            }
        }
    }
}