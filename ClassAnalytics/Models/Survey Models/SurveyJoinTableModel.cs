using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Class_Models;

namespace ClassAnalytics.Models.Survey_Models
{
    public class SurveyJoinTableModel
    {
        [Key]
        public int survey_join_Id { get; set; }

        [Display(Name = "Class")]
        public int class_Id { get; set; }
        public ClassModel ClassModel { get; set; }

        [Display(Name = "Survey")]
        public int survey_Id { get; set; }
        public SurveyModel SurveyModel { get; set; }

        [Display(Name = "Active")]
        public bool active { get; set; }
    }
}