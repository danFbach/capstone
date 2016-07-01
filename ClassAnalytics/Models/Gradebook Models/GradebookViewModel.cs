using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ClassAnalytics.Models.Gradebook_Models
{
    public class GradebookViewModel
    {
        public string studentName { get; set; }
        public List<SelectListItem> taskList { get; set; }
        public int? task_id { get; set; }
        public List<SelectListItem> classList { get; set; }
        public int? class_id { get; set; }
        public List<GradeBookModel> grades { get; set; }
    }
}