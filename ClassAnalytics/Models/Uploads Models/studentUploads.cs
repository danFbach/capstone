using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models.Uploads_Models
{
    public class studentUploads
    {
        [Key]
        public int id { get; set; }
        public string student_account_id { get; set; }
        public string class_name { get; set; }
        [Display(Name ="File Name")]
        public string file_name { get; set; }
        public DateTime createDate { get; set; }
        public int? task_id { get; set; }
        public Task_Models.TaskModel taskModel { get; set; }
    }
}