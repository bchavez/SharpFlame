#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Tiles;

#endregion

namespace SharpFlame.Mapping.IO.FMap
{
    public class FMapInfo
    {
        public InterfaceOptions InterfaceOptions = new InterfaceOptions();
        public XYInt TerrainSize = new XYInt(-1, -1);
        public Tileset Tileset;

        public FMapInfo() 
        {
            TerrainSize = new XYInt (-1, -1);
            Tileset = new Tileset ();
        }
    }
}