using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aithics.data.Models
{
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        public long UserRoleId { get; set; }    
        public long UserId { get; set; }
        public long RoleId { get; set; }

       
    }
}
