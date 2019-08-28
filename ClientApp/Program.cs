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
        static HttpClient client; //= new HttpClient();
        static string homePath;                        
        static string currentPath;

        static string[] GetServerAnswer()
        {
            HttpResponseMessage resp = client.GetAsync(client.BaseAddress + "api/values").Result;           
            resp.EnsureSuccessStatusCode();
            string[] separator = {"\",\""}; // "," is separator
            string serverAnswer = resp.Content.ReadAsStringAsync().Result;          
            serverAnswer = serverAnswer.Replace("\\\\", "\\");  //replace \\ to \
            serverAnswer = serverAnswer.Substring(2, serverAnswer.Length - 4); //cut first 2 and last 2 chars
            string[] serverAnswerArray = serverAnswer.Split(separator, 9999, StringSplitOptions.RemoveEmptyEntries);
            return serverAnswerArray;
        }       

        static async Task<Uri> PostServerQuestion(string value)
        {           
            HttpResponseMessage response = await client.PostAsJsonAsync(client.BaseAddress + "api/values", value);
            response.EnsureSuccessStatusCode();
            // return URI of the created resource.          
            return response.Headers.Location;
        }

        static void Main()
        {                                           
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Client app is starting...");
            Console.Write("Enter server adress: http://localhost:");

            while (true)
            {
                try
                {
                    client = new HttpClient();
                    client.BaseAddress = new Uri ("http://localhost:" + Console.ReadLine()+"/");                                      
                    HttpResponseMessage resp = client.GetAsync(client.BaseAddress + "api/values").Result;                                    
                    resp.EnsureSuccessStatusCode();                 
                    if (resp.IsSuccessStatusCode == true)
                    {                      
                        break;
                    }                   
                }
                catch (Exception e)
                {                  
                    Console.Write("\nInvalid server adress. Try again:\nhttp://localhost:");                  
                }
            }

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Console.WriteLine("Success.\n");
            Console.ForegroundColor = ConsoleColor.White;          
            homePath = GetServerAnswer()[0];           
            Console.WriteLine(homePath);
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
