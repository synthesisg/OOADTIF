using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ooadtest4_5.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    return View();
                case "POST":        //login
                    string user = Request["adminname"];
                    string pwd = Request["adminpwd"];
                    Session["user"] = user;
                    Session["pswd"] = pwd;
                    if (user == "123" && pwd == "456") //success
                    {
                        Session["is_admin"] = true;
                        Response.Redirect("TeacherInfo");
                    }
                    else Session["is_admin"] = false;
                    break;
            }
            return View();
        }
        public ActionResult TeacherInfo()
        {
            if (Session["is_admin"] == null) return Content("[null]Login First!");
            if ((bool)Session["is_admin"] == false) return Content("[false]Login First!");
            return View();
        }
        public ActionResult TeacherInfoMod()
        {
            return View();
        }
        public ActionResult TeacherCreate()
        {
            return View();
        }
        public ActionResult StudentInfo()
        {
            return View();
        }
        public ActionResult StudentInfoMod()
        {
            return View();
        }
        public ActionResult StudentCreate()
        {
            return View();
        }
    }
}