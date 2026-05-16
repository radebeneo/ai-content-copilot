namespace ai_content_copilot.Services;

public static class PromptTemplates
{

    public static string GenerateTitle(string content) => 
        $"""
           You are a CMS content assistant. Based on the content below, generate 3 concise, 
           compelling page titles. Return only the titles as a numbered list, nothing else.

           Content:
           {content}
        """;

    public static string RewriteContent(string content, string tone) => 
        $"""
             You are a CMS content editor. Rewrite the following content in a {tone} tone.
             Keep it concise, clear, and brand-safe. Return only the rewritten content.

             Content:
             {content}
         """;

    public static string Summarize(string content) => 
        $"""
           Summarize the following content in 2-3 sentences. 
           Be concise and preserve the key message.

           Content:
           {content}
        """;

    public static string GenerateMetaDescription(string content) => 
        $"""
             Write an SEO-friendly meta description (under 160 characters) for a page with 
             the following content. Return only the meta description, nothing else.

             Content:
             {content}
         """;

}