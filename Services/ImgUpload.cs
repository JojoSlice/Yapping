namespace miniReddit.Services
{
    public class ImgUpload(IWebHostEnvironment env)
    {
        private readonly string _webRoot = env.WebRootPath;

        public async Task<string> Upload(IFormFile img, string username)
        {
            if (img == null ||
                (img.ContentType != "image/png" && img.ContentType != "image/jpeg"))
            {
                return string.Empty;
            }

            var fileExt = Path.GetExtension(img.FileName);
            var fileName = Guid.NewGuid() + fileExt;

            var userDir = username.Replace(" ", "").Replace("/", "").Replace("\\", "").ToLowerInvariant();
            var folderPath = Path.Combine(_webRoot, "uploads", userDir);

            try
            {
                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await img.CopyToAsync(stream);

                var relativePath = $"/uploads/{userDir}/{fileName}";
                return relativePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image upload failed: {ex.Message}");
                return string.Empty;
            }
        }
    }
 
}
