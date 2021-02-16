using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace wajson
{
    /// <summary>
    /// Summary description for wsjson
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsjson : System.Web.Services.WebService
    {

        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RandomNumber()
        {
            Random ran_word = new Random();
            int num=ran_word.Next(0, 100);
      
            // Return JSON data
            JavaScriptSerializer js = new JavaScriptSerializer();
            string strJSON = js.Serialize(num);
            return strJSON;
        }

        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string TimeNow()
        {
            //return DateTime.Now.ToLongDateString();
            // Return JSON data
            JavaScriptSerializer js = new JavaScriptSerializer();
            string strJSON = js.Serialize(DateTime.Now.ToLongDateString());
            return strJSON;
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DoNothing()
        {
            //no  code
        }
    }
}

