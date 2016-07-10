using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassAnalytics.Models.Misc_Models;
using ClassAnalytics.Models.Task_Models;

namespace ClassAnalytics.Models.Class_Models
{
    public class courseListViewModel
    {
        public CourseModels course { get; set; }
        public List<TaskModel> tasks { get; set; }
    }
}