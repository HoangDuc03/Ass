using Microsoft.AspNetCore.Mvc;
using PRN222_1.Models;

namespace PRN222_1.Controllers
{
    public class CategoryController : Controller
    {
        FunewsManagementContext _context = new FunewsManagementContext();
        public IActionResult Index()
        {
            var query = (from c in _context.Categories
                        select new Category
                        {
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            CategoryDesciption = c.CategoryDesciption,
                            ParentCategoryId = c.ParentCategoryId,
                            IsActive = c.IsActive,
                            ParentCategory = c.ParentCategory
                        });
            return View(query.ToList());
        }

        public IActionResult Create()
        {
            var query = _context.Categories;
            this.ViewBag.cate = query.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model, int Status)
        {
            //model.CategoryId = (short)(_context.Categories.ToList().Count() + 1);
            model.IsActive = Status == 1? true : false;
            var query = _context.Categories.FirstOrDefault(x => x.CategoryId == model.ParentCategoryId);
            model.ParentCategory = query;
            _context.Categories.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            var query = _context.Categories;
            this.ViewBag.cate = query.ToList();
            var category = _context.Categories.FirstOrDefault(x => x.CategoryId == id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,Category model, int Status)
        {
            //model.CategoryId = (short)(_context.Categories.ToList().Count() + 1);
            
            var query = _context.Categories.FirstOrDefault(x => x.CategoryId == model.ParentCategoryId);
            var entity = _context.Categories.FirstOrDefault(x => x.CategoryId == id);
            entity.IsActive = Status == 1 ? true : false;
            entity.CategoryName = model.CategoryName;
            entity.CategoryDesciption = model.CategoryDesciption;
            entity.ParentCategory = query;
            entity.ParentCategoryId = model.ParentCategoryId;
            _context.Categories.Update(entity);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, int? x)
        {
            try
            {
                var entity = _context.Categories.FirstOrDefault(x => x.CategoryId == id);
                var query = _context.NewsArticles.Where(x => x.CategoryId == id);
                if(query != null)
                {
                    this.TempData["Message"] = "Cannot delete Category because it is in use in NewArticle";
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    if (entity != null)
                        _context.Categories.Remove(entity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                
            }
            catch (Exception e)
            {
                this.ViewBag.Message = "Error";
                return View();
            }
        }
    }
}
