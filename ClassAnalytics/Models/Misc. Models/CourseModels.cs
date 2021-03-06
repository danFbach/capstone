﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Program_Models;

namespace ClassAnalytics.Models.Misc_Models
{
    public class CourseModels
    {
        [Key]
        public int course_Id { get; set; }

        [Display(Name = "Course Name")]
        public string courseName { get; set; }

        [Required]
        [Display(Name = "Course Start Date")]
        [DataType(DataType.Date)]
        public string startDate { get; set; }

        [Required]
        [Display(Name = "Course End Date")]
        [DataType(DataType.Date)]
        public string endDate { get; set; }

        [Display(Name = "Program")]
        public int program_Id { get; set; }
        public ProgramModels programModels { get; set; }
    }
}