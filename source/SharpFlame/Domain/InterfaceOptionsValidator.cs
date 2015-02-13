using FluentValidation;

namespace SharpFlame.Domain
{
    public class InterfaceOptionsValidator : AbstractValidator<InterfaceOptions>
    {
        public InterfaceOptionsValidator()
        {
            RuleFor(x => x.CompileName)
                .NotEmpty();
            RuleFor(x => x.CompileMultiPlayers)
                .GreaterThanOrEqualTo(2)
                .When( x=> x.CompileType == CompileType.Multiplayer);
            RuleFor(x => x.CompileMultiLicense)
                .NotEmpty()
                .When(x => x.CompileType == CompileType.Multiplayer); ;
        }
    }
}