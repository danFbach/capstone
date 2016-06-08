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
    public class UnitController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Unit
        public ActionResult Index()
        {
            var units = db.unitModels.ToList();
            var courses = db.coursemodels.ToList();
            List<UnitCourseViewModel> viewModelList = new List<UnitCourseViewModel>();

            foreach(UnitModels unit in units)
            {
                UnitCourseViewModel viewModel = new UnitCourseViewModel();
                viewModel.unit_Id = Convert.ToInt32(unit.unit_Id);
                viewModel.unitName = unit.unitName;
                viewModel.startDate = unit.startDate;
                viewModel.endDate = unit.endDate;
                foreach (CourseModels course in courses)
                {
                    if(unit.course_Id == course.course_Id)
                    {
                        viewModel.CourseModels = db.coursemodels.Find(unit.course_Id);
                        viewModel.course_Id = unit.course_Id;
                    }
                }
                viewModelList.Add(viewModel);
            }

            return View(viewModelList);
        }

        // GET: Unit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitModels unitModels = db.unitModels.Find(id);
            if (unitModels == null)
            {
                return HttpNotFound();
            }
            return View(unitModels);
        }

        // GET: Unit/Create
        public ActionResult Create()
        {
            UnitCourseViewModel viewModel = new UnitCourseViewModel();
            List<SelectListItem> course_list = new List<SelectListItem>();
            var courses = db.coursemodels.ToList();
            foreach(CourseModels course in courses)
            {
                course_list.Add(new SelectListItem() { Text = course.courseName, Value = course.course_Id.ToString() });
            }
            viewModel.course_list = course_list;
            return View(viewModel);
        }

        // POST: Unit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UnitCourseViewModel viewModel)
        {
            UnitModels unitModels = new UnitModels();
            if (ModelState.IsValid)
            {
                unitModels.unit_Id = viewModel.unit_Id;
                unitModels.unitName = viewModel.unitName;
                unitModels.startDate = viewModel.startDate;
                unitModels.endDate = viewModel.endDate;
                unitModels.course_Id = Convert.ToInt32(viewModel.course_Id);

                db.unitModels.Add(unitModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // GET: Unit/Edit/5
        public ActionResult Edit(int? id)
        {
            var courses = db.coursemodels.ToList();
            UnitCourseViewModel viewModel = new UnitCourseViewModel();
            List<SelectListItem> course_list = new List<SelectListItem>();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitModels unitModels = db.unitModels.Find(id);
            
            viewModel.unit_Id = Convert.ToInt32(unitModels.unit_Id);
            viewModel.unitName = unitModels.unitName;
            viewModel.startDate = unitModels.startDate;
            viewModel.endDate = unitModels.endDate;
            foreach (CourseModels course in courses)
            {
            course_list.Add(new SelectListItem() { Text = course.courseName, Value = course.course_Id.ToString() });
                if (unitModels.course_Id == course.course_Id)
                {
                    viewModel.CourseModels = db.coursemodels.Find(unitModels.course_Id);
                    viewModel.course_Id = unitModels.course_Id;
                }
            }
            viewModel.course_list = course_list;
            if (unitModels == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        // POST: Unit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UnitCourseViewModel viewModel)
        {
            UnitModels unitModels = new UnitModels();
            if (ModelState.IsValid)
            {
                unitModels.unit_Id = viewModel.unit_Id;
                unitModels.unitName = viewModel.unitName;
                unitModels.startDate = viewModel.startDate;
                unitModels.endDate = viewModel.endDate;
                unitModels.course_Id = Convert.ToInt32(viewModel.course_Id);

                db.Entry(unitModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: Unit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitModels unitModels = db.unitModels.Find(id);
            if (unitModels == null)
            {
                return HttpNotFound();
            }
            return View(unitModels);
        }

        // POST: Unit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UnitModels unitModels = db.unitModels.Find(id);
            db.unitModels.Remove(unitModels);
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
