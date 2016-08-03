using System;
using System.Web;
using System.Net;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using ClassAnalytics.Models;
using ClassAnalytics.Models.Misc_Models;
using ClassAnalytics.Models.Class_Models;
using ClassAnalytics.Models.Instructor_Models;
using Microsoft.AspNet.Identity;


namespace ClassAnalytics.Controllers
{
    public class MessagingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Messaging
        public ActionResult Inbox()
        {
            var messages = db.messagingModel.ToList();
            messages = messages.OrderByDescending(x => x.dateSent).ToList();
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<StudentModels> students = db.studentModels.ToList();
            List<InstructorModel> instructors = db.instructorModel.ToList();
            List<MessagingViewModel> viewModel = new List<MessagingViewModel>();


            foreach (MessagingModel message in messages)
            {
                MessagingViewModel this_message = new MessagingViewModel();
                if (message.recieving_id == UserId)
                {
                    foreach(StudentModels student in students)
                    {
                        if(student.student_account_Id == message.sending_id)
                        {
                            this_message.sender = student.lName + ", " + student.fName;

                        }
                        else if(student.student_account_Id == message.recieving_id)
                        {
                            this_message.recip = student.lName + ", " + student.fName;
                        }
                    }
                    foreach(InstructorModel instructor in instructors)
                    {
                        if(instructor.instructor_account_Id == message.sending_id)
                        {
                            this_message.sender = instructor.lName + ", " + instructor.fName;
                        }
                        else if(instructor.instructor_account_Id == message.recieving_id)
                        {
                            this_message.recip = instructor.lName + ", " + instructor.fName;
                        }
                    }

                    this_message.Message = message;
                    viewModel.Add(this_message);
                }
            }
            return View(viewModel);
        }
        public ActionResult Sent()
        {
            var messages = db.messagingModel.ToList();
            messages = messages.OrderByDescending(x => x.dateSent).ToList();
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<StudentModels> students = db.studentModels.ToList();
            List<InstructorModel> instructors = db.instructorModel.ToList();
            List<MessagingViewModel> viewModel = new List<MessagingViewModel>();

            foreach (MessagingModel message in messages)
            {
                MessagingViewModel this_message = new MessagingViewModel();
                if (message.sending_id == UserId)
                {
                    foreach (StudentModels student in students)
                    {
                        if (student.student_account_Id == message.sending_id)
                        {
                            this_message.sender = student.lName + ", " + student.fName;

                        }
                        else if (student.student_account_Id == message.recieving_id)
                        {
                            this_message.recip = student.lName + ", " + student.fName;
                        }
                    }
                    foreach (InstructorModel instructor in instructors)
                    {
                        if (instructor.instructor_account_Id == message.sending_id)
                        {
                            this_message.sender = instructor.lName + ", " + instructor.fName;
                        }
                        else if (instructor.instructor_account_Id == message.recieving_id)
                        {
                            this_message.recip = instructor.lName + ", " + instructor.fName;
                        }
                    }

                    this_message.Message = message;
                    viewModel.Add(this_message);
                }
            }
            return View(viewModel);
        }

        // GET: Messaging/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MessagingModel message = db.messagingModel.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            MessagingViewModel viewModel = new MessagingViewModel();
            List<StudentModels> students = db.studentModels.ToList();
            List<InstructorModel> instructors = db.instructorModel.ToList();
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (message.recieving_id == UserId)
            {
                message.read = true;
            }
            foreach(StudentModels student in students)
            {
                string name = student.fName + " " + student.lName;
                if (student.student_account_Id == message.recieving_id)
                {
                    viewModel.recip = name;
                }
                if (student.student_account_Id == message.sending_id)
                {
                    viewModel.sender = name;
                }
            }
            foreach(InstructorModel instructor in instructors)
            {
                string name = instructor.fName + " " + instructor.lName;
                if (instructor.instructor_account_Id == message.recieving_id)
                {
                    viewModel.recip = name;
                }
                if(instructor.instructor_account_Id == message.sending_id)
                {
                    viewModel.sender = name;
                }
            }
            message.receiving_User = db.Users.Find(message.recieving_id);
            message.sending_User = db.Users.Find(message.sending_id);
            viewModel.Message = message;
            db.SaveChanges();
            return View(viewModel);
        }

        // GET: Messaging/Create
        public ActionResult Create()
        {
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            MessagingViewModel viewModel = new MessagingViewModel();
            viewModel.recipients = new List<SelectListItem>();
            var instructors = db.instructorModel.ToList();
            var students = db.studentModels.ToList();

            foreach(InstructorModel instructor in instructors)
            {
                if(instructor.instructor_account_Id != UserId)
                {
                    viewModel.recipients.Add(new SelectListItem() { Text = "Instructor: " + instructor.lName + ", " + instructor.fName, Value = instructor.instructor_account_Id });
                }
            }
            foreach(StudentModels student in students)
            {
                if(student.student_account_Id != UserId)
                {
                    viewModel.recipients.Add(new SelectListItem() { Text = "Student: " + student.lName + ", " + student.fName, Value = student.student_account_Id });
                }
            }
            return View(viewModel);
        }

        // POST: Messaging/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MessagingViewModel viewModel)
        {
            MessagingModel message = new MessagingModel();
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            message.subject = viewModel.Message.subject;
            message.message = viewModel.Message.message;
            message.recieving_id = viewModel.Message.recieving_id;
            message.receiving_User = db.Users.Find(message.recieving_id);
            message.sending_id = UserId;
            message.sending_User = db.Users.Find(message.sending_id);
            message.dateSent = DateTime.Now;
            message.read = false;

            if (ModelState.IsValid)
            {
                db.messagingModel.Add(message);
                db.SaveChanges();
                return RedirectToAction("Inbox");
            }
            viewModel.recipients = new List<SelectListItem>();
            var instructors = db.instructorModel.ToList();
            var students = db.studentModels.ToList();

            foreach (InstructorModel instructor in instructors)
            {
                viewModel.recipients.Add(new SelectListItem() { Text = "Instructor: " + instructor.lName + ", " + instructor.fName, Value = instructor.instructor_account_Id });
            }
            foreach (StudentModels student in students)
            {
                viewModel.recipients.Add(new SelectListItem() { Text = "Student: " + student.lName + ", " + student.fName, Value = student.student_account_Id });
            }

            return View(viewModel);
        }
        
        // GET: Messaging/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MessagingModel messagingModel = db.messagingModel.Find(id);
            if (messagingModel == null)
            {
                return HttpNotFound();
            }
            return View(messagingModel);
        }

        // POST: Messaging/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MessagingModel messagingModel = db.messagingModel.Find(id);
            db.messagingModel.Remove(messagingModel);
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
