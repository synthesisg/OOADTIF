using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using final.Models;
namespace final.Models
{
    public class SeminarInfo
    {
        public int round;
        public int count;
        public string title;
        public string msg;
        public string signUpTime;
        public string reportTime;
    }
    public class qt
    {
        public qt() { }

        public int ks2t(int klass_seminar_id,int student_id)
        {
            return k2t(db.klass_seminar.Find(klass_seminar_id).klass_id, student_id);
        }
        public int k2t(int klass_id,int student_id)
        {
            return c2t(db.klass.Find(klass_id).course_id, student_id);
        }
        public int c2t(int course_id,int student_id)
        {
            var teamlist = (from ts in db.team_student where ts.student_id == student_id select ts.team_id).ToList();           //该学生的所有队伍
            var klist = (from k in db.klass where k.course_id == course_id select k.id).ToList();                                  //课程的所有班
            var tl = from kt in db.klass_team where teamlist.Contains(kt.team_id) && klist.Contains(kt.klass_id) select kt.team_id;  //该生队伍
            if (tl.Count() == 0) return 0;
            return tl.ToList()[0];                                                                                     //本班队伍id
        }


        //klass_seminar_id to seminar_serial
        public byte ks2se(int klass_seminar_id)  
        {
            return db.seminar.Find(db.klass_seminar.Find(klass_seminar_id).seminar_id).seminar_serial;
        }

        //team_id to klass_serial+team_serial
        public string t2ts(int team_id)
        {
            team t = db.team.Find(team_id);
            return t.klass_serial.ToString() + '-' + t.team_serial.ToString();
        }
        MSSQLContext db = new MSSQLContext();
    }
    public class personclass        //For Seminar
    {
        public int klass_id;
        public string name;
    }
    public class round_seminar      //For ChsSpecSeminar 返回某班一个round下的seminar
    {
        public round r;
        public List<seminar> s = new List<seminar>();
        public List<int> klass_seminar_id = new List<int>();

        public round_seminar(int klass_id, int round_id)
        {
            r = db.round.Find(round_id);
            var slist = from s in db.seminar where s.round_id == r.id select s;
            foreach (var item in slist)
            {
                s.Add(item);
                var sr = from ksr in db.klass_seminar where ksr.klass_id == klass_id && ksr.seminar_id == item.id select ksr;
                klass_seminar_id.Add(sr.ToList()[0].id);
            }
        }

        MSSQLContext db = new MSSQLContext();

    }
    public class scoreboard             //For Round Score
    {
        public round_score rs;
        public round r;
        public List<string> name = new List<string>();


        public List<round_score> list = new List<round_score>();
        public List<seminar_score> ss = new List<seminar_score>();
        public List<string> team_serial = new List<string>();
        public List<byte> seminar_serial = new List<byte>();

        public scoreboard() { }//远古版本

        public scoreboard(int round_id) //Teacher 得到一轮score
        {
            r = db.round.Find(round_id);
            list = (from rs in db.round_score where r.id == rs.round_id select rs).ToList();                        //这轮的总成绩
            var slist = (from s in db.seminar where s.round_id == r.id select s.id).ToList();                       //这轮的seminar_id
            var kslist = (from ks in db.klass_seminar where slist.Contains(ks.seminar_id) select ks.id).ToList();   //这轮的ksid
            ss = (from ss in db.seminar_score where kslist.Contains(ss.klass_seminar_id) select ss).ToList();       //这轮的seminar_score
            foreach(var item in ss)
            {
                seminar_serial.Add(new qt().ks2se(item.klass_seminar_id));
                team_serial.Add(new qt().t2ts(item.team_id));
            }
        }

        MSSQLContext db = new MSSQLContext();
    }

    //某course下所有seminar成绩
    public class scoreboard_course            
    {
        public List<seminar_score> ss = new List<seminar_score>();
        public List<int> serial=new List<int>();
        public bool is_valid=true;
        public scoreboard_course(int course_id, int student_id)//For Student
        {
            int team_id = new qt().c2t(course_id, student_id);
            if(team_id==0)
            {
                is_valid = false;
                return;
            }
            int klass_id = db.team.Find(team_id).klass_id;

            var kslist = (from ks in db.klass_seminar where ks.klass_id == klass_id select ks.id).ToList();
            ss = (from ss in db.seminar_score where kslist.Contains(ss.klass_seminar_id) && ss.team_id == team_id select ss).ToList();
            for (int i = 0; i < ss.Count(); i++) serial.Add(db.seminar.Find(db.klass_seminar.Find(ss[i].klass_seminar_id).seminar_id).seminar_serial);
        }

