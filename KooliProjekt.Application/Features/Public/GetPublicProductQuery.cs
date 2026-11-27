using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Public product detail with variants, images and approved reviews.
    /// </summary>
    public class GetPublicProductQuery : IRequest<OperationResult<PublicProductDto>>
    {
        public int Id { get; set; }
    }
}
