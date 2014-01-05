namespace FlaME
{
    using System;

    public class clsRGBA_sng : clsRGB_sng
    {
        public float Alpha;

        public clsRGBA_sng(clsRGBA_sng CopyItem) : base(CopyItem.Red, CopyItem.Green, CopyItem.Blue)
        {
            this.Alpha = CopyItem.Alpha;
        }

        public clsRGBA_sng(float Red, float Green, float Blue, float Alpha) : base(Red, Green, Blue)
        {
            this.Alpha = Alpha;
        }

        public override string GetINIOutput()
        {
            return (base.GetINIOutput() + ", " + modIO.InvariantToString_sng(this.Alpha));
        }

        public override bool ReadINIText(clsSplitCommaText SplitText)
        {
            if (!base.ReadINIText(SplitText))
            {
                return false;
            }
            if (SplitText.PartCount < 4)
            {
                return false;
            }
            if (!modIO.InvariantParse_sng(SplitText.Parts[3], ref this.Alpha))
            {
                this.Alpha = 1f;
            }
            return true;
        }
    }
}

