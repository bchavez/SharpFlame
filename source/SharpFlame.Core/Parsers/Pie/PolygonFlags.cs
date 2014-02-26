using System;

namespace SharpFlame.Core.Parsers.Pie
{
    [Flags]
    public enum PolygonFlags : uint
    {
        Texture = 0x200,
        Animation = 0x4000
    }
}