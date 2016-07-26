using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Class_Models;
using ClassAnalytics.Models.Task_Models;

namespace ClassAnalytics.Models.Uploads_Models
{
    public class studentUploads
    {
        [Key]
        public int id { get; set; }
        public int student_id { get; set; }
        public StudentModels studentModel { get; set; }
        [Display(Name ="File Name")]
        public string file_name { get; set; }
        public DateTime createDate { get; set; }
        public int? task_id { get; set; }
        public TaskModel taskModel { get; set; }
    }
}