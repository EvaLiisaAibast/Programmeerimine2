using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Authentication;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Users
{
    public class SaveUserCommandHandler : IRequestHandler<SaveUserCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;

        public SaveUserCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<int>> Handle(SaveUserCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            if (request.Data == null)
            {
                return response.AddError("Andmed puuduvad.");
            }

            if (request.Data.Id < 0)
            {
                return response.AddError("Id ei tohi olla negatiivne.");
            }

            var userNameTaken = await _dbContext.Users
                .AnyAsync(x => x.UserName == request.Data.UserName && x.Id != request.Data.Id, cancellationToken);

            if (userNameTaken)
            {
                return response.AddError("See kasutajanimi on juba kasutusel.");
            }

            User entity;

            if (request.Data.Id > 0)
            {
                entity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Data.Id, cancellationToken);

                if (entity == null)
                {
                    return response.AddError("Kirjet ei leitud.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.Data.Password))
                {
                    return response.AddError("Uue kasutaja jaoks on parool kohustuslik.");
                }

                entity = new User();
                _dbContext.Users.Add(entity);
            }

            entity.UserName = request.Data.UserName;
            entity.IsAdmin = request.Data.IsAdmin;

            if (!string.IsNullOrWhiteSpace(request.Data.Password))
            {
                entity.PasswordHash = PasswordHelper.Hash(request.Data.Password);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = entity.Id;

            return response;
        }
    }
}
