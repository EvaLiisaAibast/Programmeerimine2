using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Returns the profile of the user who is currently signed in.
    /// </summary>
    public class GetMyProfileQuery : IRequest<OperationResult<MyProfileDto>>
    {
    }
}
