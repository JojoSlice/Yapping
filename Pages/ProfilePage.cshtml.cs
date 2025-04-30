using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace miniReddit.Pages
{
    public class ProfilePageModel(Services.MongoDB mongoDb, Services.AuthenticationService authentication) : PageModel
    {
        private readonly Services.MongoDB _db = mongoDb;
        private readonly Services.AuthenticationService _authentication = authentication;

        [BindProperty]
        public IFormFile? ProfilePic { get; set; }

        [BindProperty]
        public new Models.User? User { get; set; }
        public async Task OnGetAsync()
        {
            var userId = _authentication.GetLoggedInUserId();
            if (userId == null)
                RedirectToPage("/Index");
            else
                User = await _db.GetUserFromId(userId);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ProfilePic == null || ProfilePic.Length == 0)
            {
                ModelState.AddModelError("", "No file selected.");
                return Page();
            }

            var fileName = Path.GetFileName(ProfilePic.FileName);
            if (User == null)
            {
                ModelState.AddModelError("", "Not logged in.");
                RedirectToPage("/Index");
            }
            else
            {

                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", User.Username);

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var filePath = Path.Combine(uploadFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                    await ProfilePic.CopyToAsync(stream);
            }

            await _db.UpdateProfilePic(User.Id ,"/uploads/" + User.Username + fileName);
            return RedirectToPage("/Profile");
        }
    }
}
