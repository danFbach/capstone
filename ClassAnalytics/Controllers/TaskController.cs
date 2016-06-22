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

        // GET: Task
        public ActionResult Index(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id != null)
            {
                CourseModels course = db.coursemodels.Find(id);
                ViewBag.course = course.courseName + ": " + course.startDate + " - " + course.endDate;
                List<TaskModel> tasks = db.taskModel.ToList();
                List<TaskModel> new_tasks = new List<TaskModel>();
                foreach (TaskModel task in tasks)
                {
                task.TaskTypeModels = db.TaskTypeModels.Find(task.taskType_Id);
                task.CourseModels = db.coursemodels.Find(task.course_Id);
                    if (task.course_Id == id)
                    {
                        new_tasks.Add(task);
                    }
                }
                return View(new_tasks);
            }
            else
            {
                return View("Index","ProgramModels");
            }
        }

        // GET: Task/Details/5
        public ActionResult Details(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
        public ActionResult Create(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if(id != null)
            {
                TaskModel task = new TaskModel();
                int course_id = Convert.ToInt32(id);
                task.course_Id = course_id;
                CourseModels course = db.coursemodels.Find(id);
                ViewBag.course = course.courseName + ": " + course.startDate + " - " + course.endDate;
                ViewBag.taskType_Id = new SelectList(db.TaskTypeModels, "taskType_Id", "taskType");
                task.CourseModels = new CourseModels();
                task.CourseModels = db.coursemodels.Find(course_id);
                return View(task);
            }
            else
            {
                return RedirectToAction("Index","ProgramModels");
            }
        }

        // POST: Task/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskViewModel viewModel)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            CourseModels course = db.coursemodels.Find(viewModel.course_Id);
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
                List<StudentModels> students = db.studentModels.ToList();
                foreach(StudentModels student in students)
                {
                    GradeBookModel grade = new GradeBookModel();
                    student.ClassModel = db.classmodel.Find(student.class_Id);
                    viewModel.CourseModels = db.coursemodels.Find(viewModel.course_Id);
                    if(student.ClassModel.program_id == viewModel.CourseModels.program_Id)
                    {
                        grade.class_Id = student.class_Id;
                        grade.pointsEarned = null;
                        grade.possiblePoints = viewModel.points;
                        grade.student_Id = student.student_Id;
                        grade.task_Id = taskModel.task_Id;
                        db.gradeBookModel.Add(grade);
                    }
                }
                db.SaveChanges();

                return RedirectToAction("Index/" + viewModel.course_Id, "Task");
            }
            ViewBag.course = course.courseName + ": " + course.startDate + " - " + course.endDate;
            ViewBag.taskType_Id = new SelectList(db.TaskTypeModels, "taskType_Id", "taskType");
            return View(viewModel);
        }

        // GET: Task/Edit/5
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
            TaskModel taskModel = db.taskModel.Find(id);
            if (taskModel == null)
            {
                return HttpNotFound();
            }
            CourseModels course = db.coursemodels.Find(taskModel.course_Id);
            taskModel.CourseModels = course;
            ViewBag.course = course.courseName + ": " + course.startDate + " - " + course.endDate;
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
