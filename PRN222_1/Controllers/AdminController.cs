using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN222_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PRN222_1.Controllers
{
    public class AdminController : Controller
    {
        FunewsManagementContext context = new FunewsManagementContext();
        // GET: AdminController

        [HttpGet]
        public IActionResult Index()
        {
            var listdata = from i in context.SystemAccounts
                           select i;
            return View(listdata.ToList());
        }
        [HttpPost]
        public IActionResult Index(string AccountName, string AccountEmail, int? AccountRole)
        {
            var listdata = from i in context.SystemAccounts
                           select i;
            if (AccountName != "" && AccountName != null)
            {
                listdata = listdata.Where(x => x.AccountName.Contains(AccountName.Trim()));
            }
            if (AccountEmail != "" && AccountEmail != null)
            {
                listdata = listdata.Where(x => x.AccountEmail.Contains(AccountEmail.Trim()));
            }
            if (AccountRole != null)
            {
                listdata = listdata.Where(x => x.AccountRole == AccountRole);
            }
            return View(listdata.ToList());
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        public async Task<ActionResult> Create(SystemAccount model)
        {
            try
            {
                if(model.AccountName == null || model.AccountEmail == null || model.AccountPassword == null)
                {
                    this.ViewBag.Message = "An error occurred during creation.";
                    return View();
                }
                var query = context.SystemAccounts.FirstOrDefault(x => x.AccountEmail == model.AccountEmail.Trim());
                if(query != null)
                {
                    this.ViewBag.Message = "Email already exists";
                    return View();
                }
                model.AccountId = (short)(context.SystemAccounts.ToList().Count() + 1);
                while(true)
                {
                    var check = context.SystemAccounts.Find(model.AccountId);
                    if (check != null)
                        model.AccountId += 1;
                    else
                    {
                        break;
                    }
                }
                await context.SystemAccounts.AddAsync(model);
                await context.SaveChangesAsync();
                this.ViewBag.Message = "New account created successfully.";
                return View();
            }
            catch (Exception e)
            {
                this.ViewBag.Message = "An error occurred during creation.";
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            var profile = context.SystemAccounts.FirstOrDefault(x => x.AccountId == id);
            if (profile == null)
                return View("Index");
            else
                return View(profile);
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, SystemAccount model)
        {
            try
            {
                var query = context.SystemAccounts.FirstOrDefault(x => x.AccountEmail == model.AccountEmail.Trim());
                if (query != null && query.AccountId!= id)
                {
                    
                    this.ViewBag.Message = "Email already exists";
                    var profile = context.SystemAccounts.FirstOrDefault(x => x.AccountId == id);
                    return View(profile);
                }
                if (model.AccountName == null || model.AccountEmail == null || model.AccountPassword == null)
                {
                    this.ViewBag.Message = "Infomation is null.";
                    var profile = context.SystemAccounts.FirstOrDefault(x => x.AccountId == id);
                    return View(profile);
                }
                model.AccountId = (short)id;
                var ux = context.SystemAccounts.FirstOrDefault(x=> x.AccountId == id);
                ux.AccountName = model.AccountName;
                ux.AccountEmail = model.AccountEmail;
                ux.AccountPassword = model.AccountPassword;
                context.SystemAccounts.Update(ux);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                this.ViewBag.Message = "An error occurred during creation.";
                var profile = context.SystemAccounts.FirstOrDefault(x => x.AccountId == id);
                return View(profile);
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id,int? x)
        {
            try
            {
                var entity = context.SystemAccounts.FirstOrDefault(x => x.AccountId == id);
                if(entity != null)
                    context.SystemAccounts.Remove(entity);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return View();
            }
        }

        public ActionResult Report()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Report(DateTime? From, DateTime? To)
        {
            if(From== null || To == null)
            {
                this.ViewBag.Message = "Start and end times cannot be empty";
                return View();
            }
            if (From > To)
            {
                this.ViewBag.Message = "start time cannot be greater than end time";
                return View();
            }
            var reportData = context.NewsArticles
        .Include(n => n.Category)
        .Include(n => n.CreatedBy)
        .Where(n => n.CreatedDate >= From && n.CreatedDate <= To).OrderByDescending(x => x.CreatedDate)
        .ToList();

            ViewBag.Report = reportData;
            return View(reportData);
        }
    }
}
