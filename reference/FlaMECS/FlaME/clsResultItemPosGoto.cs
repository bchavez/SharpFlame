namespace FlaME
{
    using System;

    public class clsResultItemPosGoto : clsResultItemGotoInterface
    {
        public modMath.sXY_int Horizontal;
        public clsViewInfo View;

        public override void Perform()
        {
            this.View.LookAtPos(this.Horizontal);
        }
    }
}

