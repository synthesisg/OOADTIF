using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using final.Models;
namespace final.Models
{
    //Query
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
        public string k2ks(int klass_id)
        {
            klass k = db.klass.Find(klass_id);
            return k.grade.ToString() + '-' + k.klass_serial.ToString();
        }
        MSSQLContext db = new MSSQLContext();
    }

    //Delete
    public class Del
    {
        public bool DelKlassSeminar(int id)
        {
            klass_seminar ks = db.klass_seminar.Find(id);
            if (ks == null) return false;

            var alist = from a in db.attendance where a.klass_seminar_id == id select a;
            foreach (var a in alist) db.attendance.Remove(a);
            var qlist = from q in db.question where q.klass_seminar_id == id select q;
            foreach (var q in qlist) db.question.Remove(q);
            var sclist = from sc in db.seminar_score where sc.klass_seminar_id == id select sc;
            foreach (var sc in sclist) db.seminar_score.Remove(sc);

            db.klass_seminar.Remove(ks);
            db.SaveChanges();

            return true;
        }

        public bool DelSeminar(int id)
        {
            seminar s = db.seminar.Find(id);
            if (s == null) return false;

            var kslist = from ks in db.klass_seminar where ks.seminar_id == id select ks;
            foreach (var ks in kslist)
                DelKlassSeminar(ks.id);

            db.seminar.Remove(s);
            db.SaveChanges();

            return true;
        }

        public bool DelKlass(int id)
        {
            klass k = db.klass.Find(id);
            if (k == null) return false;

            var kslist = from ks in db.klass_seminar where ks.klass_id == id select ks;
            foreach (var ks in kslist)
                DelKlassSeminar(ks.id);

            var tlist = (from t in db.team where t.klass_id == id select t).ToList();
            List<int> tid = new List<int>();
            foreach (var t in tlist)
            {
                tid.Add(t.id);
                db.team.Remove(t);
            }
            var kstlist = from ks in db.klass_student where ks.klass_id == id select ks;
            foreach (var ks in kstlist) db.klass_student.Remove(ks);
            var krlist = from kr in db.klass_round where kr.klass_id == id select kr;
            foreach (var kr in krlist) db.klass_round.Remove(kr);
            var ktlist = from kt in db.klass_team where kt.klass_id == id select kt;
            foreach (var kt in ktlist) db.klass_team.Remove(kt);

            db.klass.Remove(k);
            db.SaveChanges();

            return true;
        }

        public bool DelCourse(int id)
        {
            course c = db.course.Find(id);
            if (c == null) return false;

            var kidlist = (from k in db.klass where k.course_id == id select k.id).ToList();
            foreach (var kid in kidlist) DelKlass(kid);
            var rlist = (from r in db.round where r.course_id == id select r).ToList();
            foreach (var r in rlist) db.round.Remove(r);
            var slist = (from s in db.seminar where s.course_id == id select s.id).ToList();
            foreach (var sid in slist) DelSeminar(sid);

            db.course.Remove(c);
            db.SaveChanges();

            return true;
        }

        MSSQLContext db = new MSSQLContext();
    }
    public class UpdateScore
    {
        public UpdateScore() { }

        //生成ksid的总分，与round无关
        public void UpdateKlassSeminarScore(int ksid)
        {
            course c = db.course.Find(db.seminar.Find(db.klass_seminar.Find(ksid).seminar_id).course_id);
            byte p = c.presentation_percentage, q = c.question_percentage, r = c.report_percentage;

            var ksslist = from ss in db.seminar_score where ss.klass_seminar_id == ksid select ss;
            foreach (var kss in ksslist)
            {
                kss.total_score = ((kss.presentation_score == null ? 0 : kss.presentation_score) * p + (kss.question_score == null ? 0 : kss.question_score) * q + (kss.report_score == null ? 0 : kss.report_score) * r) / 100;
            }
            db.SaveChanges();
        }

        //生成round_id的所有分数
        public void UpdateRoundScore(int round_id)
        {
            round R = db.round.Find(round_id);
            byte p = R.presentation_score_method, q = R.question_score_method, r = R.report_score_method;
            course c = db.course.Find(db.round.Find(round_id).course_id);
            byte _p = c.presentation_percentage, _q = c.question_percentage, _r = c.report_percentage;

            var rslist = from rs in db.round_score where rs.round_id==round_id select rs;
            foreach(var rs in rslist)
            {
                int team_id = rs.team_id;
                var sslist = (from ss in db.seminar_score where ss.team_id == team_id select ss).ToList();

                int cnt = 0;
                decimal PS = 0, QS = 0, RS = 0;
                foreach(var ss in sslist)
                {
                    cnt++;
                    decimal _ps = (ss.presentation_score == null ? 0 : (decimal)ss.presentation_score);
                    decimal _qs = (ss.question_score == null ? 0 : (decimal)ss.question_score);
                    decimal _rs = (ss.report_score == null ? 0 : (decimal)ss.report_score);
                    if (p == 1) PS = Math.Max(PS, _ps);
                    else PS += _ps;
                    if (q == 1) QS = Math.Max(QS, _qs);
                    else QS += _qs;
                    if (r == 1) RS = Math.Max(RS, _rs);
                    else RS += _rs;
                }

                if (p == 0) PS /= cnt;
                if (q == 0) QS /= cnt;
                if (r == 0) RS /= cnt;

                rs.presentation_score = PS;
                rs.question_score = QS;
                rs.report_score = RS;
                rs.total_score = (PS * _p + QS * _q + RS * _r) / 100;
            }
            db.SaveChanges();
        }
        MSSQLContext db = new MSSQLContext();
    };
    public class personclass        //For Seminar
    {
        public int klass_id;
        public string name;
    }
    public class shared_course
    {
        List<int> id=new List<int>();
        List<string> str=new List<string>();
        public shared_course(int course_id)
        {
            var clist = (from c in db.course where c.id != course_id select c).ToList();
            foreach(var c in clist)
            {
                id.Add(c.id);
                str.Add(c.course_name + '(' + db.teacher.Find(c.teacher_id).teacher_name + ')');
            }
        }
        MSSQLContext db = new MSSQLContext();
    }
    public class astulist
    {
        public List<int> student_id=new List<int>();
        public List<string> account=new List<string>();
        public List<string> name=new List<string>();
        public int cnt;
        public astulist(List<int> sidlist,string str)
        {
            foreach (int sid in sidlist)
            {
                student s = db.student.Find(sid);
                if (str == "" || str==null)
                {
                    student_id.Add(s.id);
                    account.Add(s.account);
                    name.Add(s.student_name);
                }
                else
                {
                    if(s.account.Contains(str)||s.student_name.Contains(str))
                    {
                        student_id.Add(s.id);
                        account.Add(s.account);
                        name.Add(s.student_name);
                    }
                }
            }
            cnt = student_id.Count();
        }
        MSSQLContext db = new MSSQLContext();
    }
    //[student]For ChsSpecSeminar 返回某班一个round下的seminar
    public class round_seminar      
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
        public List<string> rteam_serial = new List<string>();
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
            foreach (var item in list)
                rteam_serial.Add(new qt().t2ts(item.team_id));
        }

        MSSQLContext db = new MSSQLContext();
    }

    //[student]某course下所有seminar成绩
    public class scoreboard_course            
    {
        public List<seminar_score> ss = new List<seminar_score>();
        public List<round_score> rs = new List<round_score>();
        public List<int> serial = new List<int>();
        public List<string> seminar_name = new List<string>();
        public bool is_valid = true;
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
            for (int i = 0; i < ss.Count(); i++)
            {
                serial.Add(db.seminar.Find(db.klass_seminar.Find(ss[i].klass_seminar_id).seminar_id).seminar_serial);
                seminar_name.Add(db.seminar.Find(db.klass_seminar.Find(ss[i].klass_seminar_id).seminar_id).seminar_name);
            }

            var rlist = (from r in db.round where r.course_id == course_id select r.id).ToList();
            rs= (from rs in db.round_score where rlist.Contains(rs.round_id) && rs.team_id == team_id select rs).ToList();
        }

        MSSQLContext db = new MSSQLContext();
    }

    public class round_team_score
    {
        public decimal total_score;
        public string team_serial;
        public round_score rs;
        public List<line> list = new List<line>();

        public class line
        {
            public string seminar_name;
            public decimal? presentation_score;
            public decimal? question_score;
            public decimal? report_score;
            public decimal? total_score;
        }
        public round_team_score(int round_id,int team_id)
        {
            team_serial = new qt().t2ts(team_id);
            int klass_id = db.team.Find(team_id).klass_id;
            var sidlist = (from s in db.seminar where s.round_id == round_id select s.id);
            var ksidlist = (from ks in db.klass_seminar where sidlist.Contains(ks.seminar_id) && ks.klass_id == klass_id select ks.id).ToList();
            foreach(var ksid in ksidlist)
            {
                var sslist = (from ss in db.seminar_score where ss.klass_seminar_id == ksid && ss.team_id == team_id select ss).ToList();
                if(sslist.Count()>0)
                {
                    line newline = new line();
                    newline.seminar_name = db.seminar.Find(db.klass_seminar.Find(ksid).seminar_id).seminar_name;
                    newline.presentation_score = sslist[0].presentation_score;
                    newline.question_score = sslist[0].question_score;
                    newline.report_score = sslist[0].report_score;
                    newline.total_score = sslist[0].total_score;
                    list.Add(newline);
                }
            }

            var rslist=(from es in db.round_score where es.round_id == round_id && es.team_id == team_id select es).ToList();
            if (rslist.Count() > 0) rs = rslist[0];
        }

        MSSQLContext db = new MSSQLContext();
    }

    //[all]返回一个klass_seminar下已报名的队伍名
    public class klass_seminar_enroll_state_model
    {
        public int ksid;
        public int status;
        public int[] team_id;
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
            team_id = new int[se.max_team];
            for (int i = 0; i < team_order.Length; i++) team_order[i] = "";
            foreach (var a in alist)
            {
                team t = db.team.Find(a.team_id);
                team_order[a.team_order - 1] = t.team_name;
                leader_name[a.team_order - 1] = db.student.Find(t.leader_id).student_name;
                team_serial[a.team_order - 1] = t.klass_serial.ToString() + '-' + t.team_serial;
                ppt_name[a.team_order - 1] = a.ppt_name;
                ppt_url[a.team_order - 1] = a.ppt_url;
                team_id[a.team_order - 1] = a.team_id;
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

        public BEnrollSmn_model(int klass_seminar_id, int student_id=0)
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
        public string team_serial;
        public List<string> name = new List<string>();
        public List<string> account = new List<string>();

        public teamlist(int team_id)
        {
            team = db.team.Find(team_id);
            team_serial = new qt().t2ts(team_id);
            //Leader
            name.Add(db.student.Find(team.leader_id).student_name);
            account.Add(db.student.Find(team.leader_id).account);
            //Member
            var stlist = from ts in db.team_student where ts.team_id == team_id select ts.student_id;
            foreach (var st in stlist)
            if(st!=team.leader_id && db.student.Find(st)!=null)
            {
                name.Add(db.student.Find(st).student_name);
                account.Add(db.student.Find(st).account);
            }
        }

        MSSQLContext db = new MSSQLContext();
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

    //[all]一个course_id下的所有队伍
    public class course_team
    {
        public course course;
        public List<teamlist> list = new List<teamlist>();

        public course_team(int course_id)
        {
            course = db.course.Find(course_id);

            var klist = (from k in db.klass where k.course_id == course_id select k.id).ToList();
            var teamlist = from kt in db.klass_team where klist.Contains(kt.klass_id) select kt.team_id;
            foreach (var teamid in teamlist)
                list.Add(new teamlist(teamid));
        }
        public course_team(int course_id,int student_id)
        {
            int ateam_id = new qt().c2t(course_id, student_id);
            if (ateam_id > 0)
            {
                list.Add(new teamlist(ateam_id));

                var teamlist = (from t in db.team where t.course_id == course_id select t.id).ToList();
                foreach (var teamid in teamlist)
                    if (teamid != ateam_id)
                        list.Add(new teamlist(teamid));
            }
        }
        MSSQLContext db = new MSSQLContext();
    }

    //精确到 course->klass_seminar
    public class course_seminar
    {
        //课程course ->  轮round -> 某节课seminar -> 某个班的某节klass_seminar -> attendance
        //              klass_round 某轮报名次数限制
        public course course;
        public List<round_seminar_klass> rs = new List<round_seminar_klass>();

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

    //[teacher]seminar下文件列表
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
            public decimal? report_score;
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
        public seminar_report(string str,int klass_seminar_id)
        {
            //str="ksid";
            var alist = from a in db.attendance where a.klass_seminar_id==klass_seminar_id select a;
            foreach (var a in alist)
            {
                team t = db.team.Find(a.team_id);
                var sslist = (from ss in db.seminar_score where ss.klass_seminar_id == klass_seminar_id && ss.team_id == t.id select ss).ToList();

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
                    id = a.id,
                    report_score = (sslist == null ? null : sslist[0].report_score)
                };
                list.Add(tmp);
            }
            listLength = list.Count;
        }
        MSSQLContext db = new MSSQLContext();
    }

    public class team_share
    {
        share_team_application sta;
        int mc, subc;
        public team_share(int id, int op = 1)//sta_id
        {
            sta = db.share_team_application.Find(id);
            if (op == 0)   //reject
            {
                db.share_team_application.Remove(sta);
                db.SaveChanges();
                return;
            }
            //agree
            sta.status = 1;
            mc = sta.main_course_id;
            subc = sta.sub_course_id;
            db.course.Find(sta.sub_course_id).team_main_course_id = sta.main_course_id;

            DelOldTeam();

            //copy team
            var tlist = (from t in db.team where t.course_id == mc select t).ToList();
            byte new_team_serial = 0;
            foreach (team t in tlist)
            {
                List<int> sidlist = (from ts in db.team_student where ts.team_id == t.id select ts.student_id).ToList();

                List<klass_student> actkslist = (from ks in db.klass_student where ks.course_id == subc && sidlist.Contains(ks.student_id) select ks).ToList();
                if (actkslist.Count() == 0) continue;
                new_team_serial++;

                //判断班级
                Dictionary<int, int> klass_cnt = new Dictionary<int, int>();
                int nkid = 0, ncnt = 0;
                foreach (var actks in actkslist)
                {
                    var kid = actks.klass_id;
                    if (klass_cnt.ContainsKey(kid)) klass_cnt[kid]++;
                    else klass_cnt[kid] = 1;
                    if (klass_cnt[kid] > ncnt)
                    {
                        ncnt = klass_cnt[kid];
                        nkid = kid;
                    }
                }
                int nkcnt = (from kt in db.klass_team where kt.klass_id == nkid select kt).Count() + 1;

                //新建Team和Team_Student
                team NewTeam = new team
                {
                    course_id = subc,
                    team_name = t.team_name,
                    klass_id = nkid,
                    klass_serial = (byte)nkcnt,
                    leader_id = actkslist[0].student_id,    //随便抓一个当组长
                    status = 0,
                    team_serial = new_team_serial,
                };
                db.team.Add(NewTeam);
                db.SaveChanges();
                new team_valid_judge(NewTeam.id);           //合法性

                db.klass_team.Add(new klass_team { klass_id = NewTeam.klass_id, team_id = NewTeam.id });
                foreach (var sid in actkslist)
                    db.team_student.Add(new team_student { student_id = sid.student_id, team_id = NewTeam.id });
                db.SaveChanges();
            }
        }
        public team_share(string cancel, int id)
        {
            if (cancel != "cancel") return;
            sta = db.share_team_application.Find(id);
            if (sta.status != 1) return;
            //agree
            sta.status = 1;
            mc = sta.main_course_id;
            subc = sta.sub_course_id;
            db.course.Find(sta.sub_course_id).team_main_course_id = null;
            db.share_team_application.Remove(sta);
            db.SaveChanges();

            DelOldTeam();
        }

        private void DelOldTeam()
        {
            var oldtlist = (from t in db.team where t.course_id == subc select t).ToList();
            List<int> tid = new List<int>();
            foreach (var t in oldtlist)
            {
                var oldktlist = (from kt in db.klass_team where kt.team_id == t.id && kt.klass_id == t.klass_id select kt).ToList();
                db.klass_team.Remove(oldktlist[0]);
                var oldrslist = (from rs in db.round_score where rs.team_id == t.id select rs).ToList();
                foreach (var rs in oldrslist) db.round_score.Remove(rs);
                var oldsslist = (from ss in db.seminar_score where ss.team_id == t.id select ss).ToList();
                foreach (var ss in oldsslist) db.seminar_score.Remove(ss);
                var oldatlist = (from a in db.attendance where a.team_id == t.id select a).ToList();
                foreach (var a in oldatlist) db.attendance.Remove(a);
                db.team.Remove(t);
            }
            db.SaveChanges();
        }

        MSSQLContext db = new MSSQLContext();
    }
    public class seminar_share
    {
        share_seminar_application ssa;
        public seminar_share(int id, int op = 1)
        {
            ssa = db.share_seminar_application.Find(id);
            if (op == 0)   //reject
            {
                db.share_seminar_application.Remove(ssa);
                db.SaveChanges();
                return;
            }
            //agree
            ssa.status = 1;
            int mc = ssa.main_course_id, subc = ssa.sub_course_id;
            db.course.Find(ssa.sub_course_id).seminar_main_course_id = ssa.main_course_id;
            db.SaveChanges();
        }
        public seminar_share(string cancel, int id)
        {
            if (cancel != "cancel") return;
            ssa = db.share_seminar_application.Find(id);
            if (ssa.status != 1) return;
            int mc = ssa.main_course_id, subc = ssa.sub_course_id;
            db.course.Find(ssa.sub_course_id).seminar_main_course_id = null;
            db.share_seminar_application.Remove(ssa);
            db.SaveChanges();
        }

        MSSQLContext db = new MSSQLContext();
    }
    public class team_valid_judge
    {
        team t;
        course c;
        public team_valid_judge(int id)
        {
            t = db.team.Find(id);
            c = db.course.Find(t.course_id);

            var tslist = (from ts in db.team_strategy where ts.course_id == id select ts).ToList();
            if (tslist.Count() > 0)//存在组队策略
            {

                //先确定and还是or
                var tsfirst = (from ts in db.team_strategy where ts.course_id == id && ts.strategy_serial == 1 select ts).ToList()[0];
                var sidlist = (from ts in db.team_student where ts.team_id == t.id select ts.student_id).ToList();
                bool ac = (tsfirst.strategy_name == "TeamAndStrategy" ? true : false);

                int cmlscnt = 0, cmlstotal = 0;
                foreach (var ts in tslist)
                {
                    int tid = ts.strategy_id;
                    switch (ts.strategy_name)
                    {
                        case "ConflictCourseStrategy":      //冲突课程校验 必须合法
                            var ccslist = (from ccs in db.conflict_course_strategy where ccs.id == tid select ccs.course_id).ToList();
                            Dictionary<int, int> cssjudge = new Dictionary<int, int>();
                            int csscnt = 0;
                            foreach (var cid in ccslist)
                            {
                                int cidcnt = (from ks in db.klass_student where ks.course_id == cid && sidlist.Contains(ks.student_id) select ks).Count();
                                if (cidcnt > 0) csscnt++;
                            }
                            if (csscnt > 1)                 //直接打回
                            {
                                t.status = 0;
                                db.SaveChanges();
                                return;
                            }
                            break;
                        case "CourseMemberLimitStrategy":         //不定个数 针对选修xx课程的组队
                            cmlstotal++;
                            course_member_limit_strategy cmls = db.course_member_limit_strategy.Find(tid);
                            int cmlscid = cmls.course_id;
                            int cmlscidcnt = (from ks in db.klass_student where ks.course_id == cmlscid && sidlist.Contains(ks.student_id) select ks).Count();
                            if (cmls.min_member <= cmlscidcnt && cmlscidcnt <= cmls.max_member) cmlscnt++;
                            break;
                    }
                }
                if ((ac == true && cmlscnt == cmlstotal) || (ac == false && cmlscnt > 0) || (cmlstotal == 0))
                {
                    t.status = 1;
                }
                else t.status = 0;
                db.SaveChanges();
            }
            else//不存在组队策略 直接判valid
            {
                t.status = 1;
                db.SaveChanges();
            }
        }

        MSSQLContext db = new MSSQLContext();
    }
}