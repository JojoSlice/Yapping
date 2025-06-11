using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Web.WebPages;

namespace miniReddit.Pages
{
    public class PostModel(APIManager.LikeManager likeManager,APIManager.MessageManager messageManager ,APIManager.CategoryManager categoryManager,APIManager.UserManager userManager,APIManager.CommentManager commentManager,APIManager.PostManager postManager) : PageModel
    {
        private readonly APIManager.CommentManager _comManager = commentManager;
        private readonly APIManager.UserManager _userManager = userManager;
        private readonly APIManager.PostManager _postManager = postManager;
        private readonly APIManager.CategoryManager _categoryManager = categoryManager; 
        private readonly APIManager.LikeManager _likeManager = likeManager; 
        private readonly APIManager.MessageManager _messManager = messageManager;
        public Models.Post? Post {  get; set; }
        [BindProperty]
        public List<Models.Comment>? Comments { get; set; }
        [BindProperty]
        public string TextContent { get; set; } = string.Empty;
        [BindProperty]
        public string Postid { get; set; } = string.Empty;
        [BindProperty]
        public Models.UserInfo? PostUser { get; set; }
        [BindProperty]
        public Models.Category? Category { get; set; }
        [BindProperty]
        public List<Models.Likes>? Likes { get; set; }
        [BindProperty]
        public string CommentText { get; set; } = string.Empty;
        [BindProperty]
        public string CommentId { get; set; } = string.Empty;
        public List<CommentViewModel> ViewComments { get; set; } = new();
        [BindProperty]
        public string MessageText { get; set; } = string.Empty;
        [BindProperty]
        public string ReplyId { get; set; } = string.Empty;



        public async Task OnGet(string id)
        {
            Post = await _postManager.GetPost(id);
            Comments = await GetComments(id);
            ViewComments = await GetCommentViewModelList(Comments);
            PostUser = await GetUserinfo(Post.UserId);
            Category = await GetCategory(Post.CategoryId);
            Likes = await GetLikes(Post.Id);
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
        public async Task<RedirectToPageResult> OnPostCreateCommentAsync()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid.IsEmpty())
                return RedirectToPage();
           
            var comment = new Models.Comment() { PostId = CommentId, Text = CommentText, UserId = userid };

            await _comManager.NewComment(comment);
            return RedirectToPage();
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

        public async Task<List<CommentViewModel>> GetCommentViewModelList(List<Models.Comment> comments)
        {
            var ViewCommentsModel = new List<CommentViewModel>();
            foreach (var comment in comments)
            {
                var repliesToComment = await GetComments(comment.Id);

                var viewModel = new CommentViewModel
                {
                    Comment = comment,
                    UserInfo = await GetUserinfo(comment.UserId),
                    Likes = await GetLikes(comment.Id),
                    Replies = await GetCommentViewModelList(repliesToComment.ToList())
                };
                ViewCommentsModel.Add(viewModel);
            }
            return ViewCommentsModel;
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
               
        public async Task<Models.UserInfo> GetUserinfo(string id)
        {
            return await _userManager.GetUserinfo(id);
        }

    }
    public class CommentViewModel
    {
        public Models.Comment Comment { get; set; }
        public Models.UserInfo UserInfo { get; set; }
        public List<Models.Likes> Likes { get; set; }
        public List<CommentViewModel> Replies { get; set; }
    }
}
