using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Company.Mody.PL.DTOs.AppUser
{
    public class ResetPasswordViewModel
    {

        [Required]
        [FromForm]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [FromForm]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

    }
}
