using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using final.Models;

//xls
using System.Data;
using System.Data.OleDb;
//files
using System.IO;
using System.Text;

namespace final.Controllers
{
    public class StudentWebController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("StudentLogin");
        }
        public ActionResult StudentLogin()
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
                            Response.Write("<script type='text/javascript'>alert('Not Active!');</script>");
                        }
                        else {
                            Session["user_id"] = ui[0].id;
                            Session["is_student"] = true;
                            return RedirectToAction("ChsLesson");
                        }
                    }
                    else { Session["is_student"] = false;
                        ViewBag.mes = "账号或密码错误";
                    }
                    break;
            }
            return View();
        }
        public ActionResult ChsLesson()
        {
            if (is_judge)
            {
                if (Session["is_student"] == null || (bool)Session["is_student"] == false)
                    return RedirectToAction("StudentLogin");
            }
            else Session["user_id"] = test_id;

            int sid = Int32.Parse(Session["user_id"].ToString());
            var kque = from k in db.klass_student where k.student_id == sid select k;
            var klist = kque.ToList();
            List<course> co = new List<course>();
            foreach (var k in klist)
            {
                klass tmp = db.klass.Find(k.klass_id);
                co.Add(db.course.Find(tmp.course_id));
            }
            ViewBag.colist = co;

            
            return View();
        }
        
        public ActionResult SpecificSeminar(int id)//seminar_id
        {
            if (is_judge) {
                if (Session["is_student"] == null || (bool)Session["is_student"] == false)
                    return RedirectToAction("StudentLogin");
            }
            else Session["user_id"] = test_id;

            seminar s = db.seminar.Find(id);
            int sid=Int32.Parse(Session["user_id"].ToString());
            int klass_id = (from kss in db.klass_student where kss.course_id == s.course_id && kss.student_id == sid select kss).ToList()[0].klass_id;
            var kslist = from ks in db.klass_seminar where ks.seminar_id == id && ks.klass_id == klass_id select ks; 
            
            ViewBag.s = s;
            klass_seminar_enroll_state_model model = new klass_seminar_enroll_state_model(kslist.ToList()[0].id, sid);
            ViewBag.model = model;
            return View();
        }
        public ActionResult DownloadMarks()
        {
            if (is_judge)
            {
                if (Session["is_student"] == null || (bool)Session["is_student"] == false)
                    return RedirectToAction("StudentLogin");
            }
            else Session["user_id"] = test_id;


            int sid = Int32.Parse(Session["user_id"].ToString());
            var kque = from k in db.klass_student where k.student_id == sid select k;
            var klist = kque.ToList();
            List<course> co = new List<course>();
            foreach (var k in klist)
            {
                klass tmp = db.klass.Find(k.klass_id);
                co.Add(db.course.Find(tmp.course_id));
            }
            ViewBag.colist = co;

            return View();
        }


        //Createxls
        public ActionResult crtmarkxls(int id)  //course_id
        {
            int sid = Int32.Parse(Session["user_id"].ToString());

            var klass_list = (from k in db.klass where k.course_id == id select k.id).ToList();

            int team_id = new qt().c2t(id, sid);

            var rque = from r in db.round where r.course_id == id select r;
            List<int> rid = new List<int>();
            foreach (var r in rque) rid.Add(r.id);
            var rscore = from rs in db.round_score where (rid.Contains(rs.round_id) && rs.team_id == team_id) select rs;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Round", typeof(string)));
            dt.Columns.Add(new DataColumn("Presentaion", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Report", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Question", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Total", typeof(decimal)));

            int cnt = 1;
            foreach (var rs in rscore)
            {
                DataRow dr = dt.NewRow();
                dr = dt.NewRow();
                dr[0] = cnt++;
                dr[1] = (rs.presentation_score==null?0: rs.presentation_score);
                dr[2] = (rs.report_score == null ? 0 : rs.report_score);
                dr[3] =  (rs.question_score == null ? 0 : rs.question_score);
                dr[4] = (rs.total_score == null ? 0 : rs.total_score);
                dt.Rows.Add(dr);
            }
            return Redirect("/File/Download?path=" + DataToExcel(dt));
        }
        public string DataToExcel(DataTable m_DataTable)
        {
            var dt = DateTime.Now;
            string FileName = Server.MapPath("~/Files/" + string.Format("{0:yyyyMMddHHmmssffff}", dt) + ".xls");
            if (System.IO.File.Exists(FileName))
            {
                System.IO.File.Delete(FileName);
            }
            FileStream objFileStream;
            StreamWriter objStreamWriter;
            string strLine = "";
            objFileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
            objStreamWriter = new StreamWriter(objFileStream, Encoding.Unicode);


            for (int i = 0; i < m_DataTable.Columns.Count; i++)
            {
                strLine = strLine + m_DataTable.Columns[i].Caption.ToString() + Convert.ToChar(9);      //写列标题
            }
            objStreamWriter.WriteLine(strLine);
            strLine = "";
            for (int i = 0; i < m_DataTable.Rows.Count; i++)
            {
                for (int j = 0; j < m_DataTable.Columns.Count; j++)
                {
                    if (m_DataTable.Rows[i].ItemArray[j] == null)
                        strLine = strLine + " " + Convert.ToChar(9);                                    //写内容
                    else
                    {
                        string rowstr = "";
                        rowstr = m_DataTable.Rows[i].ItemArray[j].ToString();
                        if (rowstr.IndexOf("\r\n") > 0)
                            rowstr = rowstr.Replace("\r\n", " ");
                        if (rowstr.IndexOf("\t") > 0)
                            rowstr = rowstr.Replace("\t", " ");
                        strLine = strLine + rowstr + Convert.ToChar(9);
                    }
                }
                objStreamWriter.WriteLine(strLine);
                strLine = "";
            }
            objStreamWriter.Close();
            objFileStream.Close();
            return string.Format("{0:yyyyMMddHHmmssffff}", dt) + ".xls";
        }

        //AJAX:
        public string RRoundInfo(string data)
        {
            int course_id = Int32.Parse(data);
            var rlist = from r in db.round where r.course_id == course_id select r;
            if (rlist.Count() > 0)
            {
                string back = "";
                foreach (var r in rlist) back += r.round_serial.ToString() + '|' + r.id.ToString() + '|';
                return back.Remove(back.Length - 1, 1);
            }
            else return "";
        }
        public string querySeminarData(int roundN)
        {
            int round_id = roundN;
            var slist = from s in db.seminar where s.round_id == round_id select s;
            return Newtonsoft.Json.JsonConvert.SerializeObject(slist.ToList());
        }
        public string getmark(int course_id)
        {
            int sid = Int32.Parse(Session["user_id"].ToString());
            scoreboard_course model = new scoreboard_course(course_id, sid);

            return Newtonsoft.Json.JsonConvert.SerializeObject(model);
        }
        public bool signup(string data)
        {
            int ksid = 1;
            byte order = 1;

            int sid = Int32.Parse(Session["user_id"].ToString());
            int team_id = new qt().k2t(db.klass_seminar.Find(ksid).klass_id, sid);
            if (team_id == 0) return false;

            var alist_order = from a in db.attendance where a.klass_seminar_id == ksid && a.team_order == order select a;
            if (alist_order.Count() > 0) return false;//这坑有人了
            var alistteam = from a in db.attendance where a.klass_seminar_id == ksid && a.team_id == team_id select a;
            if (alistteam.Count() > 0)//改变报名次序
            {
                attendance at = alistteam.ToList()[0];
                at.team_order = order;
            }
            else
            {
                attendance NewA = new attendance
                {
                    klass_seminar_id = ksid,
                    team_id = team_id,
                    team_order = order,
                    is_present = 0
                };
                db.attendance.Add(NewA);
            }
            return true;
        }
        public bool cancelsignup(string data)
        {
            int ksid = 1;

            int sid = Int32.Parse(Session["user_id"].ToString());
            int team_id = new qt().k2t(db.klass_seminar.Find(ksid).klass_id, sid);
            if (team_id == 0) return false;
            
            var alistteam = from a in db.attendance where a.klass_seminar_id == ksid && a.team_id == team_id select a;
            if (alistteam.Count() == 0)//未报名
                return false;
            attendance at = alistteam.ToList()[0];
            db.attendance.Remove(at);
            db.SaveChanges();
            return false;
        }
        bool is_judge = false;
        int test_id = 166;
        MSSQLContext db = new MSSQLContext();
    }
}