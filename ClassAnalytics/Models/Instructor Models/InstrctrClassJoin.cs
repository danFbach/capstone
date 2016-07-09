using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Class_Models;

namespace ClassAnalytics.Models.Instructor_Models
{
    public class InstrctrClassJoin
    {
        [Key]
        public int id { get; set; }
        public int instructor_id { get; set; }
        public int class_id { get; set; }
        public ClassModel classModel { get; set; }
    }
}