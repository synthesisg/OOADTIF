using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BCL.WebSockets;

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
            if (_handlers.ContainsKey(user))
            {
                var origHandler = _handlers[user];
                await origHandler.Close();
            }

            _handlers[user] = webSocketHandler;

            webSocketHandler.TextMessageReceived += ((sendor, msg) =>
            {
                BroadcastMessage(user, user + "Says: " + msg);
            });

            webSocketHandler.Closed += (sendor, arg) =>
            {
                BroadcastMessage(user, user + " Disconnected!");
                _handlers.Remove(user);
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
    }
}
