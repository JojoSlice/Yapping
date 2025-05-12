using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace miniReddit.Pages
{
    public class RegistrationModel(APIManager.UserManager userManager) : PageModel
    {
        private readonly APIManager.UserManager _userManager = userManager;

        [BindProperty]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;
        [BindProperty]
        public string Email { get; set; } = string.Empty;
        [BindProperty]
        public string Password { get; set; } = string.Empty;
        [BindProperty]
        [Display(Name = "Re-Enter Password")]
        public string ReEnterPassword { get; set; } = string.Empty;

        public void OnGet()
        {
        }
    

public async Task<IActionResult> OnPostAsync()
        {
            bool IsUsernameTaken = await _userManager.CheckUsername(UserName);
            bool IsEmailTaken = await _userManager.CheckEmail(Email);
            if (IsUsernameTaken)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return Page();
            }
            else if (IsEmailTaken)
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return Page();
            }
            else if (Password != ReEnterPassword)
            {
                ModelState.AddModelError("Password", "Passwords must match.");
                return Page();
            }
            else
            {
                var password = BCrypt.Net.BCrypt.HashPassword(Password);
                var newUser = new Models.User
                {
                    Username = UserName,
                    Email = Email,
                    Password = password,
                };


                if (await _userManager.Register(newUser))
                    return RedirectToPage("Index");
                else
                    return Page();
            }

        }
    }
}


