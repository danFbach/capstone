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
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<MessagingModel> new_messages = new List<MessagingModel>();
            MessagingModel this_message;

            foreach (MessagingModel message in messages)
            {
                this_message = new MessagingModel();
                if(message.recieve_id == UserId)
                {
                    new_messages.Add(this_message);
                }
            }
            return View("Messages", new_messages);
        }
        public ActionResult Sent()
        {
            var messages = db.messagingModel.ToList();
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<MessagingModel> new_messages = new List<MessagingModel>();
            MessagingModel this_message;

            foreach (MessagingModel message in messages)
            {
                this_message = new MessagingModel();
                if (message.sending_id == UserId)
                {
                    new_messages.Add(this_message);
                }
            }
            return View("Messages", new_messages);
        }

        // GET: Messaging/Details/5
        public ActionResult Details(int? id)
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

        // GET: Messaging/Create
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Messaging/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MessagingModel messagingModel)
        {
            if (ModelState.IsValid)
            {
                db.messagingModel.Add(messagingModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(messagingModel);
        }

        // GET: Messaging/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Messaging/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MessagingModel messagingModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(messagingModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(messagingModel);
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
