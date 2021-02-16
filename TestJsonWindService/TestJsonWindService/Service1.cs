using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TestJsonWindService
{
    public partial class Service1 : ServiceBase
    {
        readonly Timer timer = new Timer();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
            WriteToFile("Service started at " +DateTime.Now.ToString());

            try
            {
                localhostwsjson.wsjson serviceObj = new localhostwsjson.wsjson();
                //string svcM1 = serviceObj.HelloWorld();
                //WriteToFile("output m1 " + svcM1);
                serviceObj.DoNothing();
                WriteToFile("Do nothing from on start:ok ");
            }

            catch
            {
                WriteToFile("Connection to web service failed");
            }

            timer.Interval = 10000;
            timer.Elapsed += OnElapsedEvent;
            timer.Enabled = true;
            timer.AutoReset = true;

        }

        private void OnElapsedEvent(object sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                localhostwsjson.wsjson serviceObj = new localhostwsjson.wsjson();
                //string svcM1 = serviceObj.HelloWorld();
                //WriteToFile("output m1 " + svcM1+" at "+ DateTime.Now.ToString());
                serviceObj.DoNothing();
                WriteToFile("method op parsed successfully");
            }

            catch
            {
                WriteToFile("Connection to web service failed!");
                WriteToFile("Last Attempt at " + DateTime.Now.ToString());
            }
           
        }

        private void WriteToFile(string v)
        {
            //throw new NotImplementedException();
            string path = "C:\\Users\\HP\\source\\repos\\TestJsonWindService\\TestJsonWindService\\bin\\Debug\\TestServiceLog1.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format(v, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }

        protected override void OnStop()
        {
            WriteToFile("Service stopped at " + DateTime.Now.ToString());
            timer.Stop();       
        }

    }
}
