﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassAnalytics.Models.Program_Models;

namespace ClassAnalytics.Models.Class_Models
{
    public class classListViewModel
    {
        public ClassModel _class{ get; set; }
        public ProgramModels program { get; set; }
    }
}