using Org.BouncyCastle.Bcpg;
using System.Security.Claims;

namespace miniReddit.Services
{
    public class AuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsUserAuthenticated()
        {
            Console.WriteLine("Service");
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated ?? false;
        }
        public string GetLoggedInUserId()
        {
            Console.WriteLine("service/user");
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null)
            {
                var userid = user?.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (userid != null)
                {
                    return userid;
                }
                return string.Empty;
            }
            return string.Empty;
        }
    }
}