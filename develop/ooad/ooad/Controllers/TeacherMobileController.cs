using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ooad.Models;

//mail
using System.Net;
using System.Net.Mail;
namespace ooad.Controllers
{
    public class TeacherMobileController : Controller
    {
        // GET: TeacherMobile
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
                    string user = Request["userName"];
                    string pwd = Request["userPassword"];
                    var uilist = from ui in db.teacher where (ui.account == user && ui.password == pwd) select ui;
                    if (uilist.Count() > 0) //success
                    {
                        List<teacher> ui = new List<teacher>();
                        foreach (var u in uilist) ui.Add(u);
                        if (ui[0].is_active == 0)
                        {
                            Session["tmp_id"] = ui[0].id;
                            return RedirectToAction("Activate");
                        }
                        else
                        {
                            Session["user_id"] = ui[0].id;
                            Session["is_teacher"] = true;
                            return RedirectToAction("ChsLesson");
                        }
                    }
                    else Session["is_teacher"] = false;
                    break;
            }
            return View();
        }
        public ActionResult FindPassword()
        {
            return View();
        }
        public ActionResult Activate()
        {
            if (Session["tmp_id"] == null)
                return RedirectToAction("Login");
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    return View();
                case "POST":        //login
                    student ui = db.student.Find(Int32.Parse(Session["tmp_id"].ToString()));
                    ui.password = Request["newPassword"];
                    db.SaveChanges();
                    Session["user_id"] = Session["tmp_id"];
                    Session["is_teacher"] = true;
                    return RedirectToAction("ChsLesson");
            }
            return View();
        }
        public bool SendPW2Email(string data)
        {
            var uilist = from ui in db.student where ui.account.Equals(data) select ui;
            if (uilist.Count() == 0) return false;
            string email = uilist.ToList()[0].email;
            if (email == null || email == "") return false;

            SmtpClient client = new SmtpClient("smtp.163.com", 25);
            MailMessage msg = new MailMessage("13600858179@163.com", email, "找回瓜皮账户", "感谢使用瓜皮课堂\n您的密码为" + uilist.ToList()[0].password);
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicAuthenticationInfo =
                new System.Net.NetworkCredential("13600858179@163.com", "20100710A");
            client.Credentials = basicAuthenticationInfo;
            client.EnableSsl = true;
            client.Send(msg);

            return true;
        }


        void daiban()
        {
            teacher t = db.teacher.Find(Int32.Parse(Session["user_id"].ToString()));
            int teacher_id = t.id;

            //team_valid_application
            var tvalist = from tva in db.team_valid_application where tva.teacher_id == teacher_id select tva;
            //share_team_application
            var stalist = from sta in db.share_team_application where sta.sub_course_teacher_id == teacher_id select sta;
            //share_seminar_application
            var ssalist = from ssa in db.share_seminar_application where ssa.sub_course_teacher_id == teacher_id select ssa;

        }

        public bool chgpwd(string data)
        {
            if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false) return false;
            teacher s = db.teacher.Find(Int32.Parse(Session["user_id"].ToString()));
            s.password = data;
            db.SaveChanges();
            return true;
        }
        public bool chgemail(string data)
        {
            if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false) return false;
            teacher s = db.teacher.Find(Int32.Parse(Session["user_id"].ToString()));
            s.email = data;
            db.SaveChanges();
            return true;
        }

        bool is_judge = false;
        int test_id = 1;
        MSSQLContext db = new MSSQLContext();
    }
}