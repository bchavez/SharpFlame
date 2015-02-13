using System.ComponentModel;

namespace SharpFlame.Domain
{
    public enum CampaignType
    {
        [Description("Initial scenario state")]
        ScenarioStart = 0,
        [Description("Scenario scroll area expansion")]
        ScenarioExpand,
        [Description("Stand alone mission")]
        Mission
    }
}