using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Lets the signed in user review a product that he has already received.
    /// </summary>
    public class CreateReviewCommand : IRequest<OperationResult<int>>
    {
        public int OrderLineId { get; set; }

        public int Rating { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
    }
}
