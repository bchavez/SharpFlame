#region

using Eto.Forms;
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

    public class Tileset : IListItem
    {      
        public SRgb BGColour = new SRgb(0.5F, 0.5F, 0.5F);

        public bool IsOriginal { get; set; }       

        public Tile[] Tiles { get; set; }

        public int TileCount { get; set; }

        public string Name { get; set; }   

        /// <summary>
        /// Implements IListItem, this is for the eto GUI.
        /// </summary>
        /// <value>The text.</value>
        public string Text { 
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        /// Implements IListItem, this is for the eto GUI.
        /// </summary>
        /// <value>The text.</value>
        public string Key { 
            get { return Name; }
            set { Name = value; }
        }
    }
}