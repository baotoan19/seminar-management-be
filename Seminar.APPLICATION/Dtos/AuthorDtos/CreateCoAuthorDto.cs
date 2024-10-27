using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.AuthorDtos;

public class CreateCoAuthorDto
{
    public List<CoAuthorDto> CoAuthors { get; set; }
}
