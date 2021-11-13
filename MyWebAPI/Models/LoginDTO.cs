using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Models
{
    public class LoginDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Your password is limited to {2} to {1} character", MinimumLength = 5)]
        public string Password { get; set; }
    }
}
