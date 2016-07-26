using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassAnalytics.Models.Class_Models;

namespace ClassAnalytics.Models.Uploads_Models
{
    public class uploadListViewModel
    {
        public List<UploadModel> instructorUploads { get; set; }
        public List<studentUploads> studentUploads { get; set; }
    }
}