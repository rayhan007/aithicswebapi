using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aithics.data.Models
{
    [Table("RoleToAPI")]
    public class RoleToAPI
    {
        [Key]
        public long RoleToAPIId { get; set; }
        public long RoleId { get; set; }
        public long APIListingId { get; set; }

       
    }
}
