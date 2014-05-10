
using SharpFlame.Core.Domain;


namespace SharpFlame.Mapping.Tools
{
    public abstract class clsAction
    {
        public double Effect;
        public Map Map;
        public XYInt PosNum;
        public bool UseEffect;

        protected clsAction()
        {
            PosNum = new XYInt(0, 0);
        }

        public abstract void ActionPerform();
    }
}