using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassAnalytics.Models.Program_Models;
using ClassAnalytics.Models.Class_Models;

namespace ClassAnalytics.Models.Misc._Models
{
    public class ClassAndProgram
    {
        public int program_id { get; set; }
        public ProgramModels programModels { get; set; }
        public int class_id { get; set; }
        public ClassModel classModel { get; set; }
    }
}