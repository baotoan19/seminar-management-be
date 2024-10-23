using Microsoft.AspNetCore.Http;

namespace Seminar.APPLICATION.Interfaces;

public interface IFirebaseService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName);
    Task DeleteFileAsync(string fileName);
    string GetFileUrl(string fileName);
}