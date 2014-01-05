namespace FlaME
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct sHeightmaps
    {
        public clsHeightmap[] Heightmaps;
    }
}

