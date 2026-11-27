using System.Threading.Tasks;
using KooliProjekt.Application.Features.Public;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KooliProjekt.WebAPI.Controllers
{
    /// <summary>
    /// Everything the end user of the shop can do. The catalog and registration are open
    /// to everyone, all other actions work with the data of the signed in user only.
    /// </summary>
    public class PublicController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public PublicController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registers a new end user.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Lists the products that are for sale.
        /// </summary>
        [HttpGet("products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductList(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = null,
            [FromQuery] int categoryId = 0)
        {
            var query = new ListPublicProductsQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                CategoryId = categoryId
            };

            var result = await _mediator.Send(query);

            return Result(result);
        }

        /// <summary>
        /// Shows one product with its variants, images and reviews.
        /// </summary>
        [HttpGet("products/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            var query = new GetPublicProductQuery { Id = id };
            var result = await _mediator.Send(query);

            return Result(result);
        }

        /// <summary>
        /// Lists the product categories.
        /// </summary>
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryList()
        {
            var result = await _mediator.Send(new ListPublicCategoriesQuery());

            return Result(result);
        }

        /// <summary>
        /// Shows the profile of the signed in user.
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _mediator.Send(new GetMyProfileQuery());

            return Result(result);
        }

        /// <summary>
        /// Changes the profile data of the signed in user.
        /// </summary>
        [HttpPost("profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command)
        {
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Changes the password of the signed in user.
        /// </summary>
        [HttpPost("profile/password")]
        public async Task<IActionResult> ChangeMyPassword([FromBody] ChangeMyPasswordCommand command)
        {
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Adds a new address or updates an existing address of the signed in user.
        /// </summary>
        [HttpPost("profile/addresses")]
        public async Task<IActionResult> SaveMyAddress([FromBody] SaveMyAddressCommand command)
        {
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Shows the shopping cart of the signed in user.
        /// </summary>
        [HttpGet("cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var result = await _mediator.Send(new GetMyCartQuery());

            return Result(result);
        }

        /// <summary>
        /// Adds a product to the shopping cart.
        /// </summary>
        [HttpPost("cart/items")]
        public async Task<IActionResult> AddProductToCart([FromBody] AddToCartCommand command)
        {
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Changes the quantity of a line in the shopping cart.
        /// </summary>
        [HttpPut("cart/items/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, [FromQuery] int quantity)
        {
            var command = new UpdateCartItemQuantityCommand
            {
                CartItemId = cartItemId,
                Quantity = quantity
            };

            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Removes a line from the shopping cart.
        /// </summary>
        [HttpDelete("cart/items/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var command = new RemoveCartItemCommand { CartItemId = cartItemId };
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Shows the wishlist of the signed in user.
        /// </summary>
        [HttpGet("wishlist")]
        public async Task<IActionResult> GetMyWishlist()
        {
            var result = await _mediator.Send(new GetMyWishlistQuery());

            return Result(result);
        }

        /// <summary>
        /// Adds a product to the wishlist.
        /// </summary>
        [HttpPost("wishlist/items")]
        public async Task<IActionResult> AddProductToWishlist([FromBody] AddWishlistItemCommand command)
        {
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Removes a line from the wishlist.
        /// </summary>
        [HttpDelete("wishlist/items/{wishlistItemId}")]
        public async Task<IActionResult> RemoveWishlistItem(int wishlistItemId)
        {
            var command = new RemoveWishlistItemCommand { WishlistItemId = wishlistItemId };
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Turns the shopping cart into an order.
        /// </summary>
        [HttpPost("orders")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderCommand command)
        {
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Shows the order history of the signed in user.
        /// </summary>
        [HttpGet("orders")]
        public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new ListMyOrdersQuery
            {
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Result(result);
        }

        /// <summary>
        /// Shows one order of the signed in user.
        /// </summary>
        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetMyOrder(int id)
        {
            var query = new GetMyOrderQuery { Id = id };
            var result = await _mediator.Send(query);

            return Result(result);
        }

        /// <summary>
        /// Shows the payments and invoices of the signed in user.
        /// </summary>
        [HttpGet("payments")]
        public async Task<IActionResult> GetMyPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new ListMyPaymentsQuery
            {
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Result(result);
        }

        /// <summary>
        /// Shows the notifications of the signed in user.
        /// </summary>
        [HttpGet("notifications")]
        public async Task<IActionResult> GetMyNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool unreadOnly = false)
        {
            var query = new ListMyNotificationsQuery
            {
                Page = page,
                PageSize = pageSize,
                UnreadOnly = unreadOnly
            };

            var result = await _mediator.Send(query);

            return Result(result);
        }

        /// <summary>
        /// Marks a notification of the signed in user as read.
        /// </summary>
        [HttpPost("notifications/{id}/read")]
        public async Task<IActionResult> MarkNotificationRead(int id)
        {
            var command = new MarkNotificationReadCommand { Id = id };
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Reviews a product that the signed in user has received.
        /// </summary>
        [HttpPost("reviews")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand command)
        {
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Requests a return for a line of an order of the signed in user.
        /// </summary>
        [HttpPost("returns")]
        public async Task<IActionResult> RequestReturn([FromBody] RequestReturnCommand command)
        {
            var result = await _mediator.Send(command);

            return Result(result);
        }

        /// <summary>
        /// Shows the return requests of the signed in user.
        /// </summary>
        [HttpGet("returns")]
        public async Task<IActionResult> GetMyReturns()
        {
            var result = await _mediator.Send(new ListMyReturnsQuery());

            return Result(result);
        }
    }
}
