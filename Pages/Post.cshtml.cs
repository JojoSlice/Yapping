using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Web.WebPages;

namespace miniReddit.Pages
{
    public class PostModel(APIManager.LikeManager likeManager,APIManager.CategoryManager categoryManager,APIManager.UserManager userManager,APIManager.CommentManager commentManager,APIManager.PostManager postManager) : PageModel
    {
        private readonly APIManager.CommentManager _comManager = commentManager;
        private readonly APIManager.UserManager _userManager = userManager;
        private readonly APIManager.PostManager _postManager = postManager;
        private readonly APIManager.CategoryManager _categoryManager = categoryManager; 
        private readonly APIManager.LikeManager _likeManager = likeManager; 
        public Models.Post? Post {  get; set; }
        [BindProperty]
        public List<Models.Comment>? Comments { get; set; }
        [BindProperty]
        public string TextContent { get; set; } = string.Empty;
        [BindProperty]
        public string Postid { get; set; } = string.Empty;
        [BindProperty]
        public Models.User? AktiveUser { get; set; }
        [BindProperty]
        public Models.UserInfo? PostUser { get; set; }
        [BindProperty]
        public Models.Category? Category { get; set; }
        [BindProperty]
        public List<Models.Likes>? Likes { get; set; }


        public async Task OnGet(string id)
        {
            Post = await _postManager.GetPost(id);
            Comments = await GetComments(id);
            await LoadUser();
            PostUser = await GetUserinfo(Post.UserId);
            Category = await GetCategory(Post.CategoryId);
            Likes = await GetLikes(Post.Id);

            Console.WriteLine(AktiveUser.Username + "---------------------------------------");
        }

        public async Task<RedirectToPageResult> OnPostAsync()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid.IsEmpty())
                return RedirectToPage();


            if(Postid.IsEmpty())
            {
                Console.WriteLine("post är empty");
                return RedirectToPage();
            }    

            
            var text = TextContent;
            if (text.IsEmpty())
                return RedirectToPage();

            var comment = new Models.Comment(userid, Postid, text);

            var result = await _comManager.NewComment(comment);
            if (!result)
                Console.WriteLine("Could not post comment");

            return RedirectToPage();

        }

        public async Task<List<Models.Comment>> GetComments(string id)
        {
            var comments = await _comManager.GetPostComments(id);
            if (comments != null)
                return comments;
            else
                return new();
        }

        public async Task<List<Models.Likes>> GetLikes(string objid)
        {
            var likes = await _likeManager.GetLikes(objid);
            return likes.ToList();
        }

        public async Task<Models.Category> GetCategory(string id)
        {
            var category = await _categoryManager.GetCategory(id);
            if (category != null)
                return category;
            else
                return new();
        }
        public async Task<Models.User> GetUser(string id)
        {
            Console.WriteLine(id);
            var user = await _userManager.GetUserById(id);
            if (user != null)
                return user;
            else
                return new();
        }
        public async Task<bool> LoadUser()
        {
            AktiveUser = await _userManager.GetLoggedInUserAsync();
            if (AktiveUser != null)
                return true;
            else
                return false;
        }

        public async Task<Models.UserInfo> GetUserinfo(string id)
        {
            return await _userManager.GetUserinfo(id);
        }

    }
}
