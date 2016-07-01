using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ClassAnalytics.Models
{
    public class surveyCreateViewModel
    {
        public int? survey_id { get; set; }
        public SurveyModel surveyModel { get; set; }
        public List<SelectListItem> courseList { get; set; }
        public int? class_id { get; set; }
    }
}