using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassAnalytics.Models.Class_Models;

namespace ClassAnalytics.Models.Instructor_Models
{
    public class InstructorDetailViewModel
    {
        public int instructor_id { get; set; }
        public InstructorModel instructor { get; set; }
        public List<ClassModel> classList { get; set; }

    }
}