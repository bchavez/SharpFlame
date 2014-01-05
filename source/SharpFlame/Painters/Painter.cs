using System;

namespace SharpFlame.Painters
{
    public class Painter
    {
        public Terrain[] Terrains;
        public int TerrainCount;

        public TransitionBrush[] TransitionBrushes;
        public int TransitionBrushCount;

        public CliffBrush[] CliffBrushes;
        public int CliffBrushCount;

        public Road[] Roads;
        public int RoadCount;

        public RoadBrush[] RoadBrushes;
        public int RoadBrushCount;

        public void AddBrush(TransitionBrush newBrush)
        {
            Array.Resize(ref TransitionBrushes, TransitionBrushCount + 1);
            TransitionBrushes[TransitionBrushCount] = newBrush;
            TransitionBrushCount++;
        }

        public void RemoveTransitionBrush(int num)
        {
            TransitionBrushCount--;
            if ( num != TransitionBrushCount )
            {
                TransitionBrushes[num] = TransitionBrushes[TransitionBrushCount];
            }
            Array.Resize(ref TransitionBrushes, TransitionBrushCount);
        }

        public void AddBrush(CliffBrush newBrush)
        {
            Array.Resize(ref CliffBrushes, CliffBrushCount + 1);
            CliffBrushes[CliffBrushCount] = newBrush;
            CliffBrushCount++;
        }

        public void RemoveCliffBrush(int num)
        {
            CliffBrushCount--;
            if ( num != CliffBrushCount )
            {
                CliffBrushes[num] = CliffBrushes[CliffBrushCount];
            }
            Array.Resize(ref CliffBrushes, CliffBrushCount);
        }

        public void AddTerrain(Terrain newTerrain)
        {
            newTerrain.Num = TerrainCount;
            Array.Resize(ref Terrains, TerrainCount + 1);
            Terrains[TerrainCount] = newTerrain;
            TerrainCount++;
        }

        public void RemoveTerrain(int num)
        {
            Terrains[num].Num = -1;
            TerrainCount--;
            if ( num != TerrainCount )
            {
                Terrains[num] = Terrains[TerrainCount];
                Terrains[num].Num = num;
            }
            Array.Resize(ref Terrains, TerrainCount);
        }

        public void AddBrush(RoadBrush newRoadBrush)
        {
            Array.Resize(ref RoadBrushes, RoadBrushCount + 1);
            RoadBrushes[RoadBrushCount] = newRoadBrush;
            RoadBrushCount++;
        }

        public void RemoveRoadBrush(int num)
        {
            RoadBrushCount--;
            if ( num != RoadBrushCount )
            {
                RoadBrushes[num] = RoadBrushes[RoadBrushCount];
            }
            Array.Resize(ref RoadBrushes, RoadBrushCount);
        }

        public void AddRoad(Road newRoad)
        {
            newRoad.Num = RoadCount;
            Array.Resize(ref Roads, RoadCount + 1);
            Roads[RoadCount] = newRoad;
            RoadCount++;
        }

        public void RemoveRoad(int num)
        {
            Roads[num].Num = -1;
            RoadCount--;
            if ( num != RoadCount )
            {
                Roads[num] = Roads[RoadCount];
                Roads[num].Num = num;
            }
            Array.Resize(ref Roads, RoadCount);
        }
    }
}