using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class RequestReturnCommandValidator : AbstractValidator<RequestReturnCommand>
    {
        public RequestReturnCommandValidator()
        {
            RuleFor(x => x.OrderLineId)
                .GreaterThan(0).WithMessage("Tellimuse rida on kohustuslik.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Tagastatav kogus peab olema suurem kui 0.");

            RuleFor(x => x.Reason)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Tagastuse põhjus on kohustuslik.")
                .MaximumLength(500).WithMessage("Põhjus võib olla kuni 500 tähemärki.");
        }
    }
}
