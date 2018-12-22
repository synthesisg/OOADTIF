using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ooad.Models;

//xls
using System.Data;
using System.Data.OleDb;
//files
using System.IO;
using System.Text;

namespace ooad.Controllers
{
    public class StudentWebController : Controller {
        // GET: StudentWeb
        public ActionResult Index() {
            return View();
        }
        public ActionResult StudentLogin() {
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
                            Response.Write("<script type='text/javascript'>alert('Not Active!');</script>");
                        }
                        else
                        {
                            Session["user_id"] = ui[0].id;
                            Session["is_student"] = true;
                            Response.Redirect("ChsLesson");
                        }
                    }
                    else Session["is_student"] = false;
                    break;
            }
            return View();
        }
        public ActionResult ChsLesson(decimal round_id = 0)
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("TeacherLogin");
            }
            else Session["user_id"] = 9;

            decimal sid = Int32.Parse(Session["user_id"].ToString());
            var kque = from k in db.klass_student where k.student_id == sid select k;
            var klist = kque.ToList();
            List<course> co = new List<course>();
            foreach (var k in klist)
            {
                klass tmp = db.klass.Find(k.klass_id);
                co.Add(db.course.Find(tmp.course_id));
            }
            ViewBag.colist = co;

            //*
            string n = Request["round"];
            //数据库查询第n轮讨论课的信息并用ViewBag返回
            ViewBag.obMsg = new SeminarInfo
            {
                round = 1,
                count = 1,
                title = "需求分析",
                msg = "讨论课信息",
                signUpTime = "报名起止时间",
                reportTime = "报告截止时间"
            };
            //*/

            if (round_id != 0)
            {
                var slist = from s in db.seminar where s.round_id == round_id select s;
                ViewBag.seminarlist = slist.ToList();
            }
            return View();
        }



        public ActionResult SpecificSeminar(){
            //这里要讨论课数据，根据传的round和n来
            return View();
        }
        public ActionResult DownloadMarks() {
            if (is_judge) {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("TeacherLogin");
            }
            else Session["user_id"] = 11;


            decimal sid = Int32.Parse(Session["user_id"].ToString());
            var kque = from k in db.klass_student where k.student_id == sid select k;
            var klist = kque.ToList();
            List<course> co = new List<course>();
            foreach (var k in klist)
            {
                klass tmp = db.klass.Find(k.klass_id);
                co.Add(db.course.Find(tmp.course_id));
            }
            ViewBag.colist = co;

            decimal course_id = 666;
            var team_id_que = from ks in db.klass_student where ks.student_id == sid select ks;
            decimal? team_id = team_id_que.ToList()[0].team_id;
            var rque = from r in db.round where r.course_id == course_id select r;
            List<decimal> rid = new List<decimal>();
            foreach (var r in rque) rid.Add(r.id);
            var rscore = from rs in db.round_score where ( rid.Contains(rs.round_id) && rs.team_id==team_id ) select rs;
            ViewBag.rscore = rscore.ToList();
            return View();
        }
        public string crtmarkxls()
        {
            decimal course_id = 666;
            decimal sid = Decimal.Parse(Request["user_id"]);
            var team_id_que = from ks in db.klass_student where ks.student_id == sid select ks;
            decimal? team_id = team_id_que.ToList()[0].team_id;
            var rque = from r in db.round where r.course_id == course_id select r;
            List<decimal> rid = new List<decimal>();
            foreach (var r in rque) rid.Add(r.id);
            var rscore = from rs in db.round_score where (rid.Contains(rs.round_id) && rs.team_id == team_id) select rs;

            DataTable dt=new DataTable();
            dt.Columns.Add(new DataColumn("Round", typeof(string)));
            dt.Columns.Add(new DataColumn("Presentaion", typeof(string)));
            dt.Columns.Add(new DataColumn("Report", typeof(string)));
            dt.Columns.Add(new DataColumn("Question", typeof(string)));
            dt.Columns.Add(new DataColumn("Total", typeof(string)));

            int cnt = 1;
            foreach (var rs in rscore)
            {
                DataRow dr = dt.NewRow();
                dr = dt.NewRow();
                dr["Round"] = cnt++;
                dr["Presentation"] = rs.presentation_score;
                dr["Report"] = rs.report_score;
                dr["Question"] = rs.question_score;
                dr["Total"] = rs.total_score;
                dt.Rows.Add(dr);
            }
            return DataToExcel(dt);
        }

        public string RRoundInfo(string data)
        {
            return "1|2|3|4";
            decimal course_id = Decimal.Parse(data);
            var rlist = from r in db.round where r.course_id == course_id select r;
            string back = "";
            foreach (var r in rlist) back += r.round_serial.ToString() + '|' + r.id.ToString() + '|';
            back = back.Remove(back.Length - 1, 1);
            return back;
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
            return FileName;
        }

        public class SeminarInfo
        {
            public int round;
            public int count;
            public string title;
            public string msg;
            public string signUpTime;
            public string reportTime;
        }
        bool is_judge = false;
        MSSQLContext db = new MSSQLContext();
    }
}