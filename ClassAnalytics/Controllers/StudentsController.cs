using System;
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
        

        public async Task studentConfirmationEmail(string emailaddress, string Password, string fName, string lName,string username)
        {

            var myMessage = new SendGridMessage();
            string resetURL = "http://localhost:5753/Account/Login";
            string a_message = "Hey " + fName + " " + lName + ", your account for school has been created. <br><br>Your login is: <b>" + username + "</b><br>Your Password is: <b>" + Password + "</b><br><br>Activate your account and reset your password <a href=\"" +  resetURL + "\">Here!</a>";
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
        // GET: Students/Create
        public ActionResult Create()
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            RegisterViewModel model = new RegisterViewModel();            
            StudentModels student = new StudentModels();
            List<ClassModel> classList = db.classmodel.ToList();
            List<ProgramModels> programList = db.programModels.ToList();
            List<SelectListItem> a_classList = new List<SelectListItem>();
            List<SelectListItem> a_programList = new List<SelectListItem>();

            if (ModelState.IsValid)
            {
                student.ClassModel = db.classmodel.Find(viewModel.class_Id);
                student.ClassModel.ProgramModels = db.programModels.Find(student.ClassModel.program_id);
                
                


                IdentityUserRole role = new IdentityUserRole();
                model.Email = viewModel.newEmail;
                model.Password = "R3$et_this";
                model.ConfirmPassword = "R3$et_this";
                model.ConfirmPassword = model.Password;
                student.student_Id = viewModel.student_Id;
                student.fName = viewModel.fName;
                student.lName = viewModel.lName;
                string username = makeUserName(student.fName,student.lName,0);
                var user = new ApplicationUser { UserName = username, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    role.UserId = user.Id;
                    role.RoleId = "New Student";
                    ViewBag.StatusMessage = "";
                    UserManager.AddToRole(role.UserId, role.RoleId);
                    student.student_account_Id = user.Id;
    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    studentConfirmationEmail(user.Email, model.Password, student.fName, student.lName, user.UserName);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    MessagingModel message = new MessagingModel();
                    message.subject = "Welcome to Edulytics " + student.fName + "!";
                    message.message = "Welcome " + student.fName + "! This is your direct message section which allows you to privately message anyone in your school with just the click of a mouse. You're in the \"" + student.ClassModel.className + "\" class which is part of the " + student.ClassModel.ProgramModels.programName + " program. Have a great start to this new program and once again welcome from all of us here at Edulytics.";
                    message.recieve_id = user.Id;
                    message.sending_id = this.User.Identity.GetUserId();
                    message.dateSent = DateTime.Now;
                    db.messagingModel.Add(message);
                    db.studentModels.Add(student);

                    List<TaskModel> tasks = db.taskModel.ToList();
                    foreach (TaskModel task in tasks)
                    {
                        task.CourseModels = db.coursemodels.Find(task.course_Id);
                        GradeBookModel grade = new GradeBookModel();
                        if (task.CourseModels.program_Id == student.ClassModel.program_id)
                        {
                            grade.class_Id = student.class_Id;
                            grade.pointsEarned = null;
                            grade.possiblePoints = task.points;
                            grade.student_Id = student.student_Id;
                            grade.task_Id = task.task_Id;
                            db.gradeBookModel.Add(grade);
                        }
                    }

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

        public string makeUserName(string firstName, string lastName, int count)
        {
            string username;
            if (count == 0)
            {
                username = firstName.ToLower() + lastName;
            }
            else
            {
                username = firstName.ToLower() + count.ToString() + lastName; 
            }
            ApplicationUser this_user = UserManager.FindByName(username);
            if (this_user != null)
            {
                count += 1;
                return makeUserName(firstName, lastName, count);
            }
            else
            {
                return username;
            }
        }

        // GET: Students/Edit/5
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
