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
        public ActionResult Index(int? program_id)
        {
            ViewBag.program_id = new SelectList(db.programModels, "program_Id", "programName");
            if(program_id != null)
            {
                List<ProgCourseViewModel> viewModelList = new List<ProgCourseViewModel>();
                var courses = db.coursemodels.ToList();
                foreach (CourseModels course in courses)
                {
                    if(course.program_Id == program_id)
                    {
                        ProgCourseViewModel viewModel = new ProgCourseViewModel();
                        viewModel.course_Id = course.course_Id;
                        viewModel.courseName = course.courseName;
                        viewModel.startDate = course.startDate;
                        viewModel.endDate = course.endDate;
                        viewModel.ProgramModels = db.programModels.Find(course.program_Id);
                        viewModelList.Add(viewModel);
                    }                    
                }
                return View(viewModelList);
            }
            else
            {
                List<ProgCourseViewModel> viewModel = new List<ProgCourseViewModel>();
                return View(viewModel);
            }  
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

            var programs = db.programModels.ToList();
            List<SelectListItem> program_list = new List<SelectListItem>();
            ProgCourseViewModel viewModel = new ProgCourseViewModel();
            foreach(ProgramModels program in programs)
            {
                program_list.Add(new SelectListItem() { Text = program.programName, Value = program.program_Id.ToString() });
            }
            viewModel.programs = program_list;
            return View(viewModel);
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProgCourseViewModel viewModel)
        {
            CourseModels courseModels = new CourseModels();
            if (ModelState.IsValid)
            {
                int program_id = Convert.ToInt32(viewModel.program_Id);
                courseModels.courseName = viewModel.courseName;
                courseModels.course_Id = viewModel.course_Id;
                courseModels.startDate = viewModel.startDate;
                courseModels.endDate = viewModel.endDate;
                courseModels.program_Id = program_id;
                courseModels.ProgramModels = db.programModels.Find(program_id);
                db.coursemodels.Add(courseModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var programs = db.programModels.ToList();
            List<SelectListItem> program_list = new List<SelectListItem>();
            foreach (ProgramModels program in programs)
            {
                program_list.Add(new SelectListItem() { Text = program.programName, Value = program.program_Id.ToString() });
            }
            viewModel.programs = program_list;

            return View(viewModel);
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
