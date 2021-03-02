using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SampleRestApi.Controllers
{
    public class ValuesController1 : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()  //localhost:1234/api/values
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id) //localhost:1234/api/values?id=1
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value) //localhost:1234/api/values
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value) //localhost:1234/api/values?id=1
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id) //localhost:1234/api/values?id=1
        {
        }
    }
}