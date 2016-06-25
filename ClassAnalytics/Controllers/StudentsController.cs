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
        public ActionResult Index(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id != null)
            {
                ViewBag.this_class = db.classmodel.Find(id).className;
                List<StudentModels> students = db.studentModels.ToList();
                List<StudentModels> studentModels = new List<StudentModels>();

                foreach (StudentModels student in students)
                {
                    if (student.class_Id == id)
                    {
                        studentModels.Add(student);
                    }
                }
                return View(studentModels);
            }
            else
            {
                return RedirectToAction("Index","Class");
            }            
        }
        // GET: Students/Create
        public ActionResult Create(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id != null)
            {
                ClassStudentViewModel viewModel = new ClassStudentViewModel();
                viewModel.class_Id = Convert.ToInt32(id);
                viewModel.ClassModel = db.classmodel.Find(id);
                ViewBag.StatusMessage = "";
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Class");
            } 
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
            if (ModelState.IsValid)
            {
                StudentModels student = new StudentModels();
                student.ClassModel = db.classmodel.Find(viewModel.class_Id);
                student.ClassModel.ProgramModels = db.programModels.Find(student.ClassModel.program_id);
                student.student_Id = viewModel.student_Id;
                student.fName = viewModel.fName;
                student.lName = viewModel.lName;
                RegisterViewModel newUser = new RegisterViewModel();
                newUser.Email = viewModel.newEmail;
                newUser.Password = "R3$et_this";
                newUser.ConfirmPassword = "R3$et_this";
                newUser.ConfirmPassword = newUser.Password;
                string username = makeUserName(student.fName,student.lName,0);
                var user = new ApplicationUser { UserName = username, Email = newUser.Email };
                var result = await UserManager.CreateAsync(user, newUser.Password);
                if (result.Succeeded)
                {
                    student.student_account_Id = user.Id;
                    addRole(user);
                    studentConfirmationEmail(user.Email, newUser.Password, student.fName, student.lName, user.UserName);
                    welcomeMessage(student, user);
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
                    return RedirectToAction("Index", "Class");
                }
                viewModel.ClassModel = db.classmodel.Find(viewModel.class_Id);
                ViewBag.StatusMessage = "Email is already in use or Invalid.";
                return View(viewModel);
            }
            viewModel.ClassModel = db.classmodel.Find(viewModel.class_Id);
            return View(viewModel);
        }
        public void addRole(ApplicationUser user)
        {
            IdentityUserRole role = new IdentityUserRole();
            role.UserId = user.Id;
            role.RoleId = "New Student";
            UserManager.AddToRole(role.UserId, role.RoleId);
        }
        public void welcomeMessage(StudentModels student, ApplicationUser user)
        {
            MessagingModel message = new MessagingModel();
            message.subject = "Welcome to Edulytics " + student.fName + "!";
            message.message = "Welcome " + student.fName + "! This is your direct message section which allows you to privately message anyone in your school with just the click of a mouse. You're in the \"" + student.ClassModel.className + "\" class which is part of the " + student.ClassModel.ProgramModels.programName + " program. Have a great start to this new program and once again welcome from all of us here at Edulytics.";
            message.recieve_id = user.Id;
            message.sending_id = this.User.Identity.GetUserId();
            message.dateSent = DateTime.Now;
            db.messagingModel.Add(message);
            db.studentModels.Add(student);
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
