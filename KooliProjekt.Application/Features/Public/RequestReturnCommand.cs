using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Requests a return for one line of an order of the signed in user.
    /// </summary>
    public class RequestReturnCommand : IRequest<OperationResult<int>>
    {
        public int OrderLineId { get; set; }

        public int Quantity { get; set; }

        public string Reason { get; set; }
    }
}
