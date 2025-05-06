using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Bcpg;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace miniReddit.Pages
{
    public class LogInModel(APIManager.UserManager userManager) : PageModel
    {
        private readonly APIManager.UserManager _userManager = userManager;

        [BindProperty]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (await _userManager.LogIn(UserName, Password))
            {
                var userId = await _userManager.GetUserIdAsync(UserName);

                Console.WriteLine(userId);  

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, UserName),
                    new(ClaimTypes.NameIdentifier, userId.ToString())
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                Console.WriteLine("LogIn Lyckades");

                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "Wrong username or password");
            return Page();
        }
    }
}
