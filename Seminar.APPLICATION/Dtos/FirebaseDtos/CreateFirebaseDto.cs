using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Seminar.APPLICATION.Dtos.FirebaseDtos;

public class CreateFirebaseDto
{
    [Required(ErrorMessage = "File is required")]
    public IFormFile File { get; set; }
    [Required(ErrorMessage = "Folder name is required")]
    public string FolderName { get; set; }
}
