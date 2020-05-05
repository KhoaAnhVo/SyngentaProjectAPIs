using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using System.Text;
using SQLServerSupportLib;
using GenagateQrCodeAPI;
using QrCodeManager;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using SyngentaProjectAPIs.Models;
using SyngentaProjectAPIs.Services;
using System.Linq;

namespace GenagateQrCodeAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("APIs/v0/[controller]")]
    public class MasterCodesController : ControllerBase
    {
        QrCodeDataBase QrDatabase = new QrCodeDataBase("Data Source=localhost;Initial Catalog=Syngenta_test;User ID=admin;Password=admin");
        
        QrCodeControl QrControl = new QrCodeControl();


        #region AUTHORIZE

        private IUserService _userService;

        public MasterCodesController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.UserName, model.Password);
            if (user == null)
                return BadRequest(new { message = "User or password is incorrect" });
            return Ok(user);
        }
        #endregion AUTHORIZE

        #region GET Menthods
        // GET: APIs/v0/MasterCodes ==> List toàn bộ thông tin về những GroupCode đã tạo ra.
        [HttpGet]
        public string Get()
        {
            var Msg = new ResponMsg(DateTime.Now);
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
            //return JsonConvert.SerializeObject(result);
            return FormatBody.Response(Msg, result);
        }

        // GET: APIs/v0/MasterCodes/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            var Msg = new ResponMsg(DateTime.Now);
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

        [HttpGet("GC/{id}")]
        public string GetGC(int id)
        {
            var Msg = new ResponMsg(DateTime.Now);
            DataTable result = new DataTable();
            try
            {
                var groupName = QrDatabase.GetValue($"SELECT GroupName FROM GroupCodeInfo WHERE Id = '{id}';");
                result = QrDatabase.GetDataToTable($"Select * from {groupName};");
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
            var M = new ResponMsg(DateTime.Now);
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            //Chuyển dữ liệu json thành trường trong class xác định.
            GroupCodeData data = JsonConvert.DeserializeObject<GroupCodeData>(requestBody);
            //tao ten groupcode.
            try
            {
                data.GroupName = QrControl.CreateGroupCodeNameNow();
                var groupCode = QrControl.CreateGroupCodeTable(data.GroupName, data.Quantity);
                QrDatabase.UpdateDbGroupCodeInfo(data.GroupName, data.UserCode, data.AGICode, data.Quantity, DateTime.Now.ToString());
                //QrDatabase.UpdateDbGroupCode(data.GroupName, groupCode);
                QrDatabase.UpdateDbGroupCode("GroupCodeData", groupCode);
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
        public string  Delete(int id)
        {
            var Msg = new ResponMsg(DateTime.Now);
            DataTable result = new DataTable();
            try
            {
                var groupName = QrDatabase.GetValue($"SELECT GroupName FROM GroupCodeInfo WHERE Id = '{id}';");
                QrDatabase.ExcuteQuery($"DELETE FROM GroupCodeInfo WHERE Id = '{id}';");
                QrDatabase.ExcuteQuery($"DROP TABLE {groupName}");
                result = QrDatabase.GetDataToTable($"SELECT * FROM GroupCodeInfo WHERE Id = '{id}';");
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


        
    }

    /*
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
            return "Message:"+JsonMessage + "," + "Result:"+JsonResult;
        }

        public static string Response(object Message, object Result)
        {
            var M = JsonConvert.SerializeObject(Message);
            var R = JsonConvert.SerializeObject(Result);
            //string ouput = string.Format("{\" ",M,R);

            return  "{\"Message\":[" + M + "] ," + "\"Result\":" + R+"}";
            //return  "{\"Result\":" + R + "}";
        }
    }*/

    
}
