﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class TaskViewModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Task Name")]
        public string taskName { get; set; }

        [Display(Name = "Task Type")]
        public int taskType_Id { get; set; }
        public TaskTypeModels TaskTypeModels { get; set; }

        [Display(Name = "Task Grade")]
        public int taskGrade { get; set; }

        [Display(Name = "Start Date")]
        public DateTime startDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime endDate { get; set; }

        public int unit_Id { get; set; }
        public UnitModels UnitModels { get; set; }

        [Display(Name = "Notes")]
        public string taskNotes { get; set; }

        public List<SelectListItem> taskTypes { get; set; }
    }
}