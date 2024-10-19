using Microsoft.AspNetCore.Http;

namespace Seminar.APPLICATION.Interfaces;

public interface IFirebaseService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task<bool> DeleteFileAsync(string fileName);
    string GetFileUrl(string fileName);
}