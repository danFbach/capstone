using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Class_Models;

namespace ClassAnalytics.Models.Instructor_Models
{
    public class InstructorClassJoinModel
    {
        public int join_id { get; set; }
        [Display(Name ="Instructor")]
        public int instructor_Id { get; set; }
        public InstructorModel instructor { get; set; }
        [Display(Name ="Class")]
        public int class_id { get; set; }
        public ClassModel classModel { get; set; }
        public List<SelectListItem> classes { get; set; }
    }
}