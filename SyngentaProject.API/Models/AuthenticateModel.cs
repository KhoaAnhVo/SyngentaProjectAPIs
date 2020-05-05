 
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SyngentaProjectAPIs.Models
{
    public class AuthenticateModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}