using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System;
using System.IO;

namespace SelfHost
{
    public class ValuesController : ApiController
    {
        public static List<string> valuesList = new List<string>();
        // GET api/values 
        public IEnumerable<string> Get()
        {           
                return valuesList;            
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "values";
        }

        // POST api/values 
        [HttpPost]
        public HttpResponseMessage Post([FromBody] string value)
        {
            Console.WriteLine("\nClient input: " + value);
            valuesList.Clear();  //clearing list every new input            
            Commands.CommandListener(value);
            valuesList.Insert(0, Directory.GetCurrentDirectory()); //for displaying local path
            var response = Request.CreateResponse(HttpStatusCode.Created, valuesList);            
            return response;         
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}
