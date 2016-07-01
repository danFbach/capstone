using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ClassAnalytics.Models.Misc_Models
{
    public class MessagingViewModel
    {
        public int message_Id { get; set; }
        public MessagingModel Message { get; set; }
        public List<MessagingModel> message_list { get; set; }
        public string recip { get; set; }
        public string sender { get; set; }
        public List<SelectListItem> recipients { get; set; }
    }
}