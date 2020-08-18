using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestServiceProject.Models
{
    public class User : ITrackable
    {
        [Key]
        public Guid Id { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string  Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public byte[] Password { get; set; }
        [DataType(DataType.Password)]
        public byte[] PasswordSalt { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
