using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClassAnalytics.Models;
using Microsoft.AspNet.Identity;

namespace ClassAnalytics.Controllers
{
    public class GradeBookController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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

        public ActionResult AdminCharts(GradebookViewModel viewModel)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                List<GradeBookModel> this_grades = db.gradeBookModel.ToList();
                viewModel.classList = new List<SelectListItem>();
                viewModel.taskList = new List<SelectListItem>();
                viewModel.grades = new List<GradeBookModel>();

                foreach (ClassModel a_class in db.classmodel.ToList())
                {
                    viewModel.classList.Add(new SelectListItem() { Text = a_class.className, Value = a_class.class_Id.ToString() });
                }

                foreach (GradeBookModel grade in this_grades)
                {
                    grade.TaskModel = db.taskModel.Find(grade.task_Id);
                    if (viewModel.class_id == null)
                    {
                        return View(viewModel);
                    }
                    else if (grade.class_Id == viewModel.class_id)
                    {
                        if (viewModel.task_id == null)
                        {
                            viewModel.grades = classAverage(viewModel.class_id);
                            break;
                        }
                        else if (grade.task_Id == viewModel.task_id)
                        {
                            grade.StudentModels = db.studentModels.Find(grade.student_Id);
                            grade.possiblePoints = db.taskModel.Find(grade.task_Id).points;
                            viewModel.grades.Add(grade);
                        }
                    }
                }
                viewModel.grades = viewModel.grades.OrderBy(x => x.grade).ToList();
                if (viewModel.class_id != null)
                {
                    ClassModel this_class = db.classmodel.Find(viewModel.class_id);
                    List<TaskModel> tasks = db.taskModel.ToList();
                    foreach (TaskModel task in tasks)
                    {
                        task.CourseModels = db.coursemodels.Find(task.course_Id);
                        if (task.CourseModels.program_Id == this_class.program_id)
                        {
                            viewModel.taskList.Add(new SelectListItem() { Text = task.taskName, Value = task.task_Id.ToString() });
                        }
                    }
                }

                return View(viewModel);
            }
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
                             if(one_Grade.pointsEarned != null)
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
        public ActionResult Index(int? class_id, int? task_Id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            GradebookViewModel viewModel = new GradebookViewModel();
            viewModel.taskList = new List<SelectListItem>();
            List<TaskModel> tasks = db.taskModel.ToList();
            ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
            if(class_id != null)
            {
                ClassModel this_class = db.classmodel.Find(class_id);
                foreach (TaskModel task in tasks)
                {
                    task.CourseModels = db.coursemodels.Find(task.course_Id);
                    if (task.CourseModels.program_Id == this_class.program_id)
                    {
                        viewModel.taskList.Add(new SelectListItem() { Text = task.taskName, Value = task.task_Id.ToString() });
                    }
                }
            }
            if (class_id == null)
            {
                if (task_Id == null)
                {
                    viewModel.grades = new List<GradeBookModel>();
                    return View(viewModel);
                }
                else
                {
                    viewModel.grades = new List<GradeBookModel>();
                    var gradeBookModel = db.gradeBookModel.Include(g => g.StudentModels).Include(g => g.TaskModel).Include(g => g.ClassModel).Include(g => g.TaskModel.CourseModels).ToList();
                    foreach (GradeBookModel grade in gradeBookModel)
                    {
                        if (grade.task_Id == task_Id)
                        {
                            viewModel.grades.Add(grade);
                        }
                    }
                    return View(viewModel);
                }
            }
            else
            {
                if (task_Id == null)
                {
                    viewModel.grades = new List<GradeBookModel>();
                    ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
                    ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName");
                    var gradeBookModel = db.gradeBookModel.Include(g => g.StudentModels).Include(g => g.TaskModel).Include(g => g.ClassModel).Include(g => g.TaskModel.CourseModels).ToList();
                    foreach (GradeBookModel grade in gradeBookModel)
                    {
                        if (grade.class_Id == class_id)
                        {
                            viewModel.grades.Add(grade);
                        }
                    }
                    return View(viewModel);
                }
                else
                {
                    viewModel.grades = new List<GradeBookModel>();
                    ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
                    ViewBag.task_Id = new SelectList(db.taskModel, "task_Id", "taskName");
                    var gradeBookModel = db.gradeBookModel.Include(g => g.StudentModels).Include(g => g.TaskModel).Include(g => g.ClassModel).Include(g => g.TaskModel.CourseModels).ToList();
                    foreach (GradeBookModel grade in gradeBookModel)
                    {
                        if (grade.class_Id == class_id)
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
            gradeBookModel.TaskModel = new TaskModel();
            gradeBookModel.possiblePoints = db.gradeBookModel.Find(gradeBookModel.grade_Id).possiblePoints;
            gradeBookModel.grade = (gradeBookModel.pointsEarned / gradeBookModel.possiblePoints) * 100;
            gradeBookModel.grade = Convert.ToDecimal(gradeBookModel.grade);
            gradeBookModel.StudentModels = db.studentModels.Find(gradeBookModel.student_Id);
            gradeBookModel.ClassModel = db.classmodel.Find(gradeBookModel.class_Id);
            gradeBookModel.task_Id = Convert.ToInt32(gradeBookModel.task_Id);
            gradeBookModel.TaskModel = db.taskModel.Find(gradeBookModel.task_Id);
            ViewBag.Student = gradeBookModel.StudentModels.fName + " " + gradeBookModel.StudentModels.lName;
            if (ModelState.IsValid)
            {
                if (gradeBookModel.TaskModel.points < gradeBookModel.pointsEarned) { gradeBookModel.pointsEarned = gradeBookModel.TaskModel.points; }
                if (gradeBookModel.pointsEarned < 0) { gradeBookModel.pointsEarned = 0; }
                //db.Entry(gradeBookModel).State = EntityState.Modified;
                var grade = db.gradeBookModel.Find(gradeBookModel.grade_Id);
                db.gradeBookModel.Remove(grade);
                db.gradeBookModel.Add(gradeBookModel);
                db.SaveChanges();
                return RedirectToAction("Index", "GradeBook" ,gradeBookModel.class_Id);
            }
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
