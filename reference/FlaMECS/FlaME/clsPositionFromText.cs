namespace FlaME
{
    using System;

    public class clsPositionFromText
    {
        public modMath.sXY_int Pos;

        public bool Translate(string Text)
        {
            int num;
            clsSplitCommaText text = new clsSplitCommaText(Text);
            if ((text.PartCount >= 2) && modIO.InvariantParse_int(text.Parts[0], ref num))
            {
                this.Pos.X = num;
            }
            else
            {
                return false;
            }
            if (modIO.InvariantParse_int(text.Parts[1], ref num))
            {
                this.Pos.Y = num;
                return true;
            }
            return false;
        }
    }
}

