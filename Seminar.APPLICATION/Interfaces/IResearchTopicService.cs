using Seminar.APPLICATION.Dtos.ResearchTopicDtos;

namespace Seminar.APPLICATION.Interfaces;

public interface IResearchTopicService
{
    Task CreateResearchTopicAsync(CreateResearchTopicDto createResearchTopicDto);
}