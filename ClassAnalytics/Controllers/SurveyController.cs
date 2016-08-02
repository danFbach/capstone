using System;
using System.Web;
using System.Net;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using ClassAnalytics.Models;
using ClassAnalytics.Models.Misc_Models;
using ClassAnalytics.Models.Class_Models;
using ClassAnalytics.Models.Survey_Models;
using ClassAnalytics.Models.Program_Models;

namespace ClassAnalytics.Controllers
{
    public class SurveyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult QuestionList(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "Class");
            }
            List<SurveyQuestion> questions = db.surveyQuestion.ToList();
            List<SurveyQuestion> _questions = new List<SurveyQuestion>();
            foreach(SurveyQuestion question in questions)
            {
                if(question.survey_Id == id)
                {
                    _questions.Add(question);
                }
            }
            if(_questions.Count() == 0)
            {
                int class_id = db.surveyModel.Find(id).class_id;
                return RedirectToAction("Index/" + class_id);
            }
            return View(_questions);
        }
        public ActionResult EditQuestion(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Class");
            }
            SurveyQuestion _question = db.surveyQuestion.Find(id);
            ViewBag.surveyName = db.surveyModel.Find(_question.survey_Id).SurveyName;
            return View(_question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuestion(SurveyQuestion _question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(_question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QuestionList/" + _question.survey_Id);
            }
            ViewBag.surveyName = db.surveyModel.Find(_question.survey_Id).SurveyName;
            return View(_question);
        }
        public ActionResult DeleteQuestion(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "Class");
            }
            SurveyQuestion question = db.surveyQuestion.Find(id);
            ViewBag.surveyName = db.surveyModel.Find(question.survey_Id).SurveyName;
            return View(question);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("DeleteQuestion")]
        public ActionResult ConfirmedQDelete(int id)
        {
            SurveyQuestion question = db.surveyQuestion.Find(id);
            db.surveyQuestion.Remove(question);
            db.SaveChanges();
            return RedirectToAction("QuestionList/" + question.survey_Id);
        }

        public ActionResult SurveyBarChart(int? survey_id, int? id)
        {
            SurveyChartModel chart = new SurveyChartModel();
            chart.answers = new List<List<object>>();
            ViewBag.survey_id = new SelectList(db.surveyModel, "survey_Id", "SurveyName");
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if(survey_id != null && id != null)
            {
                SurveyModel survey = db.surveyModel.Find(survey_id);
                List<SurveyQuestion> questions = db.surveyQuestion.ToList();
                List<SurveyAnswers> answers = db.surveyAnswers.ToList();
                chart.survey_name = db.surveyModel.Find(survey_id).SurveyName;
                int q_num = 1;
                foreach (SurveyQuestion question in questions)
                {
                    int answer_count = 0;
                    int response_count = 0;
                    string new_question = "";
                    int q_id = 0;
                    new_question = question.question;
                    q_id = question.question_Id;
                    if (question.survey_Id == survey_id)
                    {
                        foreach (SurveyAnswers answer in answers)
                        {
                            answer.StudentModels = new StudentModels();
                            answer.StudentModels = db.studentModels.Find(answer.student_Id);
                            if (answer.StudentModels.class_Id == id)
                            {
                                if (answer.question_Id == question.question_Id)
                                {
                                    if (answer.answer == true)
                                    {
                                        answer_count += 1;
                                        response_count += 1;
                                    }
                                    else
                                    {
                                        response_count += 1;
                                    }
                                }
                            }
                        }
                        chart.answers.Add(new List<object>() { question.question, q_num, answer_count, response_count });
                        q_num += 1;
                    }
                }
                return View(chart);
            }
            else
            {
                return View(chart);
            }
            
        }
        public bool new_answer_form(int question_id, int student_id)
        {
            List<SurveyAnswers> answers = db.surveyAnswers.ToList();

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
            if (!this.User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            SurveyQAViewModel a_survey = new SurveyQAViewModel();
            StudentModels current_student = new StudentModels();
            List<SurveyAnswers> answer_forms = db.surveyAnswers.ToList();
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            bool new_form = true;
            List<StudentModels> students = db.studentModels.ToList();
            List<SurveyQuestion> questions = db.surveyQuestion.ToList();
            a_survey.SurveyModel = db.surveyModel.Find(id);
            a_survey.survey_Id = a_survey.SurveyModel.survey_Id;
            a_survey.SurveyQuestions = new List<SurveyQuestion>();
            a_survey.answer_list = new List<SurveyAnswers>();

            foreach (StudentModels student in students)
            {
                if (student.student_account_Id == UserId)
                {
                    current_student = student;
                    a_survey.StudentModels = db.studentModels.Find(current_student.student_Id);
                    a_survey.student_Id = a_survey.StudentModels.student_Id;
                }
            }
            foreach (SurveyQuestion question in questions)
            {
                new_form = true;
                if (question.survey_Id == id)
                {
                    a_survey.SurveyQuestions.Add(question);
                    foreach (SurveyAnswers answer in answer_forms)
                    {
                        if (answer.question_Id == question.question_Id)
                        {
                            if (answer.student_Id == current_student.student_Id)
                            {
                                a_survey.answer_list.Add(answer);
                                new_form = false;
                                break;
                            }

                        }
                    }
                    if(new_form == true)
                    {
                        SurveyAnswers new_answer = new SurveyAnswers();
                        new_answer.answer = false;
                        new_answer.question_Id = question.question_Id;
                        new_answer.student_Id = current_student.student_Id;
                        a_survey.answer_list.Add(new_answer);
                        db.surveyAnswers.Add(new_answer);
                        db.SaveChanges();
                    }
                }
            }
            return View(a_survey);
        }
               
        [HttpPost]
        public ActionResult fill_out_survey(SurveyQAViewModel qa)
        {
            if (!this.User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (!this.User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            string UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<StudentModels> students = db.studentModels.ToList();
            List<SurveyModel> surveys = db.surveyModel.ToList();
            StudentModels current_student = new StudentModels();
            List<SurveyModel> survey_list = new List<SurveyModel>();
            foreach (StudentModels student in students)
            {
                if (student.student_account_Id == UserId)
                {
                    current_student = student;
                }
            }
            foreach (SurveyModel survey in surveys)
            {
                if (survey.class_id == current_student.class_Id)
                {
                    survey.classModel = db.classmodel.Find(survey.class_id);
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            SurveyModel survey = db.surveyModel.Find(id);
            survey.active = true;
            db.SaveChanges();

            return RedirectToAction("Index/" + survey.class_id);
        }
        public ActionResult Deactivate(int id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            SurveyModel survey = db.surveyModel.Find(id);
            survey.active = false;
            db.SaveChanges();
            return RedirectToAction("Index/" + survey.class_id);
        }
        
        public ActionResult Create_Question(int id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            SurveyQuestion surveyQuestion = new SurveyQuestion();
            surveyQuestion.survey_Id = id;
            surveyQuestion.survey = db.surveyModel.Find(id);
            return View(surveyQuestion);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Question(SurveyQuestion surveyQuestion)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                db.surveyQuestion.Add(surveyQuestion);
                db.SaveChanges();
                surveyQuestion.survey = db.surveyModel.Find(surveyQuestion.survey_Id);
                return RedirectToAction("Index/" + surveyQuestion.survey.class_id);
            }
            surveyQuestion.survey = db.surveyModel.Find(surveyQuestion.survey_Id);
            return View(surveyQuestion);
        }
        // GET: Survey
        public ActionResult Index(int? id, int? survey_id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if(survey_id != null)
            {
                id = db.surveyModel.Find(survey_id).class_id;
            }
            List<SurveyModel> survey_list = new List<SurveyModel>();
            ClassModel this_class = db.classmodel.Find(id);
            ViewBag.className = this_class.className;
            List<SurveyModel> surveys = db.surveyModel.ToList();
            var questions = db.surveyQuestion.ToList();


            foreach (SurveyModel survey in surveys)
            {
                if(survey.class_id == id)
                {
                    survey.CourseModels = db.coursemodels.Find(survey.course_Id);
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
                    survey_list.Add(survey);
                }                
            }
            return View(survey_list);
        }
        
        // GET: Survey/Create
        public ActionResult Create(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            surveyCreateViewModel viewModel = new surveyCreateViewModel();
            viewModel.class_id = id;
            viewModel.surveyModel = new SurveyModel();
            viewModel.courseList = new List<SelectListItem>();
            ClassModel this_class = db.classmodel.Find(id);
            List<CourseModels> courses = db.coursemodels.ToList();
            
            foreach (CourseModels course in courses)
            {
                if(course.program_Id == this_class.program_id)
                {
                    viewModel.courseList.Add(new SelectListItem() { Text = course.courseName, Value = course.course_Id.ToString() });
                }
            }            
            return View(viewModel);
        }

        // POST: Survey/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(surveyCreateViewModel viewModel)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                SurveyModel surveyModel = new SurveyModel();
                surveyModel = viewModel.surveyModel;
                surveyModel.class_id = Convert.ToInt16(viewModel.class_id);
                db.surveyModel.Add(surveyModel);
                db.SaveChanges();
                return RedirectToAction("Index/" + viewModel.class_id );
            }
            return RedirectToAction("Create/" + viewModel.class_id);
            //viewModel.surveyModel = new SurveyModel();
            //viewModel.courseList = new List<SelectListItem>();
            //ClassModel this_class = db.classmodel.Find(viewModel.class_id);
            //List<CourseModels> courses = db.coursemodels.ToList();
            //foreach (CourseModels course in courses)
            //{
            //    if (course.program_Id == this_class.program_id)
            //    {
            //        viewModel.courseList.Add(new SelectListItem() { Text = course.courseName, Value = course.course_Id.ToString() });
            //    }
            //}
            //return View(viewModel);
        }
        
        // GET: Survey/Delete/5
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
            SurveyModel survey = db.surveyModel.Find(id);
            survey.classModel = db.classmodel.Find(survey.class_id);
            survey.CourseModels = db.coursemodels.Find(survey.course_Id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }

        // POST: Survey/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) //id is survey_join_id
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            SurveyModel surveyModel = db.surveyModel.Find(id);
            db.surveyModel.Remove(surveyModel);
            db.SaveChanges();
            return RedirectToAction("Index/" + surveyModel.class_id);
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
