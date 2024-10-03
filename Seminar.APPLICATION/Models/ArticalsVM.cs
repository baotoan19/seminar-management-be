using System.ComponentModel.DataAnnotations.Schema;

namespace Seminar.APPLICATION.Models;

public class ArticalVM
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string Keyword { get; set; }
    public string FilePath { get; set; }
    public DateTime DateUpload { get; set; }
    public int DisciplineId { get; set; }
    public string DisciplineName { get; set; }
    public int ProceedingId { get; set; }
    public string ProceedingTitle { get; set; }
    public int ConferenceId { get; set; }
    public string ConferenceName { get; set; }
}