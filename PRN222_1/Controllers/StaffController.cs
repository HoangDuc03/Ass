using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN222_1.Models;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
namespace PRN222_1.Controllers
{
    public class StaffController : Controller
    {
        FunewsManagementContext context = new FunewsManagementContext();
        // GET: StaffController

        [HttpGet]
        public IActionResult Index()
        {
            var listdata = from i in context.NewsArticles
                           join c in context.Categories on i.CategoryId equals c.CategoryId
                           join a in context.SystemAccounts on i.CreatedById equals a.AccountId
                           select new NewsArticle
                           {
                               NewsArticleId  = i.NewsArticleId,
                               NewsTitle = i.NewsTitle,
                               Headline = i.Headline,
                               NewsContent = i.NewsContent,
                               NewsSource = i.NewsSource,
                               NewsStatus = i.NewsStatus,
                               CategoryId = i.CategoryId,
                               Category = c,
                               CreatedById = i.CreatedById,
                               CreatedDate = i.CreatedDate,
                               UpdatedById = i.UpdatedById,
                               ModifiedDate = i.ModifiedDate,
                               CreatedBy = a,
                               Tags = i.Tags

                           };
            return View(listdata.ToList());
        }

        // GET: StaffController/Create
        public ActionResult Create()
        {
            this.ViewBag.Category = context.Categories;
            this.ViewBag.Account = context.SystemAccounts;
            this.ViewBag.Tags = context.Tags.ToList();
            return View();
        }

        // POST: StaffController/Create
        [HttpPost]
        public async Task<ActionResult> Create(NewsArticle model, List<int> Tags, int Status)
        {
            try
            {
                model.NewsArticleId = (context.NewsArticles.ToList().Count() + 1).ToString();
                model.CreatedDate = DateTime.Now;
                model.NewsStatus = Status == 1 ? true : false;
                foreach (var i in Tags)
                {
                    var query = context.Tags.FirstOrDefault(x => x.TagId == i);
                    model.Tags.Add(query);
                }
                await context.NewsArticles.AddAsync(model);
                await context.SaveChangesAsync();
                this.ViewBag.Message = "New NewsArticle created successfully.";
                this.ViewBag.Category = context.Categories;
                this.ViewBag.Account = context.SystemAccounts;
                this.ViewBag.Tags = context.Tags.ToList();
                return View();
            }
            catch (Exception e)
            {
                this.ViewBag.Message = "An error occurred during creation.";
                this.ViewBag.Category = context.Categories;
                this.ViewBag.Account = context.SystemAccounts;
                this.ViewBag.Tags = context.Tags.ToList();
                return View();
            }
        }

        // GET: StaffController/Edit/5
        public ActionResult Edit(string id)
        {
            var profile =  (from i in context.NewsArticles
                           where i.NewsArticleId.Equals(id)
                           select new NewsArticle
                           {
                                             NewsArticleId = i.NewsArticleId,
                                             NewsTitle = i.NewsTitle,
                                             Headline = i.Headline,
                                             NewsContent = i.NewsContent,
                                             NewsSource = i.NewsSource,
                                             NewsStatus = i.NewsStatus,
                                             CategoryId = i.CategoryId,
                                             CreatedById = i.CreatedById,
                                             CreatedDate = i.CreatedDate,
                                             UpdatedById = i.UpdatedById,
                                             ModifiedDate = i.ModifiedDate,
                                             Tags = i.Tags
                           }).ToList()[0];
            if (profile == null)
                return View("Index");
            else
            {
                this.ViewBag.Category = context.Categories;
                this.ViewBag.Account = context.SystemAccounts;
                this.ViewBag.Tags = context.Tags.ToList();
                return View(profile);
            }
        }

        // POST: StaffController/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, NewsArticle model, List<int> Tags, int Status)
        {
            try
            {
                var article = context.NewsArticles.Include(n => n.Tags) // Load danh sách Tag liên quan
                                .FirstOrDefault(n => n.NewsArticleId == id);
                article.NewsTitle = model.NewsTitle;
                article.NewsContent = model.NewsContent;
                article.Headline = model.Headline;
                article.NewsSource = model.NewsSource;
                article.NewsStatus = Status == 1 ? true : false;
                article.CategoryId = model.CategoryId;
                article.UpdatedById = model.UpdatedById;
                article.ModifiedDate = DateTime.Now;
                article.UpdatedById = short.Parse(HttpContext.Session.GetString("Account"));
                article.Tags.Clear();


                foreach (var tagId in Tags)
                {
                        var query = context.Tags.Find(tagId);
                        article.Tags.Add(query);
                }
                
                context.NewsArticles.Update(article);
                await context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                this.ViewBag.Message = "An error occurred during creation.";
                this.ViewBag.Category = context.Categories;
                this.ViewBag.Account = context.SystemAccounts;
                this.ViewBag.Tags = context.Tags.ToList();
                return View();
            }
        }

        // GET: StaffController/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: StaffController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, int? x)
        {
            try
            {
                var article = context.NewsArticles.Include(n => n.Tags) // Load danh sách Tag liên quan
                                .FirstOrDefault(n => n.NewsArticleId == id);

                if (article != null)
                {
                    article.Tags.Clear(); // Xóa tất cả quan hệ với Tag trước khi xóa bài viết
                    context.NewsArticles.Remove(article);
                    context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}
