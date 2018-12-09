using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ooadtest4_5.Models;
//mail
using System.Net;
using System.Net.Mail;
//SQL
using System.Configuration;
using System.Data.SqlClient;
//xls
using System.Data;
using System.Data.OleDb;
//files
using System.IO;

/*
 * 均未填写noticeDB
 * 对于email 单独运行另一个服务器程序
 * Timer click 3600
 */
namespace ooadtest4_5.Controllers
{
    public class UsefulController : Controller
    {
        // GET: Useful
        public UsefulController()
        {
        }
        public ActionResult Index()
        {
            return View();
        }

        //account=======================================================================================================================================================
        void UpdatePassword(int user_id, string pwd)//after sendcode
        {
            userinfo u = uidb.data.Find(user_id);
            u.password = pwd;
            uidb.SaveChanges();
        }
        void UpdateEmail(int user_id, string email)
        {
            userinfo u = uidb.data.Find(user_id);
            u.email = email;
            uidb.SaveChanges();
        }
        void UpdateInterval(int user_id, int interval)
        {
            userinfo u = uidb.data.Find(user_id);
            u.interval = interval;
            uidb.SaveChanges();
        }
        void Activate(int user_id, string pwd, string email)
        {
            userinfo ui = uidb.data.Find(user_id);
            ui.is_valid = true;
            UpdatePassword(user_id, pwd);
            if (ui.is_student != true)
                UpdateEmail(user_id, email);
            uidb.SaveChanges();
        }
        void SendVeriCode(int user_id, string email = "")
        {
            userinfo ui = uidb.data.Find(user_id);
            string em = ui.email;
            if (ui.is_valid != true && ui.is_student == true) em = email;
            //SendEmail
            SmtpClient client = new SmtpClient("smtp.163.com", 25);
            Random Rdm = new Random();
            string iRdm = Rdm.Next(0, 100000).ToString().PadLeft(6, '0');
            MailMessage msg = new MailMessage("13600858179@163.com", em, "激活瓜皮账户", "感谢使用瓜皮课堂\n您的验证码为" + iRdm);
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicAuthenticationInfo =
                new System.Net.NetworkCredential("13600858179@163.com", "20100710A");
            client.Credentials = basicAuthenticationInfo;
            client.EnableSsl = true;
            client.Send(msg);

            //update table
            var query = from vc in vcdb.data where vc.id == user_id select vc;
            if (query.Count() > 0)//update
            {
                foreach (var cdx in query)
                {
                    cdx.time = DateTime.Now;
                    cdx.code = iRdm;
                }
            }
            else//create
            {
                var cdx = new verification_code
                {
                    id = 123,
                    code = iRdm,
                    user_id = user_id,
                    time = DateTime.Now
                };
                vcdb.data.Add(cdx);
            }
            vcdb.SaveChanges();
        }
        private bool login(string acad_id, string pwd)
        {
            var query = from tmp in uidb.data where (tmp.academic_id == acad_id && tmp.password == pwd) select tmp;
            if (query.Count() > 0)
            {
                Session["is_login"] = true;
                foreach (var cdx in query) Session["user_id"] = cdx.id;
                return true;
            }
            return false;
        }
        private void logout()
        {
            Session["is_login"] = false;
        }


