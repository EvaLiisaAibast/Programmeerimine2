using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class ChangeMyPasswordCommandValidator : AbstractValidator<ChangeMyPasswordCommand>
    {
        public ChangeMyPasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Praegune parool on kohustuslik.");

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Uus parool on kohustuslik.")
                .MinimumLength(6).WithMessage("Uus parool peab olema vähemalt 6 tähemärki.")
                .MaximumLength(100).WithMessage("Uus parool võib olla kuni 100 tähemärki.");
        }
    }
}
