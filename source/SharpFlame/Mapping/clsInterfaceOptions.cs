#region

using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Mapping
{
    public class clsInterfaceOptions
    {
        public bool AutoScrollLimits;
        public int CampaignGameType;
        public string CompileMultiAuthor;
        public string CompileMultiLicense;
        public string CompileMultiPlayers;
        public string CompileName;
        public sXY_uint ScrollMax;
        public XYInt ScrollMin;

        public clsInterfaceOptions()
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
        }
    }
}