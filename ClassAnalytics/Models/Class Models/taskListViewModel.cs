using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassAnalytics.Models.Task_Models;
using ClassAnalytics.Models.Gradebook_Models;

namespace ClassAnalytics.Models.Class_Models
{
    public class taskListViewModel
    {
        public TaskModel task { get; set; }
        public List<GradeBookModel> grades { get; set; }

    }
}