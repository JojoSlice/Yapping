using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using miniReddit.Models;

namespace miniReddit.Pages
{
    public class IndexModel() : PageModel
    {

        public List<Post> Posts { get; private set; } = new();

        public async Task OnGetAsync()
        {
            Console.WriteLine($"User logged in: {User.Identity?.IsAuthenticated}");
        }

        //public async Task<JsonResult> OnGetMorePostsAsync(DateTime lastCreatedAt)
        //{
        //    var morePosts = await _db.GetLatestPosts(lastCreatedAt);
        //    return new JsonResult(morePosts);
        //}

        //public async Task<JsonResult> OnGetCategoryAsync(string id)
        //{
        //    var category = await _db.GetCategory(id);
        //    return new JsonResult(category);
        //}
        //public async Task<string> GetCategoryName(string id)
        //{
        //    var category = await _db.GetCategory(id);
        //    return category?.Name ?? string.Empty;
        //}
    }
}
