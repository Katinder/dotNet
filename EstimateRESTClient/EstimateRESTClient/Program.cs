﻿using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Z.BulkOperations;

//add reference for Mysql.Data
//add NuGet package Z.BulkOperations
//add NuGet package Newtonsoft.Json

namespace EstimateRESTClient
{
    class Program
    {
        static string localDate = "20210202160000"; 
        static string locstring = "ALL";

        static HttpClient client = new HttpClient();

        private static readonly string connectionString = "server=127.0.0.1;port=3306;uid=root;pwd=root";
        private static MySqlConnection conn = new MySqlConnection(connectionString);

        static async Task Main(string[] args)
        {
            try
            {
                //-----------------GET LATEST TIMESTAMP FROM TABLE----------
                string timestamp = getLastUpdatedTime("schema1.local_last_date");
                //convert to the right format
                timestamp=DateTime.ParseExact(timestamp, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                Console.WriteLine(timestamp);

                //-----------------CALL API & GET RESPONSE-------------------------
                //get response form GET method
                var responseTask = await client.GetAsync($"https://localhost:44385/api/Date?timestamp={timestamp}&loc={locstring}");

                //if reponse is OK
                if (responseTask.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success");

                    //get the content string of the response
                    string s = await responseTask.Content.ReadAsStringAsync();

                    //parse the string into a JArray object
                    JArray jArray = JArray.Parse(s);


                    //------------READ JSON INTO TWO SEPARATE DATATABLES--------------
                    
                    JArray newMasterJarray = new JArray();
                    JArray newDetailsJarray = new JArray();

                    foreach (JObject masterRow in jArray) //iterate over each row
                    {
                        var newMasterRow = new JObject();
                        foreach (var masterCol in masterRow.Properties()) //iterate over cols in the row
                        {
                            // include values column wise

                            //if not "details" col, then add (key,value) to the master row
                            if (masterCol.Name != "details")
                                newMasterRow.Add(masterCol.Name, masterCol.Value);

                            //if "details" col, then add all rows to details JArray
                            else if (masterCol.Name == "details" && masterCol.Value != null)
                            {
                                foreach (JObject detailsRow in masterCol.Value)
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
                        newMasterJarray.Add(newMasterRow);
                    }

                    //deserialise JArrays to Data Tables
                    DataTable dt_master = JsonConvert.DeserializeObject<DataTable>(newMasterJarray.ToString());
                    DataTable dt_details = JsonConvert.DeserializeObject<DataTable>(newDetailsJarray.ToString());

                    //print datatables for verification
                    //print_results(dt_master);
                    //print_results(dt_details);

                    //get latest date from master datatable
                    var last_DT_UPDATED = (dt_master.Select("DT_UPDATED=MAX(DT_UPDATED)").First())["DT_UPDATED"].ToString();
                    last_DT_UPDATED = DateTime.ParseExact(last_DT_UPDATED, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    Console.WriteLine(last_DT_UPDATED);

                    //insert datatbles to database tables
                    insertDataTable(dt_master, "schema1.local_estimate_master");
                    Console.WriteLine("Table 1 copied to db");
                    insertDataTable(dt_details, "schema1.local_estimate_item_details");
                    Console.WriteLine("Table 2 copied to db");

                    //insert date to date table
                    insertDateDb(last_DT_UPDATED, "schema1.local_last_date");
                }

                else
                {
                    Console.WriteLine("Request not successful");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();

        }

        static void insertDataTable(DataTable dt, string tableName)
        {
            try
            {
                if (OpenConnection() == true)
                {
                    using (BulkOperation bulk = new BulkOperation(conn))
                    {

                        bulk.DestinationTableName = tableName;
                        bulk.BulkMerge(dt);
                    }
                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex raised at insertDataTable function: " + ex.Message);
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
                return false;
            }
        }



        private static string getLastUpdatedTime(string tableName)
        {

            string dateQuery = $"SELECT LAST_DT_UPDATED FROM {tableName};";

            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(dateQuery, conn);
                try
                {
                    string queryOp = cmd.ExecuteScalar().ToString();
                    
                    CloseConnection();
                    return queryOp;

                }
                catch (MySqlException ex)
                {
                    CloseConnection();
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            else
            {
                CloseConnection();
                return null;
            }
        }


        private static void insertDateDb(string lastDateUpdated, string tableName)
        {
            //delete all rows
            string delete_query = $"DELETE FROM {tableName};";

            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(delete_query, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    //Console.WriteLine("Row Deleted.");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                CloseConnection();
            }

            //insert the row
            string insert_query = $"INSERT INTO {tableName} (`LAST_DT_UPDATED`) VALUES ('{lastDateUpdated}');"; //'2021-01-28 12:59:52'

            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(insert_query, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    //Console.WriteLine("New Row Inserted.");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                CloseConnection();
            }
        }
    }
}