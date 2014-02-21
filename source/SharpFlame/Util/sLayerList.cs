#region

using System;

#endregion

namespace SharpFlame.Util
{
    public enum DrawLighting
    {
        Off,
        Half,
        Normal
    }

    public enum ViewMoveType
    {
        Free,
        RTS
    }

    public struct sLayerList
    {
        public int LayerCount;
        public clsLayer[] Layers;

        public void LayerInsert(int positionNum, clsLayer newLayer)
        {
            var a = 0;

            Array.Resize(ref Layers, LayerCount + 1);
            //shift the ones below down
            for ( a = LayerCount - 1; a >= positionNum; a-- )
            {
                Layers[a + 1] = Layers[a];
            }
            //insert the new entry
            Layers[positionNum] = newLayer;
            LayerCount++;

            for ( a = 0; a <= LayerCount - 1; a++ )
            {
                if ( Layers[a].WithinLayer >= positionNum )
                {
                    Layers[a].WithinLayer = Layers[a].WithinLayer + 1;
                }
                Array.Resize(ref Layers[a].AvoidLayers, LayerCount);
            
                var b = 0;
                for ( b = LayerCount - 2; b >= positionNum; b-- )
                {
                    Layers[a].AvoidLayers[b + 1] = Layers[a].AvoidLayers[b];
                }
                Layers[a].AvoidLayers[positionNum] = false;
            }
        }

        public void LayerRemove(int layerNum)
        {
            var a = 0;

            LayerCount--;
            for ( a = layerNum; a <= LayerCount - 1; a++ )
            {
                Layers[a] = Layers[a + 1];
            }
            Array.Resize(ref Layers, LayerCount);

            for ( a = 0; a <= LayerCount - 1; a++ )
            {
                if ( Layers[a].WithinLayer == layerNum )
                {
                    Layers[a].WithinLayer = -1;
                }
                else if ( Layers[a].WithinLayer > layerNum )
                {
                    Layers[a].WithinLayer = Layers[a].WithinLayer - 1;
                }

                var b = 0;
                for ( b = layerNum; b <= LayerCount - 1; b++ )
                {
                    Layers[a].AvoidLayers[b] = Layers[a].AvoidLayers[b + 1];
                }
                Array.Resize(ref Layers[a].AvoidLayers, LayerCount);
            }
        }

        public void LayerMove(int layerNum, int layerDestNum)
        {
            clsLayer layerTemp;
            bool boolTemp;
            var a = 0;
            var b = 0;

            if ( layerDestNum < layerNum )
            {
                //move the variables
                layerTemp = Layers[layerNum];
                for ( a = layerNum - 1; a >= layerDestNum; a-- )
                {
                    Layers[a + 1] = Layers[a];
                }
                Layers[layerDestNum] = layerTemp;
                //update the layer nums
                for ( a = 0; a <= LayerCount - 1; a++ )
                {
                    if ( Layers[a].WithinLayer == layerNum )
                    {
                        Layers[a].WithinLayer = layerDestNum;
                    }
                    else if ( Layers[a].WithinLayer >= layerDestNum && Layers[a].WithinLayer < layerNum )
                    {
                        Layers[a].WithinLayer = Layers[a].WithinLayer + 1;
                    }
                    boolTemp = Convert.ToBoolean(Layers[a].AvoidLayers[layerNum]);
                    for ( b = layerNum - 1; b >= layerDestNum; b-- )
                    {
                        Layers[a].AvoidLayers[b + 1] = Layers[a].AvoidLayers[b];
                    }
                    Layers[a].AvoidLayers[layerDestNum] = boolTemp;
                }
            }
            else if ( layerDestNum > layerNum )
            {
                //move the variables
                layerTemp = Layers[layerNum];
                for ( a = layerNum; a <= layerDestNum - 1; a++ )
                {
                    Layers[a] = Layers[a + 1];
                }
                Layers[layerDestNum] = layerTemp;
                //update the layer nums
                for ( a = 0; a <= LayerCount - 1; a++ )
                {
                    if ( Layers[a].WithinLayer == layerNum )
                    {
                        Layers[a].WithinLayer = layerDestNum;
                    }
                    else if ( Layers[a].WithinLayer > layerNum && Layers[a].WithinLayer <= layerDestNum )
                    {
                        Layers[a].WithinLayer = Layers[a].WithinLayer - 1;
                    }
                    boolTemp = Convert.ToBoolean(Layers[a].AvoidLayers[layerNum]);
                    for ( b = layerNum; b <= layerDestNum - 1; b++ )
                    {
                        Layers[a].AvoidLayers[b] = Layers[a].AvoidLayers[b + 1];
                    }
                    Layers[a].AvoidLayers[layerDestNum] = boolTemp;
                }
            }
        }
    }
}