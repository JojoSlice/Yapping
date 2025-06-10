using Microsoft.AspNetCore.Mvc;

namespace miniReddit.Pages.Api
{
    [ApiController]
    [Route("api/userapi")]
    public class UserApi(APIManager.UserManager api, APIManager.MessageManager messManager) : ControllerBase
    {
        private readonly APIManager.MessageManager _mess = messManager;
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

        [HttpGet("unread")]
        public async Task<IActionResult> HasUnreadMessages()
        {
            try
            {
                var user = await _api.GetLoggedInUserAsync();
                if (user == null)
                {
                    Console.WriteLine("No authenticated user");
                    return Unauthorized();
                }

                var unread = await _mess.HasUnread(user.Id);
                Console.WriteLine($"Unread status for {user.Id}: {unread}");

                return Ok(unread);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HasUnreadMessages: {ex}");
                return StatusCode(500);
            }
        }
    }
}
