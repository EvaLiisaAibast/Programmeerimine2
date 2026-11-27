using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class RemoveWishlistItemCommandValidator : AbstractValidator<RemoveWishlistItemCommand>
    {
        public RemoveWishlistItemCommandValidator()
        {
            RuleFor(x => x.WishlistItemId)
                .GreaterThan(0).WithMessage("Soovinimekirja rida on kohustuslik.");
        }
    }
}
