﻿using System;
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
        public ActionResult Seminar(int id)//course_id
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("Login");
            }
            else Session["user_id"] = 1;

            int tid = Int32.Parse(Session["user_id"].ToString());
            course_seminar cs = new course_seminar(id);
            ViewBag.cs = cs;
            return View();
        }
        public ActionResult Course()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("Login");
            }
            else Session["user_id"] = 1;


            int tid = Int32.Parse(Session["user_id"].ToString());
            var colist = from co in db.course where co.teacher_id == tid select co;
            ViewBag.co = colist;
            return View();
        }

        public void createclass()
        {
            int course_id=1;
            klass NewKlass = new klass
            {
                course_id = course_id,
                grade = long.Parse(Request["grade"].ToString()),
                klass_serial = byte.Parse(Request["klass_serial"].ToString()),
                klass_location = Request["klass_location"].ToString(),
                klass_time = Request["klass_time"].ToString()
            };
            db.klass.Add(NewKlass);
        }
        public void createcourse()
        {
            int teacher_id = 1;
            course NewCourse = new course
            {
                teacher_id = teacher_id,
                course_name = Request["course_name"],
                introduction = Request["introduction"],
                presentation_percentage = byte.Parse(Request["presentation_percentage"]),
                question_percentage = byte.Parse(Request["question_percentage"]),
                report_percentage = byte.Parse(Request["report_percentage"]),
                team_start_time = Convert.ToDateTime(Request["team_start_time"]),
                team_end_time = Convert.ToDateTime(Request["team_end_time"])
            };
            db.course.Add(NewCourse);
            //人数 冲突 未实现
            //两种策略:
            //member_limit_strategy 队伍人数设置
            //course_member_limit_strategy 队伍中选该课程人数
            //team_and_strategy team_or_strategy  与或 对应策略
            //team_strategy 每门课必须一个

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

        //讨论课过程结束后仅能生成提问分数？
        void updatetoseminarscore(int id)  //klass_seminar_id   klass_seminar->seminar_score
        {
            klass_seminar ks = db.klass_seminar.Find(id);
            round r = db.round.Find(db.seminar.Find(ks.seminar_id).round_id);
            bool method = false;
            if (r.question_score_method == 1) method = true;    //true为取max
            var alist = from a in db.attendance where a.klass_seminar_id == id select a.id;
            var aid = alist.ToList();
            var qlist = from q in db.question where aid.Contains(q.attendance_id) && q.is_selected == 1 select q;
            Dictionary<int, decimal> score = new Dictionary<int, decimal>();
            Dictionary<int, int> cnt = new Dictionary<int, int>();
            foreach(var q in qlist)
            {
                int team_id = (int)(from kst in db.klass_student where kst.student_id == q.student_id && kst.klass_id == ks.klass_id select kst.team_id).ToList()[0];
                if(cnt.ContainsKey(team_id))
                {
                    cnt[team_id]++;
                    if (method) score[team_id] = Math.Max((decimal)q.score, score[team_id]);
                    else score[team_id] += (decimal)q.score;
                }
                else
                {
                    cnt[team_id] = 1;
                    score[team_id] = (decimal)q.score;
                }
            }
            foreach(var tmp in score)
            {
                if (!method) score[tmp.Key] /= cnt[tmp.Key];
                var sslist = from ss in db.seminar_score where ss.klass_seminar_id == id && ss.team_id == tmp.Key select ss;
                if(sslist.Count()>0)
                    sslist.ToList()[0].question_score = tmp.Value;
                else
                {
                    seminar_score Newss = new seminar_score
                    {
                        klass_seminar_id = id,
                        question_score = tmp.Value,
                        team_id = tmp.Key
                    };
                    db.seminar_score.Add(Newss);
                }
            }
            db.SaveChanges();
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