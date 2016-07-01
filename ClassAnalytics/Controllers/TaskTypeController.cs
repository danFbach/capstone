using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClassAnalytics.Models;
using ClassAnalytics.Models.Task_Models;

namespace ClassAnalytics.Controllers
{
    public class TaskTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TaskType
        public ActionResult Index()
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View(db.TaskTypeModels.ToList());
        }

        // GET: TaskType/Create
        public ActionResult Create()
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: TaskType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskTypeModels taskTypeModels)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                db.TaskTypeModels.Add(taskTypeModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(taskTypeModels);
        }

        // GET: TaskType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskTypeModels taskTypeModels = db.TaskTypeModels.Find(id);
            if (taskTypeModels == null)
            {
                return HttpNotFound();
            }
            return View(taskTypeModels);
        }

        // POST: TaskType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskTypeModels taskTypeModels)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                db.Entry(taskTypeModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(taskTypeModels);
        }

        // GET: TaskType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskTypeModels taskTypeModels = db.TaskTypeModels.Find(id);
            if (taskTypeModels == null)
            {
                return HttpNotFound();
            }
            return View(taskTypeModels);
        }

        // POST: TaskType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            TaskTypeModels taskTypeModels = db.TaskTypeModels.Find(id);
            db.TaskTypeModels.Remove(taskTypeModels);
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
