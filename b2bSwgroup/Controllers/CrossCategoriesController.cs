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
using Spire.Xls;

namespace b2bSwgroup.Controllers
{
    [Authorize]
    public class CrossCategoriesController : Controller
    {
        private ApplicationContext db = new ApplicationContext();
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        // GET: CrossCategories
        public async Task<ActionResult> Index()
        {
            var currentUser = UserManager.FindByNameAsync(User.Identity.Name).Result;
            var crossCategories = db.CrossCategories.Include(c => c.Category).Include(c => c.Distributor).Where(d=>d.DistributorId==currentUser.OrganizationId);
            return View(await crossCategories.ToListAsync());
        }

        // GET: CrossCategories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CrossCategory crossCategory = await db.CrossCategories.FindAsync(id);
            if (crossCategory == null)
            {
                return HttpNotFound();
            }
            return View(crossCategory);
        }

        // GET: CrossCategories/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.DistributorId = new SelectList(db.Organizations, "Id", "Name");
            return View();
        }

        // POST: CrossCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,DistributorId,IdentifyCategory,CategoryId")] CrossCategory crossCategory)
        {
            if (ModelState.IsValid)
            {
                var currentUser = UserManager.FindByNameAsync(User.Identity.Name).Result;
                crossCategory.DistributorId = currentUser.OrganizationId;
                db.CrossCategories.Add(crossCategory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", crossCategory.CategoryId);
            ViewBag.DistributorId = new SelectList(db.Organizations, "Id", "Name", crossCategory.DistributorId);
            return View(crossCategory);
        }

        // GET: CrossCategories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CrossCategory crossCategory = await db.CrossCategories.FindAsync(id);
            if (crossCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", crossCategory.CategoryId);
            ViewBag.DistributorId = new SelectList(db.Organizations, "Id", "Name", crossCategory.DistributorId);
            return View(crossCategory);
        }

        // POST: CrossCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DistributorId,IdentifyCategory,CategoryId")] CrossCategory crossCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(crossCategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", crossCategory.CategoryId);
            ViewBag.DistributorId = new SelectList(db.Organizations, "Id", "Name", crossCategory.DistributorId);
            return View(crossCategory);
        }

        public ActionResult EditRow(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CrossCategory crossCategory = db.CrossCategories.Find(id);
            if (crossCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", crossCategory.CategoryId);
            ViewBag.DistributorId = new SelectList(db.Organizations, "Id", "Name", crossCategory.DistributorId);
            return View(crossCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRow([Bind(Include = "Id,DistributorId,IdentifyCategory,CategoryId")] CrossCategory crossCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(crossCategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", crossCategory.CategoryId);
            ViewBag.DistributorId = new SelectList(db.Organizations, "Id", "Name", crossCategory.DistributorId);
            return View(crossCategory);
        }

        // GET: CrossCategories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CrossCategory crossCategory = await db.CrossCategories.FindAsync(id);
            if (crossCategory == null)
            {
                return HttpNotFound();
            }
            return View(crossCategory);
        }

        public ActionResult Load()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadExcel(FormCollection formCollection)
        {
            var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
            List<CrossCategory> catalog = new List<CrossCategory>();

            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    Workbook book = new Workbook();

                    book.LoadFromStream(upload.InputStream);
                    Worksheet workSheet = book.Worksheets[0];
                    var rows = workSheet.Rows.ToList();
                    rows.RemoveAt(0);

                    var catsegoriesNew = rows.Select(c => c.Cells[2].Value).Distinct();

                    foreach (var catNew in catsegoriesNew)
                    {
                        var position = new CrossCategory()
                        {
                            Category = await db.Categories.FindAsync(5),
                            CategoryId = 5,

                            Distributor = currentUser.Organization as Distributor,
                            DistributorId = currentUser.OrganizationId,

                            IdentifyCategory = catNew,
                            //Id = db.CrossCategories.
                        };
                        catalog.Add(position);
                    }
                    db.CrossCategories.AddRange(catalog);
                    await db.SaveChangesAsync();
                    book.Dispose();
                    workSheet.Dispose();
                }
            }
            return RedirectToAction("Index");
        }


        // POST: CrossCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CrossCategory crossCategory = await db.CrossCategories.FindAsync(id);
            db.CrossCategories.Remove(crossCategory);
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
    }
}