        MSSQLContext db = new MSSQLContext();
    }
    //返回一个klass_seminar下已报名的队伍名
    public class klass_seminar_enroll_state_model
    {
        public int ksid;
        public int status;
        public string[] team_order; //team_name
        public string[] leader_name;
        public string[] team_serial;//klass+serial
        public string[] ppt_name;
        public string[] ppt_url;
        public string seminar_name;
        public string my_team_name="";
        public klass_seminar_enroll_state_model(int klass_seminar_id, int student_id = 0)
        {
            ksid = klass_seminar_id;
            klass_seminar ks = db.klass_seminar.Find(ksid);
            seminar se = db.seminar.Find(ks.seminar_id);
            seminar_name = se.seminar_name;
            var alist = from a in db.attendance where a.klass_seminar_id == ks.id select a;
            status = ks.status;
            team_order = new string[se.max_team];
            leader_name = new string[se.max_team];
            team_serial = new string[se.max_team];
            ppt_name = new string[se.max_team];
            ppt_url = new string[se.max_team];

            for (int i = 0; i < team_order.Length; i++) team_order[i] = "";
            foreach (var a in alist)
            {
                team t = db.team.Find(a.team_id);
                team_order[a.team_order - 1] = t.team_name;
                leader_name[a.team_order - 1] = db.student.Find(t.leader_id).student_name;
                team_serial[a.team_order - 1] = t.klass_serial.ToString() + '-' + t.team_serial;
                ppt_name[a.team_order - 1] = a.ppt_name;
                ppt_url[a.team_order - 1] = a.ppt_url;
            }

            if (student_id > 0) 
            {
                int kid = ks.klass_id;
                int team_id = new qt().k2t(kid, student_id);
                if (team_id > 0) my_team_name = db.team.Find(team_id).team_name;
                else my_team_name = "No Team.";
            }
        }
        
        MSSQLContext db = new MSSQLContext();
    }

    public class BEnrollSmn_model
    {
        //base
        public int sid, ksid;
        public seminar seminar;
        public klass_seminar klass_seminar;
        public attendance attendance;
        //round
        public int round;

        //seminar
        public string seminar_name;
        public string introduction;
        public string course_name;
        public byte serial;
        public bool could_enroll = true;
        public bool could_report = false;

        //klass_seminar
        public string status;
        public short seminar_status;

        //attendance
        public string enroll;
        public short enroll_status;

        public BEnrollSmn_model(int klass_seminar_id, int student_id)
        {
            sid = student_id;
            ksid = klass_seminar_id;
            klass_seminar  = db.klass_seminar.Find(ksid);
            seminar = db.seminar.Find(klass_seminar.seminar_id);

            DateTime now = DateTime.Now;
            if ((seminar.enroll_start_time <= now && now <= seminar.enroll_end_time) == false) 
                could_enroll = false;

            round = db.round.Find(seminar.round_id).round_serial;
            seminar_name = seminar.seminar_name;
            introduction = seminar.introduction;
            serial = seminar.seminar_serial;
            course_name = db.course.Find(seminar.course_id).course_name;
            seminar_status = klass_seminar.status;
            switch (klass_seminar.status)
            {
                case 0:
                    status = "未开始";
                    break;
                case 1:
                    status = "正在进行";
                    could_enroll = false;
                    break;
                case 2:
                    status = "已结束";
                    could_enroll = false;
                    break;
                case 3:
                    status = "暂停";
                    could_enroll = false;
                    break;
                default:
                    status = "";
                    break;
            };

            int team_id = new qt().c2t(seminar.course_id, student_id);
            if (team_id==0)
            {
                enroll = "未组队";
                enroll_status = 0;
                could_enroll = false;
                return;
            }
            var alist = (from a in db.attendance where a.team_id == team_id && a.klass_seminar_id == klass_seminar_id select a).ToList();
            if (alist.Count() == 0)
            {
                enroll = "未报名";
                enroll_status = 0;

                //验证本轮报名次数上限
                int kid = klass_seminar.klass_id;
                int rid = seminar.round_id;
                int maxi = 0;
                var krlist = (from kr in db.klass_round where kr.klass_id == kid && kr.round_id == rid select kr).ToList();
                if (krlist.Count() > 0) maxi = (int)krlist[0].enroll_number;
                    else maxi = 1;
                var slist = (from s in db.seminar where s.round_id == rid select s.id).ToList();
                int klass_id = db.team.Find(team_id).klass_id;
                var kslist = (from ks in db.klass_seminar where slist.Contains(ks.seminar_id) && ks.klass_id==klass_id select ks.id).ToList();
                int acount = (from a in db.attendance where kslist.Contains(a.klass_seminar_id) select a).Count();
                if (acount >= maxi) could_enroll = false;
                return;
            }
            enroll_status = 1;
            attendance = alist[0];
            enroll = "第" + alist[0].team_order.ToString() + "组";
            if (now < klass_seminar.report_ddl) could_report = true;
        }

