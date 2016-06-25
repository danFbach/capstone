using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ClassAnalytics.Models;

namespace ClassAnalytics.Models
{
    public class UploadViewModel
    {
        [Display(Name ="Class")]
        public int class_id { get; set; }
        public List<SelectListItem> classes { get; set; }
        [Display(Name ="Select File")]
        public HttpPostedFileBase newFile { get; set; }
    }
}