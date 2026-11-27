using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class GetPublicProductQueryValidator : AbstractValidator<GetPublicProductQuery>
    {
        public GetPublicProductQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id peab olema suurem kui 0.");
        }
    }
}
