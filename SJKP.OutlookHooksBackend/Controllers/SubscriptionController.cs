using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SJKP.OutlookAddinTemplateBackend.Controllers
{
    public class SubscriptionController : ApiController
    {

        [Route("api/subscription/message")]
        [HttpPost]
        public IHttpActionResult ValidationEndpoint(string validationtoken)
        {            
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(validationtoken, System.Text.Encoding.UTF8, "text/plain");
            
            return ResponseMessage(resp);
        }

        [Route("api/subscription/message")]
        [HttpPost]
        public IHttpActionResult MessageNotification(dynamic message)
        {
            Console.WriteLine(JsonConvert.SerializeObject(message));
            return Ok();
        }
    }
}
