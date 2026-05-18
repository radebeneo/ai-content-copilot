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
        int[] retryDelaysMs = [1000, 3000, 7000];

        for (int attempt = 0; attempt <= retryDelaysMs.Length; attempt++)
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
                       new { parts = new[] { new { text = prompt } } }
                   },
                   generationConfig = new { maxOutputTokens = _settings.MaxOutputTokens }
               };
   
               var json = JsonSerializer.Serialize(requestBody);
               var content = new StringContent(json, Encoding.UTF8, "application/json");
               var response = await client.PostAsync(url, content);
               
               if ((int)response.StatusCode is 503 or 429 or 500 && attempt < retryDelaysMs.Length)
               {
                   var errorBody = await response.Content.ReadAsStringAsync();
                   _logger.LogWarning(
                       "Gemini attempt {Attempt} returned {Status}, retrying in {Delay}ms: {Body}",
                       attempt + 1, (int)response.StatusCode, retryDelaysMs[attempt], errorBody);

                   await Task.Delay(retryDelaysMs[attempt]);
                   continue;
               }
   
               if (!response.IsSuccessStatusCode)
               {
                   var errorBody = await response.Content.ReadAsStringAsync();
                   _logger.LogError(
                       "Gemini API  returned {StatusCode}: {Body}",
                       (int)response.StatusCode, errorBody);
                   return null;
               }
               
               var responseJson = await response.Content.ReadAsStringAsync();
               using var doc = JsonDocument.Parse(responseJson);
               
               
               var candidates = doc.RootElement.GetProperty("candidates");
               if (candidates.GetArrayLength() == 0)
               {
                   _logger.LogWarning("Gemini returned empty candidates array");
                   return null;
               }
               
               var text = candidates[0]
                   .GetProperty("content")
                   .GetProperty("parts")[0]
                   .GetProperty("text")
                   .GetString();
           }
           catch (Exception e)
           {
               _logger.LogError(e, "Gemini API call failed on attempt {Attempt}", attempt + 1);
               
               if (attempt < retryDelaysMs.Length)
                   await Task.Delay(retryDelaysMs[attempt]);
           } 
        }
        return null;
    }
}