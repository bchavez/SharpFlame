#region

using System;
using SharpFlame.Core.Domain;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Mapping.IO.Wz
{
    public struct sWrite_WZ_Args
    {
        public enum enumCompileType
        {
            Unspecified,
            Multiplayer,
            Campaign
        }

        public clsCampaign Campaign;
        public enumCompileType CompileType;

        public string MapName;

        public clsMultiplayer Multiplayer;
        public bool Overwrite;
        public string Path;

        public sXY_uint ScrollMax;
        public XYInt ScrollMin;

        public class clsCampaign
        {
            //Public GAMTime As UInteger
            public UInt32 GAMType;
        }

        public class clsMultiplayer
        {
            public string AuthorName;
            public string License;
            public int PlayerCount;
        }
    }
}