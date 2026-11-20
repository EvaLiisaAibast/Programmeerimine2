using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Users
{
    public class DeleteUserCommand : IRequest<OperationResult<bool>>
    {
        public int Id { get; set; }
    }
}
