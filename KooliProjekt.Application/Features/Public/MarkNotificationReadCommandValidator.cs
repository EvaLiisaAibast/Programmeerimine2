using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class MarkNotificationReadCommandValidator : AbstractValidator<MarkNotificationReadCommand>
    {
        public MarkNotificationReadCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id peab olema suurem kui 0.");
        }
    }
}
