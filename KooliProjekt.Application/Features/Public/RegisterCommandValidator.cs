using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("E-posti aadress on kohustuslik.")
                .EmailAddress().WithMessage("E-posti aadress ei ole korrektne.")
                .MaximumLength(100).WithMessage("E-posti aadress võib olla kuni 100 tähemärki.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Parool on kohustuslik.")
                .MinimumLength(6).WithMessage("Parool peab olema vähemalt 6 tähemärki.")
                .MaximumLength(100).WithMessage("Parool võib olla kuni 100 tähemärki.");

            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Eesnimi on kohustuslik.")
                .MaximumLength(100).WithMessage("Eesnimi võib olla kuni 100 tähemärki.");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Perekonnanimi on kohustuslik.")
                .MaximumLength(100).WithMessage("Perekonnanimi võib olla kuni 100 tähemärki.");

            RuleFor(x => x.Phone)
                .MaximumLength(50).WithMessage("Telefoninumber võib olla kuni 50 tähemärki.");
        }
    }
}
