using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Shortcut that turns the current cart into an order in one step.
    /// </summary>
    public class PlaceOrderCommand : IRequest<OperationResult<int>>
    {
        /// <summary>
        /// Address that is used for the invoice and the delivery. 0 means the default address.
        /// </summary>
        public int AddressId { get; set; }
    }
}
