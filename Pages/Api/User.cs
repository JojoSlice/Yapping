using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;

namespace miniReddit.Pages.Api
{
    [ApiController]
    [Route("api/user")]
    public class User : ControllerBase
    {
        private readonly Services.MongoDB _db;
        private readonly Services.AuthenticationService _authenticationService;
        public User(Services.MongoDB mongoDb, Services.AuthenticationService authenticationService) 
        { 
            _db = mongoDb;
            _authenticationService = authenticationService;
        }

        [HttpGet("user")]
        public IActionResult GetUser()
        {
            var userId = UserId();
            if (userId != string.Empty)
            {
                var user = _db.GetUserFromId(userId);
                if (user != null) return Ok(new { user = user });
            }
            return Unauthorized(new { message = "User not authenticated." });
        }

        public string UserId()
        {
            var userId = _authenticationService.GetLoggedInUserId();
            if (userId != null) return userId;
            return string.Empty;
        }
    }
}
