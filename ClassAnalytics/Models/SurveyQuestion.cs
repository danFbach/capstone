using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class SurveyQuestion
    {
        [Key]
        public int question_Id { get; set; }
        
        [Display(Name = "Question")]
        public string question { get; set; }
        
        [Display(Name = "Survey Id")]
        public int survey_Id { get; set; }
        public SurveyModel survey { get; set; }

    }

}