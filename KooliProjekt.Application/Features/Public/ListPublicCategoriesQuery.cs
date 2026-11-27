using System.Collections.Generic;
using KooliProjekt.Application.Features.Public.Dtos;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Public category tree with the number of active products in each category.
    /// </summary>
    public class ListPublicCategoriesQuery : IRequest<OperationResult<IList<PublicCategoryDto>>>
    {
    }
}
