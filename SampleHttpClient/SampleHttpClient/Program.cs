using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SampleHttpClient
{
    public class Fact
    {
        public string Activity { get; set; }
        public decimal Accessibility { get; set; }
        public string Type { get; set; }
        public string Participants { get; set; }
        public decimal Price { get; set; }
        public int Key { get; set; }
    }
    class Program
    {
        static HttpClient client = new HttpClient();

        static void ShowProduct(Fact fact)
        {
            Console.WriteLine($"Activity name: {fact.Activity}\nAccessibility:{fact.Accessibility}\nType:{fact.Type}\nPrice: {fact.Price}\nParticipant {fact.Participants}");

        }

        static async Task<Fact> GetProductAsync(string path)
        {
            Fact f = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if(response.IsSuccessStatusCode)
            {
                f = await response.Content.ReadAsAsync<Fact>();
            }

            return f;
        }

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("https://www.boredapi.com/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                Fact ff = new Fact();
                Uri url = new Uri(client.BaseAddress,"activity/");
                ff = await GetProductAsync(url.PathAndQuery);
                ShowProduct(ff);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}


/* 
 * https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
 */
