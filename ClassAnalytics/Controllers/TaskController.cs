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
    public class TaskController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult TasksHome()
        {
            return View("TasksHome");
        }

        // GET: Task
        public ActionResult Index()
        {
            return View(db.taskModel.ToList());
        }

        // GET: Task/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskModel taskModel = db.taskModel.Find(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            return View(taskModel);
        }

        // GET: Task/Create
        public ActionResult Create()
        {
            TaskViewModel tasksVM = new TaskViewModel();
            var tasktypes = db.TaskTypeModels.ToList();
            List<SelectListItem> tasks = new List<SelectListItem>();

            foreach(TaskTypeModels task in tasktypes)
            {
                tasks.Add(new SelectListItem() { Text = task.taskType + " Weight: " + task.taskWeight, Value = task.taskType_Id.ToString() });
            }
            tasksVM.taskTypes = tasks;
            return View(tasksVM);
        }

        // POST: Task/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,taskName,taskGrade,startDate,endDate,taskNotes")] TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                db.taskModel.Add(taskModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(taskModel);
        }

        // GET: Task/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskModel taskModel = db.taskModel.Find(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            return View(taskModel);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,taskName,taskGrade,startDate,endDate,taskNotes")] TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taskModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(taskModel);
        }

        // GET: Task/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskModel taskModel = db.taskModel.Find(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            return View(taskModel);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaskModel taskModel = db.taskModel.Find(id);
            db.taskModel.Remove(taskModel);
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
