﻿using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace ClassAnalytics.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(this.User.IsInRole("Student") || this.User.IsInRole("Admin"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}