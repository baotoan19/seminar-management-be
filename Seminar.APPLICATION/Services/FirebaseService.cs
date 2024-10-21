using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Seminar.APPLICATION.Interfaces;

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
                ?? throw new ArgumentNullException("StorageBucket", "Bucket name is missing in configuration.");
            // Lấy đường dẫn file JSON Credential
            var credentialPath = firebaseConfig["CredentialPath"]
                ?? throw new ArgumentNullException("CredentialPath", "Credential path is missing in configuration.");
            if (!File.Exists(credentialPath))
                throw new FileNotFoundException($"Credential file not found: {credentialPath}");
            var credential = GoogleCredential.FromFile(credentialPath);
            _storageClient = StorageClient.Create(credential);
        }
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            using var stream = file.OpenReadStream();
            await _storageClient.UploadObjectAsync(
                _bucketName, fileName, file.ContentType ?? "application/octet-stream", stream,
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
                string fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
                await _storageClient.DeleteObjectAsync(_bucketName, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete file: {ex.Message}");
            }
        }
    }
}
