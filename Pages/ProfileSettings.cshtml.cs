using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace miniReddit.Pages
{
    public class ProfileSettingsModel(APIManager.UserManager userManager) : PageModel
    {
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
            Console.WriteLine("OnPost-----------------------------------------------------------------------");
            if(NewImg == null)
            {
                ModelState.AddModelError("No img", "No img found.");
                Redirect();
            }
            var load = await LoadUser();
            if(!load)
            {
                ModelState.AddModelError("No user", "User not logged in.");
                Redirect();
            }
            if(NewImg.ContentType != "image/png" && NewImg.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("Error", "Wrong file format.");
                Redirect();
            }
            var folder = string.Empty;
            var fileName = string.Empty;
            try
            {
                var oldFilePath = Path.GetFileName(NewImg.FileName);
                var fileExt = Path.GetExtension(NewImg.FileName);
                fileName = Guid.NewGuid().ToString() + fileExt;
                var userDir = string.Concat(AktiveUser.Username.Split(Path.GetInvalidFileNameChars()));
                folder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/uploads/{userDir}");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Error", "File could not be saved");
                Console.WriteLine(ex.Message);
                Redirect();
            }

            if(!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var filePath = Path.Combine(folder, fileName);

            try
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await NewImg.CopyToAsync(stream);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "File could not be saved");
                Console.WriteLine(ex.Message);
                Redirect();
            }

            var realativePath = $"/uploads/{AktiveUser.Username}/{fileName}";
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
