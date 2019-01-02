using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using final.Models;
namespace final.Controllers
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
            if (Request.Browser.Platform.ToString() == "WinNT") return RedirectToAction("WebLogin");
            else return RedirectToAction("MobileLogin");
        }

        public ActionResult WebLogin()
        {
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    return View();
                case "POST":        //login
                    string user = Request["userName"];
                    string pwd = Request["userPassword"];
                    ViewBag.mes = "";

                    var uilist = (from ui in db.student where (ui.account == user && ui.password == pwd) select ui).ToList();
                    if (uilist.Count() > 0) //success
                    {
                        if (uilist[0].is_active == 0)
                        {
                            Response.Write("<script type='text/javascript'>alert('Not Active!');</script>");
                        }
                        else
                        {
                            Session["user_id"] = uilist[0].id;
                            Session["is_student"] = true;
                            return Redirect("/StudentWeb/ChsLesson");
                        }
                    }
                    else
                    {
                        Session["is_student"] = false;

                        var ui2list = (from ui in db.teacher where (ui.account == user && ui.password == pwd) select ui).ToList();
                        if (ui2list.Count() > 0) //success
                        {
                            if (ui2list[0].is_active == 0)
                            {
                                Response.Write("<script type='text/javascript'>alert('Not Active!');</script>");
                            }
                            else
                            {
                                Session["user_id"] = ui2list[0].id;
                                Session["is_teacher"] = true;
                                return Redirect("/TeacherWeb/TeacherImport");
                            }
                        }
                        else
                        {
                            ViewBag.mes = "账号或密码错误";
                            Session["is_teacher"] = false;
                        }
                    }
                    break;
            }
            return View();
        }
        public ActionResult MobileLogin()
        {
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    return View();
                case "POST":        //login
                    string user = Request["userName"];
                    string pwd = Request["userPassword"];
                    ViewBag.mes = "";

                    var uilist = (from ui in db.student where (ui.account == user && ui.password == pwd) select ui).ToList();
                    if (uilist.Count() > 0) //success
                    {
                        if (uilist[0].is_active == 0)
                        {
                            Response.Write("<script type='text/javascript'>alert('Not Active!');</script>");
                        }
                        else
                        {
                            Session["user_id"] = uilist[0].id;
                            Session["is_student"] = true;
                            return Redirect("/StudentMobile/Seminar");
                        }
                    }
                    else
                    {
                        Session["is_student"] = false;

                        var ui2list = (from ui in db.teacher where (ui.account == user && ui.password == pwd) select ui).ToList();
                        if (ui2list.Count() > 0) //success
                        {
                            if (ui2list[0].is_active == 0)
                            {
                                Response.Write("<script type='text/javascript'>alert('Not Active!');</script>");
                            }
                            else
                            {
                                Session["user_id"] = ui2list[0].id;
                                Session["is_teacher"] = true;
                                return Redirect("/TeacherMobile/ChsLesson");
                            }
                        }
                        else
                        {
                            ViewBag.mes = "账号或密码错误";
                            Session["is_teacher"] = false;
                        }
                    }
                    break;
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["is_admin"] = false;
            Session["is_teacher"] = false;
            Session["is_student"] = false;
            return RedirectToAction("Index");
        }

        MSSQLContext db = new MSSQLContext();
    }
}