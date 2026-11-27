using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Adds a product variant to the wishlist of the signed in user.
    /// </summary>
    public class AddWishlistItemCommand : IRequest<OperationResult<int>>
    {
        public int VariantId { get; set; }
    }
}
