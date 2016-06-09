using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class SurveyStudentModel
    {
        [Key]
        public int answer_Id { get; set; }

        [Required]
        [Display(Name = "Survey")]
        public int survey_Id { get; set; }
        public SurveyModel SurveyModel { get; set; }

        [Display(Name = "Answer")]
        public bool answer { get; set; }

        [Display(Name ="Student")]
        public int student_Id { get; set; }
        public StudentModels StudentModels { get; set; }
    }
}