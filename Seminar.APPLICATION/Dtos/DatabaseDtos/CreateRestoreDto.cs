using Microsoft.AspNetCore.Http;

namespace Seminar.APPLICATION.Dtos.DatabaseDtos;

public class CreateRestoreDto
{
    public IFormFile BackupFile { get; set; }
}
