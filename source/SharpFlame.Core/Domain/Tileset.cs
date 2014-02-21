#region

using SharpFlame.Core.Domain.Colors;

#endregion

namespace SharpFlame.Core.Domain
{
    public struct Tile
    {
        public SRgb AverageColour;
        public byte DefaultType;
        public int GlTextureNum;
    }

    public class Tileset
    {      
        public SRgb BGColour = new SRgb(0.5F, 0.5F, 0.5F);

        public bool IsOriginal { get; set; }       

        public Tile[] Tiles { get; set; }

        public int TileCount { get; set; }

        public string Name { get; set; }         
    }
}