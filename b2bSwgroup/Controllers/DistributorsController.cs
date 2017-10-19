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

namespace b2bSwgroup.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DistributorsController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: Distributors
        public async Task<ActionResult> Index()
        {
            return View(await db.Distributors.ToListAsync());
        }

        // GET: Distributors/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distributor distributor = await db.Distributors.FindAsync(id);
            if (distributor == null)
            {
                return HttpNotFound();
            }
            return View(distributor);
        }

        // GET: Distributors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Distributors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,INN")] Distributor distributor)
        {
            if (ModelState.IsValid)
            {
                db.Organizations.Add(distributor);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(distributor);
        }

        // GET: Distributors/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distributor distributor = await db.Distributors.FindAsync(id);
            if (distributor == null)
            {
                return HttpNotFound();
            }
            return View(distributor);
        }

        // POST: Distributors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,INN")] Distributor distributor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(distributor).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(distributor);
        }

        // GET: Distributors/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distributor distributor = await db.Distributors.FindAsync(id);
            if (distributor == null)
            {
                return HttpNotFound();
            }
            return View(distributor);
        }

        // POST: Distributors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Distributor distributor = await db.Distributors.FindAsync(id);
            db.Organizations.Remove(distributor);
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
