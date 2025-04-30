using Microsoft.AspNetCore.Mvc;

namespace miniReddit.Pages.Api
{
    [ApiController]
    [Route("api/isauthenticated")]
    public class IsAuthenticatedController : ControllerBase
    {
        private readonly Services.AuthenticationService _authenticationService;

        public IsAuthenticatedController(Services.AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("get")]
        public IActionResult Get()
        {
            Console.WriteLine("api");
            var isAuthenticated = _authenticationService.IsUserAuthenticated();
            return Ok(new { authenticated = isAuthenticated });
        }
    }
}
