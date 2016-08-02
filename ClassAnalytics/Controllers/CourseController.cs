using System;
using System.Net;
using System.Web;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using ClassAnalytics.Models;
using ClassAnalytics.Models.Misc_Models;
using ClassAnalytics.Models.Program_Models;


namespace ClassAnalytics.Controllers
{
    public class CourseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult new_Task(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Create/" + id, "Task");
        }

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
            List<CourseModels> courses = db.coursemodels.ToList();
            List<CourseModels> new_courses = new List<CourseModels>();
            ViewBag.program_id = new SelectList(db.programModels, "program_Id", "programName");
            if(id != null)
            {
                ProgramModels program = db.programModels.Find(id);
                ViewBag.program = program.programName;
                ViewBag.date = program.startDate + " - " + program.endDate;
                foreach (CourseModels course in courses)
                {
                    if (course.program_Id == id)
                    {
                        course.programModels = program;
                        new_courses.Add(course);
                    }
                }
                ViewBag.prog_id = id;
                return View(new_courses);
            }
            else
            {
                return RedirectToAction("Index","ProgramModels");
            }  
        }

        // GET: Course/Create
        public ActionResult Create(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if(id == null)
            {
                return RedirectToAction("Index", "ProgramModels");
            }
            ProgramModels program = db.programModels.Find(id);
            ViewBag.program = program.programName;
            ViewBag.date = program.startDate + " - " + program.endDate;
            CourseModels course = new CourseModels();
            course.programModels = program;
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
            ProgramModels program = db.programModels.Find(course.program_Id);
            ViewBag.program = program.programName;
            ViewBag.date = program.startDate + " - " + program.endDate;
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
            courseModels.programModels = db.programModels.Find(courseModels.program_Id);
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
            courseModels.programModels = db.programModels.Find(courseModels.program_Id);
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