        //Course_Teacher==============================================================================================================================================
        List<int> MyCourse_Stu(int student_id)
        {
            List<int> course_id = new List<int>();
            userinfo ui = uidb.data.Find(student_id);
            var rsc = from sc in scdb.data where sc.student_id == student_id select sc;
            foreach (var r in rsc) course_id.Add(c1db.data.Find(r.class_id).course_id);
            return course_id;
        }
        List<int> MyCourse_Tea(int teacher_id)
        {
            userinfo ui = uidb.data.Find(teacher_id);
            var rco = from co in codb.data where co.teacher_id == teacher_id select co.id;
            List<int> course = ex(rco);
            return course;
        }
        void CreateCourse(course co)
        {
            codb.data.Add(co);
            codb.SaveChanges();
        }
        void DelCourse(int course_id)
        {
            //course
            course co = codb.data.Find(course_id);
            codb.data.Remove(co); codb.SaveChanges();

            //class
            var class1id = from c1 in c1db.data where c1.course_id == course_id select c1.id;
            foreach (int id in class1id) c1db.data.Remove(c1db.data.Find(id));
            c1db.SaveChanges();

            //team_share
            var tseid = from Tse in tsedb.data where (Tse.from_course_id == course_id || Tse.to_course_id == course_id) select Tse.id;
            foreach (int id in tseid) tsedb.data.Remove(tsedb.data.Find(id)); tsedb.SaveChanges();

            //team_submit
            var tstid = from Tst in tstdb.data where class1id.Contains(tdb.data.Find(Tst.team_id).class_id) select Tst.id;
            foreach (int id in tstid) tstdb.data.Remove(tstdb.data.Find(id)); tstdb.SaveChanges();

            //seminar_share
            var ssid = from Ss in ssdb.data where (Ss.from_course_id == course_id || Ss.to_course_id == course_id) select Ss.id;
            foreach (int id in ssid) ssdb.data.Remove(ssdb.data.Find(id)); ssdb.SaveChanges();

            //select_course
            var scid = from sc in scdb.data where class1id.Contains(sc.class_id) select sc;
            foreach (var id in scid) scdb.data.Remove(id); scdb.SaveChanges();

            //team
            var tid = from t in tdb.data where class1id.Contains(t.class_id) select t.id;
            foreach (var id in tid) tdb.data.Remove(tdb.data.Find(id)); tdb.SaveChanges();

            //team_member
            var tmid = from tm in tmdb.data where tid.Contains(tm.team_id) select tm.id;
            foreach (var id in tmid) tmdb.data.Remove(tmdb.data.Find(id)); tmdb.SaveChanges();

            //seminar

            //presentation

            //question
            codb.data.Remove(co);
            codb.SaveChanges();
        }
        void CteateClass(class1 c1)
        {
            c1db.data.Add(c1);
            c1db.SaveChanges();
        }
        public ActionResult LoadList(string path, int class_id)   //"Data Source=" + Server.MapPath("~/Files/NET.xls") + ";"
        {
            string strConn = "";
            if (path.Last() == 'x')
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Server.MapPath(path) + ";" + "Extended Properties='Excel 12.0;HDR=NO;IMEX=1';";
            else
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Server.MapPath(path) + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            string strExcel = "select * from   [sheet1$]";
            DataSet ds = new DataSet();
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter(strExcel, strConn);
            adapter.Fill(ds, "sheet1");

            conn.Close();

            DataTable data = ds.Tables["sheet1"];
            string output = "";
            for (int i = 0; i < data.Rows.Count; i++)
            {
                for (int j = 0; j < data.Columns.Count; j++)
                    output += data.Rows[i][j].ToString().Trim() + ' ';
                //可能带有多余空格
                output += '\n';
            }

            //学号    姓名  所属系 专业========================================================================================================[nep]===================
            /*
             *  仅完成添加
             *  考虑构建remove list 用于删除
             */
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (data.Rows[i][0].ToString().Trim() == "学号") continue;    //忽略首行
                var user = from tmp in uidb.data where tmp.academic_id == data.Rows[i][0].ToString().Trim() select tmp;
                if (user.Count() == 0)//create
                {
                    var addus = new userinfo
                    {
                        academic_id = data.Rows[i][0].ToString().Trim(),
                        password = "123456",
                        name = data.Rows[i][1].ToString().Trim()
                    };
                    uidb.data.Add(addus);
                }
                var addsc = new select_course { student_id = 5, class_id = class_id };
                scdb.data.Add(addsc);
            }
            uidb.SaveChanges();
            scdb.SaveChanges();
            return Content(output);
        }

