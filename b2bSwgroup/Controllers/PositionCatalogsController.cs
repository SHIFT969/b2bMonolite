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
using b2bSwgroup.Models.Services;
using Microsoft.AspNet.Identity.Owin;
using Spire.Xls;
using System.IO;
using AutoMapper;
using b2bSwgroup.Models.ModelsForView;

namespace b2bSwgroup.Controllers
{
    //[Authorize]
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

        private static string setPrice(double price, Currency currency)
        {
            string result="";
            if (currency != null && currency.СultureInfo != "" && currency.СultureInfo != null)
            {
                IFormatProvider formatProvider = new System.Globalization.CultureInfo(currency.СultureInfo);
                result=price.ToString("C", formatProvider);
            }
            else
            {
                result =  String.Format("{0:##,###.00}", price);
            }

            return result;
        }

        // GET: PositionCatalogsj
        public async Task<ActionResult> Index(int page=1,string key = "")
        {
            IEnumerable<PositionCatalogIndexView> positionTest = new List<PositionCatalogIndexView>();
            if (!Directory.Exists(SearchPositionCatalogService._luceneDir)) Directory.CreateDirectory(SearchPositionCatalogService._luceneDir);
            if (!System.IO.Directory.EnumerateFiles(SearchPositionCatalogService._luceneDir).Any())
            {
                Mapper.Initialize(cfg => cfg.CreateMap<PositionCatalog, PositionCatalogIndexView>().ForMember("Distributor", opt=>opt.MapFrom(c=>c.Distributor.Name))
                .ForMember("Category",opt=>opt.MapFrom(c=>c.Category.Name))
                .ForMember("Price",opt=>opt.MapFrom(c=>setPrice(c.Price,c.Currency))));

                SearchPositionCatalogService.AddUpdateLuceneIndex(Mapper.Map<IEnumerable<PositionCatalog>,IEnumerable<PositionCatalogIndexView>>(await db.Positionscatalog
                    .Include(d=>d.Distributor)
                    .Include(c=>c.Category)
                    .Include(c=>c.Currency)
                    .ToListAsync()));
            }
            if (string.IsNullOrEmpty(key))
            {
                positionTest = SearchPositionCatalogService.GetAllIndexRecords();
            }
            else
            {
                positionTest = SearchPositionCatalogService.SearchDefault(key);
            }
            int pageSize = 10;
            //var positionscatalog = await db.Positionscatalog.Where(x=>x.Name.Contains(key) || x.Articul.Contains(key) || x.PartNumber.Contains(key) || x.Distributor.Name.Contains(key))
            //    .OrderBy(x=>x.Name)
            //    .Skip((page-1)*pageSize)
            //    .Take(pageSize)
            //    .Include(p => p.Category).Include(p => p.Currency).Include(p => p.Distributor).ToListAsync();
            //PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItem = db.Positionscatalog.Where(x => x.Name.Contains(key) || x.Articul.Contains(key) || x.PartNumber.Contains(key) || x.Distributor.Name.Contains(key)).Count() };
            //IndexViewModel ivm = new IndexViewModel { PageInfo = pageInfo, Positions = positionscatalog,KeyWord=key };


            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItem = positionTest.Count() };
            IndexViewModel ivm = new IndexViewModel { PageInfo = pageInfo, Positions = positionTest.Skip((page - 1) * pageSize).Take(pageSize), KeyWord = key };
            
