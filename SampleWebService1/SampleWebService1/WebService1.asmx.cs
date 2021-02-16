using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SampleWebService1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")] //namespace to identify the ws and its vars uniquely
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService //inheritance necessary for using the asp.net sessions and objects
    {

        [WebMethod] //necessary attribute to expose the method. else normal method, cannot be used by consumer
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public int Add(int first,int second)
        {
            //int sumtotal = 0;
            //for(int i=0; i<listInt.Count; i++)
            //{
            //   sumtotal += listInt[i];
            //}
            //return sumtotal;
            return first + second;
        }
    }
}
