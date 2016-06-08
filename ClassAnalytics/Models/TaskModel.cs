using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class TaskModel
    {
        [Key]
        public int task_Id { get; set; }

        [Display(Name = "Task Name")]
        public string taskName { get; set; }

        [Display(Name = "Task Type")]
        public int taskType_Id { get; set; }
        public TaskTypeModels TaskTypeModels { get; set; }

        [Display(Name = "Possible Points")]
        public int points { get; set; }

        [Display(Name = "Start Date")]
        public DateTime startDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime endDate { get; set; }

        public int unit_Id { get; set; }
        public UnitModels UnitModels { get; set; }

        [Display(Name = "Notes")]
        public string taskNotes { get; set; }
    }
}