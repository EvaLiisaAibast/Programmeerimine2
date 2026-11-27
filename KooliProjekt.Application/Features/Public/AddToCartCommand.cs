using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Adds a product variant to the cart of the signed in user.
    /// </summary>
    public class AddToCartCommand : IRequest<OperationResult<int>>
    {
        public int VariantId { get; set; }

        public int Quantity { get; set; } = 1;
    }
}
