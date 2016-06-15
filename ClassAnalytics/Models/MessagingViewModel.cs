using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassAnalytics.Models
{
    public class MessagingViewModel
    {
        public int message_Id { get; set; }
        public MessagingModel Message { get; set; }
        public ApplicationUser sender { get; set; }
        public StudentModels student_recip { get; set; }
        public InstructorModel inst_sender { get; set; }
        public InstructorModel inst_recip { get; set; }

    }
}