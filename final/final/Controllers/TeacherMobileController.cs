using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using final.Models;
using System.Data;

//xls
using System.Data.OleDb;

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
                            return RedirectToAction("TeacherMyCourse");
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
                return Redirect("/Home/MobileLogin");
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
        //仅course 后续跳转至chsseminar
        public ActionResult Seminar()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
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
                    return Redirect("/Home/MobileLogin");
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
        public ActionResult CheckModAllMark(int id)//ksid
        {
            if(Request.HttpMethod=="POST")
            {
                int tid = Int32.Parse(Request["tid"]);
                var modss = (from ss in db.seminar_score where ss.klass_seminar_id == id && ss.team_id == tid select ss).ToList()[0];
                modss.presentation_score = decimal.Parse(Request["presc"]);
                modss.question_score = decimal.Parse(Request["quesc"]);
                modss.report_score = decimal.Parse(Request["repsc"]);
                db.SaveChanges();
            }

            var sslist = (from ss in db.seminar_score where ss.klass_seminar_id == id select ss).ToList();
            List<string> team_serial = new List<string>();
            foreach (var ss in sslist) team_serial.Add(new qt().t2ts(ss.team_id));
            ViewBag.sslist = sslist;
            ViewBag.team_serial = team_serial;
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
                        ks.report_ddl = Convert.ToDateTime(Request["ks_" + ks.id.ToString()].Replace("T", " "));
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
            }
            return View();
        }
        public ActionResult InSeminar(int id)//ksid
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;


            int tid = Int32.Parse(Session["user_id"].ToString());

            seminar seminar = db.seminar.Find(db.klass_seminar.Find(id).seminar_id);
            ViewBag.TitleText = seminar.seminar_name;
            ViewBag.seminar_name = seminar.seminar_name;
            var alist = (from a in db.attendance where a.is_present == 1 && a.klass_seminar_id == id select a).ToList();
            if (alist.Count() > 0)
            {
                ViewBag.NowAttend = new qt().t2ts(alist[0].team_id);
                ViewBag.NowQue = (from q in db.question where q.is_selected != 1 && q.attendance_id == alist[0].id select q).Count();
            }
            else
            {
                ViewBag.NowAttend = "无";
                ViewBag.NowQue = 0;
            }
            klass_seminar_enroll_state_model model = new klass_seminar_enroll_state_model(id);
            ViewBag.model = model;
            ViewBag.tid = tid;
            ViewBag.ksid = id;
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
                        enroll_start_time = Convert.ToDateTime(Request["start_date"].Replace("T", " ")),
                        enroll_end_time = Convert.ToDateTime(Request["end_date"].Replace("T", " ")),
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

                    //CreateKlassSeminar
                    foreach (var i in kidlist)
                    {
                        klass_seminar Newks = new klass_seminar
                        {
                            klass_id = i,
                            seminar_id = NewSeminar.id,
                            status = 0
                        };
                        db.klass_seminar.Add(Newks);
                    }
                    db.SaveChanges();
                        return Redirect("/TeacherMobile/ChsSpecSeminar/"+id.ToString());
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
                    db.SaveChanges();
                    //File
                    if (Request.Files.Count == 0) break;
                    klass c1 = db.klass.Find(NewKlass.id);

                    HttpPostedFileBase f = Request.Files[0];
                    if (f == null) break;
                    string str = f.FileName;
                    if (str.Length < 2) break;
                    str = str.Substring(str.Length - 3);
                    int type = 0;
                    if (str == "xls") type = 1;
                    if (str == "lsx") type = 2;
                    if (type > 0)
                    {

                        string fname = NewKlass.id.ToString() + ".xls";
                        if (type == 2) fname += 'x';

                        if (System.IO.File.Exists(Server.MapPath("~/Files/class/" + fname)))
                        {
                            System.IO.File.Delete(Server.MapPath("~/Files/class/" + fname));
                        }

                        f.SaveAs(Server.MapPath("~/Files/class/" + fname));
                        LoadList("~/Files/class/" + fname, NewKlass.id);
                        return Redirect("/TeacherMobile/Klassinfo/"+id.ToString());
                    }
                    else
                        return Content("Not a Excel.");
            }
            return RedirectToAction("/TeacherMobile/Klassinfo/" + id.ToString());
        }
        private void LoadList(string path, int class_id)   //"Data Source=" + Server.MapPath("~/Files/NET.xls") + ";"
        {
            string strConn = "";
            if (path.Last() == 'x')
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Server.MapPath(path) + ";" + "Extended Properties='Excel 12.0;HDR=NO;IMEX=1';";
            else
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Server.MapPath(path) + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            string strExcel = "select * from   [sheet1$]";
            DataSet ds = new DataSet();
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter(strExcel, strConn);
            adapter.Fill(ds, "sheet1");
            conn.Close();


            //学号    姓名  所属系 专业
            DataTable data = ds.Tables["sheet1"];

            int course_id = db.klass.Find(class_id).course_id;
            //以前的记录
            var sclist = from sc in db.klass_student where sc.klass_id == class_id select sc;
            List<decimal> scid = new List<decimal>();
            foreach (var sc in sclist) scid.Add(db.student.Find(sc.student_id).id);

            for (int i = 0; i < data.Rows.Count; i++)
            {
                string acad_id = data.Rows[i][0].ToString().Trim();
                if (acad_id == null || acad_id == "" || acad_id == "学号") continue;    //忽略首行与空行(合并行)

                //创建账户
                var uitest = from ui in db.student where ui.account.Contains(acad_id) select ui;
                if (uitest.Count() == 0)
                {
                    var addus = new student
                    {
                        account = acad_id,
                        password = "123456",
                        student_name = data.Rows[i][1].ToString().Trim(),
                        is_active = 0
                    };
                    db.student.Add(addus);
                    db.SaveChanges();
                }

                var uilist = from ui in db.student where ui.account.Contains(acad_id) select ui;
                int uid = 0;
                foreach (var ui in uilist) uid = ui.id;
                if (scid.Contains(uid)) scid.Remove(uid);   //仍在表内，踢出list
                else                                        //不在表内，加入选课表
                {
                    var addsc = new klass_student { student_id = uid, klass_id = class_id, course_id = course_id };
                    db.klass_student.Add(addsc);
                }
            }

            //删除不在新表内的
            var dellist = from sc in db.klass_student where (scid.Contains(sc.student_id) && sc.klass_id == class_id) select sc;
            foreach (var del in dellist) db.klass_student.Remove(del);

            //保存
            db.SaveChanges();
        }
        public ActionResult CreateCourse()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;


            int tid = Int32.Parse(Session["user_id"].ToString());

            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    List<string> name = new List<string>();
                    List<int> coid = new List<int>();
                    foreach (course c in db.course.ToList())
                    {
                        name.Add(c.course_name + '(' + db.teacher.Find(c.teacher_id).teacher_name + ')');
                        coid.Add(c.id);
                    }
                    ViewBag.name = name;
                    ViewBag.coid = coid;
                    return View();
                case "POST":        //create
                    course NewCourse = new course
                    {
                        teacher_id = tid,
                        course_name = Request["course_name"],
                        introduction = Request["introduction"],
                        presentation_percentage = byte.Parse(Request["presentation_percentage"]),
                        question_percentage = byte.Parse(Request["question_percentage"]),
                        report_percentage = byte.Parse(Request["report_percentage"]),
                        team_start_time = Convert.ToDateTime(Request["team_start_time"].Replace('T',' ')),
                        team_end_time = Convert.ToDateTime(Request["team_end_time"].Replace('T', ' '))
                    };
                    db.course.Add(NewCourse);
                    db.SaveChanges();
                    member_limit_strategy Newmlt = new member_limit_strategy
                    {
                        course_id = NewCourse.id,
                        max_member = byte.Parse(Request["max_member"]),
                        min_member = byte.Parse(Request["min_member"])
                    };
                    db.member_limit_strategy.Add(Newmlt);
                    db.SaveChanges();
                    int serial = 1;
                        serial++;
                    if (Request["TeamAndStrategy"]=="1")
                    {
                        team_strategy Newts = new team_strategy
                        {
                            course_id = NewCourse.id,
                            strategy_serial = 1,
                            strategy_name = "TeamAndStrategy",
                            strategy_id = 0
                        };
                        db.team_strategy.Add(Newts);
                    }
                    int num1 = Int32.Parse(Request["num1"]);//cmls
                    int num2 = Int32.Parse(Request["num2"]);//ccs
                    int[] numinner = new int[]{ Int32.Parse(Request["numinner1"]) + 2,
                    Int32.Parse(Request["numinner2"]) + 2,
                    Int32.Parse(Request["numinner3"]) + 2 };
                    //===================================================
                    for (int i = 1; i <= num1; i++) 
                    {
                        course_member_limit_strategy Newcmls = new course_member_limit_strategy
                        {
                            course_id = Int32.Parse(Request["cmls" + i.ToString()]),
                            min_member = byte.Parse(Request["minn" + i.ToString()]),
                            max_member = byte.Parse(Request["maxn" + i.ToString()]),
                        };
                        db.course_member_limit_strategy.Add(Newcmls);
                        db.SaveChanges();
                        serial++;
                        team_strategy Newts = new team_strategy
                        {
                            course_id = NewCourse.id,
                            strategy_serial = serial,
                            strategy_name = "CourseMemberLimitStrategy",
                            strategy_id = Newcmls.id
                        };
                        db.team_strategy.Add(Newts);
                        db.SaveChanges();
                    }
                    for (int i = 1; i <= num2; i++)//ccs
                    {
                        //calc ccsid
                        int ccsid = db.conflict_course_strategy.Select(ccs => ccs.id).Max() + 1;
                        serial++;
                        int cnt = numinner[i - 1];
                        for (int j = 1; j <= cnt; j++)
                        {
                            conflict_course_strategy Newccs = new conflict_course_strategy
                            {
                                id = ccsid,
                                course_id = Int32.Parse(Request["ccs" + i.ToString() + j.ToString()])
                            };
                            db.conflict_course_strategy.Add(Newccs);
                        }
                        team_strategy Newts = new team_strategy
                        {
                            course_id = NewCourse.id,
                            strategy_serial = serial,
                            strategy_name = "ConflictCourseStrategy",
                            strategy_id = ccsid
                        };
                        db.team_strategy.Add(Newts);
                        db.SaveChanges();
                    }
                    return RedirectToAction("TeacherMyCourse");
            }
            return View();
        }
        public ActionResult KlassSeminar(int id)//ksid
        {
            BEnrollSmn_model model = new BEnrollSmn_model(id, 0);
            ViewBag.model = model;
            return View();
        }
        public ActionResult TeacherIndividual()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;

            teacher s = db.teacher.Find(Int32.Parse(Session["user_id"].ToString()));
            ViewBag.name = s.teacher_name;
            ViewBag.account = s.account;
            return View();
        }
        public ActionResult AccountAndSet()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;

            ViewBag.t = db.teacher.Find(Int32.Parse(Session["user_id"].ToString()));
            return View();
        }
        public ActionResult ChangePassword()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;
            return View();
        }
        public ActionResult ChangeEmail()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;
            return View();
        }
        public ActionResult TeacherMyCourse()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;

            int tid = Int32.Parse(Session["user_id"].ToString());
            ViewBag.course = (from c in db.course where c.teacher_id==tid select c).ToList();
            return View();
        }
        List<List<round_team_score>> totalround = new List<List<round_team_score>>();
        public ActionResult CheckStuScore(int id)//course_id
        {
            var ridlist = (from r in db.round where r.course_id == id orderby r.round_serial ascending select r.id);
            foreach(var rid in ridlist)
            {
                //find teamid
                var teamidlist = (from rs in db.round_score where rs.round_id == rid select rs.team_id).Distinct().ToList();
                List<round_team_score> around = new List<round_team_score>();
                foreach(var tid in teamidlist)
                {
                    around.Add(new round_team_score(rid, tid));
                }
                totalround.Add(around);
            }
            ViewBag.totalround = totalround;
            ViewBag.course_id = id;
            ViewBag.TitleText = db.course.Find(id).course_name;
            return View();
        }
        public ActionResult CheckStuGroup(int id)//course_id
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;

            int tid = Int32.Parse(Session["user_id"].ToString());
            ViewBag.ct = new course_team(id).list;

            ViewBag.stulist = new StudentMobileController().studentlist(id);
            ViewBag.course_id = id;
            course c = db.course.Find(id);
            if (c.team_start_time <= DateTime.Now && DateTime.Now <= c.team_end_time) ViewBag.Time = true;
            else ViewBag.Time = false;
            return View();
        }
        public ActionResult CourseInfo(int id)
        {
            course c = db.course.Find(id);
            ViewBag.c = c;
            ViewBag.TitleText = c.course_name;
            //本门课人数上下限mls   返回一个member_limit_strategy或null 唯一 id 与course_id 相同(大概)
            ViewBag.mls = (from mls in db.member_limit_strategy where mls.course_id == id select mls).ToList()[0];

            /* 所有本门课的策略 ts (T/F)
             * 返回
             * strccs 代表冲突课程“A|B|C-D|E”或null 每 - 一个集合
             * 
             * ac true代表全部 false代表符合其一
             * cmlslist List<course_member_limit_strategy>模型 代表选修课人数上下限 包括一个AND或者OR（上文ac）
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
                List<string> strccs = new List<string>();
                List<course_member_limit_strategy> cmlslist = new List<course_member_limit_strategy>();
                List<string> cmlsname = new List<string>();
                foreach (var ts in tslist)
                {
                    int tid = ts.strategy_id;
                    switch (ts.strategy_name)
                    {
                        case "ConflictCourseStrategy":
                            var ccslist = (from ccs in db.conflict_course_strategy where ccs.id == tid select ccs.course_id).ToList();
                            string tmp = "";
                            foreach (var cid in ccslist)
                            {
                                if (cid != id) tmp += ' ' + db.course.Find(cid).course_name + '(' + db.teacher.Find(db.course.Find(cid).teacher_id).teacher_name + ')' + " |";
                            }
                            strccs.Add(tmp.Remove(tmp.Length - 1, 1));
                            break;
                        case "CourseMemberLimitStrategy":         //不定个数 针对选修xx课程的组队
                            course_member_limit_strategy cmls = db.course_member_limit_strategy.Find(tid);
                            cmlslist.Add(cmls);
                            cmlsname.Add(db.course.Find(cmls.course_id).course_name);
                            break;
                    }
                }
                ViewBag.cmlslist = cmlslist;
                ViewBag.cmlsname = cmlsname;
                ViewBag.strccs = strccs;
            }
            else
            {
                ViewBag.ts = false;
            }
            return View();
        }
        public ActionResult KlassInfo(int id)//course_id
        {
            ViewBag.TitleText = db.course.Find(id).course_name;
            ViewBag.klist = (from k in db.klass where k.course_id == id select k).ToList();
            ViewBag.course_id = id;
            return View();
        }
        public ActionResult ShareKlassSet(int id)//course_id
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;

            int tid = Int32.Parse(Session["user_id"].ToString());
            
            var ssalist1 = (from ssa in db.share_seminar_application where ssa.main_course_id==id && ssa.status == 1 select ssa).ToList();
            List<string> strssa1 = new List<string>();
            foreach (var ssa in ssalist1) strssa1.Add(db.course.Find(ssa.sub_course_id).course_name + '(' + db.teacher.Find(db.course.Find(ssa.sub_course_id).teacher_id).teacher_name + ')');
            var ssalist2 = (from ssa in db.share_seminar_application where ssa.sub_course_id==id && ssa.status == 1 select ssa).ToList();
            List<string> strssa2 = new List<string>();
            foreach (var ssa in ssalist2) strssa2.Add(db.course.Find(ssa.main_course_id).course_name + '(' + db.teacher.Find(db.course.Find(ssa.main_course_id).teacher_id).teacher_name + ')');

            var stalist1 = (from sta in db.share_team_application where sta.main_course_id==id && sta.status == 1 select sta).ToList();
            List<string> strsta1 = new List<string>();
            foreach (var sta in stalist1) strsta1.Add(db.course.Find(sta.sub_course_id).course_name + '(' + db.teacher.Find(db.course.Find(sta.sub_course_id).teacher_id).teacher_name + ')');
            var stalist2 = (from sta in db.share_team_application where sta.sub_course_id==id && sta.status == 1 select sta).ToList();
            List<string> strsta2 = new List<string>();
            foreach (var sta in stalist2) strsta2.Add(db.course.Find(sta.main_course_id).course_name + '(' + db.teacher.Find(db.course.Find(sta.main_course_id).teacher_id).teacher_name + ')');

            ViewBag.ssalist1 = ssalist1;
            ViewBag.strssa1 = strssa1;
            ViewBag.ssalist2 = ssalist2;
            ViewBag.strssa2 = strssa2;

            ViewBag.stalist1 = stalist1;
            ViewBag.strsta1 = strsta1;
            ViewBag.stalist2 = stalist2;
            ViewBag.strsta2 = strsta2;
            ViewBag.course_id = id;
            return View();
        }
        public ActionResult CreateShare(int id)//sa main_course_id
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;

            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    ViewBag.model = new shared_course(id);
                    ViewBag.course_id = id;
                    return View();
                case "POST":        //create
                    string type = Request["type"].ToString();
                    int subid = Int32.Parse(Request["sub_course_id"].ToString());
                    switch(type)
                    {
                        case "team":
                            share_team_application Newsta = new share_team_application
                            {
                                main_course_id = id,
                                sub_course_id = subid,
                                sub_course_teacher_id = db.course.Find(subid).teacher_id,
                                status = 0
                            };
                            db.share_team_application.Add(Newsta);
                            db.SaveChanges();
                            break;
                        case "seminar":
                            share_seminar_application Newssa = new share_seminar_application
                            {
                                main_course_id = id,
                                sub_course_id = subid,
                                sub_course_teacher_id = db.course.Find(subid).teacher_id,
                                status = 0
                            };
                            db.share_seminar_application.Add(Newssa);
                            db.SaveChanges();
                            break;
                    }
                    return RedirectToAction("ShareKlassSet/" + id.ToString());
            }
                    return View();
        }
        public ActionResult daiban()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return Redirect("/Home/MobileLogin");
            }
            else Session["user_id"] = test_id;

            teacher tea = db.teacher.Find(Int32.Parse(Session["user_id"].ToString()));
            int teacher_id = tea.id;

            //team_valid_application
            List<string> strtva = new List<string>();
            var tvalist = (from tva in db.team_valid_application where tva.teacher_id == teacher_id &&tva.status!=1 select tva).ToList();
            foreach (var tva in tvalist)
            {
                team t = db.team.Find(tva.team_id);
                strtva.Add("来自 " + db.course.Find(t.course_id).course_name + new qt().t2ts(t.id) + "小组的组队合法化申请");
            }

            //share_team_application
            List<string> strsta = new List<string>();
            var stalist = (from sta in db.share_team_application where sta.sub_course_teacher_id == teacher_id && sta.status != 1 select sta).ToList();
            foreach (var sta in stalist)
            {
                strsta.Add("来自 " + db.course.Find(sta.main_course_id).course_name + '(' + db.teacher.Find(db.course.Find(sta.main_course_id).teacher_id).teacher_name + ") 的共享组队申请");
            }

            //share_seminar_application
            List<string> strssa = new List<string>();
            var ssalist = (from ssa in db.share_seminar_application where ssa.sub_course_teacher_id == teacher_id && ssa.status != 1 select ssa).ToList();
            foreach (var ssa in ssalist)
            {
                strsta.Add("来自 " + db.course.Find(ssa.main_course_id).course_name + '(' + db.teacher.Find(db.course.Find(ssa.main_course_id).teacher_id).teacher_name + ") 的共享讨论课申请");
            }

            ViewBag.tvalist = tvalist;
            ViewBag.strtva = strtva;
            ViewBag.stalist = stalist;
            ViewBag.strsta = strsta;
            ViewBag.ssalist = ssalist;
            ViewBag.ssrsta = strssa;
            return View();
        }

        public bool start(int ksid)
        {
            klass_seminar ks = db.klass_seminar.Find(ksid);
            if (ks.status==0)
            {
                ks.status = 1;
                db.SaveChanges();
                return true;
            }
            return false;
        }
        //讨论课过程结束后调用
        public ActionResult SetReportDDL(int id)  //klass_seminar_id   klass_seminar->seminar_score
        {
            switch (Request.HttpMethod)
            {
                case "GET":         //load
                    return View();
                case "POST":        //create
                    klass_seminar ks = db.klass_seminar.Find(id);
                    round r = db.round.Find(db.seminar.Find(ks.seminar_id).round_id);

                    ks.report_ddl = Convert.ToDateTime(Request["ddl"].Replace("T", " "));
                    db.SaveChanges();

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
                    new UpdateScore().UpdateKlassSeminarScore(id);

                    //判断这个klass的round结束否

                    //本轮所有seminar_id
                    var seidlist = (from s in db.seminar where s.round_id == r.id select s.id).ToList();
                    int ksidcnt = (from aks in db.klass_seminar where seidlist.Contains(aks.seminar_id) && aks.klass_id == ks.klass_id && ks.status != 2 select aks).Count();
                    if (ksidcnt > 0) return RedirectToAction("KlassSeminar/" + id);

                    //本轮本班所有ksid
                    var oksidlist = (from aks in db.klass_seminar where seidlist.Contains(aks.seminar_id) && aks.klass_id == ks.klass_id select aks.id).ToList();
                    var tidlist = (from ss in db.seminar_score where oksidlist.Contains(ss.klass_seminar_id) select ss.team_id).Distinct().ToList();
                    foreach (var tid in tidlist)
                    {
                        round_score NewRS = new round_score
                        {
                            round_id = r.id,
                            team_id = tid
                        };
                        db.round_score.Add(NewRS);
                    }
                    db.SaveChanges();
                    new UpdateScore().UpdateRoundScore(r.id);
                    return RedirectToAction("KlassSeminar/" + id);
            }
            return RedirectToAction("KlassSeminar/" + id);
        }

        public ActionResult crtmarkxls(int id)  //course_id
        {
            List<List<round_team_score>> totalround = new List<List<round_team_score>>();
            var ridlist = (from r in db.round where r.course_id == id orderby r.round_serial ascending select r.id);

            foreach (var rid in ridlist)
            {
                //find teamid
                var teamidlist = (from rs in db.round_score where rs.round_id == rid select rs.team_id).Distinct().ToList();
                List<round_team_score> around = new List<round_team_score>();
                foreach (var tid in teamidlist)
                {
                    around.Add(new round_team_score(rid, tid));
                }
                totalround.Add(around);
            }

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("小组编号", typeof(string)));
            dt.Columns.Add(new DataColumn("参与讨论课", typeof(string)));
            dt.Columns.Add(new DataColumn("Presentation", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Report", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Question", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Total", typeof(decimal)));

            for (int i = 0; i < totalround.Count(); i++)
                for(int j=0;j<totalround[i].Count();j++)
            {
                    if (totalround[i][j].rs != null)
                    {
                        DataRow dr = dt.NewRow();
                        dr = dt.NewRow();
                        dr[0] = totalround[i][j].team_serial;
                        dr[1] = "第 " + (i + 1).ToString() + " 轮总分";
                        dr[2] = (totalround[i][j].rs.presentation_score == null ? 0 : totalround[i][j].rs.presentation_score);
                        dr[3] = (totalround[i][j].rs.report_score == null ? 0 : totalround[i][j].rs.report_score);
                        dr[4] = (totalround[i][j].rs.question_score == null ? 0 : totalround[i][j].rs.question_score);
                        dr[5] = (totalround[i][j].rs.total_score == null ? 0 : totalround[i][j].rs.total_score);
                        dt.Rows.Add(dr);
                    }
                    for(int k=0;k<totalround[i][j].list.Count();k++)
                    {
                        DataRow dr = dt.NewRow();
                        dr = dt.NewRow();
                        dr[0] = totalround[i][j].team_serial;
                        dr[1] = totalround[i][j].list[k].seminar_name;
                        dr[2] = (totalround[i][j].list[k].presentation_score == null ? 0 : totalround[i][j].list[k].presentation_score);
                        dr[3] = (totalround[i][j].list[k].report_score == null ? 0 : totalround[i][j].list[k].report_score);
                        dr[4] = (totalround[i][j].list[k].question_score == null ? 0 : totalround[i][j].list[k].question_score);
                        dr[5] = (totalround[i][j].list[k].total_score == null ? 0 : totalround[i][j].list[k].total_score);
                        dt.Rows.Add(dr);
                    }
            }
            return Redirect("/File/Download?path=" + new FileController().DataToExcel(dt));
        }

        public decimal? getscore_question(int id)
        {
            return db.question.Find(id).score;
        }
        public decimal? getscore_presentation(int id,int ksid)
        {
            var sslist = (from ss in db.seminar_score where ss.klass_seminar_id == ksid && ss.team_id == id select ss.presentation_score).ToList();
            return (sslist.Count() > 0 ? sslist[0] : null);
        }
        public bool score_presentation(int id, decimal score, int ksid)
        {
            var sslist = (from ss in db.seminar_score where ss.klass_seminar_id == ksid && ss.team_id == id select ss).ToList();
            if (sslist.Count() > 0) sslist[0].presentation_score = score;
            else
            {
                seminar_score Newss = new seminar_score
                {
                    klass_seminar_id = ksid,
                    presentation_score = score,
                    team_id = id
                };
                db.seminar_score.Add(Newss);
            }
            db.SaveChanges();
            return true;
        }
        public bool score_question(int id, decimal score)
        {
            db.question.Find(id).score = score;
            db.SaveChanges();
            return true;
        }


        public string chgscore(decimal? pre, decimal? rep, decimal? que, int tid, int ksid)
        {
            var ssl = (from ss in db.seminar_score where ss.klass_seminar_id == ksid && ss.team_id == tid select ss).ToList()[0];
            ssl.presentation_score = pre;
            ssl.report_score = rep;
            ssl.question_score = que;
            db.SaveChanges();
            return new UpdateScore().UpdataASeminarScore(ksid,tid).ToString();
        }
        public bool Dealtva(int mes,int id)
        {
            var tva = db.team_valid_application.Find(id);
            if (tva == null) return false;
            if (mes == 1) 
            {
                db.team.Find(tva.team_id).status = 1;
                db.team_valid_application.Remove(tva);
                db.SaveChanges();
            }
            else
            {
                db.team.Find(tva.team_id).status = 0;
                db.team_valid_application.Remove(tva);
                db.SaveChanges();
            }
            return true;
        }
        public bool Dealsta(int mes,int id)
        {
            var sta = db.share_team_application.Find(id);
            if (sta == null) return false;
            new team_share(id, mes);
            return true;
        }
        public bool Dealssa(int mes, int id)
        {
            var ssa = db.share_seminar_application.Find(id);
            if (ssa == null) return false;
            new seminar_share(id, mes);
            return true;
        }
        public bool DelSeminar(int id)
        {
            return new Del().DelSeminar(id);
        }
        public ActionResult DelKlass(int id)
        {
            int course_id = db.klass.Find(id).course_id;
            new Del().DelKlass(id);
            return Redirect("/TeacherMobile/Klassinfo/" + course_id.ToString());
        }
        public bool DelCourse(int id)
        {
            return new Del().DelCourse(id);
        }
        public string cancelSharesta(int id)
        {
            new team_share("cancel", id);
            return "success";
        }
        public string cancelSharessa(int id)
        {
            new seminar_share("cancel", id);
            return "success";
        }
        public string chgpwd(string oldpw, string newpw)
        {
            if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false) return "1";
            student s = db.student.Find(Int32.Parse(Session["user_id"].ToString()));
            if (s.password == oldpw)
            {
                s.password = newpw;
                db.SaveChanges();
                return "success";
            }
            else return "1";
        }
        public ActionResult chgemail()
        {
            if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false) return Content("Identity verification error.");
            student s = db.student.Find(Int32.Parse(Session["user_id"].ToString()));
            s.email = Request["email"];
            db.SaveChanges();
            return RedirectToAction("AccountAndSet");
        }

        bool is_judge = true;
        int test_id = 3;//qm
        MSSQLContext db = new MSSQLContext();
    }
}