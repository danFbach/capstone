﻿using System;
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
    public class ClassController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Class
        public ActionResult classGrades(int? id)
        {
            if(id != null)
            {
                return RedirectToAction("Index/" + id, "GradeBook");
            }
            else
            {
                return View("Index");
            }
        }
        public ActionResult Index(int? program_id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.program_id = new SelectList(db.programModels, "program_Id", "programName");
            var classes = db.classmodel.ToList();
            var programs = db.programModels.ToList();
            List<ProgClassViewModel> new_list = new List<ProgClassViewModel>();
            List<ProgramModels> progList = new List<ProgramModels>();
            if(program_id!=null)
            {
                foreach (ClassModel a_class in classes)
                {
                    if (a_class.program_id == program_id)
                    {
                        new_list.Add(new ProgClassViewModel() { class_Id = a_class.class_Id, className = a_class.className, program_id = a_class.program_id, ProgramModels = db.programModels.Find(a_class.program_id) });
                    }
                }
            }
            else
            {
                foreach (ClassModel a_class in classes)
                {
                    new_list.Add(new ProgClassViewModel() { class_Id = a_class.class_Id, className = a_class.className, program_id = a_class.program_id, ProgramModels = db.programModels.Find(a_class.program_id) });
                }
            }
            return View(new_list);
        }

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
                db.classmodel.Add(new_class);
                db.SaveChanges();
                return RedirectToAction("Index", "Students");
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
