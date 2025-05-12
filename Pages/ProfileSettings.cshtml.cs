using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace miniReddit.Pages
{
    public class ProfileSettingsModel(APIManager.UserManager userManager, Services.ImgUpload upload) : PageModel
    {
        private readonly Services.ImgUpload _upload = upload;
        private readonly APIManager.UserManager _api = userManager;
        [BindProperty]
        public Models.User? AktiveUser { get; set; }

        [BindProperty]
        public IFormFile? NewImg { get; set; }
        public async Task OnGetAsync()
        {
            var load = await LoadUser();
            if(!load)
            {
                Redirect();
            }
        }
        public async Task OnPostAsync()
        {
            var load = await LoadUser();
            if(!load)
            {
                ModelState.AddModelError("No user", "User not logged in.");
                Redirect();
            }

            var realativePath = await _upload.Upload(NewImg, AktiveUser.Username);

            try
            {
                var result = await _api.ChangeUserImg(AktiveUser.Id, realativePath);
                if (!result)
                    throw new Exception("Api failed.");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Error", "File could not be saved");
                Console.WriteLine(ex.Message);
                Redirect();
            }

            Redirect();
        }
        public RedirectToPageResult Redirect()
        {
            return RedirectToPage("/Index");
        }
        public async Task<bool> LoadUser()
        {
            AktiveUser = await _api.GetLoggedInUserAsync();
            if (AktiveUser != null)
                return true;
            else
                return false;
        }
    }
}
