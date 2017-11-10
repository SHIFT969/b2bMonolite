using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using b2bSwgroup.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;

namespace b2bSwgroup.Controllers
{
    [Authorize]
    public class ShemaController : Controller
    {
        private ApplicationContext db = new ApplicationContext();
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        // GET: Shema/Edit
        public async System.Threading.Tasks.Task<ActionResult> Edit()
        {
            var currentUser = UserManager.FindByNameAsync(User.Identity.Name).Result;
            var shema = db.Shemas.Include(a=>a.ShemaMembers).FirstOrDefault(d => d.DistributorId == currentUser.OrganizationId);
            if (shema == null)
            {
                shema = new Shema(currentUser);
                shema.IgnoreColumn = -1;
                shema.ShemaMembers = new List<ShemaMember>()
                {
                    new ShemaMember("Артикул"), new ShemaMember("P/N"), new ShemaMember("Категория"),
                    new ShemaMember("Наименование"), new ShemaMember("Стоимость"), new ShemaMember("Валюта")
                };
                shema = db.Shemas.Add(shema);
                await db.SaveChangesAsync();
            }

            return View(shema);
        }

        // POST: Shema/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Edit([Bind(Include = "Id,DistributorId,Sheet,FistRow,KeyColumn,IgnoreColumn,IgnoreValue,ShemaMembers")] Shema model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                foreach (var item in model.ShemaMembers)
                    db.Entry(item).State = EntityState.Modified;

                await db.SaveChangesAsync();

                return RedirectToAction("Index", "Cabinet");
            }

            return View(model);
        }
    }
}
