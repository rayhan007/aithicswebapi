using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aithics.data.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public long RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;// Default value
        public string? NormalizedName { get; set; }
        public string? Description { get; set; }
    }
}
