using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class UnitModels
    {
        [Key]
        public int unit_Id { get; set; }

        [Display(Name = "Unit Name")]
        public string unitName { get; set; }

        [Display(Name = "Start Date")]
        public DateTime startDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime endDate { get; set; }

        public int course_Id { get; set; }
        public CourseModels CourseModels { get; set; }


    }
}