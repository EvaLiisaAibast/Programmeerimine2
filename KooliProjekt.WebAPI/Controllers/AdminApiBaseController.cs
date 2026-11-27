using Microsoft.AspNetCore.Authorization;

namespace KooliProjekt.WebAPI.Controllers
{
    /// <summary>
    /// Base class for the management endpoints. Everything that inherits from it
    /// can only be called by a user whose token contains the Admin role.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public abstract class AdminApiBaseController : ApiControllerBase
    {
    }
}
