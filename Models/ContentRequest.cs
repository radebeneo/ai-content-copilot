namespace ai_content_copilot.Models;

public class ContentRequest
{
    public string Content { get; set; } = string.Empty;
    public string? Tone { get; set; }
}