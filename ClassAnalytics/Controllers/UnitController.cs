using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClassAnalytics.Models;

namespace ClassAnalytics.Controllers
{
    public class UnitController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Unit
        public ActionResult Index()
        {
            return View(db.unitModels.ToList());
        }

        // GET: Unit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitModels unitModels = db.unitModels.Find(id);
            if (unitModels == null)
            {
                return HttpNotFound();
            }
            return View(unitModels);
        }

        // GET: Unit/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Unit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,unitName,startDate,endDate")] UnitModels unitModels)
        {
            if (ModelState.IsValid)
            {
                db.unitModels.Add(unitModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(unitModels);
        }

        // GET: Unit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitModels unitModels = db.unitModels.Find(id);
            if (unitModels == null)
            {
                return HttpNotFound();
            }
            return View(unitModels);
        }

        // POST: Unit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,unitName,startDate,endDate")] UnitModels unitModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unitModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unitModels);
        }

        // GET: Unit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitModels unitModels = db.unitModels.Find(id);
            if (unitModels == null)
            {
                return HttpNotFound();
            }
            return View(unitModels);
        }

        // POST: Unit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UnitModels unitModels = db.unitModels.Find(id);
            db.unitModels.Remove(unitModels);
            db.SaveChanges();
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
