using System.ComponentModel.DataAnnotations;

namespace TestProjectDB.api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
