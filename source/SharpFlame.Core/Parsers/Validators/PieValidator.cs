using FluentValidation;

namespace SharpFlame.Core.Parsers.Validators
{
    public class PieValidator : AbstractValidator<Pie.Pie>
    {
        public PieValidator()
        {
            RuleFor(p => p.LevelCount)
                .Equal(p => p.Levels.Length);

            RuleFor(p => p.Levels)
                .SetCollectionValidator(new PieLevelValidator());
        }
    }
}