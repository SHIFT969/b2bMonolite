using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using b2bSwgroup.Models;
using Microsoft.AspNet.Identity.Owin;

namespace b2bSwgroup.Controllers
{
    public class PositionCatalogsController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        private ApplicationContext db = new ApplicationContext();

        // GET: PositionCatalogs
        public async Task<ActionResult> Index()
        {
            var positionscatalog = db.Positionscatalog.Include(p => p.Category).Include(p => p.Currency).Include(p => p.Distributor);
            //var distrUser = db.Users.FirstOrDefault(u=>u.Id==positionscatalog.);
            return View(await positionscatalog.ToListAsync());
        }
        public async Task<ActionResult> Search(string key)
        {
            var positionscatalog = db.Positionscatalog.Include(p => p.Category).Include(p => p.Currency).Include(p => p.Distributor).Where(n=>n.Name.Contains(key));
            //var distrUser = db.Users.FirstOrDefault(u=>u.Id==positionscatalog.);
            return View(await positionscatalog.ToListAsync());
        }


        // GET: PositionCatalogs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PositionCatalog positionCatalog = await db.Positionscatalog.FindAsync(id);
            if (positionCatalog == null)
            {
                return HttpNotFound();
            }
            return View(positionCatalog);
        }

        // GET: PositionCatalogs/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "IsoCode");
            ViewBag.DistributorId = new SelectList(db.Distributors, "Id", "Name");
            return View();
        }

        // POST: PositionCatalogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,PartNumber,Name,Quantity,Price,CurrencyId,CategoryId")] PositionCatalog positionCatalog)
        {
            
            if (ModelState.IsValid)
            {
                var currentUser = UserManager.FindByNameAsync(User.Identity.Name).Result;
                positionCatalog.DistributorApplicationUserId = currentUser.Id;
                positionCatalog.DistributorId = currentUser.OrganizationId;
                db.Positionscatalog.Add(positionCatalog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", positionCatalog.CategoryId);
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "IsoCode", positionCatalog.CurrencyId);
            ViewBag.DistributorId = new SelectList(db.Organizations, "Id", "Name", positionCatalog.DistributorId);
            return View(positionCatalog);
        }

        // GET: PositionCatalogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PositionCatalog positionCatalog = await db.Positionscatalog.FindAsync(id);
            if (positionCatalog == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", positionCatalog.CategoryId);
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "IsoCode", positionCatalog.CurrencyId);
            ViewBag.DistributorId = new SelectList(db.Organizations, "Id", "Name", positionCatalog.DistributorId);
            return View(positionCatalog);
        }

        // POST: PositionCatalogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,PartNumber,Name,Price,CurrencyId,Quantity,CategoryId,DistributorId,DistributorApplicationUserId")] PositionCatalog positionCatalog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(positionCatalog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", positionCatalog.CategoryId);
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "IsoCode", positionCatalog.CurrencyId);
            ViewBag.DistributorId = new SelectList(db.Organizations, "Id", "Name", positionCatalog.DistributorId);
            return View(positionCatalog);
        }

        // GET: PositionCatalogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PositionCatalog positionCatalog = await db.Positionscatalog.FindAsync(id);
            if (positionCatalog == null)
            {
                return HttpNotFound();
            }
            return View(positionCatalog);
        }

        // POST: PositionCatalogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PositionCatalog positionCatalog = await db.Positionscatalog.FindAsync(id);
            db.Positionscatalog.Remove(positionCatalog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<ActionResult> MyPositions()
        {
            ApplicationUser thisUser = await UserManager.FindByNameAsync(User.Identity.Name);
            
            var myCompany = await db.Organizations.Include(o => o.ApplicationUsers).FirstOrDefaultAsync(o => o.Id == thisUser.OrganizationId);
            var positionscatalog = await db.Positionscatalog.Include(p => p.Category).Include(p => p.Currency).Include(p => p.Distributor).Include(u=>u.DistributorApplicationUser).Where(a => a.DistributorId == myCompany.Id).ToListAsync();
            return View(positionscatalog);
        }
    }
}
