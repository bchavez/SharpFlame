using System.IO;
using SharpFlame.FileIO;
using SharpFlame.Core.Domain;
using SharpFlame.Maths;


namespace SharpFlame.Mapping
{
    public class InterfaceOptions
    {
        public bool AutoScrollLimits;
        public int CampaignGameType;
        public string CompileMultiAuthor;
        public string CompileMultiLicense;
        public string CompileMultiPlayers;
        public string CompileName;
        public sXY_uint ScrollMax;
        public XYInt ScrollMin;

        /// <summary>
        /// Contains the full path to the file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Helper for Path.GetFileName(FilePath);
        /// </summary>
        public string FileName
        {
            get { return Path.GetFileName(FilePath); }
        }

        public CompileType CompileType;

        public InterfaceOptions()
        {
            //set to default
            CompileName = "";
            CompileMultiPlayers = 2.ToStringInvariant();
            CompileMultiAuthor = "";
            CompileMultiLicense = "";
            AutoScrollLimits = true;
            ScrollMin = new XYInt(0, 0);
            ScrollMax.X = 0U;
            ScrollMax.Y = 0U;
            CampaignGameType = -1;
            CompileType = CompileType.Unspecified;
            FilePath = "";
        }
    }

    public enum CompileType
    {
        Unspecified,
        Multiplayer,
        Campaign
    }
}