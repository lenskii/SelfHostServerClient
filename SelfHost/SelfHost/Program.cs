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
        public static string baseAddress;
        static int basePort;
        
        static void Main()
        {
            Console.Title = "Virtual File System Server App";
            Console.Write("Input server adress:\nhttp://localhost:");
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
                Console.WriteLine("Type <exit> to close the server.");
                Console.WriteLine("User inputs listed below:");
                while (true)
                {
                    if (Console.ReadLine() == "exit")
                    break;
                }
               
            }         
        }
    }
}