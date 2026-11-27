using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Changes the password of the signed in user.
    /// </summary>
    public class ChangeMyPasswordCommand : IRequest<OperationResult<int>>
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
