using System.ComponentModel.DataAnnotations;

namespace Company.Mody.PL.DTOs.AppUser
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
