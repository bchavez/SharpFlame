using FluentValidation;

namespace SharpFlame.Core.Parsers.Validators
{
    public class LevelValidator : AbstractValidator<Level>
    {
        public LevelValidator()
        {
            RuleFor(l => l.PointsCount)
                .Equal(l => l.Points.Length);

            RuleFor(l => l.PolygonsCount)
                .Equal(l => l.Polygons.Length);

            RuleFor(l => l.ConnectorCount)
                .Equal(l => l.Connectors.Length)
                .When(l => l.ConnectorCount.HasValue);
        }
    }
}