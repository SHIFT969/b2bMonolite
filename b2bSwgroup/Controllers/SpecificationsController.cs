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
using System.IO;
using System.Xml.Linq;
using AutoMapper;

namespace b2bSwgroup.Controllers
{
    public class SpecificationsController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        // GET: Specifications
        public async Task<ActionResult> Index()
        {
            ApplicationUser thisUser = await UserManager.FindByNameAsync(User.Identity.Name);

            var myCompany = await db.Organizations.Include(o => o.ApplicationUsers).FirstOrDefaultAsync(o => o.Id == thisUser.OrganizationId);
            var specifications = await db.Specifications.Include(s => s.Customer).Include(c => c.PositionsSpecification).Include(s => s.ApplicationUser).Where(s => s.CustomerId == myCompany.Id).ToListAsync();
            return View(specifications);
        }

        public async Task<ActionResult> Search(string key)
        {
            ApplicationUser thisUser = await UserManager.FindByNameAsync(User.Identity.Name);

            var myCompany = await db.Organizations.Include(o => o.ApplicationUsers).FirstOrDefaultAsync(o => o.Id == thisUser.OrganizationId);
            var specifications = await db.Specifications.Include(s => s.Customer).Include(c => c.PositionsSpecification).Include(s => s.ApplicationUser).Where(s => s.CustomerId == myCompany.Id).Where(z=>z.Zakazchik.Contains(key)).ToListAsync();
            return View(specifications);
        }

        public async Task<ActionResult> SelectSpecForAdd(int idPosition)
        {          
            var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
            var specifications = db.Specifications.Where(a=>a.ApplicationUserId==currentUser.Id).ToList();

            //передача позиции для добавления в представление выбора спецификации
            ViewBag.idPosition = idPosition;

            return PartialView(specifications);
        }

        //public async Task<>

        // GET: Specifications/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specification specification = await db.Specifications.Include(s=>s.PositionsSpecification).FirstOrDefaultAsync(i=>i.Id==id);
            if (specification == null)
            {
                return HttpNotFound();
            }
            return View(specification);
        }

        // GET: Specifications/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CustomerId = new SelectList(db.Organizations, "Id", "Name");
        //    return View();
        //}

        // POST: Specifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,ApplicationUserId,CustomerId,CustomerApplUserId")] Specification specification)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Specifications.Add(specification);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CustomerId = new SelectList(db.Organizations, "Id", "Name", specification.CustomerId);
        //    return View(specification);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int idPosition)
        {
            var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);

            Specification specification = new Specification();
            db.Specifications.Add(specification);
            var positionCatalog = db.Positionscatalog.FirstOrDefault(i => i.Id == idPosition);
            
            specification.PositionsSpecification.Add(ConvertCatForSpec(positionCatalog));
            specification.Name = "Новая спецификация";

            specification.ApplicationUserId = currentUser.Id;
            specification.CustomerId = currentUser.OrganizationId;
            await db.SaveChangesAsync();
            return View("SuccessAddPosition",specification);
        }

        //Метод добавляет позицию в спецификацию
        public async Task<ActionResult> AddPosition(int idSpec, int idPos)
        {
            var spec = await db.Specifications.FirstOrDefaultAsync(s => s.Id == idSpec);
            var posit = await db.Positionscatalog.FirstOrDefaultAsync(p=>p.Id==idPos);            
            spec.PositionsSpecification.Add(ConvertCatForSpec(posit));
            await db.SaveChangesAsync();
            
            return View("SuccessAddPosition",spec);
        }
        //метод удаляет позицию из спецификации
        public async Task<string> DeletePosition(int idSpec, int idPos)
        {
            var spec = await db.Specifications.Include(p=>p.PositionsSpecification).FirstOrDefaultAsync(s => s.Id == idSpec);
            var posit = await db.PositionsSpecification.FirstOrDefaultAsync(p => p.Id == idPos);
            spec.PositionsSpecification.Remove(posit);
            await db.SaveChangesAsync();
            return "Позиция удалена";
        }

        // GET: Specifications/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specification specification = await db.Specifications.Include(s => s.PositionsSpecification).FirstOrDefaultAsync(i => i.Id == id);
            if (specification == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", specification.CustomerId);
            return View(specification);
        }

        // POST: Specifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(/*[Bind(Include = "Id,ApplicationUserId,CustomerId,CustomerApplUserId")]*/ Specification specification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(specification).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Organizations, "Id", "Name", specification.CustomerId);
            return View(specification);
        }

        // GET: Specifications/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specification specification = await db.Specifications.FindAsync(id);
            if (specification == null)
            {
                return HttpNotFound();
            }
            return View(specification);
        }

        // POST: Specifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Specification specification = await db.Specifications.FindAsync(id);
            db.Specifications.Remove(specification);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<FileResult> Download(int id)
        {
            Workbook book = new Workbook();
            Worksheet sheet = book.Worksheets[0];
            string[] header = new string[]{ "Парт номер","наименование товара", "Стоимость","Валюта",
                "Поставщик"};
            sheet.InsertArray(header.ToArray(), 1, 1, false);
            var specification = await db.Specifications.Include(p => p.PositionsSpecification.Select(o=>o.Distributor)).Include(p => p.PositionsSpecification.Select(o => o.Currency)).Where(i => i.Id == id).FirstOrDefaultAsync();

            int countRow = 2;
            foreach(var position in specification.PositionsSpecification)
            {
                string[] row = { position.PartNumber.ToString(),position.Name.ToString(), position.Price.ToString() ,position.Currency.Name,position.Distributor.Name };
                sheet.InsertArray(row.ToArray(), countRow, 1,false);
                countRow++;
            }
            MemoryStream mem = new MemoryStream();
            book.SaveToStream(mem, FileFormat.Version2007);
            mem.Position = 0;
            return File(mem, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", specification.Name+".xlsx");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        PositionSpecification ConvertCatForSpec(PositionCatalog pos)
        {
            PositionSpecification positionSpecification = new PositionSpecification();

            Mapper.Initialize(cfg => cfg.CreateMap<PositionCatalog, PositionSpecification>());
            positionSpecification = Mapper.Map<PositionCatalog, PositionSpecification>(pos);
            return positionSpecification;
        }
    }
}
