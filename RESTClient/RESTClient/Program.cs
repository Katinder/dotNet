using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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

                    var readTask = await responseTask.Content.ReadAsAsync<List<DBclass>>();
                    Console.WriteLine(readTask);

                     foreach(var record in readTask)
                     {
                        showProduct(record);
                     }
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

