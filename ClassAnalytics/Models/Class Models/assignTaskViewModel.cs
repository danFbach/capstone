using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ClassAnalytics.Models.Class_Models
{
    public class assignTaskViewModel
    {
        public ClassTaskJoinModel classTask { get; set; }
        public List<SelectListItem> classes { get; set; }
    }
}