using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ooadtest4_5.Models;

/*
 *  未身份验证
 */
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
                    var alist = from a in adb.data where (a.admin_name == user && a.admin_pwd == pwd) select a.id;
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
                userinfo ui = uidb.data.Find(id);
                string newpwd = Request["inputpwd_" + Request["newpass"]];
                if (newpwd != null && newpwd != "")
                {
                    ui.password = newpwd;
                    uidb.SaveChanges();
                    Response.Write("<script type='text/javascript'>alert('Modify Success!');</script>");
                }
            }

            if (Request["Search"] != null)
            {
                string search = Request["Search"];
                ViewBag.teacherlist = from ui in uidb.data where ((ui.is_student == false) && (ui.academic_id.Contains(search) || ui.name.Contains(search))) select ui;
            }
            else
                ViewBag.teacherlist = from ui in uidb.data where (ui.is_student == false) select ui;
            return View();
        }
        public ActionResult TeacherDel(int id)
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }

            userinfo ui = uidb.data.Find(id);
            if (ui != null)
            {
                uidb.data.Remove(ui);
                uidb.SaveChanges();
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

            userinfo ui = uidb.data.Find(id);
            if (Request.HttpMethod=="POST")
            {
                ui.name = Request["name"];
                ui.academic_id = Request["academic_id"];
                ui.email = Request["email"];
                uidb.SaveChanges();
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
                userinfo ui = new userinfo
                {
                    name = Request["name"],
                    academic_id = Request["academic_id"],
                    email = Request["email"],
                    password = Request["password"],
                    is_student = false
                };
                uidb.data.Add(ui);
                uidb.SaveChanges();
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
                userinfo ui = uidb.data.Find(id);
                string newpwd = Request["inputpwd_" + Request["newpass"]];
                if (newpwd != null && newpwd != "")
                {
                    ui.password = newpwd;
                    uidb.SaveChanges();
                    Response.Write("<script type='text/javascript'>alert('Modify Success!');</script>");
                }
            }
            if (Request["Search"] != null)
            {
                string search = Request["Search"];
                ViewBag.studentlist = from ui in uidb.data where ((ui.is_student == true) && (ui.academic_id.Contains(search) || ui.name.Contains(search))) select ui;
            }
            else
                ViewBag.studentlist = from ui in uidb.data where (ui.is_student == true) select ui;
            return View();
        }
        public ActionResult StudentInfoMod(int id)
        {
            if (is_judge){
                if (Session["is_admin"] == null || (bool)Session["is_admin"] == false)
                    return Logout();
            }

            userinfo ui = uidb.data.Find(id);
            if (Request.HttpMethod == "POST")
            {
                ui.name = Request["name"];
                ui.academic_id = Request["academic_id"];
                ui.email = Request["email"];
                uidb.SaveChanges();
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
                userinfo ui = new userinfo
                {
                    name = Request["name"],
                    academic_id = Request["academic_id"],
                    email = Request["email"],
                    password = Request["password"],
                    is_student = true
                };
                uidb.data.Add(ui);
                uidb.SaveChanges();
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

            userinfo ui = uidb.data.Find(id);
            if (ui != null)
            {
                uidb.data.Remove(ui);
                uidb.SaveChanges();
                Response.Write("<script type='text/javascript'>alert('Delete Success!');</script>");
            }
            return RedirectToAction("StudentInfo");
        }
        userinfoDB uidb = new userinfoDB();
        adminDB adb = new adminDB();
        bool is_judge = false;
    }
}