using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ooad.Models;
namespace ooad.Models
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

    public class personclass        //For Seminar
    {
        public int klass_id;
        public string name;
    }
    public class round_seminar      //For ChsSpecSeminar
    {
        public round r;
        public List<seminar> s = new List<seminar>();
    }
    public class scoreboard             //For score
    {
        public round_score rs;
        public List<seminar_score> ss = new List<seminar_score>();
        public List<string> name = new List<string>();
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

    //返回一个course_id下的所有队伍
    public class course_team
    {
        public course course;
        public List<teamlist> list = new List<teamlist>();

        public course_team(int course_id) { create(course_id); }
        public void create(int course_id)
        {
            course = db.course.Find(course_id);
            var teamlist = from ks in db.klass_student where ks.course_id == course_id select ks.team_id;
            var teamlistdis = teamlist.ToList().Distinct();

            foreach (var teamid in teamlistdis)
            {
                teamlist tmp = new teamlist();
                tmp.team = db.team.Find(teamid);
                var stlist = from ks in db.klass_student where ks.course_id == course_id && ks.team_id == teamid select ks.student_id;
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
        List<line> list = new List<line>();
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
        }

        MSSQLContext db = new MSSQLContext();
    }
}