        //Team===========================================================================================================================================================
        List<int> QueryList(int course_id)
        {
            course co = codb.data.Find(course_id);
            var rc1 = from c1 in c1db.data where c1.course_id == course_id select c1;
            List<int> student_id = new List<int>();
            foreach (var r in rc1)
            {
                int class_id = r.id;
                var rsc = from sc in scdb.data where (sc.class_id == class_id && sc.hasteam != true) select sc;
                foreach (var rr in rsc) student_id.Add(rr.student_id);
            }
            return student_id;
        }
        List<int> QueryMember(int student_id, int course_id)
        {
            List<int> member = new List<int>();
            var rtm = from tm in tmdb.data where tm.student_id == student_id select tm.team_id;
            int team_id = 0;
            foreach (var rr in rtm)
            {
                if (course_id == c1db.data.Find(tdb.data.Find(rr).class_id).course_id)
                    team_id = rr;
            }
            member.Add(tdb.data.Find(team_id).leader_id);
            rtm = from tm in tmdb.data where tm.team_id == team_id select tm.student_id;
            foreach (var r in rtm) if (r != member[0]) member.Add(r);
            return member;
        }
        void AddtoTeam(int team_id, int student_id)
        {
            team t = tdb.data.Find(team_id);
            class1 c1 = c1db.data.Find(t.class_id);
            course co = codb.data.Find(c1.course_id);

            //[select_course]true
            var rsc = from sc in scdb.data where (sc.student_id == student_id && sc.class_id == t.class_id) select sc;
            foreach (var r in rsc) r.hasteam = true;
            scdb.SaveChanges();

            //[tem_member]add
            var addtm = new team_member { team_id = team_id, student_id = student_id };
            tmdb.data.Add(addtm);
            tmdb.SaveChanges();

            //[team]valid
            var rtm = from tm in tmdb.data where tm.team_id == team_id select tm;
            if (rtm.Count() > co.max_member_limit || rtm.Count() < co.min_member_limit) t.is_valid = false;
            else t.is_valid = true;
            tdb.SaveChanges();
        }
        public void RemovefromTeam(int team_id, int student_id)
        {
            team t = tdb.data.Find(team_id);
            class1 c1 = c1db.data.Find(t.class_id);
            course co = codb.data.Find(c1.course_id);

            //[select_course]false
            var rsc = from sc in scdb.data where (sc.student_id == student_id && sc.class_id == t.class_id) select sc;
            foreach (var r in rsc) r.hasteam = false;
            scdb.SaveChanges();

            //[team_member]remove
            var retm = from tm in tmdb.data where (tm.team_id == team_id && tm.student_id == student_id) select tm;
            foreach (var r in retm) tmdb.data.Remove(r);
            tmdb.SaveChanges();

            //[team]valid
            var rtm = from tm in tmdb.data where tm.team_id == team_id select tm;
            if (rtm.Count() > co.max_member_limit || rtm.Count() < co.min_member_limit) t.is_valid = false;
            else t.is_valid = true;
            tdb.SaveChanges();
        }
        public void DisbandTeam(int team_id)
        {
            team t = tdb.data.Find(team_id);
            class1 c1 = c1db.data.Find(t.class_id);
            course co = codb.data.Find(c1.course_id);

            //[select_course]false
            var rsc = from sc in scdb.data where (sc.class_id == t.class_id) select sc;
            foreach (var r in rsc) r.hasteam = false;
            scdb.SaveChanges();

            //[team_member]remove
            var retm = from tm in tmdb.data where (tm.team_id == team_id) select tm;
            foreach (var r in retm) tmdb.data.Remove(r);
            tmdb.SaveChanges();

            //[team]remove
            tdb.data.Remove(t);
            tdb.SaveChanges();
        }
        bool SubmitTeam(int team_id)
        {
            team t = tdb.data.Find(team_id);
            if (t.is_valid == true) return false;
            team_submit tst = new team_submit
            {
                team_id = team_id
            };
            return true;
        }
        //share&submit==================================================================================================================================================
        void SendTeamShare(int from_course_id, int to_course_id)
        {
            team_share tse = new team_share
            {
                from_course_id = from_course_id,
                is_src_agree = true,
                to_course_id = to_course_id,
            };
            tsedb.data.Add(tse);
            tsedb.SaveChanges();
        }
        void RequestTeamShare(int from_course_id, int to_course_id)
        {
            team_share tse = new team_share
            {
                from_course_id = from_course_id,
                is_dst_agree = true,
                to_course_id = to_course_id,
            };
            tsedb.data.Add(tse);
            tsedb.SaveChanges();
        }
        bool AgreeTeamShare(int tseid)
        {
            team_share tse = tsedb.data.Find(tseid);
            if (tse.state == true) return false;
            if (tse.is_dst_agree == true)
                tse.is_src_agree = true;
            else tse.is_dst_agree = true;
            tse.state = true;
            tsedb.SaveChanges();
            return true;
        }
        bool RejectTeamShare(int tseid)
        {
            team_share tse = tsedb.data.Find(tseid);
            if (tse.state == true) return false;
            if (tse.is_dst_agree == true)
                tse.is_src_agree = false;
            else tse.is_dst_agree = false;
            tse.state = true;
            tsedb.SaveChanges();
            return true;
        }
        bool CancelTeamShare(int tseid)
        {
            team_share tse = tsedb.data.Find(tseid);
            if (tse.state != true) return false;
            if (tse.is_src_agree != true || tse.is_dst_agree != true) return false;
            tse.is_src_agree = false;
            tse.is_dst_agree = false;
            tsedb.SaveChanges();
            return true;
        }

