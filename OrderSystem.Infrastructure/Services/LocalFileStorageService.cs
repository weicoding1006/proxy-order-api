using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrderSystem.Application.Interfaces;

namespace OrderSystem.Infrastructure.Services;

public class LocalFileStorageService(IWebHostEnvironment env) : IFileStorageService
{
    public async Task<string> SaveImageAsync(IFormFile file, string folder)
    {
        var uploadDir = Path.Combine(env.WebRootPath, folder);
        Directory.CreateDirectory(uploadDir);

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadDir, fileName);

        await using var stream = File.Create(filePath);
        await file.CopyToAsync(stream);

        return $"/{folder.Replace('\\', '/')}/{fileName}";
    }

    public void DeleteImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;

        var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(env.WebRootPath, relativePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
