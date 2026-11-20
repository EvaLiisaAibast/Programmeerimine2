using System.Linq;
using System.Security.Claims;
using KooliProjekt.Application.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace KooliProjekt.WebAPI
{
    public class WebUserProvider : IUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return null;
            }

            if (!int.TryParse(claim.Value, out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}
