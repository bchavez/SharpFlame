using System.ComponentModel;
using FluentValidation;
using FluentValidation.Attributes;
using SharpFlame.Mapping;

namespace SharpFlame.Gui.Dialogs
{
    public class CompileOptions
    {
        public CompileOptions()
        {
            this.NumPlayers = 2; //default
            this.AutoScrollLimits = true;
        }

        public string MapName { get; set; }
        public int NumPlayers { get; set; }
        public string Author { get; set; }
        public string License { get; set; }

        public int ScrollMinX { get; set; }
        public int ScrollMaxX { get; set; }
        public int ScrollMinY { get; set; }
        public int ScrollMaxY { get; set; }
        public CampaignType CampType { get; set; }
        public bool AutoScrollLimits { get; set; }
        public CompileType CompileType { get; set; }
        public string FilePath { get; set; }
    }

    public enum CampaignType
    {
        [Description("Initial scenario state")]
        Initial = 0,
        [Description("Scenario scroll area expansion")]
        Scroll,
        [Description("Stand alone mission")]
        StandAlone
    }

    public class CompileOptionsValidator : AbstractValidator<CompileOptions>
    {
        public CompileOptionsValidator()
        {
            RuleFor(x => x.MapName)
                .NotEmpty();
            RuleFor(x => x.NumPlayers)
                .GreaterThanOrEqualTo(2);
            RuleFor(x => x.ScrollMaxX)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.ScrollMaxY)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.ScrollMinX)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.ScrollMinY)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.License)
                .NotEmpty();
        }
    }
}