using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    /// <summary>
    /// Resolves the customer profile that belongs to the user who sent the request.
    /// Every handler in the Public feature starts from here, so a user can only ever
    /// work with the data that is linked to his own account.
    /// </summary>
    public class CurrentCustomer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserProvider _userProvider;

        public CurrentCustomer(ApplicationDbContext dbContext, IUserProvider userProvider)
        {
            _dbContext = dbContext;
            _userProvider = userProvider;
        }

        public int? UserId()
        {
            return _userProvider.UserId();
        }

        public async Task<Customer> GetAsync(CancellationToken cancellationToken)
        {
            var customerId = await CustomerIdAsync(cancellationToken);

            if (customerId == null)
            {
                return null;
            }

            return await _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == customerId.Value, cancellationToken);
        }

        public async Task<Customer> GetForUpdateAsync(CancellationToken cancellationToken)
        {
            var customerId = await CustomerIdAsync(cancellationToken);

            if (customerId == null)
            {
                return null;
            }

            return await _dbContext.Customers
                .FirstOrDefaultAsync(x => x.Id == customerId.Value, cancellationToken);
        }

        private async Task<int?> CustomerIdAsync(CancellationToken cancellationToken)
        {
            var userId = _userProvider.UserId();

            if (userId == null)
            {
                return null;
            }

            return await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.Id == userId.Value)
                .Select(x => x.CustomerId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
