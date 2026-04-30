using Microsoft.AspNetCore.Http;

namespace OrderSystem.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(IFormFile file, string folder);
    void DeleteImage(string imageUrl);
}
