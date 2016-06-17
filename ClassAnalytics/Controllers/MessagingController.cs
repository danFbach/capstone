using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClassAnalytics.Models;
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
            List<MessagingModel> new_messages = new List<MessagingModel>();
            MessagingModel this_message;

            foreach (MessagingModel message in messages)
            {
                
                this_message = new MessagingModel();
                if(message.recieve_id == UserId)
                {
                    this_message = message;
                    this_message.sending_User = db.Users.Find(message.sending_id);
                    new_messages.Add(this_message);
                }
            }
            return View(new_messages);
        }
        public ActionResult Sent()
        {
            var messages = db.messagingModel.ToList();
            messages = messages.OrderByDescending(x => x.dateSent).ToList();
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<MessagingModel> new_messages = new List<MessagingModel>();
            MessagingModel this_message;

            foreach (MessagingModel message in messages)
            {
                this_message = new MessagingModel();
                if (message.sending_id == UserId)
                {
                    this_message = message;
                    this_message.receiving_User = db.Users.Find(message.recieve_id);
                    new_messages.Add(this_message);
                }
            }
            return View(new_messages);
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
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (message.recieve_id == UserId)
            {
                message.read = true;
            }
            message.receiving_User = db.Users.Find(message.recieve_id);
            message.sending_User = db.Users.Find(message.sending_id);
            db.SaveChanges();
            return View(message);
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
            message.recieve_id = viewModel.Message.recieve_id;
            message.receiving_User = db.Users.Find(message.recieve_id);
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
