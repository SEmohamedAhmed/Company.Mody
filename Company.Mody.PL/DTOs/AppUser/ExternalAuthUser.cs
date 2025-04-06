using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Company.Mody.PL.DTOs.AppUser
{
    public class ExternalAuthUser
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
        public bool IsAgree { get; set; }

        public string Provider { get; set; }
    }
}
