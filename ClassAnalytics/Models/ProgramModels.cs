using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class ProgramModels
    {
        [Key]
        public int program_Id { get; set; }

        [Display(Name = "Program Name")]
        public string programName { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime endDate { get; set; }


    }
}