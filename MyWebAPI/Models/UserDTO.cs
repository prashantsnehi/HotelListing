using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Models
{
    public class UserDTO : LoginDTO
    {
        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
