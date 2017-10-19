using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using b2bSwgroup.Models;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using Spire.Xls;
using System.IO;
using AutoMapper;
using b2bSwgroup.Models.ModelsForView;

namespace b2bSwgroup.Controllers
{
    [Authorize]
    public class CabinetController : Controller
    {
        private ApplicationContext db = new ApplicationContext();
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        // GET: Cabinet
        public async Task<ActionResult> Index()
        {
            ApplicationUser thisUser = await UserManager.FindByNameAsync(User.Identity.Name);
            return View(thisUser);
        }

        public async Task<ActionResult> ManagementOrganization()
        {
            ApplicationUser thisUser = await UserManager.FindByNameAsync(User.Identity.Name);
            if (thisUser.OrganizationId != null)
            {
                return RedirectToAction("Edit","Organization");
            }
            else
            {
                return RedirectToAction("Create","Organization");
            }
        }
    }
}