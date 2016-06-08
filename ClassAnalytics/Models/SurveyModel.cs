using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class SurveyModel
    {
        [Key]
        public int survey_Id { get; set; }

        [Display(Name = "Survey Name")]
        public string SurveyName { get; set; }

        [Display(Name = "Date")]
        public DateTime surveyDate { get; set; }

        [Display(Name = "Active")]
        public bool active { get; set; }

        [Display(Name = "Student")]
        public int? student_Id { get; set; }
        public StudentModels StudentModels { get; set; }

        public List<SurveyQuestion> question_list { get; set; }
    }
}