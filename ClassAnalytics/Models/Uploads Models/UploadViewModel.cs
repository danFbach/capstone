﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models.Misc_Models;
using ClassAnalytics.Models.Class_Models;

namespace ClassAnalytics.Models.Uploads_Models
{
    public class UploadViewModel
    {
        public int upload_id { get; set; }
        [Display(Name = "Upload Name")]
        public string uploadName { get; set; }
        [Display(Name = "Class")]
        public int? class_id { get; set; }
        public ClassModel classModel { get; set; }
        [Display(Name = "File")]
        public string relativePath { get; set; }
        [Required]
        [Display(Name = "Active")]
        public bool active { get; set; }
        public DateTime createDate { get; set; }
        [Display(Name = "Upload Type")]
        public string uploadType { get; set; }
        [Display(Name = "Course")]
        public int? course_Id { get; set; }
        public CourseModels courseModels { get; set; }
        public List<SelectListItem> courses { get; set; }
    }
}