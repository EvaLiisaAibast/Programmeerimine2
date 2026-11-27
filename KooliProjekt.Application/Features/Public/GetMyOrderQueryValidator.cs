using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class GetMyOrderQueryValidator : AbstractValidator<GetMyOrderQuery>
    {
        public GetMyOrderQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id peab olema suurem kui 0.");
        }
    }
}
