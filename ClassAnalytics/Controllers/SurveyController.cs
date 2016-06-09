
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
    public class SurveyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Activate(int id)
        {
            SurveyJoinTableModel joinSurvey = db.surveyJoinTableModel.Find(id);
            joinSurvey.active = true;
            db.SaveChanges();

            return RedirectToAction("Index_Class_Survey");
        }
        public ActionResult Deactivate(int id)
        {
            SurveyJoinTableModel joinSurvey = db.surveyJoinTableModel.Find(id);
            joinSurvey.active = false;
            db.SaveChanges();
            return RedirectToAction("Index_Class_Survey");
        }

        public ActionResult Index_Class_Survey()
        {
            return View(db.surveyJoinTableModel.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Join(int id)
        {
            SurveyJoinTableModel survey = new SurveyJoinTableModel();
            SurveyModel a_survey = db.surveyModel.Find(id);
            ViewBag.class_Id = new SelectList(db.classmodel,"class_Id","className");
            survey.SurveyModel = a_survey;
            return View(survey);
        }
        public ActionResult Create_Join(SurveyJoinTableModel survey)
        {
            if (ModelState.IsValid)
            {
                db.surveyJoinTableModel.Add(survey);
                db.SaveChanges();
                return RedirectToAction("Index_Class_Survey");
            }
            SurveyModel a_survey = db.surveyModel.Find(survey.survey_Id);
            ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
            survey.SurveyModel = a_survey;
            return View(survey);
        }


        public ActionResult Create_Question(int id)
        {
            SurveyQuestion surveyQuestion = new SurveyQuestion();
            string info = db.surveyModel.Find(id).SurveyName;
            ViewBag.surveyInfo = "Name: " + info + " #" + id;
            surveyQuestion.survey_Id = id;
            return View(surveyQuestion);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Question(SurveyQuestion surveyQuestion)
        {
            if (ModelState.IsValid)
            {
                db.surveyQuestion.Add(surveyQuestion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            int id = surveyQuestion.survey_Id;
            string info = db.surveyModel.Find(id).SurveyName;
            ViewBag.surveyInfo = "Name: " + info + " #" + id;
            surveyQuestion.survey_Id = id;
            return View();
        }
        // GET: Survey
        public ActionResult Index()
        {
            List<SurveyModel> survey_list = new List<SurveyModel>();
            var courses = db.coursemodels.ToList();
            var surveys = db.surveyModel.ToList();
            var questions = db.surveyQuestion.ToList();
            
            foreach (SurveyModel survey in surveys)
            {
                if(survey.question_list != null)
                {
                    survey.question_list.Clear();
                }                
                foreach(SurveyQuestion question in questions)
                {
                    if(question.survey_Id == survey.survey_Id)
                    {
                        survey.question_list.Add(question);
                    }
                }
                foreach(CourseModels course in courses)
                {
                    if(course.course_Id == survey.course_Id)
                    {
                        survey.CourseModels = course;
                        survey_list.Add(survey);
                    }
                }
            }            
            return View(survey_list);
        }

        // GET: Survey/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SurveyModel surveyModel = db.surveyModel.Find(id);
            if (surveyModel == null)
            {
                return HttpNotFound();
            }
            return View(surveyModel);
        }

        // GET: Survey/Create
        public ActionResult Create()
        {
            ViewBag.course_Id = new SelectList(db.coursemodels, "course_Id", "courseName");
            return View();
        }

        // POST: Survey/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SurveyModel surveyModel)
        {
            if (ModelState.IsValid)
            {
                db.surveyModel.Add(surveyModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.course_Id = new SelectList(db.coursemodels, "course_Id", "courseName", surveyModel.course_Id);
            return View(surveyModel);
        }

        // GET: Survey/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SurveyModel surveyModel = db.surveyModel.Find(id);
            if (surveyModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.course_Id = new SelectList(db.coursemodels, "course_Id", "courseName", surveyModel.course_Id);
            return View(surveyModel);
        }

        // POST: Survey/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SurveyModel surveyModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(surveyModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.course_Id = new SelectList(db.coursemodels, "course_Id", "courseName", surveyModel.course_Id);
            return View(surveyModel);
        }

        // GET: Survey/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SurveyModel surveyModel = db.surveyModel.Find(id);
            if (surveyModel == null)
            {
                return HttpNotFound();
            }
            return View(surveyModel);
        }

        // POST: Survey/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SurveyModel surveyModel = db.surveyModel.Find(id);
            db.surveyModel.Remove(surveyModel);
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
