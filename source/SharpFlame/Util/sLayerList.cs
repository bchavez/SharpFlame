#region

using System;

#endregion

namespace SharpFlame.Util
{
    public enum enumDrawLighting
    {
        Off,
        Half,
        Normal
    }

    public enum enumView_Move_Type
    {
        Free,
        RTS
    }

    public struct sLayerList
    {
        public int LayerCount;
        public clsLayer[] Layers;

        public void Layer_Insert(int PositionNum, clsLayer NewLayer)
        {
            var A = 0;
            var B = 0;

            Array.Resize(ref Layers, LayerCount + 1);
            //shift the ones below down
            for ( A = LayerCount - 1; A >= PositionNum; A-- )
            {
                Layers[A + 1] = Layers[A];
            }
            //insert the new entry
            Layers[PositionNum] = NewLayer;
            LayerCount++;

            for ( A = 0; A <= LayerCount - 1; A++ )
            {
                if ( Layers[A].WithinLayer >= PositionNum )
                {
                    Layers[A].WithinLayer = Layers[A].WithinLayer + 1;
                }
                Array.Resize(ref Layers[A].AvoidLayers, LayerCount);
                for ( B = LayerCount - 2; B >= PositionNum; B-- )
                {
                    Layers[A].AvoidLayers[B + 1] = Layers[A].AvoidLayers[B];
                }
                Layers[A].AvoidLayers[PositionNum] = false;
            }
        }

        public void Layer_Remove(int Layer_Num)
        {
            var A = 0;
            var B = 0;

            LayerCount--;
            for ( A = Layer_Num; A <= LayerCount - 1; A++ )
            {
                Layers[A] = Layers[A + 1];
            }
            Array.Resize(ref Layers, LayerCount);

            for ( A = 0; A <= LayerCount - 1; A++ )
            {
                if ( Layers[A].WithinLayer == Layer_Num )
                {
                    Layers[A].WithinLayer = -1;
                }
                else if ( Layers[A].WithinLayer > Layer_Num )
                {
                    Layers[A].WithinLayer = Layers[A].WithinLayer - 1;
                }
                for ( B = Layer_Num; B <= LayerCount - 1; B++ )
                {
                    Layers[A].AvoidLayers[B] = Layers[A].AvoidLayers[B + 1];
                }
                Array.Resize(ref Layers[A].AvoidLayers, LayerCount);
            }
        }

        public void Layer_Move(int Layer_Num, int Layer_Dest_Num)
        {
            var Layer_Temp = default(clsLayer);
            var boolTemp = default(bool);
            var A = 0;
            var B = 0;

            if ( Layer_Dest_Num < Layer_Num )
            {
                //move the variables
                Layer_Temp = Layers[Layer_Num];
                for ( A = Layer_Num - 1; A >= Layer_Dest_Num; A-- )
                {
                    Layers[A + 1] = Layers[A];
                }
                Layers[Layer_Dest_Num] = Layer_Temp;
                //update the layer nums
                for ( A = 0; A <= LayerCount - 1; A++ )
                {
                    if ( Layers[A].WithinLayer == Layer_Num )
                    {
                        Layers[A].WithinLayer = Layer_Dest_Num;
                    }
                    else if ( Layers[A].WithinLayer >= Layer_Dest_Num && Layers[A].WithinLayer < Layer_Num )
                    {
                        Layers[A].WithinLayer = Layers[A].WithinLayer + 1;
                    }
                    boolTemp = Convert.ToBoolean(Layers[A].AvoidLayers[Layer_Num]);
                    for ( B = Layer_Num - 1; B >= Layer_Dest_Num; B-- )
                    {
                        Layers[A].AvoidLayers[B + 1] = Layers[A].AvoidLayers[B];
                    }
                    Layers[A].AvoidLayers[Layer_Dest_Num] = boolTemp;
                }
            }
            else if ( Layer_Dest_Num > Layer_Num )
            {
                //move the variables
                Layer_Temp = Layers[Layer_Num];
                for ( A = Layer_Num; A <= Layer_Dest_Num - 1; A++ )
                {
                    Layers[A] = Layers[A + 1];
                }
                Layers[Layer_Dest_Num] = Layer_Temp;
                //update the layer nums
                for ( A = 0; A <= LayerCount - 1; A++ )
                {
                    if ( Layers[A].WithinLayer == Layer_Num )
                    {
                        Layers[A].WithinLayer = Layer_Dest_Num;
                    }
                    else if ( Layers[A].WithinLayer > Layer_Num && Layers[A].WithinLayer <= Layer_Dest_Num )
                    {
                        Layers[A].WithinLayer = Layers[A].WithinLayer - 1;
                    }
                    boolTemp = Convert.ToBoolean(Layers[A].AvoidLayers[Layer_Num]);
                    for ( B = Layer_Num; B <= Layer_Dest_Num - 1; B++ )
                    {
                        Layers[A].AvoidLayers[B] = Layers[A].AvoidLayers[B + 1];
                    }
                    Layers[A].AvoidLayers[Layer_Dest_Num] = boolTemp;
                }
            }
        }
    }
}