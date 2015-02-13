using SharpFlame.Core.Domain;
using SharpFlame.Gui.Dialogs;
using SharpFlame.Mapping;
using SharpFlame.Maths;

namespace SharpFlame.Domain
{
    public class InterfaceOptions
    {
        public InterfaceOptions()
        {
            this.CompileMultiPlayers = 2; //default
            this.AutoScrollLimits = true;
        }

        public string CompileName { get; set; }
        public int CompileMultiPlayers { get; set; }
        public string CompileMultiAuthor { get; set; }
        public string CompileMultiLicense { get; set; }

        public sXY_uint ScrollMax;
        public XYInt ScrollMin;

        public CampaignType CampaignGameType { get; set; }
        public bool AutoScrollLimits { get; set; }
        public CompileType CompileType { get; set; }
        
        public string FilePath { get; set; }

        public string FileName
        {
            get { return System.IO.Path.GetFileName(this.FilePath); }
        }
    }
}