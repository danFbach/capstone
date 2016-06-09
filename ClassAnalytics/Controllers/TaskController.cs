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
            ViewBag.course_Id = new SelectList(db.coursemodels, "course_Id","courseName");
            ViewBag.taskType_Id = new SelectList(db.TaskTypeModels, "taskType_Id", "taskType");
            
            return View();
        }

        // POST: Task/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskViewModel viewModel)
        {
            TaskModel taskModel = new TaskModel();
            if (ModelState.IsValid)
            {
                int course_id = Convert.ToInt32(viewModel.course_Id);
                int taskTypeid = Convert.ToInt16(viewModel.taskType_Id);
                taskModel.task_Id = viewModel.Id;
                taskModel.taskName = viewModel.taskName;
                taskModel.taskType_Id = taskTypeid;
                taskModel.points = viewModel.points;
                taskModel.startDate = viewModel.startDate;
                taskModel.endDate = viewModel.endDate;
                taskModel.course_Id = course_id;
                taskModel.taskNotes = viewModel.taskNotes;

                db.taskModel.Add(taskModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.course_Id = new SelectList(db.coursemodels, "course_Id", "courseName");
            ViewBag.taskType_Id = new SelectList(db.TaskTypeModels, "taskType_Id", "taskType");
            return View(viewModel);
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
            ViewBag.course_Id = new SelectList(db.coursemodels, "course_Id", "courseName");
            ViewBag.taskType_Id = new SelectList(db.TaskTypeModels, "taskType_Id", "taskType");
            return View(taskModel);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskModel taskModel)
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
