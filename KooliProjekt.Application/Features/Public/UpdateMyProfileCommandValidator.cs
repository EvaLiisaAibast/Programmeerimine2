using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
    {
        public UpdateMyProfileCommandValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("E-posti aadress on kohustuslik.")
                .EmailAddress().WithMessage("E-posti aadress ei ole korrektne.")
                .MaximumLength(100).WithMessage("E-posti aadress võib olla kuni 100 tähemärki.");

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
