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
        public List<seminar_score> ss = new List<seminar_score>();
        public List<string> name = new List<string>();
    }

    //返回一个klass_seminar下已报名的队伍名
    public class klass_seminar_enroll_state_model
    {
        public int ksid;
        public string[] team_order;
        public string seminar_name;
        public IndexOutOfRangeException status;//================================
        public klass_seminar_enroll_state_model(int klass_seminar_id)
        {
            ksid = klass_seminar_id;
            klass_seminar ks = db.klass_seminar.Find(ksid);
            seminar se = db.seminar.Find(ks.seminar_id);
            seminar_name = se.seminar_name;
            var alist = from a in db.attendance where a.klass_seminar_id == ks.id select a;

            team_order = new string[se.max_team];
            for (int i = 0; i < team_order.Length; i++) team_order[i] = "";
            foreach (var a in alist)
            {
                team_order[a.team_order - 1] = db.team.Find(a.team_id).team_name;
            }
        }
        
        MSSQLContext db = new MSSQLContext();
    }

    public class BEnrollSmn_model
    {
        //base
        public int sid, ksid;

        //round
        public int round;

        //seminar
        public string seminar_name;
        public string introduction;
        public string course_name;

        //klass_seminar
        public string status;

        //attendance
        public string enroll;
        public string ppt="";

        public BEnrollSmn_model(int klass_seminar_id, int student_id)
        {
            sid = student_id;
            ksid = klass_seminar_id;
            klass_seminar ks = db.klass_seminar.Find(ksid);
            seminar se = db.seminar.Find(ks.seminar_id);

            round = db.round.Find(se.round_id).round_serial;
            seminar_name = se.seminar_name;
            introduction = se.introduction;
            course_name = db.course.Find(se.course_id).course_name;
            switch(ks.status)
            {
                case 0:
                    status = "未开始";
                    break;
                case 1:
                    status = "正在进行";
                    break;
                case 2:
                    status = "已结束";
                    break;
                case 3:
                    status = "暂停";
                    break;
                default:
                    status = "";
                    break;
            };

            int team_id = new qt().c2t(se.course_id, student_id);
            if (team_id==0)
            {
                enroll = "未组队";
                return;
            }
            var alist = from a in db.attendance where a.team_id == team_id && a.klass_seminar_id == klass_seminar_id select a;
            if (alist.Count() == 0)
            {
                enroll = "未报名";
                return;
            }
            enroll = "第" + alist.ToList()[0].team_order.ToString() + "组";
            ppt = alist.ToList()[0].ppt_name;
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
            public string team_name;
            public string leader_name;
            public bool status;
            public string url;
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
                    status = a.report_url != null,
                    url = a.report_url
                };
                list.Add(tmp);
            }
            listLength = list.Count;
        }

        MSSQLContext db = new MSSQLContext();
    }
}