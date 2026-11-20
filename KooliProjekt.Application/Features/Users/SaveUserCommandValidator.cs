using FluentValidation;

namespace KooliProjekt.Application.Features.Users
{
    public class SaveUserCommandValidator : AbstractValidator<SaveUserCommand>
    {
        public SaveUserCommandValidator()
        {
            RuleFor(x => x.Data)
                .NotNull().WithMessage("Andmed puuduvad.");

            RuleFor(x => x.Data.Id)
                .GreaterThanOrEqualTo(0).WithMessage("Id ei tohi olla negatiivne.")
                .When(x => x.Data != null);

            RuleFor(x => x.Data.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Kasutajanimi on kohustuslik.")
                .MaximumLength(25).WithMessage("Kasutajanimi võib olla kuni 25 tähemärki.")
                .When(x => x.Data != null);

            RuleFor(x => x.Data.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Uue kasutaja jaoks on parool kohustuslik.")
                .MinimumLength(6).WithMessage("Parool peab olema vähemalt 6 tähemärki.")
                .MaximumLength(100).WithMessage("Parool võib olla kuni 100 tähemärki.")
                .When(x => x.Data != null && x.Data.Id == 0);

            RuleFor(x => x.Data.Password)
                .MinimumLength(6).WithMessage("Parool peab olema vähemalt 6 tähemärki.")
                .MaximumLength(100).WithMessage("Parool võib olla kuni 100 tähemärki.")
                .When(x => x.Data != null && x.Data.Id > 0 && !string.IsNullOrWhiteSpace(x.Data.Password));
        }
    }
}
