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
    public class TeacherWebController : Controller
    {
        // GET: TeacherWeb
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TeacherLogin()
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
                            Response.Write("<script type='text/javascript'>alert('Not Active!');</script>");
                        }
                        else
                        {
                            Session["user_id"] = ui[0].id;
                            Session["is_teacher"] = true;
                            Response.Redirect("TeacherImport");
                        }
                    }
                    else Session["is_teacher"] = false;
                    break;
            }
            return View();
        }
        public ActionResult TeacherImport()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("TeacherLogin");
            }
            else Session["user_id"] = test_id;


            int tid = Int32.Parse(Session["user_id"].ToString());
            var colistt = from cse in db.course where cse.teacher_id == tid select cse;
            var colist = colistt.ToList();
            string[] c1list = new string[colist.Count()];
            int cnt = 0;
            foreach (var co in colist)
            {
                c1list[cnt] = "";
                decimal coid = co.id;
                var ac1list = from c1 in db.klass where c1.course_id == coid select c1;
                foreach (var a in ac1list.ToList()) c1list[cnt] += a.klass_serial.ToString() + '|' + a.id.ToString() + '|';
                c1list[cnt] = c1list[cnt].Remove(c1list[cnt].Length - 1, 1);
                cnt++;
            }
            ViewBag.colist = colist;
            ViewBag.c1list = c1list;
            return View();
        }
        public string RClassInfo(string data)
        {
            int course_id = Int32.Parse(data);
            var c1list = from c1 in db.klass where c1.course_id == course_id select c1;
            string back = "";
            foreach (var c1 in c1list) back += c1.grade.ToString() + '-' + c1.klass_serial.ToString() + '|' + c1.id.ToString() + '|';
            back = back.Remove(back.Length - 1, 1);
            return back;
        }

        public ActionResult UploadList()
        {
            if (Request.Files.Count == 0) return Content("No detected file.");
            int class_id = Int32.Parse(Request["class_id"]);
            klass c1 = db.klass.Find(class_id);

            HttpPostedFileBase f = Request.Files[0];
            string str = f.FileName;
            str = str.Substring(str.Length - 3);
            int type = 0;
            if (str == "xls") type = 1;
            if (str == "lsx") type = 2;
            if (type > 0)
            {

                string fname = class_id.ToString() + ".xls";
                if (type == 2) fname += 'x';

                if (System.IO.File.Exists(Server.MapPath("~/Files/class/" + fname)))
                {
                    System.IO.File.Delete(Server.MapPath("~/Files/class/" + fname));
                }

                f.SaveAs(Server.MapPath("~/Files/class/" + fname));
                LoadList("~/Files/class/" + fname, class_id);
                Response.Write("<script type='text/javascript'>alert('Success!');</script>");
                return RedirectToAction("TeacherImport");
            }
            else
                return Content("Not a Excel.");
        }
        public ActionResult DownloadReport()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("TeacherLogin");
            }
            else Session["user_id"] = test_id;


            int teacher_id = Int32.Parse(Session["user_id"].ToString());
            teacher ui = db.teacher.Find(teacher_id);
            var rco = from co in db.course where co.teacher_id == teacher_id select co;
            ViewBag.colist = rco;

            return View();

            //确定round后
            int round_id = 1;
            var slist = from s in db.seminar where s.round_id == round_id select s;
            ViewBag.s = slist.ToList();
        }
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

        public ActionResult InnerReport(int id)//seminar_id
        {
            string n = Request["round"];
            //数据库查询第n次讨论课的信息并用ViewBag返回
            ViewBag.obMsg = new SeminarInfo
            {
                round = 1,
                title = "需求分析",
                msg = "讨论课信息",
                signUpTime = "报名起止时间",
                reportTime = "报告截止时间"
            };

            seminar_report sr = new seminar_report(id);
            ViewBag.sr = sr;
            return View();
        }
        public ActionResult inner(int id)//seminar_id
        {
            seminar_report sr = new seminar_report(id);
            ViewBag.sr = sr;
            return View();
        }
        public ActionResult DownloadMarks()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("TeacherLogin");
            }
            else Session["user_id"] = test_id;


            int tid = Int32.Parse(Session["user_id"].ToString());
            var colist = from cse in db.course where cse.teacher_id == tid select cse;
            ViewBag.colist = colist;

            return View();
            
        }
        public string queryMarkData(int round_id) {
            var rslist = from rs in db.round_score where rs.round_id == round_id select rs;
            ViewBag.rslistCount = rslist.Count();
            return Newtonsoft.Json.JsonConvert.SerializeObject(rslist.ToList());
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
        public int queryCourseRound(int courseId) {
            return 3;
        }
        public string querySeminarData(int roundN) {
            int round_id = roundN;
            var slist = from s in db.seminar where s.round_id == round_id select s;
            //[图片]ViewBag.rs = slist;
            return Newtonsoft.Json.JsonConvert.SerializeObject(slist.ToList());
        }

        public class SeminarInfo
        {
            public int round;
            public string title;
            public string msg;
            public string signUpTime;
            public string reportTime;
        }
        public class Student
        {
            public string name { get; set; }
        }
        bool is_judge = false;
        int test_id = 3;
        MSSQLContext db = new MSSQLContext();
    }
}