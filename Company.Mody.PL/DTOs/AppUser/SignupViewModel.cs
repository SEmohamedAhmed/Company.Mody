using System.ComponentModel.DataAnnotations;

namespace Company.Mody.PL.DTOs.AppUser
{
    public class SignupViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }


        [Required]
        public bool IsAgree { get; set; }

    }

}
