using System.ComponentModel.DataAnnotations;

namespace Seminar.APPLICATION.Dtos.ReviewFormDtos;

public class CUReviewFormDto
{
    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; }
    [Required(ErrorMessage = "HistoryId is required")]
    public int HistoryId {get; set;}
    [Required(ErrorMessage = "ReviewerId is required")]
    public int ReviewerId {get; set;}
    [Required(ErrorMessage = "ConcludeId is required")]
    public int ConcludeId {get; set;}
    [Required(ErrorMessage = "Date_Upload is required")]
    public DateTime Date_Upload {get; set;}
}