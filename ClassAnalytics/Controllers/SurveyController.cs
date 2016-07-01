
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
            List<SurveyJoinTableModel> tables = db.surveyJoinTableModel.ToList();

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
                        foreach(SurveyJoinTableModel table in tables)
                        {
                            if (table.survey_Id == db.surveyQuestion.Find(question.question_Id).survey_Id)
                            {
                                if(table.class_Id == current_student.class_Id)
                                {
                                    new_answer.survey_join_id = table.survey_join_Id;
                                }
                            }
                        }                        
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            SurveyJoinTableModel joinSurvey = db.surveyJoinTableModel.Find(id);
            joinSurvey.active = true;
            db.SaveChanges();

            return RedirectToAction("Index_Class_Survey/" + joinSurvey.class_Id);
        }
        public ActionResult Deactivate(int id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            SurveyJoinTableModel joinSurvey = db.surveyJoinTableModel.Find(id);
            joinSurvey.active = false;
            db.SaveChanges();
            return RedirectToAction("Index_Class_Survey/" + joinSurvey.class_Id);
        }

        public ActionResult Index_Class_Survey(int? id, bool? active)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            var joinSurvey = db.surveyJoinTableModel.ToList();
            List<SurveyJoinTableModel> survey_list = new List<SurveyJoinTableModel>();
            ViewBag.thisClass = db.classmodel.Find(id).className;
            if (id == null)
            {
                return RedirectToAction("Index","Class");
            }
            else
            {
                if (active == null)
                {
                    foreach (SurveyJoinTableModel survey in joinSurvey)
                    {
                        if (survey.class_Id == id)
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
                            if (survey.class_Id == id)
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

        public ActionResult Create_Join(int id, int class_id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            SurveyJoinTableModel joinSurvey = new SurveyJoinTableModel();
            SurveyModel a_survey = db.surveyModel.Find(id);
            joinSurvey.SurveyModel = a_survey;
            joinSurvey.survey_Id = id;
            joinSurvey.class_Id = class_id;
            db.surveyJoinTableModel.Add(joinSurvey);
            List<SurveyQuestion> questions = db.surveyQuestion.ToList();
            List<StudentModels> students = db.studentModels.ToList();
            foreach (StudentModels student in students)
            {
                if (student.class_Id == class_id)
                {
                    foreach (SurveyQuestion question in questions)
                    {
                        if (question.survey_Id == id)
                        {
                            SurveyAnswers answer = new SurveyAnswers();
                            answer.question_Id = question.question_Id;
                            answer.student_Id = student.student_Id;
                            answer.survey_join_id = joinSurvey.survey_join_Id;
                            db.surveyAnswers.Add(answer);
                        }
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index_Class_Survey/" + class_id);
        }


        public ActionResult Create_Question(int id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
        public ActionResult Index(int? id)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<SurveyModel> survey_list = new List<SurveyModel>();
            ClassModel this_class = db.classmodel.Find(id);
            ProgramModels program = db.programModels.Find(this_class.program_id);
            ViewBag.className = this_class.className;
            ViewBag.program = program.programName;
            var surveys = db.surveyModel.ToList();
            var questions = db.surveyQuestion.ToList();


            foreach (SurveyModel survey in surveys)
            {
                survey.CourseModels = db.coursemodels.Find(survey.course_Id);
                if (survey.question_list != null)
                {
                    survey.question_list.Clear();
                }
                if(survey.CourseModels.program_Id == this_class.program_id)
                {
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
            surveyCreateViewModel create = new surveyCreateViewModel();
            create.class_id = id;
            create.surveyModel = new SurveyModel();
            create.courseList = new List<SelectListItem>();
            ClassModel this_class = db.classmodel.Find(id);
            List<CourseModels> courses = db.coursemodels.ToList();
            
            foreach (CourseModels course in courses)
            {
                if(course.program_Id == this_class.program_id)
                {
                    create.courseList.Add(new SelectListItem() { Text = course.courseName, Value = course.course_Id.ToString() });
                }
            }            
            return View(create);
        }

        // POST: Survey/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(surveyCreateViewModel create)
        {
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                SurveyModel surveyModel = new SurveyModel();
                surveyModel = create.surveyModel;                
                db.surveyModel.Add(surveyModel);
                db.SaveChanges();
                return RedirectToAction("Index/" + create.class_id );
            }
            create.surveyModel = new SurveyModel();
            create.courseList = new List<SelectListItem>();
            ClassModel this_class = db.classmodel.Find(create.class_id);
            List<CourseModels> courses = db.coursemodels.ToList();
            foreach (CourseModels course in courses)
            {
                if (course.program_Id == this_class.program_id)
                {
                    create.courseList.Add(new SelectListItem() { Text = course.courseName, Value = course.course_Id.ToString() });
                }
            }
            return View(create);
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
            SurveyJoinTableModel surveyModel = db.surveyJoinTableModel.Find(id);
            surveyModel.SurveyModel = db.surveyModel.Find(surveyModel.survey_Id);
            surveyModel.ClassModel = db.classmodel.Find(surveyModel.class_Id);
            if (surveyModel == null)
            {
                return HttpNotFound();
            }
            return View(surveyModel);
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
            SurveyJoinTableModel surveyModel = db.surveyJoinTableModel.Find(id);
            db.surveyJoinTableModel.Remove(surveyModel);
            db.SaveChanges();
            return RedirectToAction("Index_Class_Survey/" + surveyModel.class_Id);
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
