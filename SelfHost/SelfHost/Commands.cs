using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SelfHost
{
    class Commands
    {
        static HttpClient client = new HttpClient();
        static Dictionary<string, Action<IEnumerable<string>>> commands = new Dictionary<string, Action<IEnumerable<string>>>()
{
    { "dir"    ,  MakeSimpleCommand(() => dir()) },
    { "mkdir"  ,  mkdir  },
    { "cd"     ,  cd     },
    { "mkfile" ,  mkfile },
    { "copy"   ,  copy   },
    { "move"   ,  move   } 
};

        public static void CommandListener(string input)
        {
                var tokens = SplitIntoTokens(input);
                var command = tokens.FirstOrDefault();

            if (command == null)
                return;
                if (commands.ContainsKey(command))
                {
                    try
                    {
                        commands[command](tokens.Skip(1));
                    }
                    catch (Exception e)
                    {
                    Console.WriteLine("Execution failed: " + e.Message);
                    ValuesController.valuesList.Add("Execution failed: " + e.Message);                                                       
                    }                                
                }

                else
                {
                Console.WriteLine("Unrecognized command: " + input);
                ValuesController.valuesList.Add("Unrecognized command: " + input);
                }               
            return;
        }

        static Action<IEnumerable<string>> MakeSimpleCommand(Action a)
        {
            return args =>
            {
                if (args.Any())
                    throw new ArgumentException("this command doesn't support args");
                    ValuesController.valuesList.Add("this command doesn't support args");
                a();
            };

        }

        static IEnumerable<string> SplitIntoTokens(string s)
        {
            return s.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
        }

        static void cd(IEnumerable<string> obj)
        {
            string[] destinationDirectory = obj.ToArray();
            switch (destinationDirectory[0])
            {
                case "..":
                    if (Directory.GetCurrentDirectory() == Program.homePath)
                    {
                        break;
                    }
                    Directory.SetCurrentDirectory(Path.GetDirectoryName(Directory.GetCurrentDirectory()));

                    break;
                default:
                    Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\" + destinationDirectory[0].Replace("C:", ""));
                    break;
            }

        }

        static void mkdir(IEnumerable<string> obj)
        {
            string[] newDirectoryProperties = obj.ToArray();
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\" + newDirectoryProperties[0]);
        }

        static void mkfile (IEnumerable<string> obj)
        {
            string[] newFileProperties = obj.ToArray();
            string fullFileName = Directory.GetCurrentDirectory() + "\\" + newFileProperties[0];           
            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fullFileName))
                {
                    File.Delete(fullFileName);
                }
                // Create a new file     
                using (FileStream fs = File.Create(fullFileName))
                {                              
                    // Add some text to file   
                    string newFileText = string.Join(" ", newFileProperties.Skip(1));
                    Byte[] newFileTextByte = new UTF8Encoding(true).GetBytes(newFileText);
                   
                    fs.Write(newFileTextByte, 0, newFileText.Length);
                }
                // Open the stream and read it back. 
                Console.WriteLine("File " + newFileProperties[0] + " created successfully.\nFile content:\n");
                ValuesController.valuesList.Add("File " + newFileProperties[0] + " created successfully.");
                ValuesController.valuesList.Add("File contents:");
                using (StreamReader sr = File.OpenText(fullFileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        ValuesController.valuesList.Add(s);
                        Console.WriteLine(s);
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
                ValuesController.valuesList.Add(Ex.ToString());
            }
        }

        static void move(IEnumerable<string> obj)
        {
            string[] moveFileProperties = obj.ToArray();
            string moveFullFileNameFrom = Program.homePath + moveFileProperties[0].Replace("C:", "");
            string moveFullFileNameTo   = Program.homePath + moveFileProperties[1].Replace("C:", "");
            
            try
            {
                File.Move(@moveFullFileNameFrom, @moveFullFileNameTo);
                ValuesController.valuesList.Add("File " + moveFileProperties[0] + " moved to " + moveFileProperties[1] + " successfully.");
            }
            catch (Exception)
            {
                Console.WriteLine("FIle " + moveFileProperties[0] + " doesn't exist");
                ValuesController.valuesList.Add("FIle " + moveFileProperties[0] + " doesn't exist");
            }
            }

        static void copy(IEnumerable<string> obj)
        {
            string[] copyFileProperties = obj.ToArray();
            string copyFullFileNameFrom = Program.homePath + copyFileProperties[0].Replace("C:", "");
            string copyFullFileNameTo   = Program.homePath + copyFileProperties[1].Replace("C:", "");
            
            try {
                File.Copy(copyFullFileNameFrom, copyFullFileNameTo);
                ValuesController.valuesList.Add("File " + copyFileProperties[0] + " copied to " + copyFileProperties[1] + " successfully.");
            }
            catch (Exception){              
                Console.WriteLine("FIle " + copyFileProperties[0] + " doesn't exist");
                ValuesController.valuesList.Add("FIle " + copyFileProperties[0] + " doesn't exist");
            }         
        }

        static void dir()
        {
            
            //Delete unused exception info - костыль :С
            ValuesController.valuesList.RemoveAt(0);
           //List of folders in current folder           
            string[] currentFolders = Directory.GetDirectories(Directory.GetCurrentDirectory());           
            //List of files in current folder
            string[] currentFiles = Directory.GetFiles(Directory.GetCurrentDirectory());
            foreach (string element in currentFolders)
            {
                Console.WriteLine("C:{0}", element.Replace(Program.homePath, ""));
                ValuesController.valuesList.Add("C:" + element.Replace(Program.homePath, ""));
            }
            Console.WriteLine();
            foreach (string element in currentFiles)
            {
                Console.WriteLine("C:{0}", element.Replace(Program.homePath, ""));
                ValuesController.valuesList.Add("C:" + element.Replace(Program.homePath, ""));
            }
        }

        public static async Task<Uri> CreateValueAsync(string value)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(Program.baseAddress + "api/values", value);
            response.EnsureSuccessStatusCode();
            // return URI of the created resource.
            return response.Headers.Location;
        }
    }
}
