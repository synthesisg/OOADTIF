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
namespace ooadtest4_5.Controllers
{
    public class BController : Controller
    {
        public BController()
        {
            //SQL init
            constr = ConfigurationManager.AppSettings["connstring"].ToString();
            myconn = new SqlConnection(constr);
            myconn.Open();
        }
        // GET: B
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Alogin()
        {
                    ViewBag.cdx = 666;
                    ViewData["CDX2"] = "cdx";
                    int[] a = new int[] { 1, 2, 3, 4, 5 };
                    ViewBag.arr = a;
            switch (Request.HttpMethod) {
                case "GET":         //load
                    return View();
                case "POST":        //login
                    user = Request["user"];
                    pswd = Request["pswd"];
                    Session["user"] = user;
                    Session["pswd"] = pswd;
                    if(account(user,pswd))//if (user == "123" && pswd == "456") //success
                    {
                        Response.Redirect("Index");
                        return Index();
                    }
                    else                                //fail
                        return View();
            }
            return View();
        }
        public ActionResult afile()
        {
            return View();
        }
        [HttpPost]
        public void uploadfile()
        {
                    if (Request.Files.Count > 0)
                    {
                HttpPostedFileBase f = Request.Files["afile"];//[0];
                        string fname = f.FileName;
                        /* startIndex */
                        int index = fname.LastIndexOf("\\") + 1;
                        /* length */
                        int len = fname.Length - index;
                        fname = fname.Substring(index, len);
                        /* save to server */
                        f.SaveAs(this.Server.MapPath("~/Files/" + fname));
                        Response.Write("<script type='text/javascript'>alert('Success!');</script>");
                    }
        }
        public FileResult Download(string fileName)
        {
            return File(Server.MapPath("~/Files/" + fileName), "application/octet-stream", fileName);
            FileStream fs = new FileStream(Server.MapPath("~/Files/"+fileName), FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.Charset = "UTF-8";
            Response.ContentType = "application/octet-stream";
            Response.ContentEncoding = System.Text.Encoding.Default;
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
            //return new EmptyResult();

        }
        public ActionResult personal()
        {
            return Content( "personal"+Session["user"]);
        }
        //[HttpPost]
        public ActionResult judgelogin()
        {
            user = Request["user"];
            pswd = Request["pswd"];
            Session["user"] = user;
            Session["pswd"] = pswd;
            if (user == "123" && pswd == "456")
                //Response.Redirect("personal");
                return personal();
            else
                return Content("WA!");
                //Response.Redirect("gg");
        }

        public ActionResult Amail()
        {
            SmtpClient client = new SmtpClient("smtp.163.com", 25);
            Random Rdm = new Random();
            int iRdm = Rdm.Next(0, 100000);
            MailMessage msg = new MailMessage("13600858179@163.com", "110110071@qq.com", "激活瓜皮账户", "感谢使用瓜皮课堂\n您的验证码为"+iRdm.ToString());
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicAuthenticationInfo =
                new System.Net.NetworkCredential("13600858179@163.com", "20100710A");
            client.Credentials = basicAuthenticationInfo;
            client.EnableSsl = true;
            client.Send(msg);
            return Content("s");
        }
        public ActionResult Axls()
        {
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Server.MapPath("~/Files/NET.xls") + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            // 若为xlsx 需要修改Extended Properties='Excel 12.0;HDR=NO;IMEX=1';
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
                for (int j=0;j<data.Columns.Count;j++)
                    output+=data.Rows[i][j].ToString().Trim()+' ';
                    //可能带有多余空格
                output += '\n';
            }
            return Content(output);
        }

        public ActionResult Asql()
        {
            string str = "select * from us where [user]='123';";
            SqlCommand cmd = new SqlCommand(str, myconn);
            SqlDataReader dataReader = cmd.ExecuteReader();
            string output="";
            while (dataReader.Read())
            {
                output += dataReader[0].ToString() + ' ' + dataReader[1].ToString() + ' ' ;
            }
                return Content(output);
        }
        public ActionResult msql(Models.AtestDB atest)
        {
            if (atest.ID == 7) return Content("AC");
            else return Content("WA");
        }
        string user, pswd;
        public ActionResult testquery()
        {
            AtestDBContext db = new AtestDBContext();
            var cdx = db.Atest.Find(2); //by key

            //by someone
            var query = from d in db.Atest
                        where d.Title=="4" select d;
            string opt = "";
            foreach (var tmp in query)
            {
                opt += tmp.ID.ToString() + tmp.Title + tmp.Date;
            }
            return Content("[by ID]"+cdx.Date.ToString()+"&[by some]"+opt);
        }
        public ActionResult testqueryall()
        {
            AtestDBContext db = new AtestDBContext();
            string opt="";
            foreach (var tmp in db.Atest.ToList())
            {
                opt += tmp.ID.ToString() + tmp.Title+tmp.Date;
            }
            return Content(opt);
        }
        public ActionResult testadd()
        {
            usContext db = new usContext();
            var cdx = new us
            {
                user = "123456",
                pwd = "654321",
            };
            db.db.Add(cdx);
            db.SaveChanges();
            return Content("done");
        }
        public ActionResult testup()
        {
            AtestDBContext db = new AtestDBContext();
            var query = from d in db.Atest
                        where d.Title == "mod"
                        select d;
            if (query.Count()==0) return Content("Fail.");
            //update one(by key)
            var cdx = db.Atest.Find(3);
            cdx.Director = "MOD";
            db.Entry<AtestDB>(cdx).State= System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            //update normal
            string opt = "";
            foreach (var tmp in query)
            {
                //tmp.Director = "aftmod";
                opt += tmp.ID.ToString() + tmp.Title + tmp.Date;
            }
            db.SaveChanges();
            return Content("[by some]" + opt);
        }
        public ActionResult testdel()
        {
            AtestDBContext db = new AtestDBContext();
            AtestDB at = db.Atest.Find(6);
            db.Atest.Remove(at);
            db.SaveChanges();
            return Content("done");
        }
        private bool account(string user,string pswd)
        {
            string str = "select * from us where [user]='" + user + "';";
            SqlDataReader reader= SQLQuery(str);
            if (reader.HasRows)
            {
                reader.Read();
                if ((string)reader["pwd"] == pswd) return true;
                else return false;
            }
            else return false;
        }

        private SqlDataReader SQLQuery(string str)
        {
            SqlCommand cmd = new SqlCommand(str, myconn);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader;
        }
        //SQL
        String constr;
        SqlConnection myconn;
    }
}