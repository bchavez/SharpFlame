

using System;
using SharpFlame.FileIO;
using SharpFlame.Core.Domain;


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
