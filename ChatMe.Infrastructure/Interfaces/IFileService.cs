using Microsoft.AspNetCore.Http;

namespace ChatMe.Infrastructure.Interfaces
{
    public interface IFileService
    {
        // Returns the URL of the saved file
        Task<string> SaveImageAsync(IFormFile file);
    }
}
