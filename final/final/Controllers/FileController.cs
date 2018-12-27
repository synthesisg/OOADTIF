using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
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

            return Redirect("/StudentWeb/SpecificSeminar/" + db.klass_seminar.Find(ksid).seminar_id.ToString());
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
        MSSQLContext db = new MSSQLContext();
    }
}