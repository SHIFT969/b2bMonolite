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
using PagedList.Mvc;
using PagedList;
using System.Text.RegularExpressions;

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
            Shema shema = db.Shemas.Include(a => a.ShemaMembers).FirstOrDefault(d => d.DistributorId == currentUser.OrganizationId);

            if (shema != null && shema.Sheet != -1)
                await ReadFileWithSpire(catalog, shema, currentUser);
            else
            {
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
                        foreach (var row in rows)
                        {
                            var cellArt = row.Cells[0].Value;
                            var cellPart = row.Cells[1].Value;
                            var cellCat = row.Cells[2].Value;
                            var cellName = row.Cells[3].Value;
                            var cellPrice = row.Cells[4].Value;
                            var cellCur = row.Cells[5].Value;

                            int? idCat = null;
                            if (myCategories.FirstOrDefault(c => c.IdentifyCategory == cellCat) != null)
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
                        book.Dispose();
                        workSheet.Dispose();
                    }
                }
            }
            db.Positionscatalog.AddRange(catalog);
            await db.SaveChangesAsync();
            return RedirectToAction("MyPositions");
        }

        private Dictionary<int, string> parentsByLevel;
        private Dictionary<int, int> LevelsToKeep;

        private async Task ReadFileWithSpire(List<PositionCatalog> li, Shema shema, ApplicationUser currentUser)
        {
            List<CrossCategory> myCategories = await db.CrossCategories.Where(c => c.DistributorId == currentUser.OrganizationId).ToListAsync();
            LevelsToKeep = new Dictionary<int, int>();
            parentsByLevel = new Dictionary<int, string>();

            foreach (var item in shema.ShemaMembers)
            {
                if (item.ParentLevel != -1)
                {
                    var lv = new KeyValuePair<int, int>(item.ParentLevel, item.Column);
                    for (int i = 0; i <= item.ParentLevel - 1; i++)
                        if (!LevelsToKeep.ContainsKey(item.ParentLevel))
                            LevelsToKeep.Add(i, item.Column);
                }
            }

            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    Workbook workbook = new Workbook();
                    workbook.LoadFromStream(upload.InputStream);
                    Worksheet sheet = workbook.Worksheets[shema.Sheet - 1];
                    var rows = sheet.Rows;

                    var smArticul = shema.ShemaMembers.First(s => s.Name == "Артикул");
                    var smPartNumber = shema.ShemaMembers.First(s => s.Name == "P/N");
                    var smCategory = shema.ShemaMembers.First(s => s.Name == "Категория");
                    var smName = shema.ShemaMembers.First(s => s.Name == "Наименование");
                    var smPrice = shema.ShemaMembers.First(s => s.Name == "Стоимость");
                    var smCurrency = shema.ShemaMembers.First(s => s.Name == "Валюта");

                    for (int i = 0; i < rows.Length; i++)
                    {
                        var row = rows[i].Cells; 

                        if (i >= shema.FistRow - 1) // если пошла таблица
                        {
                            CheckLevels(shema, row); // проверит, нужно ли перезаписать уровни
                            if (CheckForIgnore(shema, row))
                                continue;

                            if (row.Length >= shema.KeyColumn - 1 // если ключевая колонка по индексу в массиве
                                && !string.IsNullOrWhiteSpace(row[shema.KeyColumn - 1].Value)) // и ключевая колонка не пустая
                            {
                                int? idCat = null;
                                string cellCat = GetCellValue(row, smCategory);
                                var ca = myCategories.FirstOrDefault(c => c.IdentifyCategory == cellCat);
                                if (ca != null)
                                    idCat = ca.CategoryId;

                                var myIsoCode = GetCellValue(row, smCurrency);
                                double myPrice;
                                string cellPrice = GetCellValue(row, smPrice, true);
                                if (!double.TryParse(cellPrice.Replace(".", ","), out myPrice))
                                    myPrice = 0;

                                var item = new PositionCatalog(); // новая строка
                                item.CategoryId = idCat;
                                item.Articul = GetCellValue(row, smArticul);
                                item.PartNumber = GetCellValue(row, smPartNumber);
                                item.Name = GetCellValue(row, smName);
                                item.Price = myPrice;
                                item.Currency = db.Currencies.FirstOrDefault(j => j.IsoCode == myIsoCode);

                                item.DistributorApplicationUserId = currentUser.Id;
                                item.DistributorId = currentUser.OrganizationId;

                                li.Add(item); // в лист
                            }
                        }
                    }

                    sheet.Dispose();
                    workbook.Dispose();
                }
            }
        }

        private string GetCellValue(CellRange[] reader, ShemaMember sm, bool price = false)
        {
            if (sm.ParentLevel != -1) // по родителям
            {
                if (reader[0].RowGroupLevel == sm.ParentLevel && sm.ParentLevel > 1) // если item лежит на родилельском уровне
                    // и индекс больше нуля. это заглушка для файлов, где категория берется из родителя, при этом все строки на одном уровне (а родитель - это строка с пустым ключевым полем)
                    return parentsByLevel[sm.ParentLevel - 2]; // тогда берем категорию с уровня выше
                else
                    return parentsByLevel[sm.ParentLevel - 1]; // обычный выбор категории
            }
            if (sm.Column == -1 && sm.DefaultValue != null) // по умолчанию
                return sm.DefaultValue;
            else if (sm.Column != -1 && price) // по колонке если цена
            {
                var match = Regex.Match(reader[sm.Column - 1].Value, @"\d+\.\d*").Value;
                return string.IsNullOrWhiteSpace(match) ? reader[sm.Column - 1].Value : match;
            }
            else if (sm.Column != -1) // по колонке
                return reader[sm.Column - 1].Value;

            return "";
        }

        private void CheckLevels(Shema shema, CellRange[] row)
        {
            var level = row[0].RowGroupLevel; // берем уровень вложенности
            // если нам нужен этот уровень вложенности
            if (LevelsToKeep.Count > 0 && LevelsToKeep.ContainsKey(level))
            {
                var col = LevelsToKeep[level] - 1;
                if (string.IsNullOrWhiteSpace(row[shema.KeyColumn - 1].Value)) // это группа, если ключевая колонка пустая
                    if (parentsByLevel.ContainsKey(level)) // если ключ есть
                        parentsByLevel[level] = row[col].Value; // перезаписывем
                    else
                        parentsByLevel.Add(level, row[col].Value); // добавляем
            }
        }

        private bool CheckForIgnore(Shema shema, CellRange[] row)
        {
            bool ignore = false;

            if (row[shema.IgnoreColumn - 1].Value == shema.IgnoreValue)
                ignore = true;

            return ignore;
        }
    }
}
