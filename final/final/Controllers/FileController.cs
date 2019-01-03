using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//xls
using System.Data;
using System.Data.OleDb;
//files
using System.IO;
using System.Text;

using final.Models;
namespace final.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        //Student Upload ppt&report
        public ActionResult Upload_stu()
        {
            if (Request.Files.Count == 0) return Content("No detected file.");
            int ksid = Int32.Parse(Request["ksid"]);
            string filetype = Request["filetype"];
            int sid = Int32.Parse(Session["user_id"].ToString());
            int team_id = new qt().ks2t(ksid, sid);
            var at = (from a in db.attendance where a.klass_seminar_id == ksid && a.team_id == team_id select a).ToList();
            if (at.Count() == 0) return Content("Identity verification error.");
            int at_id = at[0].id;

            HttpPostedFileBase f = Request.Files[0];
            string fname = at_id.ToString() + '/' + f.FileName;
            string ServerPath = Server.MapPath("~/Files/" + fname);

            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Files/"+ at_id.ToString()));
            if (!dir.Exists) dir.Create();
            if (System.IO.File.Exists(ServerPath)) System.IO.File.Delete(ServerPath);
            f.SaveAs(ServerPath);
            

            switch (filetype)
            {
                case "ppt":
                    at[0].ppt_name = f.FileName;
                    at[0].ppt_url = "?ksid=" + at[0].klass_seminar_id.ToString() + "&order=" + at[0].team_order + "&path=" + f.FileName;
                    break;
                case "report":
                    at[0].report_name = f.FileName;
                    at[0].report_url = "?ksid=" + at[0].klass_seminar_id.ToString() + "&order=" + at[0].team_order + "&path=" + f.FileName;
                    break;
                default:
                    return Content("error file type.");
            }
            db.SaveChanges();

            if (Request.Browser.Platform.ToString() == "WinNT")
                return Redirect("/StudentWeb/SpecificSeminar/" + db.klass_seminar.Find(ksid).seminar_id.ToString());
            else return Redirect("StudentMobile/KlassSEminar/" + ksid.ToString());
        }
        //Download Seminar ppt&report
        public FileResult Download_stu(int ksid,int order,string path)
        {
            var at = (from a in db.attendance where a.klass_seminar_id == ksid && a.team_order == order select a.id).ToList();
            if (at.Count() == 0) return null;
            int atid = at[0];
            return File(Server.MapPath("~/Files/" + atid.ToString() + '/' + path), "application/octet-stream", path);
        }

        public FileResult Download(string path)
        {
            return File(Server.MapPath("~/Files/" + path), "application/octet-stream", path);
        }

        public string DataToExcel(DataTable m_DataTable)
        {
            var dt = DateTime.Now;
            string FileName = System.Web.HttpContext.Current.Server.MapPath("~/Files/" + string.Format("{0:yyyyMMddHHmmssffff}", dt) + ".xls");
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

        MSSQLContext db = new MSSQLContext();
    }
}