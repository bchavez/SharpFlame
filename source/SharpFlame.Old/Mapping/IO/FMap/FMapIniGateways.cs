#region

using System;
using SharpFlame.Old.FileIO;
using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Old.Mapping.IO.FMap
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
