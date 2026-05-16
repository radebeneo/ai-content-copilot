namespace ai_content_copilot.Models;

public class AiCopilotSettings
{
    public string GeminiApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
    public int MaxOutputTokens { get; set; } = 1000;
}
