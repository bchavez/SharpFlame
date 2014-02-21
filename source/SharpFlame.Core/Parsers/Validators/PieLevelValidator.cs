using FluentValidation;
using SharpFlame.Core.Parsers.Pie;

namespace SharpFlame.Core.Parsers.Validators
{
    public class PieLevelValidator : AbstractValidator<Level>
    {
        public PieLevelValidator()
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