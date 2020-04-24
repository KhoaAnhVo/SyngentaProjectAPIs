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
using SQLServerSupportLib;
using GenagateQrCodeAPI;
using QrCodeManager;
using System.Data;

namespace GenagateQrCodeAPI.Controllers
{
    
    [Route("APIs/v0/[controller]")]
    [ApiController]
    public class MasterCodesController : ControllerBase
    {
        QrCodeDataBase QrDatabase = new QrCodeDataBase();
        QrCodeControl QrControl = new QrCodeControl();

        #region GET Menthods
        // GET: APIs/v0/MasterCodes ==> List toàn bộ thông tin về những GroupCode đã tạo ra.
        [HttpGet]
        public string Get()
        {
            var Msg = new RespMsg(DateTime.Now);
            DataTable result = new DataTable();
            try
            {
                result = QrDatabase.GetDataToTable($"Select * from GroupCodeInfo;");
                Msg.Message = "Sucessfully excution!";
            }catch(Exception ex)
            {
                Msg.Status = "Not OK";
                Msg.Message = "Some things wrong happed!";
                Msg.Description = ex.Message;
            }
            Msg.UpdateExecutionTime();
            return FormatBody.Response(Msg, result);
        }

        // GET: APIs/v0/MasterCodes/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            var Msg = new RespMsg(DateTime.Now);
            DataTable result = new DataTable();
            try
            {
                result = QrDatabase.GetDataToTable($"Select * from GroupCodeInfo WHERE Id = '{id}';");
                Msg.Message = "Sucessfully excution!";
            }
            catch (Exception ex)
            {
                Msg.Status = "Not OK";
                Msg.Message = "Some things wrong happed!";
                Msg.Description = ex.Message;
            }
            Msg.UpdateExecutionTime();
            return FormatBody.Response(Msg, result);
        }
        #endregion GET Menthods

        #region POST Menthods
        //Nhận lệnh tạo cơ sỡ dữ liệu update csdl GroupCodeInfo và tạo một GroupCode mới dựa vào thời điểm và số lượng
        // POST: APIs/v0/MasterCodes
        [HttpPost]
        public async Task<string> GetPostRequestBody(string request)
        {
            string requestBody, responseBody = "{\" Status \":\" OK \"}";
            var M = new RespMsg(DateTime.Now);
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            //Chuyển dữ liệu json thành trường trong class xác định
            GroupCodeData data = JsonConvert.DeserializeObject<GroupCodeData>(requestBody);
            //tao ten groupcode
            try
            {
                data.GroupName = QrControl.CreateGroupCodeNameNow();
                var groupCode = QrControl.CreateGroupCodeTable(data.GroupName, data.Quantity);
                QrDatabase.UpdateDbGroupCodeInfo(data.GroupName, data.UserCode, data.AGICode, data.Quantity, DateTime.Now.ToString());
                QrDatabase.UpdateDbGroupCode(data.GroupName, groupCode);
            }
            catch(Exception ex)
            {
                M.Status = "ERROR";
                M.Message = "Tạo mã code không thành công!";
                M.Description = ex.Message;
            }
            var result = QrDatabase.GetDataToTable($"Select * from GroupCodeInfo where GroupName LIKE '%{data.GroupName}';");
            responseBody = JsonConvert.SerializeObject(result);
            M.UpdateExecutionTime();
            string Msg= JsonConvert.SerializeObject(M);
            return FormatBody.Response(Msg, responseBody);
        }


        #endregion POST Menthods

        // PUT: APIs/v0/MasterCodes/5
        [HttpPut("{id}")]
        public async Task<string> GetBodyRequestPut(string id)
        {
            string requestBody, responseBody = "{\" Status \":\" OK \"}";
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            

            return responseBody;
        }

        // DELETE: APIs/v0/MasterCodes/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
    }

    //
    public class GroupCodeData
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string UserCode { get; set; } 
        public string Quantity { get; set; }
        public string AGICode { get; set; } 
        public string Note { get; set; }
        public string DataTime { get; set; }
        public class CodeData
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string CodeMD5 { get; set; }
            public string CodeSHA1 { get; set; }
            public string CodeSHA256 { get; set; }
        }
    }
    
    public class RespMsg
    {
        public RespMsg(DateTime _dateTime)
        {
            DateTime = _dateTime.ToString();
        }
        public void UpdateExecutionTime()
        {
            ExecutionTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ExecutionTime;
        }

        public string Status { get; set; } = "OK";
        public string Message { get; set; } = "Successfully created";
        public string Description { get; set; } = "No";
        public string DateTime { get; set; }
        public long ExecutionTime { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public static class FormatBody
    {
        public static string Response(string JsonMessage, string JsonResult)
        {
            return "Message:"+JsonMessage + "," + "Result:"+JsonResult;
        }

        public static string Response(object Message, object Result)
        {
            var M = JsonConvert.SerializeObject(Message);
            var R = JsonConvert.SerializeObject(Result);
            return "Message:" + M + "," + "Result:" + R;
        }
    }


}
