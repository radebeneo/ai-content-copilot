using Microsoft.AspNetCore.Mvc;
using MyProject.Models;
using MyProject.Services;

namespace ai_content_copilot.Controllers;

[ApiController]
[Route("api/ai-copilot")]
public class AiCopilotController : Controller 
{
    private readonly GeminiService _geminiService;
    
    public AiCopilotController(GeminiService geminiService)
    {
        _geminiService = geminiService;
    }
    
    [HttpPost("generate-title")]
        public async Task<IActionResult> GenerateTitle([FromBody] ContentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Content is required." });
    
            var prompt = PromptTemplates.GenerateTitle(request.Content);
            var result = await _geminiService.GenerateAsync(prompt);
    
            return result is null
                ? StatusCode(500, new { error = "Generation failed." })
                : Ok(new { result });
        }
        
    [HttpPost("rewrite")]
        public async Task<IActionResult> Rewrite([FromBody] ContentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Content is required." });
    
            var tone = string.IsNullOrWhiteSpace(request.Tone) ? "professional" : request.Tone;
            var prompt = PromptTemplates.RewriteContent(request.Content, tone);
            var result = await _geminiService.GenerateAsync(prompt);
    
            return result is null
                ? StatusCode(500, new { error = "Generation failed." })
                : Ok(new { result });
        }
        
    [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] ContentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Content is required." });
    
            var prompt = PromptTemplates.Summarize(request.Content);
            var result = await _geminiService.GenerateAsync(prompt);
    
            return result is null
                ? StatusCode(500, new { error = "Generation failed." })
                : Ok(new { result });
        }
        
    [HttpPost("meta-description")]
        public async Task<IActionResult> MetaDescription([FromBody] ContentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Content is required." });
    
            var prompt = PromptTemplates.GenerateMetaDescription(request.Content);
            var result = await _geminiService.GenerateAsync(prompt);
    
            return result is null
                ? StatusCode(500, new { error = "Generation failed." })
                : Ok(new { result });
        }

}