using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Seminar.APPLICATION.Dtos.FirebaseDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;

namespace Seminar.APPLICATION.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;
        public FirebaseService(IConfiguration configuration)
        {
            var firebaseConfig = configuration.GetSection("Firebase");
            // Lấy tên bucket từ cấu hình
            _bucketName = firebaseConfig["StorageBucket"]
                ?? throw new ArgumentNullException("StorageBucket", "Tên bucket là bắt buộc trong cấu hình.");
            // Lấy đường dẫn file JSON Credential
            var credentialPath = firebaseConfig["CredentialPath"]
                ?? throw new ArgumentNullException("CredentialPath", "Đường dẫn file JSON Credential là bắt buộc trong cấu hình.");
            if (!File.Exists(credentialPath))
                throw new FileNotFoundException($"File Credential không tồn tại: {credentialPath}");
            var credential = GoogleCredential.FromFile(credentialPath);
            _storageClient = StorageClient.Create(credential);
        }
        public async Task<string> UploadFileAsync(CreateFirebaseDto createFirebaseDto)
        {
            if (!FirebaseConstants.AllFolders.Contains(createFirebaseDto.FolderName))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_DATA, "Tên thư mục không hợp lệ.");
            }
            if (createFirebaseDto.File == null || createFirebaseDto.File.Length == 0)
                throw new ArgumentException("No file uploaded.");
            var fileName = $"{createFirebaseDto.FolderName}/{Guid.NewGuid()}_{createFirebaseDto.File.FileName}";
            using var stream = createFirebaseDto.File.OpenReadStream();
            await _storageClient.UploadObjectAsync(
                _bucketName, fileName, createFirebaseDto.File.ContentType ?? "application/octet-stream", stream,
                new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }
            );
            return GetFileUrl(fileName);
        }

        public string GetFileUrl(string fileName)
        {
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(fileName)}?alt=media";
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                // Trích xuất tên file từ URL, bao gồm cả đường dẫn thư mục
                var uri = new Uri(fileUrl);
                var fileName = uri.Segments[uri.Segments.Length - 1];
                fileName = Uri.UnescapeDataString(fileName);

                await _storageClient.DeleteObjectAsync(_bucketName, fileName);
            }
            catch (Exception ex)
            {
                throw; // Re-throw để caller có thể xử lý
            }
        }
    }
}
