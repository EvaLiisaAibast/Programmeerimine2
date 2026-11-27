using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Detail of one order. Only the order of the signed in user is returned.
    /// </summary>
    public class GetMyOrderQuery : IRequest<OperationResult<MyOrderDto>>
    {
        public int Id { get; set; }
    }
}
