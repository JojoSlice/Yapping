using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace miniReddit.Pages
{
    public class IndexModel (Services.ImgUpload upload, APIManager.CommentManager commentManager,APIManager.PostManager postManager, APIManager.CategoryManager categoryManager, APIManager.UserManager userManager): PageModel
    {
        private readonly APIManager.CommentManager _comManager = commentManager;
        private readonly APIManager.UserManager _userManager = userManager;
        private readonly APIManager.PostManager _postManager = postManager;
        private readonly APIManager.CategoryManager _categoryManager = categoryManager;
        private readonly Services.ImgUpload _upload = upload;
        public List<Models.Post> Posts { get; private set; } = new();

        public List<Models.Category> Categories { get; private set; } = new();

        [BindProperty]
        public string Title { get; set; } = string.Empty;
        [BindProperty]
        public string Category { get; set; } = string.Empty;
        [BindProperty]
        public string TextContent { get; set; } = string.Empty;
        [BindProperty]
        public IFormFile? Img { get; set; }

        public async Task OnGet()
        {
            Posts = await GetPostsAsync();
            Categories = await GetCategoriesAsync();
        }

        public async Task<List<Models.Category>> GetCategoriesAsync()
        {
            var categories = await _categoryManager.GetCategoriesAsync();
            return categories;
        }

        public async Task<List<Models.Comment>> GetComments(string postId)
        {
            var comments = await _comManager.GetPostComments(postId);
            return comments;
        }
        public async Task<Models.Category> GetCategory(string catId)
        {
            var cat = await _categoryManager.GetCategory(catId);
            return cat;
        }

        public async Task<string> GetUsername(string userId)
        {
            var postUser = await _userManager.GetUserById(userId);
            return postUser.Username;
        }

        public async Task<IActionResult> OnPostLikePostAsync()
        {
            Console.WriteLine("OnPostLikePostAsync");

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            Console.WriteLine("Raw Body: " + body);

            var data = JsonSerializer.Deserialize<LikeRequest>(body);

            Console.WriteLine(data.PostId);

            if (string.IsNullOrEmpty(data?.PostId))
            {
                return BadRequest();
            }

            await _postManager.LikePost(data.PostId);
            return new OkResult();
        }

        public class LikeRequest
        {
            [JsonPropertyName("postId")]
            public string PostId { get; set; }
        }
        public async Task<IActionResult> OnGetLikesOnPost(string postid)
        {
            Console.WriteLine("OnGetLikesOnPost");
            var post = await _postManager.GetPost(postid);
            var likes = post.Like;
            return Content(likes.ToString());
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

            var post = new Models.Post(userid, cat.Id, TextContent, Title, imgPath);

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
        public async Task<List<Models.Post>> GetPostsAsync()
        {
            Console.WriteLine("Get posts körs");
            var posts = await _postManager.GetPosts();
            return posts;
        }
    }
}
