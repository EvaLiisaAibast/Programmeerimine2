using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.OrderLineId)
                .GreaterThan(0).WithMessage("Tellimuse rida on kohustuslik.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Hinne peab olema vahemikus 1 kuni 5.");

            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Arvustuse pealkiri on kohustuslik.")
                .MaximumLength(200).WithMessage("Pealkiri võib olla kuni 200 tähemärki.");

            RuleFor(x => x.Content)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Arvustuse sisu on kohustuslik.")
                .MaximumLength(4000).WithMessage("Arvustus võib olla kuni 4000 tähemärki.");
        }
    }
}
