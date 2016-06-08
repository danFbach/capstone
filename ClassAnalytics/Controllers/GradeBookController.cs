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
    public class GradeBookController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: GradeBook
        public ActionResult Index(int? class_id, int? task_Id)
        {
            if(class_id == null)
            {
                if(task_Id == null)
                {
                    
                    ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
                    ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName");
                    var gradeBookModel = db.gradeBookModel.Include(g => g.StudentModels).Include(g => g.TaskModel).Include(g => g.ClassModel).Include(g => g.TaskModel.UnitModels);
                    return View(gradeBookModel.ToList());
                }
                else
                {
                    List<GradeBookModel> grades = new List<GradeBookModel>();
                    ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
                    ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName");
                    var gradeBookModel = db.gradeBookModel.Include(g => g.StudentModels).Include(g => g.TaskModel).Include(g => g.ClassModel).Include(g => g.TaskModel.UnitModels).ToList();
                    foreach(GradeBookModel grade in gradeBookModel)
                    {
                        if(grade.task_Id == task_Id)
                        {
                            grades.Add(grade);
                        }
                    }
                    return View(grades);
                }
            }
            else
            {
                if (task_Id == null)
                {
                    List<GradeBookModel> grades = new List<GradeBookModel>();
                    ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
                    ViewBag.task_Id = new SelectList(db.taskModel, "unit_Id", "taskName");
                    var gradeBookModel = db.gradeBookModel.Include(g => g.StudentModels).Include(g => g.TaskModel).Include(g => g.ClassModel).Include(g => g.TaskModel.UnitModels).ToList();
                    foreach (GradeBookModel grade in gradeBookModel)
                    {
                        if (grade.class_Id == class_id)
                        {
                            grades.Add(grade);
                        }
                    }
                    return View(grades);
                }
                else
                {
                    List<GradeBookModel> grades = new List<GradeBookModel>();
                    ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
                    ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName");
                    var gradeBookModel = db.gradeBookModel.Include(g => g.StudentModels).Include(g => g.TaskModel).Include(g => g.ClassModel).Include(g => g.TaskModel.UnitModels).ToList();
                    foreach (GradeBookModel grade in gradeBookModel)
                    {
                        if(grade.class_Id == class_id)
                        {
                            if (grade.task_Id == task_Id)
                            {
                                grades.Add(grade);
                            }
                        }
                    }
                    return View(grades);
                }
                
            }
            
        }

        // GET: GradeBook/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradeBookModel gradeBookModel = db.gradeBookModel.Find(id);
            if (gradeBookModel == null)
            {
                return HttpNotFound();
            }
            return View(gradeBookModel);
        }
        public ActionResult CreateOne()
        {
            ViewBag.student_Id = new SelectList(db.studentModels, "student_Id", "fName");
            ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName");
            return View();
        }

        // POST: GradeBook/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOne(GradeBookModel gradeBookModel)
        {
            int classid = Convert.ToInt32(db.studentModels.Find(gradeBookModel.student_Id).class_Id);
            gradeBookModel.class_Id = classid;
            if (ModelState.IsValid)
            {
                if (gradeBookModel.grade > 100) { gradeBookModel.grade = 100; }
                if (gradeBookModel.grade < 0) { gradeBookModel.grade = 0; }
                db.gradeBookModel.Add(gradeBookModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.student_Id = new SelectList(db.studentModels, "student_Id", "fName", gradeBookModel.student_Id);
            ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName", gradeBookModel.task_Id);
            return View(gradeBookModel);
        }
        // GET: GradeBook/Create
        public ActionResult Create()
        {
            ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
            ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName");
            return View();
        }

        // POST: GradeBook/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GradeBookModel gradeBookModel)
        {
            if (ModelState.IsValid)
            {
                var students = db.studentModels.ToList();
                foreach(StudentModels student in students)
                {
                    if (student.class_Id == gradeBookModel.class_Id)
                    {
                        gradeBookModel.StudentModels = student;
                        gradeBookModel.student_Id = student.student_Id;
                        if (gradeBookModel.grade > 100) { gradeBookModel.grade = 100; }
                        if (gradeBookModel.grade < 0) { gradeBookModel.grade = 0; }
                        db.gradeBookModel.Add(gradeBookModel);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }


            ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className", gradeBookModel.class_Id);
            ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName", gradeBookModel.task_Id);
            return View(gradeBookModel);
        }

        // GET: GradeBook/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradeBookModel gradeBookModel = db.gradeBookModel.Find(id);
            if (gradeBookModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.student_Id = new SelectList(db.studentModels, "student_Id", "fName", gradeBookModel.student_Id);
            ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName", gradeBookModel.task_Id);
            return View(gradeBookModel);
        }

        // POST: GradeBook/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GradeBookModel gradeBookModel)
        {
            if (ModelState.IsValid)
            {
                if (gradeBookModel.grade > 100) { gradeBookModel.grade = 100; }
                if (gradeBookModel.grade < 0) { gradeBookModel.grade = 0; }
                db.Entry(gradeBookModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.student_Id = new SelectList(db.studentModels, "student_Id", "fName", gradeBookModel.student_Id);
            ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName", gradeBookModel.task_Id);
            return View(gradeBookModel);
        }

        // GET: GradeBook/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradeBookModel gradeBookModel = db.gradeBookModel.Find(id);
            if (gradeBookModel == null)
            {
                return HttpNotFound();
            }
            return View(gradeBookModel);
        }

        // POST: GradeBook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GradeBookModel gradeBookModel = db.gradeBookModel.Find(id);
            db.gradeBookModel.Remove(gradeBookModel);
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
