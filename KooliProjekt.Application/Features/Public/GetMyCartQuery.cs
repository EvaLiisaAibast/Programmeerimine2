using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Returns the shopping cart of the signed in user with current prices and stock.
    /// </summary>
    public class GetMyCartQuery : IRequest<OperationResult<MyCartDto>>
    {
    }
}
