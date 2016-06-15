using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClassAnalytics.Models
{
    public class MessagingModel
    {
        [Key]
        public int message_Id { get; set; }

        [Display(Name = "Sender")]
        public string sending_id { get; set; }
        public ApplicationUser sending_User { get; set; }
        
        [Display(Name = "Recipient")]
        public string recieve_id { get; set; }
        public ApplicationUser receiving_User { get; set; }
        
        [Display(Name = "Message")]
        public string message { get; set; }

        [Display(Name = "Subject")]
        public string subject { get; set; }

        [Display(Name = "Read")]
        public bool read { get; set; }

        [Display(Name = "Date Sent")]
        public DateTime dateSent { get; set; }
    }
}