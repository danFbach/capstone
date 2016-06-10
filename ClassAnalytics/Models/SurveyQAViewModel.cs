using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class SurveyQAViewModel
    {
        [Key]
        public int answer_Id { get; set; }

        [Display(Name = "Questions")]
        public List<SurveyQuestion> SurveyQuestions { get; set; }
        
        [Display(Name = "Survey")]
        public int survey_Id { get; set; }
        public SurveyModel SurveyModel { get; set; }

        public List<SurveyAnswers> answer_list { get; set; }

        [Display(Name = "Student")]
        public int student_Id { get; set; }
        public StudentModels StudentModels { get; set; }
    }
}