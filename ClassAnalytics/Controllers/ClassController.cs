﻿using System;
using System.IO;
using System.Web;
using System.Net;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using ClassAnalytics.Models;
using ClassAnalytics.Models.Task_Models;
using ClassAnalytics.Models.Misc_Models;
using ClassAnalytics.Models.Class_Models;
using ClassAnalytics.Models.Program_Models;
using ClassAnalytics.Models.Gradebook_Models;

namespace ClassAnalytics.Controllers
{
    public class ClassController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult dashboard()
        {
            List<programListViewModel> classList = new List<programListViewModel>();
            List<ClassModel> classes = db.classmodel.ToList();
            List<StudentModels> students = db.studentModels.ToList();
            List<CourseModels> courses = db.coursemodels.ToList();
            List<ClassTaskJoinModel> tasks = db.classTask.ToList();
            foreach(ClassModel _class in classes)
            {
                programListViewModel _thisClass = new programListViewModel();
                _thisClass.studentCount = 0;
                _thisClass.courseCount = 0;
                _thisClass.taskCount = 0;
                foreach (StudentModels student in students)
                {
                    if(student.class_Id == _class.class_Id)
                    {
                        _thisClass.studentCount += 1;
                    }
                }
                foreach(CourseModels course in courses)
                {
                    if(course.program_Id == _class.program_id)
                    {
                        _thisClass.courseCount += 1;
                    }
                }
                foreach(ClassTaskJoinModel _task in tasks)
                {
                    if(_task.class_id == _class.class_Id)
                    {
                        _thisClass.taskCount += 1;
                    }
                }
                _thisClass._class = _class;
                _thisClass.program = db.programModels.Find(_class.program_id);
                classList.Add(_thisClass);
            }
            return View(classList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult partialEdit(GradeBookModel gradeBookModel)
        {
            GradeBookModel grade = db.gradeBookModel.Find(gradeBookModel.grade_Id);
            if(grade.pointsEarned != gradeBookModel.pointsEarned || grade.assignment_notes != gradeBookModel.assignment_notes)
            {
                if (gradeBookModel.assignment_notes == null)
                {
                    gradeBookModel.assignment_notes = "";
                }
                if (gradeBookModel.pointsEarned < 0) { gradeBookModel.pointsEarned = 0; }
                gradeBookModel.grade = Convert.ToDecimal((gradeBookModel.pointsEarned / gradeBookModel.possiblePoints) * 100);
                db.gradeBookModel.Remove(grade);
                db.gradeBookModel.Add(gradeBookModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult StudentDetails(int? id)
        {
            if (!this.User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            var grade = db.gradeBookModel.Find(id);
            grade.ClassModel = db.classmodel.Find(grade.class_Id);
            grade.StudentModels = db.studentModels.Find(grade.student_Id);
            grade.TaskModel = db.taskModel.Find(grade.task_Id);
            grade.possiblePoints = grade.TaskModel.points;
            return View(grade);
        }
        public ActionResult Index(string status)
        {
            if (this.User.IsInRole("Student"))
            {
                return RedirectToAction("studentIndex","Upload");
            }
            List<programListViewModel> listViewModel = new List<programListViewModel>();
            courseListViewModel courseModel = new courseListViewModel();
            programListViewModel programListView = new programListViewModel();
            List<GradeBookModel> grades = db.gradeBookModel.ToList();
            List<ClassTaskJoinModel> joinTask = db.classTask.ToList();
            List<ClassModel> classes = db.classmodel.ToList();
            List<ProgramModels> programs = db.programModels.ToList();
            List<CourseModels> courses = db.coursemodels.ToList();
            foreach (ClassModel _class in classes)
            {
                programListView = new programListViewModel();
                programListView._class = _class;
                foreach (ProgramModels program in programs)
                {
                    if (program.program_Id == _class.program_id)
                    {
                        programListView.program = program;
                        programListView.courses = new List<courseListViewModel>();
                        foreach (CourseModels course in courses)
                        {
                            courseModel = new courseListViewModel();
                            if (course.program_Id == program.program_Id)
                            {
                                courseModel.course = course;
                                courseModel.tasks = new List<taskListViewModel>();
                                foreach (ClassTaskJoinModel classTask in joinTask)
                                {
                                    taskListViewModel newTask = new taskListViewModel();
                                    newTask.grades = new List<GradeBookModel>();
                                    if (classTask.class_id == _class.class_Id)
                                    {
                                        TaskModel task = db.taskModel.Find(classTask.task_id);
                                        if (task.course_Id == course.course_Id)
                                        {
                                            task.TaskTypeModels = db.TaskTypeModels.Find(task.taskType_Id);
                                            newTask.task = task;
                                            foreach (GradeBookModel grade in grades)
                                            {
                                                if (grade.class_Id == _class.class_Id)
                                                {
                                                    if (grade.task_Id == task.task_Id)
                                                    {
                                                        grade.StudentModels = db.studentModels.Find(grade.student_Id);
                                                        newTask.grades.Add(grade);
                                                    }
                                                }
                                            }
                                            newTask.grades = newTask.grades.OrderBy(x => x.StudentModels.lName).ToList();
                                            courseModel.tasks.Add(newTask);
                                        }
                                    }
                                }
                                courseModel.tasks = courseModel.tasks.OrderBy(x => x.task.TaskTypeModels.taskType).ToList();
                                programListView.courses.Add(courseModel);
                            }
                        }
                        listViewModel.Add(programListView);
                    }
                }
            }
            return View(listViewModel);
        }
        public ActionResult assignClass(int? task_id)
        {
            if (task_id != null)
            {
                assignTaskViewModel viewModel = new assignTaskViewModel();
                ClassTaskJoinModel classTask = new ClassTaskJoinModel();
                classTask.task = db.taskModel.Find(task_id);
                classTask.task_id = classTask.task.task_Id;
                viewModel.classTask = classTask;
                viewModel = getClasses(viewModel);
                return View(viewModel);
            }
            ViewBag.statusMessage = "Error";
            return View("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult assignClass(assignTaskViewModel viewModel)
        {
            if (viewModel != null)
            {
                ClassTaskJoinModel classTask = new ClassTaskJoinModel();
                List<ClassTaskJoinModel> tasks = db.classTask.ToList();
                classTask.class_id = viewModel.classTask.class_id;
                classTask.task_id = viewModel.classTask.task_id;
                classTask._class = db.classmodel.Find(classTask.class_id);
                classTask.task = db.taskModel.Find(classTask.task_id);
                foreach (ClassTaskJoinModel task in tasks)
                {
                    if (task.class_id == classTask.class_id)
                    {
                        if (task.task_id == classTask.task_id)
                        {
                            viewModel.classTask.task = db.taskModel.Find(classTask.task_id);
                            viewModel = getClasses(viewModel);
                            ViewBag.statusMessage = classTask.task.taskName + " is already assigned to " + classTask._class.className + ".";
                            return View(viewModel);
                        }
                    }
                }
                db.classTask.Add(classTask);
                createGrades(classTask.task, classTask.class_id);
                db.SaveChanges();
                ViewBag.statusMessage = classTask.task.taskName + " has been assigned to" + classTask._class.className;
                return RedirectToAction("Index");
            }
            ViewBag.statusMessage = "Invalid Entry";
            return RedirectToAction("Index");
        }
        public assignTaskViewModel getClasses(assignTaskViewModel viewModel)
        {
            List<ClassModel> classes = db.classmodel.ToList();
            viewModel.classes = new List<SelectListItem>();
            foreach (ClassModel _class in classes)
            {
                CourseModels course = db.coursemodels.Find(viewModel.classTask.task.course_Id);
                if (_class.program_id == course.program_Id)
                {
                    viewModel.classes.Add(new SelectListItem() { Text = _class.className, Value = _class.class_Id.ToString() });
                }
            }
            return viewModel;
        }
        public void createGrades(TaskModel task, int class_id)
        {
            List<StudentModels> students = db.studentModels.ToList();
            foreach (StudentModels student in students)
            {
                GradeBookModel grade = new GradeBookModel();
                student.ClassModel = db.classmodel.Find(student.class_Id);
                task.CourseModels = db.coursemodels.Find(task.course_Id);
                if (student.class_Id == class_id)
                {
                    grade.class_Id = class_id;
                    grade.pointsEarned = null;
                    grade.possiblePoints = task.points;
                    grade.student_Id = student.student_Id;
                    grade.task_Id = task.task_Id;
                    db.gradeBookModel.Add(grade);
                }
            }
        }
        public ActionResult studentIndex(int? id)
        {
            if (id != null)
            {
                return RedirectToAction("Index/" + id, "Students");
            }
            else
            {
                return View("Index");
            }
        }
        public ActionResult addStudent(int? id)
        {
            if (id != null)
            {
                return RedirectToAction("Create/" + id, "Students");
            }
            else
            {
                return View("Index");
            }
        }

        // GET: Class
        public ActionResult classGrades(int? id)
        {
            if (id != null)
            {
                return RedirectToAction("Index", "GradeBook", id);
            }
            else
            {
                return View("Index");
            }
        }
        //public ActionResult Index(int? program_id)
        //{
        //    if (!this.User.IsInRole("Admin"))
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    ViewBag.statusMessage = "";
        //    ViewBag.program_id = new SelectList(db.programModels, "program_Id", "programName");
        //    var classes = db.classmodel.ToList();
        //    List<ProgClassViewModel> new_list = new List<ProgClassViewModel>();
        //    if (program_id != null)
        //    {
        //        foreach (ClassModel a_class in classes)
        //        {
        //            if (a_class.program_id == program_id)
        //            {
        //                new_list.Add(new ProgClassViewModel() { class_Id = a_class.class_Id, className = a_class.className, program_id = a_class.program_id, ProgramModels = db.programModels.Find(a_class.program_id) });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (ClassModel a_class in classes)
        //        {
        //            new_list.Add(new ProgClassViewModel() { class_Id = a_class.class_Id, className = a_class.className, program_id = a_class.program_id, ProgramModels = db.programModels.Find(a_class.program_id) });
        //        }
        //    }
        //    return View(new_list);
        //}

        // GET: Class/Details/5
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
            ClassModel classModel = db.classmodel.Find(id);
            if (classModel == null)
            {
                return HttpNotFound();
            }
            return View(classModel);
        }

        // GET: Class/Create
        public ActionResult Create()
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ProgClassViewModel viewModel = new ProgClassViewModel();
            List<ProgramModels> programs = db.programModels.ToList();
            viewModel.program_list = new List<SelectListItem>();

            foreach (ProgramModels program in programs)
            {
                viewModel.program_list.Add(new SelectListItem() { Text = program.programName, Value = program.program_Id.ToString() });
            }
            return View(viewModel);
        }

        // POST: Class/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProgClassViewModel viewModel)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ClassModel new_class = new ClassModel();
            if (ModelState.IsValid)
            {
                new_class.className = viewModel.className;
                new_class.class_Id = viewModel.class_Id;
                new_class.program_id = viewModel.program_id;
                new_class.ProgramModels = db.programModels.Find(viewModel.program_id);
                if (!Directory.Exists(Server.MapPath("~//Uploads//classData//" + new_class.className)))
                {
                    Directory.CreateDirectory(Server.MapPath("~//Uploads//classData//" + new_class.className));
                }
                db.classmodel.Add(new_class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // GET: Class/Edit/5
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
            ClassModel classModel = db.classmodel.Find(id);
            if (classModel == null)
            {
                return HttpNotFound();
            }

            ProgClassViewModel viewModel = new ProgClassViewModel();
            List<ProgramModels> programs = db.programModels.ToList();
            viewModel.program_list = new List<SelectListItem>();
            viewModel.program_id = classModel.program_id;
            viewModel.class_Id = classModel.class_Id;
            viewModel.className = classModel.className;
            foreach (ProgramModels program in programs)
            {
                viewModel.program_list.Add(new SelectListItem() { Text = program.programName, Value = program.program_Id.ToString() });
            }
            return View(viewModel);
        }

        // POST: Class/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProgClassViewModel viewModel)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ClassModel model = new ClassModel();
            model.className = viewModel.className;
            model.class_Id = viewModel.class_Id;
            model.program_id = viewModel.program_id;
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.program_Id = new SelectList(db.programModels, "pragram_Id", "programName");
            List<ProgramModels> programs = db.programModels.ToList();
            viewModel.program_list = new List<SelectListItem>();
            foreach (ProgramModels program in programs)
            {
                viewModel.program_list.Add(new SelectListItem() { Text = program.programName, Value = program.program_Id.ToString() });
            }
            return View(viewModel);
        }

        // GET: Class/Delete/5
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
            ClassModel classModel = db.classmodel.Find(id);
            if (classModel == null)
            {
                return HttpNotFound();
            }
            return View(classModel);
        }

        // POST: Class/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ClassModel classModel = db.classmodel.Find(id);
            db.classmodel.Remove(classModel);
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
