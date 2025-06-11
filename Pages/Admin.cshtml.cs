using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.WebPages;

namespace miniReddit.Pages
{
    public class AdminModel(APIManager.ReportManager reportManager,APIManager.PostManager postManager, APIManager.CommentManager commentManager, APIManager.UserManager userManager) : PageModel
    {
        private readonly APIManager.ReportManager _reportManager = reportManager;
        private readonly APIManager.PostManager _postManager = postManager;
        private readonly APIManager.CommentManager _commentManager = commentManager;
        private readonly APIManager.UserManager _userManager = userManager;
        public List<Models.Report> Reports { get; set; } = new();
        public List<ReportModel> ReportModels { get; set; } = new();
        public List<Models.Report> AllReports { get; set; }
        public Models.User AdminUser { get; set; }
        public async Task OnGet()
        {
            AdminUser = await GetUser();
            if (AdminUser == null || !AdminUser.Admin)
                RedirectToPage("/Index");

            Reports = await GetAllUnHandledReports();
            foreach (var report in Reports)
            {
                var model = await GetReportModel(report);
                ReportModels.Add(model);
            }
        }
        public async Task OnPostReportComment(string commentid)
        {
            var report = new Models.Report
            {
                CommentId = commentid,
            };
            try
            {
                await _reportManager.CreateRaport(report);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            };
        }

        public async Task OnPostReportPost(string postid)
        {
            var report = new Models.Report
            {
                PostId = postid,
            };
            try
            {
                await _reportManager.CreateRaport(report);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IActionResult> OnPostMarkAsHandled(string Id)
        {
            try
            {
                await _reportManager.MarkReportAsHandled(Id);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDelete(string Id)
        {
            var report = Reports.Where(R => R.Id == Id).FirstOrDefault();
            try
            {
                if (string.IsNullOrWhiteSpace(report.PostId))
                    await _reportManager.DeleteComment(report.CommentId);
                else
                    await _reportManager.DeletePost(report.PostId);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToPage();
        }
        public async Task<Models.User> GetUser()
        {
            var userid = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _userManager.GetUserById(userid);

        }
        public async Task<Models.Post> GetPost(string id)
        {
            return await _postManager.GetPost(id);
        }
        public async Task<Models.Comment> GetComment(string id)
        {
            return await _commentManager.GetComment(id);
        }
        public async Task<List<Models.Report>> GetAllUnHandledReports()
        {
            return await _reportManager.GetUnReadReports();
        }
        public async Task<List<Models.Report>> GetAllReports()
        {
            return await _reportManager.GetAllReports();
        }

        public async Task<ReportModel> GetReportModel(Models.Report report)
        {
            if(string.IsNullOrWhiteSpace(report.CommentId))
            {
                var post = await GetPost(report.PostId);
                var userModel = await _userManager.GetUserinfo(post.UserId);
                var model = new ReportModel
                {
                    Report = report,
                    Post = post,
                    UserInfo = userModel
                };
                return model;
            }
            else
            {
                var comment = await GetComment(report.CommentId);
                var userModel = await _userManager.GetUserinfo(comment.UserId);
                var model = new ReportModel
                {
                    Report = report,
                    Comment = comment,
                    UserInfo = userModel
                };
                return model;
            }

        }
        public class ReportModel
        {
            public Models.Report Report { get; set; }
            public Models.Post? Post { get; set; }
            public Models.Comment? Comment { get; set; }
            public Models.UserInfo UserInfo { get; set; }
        }
    }
}
