using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Class_Models;

namespace ClassAnalytics.Models.Survey_Models
{
    public class SurveyAnswers
    { 
        [Key]
        public int answer_Id { get; set; }

        public int student_Id { get; set; }
        public StudentModels StudentModels { get; set; }

        public bool answer { get; set; }

        public int question_Id { get; set; }
        public SurveyQuestion SurveyQuestion { get; set; }
        public int survey_join_id { get; set; }
        public SurveyJoinTableModel surveyJoinTableModel { get; set; }
    }
}