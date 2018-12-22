using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ooad.Models;
namespace ooad.Controllers
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
                    var alist = from a in db.admin where (a.account == user && a.password == pwd) select a.id;
                    if ((user == "123" && pwd == "456")||(alist.Count()>0)) //success
                    {
                        Session["is_admin"] = true;
                        Response.Redirect("TeacherInfo");
                    }
                    else Session["is_admin"] = false;
                    break;
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["is_admin"] = false;
            return RedirectToAction("Login");
        }
        public ActionResult TeacherInfo()
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }


            if (Request.HttpMethod == "POST")
            {
                string sid = Request["newpass"];
                int id = Convert.ToInt32(sid);
                teacher ui = db.teacher.Find(id);
                string newpwd = Request["inputpwd_" + Request["newpass"]];
                if (newpwd != null && newpwd != "")
                {
                    ui.password = newpwd;
                    db.SaveChanges();
                    Response.Write("<script type='text/javascript'>alert('Modify Success!');</script>");
                }
            }

            if (Request["Search"] != null)
            {
                string search = Request["Search"];
                ViewBag.teacherlist = from ui in db.teacher where ((ui.account.Contains(search) || ui.teacher_name.Contains(search))) select ui;
            }
            else
                ViewBag.teacherlist = from ui in db.teacher select ui;
            return View();
        }
        public ActionResult TeacherDel(int id)
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }

            teacher ui = db.teacher.Find(id);
            if (ui != null)
            {
                db.teacher.Remove(ui);
                db.SaveChanges();
                Response.Write("<script type='text/javascript'>alert('Delete Success!');</script>");
            }
            return RedirectToAction("TeacherInfo");
        }
        public ActionResult TeacherInfoMod(int id)
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }

            teacher ui = db.teacher.Find(id);
            if (Request.HttpMethod=="POST")
            {
                ui.teacher_name = Request["name"];
                ui.account = Request["academic_id"];
                ui.email = Request["email"];
                db.SaveChanges();
                Response.Write("<script type='text/javascript'>alert('Success!');</script>");
            }
            ViewBag.uimod = ui;
            return View();
        }
        public ActionResult TeacherCreate()
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }

            if (Request.HttpMethod=="POST")
            {
                teacher ui = new teacher
                {
                    teacher_name = Request["name"],
                    account = Request["academic_id"],
                    email = Request["email"],
                    password = Request["password"],
                    is_active = 0
                };
                db.teacher.Add(ui);
                db.SaveChanges();
                Response.Write("<script type='text/javascript'>alert(' Create Success!');</script>");
                return RedirectToAction("TeacherInfo");
            }
            return View();
        }

        public ActionResult StudentInfo()
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }

            if (Request.HttpMethod == "POST")
            {
                string sid = Request["newpass"];
                int id = Convert.ToInt32(sid);
                student ui = db.student.Find(id);
                string newpwd = Request["inputpwd_" + Request["newpass"]];
                if (newpwd != null && newpwd != "")
                {
                    ui.password = newpwd;
                    db.SaveChanges();
                    Response.Write("<script type='text/javascript'>alert('Modify Success!');</script>");
                }
            }
            if (Request["Search"] != null)
            {
                string search = Request["Search"];
                ViewBag.studentlist = from ui in db.student where ((ui.account.Contains(search) || ui.student_name.Contains(search))) select ui;
            }
            else
                ViewBag.studentlist = from ui in db.student select ui;
            return View();
        }
        public ActionResult StudentInfoMod(int id)
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }

            student ui = db.student.Find(id);
            if (Request.HttpMethod == "POST")
            {
                ui.student_name = Request["name"];
                ui.account = Request["academic_id"];
                ui.email = Request["email"];
                db.SaveChanges();
                Response.Write("<script type='text/javascript'>alert('Success!');</script>");
            }
            ViewBag.uimod = ui;
            return View();
        }
        public ActionResult StudentCreate()
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }

            if (Request.HttpMethod == "POST")
            {
                student ui = new student
                {
                    student_name = Request["name"],
                    account = Request["academic_id"],
                    email = Request["email"],
                    password = Request["password"],
                    is_active = 0
                };
                db.student.Add(ui);
                db.SaveChanges();
                Response.Write("<script type='text/javascript'>alert(' Create Success!');</script>");
                return RedirectToAction("StudentInfo");
            }
            return View();
        }
        public ActionResult StudentDel(int id)
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }

            student ui = db.student.Find(id);
            if (ui != null)
            {
                db.student.Remove(ui);
                db.SaveChanges();
                Response.Write("<script type='text/javascript'>alert('Delete Success!');</script>");
            }
            return RedirectToAction("StudentInfo");
        }
        MSSQLContext db = new MSSQLContext();
        bool is_judge = true;
    }
}