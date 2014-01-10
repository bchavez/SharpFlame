using SharpFlame.FileIO;
using SharpFlame.Maths;

namespace SharpFlame.Mapping
{
    public class clsInterfaceOptions
    {
        public string CompileName;
        public string CompileMultiPlayers;
        public bool CompileMultiXPlayers;
        public string CompileMultiAuthor;
        public string CompileMultiLicense;
        public bool AutoScrollLimits;
        public sXY_int ScrollMin;
        public sXY_uint ScrollMax;
        public int CampaignGameType;

        public clsInterfaceOptions()
        {
            //set to default
            CompileName = "";
            CompileMultiPlayers = 2.ToStringInvariant();
            CompileMultiXPlayers = false;
            CompileMultiAuthor = "";
            CompileMultiLicense = "";
            AutoScrollLimits = true;
            ScrollMin.X = 0;
            ScrollMin.Y = 0;
            ScrollMax.X = 0U;
            ScrollMax.Y = 0U;
            CampaignGameType = -1;
        }
    }
}