            return View(ivm);
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
                try
                {
                    await db.SaveChangesAsync();
                    Mapper.Initialize(cfg => cfg.CreateMap<PositionCatalog, PositionCatalogIndexView>().ForMember("Distributor", opt => opt.MapFrom(c => c.Distributor.Name))
                      .ForMember("Category", opt => opt.MapFrom(c => c.Category.Name))
                      .ForMember("Price", opt => opt.MapFrom(c => setPrice(c.Price, c.Currency))));
                    SearchPositionCatalogService.AddUpdateLuceneIndex(Mapper.Map<PositionCatalog, PositionCatalogIndexView>(positionCatalog));
                }
                catch
                {

                }
                
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,Articul,PartNumber,Name,Price,CurrencyId,Quantity,CategoryId,DistributorId,DistributorApplicationUserId")] PositionCatalog positionCatalog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(positionCatalog).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    Mapper.Initialize(cfg => cfg.CreateMap<PositionCatalog, PositionCatalogIndexView>().ForMember("Distributor", opt => opt.MapFrom(c => c.Distributor.Name))
                      .ForMember("Category", opt => opt.MapFrom(c => c.Category.Name))
                      .ForMember("Price", opt => opt.MapFrom(c => setPrice(c.Price, c.Currency))));
                    SearchPositionCatalogService.AddUpdateLuceneIndex(Mapper.Map<PositionCatalog, PositionCatalogIndexView>(positionCatalog));
                }
                catch
                {

                }
                
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
            try
            {
                await db.SaveChangesAsync();
                SearchPositionCatalogService.ClearLuceneIndexRecord(id);
            }
            catch
            {

            }
            
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

        public async Task<ActionResult> RemoveAllCatalog()
        {
            var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
            var oldPositionsCatalog = await db.Positionscatalog.Where(p => p.DistributorId == currentUser.OrganizationId).ToListAsync();
            db.Positionscatalog.RemoveRange(oldPositionsCatalog);
            try
            {
                await db.SaveChangesAsync();
                foreach(var pos in oldPositionsCatalog)
                {
                    SearchPositionCatalogService.ClearLuceneIndexRecord(pos.Id);
                }                
            }
            catch
            {

            }
            
            return RedirectToAction("MyPositions");
        }

        [Authorize(Roles ="Distributor")]
        public async Task<ActionResult> MyPositions(int page=1)
        {
            ApplicationUser thisUser = await UserManager.FindByNameAsync(User.Identity.Name);
            var myCompany = await db.Organizations.Include(o => o.ApplicationUsers).FirstOrDefaultAsync(o => o.Id == thisUser.OrganizationId);
            int pageSize = 10;
            var positionscatalog = await db.Positionscatalog.Where(a => a.DistributorId == myCompany.Id).OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize).Include(p => p.Category).Include(p => p.Currency).Include(p => p.Distributor).ToListAsync();
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItem = db.Positionscatalog.Where(a => a.DistributorId == myCompany.Id).Count() };
            MyPositionsViewModel ivm = new MyPositionsViewModel { PageInfo = pageInfo, Positions = positionscatalog };           
            return View(ivm);
        }
        [Authorize(Roles = "Distributor")]
        public ActionResult UploadExcel()
        {
            return View();
        }
        [Authorize(Roles = "Distributor")]
        [HttpPost]
        public async Task<ActionResult> UploadExcel(FormCollection formCollection)
        {
            var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
            var oldPositionsCatalog = await db.Positionscatalog.Where(p=>p.DistributorId==currentUser.OrganizationId).ToListAsync();
            db.Positionscatalog.RemoveRange(oldPositionsCatalog);
                      
            List<CrossCategory> myCategories = await db.CrossCategories.Where(c => c.DistributorId == currentUser.OrganizationId).ToListAsync();
            List<PositionCatalog> catalog = new List<PositionCatalog>();
            int countPosition = 0;
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
                    foreach(var row in rows)
                    {
                        var cellArt = row.Cells[0].Value;
                        var cellPart = row.Cells[1].Value;
                        var cellCat = row.Cells[2].Value;
                        var cellName = row.Cells[3].Value;
                        var cellPrice = row.Cells[4].Value;
                        var cellCur = row.Cells[5].Value;

                        int? idCat = null;
                        if(myCategories.FirstOrDefault(c => c.IdentifyCategory == cellCat)!=null)
                        {
                            idCat = myCategories.FirstOrDefault(c => c.IdentifyCategory == cellCat).CategoryId;
                        }
                        var myIsoCode = cellCur;
                        double myPrice;
                        try
                        {
                            myPrice = double.Parse(cellPrice.Replace(".", ","));
                        }
                        catch
                        {
                            myPrice = 0;
                        }
                        var position = new PositionCatalog()
                        {
                            Articul = cellArt,
                            PartNumber = cellPart,
                            CategoryId = idCat,
                            Name = cellName,
                            Price = myPrice,
                            Currency = db.Currencies.FirstOrDefault(j => j.IsoCode == myIsoCode),

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
