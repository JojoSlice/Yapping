using Microsoft.AspNetCore.Hosting;

namespace miniReddit.Services
{
    public class ImgUpload(IWebHostEnvironment webHostEnviroment)
    {
        private readonly IWebHostEnvironment _webEnv = webHostEnviroment;
        public async Task<string> Upload(IFormFile imageFile, string username)
        {
            // Validation (same as before)
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("No image file provided");

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required");

            // Additional security: sanitize username
            var cleanUsername = Path.GetInvalidFileNameChars()
                .Aggregate(username, (current, c) => current.Replace(c.ToString(), "_"));

            // Azure-compatible path construction
            string uploadsFolder = Path.Combine(_webEnv.WebRootPath, "uploads", cleanUsername);

            // Create directory if needed
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate unique filename with original extension
            string safeFileName = Path.GetFileNameWithoutExtension(imageFile.FileName)
                .Replace(" ", "_");
            safeFileName = Path.GetInvalidFileNameChars()
                .Aggregate(safeFileName, (current, c) => current.Replace(c.ToString(), "_"));

            string uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}{Path.GetExtension(imageFile.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Return web-accessible path
            return $"/uploads/{cleanUsername}/{uniqueFileName}";
        }
    }
}
