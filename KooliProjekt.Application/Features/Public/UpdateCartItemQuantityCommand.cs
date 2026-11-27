using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Changes the quantity of a cart line that belongs to the signed in user.
    /// </summary>
    public class UpdateCartItemQuantityCommand : IRequest<OperationResult<int>>
    {
        public int CartItemId { get; set; }

        public int Quantity { get; set; }
    }
}
