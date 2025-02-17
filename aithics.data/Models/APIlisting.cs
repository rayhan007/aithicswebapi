using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aithics.data.Models
{
    [Table("APIListing")]
    public class APIListing
    {
        [Key]
        public int APIListingId { get; set; }

        public string ApiEndpoint { get; set; } = string.Empty;
    }
}
