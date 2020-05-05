

namespace SyngentaProjectAPIs.Models
{
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
}
