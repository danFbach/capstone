using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Data;
using System.Threading.Tasks;
using SendGrid;
using ClassAnalytics.Models;
using ClassAnalytics.Models.Misc_Models;
using ClassAnalytics.Models.Class_Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Net.Mail;

namespace ClassAnalytics.Controllers
{
    public class CommunicationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        [HttpPost]
        public ActionResult InstToStudent(int student_id,string message)
        {
            string instructor = "";
            string email = "";
            var this_student = db.studentModels.Find(student_id);
            var users = db.Users.ToList();

            foreach (var user in users)
            {
                if (user.Id == this_student.student_account_Id)
                {
                    email = user.Email;
                }
                if (user.Email == this.User.Identity.Name)
                {
                    instructor = user.Email;
                }
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            message_student(null, email, instructor, message);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            return Redirect("AdminMessaging");
        }
        public ActionResult InstToClass()
        {
            ViewBag.class_id = new SelectList(db.classmodel,"class_Id","className");
            return View();
        }
        [HttpPost]
        public ActionResult InstToClass(int? class_id, string message)
        {
            string instructor = "";
            List<string> emails = new List<string>();
            ClassModel a_class = db.classmodel.Find(class_id);
            var students = db.studentModels.ToList();
            List<StudentModels> current_students = new List<StudentModels>();
            var users = db.Users.ToList();
            
            foreach(var user in users)
            {
                if(user.Email == this.User.Identity.Name)
                {
                    instructor = user.Email;
                }
                foreach(StudentModels student in students)
                {
                    if(student.class_Id == class_id)
                    {
                        if (user.Id == student.student_account_Id)
                        {
                            emails.Add(user.Email);
                        }
                    }
                }
            }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            message_student(emails, null, instructor, message);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            return RedirectToAction("AdminMessaging");
        }
        public async Task message_student(List<string> addresses, string email, string instructorName,string message)
        {

            var myMessage = new SendGridMessage();
            if(email == null)
            {
                myMessage.AddTo(addresses);
            }
            else
            {
                myMessage.AddTo(email);
            }
            myMessage.Text = "The message that is being sent is: " + message + "...Html can also be added to make the emails look nicer.";
            myMessage.From = new MailAddress("no-reply@devHax.prod", "Edulytics Account Services");
            myMessage.Subject = "New Edulytics Message From " + instructorName;
            var credentials = new NetworkCredential("quikdevstudent", "Lexusi$3");
            var transportWeb = new Web(credentials);
            await transportWeb.DeliverAsync(myMessage);
        }
        public ActionResult AdminMessaging(int? class_id)
        {
            if(class_id == null)
            {
                ViewBag.class_id = new SelectList(db.classmodel, "class_Id", "className");
                return View();
            }else
            {
                StudentEmail message = new StudentEmail();
                var students = db.studentModels.ToList();
                message.students = new List<SelectListItem>();
                foreach (StudentModels student in students)
                {
                    string name = "";
                    if (student.class_Id == class_id)
                    {
                        name = student.lName + ", " + student.fName;
                        message.students.Add(new SelectListItem() { Text = name, Value = student.student_Id.ToString() });
                    }
                }
                return View("InstToStudent",message);
            }
        }
        public ActionResult StudentToInstructor()
        {
            return View();
        }
        public ActionResult StudentToStudent()
        {
            return View();
        }
    }
}
