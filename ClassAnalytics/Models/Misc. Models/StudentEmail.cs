using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ClassAnalytics.Models.Misc_Models
{
    public class StudentEmail
    {
        public int id { get; set; }
        public int class_id { get; set; }
        public string message { get; set; }
        public List<SelectListItem> students { get; set; }
        public int student_Id { get; set; }
    }
}