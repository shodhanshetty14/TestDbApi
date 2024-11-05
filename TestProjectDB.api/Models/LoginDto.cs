using System.ComponentModel.DataAnnotations;

namespace TestProjectDB.api.Models
{
    public class LoginDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string password { get; set; }
    }
}
