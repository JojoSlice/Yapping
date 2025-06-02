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
        public Models.Post? post {  get; set; }
        public List<Models.Comment>? comments { get; set; }
        [BindProperty]
        public string TextContent { get; set; } = string.Empty;
        [BindProperty]
        public string postid { get; set; } = string.Empty;

        public async Task OnGet(string id)
        {
            post = await _postManager.GetPost(id);
            comments = await _comManager.GetPostComments(id);
        }

        public async Task OnPostAsync()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid.IsEmpty())
                return;

            if(postid.IsEmpty())
            {
                Console.WriteLine("post är empty");
                return;
            }    

            
            var text = TextContent;
            if (text.IsEmpty())
                return;

            var comment = new Models.Comment(userid, postid, text);

            var result = await _comManager.NewComment(comment);
            if (!result)
                Console.WriteLine("Could not post comment");
        }

        public async Task<List<Models.Comment>> GetComments(string id)
        {
            var comments = await _comManager.GetPostComments(id);
            if (comments != null)
                return comments;
            else
                return [];
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
            var user = await _userManager.GetUserById(id);
            if (user != null)
                return user;
            else
                return new();
        }
    }
}
