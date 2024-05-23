using System.Security.Claims;

namespace ServiceEsgDataHub.Services
{
    public class UserResolverService
    {
        public readonly IHttpContextAccessor context;

        public UserResolverService(IHttpContextAccessor context)
        {
            this.context = context;
        }

        public string? GetNameIdentifier()
        {
            if (context.HttpContext == null || context.HttpContext.User == null)
                return null;

            return context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}