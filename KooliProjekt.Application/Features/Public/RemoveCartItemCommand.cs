using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Removes one line from the cart of the signed in user.
    /// </summary>
    public class RemoveCartItemCommand : IRequest<OperationResult<int>>
    {
        public int CartItemId { get; set; }
    }
}
