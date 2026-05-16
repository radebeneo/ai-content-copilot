using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ai_content_copilot.Models;

namespace ai_content_copilot.Services;

public class GeminiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AiCopilotSettings _settings;
    private readonly ILogger<GeminiService> _logger;
    
    public GeminiService(
        IHttpClientFactory httpClientFactory,
        IOptions<AiCopilotSettings> settings,
        ILogger<GeminiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string?> GenerateAsync(string prompt)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("GeminiClient");
            string model = _settings.Model;
            var url = $"v1beta/models/{model}:generateContent";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    maxOutputTokens = _settings.MaxOutputTokens
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse((string)responseJson, new JsonDocumentOptions());
            
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Gemini API call failed");
            return null;
        }
    }
}