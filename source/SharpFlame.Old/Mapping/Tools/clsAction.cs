#region

using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Old.Mapping.Tools
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