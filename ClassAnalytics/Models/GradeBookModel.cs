using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class GradeBookModel
    {
        [Key]
        public int grade_Id { get; set; }

        [Display(Name = "Student Id")]
        public int student_Id { get; set; }
        public StudentModels StudentModels { get; set; }
        public int task_Id { get; set; }
        public TaskModel TaskModel { get; set; }
        public int class_Id { get; set; }
        public ClassModel ClassModel { get; set; }
        [Display(Name = "Notes")]
        public string assignment_notes { get; set; }
        [Display(Name = "Grade")]
        public decimal? grade { get; set; }
    }
}