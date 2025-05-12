using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using miniReddit.Models;
using System.Security.Claims;

namespace miniReddit.Pages
{
    public class IndexModel(Services.ImgUpload upload,APIManager.PostManager postManager, APIManager.CategoryManager categoryManager, APIManager.UserManager userManager) : PageModel
    {
        private readonly Services.ImgUpload _upload = upload;
        private readonly APIManager.UserManager _userManager = userManager;
        private readonly APIManager.PostManager _postManager = postManager;
        private readonly APIManager.CategoryManager _categoryManager = categoryManager;
        public List<Post> Posts { get; private set; } = new();

        [BindProperty]
        public string Title { get; set; } = string.Empty;
        [BindProperty]
        public string Category { get; set; } = string.Empty;
        [BindProperty]
        public string TextContent { get; set; } = string.Empty;
        [BindProperty]
        public IFormFile? Img { get; set; }
        public async Task OnGetAsync()
        {
            Console.WriteLine($"User logged in: {User.Identity?.IsAuthenticated}");
        }

        public async Task<PageResult> OnPostAsync()
        {

            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid == null)
                return Page();
            var user = await _userManager.GetUserById(userid);    

            var cat = await _categoryManager.CategoryCheck(Category);
            var imgPath = string.Empty;
            if (Img != null)
            {
                imgPath = await _upload.Upload(Img, user.Username);
            }

            var post = new Post(userid, cat.Id, TextContent, Title, imgPath);

            return Page();
        }

        public async Task<JsonResult> OnGetMorePostsAsync(DateTime lastCreatedAt)
        {
            Console.WriteLine("More post körs");
            var morePosts = await _postManager.GetLatestPosts(lastCreatedAt);
            return new JsonResult(morePosts);
        }

        public async Task<JsonResult> OnGetPostUserAsync(string id)
        {
            var user = await _userManager.GetUserById(id);
            if(user != null) 
                return new JsonResult(user);
            else 
                return new JsonResult(null);
        }

        public async Task<JsonResult> OnGetCategoryAsync(string id)
        {
            var category = await _categoryManager.GetCategory(id);
            return new JsonResult(category);
        }
        public async Task<string> GetCategoryName(string id)
        {
            var category = await _categoryManager.GetCategory(id);
            return category?.Name ?? string.Empty;
        }
    }
}
