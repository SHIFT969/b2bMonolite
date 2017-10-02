using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using b2bSwgroup.Models;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace b2bSwgroup.Controllers
{
   // [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private ApplicationContext db = new ApplicationContext();
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }
        // GET: Admin
        public ActionResult Index()
        {
            ///Lis
            List<UserAdmin> usersAdmin = new List<UserAdmin>();
            var users = UserManager.Users.ToList();
            using (ApplicationContext context = new ApplicationContext())
            {
                foreach(var user in users)
                {
                    var userAdmin = new UserAdmin() { User=user };
                    foreach(var role in user.Roles)
                    {
                        var test = RoleManager.FindByIdAsync(role.RoleId).Result;
                        userAdmin.Roles.Add(test);
                    }
                    usersAdmin.Add(userAdmin);
                }
            }                
            return View(usersAdmin);
        }
        [HttpGet]
        public async Task<ActionResult> Edit(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var userAdmin = new UserAdmin() { User = user };
            foreach (var role in user.Roles)
            {
                var test = RoleManager.FindByIdAsync(role.RoleId).Result;
                userAdmin.Roles.Add(test);                
            }
            ViewBag.AllRoles = RoleManager.Roles;
            var a= user is DistributorApplicationUser ? 0 : 1; //user is DistributorApplicationUser ? new SelectList(db.Distributors, "Id", "Name") : new SelectList(db.Customers, "Id", "Name");
            return View(userAdmin);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(EditUserView model)
        {
            var newroles = model.RoleId.Where(r => r != "false");
            var oldroles = UserManager.FindByIdAsync(model.User.Id).Result.Roles.Select(s => s.RoleId);
            var rolesForAdd = newroles.Except<string>(oldroles);
            var rolesForRemove = oldroles.Except<string>(newroles);
            List<string> rolesNameForAdd = new List<string>();
            List<string> rolesNameForRemove = new List<string>();
            foreach (var roleId in rolesForAdd)
            {
                rolesNameForAdd.Add(RoleManager.Roles.Where(a=>a.Id==roleId).Select(n=>n.Name).FirstOrDefault());
            }
            foreach (var roleId in rolesForRemove)
            {
                rolesNameForRemove.Add(RoleManager.Roles.Where(a => a.Id == roleId).Select(n => n.Name).FirstOrDefault());
            }
            await UserManager.AddToRolesAsync(model.User.Id, rolesNameForAdd.ToArray());            
            await UserManager.RemoveFromRolesAsync(model.User.Id,rolesNameForRemove.ToArray());
            return RedirectToAction("Index");
        }
    }
}