using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Seminar.APPLICATION.Dtos.GoogleAiDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;

namespace Seminar.APPLICATION.Services;

public class GoogleAIService : IGoogleAIService
{
    private readonly HttpClient _client;
    private readonly string _apiKey;
    private readonly ILogger<GoogleAIService> _logger;

    public GoogleAIService(IConfiguration config, ILogger<GoogleAIService> logger)
    {
        _apiKey = config["AI:Google:ApiKey"] ?? throw new ArgumentNullException("Google AI API Key is missing");
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/")
        };
        _logger = logger;
    }

    public async Task<SummaryResultDto> SummarizeTextAsync(string text, int maxLength = 150, string language = "vi")
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var request = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = $"Summarize the following text in {language} language in {maxLength} words or less:\n\n{text}"
                            }
                        }
                    }
                }
            };

            var response = await _client.PostAsJsonAsync(
                $"models/gemini-pro:generateContent?key={_apiKey}",
                request
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Google AI API Error: {error}");
                throw new Exception($"Google AI API Error: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<GoogleAIResponse>();
            var summary = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "";

            stopwatch.Stop();

            return new SummaryResultDto
            {
                Summary = summary,
                SummaryLength = summary.Split().Length,
                ProcessingTime = stopwatch.Elapsed.TotalSeconds,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error summarizing text");
            throw;
        }
    }

}