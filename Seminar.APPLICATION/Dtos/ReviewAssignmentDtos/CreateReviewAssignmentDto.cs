namespace Seminar.APPLICATION.Dtos.ReviewAssignmentDtos;

public class CreateReviewAssignmentDto
{
    public int ResearchTopicId { get; set;}
    public List<ListReviewerDto> ListReviewerDtos { get; set;}
    public DateTime DateStart { get; set;}
    public DateTime DateEnd { get; set;}
}

public class ListReviewerDto
{
    public List<int> ReviewerIds { get; set;}
}