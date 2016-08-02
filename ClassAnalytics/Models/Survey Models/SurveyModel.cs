using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Misc_Models;
using ClassAnalytics.Models.Class_Models;

namespace ClassAnalytics.Models.Survey_Models
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

        [Display(Name = "Class")]
        public int class_id { get; set; }
        public ClassModel classModel { get; set; }

        [Display(Name = "Active")]
        public bool active { get; set; }

        public List<SurveyQuestion> question_list { get; set; }
    }
}