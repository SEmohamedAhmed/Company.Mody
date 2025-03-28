using System.ComponentModel.DataAnnotations;

namespace Company.Mody.PL.DTOs.AppUser
{
    public class SigninViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RemeberMe { get; set; }
    }

}
