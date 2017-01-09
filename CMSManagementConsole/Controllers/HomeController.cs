using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSManagementConsole.Controllers
    {
    public class HomeController : Controller
        {
        public ActionResult Index()
            {
            ViewBag.loginFailed = "";
            return View("Login");
            }

        [HttpPost]
        public ActionResult Login()
            {
            return RedirectToAction("Welcome");
            }

        public ActionResult Welcome()
            {
            return View();
            }
        
        }
    }