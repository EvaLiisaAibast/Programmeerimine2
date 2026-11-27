using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
    {
        public UpdateCartItemQuantityCommandValidator()
        {
            RuleFor(x => x.CartItemId)
                .GreaterThan(0).WithMessage("Ostukorvi rida on kohustuslik.");

            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 99).WithMessage("Kogus peab olema vahemikus 1 kuni 99.");
        }
    }
}
