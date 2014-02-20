namespace SharpFlame.Pathfinding
{
    public class LargeArrays
    {
        public bool[] Nodes_Booleans;
        public PathfinderNode[] Nodes_Nodes;
        public Path Nodes_Path = new Path();
        public float[] Nodes_ValuesA;
        public float[] Nodes_ValuesB;
        public int Size;

        public float SizeEnlargementRatio = 2.0F;
        public float SizeReductionRatio = 3.0F;

        public void Resize(PathfinderNetwork NetworkForSize)
        {
            var NewSize = 0;

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
                var Num = 0;
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
                    var Num = 0;
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