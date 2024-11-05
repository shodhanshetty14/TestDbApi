using System.ComponentModel.DataAnnotations;

namespace TestProjectDB.api.Models
{
    public class RegistrationDto
    {
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        [MaxLength(20), MinLength(3)]
        public required string Password { get; set; }
        [MaxLength(20), MinLength(3)]
        public required string ConfirmPassword { get; set; }
    }
}
