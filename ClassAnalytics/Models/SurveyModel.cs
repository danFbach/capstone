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

        [Display(Name = "Course")]
        public int course_Id { get; set; }
        public CourseModels CourseModels { get; set; }
                        
        public List<SurveyQuestion> question_list { get; set; }
    }
}