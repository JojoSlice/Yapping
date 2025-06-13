using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace miniReddit.Pages
{
    public class IndexModel (Services.ImgUpload upload,APIManager.LikeManager likeManager,APIManager.MessageManager messageManager, APIManager.CommentManager commentManager,APIManager.PostManager postManager, APIManager.CategoryManager categoryManager, APIManager.UserManager userManager): PageModel
    {
        private readonly APIManager.CommentManager _comManager = commentManager;
        private readonly APIManager.UserManager _userManager = userManager;
        private readonly APIManager.PostManager _postManager = postManager;
        private readonly APIManager.LikeManager _likeManager = likeManager;
        private readonly APIManager.CategoryManager _categoryManager = categoryManager;
        private readonly Services.ImgUpload _upload = upload;
        private readonly APIManager.MessageManager _messManager = messageManager;
        public List<Models.Post> Posts { get; private set; } = new();
        public List<PostViewModel> PostViewModels { get; private set; } = new();

        public List<Models.Category> Categories { get; private set; } = new();

        [BindProperty]
        public string Title { get; set; } = string.Empty;
        [BindProperty]
        public string Category { get; set; } = string.Empty;
        [BindProperty]
        public string TextContent { get; set; } = string.Empty;
        [BindProperty]
        public IFormFile? Img { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedCategoryId { get; set; }
        [BindProperty]
        public string MessageText { get; set; } = string.Empty;
        [BindProperty]
        public string ReplyId { get; set; } = string.Empty;

        public string? userid { get; set; }


        public async Task OnGet()
        {
            Posts = await GetPostsAsync();
            Categories = await GetCategoriesAsync();

            if (!string.IsNullOrEmpty(SelectedCategoryId))
            {
                Posts = Posts.Where(p => p.CategoryId == SelectedCategoryId).ToList();
            }

            foreach (var post in Posts)
            {
                var postModel = await CreatePostViewModel(post);
                PostViewModels.Add(postModel);
            }

            userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        public async Task<Models.UserInfo> GetUserinfo(string userId)
        {
            var postUser = await _userManager.GetUserinfo(userId);
            return postUser;
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
                return BadRequest("No objId found");
            }
            var user = await _userManager.GetLoggedInUserAsync();
            if (user == null)
                return BadRequest("No user found");

            await _likeManager.Like(data.PostId, user.Id);
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
            var likes = await GetLikes(postid);
            var likeAmount = likes.Count();
            return Content(likeAmount.ToString());
        }

        public async Task<List<Models.Likes>> GetLikes(string objid)
        {
            var likes = await _likeManager.GetLikes(objid);
            return likes.ToList();
        }
        public async Task<RedirectToPageResult> OnPostAsync()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid == null)
            {
                ModelState.AddModelError("Error", "User not logged in!");
                return RedirectToPage();
            }

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
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToPage();
            }
        }
        public async Task<RedirectToPageResult> OnPostReplyAsync()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = new Models.Message { SendId = userid, ResiveId = ReplyId, Text = MessageText };
            try
            {
                await _messManager.SendMessage(message);
                Console.WriteLine("Message sent to " + message.ResiveId + " wrote: " + message.Text);
                return RedirectToPage();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); ModelState.AddModelError("", "Failed to send message"); }
            return RedirectToPage();

        }

        public async Task<List<Models.Post>> GetPostsAsync()
        {
            Console.WriteLine("Get posts körs");
            var posts = await _postManager.GetPosts();
            return posts;
        }
        public async Task<PostViewModel> CreatePostViewModel(Models.Post post)
        {
            var postModel = new PostViewModel
            {
                Post = post,
                Likes = await GetLikes(post.Id),
                Comments = await GetComments(post.Id),
                UserInfo = await GetUserinfo(post.UserId),
                Category = await GetCategory(post.CategoryId),
            };
            return postModel; 
        }

        public class PostViewModel
        {
            public Models.Post Post { get; set; }
            public List<Models.Likes> Likes { get; set; }
            public List<Models.Comment> Comments { get; set; }
            public Models.UserInfo UserInfo { get; set; }
            public Models.Category Category { get; set; }
        }
    }
}
