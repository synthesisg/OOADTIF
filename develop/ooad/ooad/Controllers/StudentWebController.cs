using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ooad.Models;

//xls
using System.Data;
using System.Data.OleDb;

namespace ooadtest4_5.Controllers
{
    public class StudentWebController : Controller {
        // GET: StudentWeb
        public ActionResult Index() {
            return View();
        }
        public ActionResult StudentLogin() {
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    return View();
                case "POST":        //login
                    string user = Request["userName"];
                    string pwd = Request["userPassword"];
                    var uilist = from ui in db.student where (ui.account == user && ui.password == pwd) select ui;
                    if (uilist.Count() > 0) //success
                    {
                        List<student> ui = new List<student>();
                        foreach (var u in uilist) ui.Add(u);
                        if (ui[0].is_active == 0)
                        {
                            Response.Write("<script type='text/javascript'>alert('Not Active!');</script>");
                        }
                        else
                        {
                            Session["user_id"] = ui[0].id;
                            Session["is_student"] = true;
                            Response.Redirect("ChsLesson");
                        }
                    }
                    else Session["is_student"] = false;
                    break;
            }
            return View();
        }
        public ActionResult ChsLesson() {
            if (is_judge) {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("TeacherLogin");
            }
            else Session["user_id"] = 11;


            int tid = Int32.Parse(Session["user_id"].ToString());
            var colist = from cse in db.course where cse.teacher_id == tid select cse;
            ViewBag.colist = colist;

            string n = Request["round"];
            //数据库查询第n轮讨论课的信息并用ViewBag返回
            ViewBag.obMsg = new SeminarInfo {
                round = 1,
                count = 1,
                title = "需求分析",
                msg = "讨论课信息",
                signUpTime = "报名起止时间",
                reportTime = "报告截止时间"
            };
            return View();
        }
        public ActionResult SpecificSeminar(){
            //这里要讨论课数据，根据传的round和n来
            return View();
        }
        public ActionResult DownloadMarks() {
            if (is_judge) {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("TeacherLogin");
            }
            else Session["user_id"] = 11;


            int tid = Int32.Parse(Session["user_id"].ToString());
            var colist = from cse in db.course where cse.teacher_id == tid select cse;
            ViewBag.colist = colist;
            return View();
        }
        public class SeminarInfo
        {
            public int round;
            public int count;
            public string title;
            public string msg;
            public string signUpTime;
            public string reportTime;
        }
        bool is_judge = false;
        MySQLContext db = new MySQLContext();
    }
}