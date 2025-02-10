using Microsoft.AspNetCore.Mvc;
using PRN222_1.Models;

namespace PRN222_1.Controllers
{
    public class IdentityController : Controller
    {
        FunewsManagementContext _context = new FunewsManagementContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Login(SystemAccount model)
        {
            var account = _context.SystemAccounts.FirstOrDefault(x => x.AccountEmail == model.AccountEmail && x.AccountPassword == model.AccountPassword);
            this.ViewBag.Message = account == null ? "Invalid email or password." : "Login successful.";
            if (account != null)
            {
                HttpContext.Session.SetString("Account", account.AccountRole.ToString());
                return Redirect(Url.Action("Index", "Home"));
            }
            else
                return View("Index");
        }

		public IActionResult Logout()
		{
			var account = HttpContext.Session.GetString("Account");

            if (account != null)
            {
				HttpContext.Session.Remove("Account");
			}
            return Redirect(Url.Action("Index","Home"));
        }
	}
}
