using Seminar.APPLICATION.Dtos.DatabaseDtos;

namespace Seminar.APPLICATION.Interfaces;

public interface IDatabaseService
{
    // Backup operation
    Task CreateBackupAsync(CreateBackupDto createBackupDto);
    // Restore operation
    Task RestoreBackupAsync(CreateRestoreDto createRestoreDto);
}
