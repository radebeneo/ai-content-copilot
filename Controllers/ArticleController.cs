using Microsoft.AspNetCore.Mvc;

namespace ai_content_copilot.Controllers
{
    public class ArticleController : Controller
    {
        [HttpGet("/article")]
        public IActionResult Index()
        {
            return View("~/Views/ArticlePage.cshtml");
        }
    }
}
