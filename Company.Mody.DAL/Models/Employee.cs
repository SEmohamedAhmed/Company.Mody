using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Mody.DAL.Models
{
    public class Employee: BaseEntity
    {
        public int Id { get; set; }
        [Range(20,60,ErrorMessage = "!!!Age Must be from 20 to 60!!!")]
        public int? Age { get; set; }
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "!!!Salary Is Required!!!")]
        public decimal Salary { get; set; }
        [Required(ErrorMessage ="!!!Name Is Required!!!")]
        public string Name { get; set; }
        [RegularExpression(@"^[0-9]{1,3}-[a-zA-Z]{5,10}-[a-zA-Z]{4,10}-[a-zA-Z]{5,10}$", ErrorMessage ="Must be like: 123-Street-City-Country")]
        public string Address { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime HiringDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
