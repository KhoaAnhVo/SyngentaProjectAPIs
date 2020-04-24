using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Text.Unicode;

namespace GenagateQrCodeAPI.Controllers
{
    
    [Route("api/[controller]/v1/")]
    [ApiController]
    public class MasterCodeController : ControllerBase
    {
        public string abc = "123";
        // GET: api/MasterCode
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MasterCode/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return $"value={abc}";
        }

        // POST: api/MasterCode
        [HttpPost]
        public async Task<string> GetPostRequestBody(string request)
        {
            string result;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                result = await reader.ReadToEndAsync();
            }

            return result;
        }

        // PUT: api/MasterCode/5
        [HttpPut("{id}")]
        public async Task<string> GetBodyRequestPut(string id)
        {
            string result;
            using (StreamReader reader = new StreamReader(Request.Body,Encoding.UTF8))
            {
                result = await reader.ReadToEndAsync();
            }

            return result+id;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
    }
}
