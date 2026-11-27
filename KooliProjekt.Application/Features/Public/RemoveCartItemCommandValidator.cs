using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
    {
        public RemoveCartItemCommandValidator()
        {
            RuleFor(x => x.CartItemId)
                .GreaterThan(0).WithMessage("Ostukorvi rida on kohustuslik.");
        }
    }
}
