using Seminar.DOMAIN.Enum;

namespace Seminar.APPLICATION.Dtos.DatabaseDtos;

public class CreateBackupDto
{
    public BackupType BackupType { get; set; }
    public string BackupPath { get; set; }
    public string Description { get; set; }
}
