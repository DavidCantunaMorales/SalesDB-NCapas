using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NSalesMVCPLS.Models
{
    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*(),.?\:{}|<>]).*$", 
            ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
        public string Password { get; set; }
}
}