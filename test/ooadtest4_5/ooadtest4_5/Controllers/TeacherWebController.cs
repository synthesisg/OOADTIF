using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ooadtest4_5.Models;

//xls
using System.Data;
using System.Data.OleDb;
//files
using System.IO;

namespace ooadtest4_5.Controllers
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
                    var uilist = from ui in uidb.data where (ui.academic_id == user && ui.password == pwd &&ui.is_student!=true) select ui;
                    if (uilist.Count()>0) //success
                    {
                        List<userinfo> ui = new List<userinfo>();
                        foreach (var u in uilist) ui.Add(u);
                        if (ui[0].is_valid != true)
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
            else Session["user_id"] = 11;


            int tid = Int32.Parse(Session["user_id"].ToString());
            var colist = from cse in codb.data where cse.teacher_id == tid select cse;
            string[] c1list = new string[colist.Count()];
            int cnt = 0;
            foreach (var co in colist)
            {
                c1list[cnt] = "";
                int coid = co.id;
                var ac1list = from c1 in c1db.data where c1.course_id == coid select c1;
                foreach (var a in ac1list) c1list[cnt] += a.name + '|' + a.id + '|';
                c1list[cnt]= c1list[cnt].Remove(c1list[cnt].Length - 1, 1);
                cnt++;
            }
            ViewBag.colist = colist;
            ViewBag.c1list = c1list;
            return View();
        }
        public string RClassInfo(string data)
        {
            int course_id = Int32.Parse(data);
            var c1list = from c1 in c1db.data where c1.course_id == course_id select c1;
            string back = "";
            foreach (var c1 in c1list) back += c1.name + '|' + c1.id + '|';
            back = back.Remove(back.Length - 1, 1);
            return back;
        }

        public ActionResult UploadList()
        {
            if (Request.Files.Count == 0) return Content("No detected file.");
            int class_id = Int32.Parse(Request["class_id"]);
            class1 c1 = c1db.data.Find(class_id);

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
                f.SaveAs(this.Server.MapPath("~/Files/Class/" + fname));
                LoadList("~/Files/Class/" + fname, class_id);
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
            else Session["user_id"] = 11;


            int teacher_id = Int32.Parse(Session["user_id"].ToString());
            userinfo ui = uidb.data.Find(teacher_id);
            var rco = from co in codb.data where co.teacher_id == teacher_id select co;


            SeminarInfo smnInfo = new SeminarInfo
            {
                round = 1,
                title = "需求设计",
                msg = "这是第一次讨论课信息"
            };
            SeminarInfo smnInfo2 = new SeminarInfo
            {
                round = 2,
                title = "界面设计",
                msg = "这是第二次讨论课信息"
            };
            SeminarInfo[] arrSmn = { smnInfo, smnInfo2 };
            ViewBag.arrSeminar = arrSmn;
            return View();
        }
        public string RRoundInfo(string data)
        {
            return "1|2|3|4";
            int course_id = Int32.Parse(data);
            var rlist = from r in rdb.data where r.course_id == course_id select r;
            string back = "";
            foreach (var r in rlist) back += r.number + '|';
            back = back.Remove(back.Length - 1, 1);
            return back;
        }

        public ActionResult InnerReport()
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
            return View();
        }
        public ActionResult DownloadMarks()
        {
            if (is_judge)
            {
                if (Session["is_teacher"] == null || (bool)Session["is_teacher"] == false)
                    return RedirectToAction("TeacherLogin");
            }
            else Session["user_id"] = 11;


            int tid = Int32.Parse(Session["user_id"].ToString());
            var colist = from cse in codb.data where cse.teacher_id == tid select cse;
            ViewBag.colist = colist;

            return View();
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

            //以前的记录
            var sclist = from sc in scdb.data where sc.class_id == class_id select sc;
            List<int> scid = new List<int>();
            foreach (var sc in sclist) scid.Add(uidb.data.Find(sc.student_id).id);

            for (int i = 0; i < data.Rows.Count; i++)
            {
                string acad_id = data.Rows[i][0].ToString().Trim();
                if (acad_id == null || acad_id == "" || acad_id == "学号")  continue;    //忽略首行与空行(合并行)

                //创建账户
                var uitest = from ui in uidb.data where ui.academic_id.Contains(acad_id) select ui;
                if (uitest.Count() == 0)
                {
                    var addus = new userinfo
                    {
                        academic_id = acad_id,
                        password = "123456",
                        name = data.Rows[i][1].ToString().Trim(),
                        is_student=true,
                        is_valid=false
                    };
                    uidb.data.Add(addus);
                    uidb.SaveChanges();
                }

                var uilist = from ui in uidb.data where ui.academic_id.Contains(acad_id) select ui;
                int uid = 0;
                foreach (var ui in uilist) uid = ui.id;
                if (scid.Contains(uid)) scid.Remove(uid);   //仍在表内，踢出list
                else                                        //不在表内，加入选课表
                {
                    var addsc = new select_course { student_id = uid, class_id = class_id };
                    scdb.data.Add(addsc);
                }
            }

            //删除不在新表内的
            var dellist = from sc in scdb.data where (scid.Contains(sc.student_id) && sc.class_id == class_id) select sc;
            foreach (var del in dellist) scdb.data.Remove(del);

            //保存
            scdb.SaveChanges();
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

        userinfoDB uidb = new userinfoDB();
        courseDB codb = new courseDB();
        class1DB c1db = new class1DB();
        roundDB rdb = new roundDB();
        select_courseDB scdb = new select_courseDB();
        bool is_judge = false;
    }
}