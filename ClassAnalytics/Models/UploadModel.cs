using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class UploadModel
    {
        [Key]
        public int upload_id { get; set; }
        [Display(Name ="Upload Name")]
        public string uploadName { get; set; }
        [Display(Name ="Class")]
        public int? class_id { get; set; }
        public ClassModel classModel { get; set; }
        [Display(Name = "File")]
        public string filePath { get; set; }
    }
}