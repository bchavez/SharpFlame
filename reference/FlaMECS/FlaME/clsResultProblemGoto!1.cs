namespace FlaME
{
    using System;

    public class clsResultProblemGoto<GotoType> : clsResult.clsProblem where GotoType: clsResultItemGotoInterface
    {
        public GotoType MapGoto;

        public override void DoubleClicked()
        {
            base.DoubleClicked();
            this.MapGoto.Perform();
        }
    }
}

