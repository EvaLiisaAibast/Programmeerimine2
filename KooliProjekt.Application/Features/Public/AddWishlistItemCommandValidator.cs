using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class AddWishlistItemCommandValidator : AbstractValidator<AddWishlistItemCommand>
    {
        public AddWishlistItemCommandValidator()
        {
            RuleFor(x => x.VariantId)
                .GreaterThan(0).WithMessage("Toote variant on kohustuslik.");
        }
    }
}
