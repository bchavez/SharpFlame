#region

using System;
using System.Collections.Generic;
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

        public List<Tile> Tiles { get; private set; }

        /// <summary>
        /// Gets or sets the tile count, given by the TTP.
        /// WARNING: This is not the count of Tiles in Tiles, use Tiles.Count for that!
        /// </summary>
        /// <value>The TTP tile count.</value>
        public int TileCount { get; set; }

        public string Name { get; set; }  

        /// <summary>
        /// Gets or sets the directory where this tileset is stored.
        /// </summary>
        /// <value>The directory.</value>
        public string Directory { get; set; } 

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

        public Tileset() {
            Tiles = new List<Tile>();
        }
    }
}