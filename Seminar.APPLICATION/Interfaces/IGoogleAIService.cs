using Microsoft.AspNetCore.Http;
using Seminar.APPLICATION.Dtos.GoogleAiDtos;

namespace Seminar.APPLICATION.Interfaces;

public interface IGoogleAIService
{
    Task<SummaryResultDto> SummarizeTextAsync(string text, int maxLength = 200, string language = "vi");
}
