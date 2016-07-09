using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Task_Models;

namespace ClassAnalytics.Models.Class_Models
{
    public class ClassTaskJoinModel
    {
        [Key]
        public int id { get; set; }
        
        [Display(Name ="Class")]
        public int class_id { get; set; }
        public ClassModel _class { get; set; }

        [Display(Name ="Task")]
        public int task_id { get; set; }
        public TaskModel task { get; set; }
    }
}