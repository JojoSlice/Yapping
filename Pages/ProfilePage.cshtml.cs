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
            if (ProfilePic == null || ProfilePic.Length == 0)
            {
                ModelState.AddModelError("", "No file selected.");
                return Page();
            }

            var fileName = Path.GetFileName(ProfilePic.FileName);
            if (User == null)
            {
                ModelState.AddModelError("", "Not logged in.");
                return RedirectToPage("/Index");
            }
            else
            {
                try
                {


                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/", User.Username);

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    var filePath = Path.Combine(uploadFolder, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await ProfilePic.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                    Console.WriteLine(ex.Message);
                    return Page();
                }
            }

            await _db.UpdateProfilePic(User.Id, $"/uploads/{User.Username}/{fileName}");
            return RedirectToPage("/Profile");
        }
    }
}
