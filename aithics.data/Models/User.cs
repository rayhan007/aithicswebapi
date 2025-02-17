using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aithics.data.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public long UserId { get; set; }

        public string? FullName { get; set; }
        public string? UserName { get; set; } 
        public string? Email { get; set; } 
        public string? PasswordHash { get; set; }
        public string? RefreshToken { get; set; } 
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
