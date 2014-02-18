using SharpFlame.Core.Domain;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Tools
{
    public abstract class clsAction
    {
        public clsMap Map;
        public XYInt PosNum;
        public bool UseEffect;
        public double Effect;

		public clsAction() {
			PosNum = new XYInt (0, 0);
		}

        public abstract void ActionPerform();
    }
}