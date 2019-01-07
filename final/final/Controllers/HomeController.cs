using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
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
                            Session["tmp_id"] = uilist[0].id;
                            Session["tmp_iden"] = "s";
                            return RedirectToAction("Activate");
                        }
                        else
                        {
                            Session["user_id"] = uilist[0].id;
                            Session["is_student"] = true;
                            return Redirect("/StudentMobile/StudentMyCourse");
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
                                Session["tmp_id"] = ui2list[0].id;
                                Session["tmp_iden"] = "t";
                                return RedirectToAction("Activate");
                            }
                            else
                            {
                                Session["user_id"] = ui2list[0].id;
                                Session["is_teacher"] = true;
                                return Redirect("/TeacherMobile/TeacherMyCourse");
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

        public ActionResult Activate()
        {
            if (Session["tmp_id"] == null)
                return Redirect("/Home/MobileLogin");
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    if (Session["tmp_iden"].ToString() == "t") ViewBag.iden = false;
                    else ViewBag.iden = true;
                    return View();
                case "POST":        //login
                    int id = Int32.Parse(Session["tmp_id"].ToString());
                    switch (Session["tmp_iden"].ToString())
                    {
                        case "t":
                            teacher ui = db.teacher.Find(id);
                            ui.password = Request["newPassword"];
                            ui.is_active = 1;
                            db.SaveChanges();
                            Session["user_id"] = Session["tmp_id"];
                            Session["is_teacher"] = true;
                            return Redirect("/TeacherMobile/TeacherMyCourse");
                        case "s":
                            student ui2 = db.student.Find(id);
                            ui2.password = Request["newPassword"];
                            ui2.email = Request["email"];
                            ui2.is_active = 1;
                            db.SaveChanges();
                            Session["user_id"] = Session["tmp_id"];
                            Session["is_student"] = true;
                            return Redirect("/StudentMobile/StudentMyCourse");
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

        public ActionResult FindPassword()
        {
            return View();
        }

        public bool SendPW2Email(string data)
        {
            var uilist = (from ui in db.student where ui.account.Equals(data) select ui).ToList();
            var uilist2 = (from ui in db.teacher where ui.account.Equals(data) select ui).ToList();
            if (uilist.Count() == 0 && uilist2.Count()==0) return false;
            string email, account, pwd;
            if (uilist.Count() > 0 )
            {
                email = uilist.ToList()[0].email;
                account = uilist.ToList()[0].account;
                pwd = uilist.ToList()[0].password;
            }
            else
            {
                email = uilist2.ToList()[0].email;
                account = uilist2.ToList()[0].account;
                pwd = uilist2.ToList()[0].password;
            }

            if (email == null || email == "") return false;

            SmtpClient client = new SmtpClient("smtp.163.com", 25);
            MailMessage msg = new MailMessage("13600858179@163.com", email, "找回瓜皮账户", "感谢使用瓜皮课堂\n您的账号" + account + "的password为" + pwd);
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicAuthenticationInfo =
                new System.Net.NetworkCredential("13600858179@163.com", "20100710A");
            client.Credentials = basicAuthenticationInfo;
            client.EnableSsl = true;
            client.Send(msg);

            return true;
        }

        MSSQLContext db = new MSSQLContext();
    }
}