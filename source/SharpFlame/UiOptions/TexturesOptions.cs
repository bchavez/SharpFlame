 #region License
 // /*
 // The MIT License (MIT)
 //
 // Copyright (c) 2013-2014 The SharpFlame Authors.
 //
 // Permission is hereby granted, free of charge, to any person obtaining a copy
 // of this software and associated documentation files (the "Software"), to deal
 // in the Software without restriction, including without limitation the rights
 // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 // copies of the Software, and to permit persons to whom the Software is
 // furnished to do so, subject to the following conditions:
 //
 // The above copyright notice and this permission notice shall be included in
 // all copies or substantial portions of the Software.
 //
 // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 // THE SOFTWARE.
 // */
 #endregion

using SharpFlame.Mapping.Tiles;

namespace SharpFlame.UiOptions
{
    /// <summary>
    /// DO NOT Change: Enumeration of the terrain modes.
    /// If you change also change the UI (Sections/TextureTab.cs -> 
    /// </summary>
    public enum TerrainMode {
        IgnoreTerrain,
        Reinterpret,
        RemoveTerrain
    }

    public enum TerrainMouseMode {
        Circular,
        Square
    }

    public class TexturesOptions
    {
        public bool SetTexture { get; set; }
        public bool SetOrientation { get; set; }
        public bool Randomize { get; set; }

        public TerrainMode TerrainMode { get; set; }
        public TerrainMouseMode TerrainMouseMode { get; set; }

        public double Radius { get; set; }

        /// <summary>
        /// Index of the selected Tileset
        /// </summary>
        public int TilesetNum { get; set; }

        public int SelectedTile { get; set; }

        public TileOrientation TextureOrientation;

        public TexturesOptions()
        {
            Radius = 2;
            SetTexture = true;
            SetOrientation = true;
            Randomize = false;
            TilesetNum = -1;
            SelectedTile = -1;
            TextureOrientation = new TileOrientation (false, false, false);
        }
    }
}

