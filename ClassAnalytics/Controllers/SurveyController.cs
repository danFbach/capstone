
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ClassAnalytics.Models;

namespace ClassAnalytics.Controllers
{
    public class SurveyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult SurveyBarChart(int? survey_id, int? class_id)
        {
            ViewBag.class_id = new SelectList(db.classmodel, "class_Id", "className");
            ViewBag.survey_id = new SelectList(db.surveyModel, "survey_Id", "SurveyName");
            SurveyModel survey = db.surveyModel.Find(survey_id);
            List<SurveyChartModel> charts = new List<SurveyChartModel>();
            var questions = db.surveyQuestion.ToList();
            var answers = db.surveyAnswers.ToList();
            List<List<Object>> answerList = new List<List<object>>();

            foreach (SurveyQuestion question in questions)
            {
                SurveyChartModel chart = new SurveyChartModel();
                chart.question = question.question;
                chart.question_id = question.question_Id;
                chart.answer_count = 0;
                if (question.survey_Id == survey_id)
                {
                    foreach (SurveyAnswers answer in answers)
                    {
                        answer.StudentModels = new StudentModels();
                        answer.StudentModels = db.studentModels.Find(answer.student_Id);
                        if (answer.StudentModels.class_Id == class_id)
                        {
                            if (answer.question_Id == question.question_Id)
                            {
                                if (answer.answer == true)
                                {
                                    chart.answer_count += 1;
                                    chart.response_count += 1;
                                }
                                else
                                {
                                    chart.response_count += 1;
                                }
                            }
                        }
                    }
                    charts.Add(chart);
                }
            }
            return View(charts);
        }
        public bool new_answer_form(int question_id, int student_id)
        {
            var answers = db.surveyAnswers.ToList();

            foreach (SurveyAnswers answer in answers)
            {
                if (answer.question_Id == question_id)
                {
                    if (answer.student_Id == student_id)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public ActionResult fill_out_survey(int id)
        {
            SurveyQAViewModel a_survey = new SurveyQAViewModel();
            a_survey.SurveyModel = db.surveyModel.Find(id);
            List<SurveyAnswers> answer_forms = db.surveyAnswers.ToList();
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var students = db.studentModels.ToList();
            StudentModels current_student = new StudentModels();
            a_survey.answer_list = new List<SurveyAnswers>();

            foreach (StudentModels student in students)
            {
                if (student.student_account_Id == UserId)
                {
                    current_student = student;
                }
            }
            a_survey.StudentModels = db.studentModels.Find(current_student.student_Id);
            foreach (SurveyAnswers answer in answer_forms)
            {
                bool new_form = false;
                answer.SurveyQuestion = db.surveyQuestion.Find(answer.question_Id);
                if (answer.SurveyQuestion.survey_Id == id)
                {
                    if (answer.student_Id == current_student.student_Id)
                    {
                        a_survey.answer_list.Add(answer);
                    }
                    else if (answer.student_Id != current_student.student_Id)
                    {
                        new_form = new_answer_form(answer.question_Id, current_student.student_Id);
                        if (new_form == true)
                        {
                            answer.answer = false;
                            answer.student_Id = current_student.student_Id;
                            db.surveyAnswers.Add(answer);
                            a_survey.answer_list.Add(answer);
                            db.SaveChanges();
                        }
                    }
                }
            }
            return View(a_survey);
        }

        [HttpPost]
        public ActionResult fill_out_survey(SurveyQAViewModel qa)
        {
            foreach (SurveyAnswers answer in qa.answer_list)
            {
                SurveyAnswers a = db.surveyAnswers.Find(answer.answer_Id);
                a.answer = answer.answer;
                db.SaveChanges();
            }
            return RedirectToAction("Student_Index");
        }
        public ActionResult Student_Index(int? id)
        {
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var students = db.studentModels.ToList();
            var surveys = db.surveyJoinTableModel.ToList();
            StudentModels current_student = new StudentModels();
            List<SurveyJoinTableModel> survey_list = new List<SurveyJoinTableModel>();
            foreach (StudentModels student in students)
            {
                if (student.student_account_Id == UserId)
                {
                    current_student = student;
                }
            }
            int student_class_Id = current_student.class_Id;
            foreach (SurveyJoinTableModel survey in surveys)
            {
                if (survey.class_Id == student_class_Id)
                {
                    survey.SurveyModel = db.surveyModel.Find(survey.survey_Id);
                    survey.ClassModel = db.classmodel.Find(survey.class_Id);
                    survey_list.Add(survey);
                }
            }
            if (id == null)
            {
                ViewBag.StatusMessage = "";
                return View(survey_list);
            }
            else
            {
                ViewBag.StatusMessage = "This survey is not currently active.";
                return View(survey_list);
            }
        }
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

        public ActionResult Index_Class_Survey(int? class_id, bool? active)
        {
            var joinSurvey = db.surveyJoinTableModel.ToList();
            List<SurveyJoinTableModel> survey_list = new List<SurveyJoinTableModel>();
            ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
            if (class_id == null)
            {
                if (active == null)
                {
                    foreach (SurveyJoinTableModel survey in joinSurvey)
                    {
                        survey.SurveyModel = db.surveyModel.Find(survey.survey_Id);
                        survey.ClassModel = db.classmodel.Find(survey.class_Id);
                        survey_list.Add(survey);
                    }
                }
                else
                {
                    foreach (SurveyJoinTableModel survey in joinSurvey)
                    {
                        if (survey.active == active)
                        {
                            survey.SurveyModel = db.surveyModel.Find(survey.survey_Id);
                            survey.ClassModel = db.classmodel.Find(survey.class_Id);
                            survey_list.Add(survey);
                        }
                    }
                }
                return View(survey_list);
            }
            else
            {
                if (active == null)
                {
                    foreach (SurveyJoinTableModel survey in joinSurvey)
                    {
                        if (survey.class_Id == class_id)
                        {
                            survey.SurveyModel = db.surveyModel.Find(survey.survey_Id);
                            survey.ClassModel = db.classmodel.Find(survey.class_Id);
                            survey_list.Add(survey);
                        }
                    }
                }
                else
                {
                    foreach (SurveyJoinTableModel survey in joinSurvey)
                    {
                        if (survey.active == active)
                        {
                            if (survey.class_Id == class_id)
                            {
                                survey.SurveyModel = db.surveyModel.Find(survey.survey_Id);
                                survey.ClassModel = db.classmodel.Find(survey.class_Id);
                                survey_list.Add(survey);
                            }
                        }
                    }
                }
                return View(survey_list);
            }
        }

        public ActionResult Create_Join(int id)
        {
            SurveyJoinTableModel joinSurvey = new SurveyJoinTableModel();
            SurveyModel a_survey = db.surveyModel.Find(id);
            ViewBag.class_Id = new SelectList(db.classmodel, "class_Id", "className");
            joinSurvey.SurveyModel = a_survey;
            joinSurvey.survey_Id = id;
            return View(joinSurvey);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Join(SurveyJoinTableModel survey)
        {
            List<SurveyQuestion> questions = db.surveyQuestion.ToList();
            if (ModelState.IsValid)
            {
                var students = db.studentModels.ToList();
                foreach (StudentModels student in students)
                {
                    if (student.class_Id == survey.class_Id)
                    {
                        foreach (SurveyQuestion question in questions)
                        {
                            if (question.survey_Id == survey.survey_Id)
                            {
                                SurveyAnswers answer = new SurveyAnswers();
                                answer.question_Id = question.question_Id;
                                answer.student_Id = student.student_Id;
                                db.surveyAnswers.Add(answer);
                            }
                        }
                    }
                }
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
                if (survey.question_list != null)
                {
                    survey.question_list.Clear();
                }
                foreach (SurveyQuestion question in questions)
                {
                    if (question.survey_Id == survey.survey_Id)
                    {
                        survey.question_list.Add(question);
                    }
                }
                foreach (CourseModels course in courses)
                {
                    if (course.course_Id == survey.course_Id)
                    {
                        survey.CourseModels = course;
                    }
                }
                survey_list.Add(survey);
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
