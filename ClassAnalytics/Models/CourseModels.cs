using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class CourseModels
    {
        [Key]
        public int course_Id { get; set; }

        [Display(Name = "Course Name")]
        public string courseName { get; set; }

        public int program_Id { get; set; }
        public ProgramModels ProgramModels { get; set; }
    }

}