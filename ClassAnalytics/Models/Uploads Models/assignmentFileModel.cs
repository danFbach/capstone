using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class assignmentFileModel
    {
        public string assign_id { get; set; }
        [Display(Name ="File")]
        public string filePath { get; set; }
        [Display(Name ="Due Date")]
        [DataType(DataType.DateTime)]
        public DateTime dueDate { get; set; }
        [Display(Name ="Class")]
        public int class_id { get; set; }
    }
}