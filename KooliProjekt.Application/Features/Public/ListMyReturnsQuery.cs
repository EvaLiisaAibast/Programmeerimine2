using System.Collections.Generic;
using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Return requests of the signed in user.
    /// </summary>
    public class ListMyReturnsQuery : IRequest<OperationResult<IList<MyReturnDto>>>
    {
    }
}
