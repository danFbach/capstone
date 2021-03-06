﻿using System;
using System.Web;
using System.Net;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using ClassAnalytics.Models;
using ClassAnalytics.Models.Task_Models;
using ClassAnalytics.Models.Class_Models;
using ClassAnalytics.Models.Gradebook_Models;
using Microsoft.AspNet.Identity;

namespace ClassAnalytics.Controllers
{
    public class GradeBookController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Student_Chart()
        {
            if (!this.User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<GradeBookModel> this_grade = new List<GradeBookModel>();
            //List<TaskModel> tasks = new List<TaskModel>(); FOR TASK DROPDOWN
            GradeBookModel a_grade = new GradeBookModel();
            var students = db.studentModels.ToList();
            var grades = db.gradeBookModel.ToList();
            StudentModels this_student = new StudentModels();
            foreach (StudentModels student in students)
            {
                if (student.student_account_Id == UserId)
                {
                    this_student = student;
                }
            }
            foreach (GradeBookModel grade in grades)
            {
                grade.TaskModel = new TaskModel();
                grade.TaskModel = db.taskModel.Find(grade.task_Id);
                grade.task_Id = grade.TaskModel.task_Id;
                int class_possible = 0;
                decimal? class_total = 0;
                if (grade.student_Id == this_student.student_Id)
                {
                    //tasks.Add(grade.TaskModel); ADD TASK TO DROPDOWN
                    foreach (GradeBookModel grade1 in grades)
                    {
                        if (this_student.class_Id == grade1.class_Id)
                        {
                            grade.possiblePoints = grade.TaskModel.points;
                            if (grade.task_Id == grade1.task_Id)
                            {
                                class_possible += grade.TaskModel.points;
                                class_total += grade1.pointsEarned;
                            }
                        }
                    }
                    grade.grade = (class_total / class_possible) * 100;
                    a_grade = grade;
                    this_grade.Add(a_grade);
                }
            }
            //ViewBag.task_id = new SelectList(tasks, "task_Id", "taskName"); SEND TASK DROPDOWN
            return View(this_grade);
        }

        public ActionResult AdminCharts(int? id, int? task_id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            GradebookViewModel viewModel = new GradebookViewModel();
            viewModel.taskList = new List<SelectListItem>();
            viewModel.grades = new List<GradeBookModel>();
            List<ClassTaskJoinModel> classTasks = db.classTask.ToList();
            List<TaskModel> tasks = db.taskModel.ToList();
            List<GradeBookModel> grades = db.gradeBookModel.ToList();
            ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
            if (id == null)
            {
                return RedirectToAction("Index", "Class");
            }
            ClassModel this_class = db.classmodel.Find(id);
            foreach(ClassTaskJoinModel task in classTasks)
            {
                if(task.class_id == this_class.class_Id)
                {
                    TaskModel newTask = db.taskModel.Find(task.task_id);
                    viewModel.taskList.Add(new SelectListItem() { Text = newTask.taskName, Value = newTask.task_Id.ToString() });
                }
            }
            foreach (GradeBookModel grade in grades)
            {
                grade.TaskModel = db.taskModel.Find(grade.task_Id);

                if (grade.class_Id == id)
                {
                    if (task_id == null)
                    {
                        viewModel.grades = classAverage(id);
                        break;
                    }
                    else if (grade.task_Id == task_id)
                    {
                        grade.StudentModels = db.studentModels.Find(grade.student_Id);
                        grade.possiblePoints = db.taskModel.Find(grade.task_Id).points;
                        viewModel.grades.Add(grade);
                    }
                }
            }
            viewModel.taskList = viewModel.taskList.OrderBy(x => x.Text).ToList();
            viewModel.grades = viewModel.grades.OrderByDescending(x => x.grade).ToList();
            return View(viewModel);
        }
        public List<GradeBookModel> classAverage(int? class_id)
        {
            List<GradeBookModel> grade_list = new List<GradeBookModel>();
            List<GradeBookModel> grades = db.gradeBookModel.ToList();
            List<StudentModels> students = db.studentModels.ToList();

            foreach (StudentModels student in students)
            {
                GradeBookModel grade = new GradeBookModel();
                if (student.class_Id == class_id)
                {
                    int possible = 0;
                    decimal? earned = 0;
                    grade.StudentModels = student;
                    grade.student_Id = student.student_Id;
                    foreach (GradeBookModel one_Grade in grades)
                    {
                        grade.TaskModel = new TaskModel();
                        if (one_Grade.student_Id == student.student_Id)
                        {
                            if (one_Grade.pointsEarned != null)
                            {
                                grade.TaskModel = db.taskModel.Find(one_Grade.task_Id);
                                possible += one_Grade.TaskModel.points;
                                earned += one_Grade.pointsEarned;
                            }
                        }
                    }
                    if (earned != 0)
                    {
                        grade.StudentModels = db.studentModels.Find(grade.student_Id);
                        grade.pointsEarned = earned;
                        grade.possiblePoints = possible;
                        grade.grade = (earned / possible) * 100;
                        grade_list.Add(grade);
                    }
                }
            }
            return grade_list;
        }

        public ActionResult StudentDetails(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Student_Index");
            }
            GradeBookModel grade = db.gradeBookModel.Find(id);
            grade.TaskModel = db.taskModel.Find(grade.task_Id);
            return View(grade);
        }

