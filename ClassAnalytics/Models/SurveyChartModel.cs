using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassAnalytics.Models
{
    public class SurveyChartModel
    {
        public string question { get; set; }
        public int question_id { get; set; }
        public int answer_count { get; set; }
        public int response_count { get; set; }
    }
}