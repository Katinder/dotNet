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

namespace EstimateRESTApi.Controllers
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
            if (DateTime.TryParseExact(timestamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
            {
                
                string masterQuery = string.Format("SELECT * FROM masterdb.estimate_master where DATE_FORMAT(dt_updated, '%Y%m%d%H%i%s')>='{0}'", timestamp);
                
                //sql logic

 
                try
                {
                    var dt1 = new DataTable();

                    if (OpenConnection() == true)
                    {
                        MySqlCommand cmd = new MySqlCommand(masterQuery, conn);
                        MySqlDataReader masterQueryOp = cmd.ExecuteReader();
                        dt1.Load(masterQueryOp);
                        masterQueryOp.Close();
                        CloseConnection();
                    }

                    //add new column for details json
                    dt1.Columns.Add("details", typeof(DataTable)).AllowDBNull = true;

                    //-------------------GET TABLE 2--------------------

                    if (dt1.Rows.Count > 0)
                    {
                        //get estimate_codes from dt1
                        string estimate_code_string = "";
                        foreach (DataRow row in dt1.Rows)
                        {
                            //in sql format "E-123","E-234","E-345"
                            estimate_code_string += ",\"" + (string)row["ESTIMATE_CODE"] + "\"";
                        }

                        estimate_code_string = estimate_code_string.Remove(0, 1);


                        //query details database for dt2

                        string detailsQuery = string.Format("SELECT * FROM masterdb.estimate_item_details where ESTIMATE_CODE IN ({0})", estimate_code_string);
                        var dt2 = new DataTable();

                        if (OpenConnection() == true)
                        {
                            MySqlCommand cmd2 = new MySqlCommand(detailsQuery, conn);
                            MySqlDataReader detailsQueryOp = cmd2.ExecuteReader();
                            dt2.Load(detailsQueryOp);
                            detailsQueryOp.Close();
                            CloseConnection();
                        }


                        //--------------FIND ESTIMATE DETAILS FROM DT2(details) ACC TO DT1(master)-----------------
                        foreach (DataRow row in dt1.Rows)
                        {
                            //get datarow aray from dt2 where estimate code is same as in the current dt1 row
                            DataRow[] foundRows = dt2.Select($"estimate_code = '{((string)row["ESTIMATE_CODE"])}'");

                            if (foundRows.Length > 0)
                            {
                                //get the format from dt2 
                                //(new table required because cannot convert datarow to json directly)
                                DataTable cloned = dt2.Clone();

                                foreach (DataRow r in foundRows)
                                {
                                    //add the row to the new table
                                    //Console.WriteLine("{0}, {1}", r[0], r[1]);
                                    cloned.Rows.Add(r.ItemArray);
                                }

                                //copy the new table to details column of the current row
                                row["details"] = cloned;
                            }

                            //if no rows found in details table
                            else
                            {
                                row["details"] = null;
                            }


                        }
                    }
                    //------------SERIALIZE & CREATE RESPONSE--------------------
                    string sJSONResponse = JsonConvert.SerializeObject(dt1);

                    //create response
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(sJSONResponse);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return response;

                }
                catch (MySqlException ex)
                {
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
                //change for exception logging
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }


        //get timestamp and location
        public HttpResponseMessage GetOverview(string timestamp, string loc)
        {
            DateTime dateValue;
            
            if (DateTime.TryParseExact(timestamp, "yyyyMMddHHmmss", new CultureInfo("en-US"), DateTimeStyles.None, out dateValue))
            {
                
                string masterQuery = "";

                if (loc == "ALL")
                {
                    masterQuery = string.Format("SELECT * FROM masterdb.estimate_master where DATE_FORMAT(dt_updated, '%Y%m%d%H%i%s')>='{0}';", timestamp);

                }
                else
                {
                    masterQuery = string.Format("SELECT * FROM masterdb.estimate_master where location_code='{0}' and DATE_FORMAT(dt_updated, '%Y%m%d%H%i%s')>='{1}';", loc, timestamp);
                }

                //sql logic    

                try
                {
                    
                    var dt1 = new DataTable();

                    if (OpenConnection() == true)
                    {
                        MySqlCommand cmd = new MySqlCommand(masterQuery, conn);
                        MySqlDataReader masterQueryOp = cmd.ExecuteReader();
                        dt1.Load(masterQueryOp);
                        masterQueryOp.Close();
                        CloseConnection();
                    }
                        
                    //add new column for details json
                    dt1.Columns.Add("details", typeof(DataTable)).AllowDBNull = true;


                    //-------------------GET TABLE 2--------------------

                    //if datatable 1 is not null
                    if (dt1.Rows.Count > 0)
                    {
                       
                        //get estimate_codes from dt1
                        string estimate_code_string = "";
                        foreach (DataRow row in dt1.Rows)
                        {
                            //in sql format "E-123","E-234","E-345"
                            estimate_code_string += ",\"" + (string)row["ESTIMATE_CODE"] + "\"";
                        }

                        estimate_code_string = estimate_code_string.Remove(0, 1);


                        //query details database for dt2

                        string detailsQuery = string.Format("SELECT * FROM masterdb.estimate_item_details where ESTIMATE_CODE IN ({0})", estimate_code_string);
                        var dt2 = new DataTable();

                        if (OpenConnection() == true)
                        {
                            MySqlCommand cmd2 = new MySqlCommand(detailsQuery, conn);
                            MySqlDataReader detailsQueryOp = cmd2.ExecuteReader();
                            dt2.Load(detailsQueryOp);
                            detailsQueryOp.Close();
                            CloseConnection();
                        }


                    //--------------FIND ESTIMATE DETAILS FROM DT2(details) ACC TO DT1(master)-----------------
                    foreach (DataRow row in dt1.Rows)
                    {
                        //get datarow aray from dt2 where estimate code is same as in the current dt1 row
                        DataRow[] foundRows = dt2.Select($"estimate_code = '{((string)row["ESTIMATE_CODE"])}'");

                        if (foundRows.Length > 0)
                        {
                            //get the format from dt2 
                            //(new table required because cannot convert datarow to json directly)
                            DataTable cloned = dt2.Clone();

                            foreach (DataRow r in foundRows)
                            {
                                //add the row to the new table
                                //Console.WriteLine("{0}, {1}", r[0], r[1]);
                                cloned.Rows.Add(r.ItemArray);
                            }

                            //copy the new table to details column of the current row
                            row["details"] = cloned;
                        }

                        //if no rows found in details table
                        else
                        {
                            row["details"] = null;
                        }
                    }

                    }

                    //------------SERIALIZE & CREATE RESPONSE--------------------
                    string sJSONResponse = JsonConvert.SerializeObject(dt1);

                    //create response
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(sJSONResponse);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    return response;

                }
                catch (MySqlException ex)
                {
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