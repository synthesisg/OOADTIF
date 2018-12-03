using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ooadtest4_5.Models;
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
            /*
            if (Session["is_admin"] == null) return Content("[null]Login First!");
            if ((bool)Session["is_admin"] == false) return Content("[false]Login First!");
            //*/
            var teacher = from ui in uidb.data where (ui.is_student == false) select ui;
            //return Content(teacher.Count().ToString());
            ViewBag.teacherlist = teacher;
            return View();
        }
        public ActionResult TeacherInfoMod(int id)
        {
            userinfo ui = uidb.data.Find(id);
            ViewBag.uimod = ui;
            //return Content(id.ToString());
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

        userinfoDB uidb = new userinfoDB();
    }
}