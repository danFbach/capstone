using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class SurveyResultModel
    {
        [Key]
        public int surveyResult_Id { get; set; }

        [Display(Name = "Survey Id")]
        public int survey_Id { get; set; }
        public SurveyModel SurveyModel { get; set; }
        
    }
}