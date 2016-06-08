using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class StudentModels
    {
        [Key]
        public int student_Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string fName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lName { get; set; }

        [Display(Name = "Class")]
        public int class_Id { get; set; }
        public ClassModel ClassModel { get; set; }
        public string student_account_Id { get; set; }
    }
}