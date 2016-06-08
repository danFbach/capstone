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
        public int Id { get; set; }

        [Display(Name = "Survey Name")]
        public string SurveyName { get; set; }

        [Display(Name = "Active")]
        public bool active { get; set; }
    }
    public class SurveyQuestion
    {
        [Display(Name = "Question")]
        public string question { get; set; }

        [Display(Name = "Answer")]
        public bool answer { get; set; }

        public SurveyModel survey { get; set; }

    }
}