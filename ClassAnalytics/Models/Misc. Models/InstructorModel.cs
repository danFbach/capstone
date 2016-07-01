using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models.Misc_Models
{
    public class InstructorModel
    {
        [Key]
        public int instuctor_Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string fName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lName { get; set; }

        [Required]
        public string instructor_account_Id { get; set; }
    }
}