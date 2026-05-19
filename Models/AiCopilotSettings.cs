namespace ai_content_copilot.Models;

public class AiCopilotSettings
{
    public int MaxOutputTokens { get; set; } = 1000;
    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";
    public string OllamaModel { get; set; } = "llama3";
}
