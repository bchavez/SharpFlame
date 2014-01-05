using System;
using SharpFlame.Maths;

namespace SharpFlame.Mapping.Wz
{
    public struct sWrite_WZ_Args
    {
        public string Path;
        public bool Overwrite;
        public string MapName;

        public class clsMultiplayer
        {
            public int PlayerCount;
            public string AuthorName;
            public string License;
            public bool IsBetaPlayerFormat;
        }

        public clsMultiplayer Multiplayer;

        public class clsCampaign
        {
            //Public GAMTime As UInteger
            public UInt32 GAMType;
        }

        public clsCampaign Campaign;

        public enum enumCompileType
        {
            Unspecified,
            Multiplayer,
            Campaign
        }

        public sXY_int ScrollMin;
        public sXY_uint ScrollMax;
        public enumCompileType CompileType;
    }
}