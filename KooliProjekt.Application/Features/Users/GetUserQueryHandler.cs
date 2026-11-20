using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Dto;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Users
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, OperationResult<UserDto>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetUserQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<UserDto>();

            response.Value = await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    IsAdmin = x.IsAdmin,
                })
                .FirstOrDefaultAsync(cancellationToken);

            return response;
        }
    }
}
