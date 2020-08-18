using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestServiceProject.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        // Only displaying this for demonstration purposes
        // The hashed password
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
