using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(x => x.VariantId)
                .GreaterThan(0).WithMessage("Toote variant on kohustuslik.");

            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 99).WithMessage("Kogus peab olema vahemikus 1 kuni 99.");
        }
    }
}
