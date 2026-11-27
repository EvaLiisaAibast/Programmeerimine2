using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderCommandValidator()
        {
            RuleFor(x => x.AddressId)
                .GreaterThanOrEqualTo(0).WithMessage("Aadressi id ei tohi olla negatiivne.");
        }
    }
}