        void SendSeminarShare(int from_course_id, int to_course_id)
        {
            seminar_share ss = new seminar_share
            {
                from_course_id = from_course_id,
                is_src_agree = true,
                to_course_id = to_course_id,
            };
            ssdb.data.Add(ss);
            ssdb.SaveChanges();
        }
        void RequestSeminarShare(int from_course_id, int to_course_id)
        {
            seminar_share ss = new seminar_share
            {
                from_course_id = from_course_id,
                is_dst_agree = true,
                to_course_id = to_course_id,
            };
            ssdb.data.Add(ss);
            ssdb.SaveChanges();
        }
        bool AgreeSeminarShare(int ssid)
        {
            seminar_share ss = ssdb.data.Find(ssid);
            if (ss.state == true) return false;
            if (ss.is_dst_agree == true)
                ss.is_src_agree = true;
            else ss.is_dst_agree = true;
            ss.state = true;
            ssdb.SaveChanges();
            return true;
        }
        bool RejectSeminarShare(int tseid)
        {
            seminar_share ss = ssdb.data.Find(tseid);
            if (ss.state == true) return false;
            if (ss.is_dst_agree == true)
                ss.is_src_agree = false;
            else ss.is_dst_agree = false;
            ss.state = true;
            ssdb.SaveChanges();
            return true;
        }
        bool CancelSeminarShare(int tseid)
        {
            seminar_share ss = ssdb.data.Find(tseid);
            if (ss.state != true) return false;
            if (ss.is_src_agree != true || ss.is_dst_agree != true) return false;
            ss.is_src_agree = false;
            ss.is_dst_agree = false;
            ssdb.SaveChanges();
            return true;
        }
        bool AgreeTeamSubmit(int tstid)
        {
            team_submit tst = tstdb.data.Find(tstid);
            if (tst.state == true) return false;
            team t = tdb.data.Find(tst.team_id);
            t.is_valid = true;
            tdb.SaveChanges();
            tst.state = true;
            tstdb.SaveChanges();
            return true;
        }
        bool RejectTeamSubmit(int tstid)
        {
            team_submit tst = tstdb.data.Find(tstid);
            if (tst.state == true) return false;
            team t = tdb.data.Find(tst.team_id);
            t.is_valid = false;
            tdb.SaveChanges();
            tst.state = true;
            tstdb.SaveChanges();
            return true;
        }

