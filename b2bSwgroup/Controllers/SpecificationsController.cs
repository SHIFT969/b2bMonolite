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
using AutoMapper;
using b2bSwgroup.Models.ModelsForView;

namespace b2bSwgroup.Controllers
{
    [Authorize]
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
            List<Specification> specifications = new List<Specification>();
            if(myCompany!=null && thisUser is CustomerApplUser)
            {
                specifications = await db.Specifications.Include(s => s.Customer).Include(c => c.PositionsSpecification).Include(s => s.ApplicationUser).Where(s => s.CustomerId == myCompany.Id).ToListAsync();
                return View(specifications);
            }
            else
            {
                return RedirectToAction("NotOrganization", "Error");
            }                 
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
            if (currentUser.OrganizationId!=null)
            {                
                db.Specifications.Add(specification);
                var positionCatalog = db.Positionscatalog.FirstOrDefault(i => i.Id == idPosition);
                var positionSpec = ConvertCatForSpec(positionCatalog);
                positionSpec.Quantity = 1;
                specification.PositionsSpecification.Add(positionSpec);
                specification.Name = "Новая спецификация";

                specification.ApplicationUserId = currentUser.Id;
                specification.CustomerId = currentUser.OrganizationId;
                specification.DateCreate = DateTime.Now;
                specification.DateEdit = DateTime.Now;
                await db.SaveChangesAsync();
                await SetActivity(specification);
                ViewForAddPosition custView = new ViewForAddPosition() { Specification = specification, PosSpec = positionSpec };
                return View("SuccessAddPosition", custView);
            }
            else
            {
                return RedirectToAction("NotOrganizationPartial","Error");
            }
            
        }

        //Метод добавляет позицию в спецификацию
        public async Task<ActionResult> AddPosition(int idSpec, int idPos)
        {
            var spec = await db.Specifications.FirstOrDefaultAsync(s => s.Id == idSpec);
            var posit = await db.Positionscatalog.FirstOrDefaultAsync(p=>p.Id==idPos);
            var positionSpec = ConvertCatForSpec(posit);
            positionSpec.Quantity = 1;
            spec.PositionsSpecification.Add(positionSpec);
            await db.SaveChangesAsync();
            await SetActivity(spec);
            ViewForAddPosition custView = new ViewForAddPosition() { Specification = spec, PosSpec = positionSpec };
            return View("SuccessAddPosition", custView);
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
            Specification specification = await db.Specifications.Include(s => s.PositionsSpecification.Select(o => o.Currency)).FirstOrDefaultAsync(i => i.Id == id);
            //PositionSpecification spec = new PositionSpecification();
            
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
                specification.DateEdit = DateTime.Now;
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
                string[] row = { position.PartNumber.ToString(), position.Name.ToString(), position.Price.ToString() ,position.Currency==null ? "" : position.Currency.Name,position.Distributor.Name };
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

        public async Task EditQuantity(int positionId, int quant)
        {
            PositionSpecification position = await db.PositionsSpecification.FirstOrDefaultAsync(i => i.Id == positionId);
            position.Quantity = quant;
            await db.SaveChangesAsync();
        }

        [Authorize(Roles ="User")]
        public async Task<ActionResult> CurrentSpecsInfo()
        {
            var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
            Specification specification = await db.Specifications.Include(p=>p.PositionsSpecification).Where(u => u.ApplicationUserId == currentUser.Id).FirstOrDefaultAsync(a => a.Activity == true);
            return PartialView(specification);
        }
        async Task SetActivity(Specification specification)
        {
            var currentUser = await UserManager.FindByNameAsync(User.Identity.Name);
            List<Specification> AllSpecificationsActivity = await db.Specifications.Where(u => u.ApplicationUserId == currentUser.Id).Where(a => a.Activity == true).ToListAsync();
            foreach(var spec in AllSpecificationsActivity)
            {
                spec.Activity = false;
            }
            specification.Activity = true;
            await db.SaveChangesAsync();
        }
    }
}
