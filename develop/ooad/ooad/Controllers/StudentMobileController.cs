using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ooadtest4_5.Controllers
{
    public class StudentMobileController : Controller
    {
        // GET: StudentMobile
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login() {
            return View();
        }
        public ActionResult FindPassword() {
            return View();
        }
        public ActionResult Activate() {
            return View();
        }
        public ActionResult Seminar() {
            return View();
        }
        public ActionResult ChsSpecSeminar() {
            //TitleText = 课程名称
            ViewBag.TitleText = "1";
            return View();
        }public ActionResult BUEnrollSmn() {
            //TitleText = 课程名称+讨论课名
            ViewBag.TitleText = "1";
            return View();         
        }
        public void SendPW2Email(string email) {

        }
        public string Move2CourSeminar(string str) {
            return "a";
        }
        public string Enroll(string str) {
            return "123";
        }
    }
}