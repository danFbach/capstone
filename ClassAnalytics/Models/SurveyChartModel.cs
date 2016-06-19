using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassAnalytics.Models
{
    public class SurveyChartModel
    {
        public List<List<Object>> answers { get; set; }
        public string survey_name { get; set; }
    }
}