        public ActionResult Student_Index()
        {
            if (!this.User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            GradebookViewModel viewModel = new GradebookViewModel();
            viewModel.grades = new List<GradeBookModel>();
            StudentModels currentStudent = new StudentModels();
            List<StudentModels> students = db.studentModels.ToList();
            List<GradeBookModel> all_grades = db.gradeBookModel.ToList();
            foreach (StudentModels student in students)
            {
                if (student.student_account_Id == UserId)
                {
                    currentStudent = student;
                    viewModel.studentName = student.fName + " " + student.lName;
                    break;
                }
            }
            foreach (GradeBookModel grade in all_grades)
            {
                if (grade.student_Id == currentStudent.student_Id)
                {
                    grade.ClassModel = db.classmodel.Find(grade.class_Id);
                    grade.TaskModel = db.taskModel.Find(grade.task_Id);
                    viewModel.grades.Add(grade);
                }
            }
            return View(viewModel);
        }
        // GET: GradeBook
        public ActionResult Index(int? task_Id, int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || task_Id == null)
            {
                return RedirectToAction("Index", "Class");
            }
            else
            {
                GradebookViewModel viewModel = new GradebookViewModel();
                List<ClassTaskJoinModel> joinModel = db.classTask.ToList();
                viewModel.taskList = new List<SelectListItem>();
                List<TaskModel> tasks = db.taskModel.ToList();
                viewModel.grades = new List<GradeBookModel>();
                List<GradeBookModel> gradeBookModel = db.gradeBookModel.Include(g => g.StudentModels).Include(g => g.TaskModel).Include(g => g.ClassModel).Include(g => g.TaskModel.CourseModels).ToList();
                ClassModel this_class = db.classmodel.Find(id);
                foreach (ClassTaskJoinModel classTask in joinModel)
                {
                    if (classTask.class_id == id)
                    {
                        classTask.task = db.taskModel.Find(classTask.task_id);
                        viewModel.taskList.Add(new SelectListItem() { Text = classTask.task.taskName, Value = classTask.task_id.ToString() });
                    }
                }
                foreach (GradeBookModel grade in gradeBookModel)
                {
                    if (grade.class_Id == id)
                    {
                        if (grade.task_Id == task_Id)
                        {
                            viewModel.grades.Add(grade);
                        }
                    }
                }
                return View(viewModel);
            }
        }

        // GET: GradeBook/Details/5
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
            GradeBookModel gradeBookModel = db.gradeBookModel.Find(id);
            if (gradeBookModel == null)
            {
                return HttpNotFound();
            }
            return View(gradeBookModel);
        }
        public ActionResult CreateOne()
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<SelectListItem> student_drop = new List<SelectListItem>();
            var students = db.studentModels.ToList();

            foreach (StudentModels student in students)
            {
                student_drop.Add(new SelectListItem() { Text = student.lName + ", " + student.fName, Value = student.student_Id.ToString() });
            }

            ViewBag.student_Id = student_drop;
            ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOne(GradeBookModel gradeBookModel)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            gradeBookModel.StudentModels = db.studentModels.Find(gradeBookModel.student_Id);
            gradeBookModel.class_Id = gradeBookModel.StudentModels.class_Id;
            gradeBookModel.TaskModel = db.taskModel.Find(gradeBookModel.task_Id);
            gradeBookModel.TaskModel.points = gradeBookModel.TaskModel.points;
            if (ModelState.IsValid)
            {
                if (gradeBookModel.TaskModel.points < gradeBookModel.pointsEarned) { gradeBookModel.pointsEarned = gradeBookModel.TaskModel.points; }
                if (gradeBookModel.pointsEarned < 0) { gradeBookModel.pointsEarned = 0; }
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                var students = db.studentModels.ToList();
                foreach (StudentModels student in students)
                {
                    if (student.class_Id == gradeBookModel.class_Id)
                    {
                        int points = db.taskModel.Find(gradeBookModel.task_Id).points;
                        gradeBookModel.possiblePoints = points;
                        gradeBookModel.StudentModels = student;
                        gradeBookModel.student_Id = student.student_Id;
                        gradeBookModel.pointsEarned = 0;
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GradeBookModel gradeBookModel = db.gradeBookModel.Find(id);
            StudentModels student = db.studentModels.Find(gradeBookModel.student_Id);
            if (gradeBookModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Student = student.fName + " " + student.lName;
            gradeBookModel.TaskModel = db.taskModel.Find(gradeBookModel.task_Id);
            return View(gradeBookModel);
        }

        // POST: GradeBook/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GradeBookModel gradeBookModel)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (gradeBookModel.assignment_notes == null)
            {
                gradeBookModel.assignment_notes = "";
            }
            if (ModelState.IsValid)
            {
                if (gradeBookModel.pointsEarned < 0) { gradeBookModel.pointsEarned = 0; }
                gradeBookModel.grade = Convert.ToDecimal((gradeBookModel.pointsEarned / gradeBookModel.possiblePoints) * 100);
                GradeBookModel grade = db.gradeBookModel.Find(gradeBookModel.grade_Id);
                db.gradeBookModel.Remove(grade);
                db.gradeBookModel.Add(gradeBookModel);
                db.SaveChanges();
                return RedirectToAction("Index/" + gradeBookModel.class_Id);
            }
            ViewBag.Student = gradeBookModel.StudentModels.fName + " " + gradeBookModel.StudentModels.lName;
            gradeBookModel.ClassModel = db.classmodel.Find(gradeBookModel.class_Id);
            gradeBookModel.StudentModels = db.studentModels.Find(gradeBookModel.student_Id);
            gradeBookModel.TaskModel = db.taskModel.Find(gradeBookModel.task_Id);
            return View(gradeBookModel);
        }

        // GET: GradeBook/Delete/5
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
            GradeBookModel gradeBookModel = db.gradeBookModel.Find(id);
            gradeBookModel.StudentModels = db.studentModels.Find(gradeBookModel.student_Id);
            gradeBookModel.TaskModel = db.taskModel.Find(gradeBookModel.task_Id);
            gradeBookModel.TaskModel.CourseModels = db.coursemodels.Find(gradeBookModel.TaskModel.course_Id);
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
