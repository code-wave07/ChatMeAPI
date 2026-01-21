using ChatMe.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ChatMe.Infrastructure.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            // 1. Validate
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Invalid image format. Only JPG, PNG, and GIF allowed.");

            // 2. Create Unique Name (to prevent overwriting)
            var fileName = $"{Guid.NewGuid()}{extension}";

            // 3. Define Path (wwwroot/uploads)
            var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, fileName);

            // 4. Save
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Return relative URL
            // Assuming your API runs on localhost:7000, result is "/uploads/abc.jpg"
            return $"/uploads/{fileName}";
        }
    }
}
