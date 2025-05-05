using Microsoft.AspNetCore.Mvc;

namespace miniReddit.Pages.Api
{
    [ApiController]
    [Route("api/userapi")]
    public class UserApi(APIManager.UserManager api) : ControllerBase
    {
        private readonly APIManager.UserManager _api = api;

        [HttpGet("user")]
        public async Task<IActionResult> GetUserAsync()
        {
            var user = await _api.GetLoggedInUserAsync();
            if (user != null)
            {
                return Ok(user);
            }
            return Unauthorized(new { message = "User not authenticated."});
        }
    }
}
