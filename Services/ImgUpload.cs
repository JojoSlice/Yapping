namespace miniReddit.Services
{
    public class ImgUpload()
    {
        public async Task<string> Upload(IFormFile img, string username)
        {
             if(img == null)
            {
                return string.Empty;
            }
            if(img.ContentType != "image/png" && img.ContentType != "image/jpeg")
            {
                return string.Empty;
            }
            var folder = string.Empty;
            var fileName = string.Empty;
            try
            {
                var oldFilePath = Path.GetFileName(img.FileName);
                var fileExt = Path.GetExtension(img.FileName);
                fileName = Guid.NewGuid().ToString() + fileExt;
                var userDir = string.Concat(username.Split(Path.GetInvalidFileNameChars()));
                folder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/uploads/{userDir}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }

            if(!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var filePath = Path.Combine(folder, fileName);

            try
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await img.CopyToAsync(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }

            var realativePath = $"/uploads/{username}/{fileName}";

            return realativePath;
        }
    }
}
