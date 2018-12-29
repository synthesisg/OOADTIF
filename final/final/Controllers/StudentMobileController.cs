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
                    ViewBag.mes = "";
                    var uilist = from ui in db.student where (ui.account == user && ui.password == pwd) select ui;
                    if (uilist.Count() > 0) //success
                    {
                        List<student> ui = new List<student>();
                        foreach (var u in uilist) ui.Add(u);
                        if (ui[0].is_active == 0) {
                            Session["tmp_id"] = ui[0].id;
                            return RedirectToAction("Activate");
                        }
                        else {
                            Session["user_id"] = ui[0].id;
                            Session["is_student"] = true;
                            return RedirectToAction("Seminar");
                        }
                    }
                    else { Session["is_student"] = false;
                        ViewBag.mes = "账号或密码错误";
                    }
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
            foreach (var r in rlist)
            {
                round_seminar tmp = new round_seminar(id, r.id);
                rs.Add(tmp);
            }
            ViewBag.rs = rs;
            ViewBag.seminar_id = id;
            ViewBag.TitleText = db.course.Find(k.course_id).course_name;
            return View();
        }
        public ActionResult BUEnrollSmn(int id)     //klass_seminar_id 报名
        {
            klass_seminar_enroll_state_model model = new klass_seminar_enroll_state_model(id);
            ViewBag.model = model;
            ViewBag.TitleText = model.seminar_name;
            return View();
        }
        public ActionResult BChangeEnrollSmn(int id)//klass_seminar_id 修改
        {
            klass_seminar_enroll_state_model model = new klass_seminar_enroll_state_model(id);
            ViewBag.model = model;
            ViewBag.TitleText = model.seminar_name;
            return View();
        }
        public ActionResult BQueryEnrollSmn(int id)//klass_seminar_id 查看
        {
            klass_seminar_enroll_state_model model = new klass_seminar_enroll_state_model(id);
            ViewBag.model = model;
            ViewBag.TitleText = model.seminar_name;
            return View();
        }
        public ActionResult KlassSeminar(int id)//klass_seminar_id
        {
            if (is_judge)
            {
                if (Session["is_student"] == null || (bool)Session["is_student"] == false)
                    return RedirectToAction("StudentLogin");
            }
            else Session["user_id"] = test_id;
            

            BEnrollSmn_model model = new BEnrollSmn_model(id, Int32.Parse(Session["user_id"].ToString()));
            ViewBag.model = model;

            return View();
        }
        public ActionResult courseinfor(int id)//course_id
        {
            course c = db.course.Find(id);
            ViewBag.c = c;

            //本门课人数上下限mls   返回一个member_limit_strategy或null 唯一 id 与course_id 相同(大概)
            ViewBag.mls=db.member_limit_strategy.Find(id);

            /* 所有本门课的策略 ts (T/F)
             * 返回
             * ac true代表全部 false代表符合其一
             * strccs 代表冲突课程“A|B|C”或null
             * cmlslist List<course_member_limit_strategy>模型 代表选修课人数上下限
             * cmlsname List<string>模型  对应课程名
             */
            var tslist = (from ts in db.team_strategy where ts.course_id == id select ts).ToList();
            if (tslist.Count() > 0)
            {
                ViewBag.ts = true;

                //先确定and还是or
                var tsfirst = (from ts in db.team_strategy where ts.course_id == id && ts.strategy_serial == 1 select ts).ToList()[0];
                bool ac = (tsfirst.strategy_name == "TeamAndStrategy" ? true : false);
                ViewBag.ac = ac;
                string strccs = "";
                List<course_member_limit_strategy> cmlslist = new List<course_member_limit_strategy>();
                List<string> cmlsname = new List<string>();
                foreach (var ts in tslist)
                {
                    int tid = ts.strategy_serial;
                    switch (ts.strategy_name)
                    {
                        case "ConflictCourseStrategy":
                            var ccslist = (from ccs in db.conflict_course_strategy where ccs.id == tid select ccs.course_id).ToList();
                            foreach (var cid in ccslist)
                            {
                                if (cid != id) strccs += db.course.Find(cid).course_name + '|';
                            }
                            ViewBag.strccs = strccs.Remove(strccs.Length - 1, 1);
                            break;
                        case "CourseMemberLimitStrategy":         //不唯一 针对选修xx课程的组队
                            course_member_limit_strategy cmls = db.course_member_limit_strategy.Find(tid);
                            cmlslist.Add(cmls);
                            cmlsname.Add(db.course.Find(cmls.course_id).course_name);
                            break;
                    }
                }
            }
            else
            {
                ViewBag.ts = false;
            }
            
            return View();
        }
        public ActionResult NowSmnPPT(int id)//ksid
        {
            klass_seminar_enroll_state_model model = new klass_seminar_enroll_state_model(id, 0);
            ViewBag.model = model;
            //要  ViewBag.TitleText = 课程名 + 讨论课名
            return View();
        }
        public ActionResult NowSmnDisplay(int id)//ksid 过程环节
        {
            //要  ViewBag.TitleText = 课程名 + 讨论课名
            return View();
        }
        public bool SendPW2Email(string data)
        {
            var uilist = (from ui in db.student where ui.account.Equals(data) select ui).ToList();
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
        public string Enroll(byte order,int ksid)
        {
            if (Session["is_student"] == null || (bool)Session["is_student"] == false) return "Identity verification error.";

            int sid = Int32.Parse(Session["user_id"].ToString());
            var alist = from a in db.attendance where a.klass_seminar_id == ksid && a.team_order == order select a;
            if (alist.Count() > 0) return "该位次已有人报名";

            int klass_id = db.klass_seminar.Find(ksid).klass_id;
            int team_id = new qt().k2t(klass_id,sid);
            if (team_id == 0) return "未加入队伍";

            team t = db.team.Find(team_id);
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
        public string CancelEnroll(int ksid)
        {
            int sid = Int32.Parse(Session["user_id"].ToString());
            int team_id = new qt().k2t(db.klass_seminar.Find(ksid).klass_id, sid);
            if (team_id == 0) return "no team";

            var alistteam = from a in db.attendance where a.klass_seminar_id == ksid && a.team_id == team_id select a;
            if (alistteam.Count() == 0) return "team no enroll";
            attendance at = alistteam.ToList()[0];
            db.attendance.Remove(at);
            db.SaveChanges();
            return "success";
        }

        public void score(int id)           //course_id
        {
            int sid = Int32.Parse(Request["user_id"]);

            var kslist = (from ks in db.klass_student where ks.student_id == sid && ks.course_id == id select ks).ToList();

            int klass_id = kslist[0].klass_id;
            int team_id = new qt().k2t(klass_id, sid);

            if (team_id > 0)
            {
                ViewBag.hasteam = true;
                team t = db.team.Find(team_id);
                var rque = from r in db.round where r.course_id == id select r;

                List<scoreboard> sb = new List<scoreboard>();
                foreach (var r in rque)
                {
                    scoreboard tmp = new scoreboard();
                    var rscore = from rs in db.round_score where (r.id == rs.round_id && rs.team_id == team_id) select rs;
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

            var klist = (from k in db.klass where k.course_id == id select k.id).ToList();
            var teamlistdis = from kt in db.klass_team where klist.Contains(kt.klass_id) select kt.team_id;

            int ateam_id = new qt().c2t(id, sid);
            List<teamlist> tl = new List<teamlist>();
            if (ateam_id > 0)
            {
                ViewBag.hasteam = true;
                int myteamid = ateam_id;

                teamlist tmp = new teamlist();
                tmp.team = db.team.Find(myteamid);
                var stlist = from ts in db.team_student where ts.team_id == myteamid select ts.student_id;
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
                        var stlistt = from ts in db.team_student where ts.team_id == teamid select ts.student_id;
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
                course_team ct = new course_team(id);
                ViewBag.ct = ct.list;
            }
        }
        public ActionResult CreateTeam(int id)  //course_id
        {
            var klist = (from k in db.klass where k.course_id == id select k.id).ToList();
            string[] klass_serial = new string[klist.Count()]; 
            int[] klass_id = new int[klist.Count()];
            for (int i = 0; i < klist.Count(); i++)
            {
                klass_serial[i] = new qt().k2ks(klist[i]);
                klass_id[i] = klist[i];
            }
            ViewBag.klass_serial = klass_serial;
            ViewBag.klass_id = klass_id;
            ViewBag.stulist = studentlist(id);
            ViewBag.course_id = id;
            return View();
        }
        //course下未组队学生
        public astulist studentlist(int id, string str = "")
        {
            var klist = (from k in db.klass where k.course_id == id select k.id).ToList();
            var tlist = (from kt in db.klass_team where klist.Contains(kt.klass_id) select kt.team_id).ToList();
            var slist= (from ts in db.team_student where tlist.Contains(ts.team_id) select ts.student_id).ToList();
            List<int> sidlist = (from ks in db.klass_student where ks.course_id == id && (slist.Contains(ks.student_id) == false) select ks.student_id).ToList();
            return new astulist(sidlist, str);
        }
        public string search(string sid, int cid)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(studentlist(cid, sid));
        }
        public void _createteam()//method
        {
            int klass_id = 1;
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

            klass_team Newks = new klass_team
            {
                klass_id = klass_id,
                team_id = NewTeam.id
            };
            db.klass_team.Add(Newks);
            team_student Newts = new team_student
            {
                team_id = NewTeam.id,
                student_id = sid
            };
            db.team_student.Add(Newts);
            db.SaveChanges();
        }
        public void remove()
        {
            int klass_id = 1;

            int sid = Int32.Parse(Request["user_id"]);


            int team_id = new qt().k2t(klass_id, sid);
            if (team_id==0) return;                                                                                     //本班队伍id

            if (db.team.Find(team_id).leader_id == sid)//解散
            {
                var tslist = from ts in db.team_student where ts.team_id == team_id select ts;
                foreach (var ats in tslist) db.team_student.Remove(ats);
                var ktlist = from kt in db.klass_team where kt.klass_id == klass_id && kt.team_id == team_id select kt;
                foreach (var akt in ktlist) db.klass_team.Remove(akt);
                db.team.Remove(db.team.Find(team_id));
            }
            else//单人退出
            {
                var tslist = from ts in db.team_student where ts.team_id == team_id&&ts.student_id==sid select ts;
                foreach (var ats in tslist) db.team_student.Remove(ats);
                //============================================================================================================================【退队合法校验】
            }
            db.SaveChanges();
        }
        public void add()
        {
            int team_id = 1;
            List<int> student_id = new List<int>();

            var ktlist = from kt in db.klass_team where kt.team_id == team_id select kt.klass_id;
            int klass_id = ktlist.ToList()[0];
            int course_id = db.klass.Find(klass_id).course_id;

            //============================================================================================================================【检查是否有队 略】
            foreach(int sid in student_id)
            {
                team_student Newts = new team_student
                {
                    student_id = sid,
                    team_id = team_id
                };
                db.team_student.Add(Newts);
            }
            //============================================================================================================================【入队合法校验】
            db.SaveChanges();
        }
        public void submit_team_valid()
        {
            int team_id = 1;
            team t = db.team.Find(team_id);
            if (t.status == 0)
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
            student s = db.student.Find(Int32.Parse(Session["user_id"].ToString()));
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
        int test_id = 166; //2809
        MSSQLContext db = new MSSQLContext();
    }
}