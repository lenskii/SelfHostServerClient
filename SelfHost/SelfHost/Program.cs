using Microsoft.Owin.Hosting;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace SelfHost
{
    public class Program
    {
        public static String homePath = Directory.GetCurrentDirectory() + "\\UserFiles";
        static HttpClient client = new HttpClient();
        public static string baseAddress;// = "http://localhost:9000/";
        static int basePort;
        
        static void Main()
        {
            Console.Write("Server adress: http://localhost:");

            while (true)
            {
                try
                {
                    basePort = Int32.Parse(Console.ReadLine());
                    if (basePort < 1000)
                    {
                        Console.Write("Port must be 4-digit at least. Try again:\nhttp://localhost:");
                        
                        continue;
                    }

                    break;
                }
                catch (FormatException)
                {                   
                    Console.Write("\nInvalid server adress. Try again:\nhttp://localhost:");
                }
            }

            Console.WriteLine(basePort.ToString());
            baseAddress = "http://localhost:" + basePort.ToString() + "/";
            Console.WriteLine(baseAddress);
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {           
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