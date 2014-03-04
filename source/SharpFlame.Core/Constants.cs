using System.Reflection;
using SharpFlame.Core.Extensions;

namespace SharpFlame.Core
{
    public static class Constants
    {
        public const string ProgramName = "SharpFlame";
#if Mono
        public const string ProgramPlatform = "Mono";
#elif Mac
        public const string ProgramPlatform = "MacOS";
#else
        public const string ProgramPlatform = "Windows";
#endif
        public const int PlayerCountMax = 10;
        public const int GameTypeCount = 3;
        public const int DefaultHeightMultiplier = 2;
        public const int MinimapDelay = 100;
        public const int SectorTileSize = 8;
        public const int MaxDroidWeapons = 3;
        public const int WzMapMaxSize = 250;
        public const int MapMaxSize = 512;
        public const int MinimapMaxSize = 512;

        public const int IniRotationMax = 65536;
        public const int TileTypeNumWater = 7;
        public const int TileTypeNumCliff = 8;
        public const int TerrainGridSpacing = 128;

        public static string ProgramVersion()
        {
            var ver = Assembly.GetExecutingAssembly()
                .GetName().Version;
            return "{0}.{1}".Format2(ver.Major, ver.Minor);
        }
    }
}