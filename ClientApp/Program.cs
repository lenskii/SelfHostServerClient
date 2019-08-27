using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace Client
{
    class Program
    {        
        static HttpClient client = new HttpClient();
        static String homePath;                        
        static String currentPath;

        static string[] GetServerAnswer()
        {
            HttpResponseMessage resp = client.GetAsync(client.BaseAddress + "api/values").Result;       
            resp.EnsureSuccessStatusCode();
            String[] separator = {"\",\""}; // "," is separator
            String serverAnswer = resp.Content.ReadAsStringAsync().Result;
            serverAnswer = serverAnswer.Replace("\\\\", "\\");  //replace \\ to \
            serverAnswer = serverAnswer.Substring(2, serverAnswer.Length - 4); //cut first 2 and last 2 chars
            string[] serverAnswerArray = serverAnswer.Split(separator, 9999, StringSplitOptions.RemoveEmptyEntries);              
            return serverAnswerArray;
        }       

        static async Task<Uri> PostServerQuestion(string value)
        {

            HttpResponseMessage response = await client.PostAsJsonAsync(client.BaseAddress +"api/values", value);
            response.EnsureSuccessStatusCode();
            // return URI of the created resource.
            return response.Headers.Location;
        }

        static void Main()
        {
            //wait for server start - only for debug
            Thread.Sleep(3000);
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Client app is starting...");
            Console.Write("Server adress: ");

            //TODO: Exceptions with for loop
            //string baseAddress = Console.ReadLine();
            //Or just keep this adress

            string baseAddress = "http://localhost:9000/";
            Console.WriteLine(baseAddress);
            client.BaseAddress = new Uri(baseAddress);

            Console.WriteLine("Client started.\n");
            Console.ForegroundColor = ConsoleColor.White;

            homePath = GetServerAnswer()[0];
            currentPath = homePath; //for 1st time           

            while (true)
            {
                Console.Write("C:" + currentPath.Replace(homePath, "") + ">");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "cls": 
                        Console.Clear();
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    case "help":
                        //TODO: add help
                        Console.Write("Help:\n");
                        break;
                    default:
                        // TODO: server not responding exception

                        Console.WriteLine();
                        await PostServerQuestion(input);
                        currentPath = GetServerAnswer()[0];
                        // Skip(1) - full server output, without current path string
                        foreach (string element in GetServerAnswer().Skip(1))
                        {
                            Console.WriteLine(element);
                        }
                        Console.WriteLine();
                        break;
                }
            }                
        }
    }
}
