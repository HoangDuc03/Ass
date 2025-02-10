using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                model.AccountId = (short)(context.SystemAccounts.ToList().Count() + 1); 
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
                model.AccountId = (short)id;
                context.SystemAccounts.Update(model);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                this.ViewBag.Message = "An error occurred during creation.";
                return View();
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
    }
}