        MSSQLContext db = new MSSQLContext();
    }
    public class teamlist
    {
        public int course_id;
        public team team;
        public List<string> name = new List<string>();
        public List<string> account = new List<string>();
    }
    public class seminar_klass
    {
        public seminar seminar;
        public List<klass_seminar> sk = new List<klass_seminar>();
    }
    public class round_seminar_klass
    {
        public round r;
        public List<seminar_klass> sklist = new List<seminar_klass>();
    }

    //一个course_id下的所有队伍
    public class course_team
    {
        public course course;
        public List<teamlist> list = new List<teamlist>();

        public course_team(int course_id) { create(course_id); }
        public void create(int course_id)
        {
            course = db.course.Find(course_id);

            var klist = (from k in db.klass where k.course_id == course_id select k.id).ToList();
            var teamlist = from kt in db.klass_team where klist.Contains(kt.klass_id) select kt.team_id;
            foreach (var teamid in teamlist)
            {
                teamlist tmp = new teamlist();
                tmp.team = db.team.Find(teamid);
                var stlist = from ts in db.team_student where ts.team_id == teamid select ts.student_id;
                foreach (var st in stlist)
                {
                    tmp.name.Add(db.student.Find(st).student_name);
                    tmp.account.Add(db.student.Find(st).account);
                }
                list.Add(tmp);
            }
        }

        MSSQLContext db = new MSSQLContext();
    }

    public class course_seminar
    {
        //课程course ->  轮round -> 某节课seminar -> 某个班的某节klass_seminar -> attendance
        //              klass_round 某轮报名次数限制
        public course course;
        List<round_seminar_klass> rs = new List<round_seminar_klass>();

        public course_seminar(int course_id)
        {
            course = db.course.Find(course_id);
            var rlist = from r in db.round where r.course_id == course_id select r;
            foreach(var r in rlist)                                                         //round
            {
                round_seminar_klass tmprsk = new round_seminar_klass();
                tmprsk.r = r;
                var slist = from s in db.seminar where s.round_id == r.id select s;
                foreach(var s in slist)                                                     //seminar
                {
                    var tmpsk = new seminar_klass();
                    tmpsk.seminar = s;
                    var kslist = from ks in db.klass_seminar where ks.seminar_id == s.id select ks;
                    foreach (var ks in kslist) tmpsk.sk.Add(ks);                            //klass_seminar
                    tmprsk.sklist.Add(tmpsk);
                }
                rs.Add(tmprsk);
            }
        }
        
        MSSQLContext db = new MSSQLContext();
    }

    public class seminar_report
    {
        public seminar seminar;
        public List<line> list = new List<line>();
        public int listLength;
        public class line
        {
            public int id;

            public string team_name;
            public string leader_name;
            public bool report_status;
            public string report_url;
            public string report_name="";
            public bool ppt_status;
            public string ppt_url;
            public string ppt_name="";
        }
        public seminar_report(int seminar_id)
        {
            seminar = db.seminar.Find(seminar_id);

            var ksidlist = from ks in db.klass_seminar where ks.seminar_id == seminar_id select ks.id;
            var alist = from a in db.attendance where ksidlist.Contains(a.klass_seminar_id) select a;
            foreach(var a in alist)
            {
                team t = db.team.Find(a.team_id);
                line tmp = new line
                {
                    team_name = t.team_name,
                    leader_name = db.student.Find(t.leader_id).student_name,
                    report_status = (a.report_url != null),
                    report_url = a.report_url,
                    report_name = a.report_name,
                    ppt_status = (a.ppt_url != null),
                    ppt_url = a.ppt_url,
                    ppt_name = a.ppt_name,
                    id = a.id
                };
                list.Add(tmp);
            }
            listLength = list.Count;
        }

        MSSQLContext db = new MSSQLContext();
    }
}