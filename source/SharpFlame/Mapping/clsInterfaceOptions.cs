using SharpFlame.FileIO;
using SharpFlame.Core.Domain;
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
        public XYInt ScrollMin;
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
			ScrollMin = new XYInt (0, 0);
            ScrollMax.X = 0U;
            ScrollMax.Y = 0U;
            CampaignGameType = -1;
        }
    }
}