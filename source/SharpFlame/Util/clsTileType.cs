using System;
using SharpFlame.Colors;
using SharpFlame.Core.Domain.Colors;
using Eto.Forms;

namespace SharpFlame.Util
{
    public class clsTileType : IListItem, IEquatable<clsTileType>
    {
        public SRgb DisplayColour;
        public string Name;

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

        public bool Equals(clsTileType other) {
            return Name == other.Name;
        }
    }
}