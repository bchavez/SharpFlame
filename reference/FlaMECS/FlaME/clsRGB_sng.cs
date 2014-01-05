namespace FlaME
{
    using System;

    public class clsRGB_sng
    {
        public float Blue;
        public float Green;
        public float Red;

        public clsRGB_sng(float Red, float Green, float Blue)
        {
            this.Red = Red;
            this.Green = Green;
            this.Blue = Blue;
        }

        public virtual string GetINIOutput()
        {
            return (modIO.InvariantToString_sng(this.Red) + ", " + modIO.InvariantToString_sng(this.Green) + ", " + modIO.InvariantToString_sng(this.Blue));
        }

        public virtual bool ReadINIText(clsSplitCommaText SplitText)
        {
            sRGB_sng _sng;
            if (SplitText.PartCount < 3)
            {
                return false;
            }
            if (!modIO.InvariantParse_sng(SplitText.Parts[0], ref _sng.Red))
            {
                return false;
            }
            if (!modIO.InvariantParse_sng(SplitText.Parts[1], ref _sng.Green))
            {
                return false;
            }
            if (!modIO.InvariantParse_sng(SplitText.Parts[2], ref _sng.Blue))
            {
                return false;
            }
            this.Red = _sng.Red;
            this.Green = _sng.Green;
            this.Blue = _sng.Blue;
            return true;
        }
    }
}

