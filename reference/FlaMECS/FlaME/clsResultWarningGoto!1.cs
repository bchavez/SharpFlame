namespace FlaME
{
    using System;

    public class clsResultWarningGoto<GotoType> : clsResult.clsWarning where GotoType: clsResultItemGotoInterface
    {
        public GotoType MapGoto;

        public override void DoubleClicked()
        {
            base.DoubleClicked();
            this.MapGoto.Perform();
        }
    }
}

