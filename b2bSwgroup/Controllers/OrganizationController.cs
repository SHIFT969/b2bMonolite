using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using b2bSwgroup.Models;
using System.Threading.Tasks;
using System.Data.Entity;


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
                if(thisUser is CustomerApplUser)
                {
                    db.Organizations.Add(new Customer() { INN = organization.INN, Name = organization.Name });                    
                };
                if (thisUser is DistributorApplicationUser)
                {
                    db.Organizations.Add(new Distributor() { INN = organization.INN, Name = organization.Name });
                };
                var user = await db.Users.FirstOrDefaultAsync(i=>i.Id==thisUser.Id);
                user.Organization = organization;                
                await db.SaveChangesAsync();
                return RedirectToAction("Index","Cabinet");
            }

            return View(organization);
        }

    }
}