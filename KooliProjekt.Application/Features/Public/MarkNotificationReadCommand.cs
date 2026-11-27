using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Marks one of the notifications of the signed in user as read.
    /// </summary>
    public class MarkNotificationReadCommand : IRequest<OperationResult<int>>
    {
        public int Id { get; set; }
    }
}
