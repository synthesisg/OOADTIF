using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ooadtest4_5.Controllers
{
    public class AController : Controller
    {
        // GET: A
        public ActionResult Index()
         {
             return View();
         }
        public ActionResult Aindex()
        {
            return View();
        }
        public string Index2()
          {
              return "This is my <b>default</b> action";
          }
  
         public string Welcome()
         {
            return "This is Welcome action method...";
         }
        public string Welcome2(string name, int numTimes = 1)
         {
             //return "This is Welcome2 action method...";
             return HttpUtility.HtmlEncode("Hello " + name + ",NumTimes is :" + numTimes);
         }
}
    
}