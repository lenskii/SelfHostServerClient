using Microsoft.Owin.Hosting;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SelfHost
{
    public class Program
    {
        public static String homePath = Directory.GetCurrentDirectory() + "\\UserFiles";
        static HttpClient client = new HttpClient();
        public static string baseAddress = "http://localhost:9000/";

        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {            
            //Console.WriteLine("Server adress:");
            //string baseAddress = Console.ReadLine();
            //TODO: Exceptions with for loop

            //Or just keep this adress
                                  
            using (WebApp.Start<Startup>(url: baseAddress))
             {                                                            
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\UserFiles");
                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\UserFiles");
                // For displaying local path in client app:
                await Commands.CreateValueAsync(Directory.GetCurrentDirectory());

                Console.Clear();
                Console.WriteLine("Server started.");
                Console.WriteLine("Server adress: "+ baseAddress);
                Console.ReadLine();
            }         
        }
    }
}