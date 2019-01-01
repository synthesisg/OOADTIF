using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using final.Models;

//mail
using System.Net;
using System.Net.Mail;
namespace final.Controllers
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
                    teacher ui = db.teacher.Find(Int32.Parse(Session["tmp_id"].ToString()));
                    ui.password = Request["newPassword"];
                    ui.is_active = 1;
                    db.SaveChanges();
                    Session["user_id"] = Session["tmp_id"];
                    Session["is_teacher"] = true;
                    return RedirectToAction("ChsLesson");
            }
            return View();
        }
        public bool SendPW2Email(string data)
        {
            var uilist = (from ui in db.teacher where ui.account.Equals(data) select ui).ToList();
            if (uilist.Count() == 0) return false;
            string email = uilist.ToList()[0].email;
            if (email == null || email == "") return false;

            SmtpClient client = new SmtpClient("smtp.163.com", 25);
            MailMessage msg = new MailMessage("13600858179@163.com", email, "找回瓜皮账户", "感谢使用瓜皮课堂\n您的账号" + uilist[0].account + "的password为" + uilist[0].password);
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicAuthenticationInfo =
                new System.Net.NetworkCredential("13600858179@163.com", "20100710A");
            client.Credentials = basicAuthenticationInfo;
            client.EnableSsl = true;
            client.Send(msg);

            return true;
        }
        //仅course 后续跳转至chsseminar
        public ActionResult Seminar()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("Login");
            }
            else Session["user_id"] = test_id;

            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("Login");
            }
            else Session["user_id"] = test_id;


            int tid = Int32.Parse(Session["user_id"].ToString());
            var colist = (from co in db.course where co.teacher_id == tid select co).ToList();
            ViewBag.colist = colist;
            return View();
        }

        public ActionResult ChsSpecSeminar(int id)//course_id 返回round+seminar
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("Login");
            }
            else Session["user_id"] = test_id;

            int tid = Int32.Parse(Session["user_id"].ToString());
            course_seminar cs = new course_seminar(id);
            ViewBag.cs = cs;
            ViewBag.TitleText = cs.course.course_name;
            return View();
        }
        public ActionResult ChsSpecKlass(int id)//seminar_id
        {
            var kslist = (from ks in db.klass_seminar where ks.seminar_id == id select ks).ToList();
            List<string> serial = new List<string>();
            foreach (var ks in kslist) serial.Add(new qt().k2ks(ks.klass_id));
            seminar s = db.seminar.Find(id);
            ViewBag.s = s;
            ViewBag.ks = kslist;
            ViewBag.serial = serial;
            ViewBag.TitleText = db.seminar.Find(id).seminar_name;
            return View();
        }
        public ActionResult SetSeminarSerial(int id)//round_id
        {
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    ViewBag.seminar_name = (from s in db.seminar where s.round_id == id select s.seminar_name).ToList();
                    ViewBag.round = db.round.Find(id);
                    var krlist = (from kr in db.klass_round where kr.round_id == id select kr).ToList();
                    List<string> klass_serial = new List<string>();
                    List<int> enroll_number = new List<int>();
                    List<int> klassid = new List<int>();
                    foreach (var kr in krlist)
                    {
                        klass_serial.Add(new qt().k2ks(kr.klass_id));
                        enroll_number.Add((int)kr.enroll_number);
                        klassid.Add(kr.klass_id);
                    }
                    ViewBag.klass_serial = klass_serial;
                    ViewBag.enroll_number = enroll_number;
                    ViewBag.klassid = klassid;
                    return View();

                case "POST":        //modify
                    round r = db.round.Find(id);
                    r.presentation_score_method = byte.Parse(Request["presentation_score_method"]);
                    r.question_score_method = byte.Parse(Request["question_score_method"]);
                    r.report_score_method = byte.Parse(Request["report_score_method"]);

                    var krdlist = (from kr in db.klass_round where kr.round_id == id select kr).ToList();
                    foreach (var kr in krdlist)
                    {
                        kr.enroll_number = byte.Parse(Request["k_" + kr.klass_id.ToString()]);
                    }
                    db.SaveChanges();
                    return RedirectToAction("SetSeminarSerial");
            }

            return View();
        }
        public ActionResult StudentScore2() { return View(); }//===============================================================[???]===========================
        public ActionResult QueryEnrollSmn(int id)//klass_seminar_id 查看
        {
            klass_seminar_enroll_state_model model = new klass_seminar_enroll_state_model(id);
            ViewBag.model = model;
            ViewBag.TitleText = model.seminar_name;
            return View();
        }
        public ActionResult MarkReport(int id)      //ksid
        {
            switch (Request.HttpMethod)
            {
                case "GET":
                    ViewBag.model = new seminar_report("ksid", id);
                    break;
                case "POST":
                    seminar_report sr= new seminar_report("ksid", id);

                    foreach (var item in sr.list)
                    {
                        string str = Request[item.id.ToString()];
                        decimal? score = (str == null ? null : (decimal?) decimal.Parse(str));

                        int team_id = db.attendance.Find(item.id).team_id;

                        var sslist = (from ss in db.seminar_score where ss.klass_seminar_id == id && ss.team_id == team_id select ss).ToList();
                        if (sslist.Count() > 0)
                        {
                            sslist[0].report_score = score;
                        }
                        else
                        {
                            seminar_score NewSS = new seminar_score { klass_seminar_id = id, report_score = score, team_id = team_id };
                            db.seminar_score.Add(NewSS);
                        }
                        db.SaveChanges();

                        UpdateScore us = new UpdateScore();
                        us.UpdateKlassSeminarScore(id);
                        us.UpdateRoundScore(db.seminar.Find(db.klass_seminar.Find(id).seminar_id).round_id);
                    }
                    break;
            }
                    return View();
        }
        public ActionResult CheckModAllMark() {
            return View();
        }
        public ActionResult NowSmnDisplay() {
            return View();
        }
        public ActionResult modSeminar(int id)//seminar_id
        {
            switch (Request.HttpMethod)
            {
                case "GET":
                    var rlist = (from r in db.round where r.course_id == id select r).ToList();
                    ViewBag.rlist = rlist;
                    seminar seminar = db.seminar.Find(id);
                    ViewBag.seminar = seminar;
                    ViewBag.enroll_start_time = ((DateTime)seminar.enroll_start_time).ToString("yyyy-MM-ddTHH:mm:ss.fff");
                    ViewBag.enroll_end_time = ((DateTime)seminar.enroll_end_time).ToString("yyyy-MM-ddTHH:mm:ss.fff");
                    var kslist = (from ks in db.klass_seminar where ks.seminar_id == id select ks).ToList();
                    List<string> name = new List<string>();
                    List<string> date = new List<string>();
                    List<int> ksid = new List<int>();
                    foreach(var ks in kslist)
                    {
                        name.Add(new qt().k2ks(ks.klass_id));
                        if (ks.report_ddl != null)
                            date.Add(((DateTime)ks.report_ddl).ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                        else date.Add(null);
                        ksid.Add(ks.id);
                    }
                    ViewBag.name = name;
                    ViewBag.date = date;
                    ViewBag.ksid = ksid;
                    break;
                case "POST":
                    int rid = Int32.Parse(Request["roundInfo"]);
                    byte vis = 0;
                    string str = Request["visible"];
                    if (str != null) vis = 1;

                    if (rid == 0)
                    {
                        int cnt = (from r in db.round where r.course_id == id select r).Count() + 1;
                        round NewRound = new round
                        {
                            course_id = id,
                            presentation_score_method = 0,
                            question_score_method = 0,
                            report_score_method = 0,
                            round_serial = (byte)cnt
                        };
                        db.round.Add(NewRound);
                        db.SaveChanges();
                        rid = NewRound.id;
                    }
                    seminar s = db.seminar.Find(id);
                    s.enroll_start_time = Convert.ToDateTime(Request["start_date"].Replace("T"," "));
                    s.enroll_end_time = Convert.ToDateTime(Request["end_date"].Replace("T", " "));
                    s.introduction = Request["content"];
                    s.seminar_name = Request["title"];
                    s.max_team = byte.Parse(Request["groupCount"]);
                    s.round_id = rid;
                    s.is_visible = vis;
                    var ksrlist = (from ks in db.klass_seminar where ks.seminar_id == id select ks).ToList();
                    foreach (var ks in ksrlist)
                        ks.report_ddl = Convert.ToDateTime(Request["ks_" + ks.id.ToString()]);
                    db.SaveChanges();

                    //CreateKlassRound
                    List<int> ridlist = (from r in db.round where r.course_id == id select r.id).ToList();
                    List<int> kidlist = (from k in db.klass where k.course_id == id select k.id).ToList();
                    foreach(var i in ridlist)
                        foreach(var j in kidlist)
                        {
                            int cnt = (from kr in db.klass_round where kr.round_id == i && kr.klass_id == j select kr).Count();
                            if (cnt == 0) 
                            {
                                klass_round NewKR = new klass_round { round_id = i, klass_id = j, enroll_number = 1 };
                                db.klass_round.Add(NewKR);
                            }
                        }
                    db.SaveChanges();

                    return RedirectToAction("/Seminar");
                    break;
            }
            return View();
        }
        public ActionResult InSeminar(int id)//ksid
        {
            ViewBag.seminar_name = db.seminar.Find(db.klass_seminar.Find(id).seminar_id).seminar_name;
            return View();
        }
        public ActionResult CreateSeminar(int id)//course_id
        {
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    var rlist = (from r in db.round where r.course_id == id select r).ToList();
                    ViewBag.rlist = rlist;
                    return View();
                case "POST":        //create
                    int rid = Int32.Parse(Request["roundInfo"]);
                    byte vis = 0;
                    string str = Request["visible"];
                    if (str != null) vis = 1;

                    if (rid == 0)
                    {
                        int cnt = (from r in db.round where r.course_id == id select r).Count() + 1;
                        round NewRound = new round
                        {
                            course_id = id,
                            presentation_score_method = 0,
                            question_score_method = 0,
                            report_score_method = 0,
                            round_serial = (byte)cnt
                        };
                        db.round.Add(NewRound);
                        db.SaveChanges();
                        rid = NewRound.id;
                    }
                    int serial = (from s in db.seminar where s.course_id == id select s).Count() + 1;
                    seminar NewSeminar = new seminar
                    {
                        course_id = id,
                        enroll_start_time = Convert.ToDateTime(Request["start_date"]),
                        enroll_end_time = Convert.ToDateTime(Request["end_date"]),
                        introduction = Request["content"],
                        seminar_name = Request["title"],
                        max_team = byte.Parse(Request["groupCount"]),
                        round_id = rid,
                        seminar_serial = (byte)serial,
                        is_visible = vis
                    };
                    db.seminar.Add(NewSeminar);
                    db.SaveChanges();

                    //CreateKlassRound
                    List<int> ridlist = (from r in db.round where r.course_id == id select r.id).ToList();
                    List<int> kidlist = (from k in db.klass where k.course_id == id select k.id).ToList();
                    foreach(var i in ridlist)
                        foreach(var j in kidlist)
                        {
                            int cnt = (from kr in db.klass_round where kr.round_id == i && kr.klass_id == j select kr).Count();
                            if(cnt==0)
                            {
                                klass_round NewKR = new klass_round { round_id = i, klass_id = j, enroll_number = 1 };
                                db.klass_round.Add(NewKR);
                            }
                        }
                    db.SaveChanges();
                    return RedirectToAction("/Seminar");
            }
            return View();
        }
        public ActionResult CreateClass(int id)//course_id
        {
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    return View();
                case "POST":        //create
                    klass NewKlass = new klass
                    {
                        course_id = id,
                        grade = long.Parse(Request["grade"].ToString()),
                        klass_serial = byte.Parse(Request["klass_serial"].ToString()),
                        klass_location = Request["klassPlace"].ToString(),
                        klass_time = Request["klassTime"].ToString()
                    };
                    db.klass.Add(NewKlass);
                    return View();
            }
            return View();
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
        public ActionResult KlassSeminar(int id)//ksid
        {
            BEnrollSmn_model model = new BEnrollSmn_model(id, 0);
            ViewBag.model = model;
            return View();
        }
        public ActionResult SetReportDDL(int id)//ksid
        {
            return View();
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
            foreach (var q in qlist)
            {
                int team_id = new qt().k2t(ks.klass_id, q.student_id);
                if (cnt.ContainsKey(team_id))
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
            foreach (var tmp in score)
            {
                if (!method) score[tmp.Key] /= cnt[tmp.Key];
                var sslist = from ss in db.seminar_score where ss.klass_seminar_id == id && ss.team_id == tmp.Key select ss;
                if (sslist.Count() > 0)
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

        public bool DelSeminar(int seminarId)
        {
            return new Del().DelSeminar(seminarId);
        }
        public bool DelKlass(int id)
        {
            return new Del().DelKlass(id);
        }
        public bool DelCourse(int id)
        {
            return new Del().DelCourse(id);
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
        int test_id = 3;//qm
        MSSQLContext db = new MSSQLContext();
    }
}