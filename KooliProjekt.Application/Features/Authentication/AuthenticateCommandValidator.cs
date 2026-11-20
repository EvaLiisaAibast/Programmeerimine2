using FluentValidation;

namespace KooliProjekt.Application.Features.Authentication
{
    public class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
    {
        public AuthenticateCommandValidator()
        {
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Kasutajanimi on kohustuslik.")
                .MaximumLength(25).WithMessage("Kasutajanimi võib olla kuni 25 tähemärki.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Parool on kohustuslik.")
                .MaximumLength(100).WithMessage("Parool võib olla kuni 100 tähemärki.");
        }
    }
}
