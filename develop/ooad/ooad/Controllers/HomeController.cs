using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ooad.Models;
namespace ooad.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            /*
            admin cdx = new admin
            {
                account = "admin",
                password = "admin"
            };
            MSSQLContext c = new MSSQLContext();
            c.admin.Add(cdx);
            c.SaveChanges();
            //*/
            return View();
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