using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
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
        [BindProperty]
        [DisplayName("Posts")]
        public int PostAmount { get; set; } = 0;
        [BindProperty]
        [DisplayName("Comments")]
        public int CommentAmount { get; set; } = 0;
        public async Task OnGetAsync()
        {
            var userId = _authentication.GetLoggedInUserId();
            if (userId == null)
                RedirectToPage("/Index");
            else
            {
                User = await _db.GetUserFromId(userId);
                CommentAmount = await GetCommentAmount();
                PostAmount = await GetPostAmount();
            }
        }
        public async Task<int> GetPostAmount()
        {
            var posts = await _db.GetUserPosts(User);
            if(posts.Any())
            {
                return posts.Count();
            }
            else
            {
                return 0;
            }
        }
        public async Task<int> GetCommentAmount()
        {
            var comments = await _db.GetUserComments(User);
            if (comments.Any())
            {
                return comments.Count();
            }
            else
                return 0;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("onPost runs");
            if (ProfilePic == null || ProfilePic.Length == 0)
            {
                ModelState.AddModelError("", "No file selected.");
                return Page();
            }
            var userId = _authentication.GetLoggedInUserId();
            if (userId == null)
            {
                ModelState.AddModelError("", "Not logged in.");
                return RedirectToPage("/Index");
            }
            else
            {
                try
                {
                    if (ProfilePic != null && (ProfilePic.ContentType == "image/png" || ProfilePic.ContentType == "image/jpeg"))
                    {

                        var oldFileName = Path.GetFileName(ProfilePic.FileName);
                        var fileExt = Path.GetExtension(ProfilePic.FileName);
                        var fileName = Guid.NewGuid().ToString() + fileExt;
                        var user = await _db.GetUserFromId(userId);
                        var safeUsername = string.Concat(user.Username.Split(Path.GetInvalidFileNameChars()));

                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/uploads/{safeUsername}");

                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        var filePath = Path.Combine(uploadFolder, fileName);
                        using var stream = new FileStream(filePath, FileMode.Create);
                        await ProfilePic.CopyToAsync(stream);

                        var relativePath = $"/uploads/{safeUsername}/{fileName}";
                        await _db.UpdateProfilePic(userId, relativePath);
                        Console.WriteLine("Sparad bild i" + relativePath);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                    Console.WriteLine(ex.Message);
                    return Page();
                }
            }

            return RedirectToPage("/ProfilePage");
        }
    }
}
