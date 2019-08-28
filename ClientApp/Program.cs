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
        static HttpClient client;
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
            Console.Title = "Virtual File System Client App";
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
                catch (Exception)
                {                  
                    Console.Write("\nServer adress is invalid. Try again:\nhttp://localhost:");                  
                }
            }

            while (true)
            {
                try
                {
                    MainAsync().GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                    Console.WriteLine("Server is not responding...\nPress <Enter> to reconnect.");
                    Console.ReadLine();
                }
            }
        }

        static async Task MainAsync()
        {           
            Console.ForegroundColor = ConsoleColor.White;          
            homePath = GetServerAnswer()[0];
            Console.WriteLine("Success.\n");
            Console.WriteLine("Type <help> to list supported commands.");
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
                        Console.Write(
     "\ndir                                         | Showing all directories and files in current directory." +       
     "\ncd  <path>                                  | Move to directory"+
     "\nmkdir  <name>                               | Create new directory in specified location" +
     "\nmkfile  <name.extention> <content>          | Create new file with content in specified location" +
     "\ncopy  <path_to_1st_file> <path_to_2nd_file> | Copying specified file into another location and name." +
     "\nmove  <path_to_1st_file> <path_to_2nd_file> | Moving specified file into another location and name." +
     "\ncls                                         | Clearing command prompt window" +
     "\nexit                                        | Exit client.\n"
                            );

                        break;
                    default:                       
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
