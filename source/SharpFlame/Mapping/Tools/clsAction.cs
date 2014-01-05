using SharpFlame.Maths;

namespace SharpFlame.Mapping.Tools
{
    public abstract class clsAction
    {
        public clsMap Map;
        public sXY_int PosNum;
        public bool UseEffect;
        public double Effect;

        public abstract void ActionPerform();
    }
}