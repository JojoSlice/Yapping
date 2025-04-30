using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace miniReddit.Pages.Shared
{
    public class SignInPartialModel(Services.MongoDB mongoDb) : PageModel
    {
        private readonly Services.MongoDB _mongoDb = mongoDb;

        [BindProperty]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;
        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(await _mongoDb.LogIn(UserName, Password))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, UserName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");

                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity));
                return RedirectToPage("/Index");
            }
            ModelState.AddModelError("Sign in", "Wrong username or password");
            return Page();
        }
    }
}
