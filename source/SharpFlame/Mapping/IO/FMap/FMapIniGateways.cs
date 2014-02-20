#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;

#endregion

namespace SharpFlame.Mapping.IO.FMap
{
    public class INIGateway
    {
        public XYInt PosA;
        public XYInt PosB;

		public INIGateway()
		{
			PosA = new XYInt(0, 0);
			PosB = new XYInt(0, 0);
		}
    }
}
