using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SampleRestApi.Controllers
{
    public class ApiOutput
    {

        public int log_id { get; set; }
        public int c1_pk { get; set; }
        public string c2_name { get; set; }
        public float c3_amount { get; set; }
        public DateTime c4_updated_at { get; set; }
        public string Operation { get; set; }
    }

    public class DateController : ApiController
    {
        private static readonly string connectionString = "server=127.0.0.1;port=3306;uid=root;pwd=root";
        private static MySqlConnection conn = new MySqlConnection(connectionString);

        //get timestamp
        public HttpResponseMessage Get(string dateString)
        {
            DateTime dateValue;
            int count = 0;

            ///check if a date logic
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out dateValue))
            {
                
                string main_query = $"select * from (select c1_pk, max(c4_updated_at) as last_update " +
                $"from schema1.log_table group by c1_pk) as x " +
                $"inner join schema1.log_table as l on l.c1_pk=x.c1_pk and l.c4_updated_at=x.last_update " +
                $"where l.log_id in (select log_id from schema1.log_table where c4_updated_at>= '{dateString}');";

                //sql logic
                
                if (OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(main_query, conn);
                    List<ApiOutput> outputList = new List<ApiOutput>();

                    try
                    {
                        MySqlDataReader queryOp = cmd.ExecuteReader();

                        //create a list of objects from datatable
                        while (queryOp.Read())
                        {
                            ApiOutput newRecord = new ApiOutput();
                            newRecord.log_id = (int)queryOp["log_id"];
                            newRecord.c1_pk = (int)queryOp["c1_pk"];
                            newRecord.c2_name = (string)queryOp["c2_name"];
                            newRecord.c3_amount = (int)queryOp["c3_amount"];
                            newRecord.c4_updated_at = (DateTime)queryOp["c4_updated_at"];
                            newRecord.Operation = (string)queryOp["operation"];

                            outputList.Add(newRecord);

                            //count = count + 1;
                            //Console.WriteLine(queryOp[0] + " -- " + queryOp[1] + " -- " + queryOp[2] + " -- " + queryOp[3] + " -- " + queryOp[4] + " -- " + queryOp[5] + " -- " + queryOp[6] + " -- " + queryOp[7]);
                            //Console.WriteLine(queryOp.ToString());
                        }

                        queryOp.Close();

                    }
                    catch (MySqlException ex)
                    {
                        //Console.WriteLine(ex.Message);
                        //return ex.Message;
                    }
                    
                    CloseConnection();
                    string sJSONResponse = JsonConvert.SerializeObject(outputList);

                    //create response
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(sJSONResponse);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return response;
                    
                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
                //return count.ToString();

            }
            

            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private static bool CloseConnection()
        {
            try
            {
                conn.Close();
                //Console.WriteLine("Connection Closed.");
                return true;
            }
            catch (MySqlException ex)
            {
                //Console.WriteLine(ex.Message);
                //Console.ReadKey();
                return false;
            }
        }

        private static bool OpenConnection()
        {
            try
            {
                conn.Open();
                //Console.WriteLine("Connection Opened.");
                return true;
            }
            catch (MySqlException ex)
            {
                //Console.WriteLine(ex.Message);
                //Console.ReadKey();
                return false;
            }
        }

    }
}
