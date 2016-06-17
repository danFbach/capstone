using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class GradebookViewModel
    {
        public string studentName { get; set; }
        public List<GradeBookModel> grades { get; set; }
    }
}