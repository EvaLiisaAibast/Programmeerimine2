using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Shows the wishlist of the signed in user.
    /// </summary>
    public class GetMyWishlistQuery : IRequest<OperationResult<MyWishlistDto>>
    {
    }
}
