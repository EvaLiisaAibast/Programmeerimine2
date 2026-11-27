using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class ListMyOrdersQueryValidator : AbstractValidator<ListMyOrdersQuery>
    {
        public ListMyOrdersQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Lehekülg peab olema suurem kui 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Lehe suurus peab olema vahemikus 1 kuni 100.");
        }
    }
}
