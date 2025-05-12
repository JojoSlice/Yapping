using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using miniReddit.Models;

namespace miniReddit.Pages
{
    public class IndexModel(APIManager.PostManager postManager, APIManager.CategoryManager categoryManager) : PageModel
    {
        private readonly APIManager.PostManager _postManager = postManager;
        private readonly APIManager.CategoryManager _categoryManager = categoryManager;
        public List<Post> Posts { get; private set; } = new();

        public async Task OnGetAsync()
        {
            Console.WriteLine($"User logged in: {User.Identity?.IsAuthenticated}");
        }

        public async Task<JsonResult> OnGetMorePostsAsync(DateTime lastCreatedAt)
        {
            Console.WriteLine("More post körs");
            var morePosts = await _postManager.GetLatestPosts(lastCreatedAt);
            return new JsonResult(morePosts);
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
