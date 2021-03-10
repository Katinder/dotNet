using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SampleRestApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get(int id)  //localhost:1234/api/values
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
       /* public HttpResponseMessage Get() //localhost:1234/api/values?id=1
        {
            string path = "C:\\Users\\HP\\Desktop\\test_file.json";
            string json = "";
            try 
            { 
                using(StreamReader r =new StreamReader(path))
                {
                    json = r.ReadToEnd();
                   
                }
                string sJSONResponse = JsonConvert.SerializeObject(json);

                //create response
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(sJSONResponse);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return response;
            }
            catch (Exception e)
            {
                string sJSONResponse = JsonConvert.SerializeObject(e);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(sJSONResponse);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return response;
            }
            
        }
       */

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