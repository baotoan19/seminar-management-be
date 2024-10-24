using Microsoft.AspNetCore.Http;
using Seminar.APPLICATION.Dtos.FirebaseDtos;

namespace Seminar.APPLICATION.Interfaces;

public interface IFirebaseService
{
    Task<string> UploadFileAsync(CreateFirebaseDto createFirebaseDto);
    Task DeleteFileAsync(string fileName);
    string GetFileUrl(string fileName);
}