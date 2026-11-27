using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Removes one line from the wishlist of the signed in user.
    /// </summary>
    public class RemoveWishlistItemCommand : IRequest<OperationResult<int>>
    {
        public int WishlistItemId { get; set; }
    }
}
