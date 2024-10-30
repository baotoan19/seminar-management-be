using Seminar.APPLICATION.Dtos.ResearchTopicDtos;
using Seminar.APPLICATION.Models;

namespace Seminar.APPLICATION.Interfaces;

public interface IResearchTopicService
{
    Task CreateResearchTopicAsync(CreateResearchTopicDto createResearchTopicDto);
    Task<ResearchTopicVM> GetResearchTopicByIdAsync(int id);
    Task<List<ResearchTopicAuthorVM>> GetAuthorByResearchTopicIdAsync(int id);

}