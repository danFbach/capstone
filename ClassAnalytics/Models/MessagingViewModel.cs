using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ClassAnalytics.Models
{
    public class MessagingViewModel
    {
        public int message_Id { get; set; }
        public MessagingModel Message { get; set; }

        public List<SelectListItem> recipients { get; set; }
    }
}