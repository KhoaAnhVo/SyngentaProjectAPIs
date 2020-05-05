using System;
using Newtonsoft.Json;

namespace SyngentaProjectAPIs.Models
{

    public class ResponMsg
    {
        public ResponMsg(DateTime _dateTime)
        {
            DateTime = _dateTime.ToString();
            ExecutionTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        public void UpdateExecutionTime()
        {
            ExecutionTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ExecutionTime;
        }

        public string Status { get; set; } = "OK";
        public string Message { get; set; } = "Successfully created";
        public string Description { get; set; } = "No";
        public string DateTime { get; set; }
        public long ExecutionTime { get; set; }
    }

    public static class FormatBody
    {
        public static string Response(string JsonMessage, string JsonResult)
        {
            return "Message:" + JsonMessage + "," + "Result:" + JsonResult;
        }

        public static string Response(object Message, object Result)
        {
            var M = JsonConvert.SerializeObject(Message);
            var R = JsonConvert.SerializeObject(Result);
            //string ouput = string.Format("{\" ",M,R);

            return "{\"Message\":[" + M + "] ," + "\"Result\":" + R + "}";
            //return  "{\"Result\":" + R + "}";
        }
    }
}
