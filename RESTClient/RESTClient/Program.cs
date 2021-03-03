﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Data;

//add nuget package
//Install-Package Microsoft.AspNet.WebApi.Client

namespace RESTClient
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static string localDate = "2021-01-28 13:00:00";
        static void showProduct(DBclass record)
        {
            Console.WriteLine($"Log-id: {record.log_id}\n" +
                $"pk:{record.c1_pk}\n" +
                $"name:{record.c2_name}\n" +
                $"amount:{record.c2_name}\n" +
                $"Last updates at:{record.c4_updated_at}" +
                $"operation:{record.Operation}\n");

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
                var responseTask = await client.GetAsync($"https://localhost:44366/api/Date?dateString={localDate}");

                if (responseTask.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success");

                    DataTable dt = new DataTable();

                    //var readTask = await responseTask.Content.ReadAsAsync<DataTable>();
                    var readTask = await responseTask.Content.ReadAsAsync<List<EstimateClass>>();
                    //Console.WriteLine(readTask.Rows.Count);
                    Console.WriteLine(readTask);
                    //print_results(readTask);

                    //delete entries from tables
                    //create array of pks to delete

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
    }
}



/* 
 * https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
 */

