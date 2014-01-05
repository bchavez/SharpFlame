namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;

    [StandardModule]
    public sealed class modColour
    {
        public static int OSRGB(int Red, int Green, int Blue)
        {
            return Information.RGB(Red, Green, Blue);
        }
    }
}

