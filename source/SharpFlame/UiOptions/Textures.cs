using System;
using SharpFlame.Mapping.Tiles;

namespace SharpFlame.UiOptions
{
    /// <summary>
    /// DO NOT Change: Enumeration of the terrain modes.
    /// If you change also change the UI (Sections/TextureTab.cs -> 
    /// </summary>
    public enum TerrainMode {
        Ignore,
        Reinterpret,
        Remove
    }

    public class Textures
    {
        public readonly clsBrush Brush;

        public bool SetTexture { get; set; }
        public bool SetOrientation { get; set; }
        public bool Randomize { get; set; }

        public TerrainMode TerrainMode { get; set; }

        public EventHandler TilesetNumChanged = delegate {};

        private int tilesetNum;
        /// <summary>
        /// Index of the selected Tileset
        /// </summary>
        public int TilesetNum { 
            get { return tilesetNum; }
            set { 
                if(tilesetNum != value)
                {
                    tilesetNum = value;
                    TilesetNumChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of the selected tile/texture.
        /// </summary>
        /// <value>The selected tile.</value>
        public int SelectedTile { get; set; }

        public TileOrientation TextureOrientation;

        public Textures()
        {
            Brush = new clsBrush(0.0D, ShapeType.Circle);
            SetTexture = true;
            SetOrientation = true;
            Randomize = false;
            TilesetNum = -1;
            SelectedTile = -1;
            TextureOrientation = new TileOrientation (false, false, false);
        }
    }
}

