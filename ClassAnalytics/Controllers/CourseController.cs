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


        public ActionResult task_Index(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index/" + id, "Task");

        }
        // GET: Course
        public ActionResult Index(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ProgramModels program = db.programModels.Find(id);
            List<CourseModels> courses = db.coursemodels.ToList();
            List<CourseModels> new_courses = new List<CourseModels>();
            ViewBag.program_id = new SelectList(db.programModels, "program_Id", "programName");
            if(id != null)
            {
                foreach (CourseModels course in courses)
                {
                    if (course.program_Id == id)
                    {
                        course.ProgramModels = program;
                        new_courses.Add(course);
                    }
                }              
                return View(new_courses);
            }
            else
            {
                return View(db.coursemodels.ToList());
            }  
        }

        // GET: Course/Create
        public ActionResult Create(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ProgramModels program = db.programModels.Find(id);
            CourseModels course = new CourseModels();
            course.ProgramModels = program;
            course.program_Id = program.program_Id;

            return View(course);
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseModels course)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                int id = 0;
                id = course.program_Id;
                db.coursemodels.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index/" + id);
            }
            var programs = db.programModels.ToList();
            List<SelectListItem> program_list = new List<SelectListItem>();
            foreach (ProgramModels program in programs)
            {
                program_list.Add(new SelectListItem() { Text = program.programName, Value = program.program_Id.ToString() });
            }
            return View(course);
        }

        // GET: Course/Edit/5
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
            CourseModels courseModels = db.coursemodels.Find(id);
            courseModels.ProgramModels = db.programModels.Find(courseModels.program_Id);
            ViewBag.program_id = new SelectList(db.programModels, "program_Id", "programName");
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
        public ActionResult Edit(CourseModels courseModels)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseModels courseModels = db.coursemodels.Find(id);
            courseModels.ProgramModels = db.programModels.Find(courseModels.program_Id);
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
