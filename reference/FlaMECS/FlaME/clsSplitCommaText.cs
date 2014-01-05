namespace FlaME
{
    using System;

    public class clsSplitCommaText
    {
        public int PartCount;
        public string[] Parts;

        public clsSplitCommaText(string Text)
        {
            this.Parts = Text.Split(new char[] { ',' });
            this.PartCount = this.Parts.GetUpperBound(0) + 1;
            int num2 = this.PartCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.Parts[i] = this.Parts[i].Trim();
            }
        }
    }
}

