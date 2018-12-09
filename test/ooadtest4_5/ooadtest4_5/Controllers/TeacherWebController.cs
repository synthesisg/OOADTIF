using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ooadtest4_5.Models;
namespace ooadtest4_5.Controllers
{
    public class TeacherWebController : Controller
    {
        // GET: TeacherWeb
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TeacherLogin()
        {
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    return View();
                case "POST":        //login
                    string user = Request["userName"];
                    string pwd = Request["userPassword"];
                    var uilist = from ui in uidb.data where (ui.academic_id == user && ui.password == pwd &&ui.is_student!=true) select ui;
                    if (uilist.Count()>0) //success
                    {
                        List<userinfo> ui = new List<userinfo>();
                        foreach (var u in uilist) ui.Add(u);
                        if (ui[0].is_valid != true)
                        {
                            Response.Write("<script type='text/javascript'>alert('Not Active!');</script>");// 跳不出来
                            //return RedirectToAction("TeacherLogin");
                        }
                        else
                        {
                            Session["user_id"] = ui[0].id;
                            Session["is_teacher"] = true;
                            Response.Redirect("TeacherImport");
                        }
                    }
                    else Session["is_teacher"] = false;
                    break;
            }
            return View();
        }
        public ActionResult TeacherImport()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("TeacherLogin");
            }
            else Session["user_id"] = 11;


            int tid = Int32.Parse(Session["user_id"].ToString());
            var colist = from cse in codb.data where cse.teacher_id == tid select cse;
            string[] c1list = new string[colist.Count()];
            int cnt = 0;
            foreach (var co in colist)
            {
                c1list[cnt] = "";
                int coid = co.id;
                var ac1list = from c1 in c1db.data where c1.course_id == coid select c1;
                foreach (var a in ac1list) c1list[cnt] += a.name + '|';
                c1list[cnt]= c1list[cnt].Remove(c1list[cnt].Length - 1, 1);
                cnt++;
            }
            ViewBag.colist = colist;
            ViewBag.c1list = c1list;
            return View();
        }
        public ActionResult DownloadReport()
        {
            SeminarInfo smnInfo = new SeminarInfo
            {
                round = 1,
                title = "需求设计",
                msg = "这是第一次讨论课信息"
            };
            SeminarInfo smnInfo2 = new SeminarInfo
            {
                round = 2,
                title = "界面设计",
                msg = "这是第二次讨论课信息"
            };
            SeminarInfo[] arrSmn = { smnInfo, smnInfo2 };
            ViewBag.arrSeminar = arrSmn;
            return View();
        }

        public ActionResult InnerReport()
        {
            string n = Request["round"];
            //数据库查询第n次讨论课的信息并用ViewBag返回
            ViewBag.obMsg = new SeminarInfo
            {
                round = 1,
                title = "需求分析",
                msg = "讨论课信息",
                signUpTime = "报名起止时间",
                reportTime = "报告截止时间"
            };
            return View();
        }
        public ActionResult DownloadMarks()
        {
            return View();
        }


        public class SeminarInfo
        {
            public int round;
            public string title;
            public string msg;
            public string signUpTime;
            public string reportTime;
        }
        public class Student
        {
            public string name { get; set; }
        }

        userinfoDB uidb = new userinfoDB();
        courseDB codb = new courseDB();
        class1DB c1db = new class1DB();
        bool is_judge = false;
    }
}