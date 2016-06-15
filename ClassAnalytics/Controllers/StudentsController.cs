﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Net.Mail;
using SendGrid;
using System.Web.Mvc;
using ClassAnalytics.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace ClassAnalytics.Controllers
{
    public class StudentsController : Controller
    {
        
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        

        public async Task studentConfirmation(string emailaddress, string Password, string fName, string lName)
        {

            var myMessage = new SendGridMessage();
            string resetURL = "http://localhost:5753/Account/Login";
            string a_message = "Hey " + fName + " " + lName + ", your account for school has been created. <br><br>Your login is: <b>" + emailaddress + "</b><br>Your Password is: <b>" + Password + "</b><br><br>Acvtivate your account and reset your password <a href=\"" +  resetURL + "\">Here!</a>";
            myMessage.Html = a_message;
            myMessage.From = new MailAddress("no-reply@devHax.prod", "Edulytics Account Services");
            myMessage.AddTo(emailaddress);
            myMessage.Subject = "Hey " + fName + "! Edulytics Account Activation Enclosed";
            var credentials = new NetworkCredential("quikdevstudent", "Lexusi$3");
            var transportWeb = new Web(credentials);
            await transportWeb.DeliverAsync(myMessage);
        }

        public void AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Students
        public ActionResult Index(int? class_id)
        {
            ViewBag.class_id = new SelectList(db.classmodel, "class_Id", "className");
            if (class_id != null)
            {
                List<StudentModels> students = db.studentModels.ToList();
                List<ClassStudentViewModel> viewModel = new List<ClassStudentViewModel>();

                foreach (StudentModels student in students)
                {
                    if (student.class_Id == class_id)
                    {
                        viewModel.Add(new ClassStudentViewModel() { student_Id = student.student_Id, fName = student.fName, lName = student.lName, ClassModel = db.classmodel.Find(student.class_Id) });
                    }
                }
                return View(viewModel);
            }
            else
            {
                List<ClassStudentViewModel> viewModel = new List<ClassStudentViewModel>();
                return View(viewModel);
            }            
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
            ClassStudentViewModel viewModel = new ClassStudentViewModel();
            List<ClassModel> classes = db.classmodel.ToList();
            List<ProgramModels> programs = db.programModels.ToList();
            List<SelectListItem> a_classList = new List<SelectListItem>();
            List<SelectListItem> a_programList = new List<SelectListItem>();

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

            ViewBag.StatusMessage = "";
            return View(viewModel);
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ClassStudentViewModel viewModel)
        {
            RegisterViewModel model = new RegisterViewModel();
            
            StudentModels student = new StudentModels();
            List<ClassModel> classList = db.classmodel.ToList();
            List<ProgramModels> programList = db.programModels.ToList();
            List<SelectListItem> a_classList = new List<SelectListItem>();
            List<SelectListItem> a_programList = new List<SelectListItem>();

            if (ModelState.IsValid)
            {
                foreach(ClassModel a_class in classList)
                {
                    if(viewModel.class_Id == a_class.class_Id)
                    {
                        student.ClassModel = a_class;
                    }
                }
                IdentityUserRole role = new IdentityUserRole();
                model.Email = viewModel.newEmail;
                model.Password = "D3v$tudent";
                model.ConfirmPassword = "D3v$tudent";
                model.ConfirmPassword = model.Password;
                string username = viewModel.fName.ToCharArray()[0].ToString().ToLower() + viewModel.lName;
                student.student_Id = viewModel.student_Id;
                student.fName = viewModel.fName;
                student.lName = viewModel.lName;
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    role.UserId = user.Id;
                    role.RoleId = "New Student";
                    ViewBag.StatusMessage = "";
                    UserManager.AddToRole(role.UserId, role.RoleId);
                    student.student_account_Id = user.Id;
    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    studentConfirmation(model.Email, model.Password, student.fName, student.lName);
    #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    db.studentModels.Add(student);
                    db.SaveChanges();
                
                    return RedirectToAction("Index");
                }
            }

            foreach (ClassModel a_class in classList)
            {
                a_classList.Add(new SelectListItem() { Text = a_class.className, Value = a_class.class_Id.ToString() });
            }
            foreach (ProgramModels program in programList)
            {
                a_programList.Add(new SelectListItem() { Text = program.programName, Value = program.program_Id.ToString() });
            }
            viewModel.classList = a_classList;
            viewModel.programList = a_programList;
            ViewBag.StatusMessage = "Email is already in use or Invalid.";
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
            ViewBag.class_id = new SelectList(db.classmodel, "class_Id", "className");
            return View(studentModels);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentModels studentModels)
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
