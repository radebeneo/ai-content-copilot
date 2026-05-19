using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ai_content_copilot.Models;

namespace ai_content_copilot.Services;

public class OllamaService : IAiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AiCopilotSettings _settings;
    private readonly ILogger<OllamaService> _logger;

    public OllamaService(
        IHttpClientFactory httpClientFactory,
        IOptions<AiCopilotSettings> settings,
        ILogger<OllamaService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string?> GenerateAsync(string prompt)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("OllamaClient");
            var requestBody = new
            {
                model = _settings.OllamaModel,
                prompt = prompt,
                stream = false,
                options = new
                {
                    num_predict = _settings.MaxOutputTokens
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/generate", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Ollama API returned {StatusCode}: {Body}", (int)response.StatusCode, errorBody);
                return null;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            return doc.RootElement.GetProperty("response").GetString();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ollama API call failed");
            return null;
        }
    }
}
