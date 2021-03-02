using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SampleRestApi.Controllers
{
    public class HelloController : ApiController
    {
        //all public methods here are action methods

        // GET: Hello
        public string Get()
        {
            return "Hello from the controller";
        }
    }
}