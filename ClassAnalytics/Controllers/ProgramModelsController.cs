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
    public class ProgramModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult ProgramHome()
        {
            return View("ProgramHome");
        }
        // GET: ProgramModels
        public ActionResult Index()
        {
            return View(db.programModels.ToList());
        }

        // GET: ProgramModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramModels programModels = db.programModels.Find(id);
            if (programModels == null)
            {
                return HttpNotFound();
            }
            return View(programModels);
        }

        // GET: ProgramModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProgramModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProgramModels programModels)
        {
            if (ModelState.IsValid)
            {
                db.programModels.Add(programModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programModels);
        }

        // GET: ProgramModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramModels programModels = db.programModels.Find(id);
            if (programModels == null)
            {
                return HttpNotFound();
            }
            return View(programModels);
        }

        // POST: ProgramModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProgramModels programModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(programModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(programModels);
        }

        // GET: ProgramModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramModels programModels = db.programModels.Find(id);
            if (programModels == null)
            {
                return HttpNotFound();
            }
            return View(programModels);
        }

        // POST: ProgramModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProgramModels programModels = db.programModels.Find(id);
            db.programModels.Remove(programModels);
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
