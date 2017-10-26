using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using b2bSwgroup.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;


namespace b2bSwgroup.Controllers
{
    [Authorize]
    public class OrganizationController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        private ApplicationContext db = new ApplicationContext();
        // GET: Organization
        public async Task<ActionResult> Edit()
        {
            ApplicationUser thisUser = await UserManager.FindByNameAsync(User.Identity.Name);
            Organization organization =  await db.Organizations.FirstOrDefaultAsync(i => i.Id == thisUser.OrganizationId);
            return View(organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Organization organization)
        {
            if (ModelState.IsValid)
            {
                db.Entry(organization).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Cabinet");
            }
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Organization organization)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser thisUser = await UserManager.FindByNameAsync(User.Identity.Name);
                var user = await db.Users.FirstOrDefaultAsync(i => i.Id == thisUser.Id);
                if (thisUser is CustomerApplUser)
                {
                    var org = new Customer() { INN = organization.INN, Name = organization.Name };
                    db.Customers.Add(org);
                    user.Organization = org;
                };
                if (thisUser is DistributorApplicationUser)
                {
                    var org = new Distributor() { INN = organization.INN, Name = organization.Name };
                    db.Distributors.Add(org);
                    user.Organization = org;
                };         
                               
                await db.SaveChangesAsync();
                return RedirectToAction("Index","Cabinet");
            }

            return View(organization);
        }
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Organizations.ToListAsync());
        }
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = await db.Organizations.FindAsync(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            db.Organizations.Remove(organization);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}