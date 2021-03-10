using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Data;
using Z.BulkOperations;
using MySql.Data.MySqlClient;
using System.Reflection;
using Newtonsoft.Json.Linq;

//add nuget package
//Install-Package Microsoft.AspNet.WebApi.Client

namespace RESTClient
{
    class Program
    {
        static string localDate = "20210202160000"; //"2021-01-28 13:00:00";
        static string locstring = "ALL";

        static HttpClient client = new HttpClient();
        
        private static readonly string connectionString = "server=127.0.0.1;port=3306;uid=root;pwd=root";
        private static MySqlConnection conn = new MySqlConnection(connectionString);

        
        static void insertDataTable(DataTable dt, string tableName)
        {
            try
            {
                if(OpenConnection()==true)
                {
                    using (BulkOperation bulk = new BulkOperation(conn))
                    {
                
                        bulk.DestinationTableName = tableName;
                        //bulk.BulkDelete(dt);
                        //bulk.BulkInsert(dt);
                        bulk.BulkMerge(dt);
                    }
                    CloseConnection();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ex raised at BulkInsert: " + ex.Message);
            }           

        }

        //print function taken from SO
        static void print_results(DataTable data)
        {
            Console.WriteLine();
            Dictionary<string, int> colWidths = new Dictionary<string, int>();

            foreach (DataColumn col in data.Columns)
            {
                Console.Write(col.ColumnName);
                var maxLabelSize = data.Rows.OfType<DataRow>()
                        .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                        .OrderByDescending(m => m).FirstOrDefault();

                colWidths.Add(col.ColumnName, maxLabelSize);
                for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 10; i++) Console.Write(" ");
            }

            Console.WriteLine();

            foreach (DataRow dataRow in data.Rows)
            {
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    Console.Write(dataRow.ItemArray[j]);
                    for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 10; i++) Console.Write(" ");
                }
                Console.WriteLine();
            }
        }


        static async Task Main(string[] args)
        {

        //client.BaseAddress= new Uri("https://localhost:44366/api/");
        //client.DefaultRequestHeaders.Accept.Clear();
        //Uri url = new Uri(client.BaseAddress, $"Date?dateString={localDate}");
        
            try
            {
                var responseTask = await client.GetAsync($"https://localhost:44366/api/Date?timestamp={localDate}");//&loc={locstring}");

                if (responseTask.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success");

                    string s = await responseTask.Content.ReadAsStringAsync();

                    JArray jArray = JArray.Parse(s);

                    var jsonArray = new JArray();
                    JArray newDetailsJarray = new JArray();
                    foreach (JObject row in jArray)
                    {
                        var newRow = new JObject();
                        foreach (var column in row.Properties())
                        {
                            // include values column wise
                            if (column.Name != "details")
                                newRow.Add(column.Name, column.Value);

                            else if (column.Name == "details" && column.Value != null)
                            {
                                foreach (JObject detailsRow in column.Value)
                                {
                                    var newDetailsRow = new JObject();
                                    foreach (var detailsCol in detailsRow.Properties())
                                    {
                                        newDetailsRow.Add(detailsCol.Name, detailsCol.Value);
                                    }
                                    newDetailsJarray.Add(newDetailsRow);
                                }
                            }
                        }
                        jsonArray.Add(newRow);
                    }

                    DataTable dt_master = JsonConvert.DeserializeObject<DataTable>(jsonArray.ToString());
                    DataTable dt_details = JsonConvert.DeserializeObject<DataTable>(newDetailsJarray.ToString());


                    /*
                    string est_codes = "";
                    foreach (DataRow row in dtResponse1.Rows)
                    {
                        est_codes= est_codes+ "&pk=" +((string)row["ESTIMATE_CODE"]);
                        //Console.WriteLine((string)row["ESTIMATE_CODE"]);
                    }

                    est_codes = est_codes.Remove(0,1);
                    Console.WriteLine(est_codes);

                    /*
                        List<string> estimate_codes = new List<string>(readTask.Rows.Count);
                        foreach (DataRow row in readTask.Rows)
                            estimate_codes.Add((string)row["ESTIMATE_CODE"]);

                        Console.WriteLine(estimate_codes[7]);

                        /* //Post request attempt

                        try
                        {
                            var responsePost = await client.PostAsJsonAsync("https://localhost:44366/api/Date", estimate_codes).ConfigureAwait(false);

                            if(responsePost.IsSuccessStatusCode)
                            {
                                var resultPost = await responsePost.Content.ReadAsAsync<DataTable>();
                                Console.WriteLine(resultPost);
                            }

                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    */
                    /*
                    try
                    {
                        var responseTask2 = await client.GetAsync($"https://localhost:44366/api/Date?{est_codes}");
                        int count = 0;
                        if (responseTask.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Success");

                            var dtResponse2 = await responseTask2.Content.ReadAsAsync<DataTable>();

                            foreach (DataRow row in dtResponse2.Rows)
                            {
                                count++;
                            }

                            dtResponse1.Columns.Add("details", typeof(DataTable));

                            foreach (DataRow row in dtResponse1.Rows)
                            {
                                //DataRow foundRow = dt2.Rows.Find(row["ESTIMATE_CODE"]);
                                Console.WriteLine((string)row["ESTIMATE_CODE"]);
                                DataRow[] foundRows = dtResponse2.Select($"estimate_code = '{((string)row["ESTIMATE_CODE"])}'");

                                string jsonFoundRows = "";

                                if (foundRows.Length > 0)
                                {
                                    DataTable cloned = dtResponse2.Clone();
                                    count = 0;
                                    foreach (DataRow r in foundRows)
                                    {
                                        count++;
                                        //Console.WriteLine("{0}, {1}", r[0], r[1]);
                                        cloned.Rows.Add(r.ItemArray);
                                    }
                                    //Console.WriteLine(count);
                                    
                                    //convert rows to json
                                    jsonFoundRows = JsonConvert.SerializeObject(cloned);

                                    //Console.WriteLine(jsonFoundRows);
                                    //Console.WriteLine(cloned.Rows.Count);
                                    row["details"] =cloned;
                                }
                                

                               
                            }

                            string j = JsonConvert.SerializeObject(dtResponse1);

                            Console.WriteLine(j);
                    
                    //Console.WriteLine("db2 count= {0}", count);
                    //insertDataTable(dtResponse1, "schema1.local_estimate_master");
                    //Console.WriteLine("Table 1 copied to db");
                    //insertDataTable(dtResponse2, "schema1.local_estimate_item_details");
                    //Console.WriteLine("Table 2 copied to db");

                    
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    */
                    // string j = JsonConvert.SerializeObject(dtResponse1);

                    //                    Console.WriteLine(j);

                    insertDataTable(dt_master, "schema1.local_estimate_master");
                    Console.WriteLine("Table 1 copied to db");
                    insertDataTable(dt_details, "schema1.local_estimate_item_details");
                    Console.WriteLine("Table 2 copied to db");

                }

                else
                {
                    Console.WriteLine("Request not successful");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        
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
 * https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
 */

