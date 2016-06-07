using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class TaskTypeModels
    {
        [Key]
        public int taskType_Id { get; set; }

        [Display(Name = "Task Type")]
        public string taskType { get; set; }

        [Display(Name = "Weight")]
        public decimal taskWeight { get; set; }
    }
}