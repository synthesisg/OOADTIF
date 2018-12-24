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
    public class StudentMobileController : Controller
    {
        // GET: StudentMobile
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
                    var uilist = from ui in db.student where (ui.account == user && ui.password == pwd) select ui;
                    if (uilist.Count() > 0) //success
                    {
                        List<student> ui = new List<student>();
                        foreach (var u in uilist) ui.Add(u);
                        if (ui[0].is_active == 0)
                        {
                            Session["tmp_id"] = ui[0].id;
                            return RedirectToAction("Activate");
                        }
                        else
                        {
                            Session["user_id"] = ui[0].id;
                            Session["is_student"] = true;
                            return RedirectToAction("Seminar");
                        }
                    }
                    else Session["is_student"] = false;
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
                    ui.email = Request["email"];
                    ui.is_active = 1;
                    db.SaveChanges();
                    Session["user_id"] = Session["tmp_id"];
                    Session["is_student"] = true;
                    return RedirectToAction("Seminar");
            }
            return View();
        }
        public ActionResult Seminar()
        {
            if (is_judge)
            {
                if (Session["is_student"] == null || (bool)Session["is_student"] == false)
                    return RedirectToAction("StudentLogin");
            }
            else Session["user_id"] = test_id;

            int student_id = Int32.Parse(Session["user_id"].ToString());

            var cslist = from cs in db.klass_student where cs.student_id == student_id select cs;
            List<personclass> pc = new List<personclass>();
            foreach (var cs in cslist)
            {
                klass k = db.klass.Find(cs.klass_id);
                personclass tmp = new personclass
                {
                    klass_id = cs.klass_id,
                    name = db.course.Find(cs.course_id).course_name + k.grade.ToString() + '-' + k.klass_serial.ToString()
                };
                pc.Add(tmp);
            }
            ViewBag.pc = pc;
            return View();
        }
        public ActionResult ChsSpecSeminar(int id)      //class_id
        {
            if (is_judge)
            {
                if (Session["is_student"] == null || (bool)Session["is_student"] == false)
                    return RedirectToAction("StudentLogin");
            }
            else Session["user_id"] = test_id;

            klass k = db.klass.Find(id);
            var rlist = from r in db.round where r.course_id == k.course_id select r;
            var kslist = from ks in db.klass_seminar where ks.klass_id == id select ks;

            List<round_seminar> rs = new List<round_seminar>();
            foreach(var r in rlist)
            {
                round_seminar tmp = new round_seminar
                {
                    r = r,
                };
                var slist = from s in db.seminar where s.round_id == r.id select s;
                foreach (var s in slist) tmp.s.Add(s);
                rs.Add(tmp);
            }
            ViewBag.rs = rs;
            ViewBag.seminar_id = id;
            ViewBag.TitleText = db.course.Find(k.course_id).course_name;
            return View();
        }
        public ActionResult BUEnrollSmn(int id)     //klass_seminar_id 报名界面
        {
            if (is_judge)
            {
                if (Session["is_student"] == null || (bool)Session["is_student"] == false)
                    return RedirectToAction("StudentLogin");
            }
            else Session["user_id"] = test_id;

            int student_id = Int32.Parse(Session["user_id"].ToString());
            klass_seminar_enroll_state_model model = new klass_seminar_enroll_state_model(id);
            ViewBag.model = model;
            ViewBag.TitleText = model.seminar_name;
            return View();
        }
        public ActionResult BUSmnInfo(int id)//klass_seminar_id
        {
            var ks = db.klass_seminar.Find(id);
            var s = db.seminar.Find(ks.seminar_id);
            ViewBag.ks = ks;

            ViewBag.TitleText = s.seminar_name;
            return View();
        }
        public ActionResult BEnrollSmn(int id)  //klass_seminar_id          讨论课详细信息(报名前)界面
        {
            if (is_judge)
            {
                if (Session["is_student"] == null || (bool)Session["is_student"] == false)
                    return RedirectToAction("StudentLogin");
            }
            else Session["user_id"] = test_id;

            var model = new BEnrollSmn_model(id, Int32.Parse(Session["user_id"].ToString()));
            ViewBag.model = model;

            ViewBag.TitleText = model.course_name + model.seminar_name;
            return View();
        }
        public ActionResult BChangeEnrollSmn(int id)//klass_seminar_id
        {
            klass_seminar_enroll_state_model model = new klass_seminar_enroll_state_model(id);
            ViewBag.model = model;
            ViewBag.TitleText = model.seminar_name;
            return View();
        }
        public ActionResult NowUEnrollSmnInfo() {
            //要  ViewBag.TitleText =课程名 + 讨论课名
            return View();
        }
        public ActionResult NowSmnPPT() {
            //要  ViewBag.TitleText = 课程名 + 讨论课名
            return View();
        }
        public ActionResult NowSmnDisplay() {
            //要  ViewBag.TitleText = 课程名 + 讨论课名
            return View();
        }
        public ActionResult NowEnrollSmnInfo() {
            //要  ViewBag.TitleText =课程名 + 讨论课名
            return View();
        }
        public ActionResult LaterUErollSmnInfo() {
            //要  ViewBag.TitleText =课程名 + 讨论课名
            return View();
        }
        public ActionResult LaterEnrollSmnInfo() {
            //要  ViewBag.TitleText =课程名 + 讨论课名
            return View();
        }
        public ActionResult LaterEnrollEndSmnInfo() {
            //要  ViewBag.TitleText =课程名 + 讨论课名
            return View();
        }
        public bool SendPW2Email(string data)
        {
            var uilist = (from ui in db.student where ui.account.Equals(data) select ui).ToList();
            if (uilist.Count() == 0) return false;
            string email = uilist.ToList()[0].email;
            if (email == null || email == "") return false;

            SmtpClient client = new SmtpClient("smtp.163.com", 25);
            MailMessage msg = new MailMessage("13600858179@163.com", email, "找回瓜皮账户", "感谢使用瓜皮课堂\n您的账号"+uilist[0].account+"的password为" + uilist[0].password);
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicAuthenticationInfo =
                new System.Net.NetworkCredential("13600858179@163.com", "20100710A");
            client.Credentials = basicAuthenticationInfo;
            client.EnableSsl = true;
            client.Send(msg);

            return true;
        }
        public string Move2CourSeminar(string str) {
            return "a";
        }
        public string Enroll(string str)
        {
            byte order = (byte)Int32.Parse(str);
            if (Session["klass_seminar_id"] == null || Session["klass_seminar_id"].ToString() == "") return "fail";
            int ksid = Int32.Parse(Session["klass_seminar_id"].ToString());

            var alist = from a in db.attendance where a.klass_seminar_id == ksid && a.team_order==order select a;
            if (alist.Count() > 0) return "fail";

            int student_id = Int32.Parse(Session["user_id"].ToString());
            var teamlist = from kst in db.klass_student where kst.student_id == student_id select kst;
            team t = db.team.Find(teamlist.ToList()[0].team_id);
            attendance tmp = new attendance
            {
                klass_seminar_id = ksid,
                team_id = t.id,
                team_order = order
            };
            db.attendance.Add(tmp);
            db.SaveChanges();
            return "success";
        }

        public void score(int id)           //course_id
        {
            int sid = Int32.Parse(Request["user_id"]);
            var team_id_que = from ks in db.klass_student where ks.student_id == sid select ks;
            if (team_id_que.Count() > 0)
            {
                ViewBag.hasteam = true;

                int team_id = (int)team_id_que.ToList()[0].team_id;
                team t = db.team.Find(team_id);
                var rque = from r in db.round where r.course_id == id select r;

                List<scoreboard> sb = new List<scoreboard>();
                foreach (var r in rque)
                {
                    scoreboard tmp = new scoreboard();
                    var rscore = from rs in db.round_score where (r.id==rs.round_id && rs.team_id == team_id) select rs;
                    tmp.rs = rscore.ToList()[0];

                    var ksrlist = from ksr in db.klass_seminar where ksr.klass_id == t.klass_id select ksr;
                    foreach (var ksr in ksrlist)
                    {
                        var sslist = from ss in db.seminar_score where ss.team_id == team_id && ss.klass_seminar_id == ksr.id select ss;
                        tmp.ss.Add(sslist.ToList()[0]);
                        tmp.name.Add(db.seminar.Find(db.klass_seminar.Find(sslist.ToList()[0].klass_seminar_id).seminar_id).seminar_name);
                    }
                    sb.Add(tmp);
                }
                ViewBag.sb = sb;
            }
            else
            {
                ViewBag.hasteam = false;
            }
        }
        public void team(int id)        //course_id
        {
            int sid = Int32.Parse(Request["user_id"]);
            var teamlist = from ks in db.klass_student where ks.course_id == id select ks.team_id;
            var teamlistdis = teamlist.ToList().Distinct();
            var team_id_que = from ks in db.klass_student where ks.student_id == sid select ks;
            List<teamlist> tl = new List<teamlist>();
            if (team_id_que.Count() > 0)
            {
                ViewBag.hasteam = true;
                int myteamid = (int)team_id_que.ToList()[0].team_id;

                teamlist tmp = new teamlist();
                tmp.team = db.team.Find(myteamid);
                var stlist = from ks in db.klass_student where ks.course_id == id && ks.team_id == myteamid select ks.student_id;
                foreach (var st in stlist)
                {
                    tmp.name.Add(db.student.Find(st).student_name);
                    tmp.account.Add(db.student.Find(st).account);
                }
                tl.Add(tmp);

                foreach (var teamid in teamlistdis)
                    if (teamid != myteamid)
                    {
                        teamlist tmpp = new teamlist();
                        tmpp.team = db.team.Find(teamid);
                        var stlistt = from ks in db.klass_student where ks.course_id == id && ks.team_id == teamid select ks.student_id;
                        foreach (var st in stlistt)
                        {
                            tmpp.name.Add(db.student.Find(st).student_name);
                            tmpp.account.Add(db.student.Find(st).account);
                        }
                        tl.Add(tmpp);
                    }
                ViewBag.ct = tl;
            }
            else
            {
                ViewBag.hasteam = false;
                course_team ct=new course_team(id);
                ViewBag.ct = ct.list;
            }
        }
        public void studentlist(int id)        //course_id
        {
            var stlist = from ks in db.klass_student where ks.course_id == id && ks.team_id == null select ks.student_id;
        }
        public void createteam()
        {
            int klass_id=1;
            string team_name = "";

            int course_id = db.klass.Find(klass_id).course_id;
            var tlist = from t in db.team where t.course_id == course_id select t;
            int sid = Int32.Parse(Request["user_id"]);
            team NewTeam = new team
            {
                klass_id = klass_id,
                course_id = course_id,
                leader_id = sid,
                team_name = team_name,
                team_serial = (byte)(tlist.Count() + 1),
                status = 0
            };
            db.team.Add(NewTeam);
            db.SaveChanges();

            var kslist = from ks in db.klass_student where ks.course_id == course_id && ks.student_id == sid select ks;
            foreach (var ks in kslist) ks.team_id = NewTeam.id;
            db.SaveChanges();
        }
        public void remove()
        {
            int course_id = 1;

            int sid = Int32.Parse(Request["user_id"]);
            var ks = (from aks in db.klass_student where aks.course_id == course_id && aks.student_id == sid select aks).ToList()[0];
            if (ks.team_id == null) return;
            int team_id = (int)ks.team_id;
            if(db.team.Find(team_id).leader_id==sid)
            {
                var kslist = (from aks in db.klass_student where aks.course_id == course_id && aks.team_id == team_id select aks).ToList();
                foreach (var aks in kslist) aks.team_id = null;
            }
            else
                ks.team_id = null;
                db.SaveChanges();
        }
        public void add()
        {
            int course_id = 1;
            int team_id = 1;
            List<int> student_id = new List<int>();
            
            var kslist = from aks in db.klass_student where aks.course_id == course_id && student_id.Contains(aks.student_id) select aks;
            foreach (var ks in kslist)
            {
                if (ks.team_id == null) ks.team_id = team_id;
            }
            db.SaveChanges();
        }
        public void submit_team_valid()
        {
            int team_id = 1;
            team t = db.team.Find(team_id);
            if (t.status==0)
            {
                team_valid_application Newtva = new team_valid_application
                {
                    team_id = team_id,
                    reason = Request["reason"],
                    status = null,
                    teacher_id = db.course.Find(t.course_id).teacher_id
                };
                db.team_valid_application.Add(Newtva);
                db.SaveChanges();
            }
        }

        public bool chgpwd(string data)
        {
            if (Session["is_student"] == null || (bool)Session["is_student"] == false) return false;
            student s=db.student.Find(Int32.Parse(Session["user_id"].ToString()));
            s.password = data;
            db.SaveChanges();
            return true;
        }
        public bool chgemail(string data)
        {
            if (Session["is_student"] == null || (bool)Session["is_student"] == false) return false;
            student s = db.student.Find(Int32.Parse(Session["user_id"].ToString()));
            s.email = data;
            db.SaveChanges();
            return true;
        }

        
        bool is_judge = false;
        int test_id = 6;
        MSSQLContext db = new MSSQLContext();
    }
}