using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Public
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, OperationResult<int>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CurrentCustomer _currentCustomer;

        public CreateReviewCommandHandler(ApplicationDbContext dbContext, CurrentCustomer currentCustomer)
        {
            _dbContext = dbContext;
            _currentCustomer = currentCustomer;
        }

        public async Task<OperationResult<int>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var response = new OperationResult<int>();

            var customer = await _currentCustomer.GetAsync(cancellationToken);

            if (customer == null)
            {
                return response.AddError(PublicMessages.CustomerNotFound);
            }

            var line = await (from orderLine in _dbContext.OrderLines
                              join order in _dbContext.Orders on orderLine.OrderId equals order.Id
                              where orderLine.Id == request.OrderLineId && order.CustomerId == customer.Id
                              select new
                              {
                                  orderLine.Id,
                                  orderLine.ProductId,
                                  order.Status
                              })
                              .FirstOrDefaultAsync(cancellationToken);

            if (line == null)
            {
                return response.AddError("Tellimuse rida ei leitud.");
            }

            if (line.Status != "delivered")
            {
                return response.AddError("Arvustada saab ainult kättesaadud tellimust.");
            }

            var alreadyReviewed = await _dbContext.Reviews
                .AnyAsync(x => x.OrderLineId == line.Id, cancellationToken);

            if (alreadyReviewed)
            {
                return response.AddError("Seda ostu saab arvustada ainult üks kord.");
            }

            var review = new Review
            {
                CustomerId = customer.Id,
                ProductId = line.ProductId,
                OrderLineId = line.Id,
                Rating = request.Rating,
                Title = request.Title.Trim(),
                Content = request.Content.Trim(),
                CreatedAt = DateTime.UtcNow,
                Status = "Ootel"
            };

            _dbContext.Reviews.Add(review);
            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Value = review.Id;

            return response;
        }
    }
}
