using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using miniReddit.Models;
using System.Security.Claims;

namespace miniReddit.Pages
{
    public class IndexSPARAModel(Services.ImgUpload upload,APIManager.PostManager postManager, APIManager.CategoryManager categoryManager, APIManager.UserManager userManager) : PageModel
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
            Posts = await GetPostsAsync();
        }

        public async Task<JsonResult> OnPostAsync()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid == null)
                return new JsonResult(new { success = false, message = "User not authenticated" });

            var user = await _userManager.GetUserById(userid);

            var cat = await _categoryManager.CategoryCheck(Category);
            var imgPath = string.Empty;
            if (Img != null)
            {
                imgPath = await _upload.Upload(Img, user.Username);
            }

            var post = new Post(userid, cat.Id, TextContent, Title, imgPath);

            try
            {
                await _postManager.CreatePost(post);
                return new JsonResult(new { success = true, post });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            Console.WriteLine("Get post körs");
            var posts = await _postManager.GetPosts();
            return posts;
        }

        public async Task<JsonResult> OnGetMorePostsAsync(long lastCreatedAtMs)
        {
            Console.WriteLine("More post körs");

            var lastCreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(lastCreatedAtMs).UtcDateTime;

            Console.WriteLine(lastCreatedAt);
            var morePosts = await _postManager.GetLatestPosts(lastCreatedAt);
            if(morePosts.Any())
            {
            foreach(var post in morePosts)
            {
                Console.WriteLine(post.Id);
            }
            }
            else
            {
                Console.WriteLine("inga posts");
            }
                return new JsonResult(morePosts);
        }

        public JsonResult OnGetLastPost()
        {
            Console.WriteLine("get last post");

            var oldestPost = Posts.OrderBy(p => p.CreatedAt).FirstOrDefault();

            if (oldestPost == null)
            {
                return new JsonResult(new { success = false, message = "No posts available" });
            }

            return new JsonResult(new
            {
                success = true,
                createdAt = oldestPost.CreatedAt.ToUniversalTime().ToString("o") // ISO 8601-format
            });
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
