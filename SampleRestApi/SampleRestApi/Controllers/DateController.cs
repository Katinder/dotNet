using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SampleRestApi.Controllers
{
    public class DateController : ApiController
    {
        private static readonly string connectionString = "server=127.0.0.1;port=3306;uid=root;pwd=root";
        private static MySqlConnection conn = new MySqlConnection(connectionString);

        //only timestamp
        public HttpResponseMessage GetOverview(string timestamp)
        {
            DateTime dateValue;
           
            ///check if a date logic
            if (DateTime.TryParseExact(timestamp, "yyyyMMddHHmmss", new CultureInfo("en-US"), DateTimeStyles.None, out dateValue))
            {
                string query = "";
                if (timestamp.All(char.IsDigit) && timestamp.Length == 14)
                {
                         query = string.Format("SELECT * FROM masterdb.estimate_master where DATE_FORMAT(dt_updated, '%Y%m%d%H%i%s')>='{0}'", timestamp);             
                }

                //sql logic

                if (OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var dt = new DataTable();

                    try
                    {
                        MySqlDataReader queryOp = cmd.ExecuteReader();
                       
                        dt.Load(queryOp);

                        queryOp.Close();

                        CloseConnection();

                        string sJSONResponse = JsonConvert.SerializeObject(dt);

                        //create response
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent(sJSONResponse);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        return response;

                    }
                    catch (MySqlException ex)
                    {
                        //Console.WriteLine(ex.Message);
                        //return ex.Message;
                        CloseConnection();

                        string sJSONResponse = JsonConvert.SerializeObject(ex);
                        //create response
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent(sJSONResponse);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        return response;
                    }                

                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }


            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        
        //get timestamp and location
        public HttpResponseMessage GetOverview(string timestamp, string loc)
        {
            DateTime dateValue;
            //int count = 0;

            ///check if a date logic
            if (DateTime.TryParseExact(timestamp, "yyyyMMddHHmmss", new CultureInfo("en-US"), DateTimeStyles.None, out dateValue))
            {
                
               // string main_query = $"select * from (select c1_pk, max(c4_updated_at) as last_update " +
               // $"from schema1.log_table group by c1_pk) as x " +
               // $"inner join schema1.log_table as l on l.c1_pk=x.c1_pk and l.c4_updated_at=x.last_update " +
               // $"where l.log_id in (select log_id from schema1.log_table where c4_updated_at>= '{dateString}');";

                string query = "";
                if (timestamp.All(char.IsDigit) && timestamp.Length == 14)
                {
                    if (loc == "ALL")
                    {
                        query = string.Format("SELECT * FROM masterdb.estimate_master where DATE_FORMAT(dt_updated, '%Y%m%d%H%i%s')>='{0}';",timestamp);
                            
                    }
                    else
                    {
                        query = string.Format("SELECT * FROM masterdb.estimate_master where location_code='{0}' and DATE_FORMAT(dt_updated, '%Y%m%d%H%i%s')>='{1}';", loc, timestamp);
                    }
                }

                //sql logic

                if (OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    //List<EstimateClass> outputList = new List<EstimateClass>();
                    var dt = new DataTable();

                    try
                    {
                        MySqlDataReader queryOp = cmd.ExecuteReader();
                        
                        dt.Load(queryOp);

                        queryOp.Close();
                        CloseConnection();
                        string sJSONResponse = JsonConvert.SerializeObject(dt);

                        //create response
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent(sJSONResponse);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        return response;

                    }
                    catch (MySqlException ex)
                    {
                        //Console.WriteLine(ex.Message);
                        //return ex.Message;

                        string sJSONResponse = JsonConvert.SerializeObject(ex);

                        //create response
                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StringContent(sJSONResponse);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        return response;
                    }
                    
                    
                    
                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
                //return count.ToString();

            }
            

            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        //get details using estimade_codes
        public HttpResponseMessage GetDetails([FromUri]string[] pk)
        {
            string estimate_code_string = "";

            foreach (string s in pk)
            {
                estimate_code_string += ",\""+s+"\"";
            }

            estimate_code_string=estimate_code_string.Remove(0, 1);

            string query = "";

            query = string.Format("SELECT * FROM masterdb.estimate_item_details where ESTIMATE_CODE IN ({0})", estimate_code_string);
            
            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var dt = new DataTable();

                try
                {
                    MySqlDataReader queryOp = cmd.ExecuteReader();
                    //EstimateClass newRecord = new EstimateClass();


                    dt.Load(queryOp);

                    queryOp.Close();

                    CloseConnection();
                    string sJSONResponse = JsonConvert.SerializeObject(dt);

                    //create response
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(sJSONResponse);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return response;

                }
                catch (MySqlException ex)
                {
                    //Console.WriteLine(ex.Message);
                    //return ex.Message;
                    CloseConnection();
                    string sJSONResponse = JsonConvert.SerializeObject(ex);

                    //create response
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(sJSONResponse);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return response;
                }

            }

            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            
            /*string sJSONResponse = JsonConvert.SerializeObject(pk);

            //create response
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(sJSONResponse);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return response;
            */
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

/*
 * pk=E-1718-203-01089&pk=E-1819-212-00677&pk=E-1920-203-00583&pk=E-1920-214-00674&pk=E-1920-603-00681&pk=E-1920-636-03763&pk=E-1920-636-03764&pk=E-1920-636-03765&pk=E-2021-221-03778&pk=E-2021-221-03779&pk=E-2021-221-03780&pk=E-2021-613-02607&pk=E-2021-613-03782&pk=E-2021-613-03783&pk=E-2021-613-03786&pk=E-2021-622-03785&pk=E-2021-623-03757&pk=E-2021-624-02125&pk=E-2021-631-03751&pk=E-2021-631-03752&pk=E-2021-631-03770&pk=E-2021-631-03771&pk=E-2021-631-03772&pk=E-2021-631-03773&pk=E-2021-631-03775&pk=E-2021-631-03776&pk=E-2021-636-03760&pk=E-2021-636-03761&pk=E-2021-636-03767&pk=E-2021-636-03768&pk=E-2021-662-03762&pk=E-2021-662-03766&pk=E-2021-662-03769&pk=E-2021-669-03756&pk=E-2021-669-03777&pk=E-2021-670-03774&pk=E-2021-671-03784&pk=E-2021-780-01741&pk=E-2021-782-01997&pk=E-2021-782-02735&pk=E-2021-782-03456&pk=E-2021-782-03781&pk=E-2021-793-01907
 */
