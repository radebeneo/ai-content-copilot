using System.Threading.Tasks;

namespace ai_content_copilot.Services;

public interface IAiService
{
    Task<string?> GenerateAsync(string prompt);
}
