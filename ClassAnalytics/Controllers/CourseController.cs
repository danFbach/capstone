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
    public class CourseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Course
        public ActionResult Index()
        {
            return View(db.coursemodels.ToList());
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseModels courseModels = db.coursemodels.Find(id);
            if (courseModels == null)
            {
                return HttpNotFound();
            }
            return View(courseModels);
        }

        // GET: Course/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,courseName")] CourseModels courseModels)
        {
            if (ModelState.IsValid)
            {
                db.coursemodels.Add(courseModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseModels);
        }

        // GET: Course/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseModels courseModels = db.coursemodels.Find(id);
            if (courseModels == null)
            {
                return HttpNotFound();
            }
            return View(courseModels);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,courseName")] CourseModels courseModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(courseModels);
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseModels courseModels = db.coursemodels.Find(id);
            if (courseModels == null)
            {
                return HttpNotFound();
            }
            return View(courseModels);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseModels courseModels = db.coursemodels.Find(id);
            db.coursemodels.Remove(courseModels);
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
