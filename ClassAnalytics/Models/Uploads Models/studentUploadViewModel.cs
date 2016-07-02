using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassAnalytics.Models.Uploads_Models
{
    public class studentUploadViewModel
    {
        public List<UploadModel> uploadList { get; set; }
        public List<studentUploads> studentUploadList { get; set; }
    }
}