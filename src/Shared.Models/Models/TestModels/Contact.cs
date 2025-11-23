using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Models.TestModels
{
    public class Contact
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
