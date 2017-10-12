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
using PagedList.Mvc;
using PagedList;

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
        public async Task<ActionResult> Index(int? page)
        {
            var positionscatalog = await db.Positionscatalog.Include(p => p.Category).Include(p => p.Currency).Include(p => p.Distributor).ToListAsync();
            //var distrUser = db.Users.FirstOrDefault(u=>u.Id==positionscatalog.);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(positionscatalog.ToPagedList(pageNumber,pageSize));
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
        public ActionResult UploadExcel()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UploadExcel(FormCollection formCollection)
        {

            //var currencyExcelList = new Dictionary<string, string>();
            //currencyExcelList.Add("RUB", "6717f5b5-50f1-e411-80f2-c4346bad2214");
            //currencyExcelList.Add("USD", "ee9e8e27-5e42-e511-8114-c4346bacce50");
            //currencyExcelList.Add("KZT", "db8bbc3b-bd49-e611-80d6-24b6fdf8545c");
            //currencyExcelList.Add("EUR", "4be2906f-6509-e711-80dd-24b6fdf8545c");
            //currencyExcelList.Add("", "be604f9c-4319-e711-80dd-24b6fdf8545c");
            var currentUser = UserManager.FindByNameAsync(User.Identity.Name).Result;
            var oldPositionsCatalog = await db.Positionscatalog.Where(p=>p.DistributorId==currentUser.OrganizationId).ToListAsync();
            foreach(var pos in oldPositionsCatalog)
            {
                db.Positionscatalog.Remove(pos);
            }
            List<CrossCategory> myCategories = await db.CrossCategories.Where(c => c.DistributorId == currentUser.OrganizationId).ToListAsync();
            List<PositionCatalog> catalog = new List<PositionCatalog>();
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    Workbook book = new Workbook();
                    book.LoadFromStream(upload.InputStream);
                    Worksheet workSheet = book.Worksheets[0];

                    for (int i = 1; i < workSheet.Rows.Length; i++)
                    {
                        int? idCat = null;
                        if(myCategories.FirstOrDefault(c => c.IdentifyCategory == workSheet.Rows[i].Cells[1].Value)!=null)
                        {
                            idCat = myCategories.FirstOrDefault(c => c.IdentifyCategory == workSheet.Rows[i].Cells[1].Value).CategoryId;
                        }
                        
                        
                        var position = new PositionCatalog()
                        {
                            PartNumber = workSheet.Rows[i].Cells[0].Value,
                            CategoryId = idCat, 
                            Name = workSheet.Rows[i].Cells[2].Value,
                            Price = double.Parse(workSheet.Rows[i].Cells[3].Value.Replace(".",",")),
                            DistributorApplicationUserId = currentUser.Id,
                            DistributorId = currentUser.OrganizationId
                        };
                        catalog.Add(position);
                    }
                    db.Positionscatalog.AddRange(catalog);
                    await db.SaveChangesAsync();                    
                    book.Dispose();
                    workSheet.Dispose();
                }
            }
            return RedirectToAction("MyPositions");
        }
    }
}