        //Seminar_Teacher==============================================================================================================================
        void CreateRound(int course_id, bool is_pre_avg, bool is_rep_avg, bool is_que_avg)
        {
            var rr = from r in rdb.data where r.course_id == course_id select r.id;
            round addr = new round
            {
                course_id = course_id,
                number = rr.Count() + 1,
                is_pre_avg = is_pre_avg,
                is_rep_avg = is_rep_avg,
                is_que_avg = is_que_avg,
            };
            rdb.data.Add(addr);
            rdb.SaveChanges();

            var c1id = from c1 in c1db.data where c1.course_id == course_id select c1.id;
            foreach (int id in c1id)
            {
                var addcr = new class_round
                {
                    class_id = id,
                    round_id = addr.id,
                    times = 1
                };
                crdb.data.Add(addcr);
            }
            crdb.SaveChanges();
        }
        void ModifyRound(round mor)
        {
            round r = rdb.data.Find(mor.id);
            r = mor;
            rdb.SaveChanges();
        }
        void CreateSeminar(seminar s)
        {
            sdb.data.Add(s);
            sdb.SaveChanges();
        }
        void ModifySeminar(seminar mos)
        {
            seminar s = sdb.data.Find(mos.id);
            s = mos;
            sdb.SaveChanges();
        }
        void DelSeminar(int seminar_id)
        {
            seminar s = sdb.data.Find(seminar_id);
            sdb.data.Remove(s);
            sdb.SaveChanges();
        }
        //SeminarProcess================================================================================================================================
        bool Begin(int seminar_id)//continue
        {
            seminar s = sdb.data.Find(seminar_id);
            if (s.state == true) return false;
            s.state = false;
            sdb.SaveChanges();
            return true;
        }
        void  NextPresnetation(int seminar_id)
        {
            //拉取前端时间存入
        }
        void NextQuestion()
        {

        }
        void pause(int seminar_id)
        {

            //拉取前端时间存入
        }
        void score()
        {
            string oper=Request["oper"];
            int id = Int32.Parse(Request["id"]);
            decimal score= Decimal.Parse(Request["score"]);
        }
        //Presentation==================================================================================================================================
        void SignUpSeiminar(int team_id,int seminar_id,int queue_no=0)
        {
            presentation p=new presentation
            {
                team_id=team_id,
                round_seminar_id=seminar_id,
                queue_no=queue_no
            };
        }
        [HttpPost]
        bool UploadPPT(int presentation_id)
        {
            if (Request.Files.Count == 0) return false;

            presentation p = pdb.data.Find(presentation_id);

            HttpPostedFileBase f = Request.Files[0];
            string fname = presentation_id + '-' + "ppt";
            f.SaveAs(this.Server.MapPath("~/Files/Presentaion/" + fname));

            p.presentation_file = fname;
            pdb.SaveChanges();
            Response.Write("<script type='text/javascript'>alert('Success!');</script>");
            return true;
        }
        [HttpPost]
        bool UploadReport(int presentation_id)
        {
            if (Request.Files.Count == 0) return false;

            presentation p = pdb.data.Find(presentation_id);

            HttpPostedFileBase f = Request.Files[0];
            string fname = presentation_id+'-'+"rep";
            f.SaveAs(this.Server.MapPath("~/Files/Presentaion/" + fname));

            p.report_file = fname;
            pdb.SaveChanges();
            Response.Write("<script type='text/javascript'>alert('Success!');</script>");
            return true;
        }
        void CancelPresentation(int presentation_id)
        {
            presentation p = pdb.data.Find(presentation_id);
            pdb.data.Remove(p);
            pdb.SaveChanges();
        }
        //==============================================================================================================================================
        private List<T> ex<T>(IQueryable<T> cdx)
        {
            List<T> ret = new List<T>();
            foreach (T id in cdx) ret.Add(id);
            return ret;
        }

        adminDB adb = new adminDB();
        userinfoDB uidb = new userinfoDB();
        courseDB codb = new courseDB();
        class1DB c1db = new class1DB();
        select_courseDB scdb = new select_courseDB();
        teamDB tdb = new teamDB();
        team_memberDB tmdb = new team_memberDB();
        roundDB rdb = new roundDB();
        class_roundDB crdb = new class_roundDB();
        round_seminarDB rsdb = new round_seminarDB();
        seminarDB sdb = new seminarDB();
        presentationDB pdb = new presentationDB();
        questionDB qdb = new questionDB();
        team_shareDB tsedb = new team_shareDB();
        team_submitDB tstdb = new team_submitDB();
        seminar_shareDB ssdb = new seminar_shareDB();
        verification_codeDB vcdb = new verification_codeDB();
        noticeDB ndb = new noticeDB();
    }
}