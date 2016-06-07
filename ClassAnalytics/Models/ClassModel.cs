﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class ClassModel
    {
        [Key]
        public int class_Id { get; set; }

        [Display(Name = "Class Name")]
        public string className { get; set; }

        [Display(Name = "Program")]
        public int program_id { get; set; }
        public ProgramModels ProgramModels { get; set; }
    }
}