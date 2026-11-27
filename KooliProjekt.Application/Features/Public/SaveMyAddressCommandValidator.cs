using FluentValidation;

namespace KooliProjekt.Application.Features.Public
{
    public class SaveMyAddressCommandValidator : AbstractValidator<SaveMyAddressCommand>
    {
        public SaveMyAddressCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0).WithMessage("Id ei tohi olla negatiivne.");

            RuleFor(x => x.Country)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Riik on kohustuslik.")
                .MaximumLength(100).WithMessage("Riik võib olla kuni 100 tähemärki.");

            RuleFor(x => x.City)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Linn on kohustuslik.")
                .MaximumLength(100).WithMessage("Linn võib olla kuni 100 tähemärki.");

            RuleFor(x => x.PostalCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Postiindeks on kohustuslik.")
                .MaximumLength(20).WithMessage("Postiindeks võib olla kuni 20 tähemärki.");

            RuleFor(x => x.Street)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Tänav on kohustuslik.")
                .MaximumLength(150).WithMessage("Tänav võib olla kuni 150 tähemärki.");

            RuleFor(x => x.HouseNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Maja number on kohustuslik.")
                .MaximumLength(20).WithMessage("Maja number võib olla kuni 20 tähemärki.");

            RuleFor(x => x.Apartment)
                .MaximumLength(20).WithMessage("Korter võib olla kuni 20 tähemärki.");
        }
    }
}
