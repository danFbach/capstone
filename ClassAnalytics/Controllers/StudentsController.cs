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
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Students
        public ActionResult Index()
        {
            List<StudentModels> students = db.studentModels.ToList();
            List<ClassStudentViewModel> viewModel = new List<ClassStudentViewModel>();

            foreach(StudentModels student in students)
            {
                viewModel.Add(new ClassStudentViewModel() {student_Id = student.student_Id, fName = student.fName, lName = student.lName, ClassModel = db.classmodel.Find(student.class_Id)});
            }            

            return View(viewModel);
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentModels studentModels = db.studentModels.Find(id);
            if (studentModels == null)
            {
                return HttpNotFound();
            }
            return View(studentModels);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            List<ClassModel> classes = db.classmodel.ToList();
            List<ProgramModels> programs = db.programModels.ToList();
            List<SelectListItem> a_classList = new List<SelectListItem>();
            List<SelectListItem> a_programList = new List<SelectListItem>();
            ClassStudentViewModel viewModel = new ClassStudentViewModel();

            foreach (ClassModel a_class in classes)
            {
                a_classList.Add(new SelectListItem() { Text = a_class.className, Value = a_class.class_Id.ToString() });
            }
            foreach(ProgramModels program in programs)
            {
                a_programList.Add(new SelectListItem() { Text = program.programName, Value = program.program_Id.ToString() });
            }
            viewModel.classList = a_classList;
            viewModel.programList = a_programList;

            return View(viewModel);
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassStudentViewModel viewModel)
        {
            StudentModels student = new StudentModels();
            List<ClassModel> classList = db.classmodel.ToList();
            List<ProgramModels> programList = db.programModels.ToList();

            if (ModelState.IsValid)
            {
                foreach(ClassModel classs in classList)
                {
                    if(viewModel.ClassModel.class_Id == classs.class_Id)
                    {
                        student.ClassModel = classs;
                    }                    
                }
                student.student_Id = viewModel.student_Id;
                student.fName = viewModel.fName;
                student.lName = viewModel.lName;
                db.studentModels.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentModels studentModels = db.studentModels.Find(id);
            if (studentModels == null)
            {
                return HttpNotFound();
            }
            return View(studentModels);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,fName,lName")] StudentModels studentModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(studentModels);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentModels studentModels = db.studentModels.Find(id);
            if (studentModels == null)
            {
                return HttpNotFound();
            }
            return View(studentModels);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentModels studentModels = db.studentModels.Find(id);
            db.studentModels.Remove(studentModels);
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
