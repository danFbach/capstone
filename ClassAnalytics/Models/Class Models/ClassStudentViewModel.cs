using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Program_Models;

namespace ClassAnalytics.Models.Class_Models
{
    public class ClassStudentViewModel
    {
        public int student_Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string fName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lName { get; set; }
        
        public int class_Id { get; set; }
        public ClassModel ClassModel { get; set; }
        
        public int program_Id { get; set; }
        public ProgramModels ProgramModels { get; set; }

        public List<SelectListItem> classList { get; set; }
        public List<SelectListItem> programList { get; set; }

        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string newEmail { get; set; }
    }
}