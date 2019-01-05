using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using final.WebSockets;

using final.Models;
namespace final.Controllers
{
    [RoutePrefix("api/wsconn")]
    public class wsController : ApiController
    {
        private static Dictionary<string, WebSocketHandler> _handlers = new Dictionary<string, WebSocketHandler>();

        [Route]
        [HttpGet]
        public async Task<HttpResponseMessage> Connect(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var webSocketHandler = new WebSocketHandler();
            /*
            if (_handlers.ContainsKey(user))
            {
                var origHandler = _handlers[user];
                await origHandler.Close();
            }
            //*/
            _handlers[user] = webSocketHandler;

            webSocketHandler.TextMessageReceived += ((sendor, msg) =>
            {
                string[] str = msg.Split('|');
                string ret = "";
                int ksid=Int32.Parse(str[1]);
                attendance cnt = Now(ksid);
                switch(str[0])
                {
                    case "1":   //Question
                        int sid = Int32.Parse(str[2]);
                        if(question(ksid, sid))
                        {
                            int atid = Now(ksid).id;
                            ret = "1|" + ksid.ToString()+'|'+(from q in db.question where q.attendance_id==atid && q.is_selected!=1 select q).Count().ToString();
                        }
                        break;
                    case "2":   //Extract
                        question ex = Extract(ksid);
                        if (ex != null)
                        {
                            int atid = Now(ksid).id;
                            ret = "2|" + ksid.ToString() + '|' + new qt().t2ts(ex.team_id) + ' ' + db.student.Find(ex.student_id).student_name + '|' + ex.id.ToString() + '|' + (from q in db.question where q.attendance_id == atid && q.is_selected != 1 select q).Count().ToString();
                        }
                        break;
                    case "3":   //Next
                        attendance a = NextAttendace(ksid);
                        if (a == null)
                        {
                            ret = "4|" + ksid.ToString();  //End
                            break;
                        }
                        else
                        {
                            ret = "3|" + ksid.ToString() + '|' + new qt().t2ts(a.team_id) + '|' + a.id.ToString();
                            break;
                        }
                }
                if(ret!="")
                BroadcastMessage(user, ret);
            });

            webSocketHandler.Closed += (sendor, arg) =>
            {
                BroadcastMessage(user, user + " Disconnected!");
                //_handlers.Remove(user);
            };

            webSocketHandler.Opened += (sendor, arg) =>
            {
                BroadcastMessage(user, user + " Connected!");
            };

            HttpContext.Current.AcceptWebSocketRequest(webSocketHandler);

            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }

        private void BroadcastMessage(string sid, string message)
        {
            foreach (var handlerKvp in wsController._handlers)
            {
                //if (handlerKvp.Key != sid)
                {
                    handlerKvp.Value.SendMessage(message).Wait();
                }
            }
        }

        //[student]发出提问
        public bool question(int ksid,int sid)   
        {
            attendance now = Now(ksid);
            int team_id = new qt().ks2t(ksid, sid);
            if (now == null || team_id == 0) return false;
            var ql = (from q in db.question where q.klass_seminar_id == ksid && q.student_id == sid && q.is_selected == 0 && q.attendance_id == now.id select q).Count();
            if (ql > 0) return false;
            question NewQuestion = new question
            {
                klass_seminar_id = ksid,
                student_id = sid,
                team_id = team_id,
                attendance_id = now.id
            };
            db.question.Add(NewQuestion);
            db.SaveChanges();
            return true;
        }

        //[teacher]下一组
        public attendance NextAttendace(int ksid)
        {
            attendance now = Now(ksid);
            int cnt = 1, max_team = db.seminar.Find(db.klass_seminar.Find(ksid).seminar_id).max_team;
            bool found = false;
            if (now!=null)
            {
                cnt = now.team_order + 1;
                now.is_present = 0;
                db.SaveChanges();
            }

            List<attendance> anlist=new List<attendance>();
            while (found == false && cnt <= max_team)
            {
                anlist = (from a in db.attendance where a.klass_seminar_id == ksid && a.team_order == cnt select a).ToList();
                if (anlist.Count() == 0) cnt++;
                else found = true;
            }
            if (cnt > max_team) return null;
            attendance at = db.attendance.Find(anlist[0].id);
            at.is_present = 1;
            db.SaveChanges();
            return at;
        }

        //[teacher]抽取提问
        public question Extract(int ksid)
        {
            int atid = Now(ksid).id;
            var qlist = (from q in db.question where q.klass_seminar_id == ksid && q.attendance_id == atid select q).ToList();
            if (qlist.Count() == 0) return null;
            qlist[0].is_selected = 1;
            db.SaveChanges();
            return qlist[0];
        }

        //[public]当前attendance
        public attendance Now(int ksid)
        {
            var alist = (from a in db.attendance where a.klass_seminar_id == ksid && a.is_present == 1 select a).ToList();
            if (alist.Count() > 0) return alist[0];
            else return null;
        }
        MSSQLContext db = new MSSQLContext();
    }
}
