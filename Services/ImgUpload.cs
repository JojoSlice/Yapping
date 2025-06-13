using Microsoft.AspNetCore.Hosting;

namespace miniReddit.Services
{
    public class ImgUpload(IWebHostEnvironment webHostEnviroment)
    {
        private readonly IWebHostEnvironment _webEnv = webHostEnviroment;
        public async Task<string> Upload(IFormFile imageFile, string username)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("No image file provided");

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required");

            var cleanUsername = Path.GetInvalidFileNameChars()
                .Aggregate(username, (current, c) => current.Replace(c.ToString(), "_"));

            string uploadsFolder = Path.Combine(_webEnv.WebRootPath, "uploads", cleanUsername);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string safeFileName = Path.GetFileNameWithoutExtension(imageFile.FileName)
                .Replace(" ", "_");
            safeFileName = Path.GetInvalidFileNameChars()
                .Aggregate(safeFileName, (current, c) => current.Replace(c.ToString(), "_"));

            string uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}{Path.GetExtension(imageFile.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/uploads/{cleanUsername}/{uniqueFileName}";
        }
    }
}
