using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace JustEat.RecruitmentTest.App
{
    public class Program
    {
        // Just setting the API Root
        public const string API_ROOT = "http://public.je-apis.com:80/";
        public static HttpClient client = new HttpClient();

        // returns empty string if not valid and outcode otherwise
        public static string ValidatingOutcode(String outcode)
        {
            // simple validation, 2 letters and 2 numbers
            Regex r = new Regex("^[a-zA-Z]{2}[0-9]{2}$");
            return r.IsMatch(outcode) ? outcode : "";
        }

        private static void AddingHeaders(HttpRequestMessage request)
        {

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Authorization", "Basic VGVjaFRlc3RBUEk6dXNlcjI=");
            request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-GB"));
            request.Headers.Add("Accept-Tenant", "uk");
            request.Headers.Host = "public.je-apis.com";
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("-- JustEat restaurant listing by outcode --");

            // Just getting outcode input and validating it
            Console.WriteLine("Just enter your outcode:");
            String outcode = "";
            while (outcode == "")
            {
                outcode = Program.ValidatingOutcode(Console.ReadLine());
                if (outcode == "") Console.WriteLine("Wrong outcode! just try again:");
            }


            // Just creating the request
            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = Program.client.BaseAddress,
                Method = HttpMethod.Get
            };

            // Just adding the headers
            AddingHeaders(request);

            // Just adding parameters
            String requestParameters = String.Format("restaurants?q={0}", outcode);
            Program.client.BaseAddress = new Uri(API_ROOT + requestParameters);

            // Just calling the API, no need to be asynchronous in this system
            HttpResponseMessage call = client.SendAsync(request).GetAwaiter().GetResult();
            if (call.StatusCode == System.Net.HttpStatusCode.OK) {
                string response = call.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                // Just reading from JSON String
                JObject result = (JObject)JsonConvert.DeserializeObject(response);

                // Just ordering
                var restaurants = ((JArray)result.GetValue("Restaurants"))
                                    .OrderByDescending(
                                        restaurant => ((JObject)restaurant).GetValue("RatingAverage"));

                // Just outputing the restaurants with their ratings and types of food in the correct formats
                foreach (JObject restaurant in restaurants)
                {
                    Console.WriteLine("------------");
                    Console.WriteLine(String.Format("Name: {0}", restaurant.GetValue("Name")));
                    Console.WriteLine(String.Format("Rating(avg): {0}", restaurant.GetValue("RatingAverage")));
                    Console.Write("Types of food:");
                    foreach (JObject type in (JArray)restaurant.GetValue("CuisineTypes"))
                    {
                        Console.Write(String.Format(" {0}", type.GetValue("Name")));
                    }
                    Console.WriteLine("");

                }
            } else
            {
                Console.WriteLine("Sorry! The request just failed!");
            }

        }
    }
}
