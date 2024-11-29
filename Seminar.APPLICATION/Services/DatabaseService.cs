using AutoMapper;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.DatabaseDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Enum;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;


    public DatabaseService(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
            throw new InvalidOperationException("Database connection string not found in environment variables");
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateBackupAsync(CreateBackupDto createBackupDto)
    {
        try
        {
            // Lấy tên database từ env
            var dbName = Environment.GetEnvironmentVariable("DB_NAME")
                ?? throw new ErrorException(StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND, "Tên cơ sở dữ liệu không tìm thấy trong các biến môi trường");

            // Kiểm tra đường dẫn backup
            if (!Path.IsPathRooted(createBackupDto.BackupPath))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "Đường dẫn backup không hợp lệ. Vui lòng cung cấp đường dẫn đầy đủ");
            }

            // Kiểm tra đường dẫn backup có tồn tại không
            var backupDirectory = new DirectoryInfo(createBackupDto.BackupPath);
            if (!backupDirectory.Exists)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.BADREQUEST, "Đường dẫn backup không tồn tại. Vui lòng cung cấp đường dẫn tồn tại");
            }

            // Tạo đường dẫn backup
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"{dbName}_{createBackupDto.BackupType}_{timestamp}.bak";
            var localBackupPath = Path.Combine(createBackupDto.BackupPath, backupFileName);

            // Thực thi backup
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string backupQuery = GetBackupQuery(dbName, localBackupPath, createBackupDto.BackupType);
                using var command = new SqlCommand(backupQuery, connection);
                command.CommandTimeout = 3600; // 1 giờ
                await command.ExecuteNonQueryAsync();
            }

            // Kiểm tra file backup có được tạo thành công không
            if (!File.Exists(localBackupPath))
            {
                throw new ErrorException(
                    StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.FAILED,
                    "Backup không thành công: File backup không được tạo");
            }

            // Verify backup
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var verifyQuery = $"RESTORE VERIFYONLY FROM DISK = N'{localBackupPath}'";
                using var command = new SqlCommand(verifyQuery, connection);
                command.CommandTimeout = 300; // 5 phút
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            throw new ErrorException(
                StatusCodes.Status500InternalServerError,
                ResponseCodeConstants.FAILED,
                $"Lỗi trong quá trình backup: {ex.Message}");
        }
    }

    private string GetBackupQuery(string databaseName, string backupPath, BackupType backupType)
    {
        switch (backupType)
        {
            case BackupType.FULL:
                return $@"BACKUP DATABASE [{databaseName}] 
                    TO DISK = '{backupPath}'
                    WITH FORMAT, INIT, NAME = N'Full Database Backup'";

            case BackupType.DIFFERENTIAL:
                return $@"BACKUP DATABASE [{databaseName}] 
                    TO DISK = '{backupPath}'
                    WITH DIFFERENTIAL, FORMAT, INIT, NAME = N'Differential Database Backup'";

            case BackupType.LOG:
                return $@"BACKUP LOG [{databaseName}] 
                    TO DISK = '{backupPath}'
                    WITH FORMAT, INIT, NAME = N'Transaction Log Backup'";

            default:
                throw new ArgumentException($"Invalid backup type: {backupType}");
        }
    }

    public async Task RestoreBackupAsync(CreateRestoreDto createRestoreDto)
    {
        // Validate backup file
        if (createRestoreDto.BackupFile == null || createRestoreDto.BackupFile.Length == 0)
        {
            throw new ErrorException(StatusCodes.Status400BadRequest,
                ResponseCodeConstants.BADREQUEST, "File backup không hợp lệ hoặc trống");
        }

        // Kiểm tra định dạng file
        if (!createRestoreDto.BackupFile.FileName.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
        {
            throw new ErrorException(StatusCodes.Status400BadRequest,
                ResponseCodeConstants.BADREQUEST, "Chỉ chấp nhận file .bak");
        }

        // Tạo thư mục temp nếu chưa tồn tại
        var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "TempBackups");
        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
        }

        // Tạo file tạm với tên unique
        var tempFileName = $"temp_backup_{DateTime.Now:yyyyMMddHHmmss}.bak";
        var tempFilePath = Path.Combine(tempDir, tempFileName);

        try
        {
            // Lưu file vào thư mục tạm
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await createRestoreDto.BackupFile.CopyToAsync(stream);
            }

            var dbName = Environment.GetEnvironmentVariable("DB_NAME")
                ?? throw new ErrorException(StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND, "Tên cơ sở dữ liệu không tìm thấy");

            string dataPath;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using var command = new SqlCommand(
                    "SELECT SERVERPROPERTY('InstanceDefaultDataPath')", connection);
                dataPath = (string)await command.ExecuteScalarAsync();
            }

            // Tạo connection string tới master database
            var masterConnection = _connectionString.Replace(dbName, "master");

            using (var connection = new SqlConnection(masterConnection))
            {
                await connection.OpenAsync();

                try
                {
                    // Set database to SINGLE_USER mode
                    var singleUserQuery = $@"
                    IF EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}')
                    BEGIN
                        ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    END";
                    using (var command = new SqlCommand(singleUserQuery, connection))
                    {
                        command.CommandTimeout = 300; // 5 phút
                        await command.ExecuteNonQueryAsync();
                    }

                    // Drop database if exists
                    var dropQuery = $@"
                    IF EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}')
                    BEGIN
                        DROP DATABASE [{dbName}];
                    END";
                    using (var command = new SqlCommand(dropQuery, connection))
                    {
                        command.CommandTimeout = 300;
                        await command.ExecuteNonQueryAsync();
                    }

                    // Lấy logical file names từ backup
                var fileListQuery = $"RESTORE FILELISTONLY FROM DISK = N'{tempFilePath}'";
                var fileList = new List<(string LogicalName, string Type)>();
                
                using (var command = new SqlCommand(fileListQuery, connection))
                {
                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        fileList.Add((
                            reader["LogicalName"].ToString(),
                            reader["Type"].ToString()
                        ));
                    }
                }

                // Tạo câu lệnh MOVE cho từng file
                var moveStatements = fileList.Select(file =>
                {
                    var extension = file.Type == "D" ? ".mdf" : ".ldf";
                    var fileName = $"{dbName}_{file.LogicalName}{extension}";
                    return $"MOVE N'{file.LogicalName}' TO N'{Path.Combine(dataPath, fileName)}'";
                });

                // Execute restore với MOVE
                var restoreQuery = $@"RESTORE DATABASE [{dbName}] 
                    FROM DISK = N'{tempFilePath}'
                    WITH STATS = 10,
                    {string.Join(",\n", moveStatements)},
                    REPLACE";

                    using (var command = new SqlCommand(restoreQuery, connection))
                    {
                        command.CommandTimeout = 3600; // 1 giờ
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new ErrorException(
                        StatusCodes.Status500InternalServerError,
                        ResponseCodeConstants.FAILED,
                        $"Lỗi trong quá trình restore: {ex.Message}");
                }
            }
        }
        finally
        {
            // Cleanup: Xóa file tạm sau khi restore xong hoặc có lỗi
            if (File.Exists(tempFilePath))
            {
                try
                {
                    File.Delete(tempFilePath);
                }
                catch
                {
                    throw;
                }
            }
        }
    }


}