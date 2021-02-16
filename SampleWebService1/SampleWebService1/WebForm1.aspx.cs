using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SampleWebService1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button_click(object sender, EventArgs e)
        {
            WebService1 webService = new WebService1();
            List<int> lstIntegers = new List<int> { 5, 5, 5 };
            Label1.Text = "Output of WebService: " + webService.Add(2,3).ToString();

        }  
    }
}