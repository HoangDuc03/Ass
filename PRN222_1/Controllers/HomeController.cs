using Microsoft.AspNetCore.Mvc;
using PRN222_1.Models;
using System.Diagnostics;

namespace PRN222_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        FunewsManagementContext context = new FunewsManagementContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var listdata = from i in context.NewsArticles
                           join c in context.Categories on i.CategoryId equals c.CategoryId
                           join a in context.SystemAccounts on i.CreatedById equals a.AccountId
                           where i.NewsStatus == true
                           select new NewsArticle
                           {
                               NewsArticleId = i.NewsArticleId,
                               NewsTitle = i.NewsTitle,
                               Headline = i.Headline,
                               Category = c,
                               CreatedBy = a,
                              
                           };
            return View(listdata.ToList());
        }

        public IActionResult Detail(string id)
        {
            var query =                (from i in context.NewsArticles
                                       join c in context.Categories on i.CategoryId equals c.CategoryId
                                       join a in context.SystemAccounts on i.CreatedById equals a.AccountId
                                       where i.NewsArticleId == id
                                       select new NewsArticle
                                       {
                                           NewsArticleId = i.NewsArticleId,
                                           NewsTitle = i.NewsTitle,
                                           Headline = i.Headline,
                                           NewsContent = i.NewsContent,
                                           Category = c,
                                           CreatedBy = a,
                                           Tags = i.Tags
                                       });
            if (query != null)
            {
                var x = query.ToList()[0];
                return View(x);
            }
            else { return Redirect(Url.Action("", "Home")); }
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
